using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Vision.Detection;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows;
using AForge.Video.DirectShow;
using AForge.Video;
using System.Threading;
using AForge.Vision.Motion;
using Accord.Controls;
using Accord;
using AForge;
//using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
//using System.Windows.Shapes;
using DlibDotNet;
using DlibDotNet.Extensions;
using DlibDotNet.Dnn;
using Dlib = DlibDotNet.Dlib;
using Rectangle = DlibDotNet.Rectangle;
using Emgu.CV;
//using Accord.Imaging;

namespace LFM_CAM_FACE
{
    class Detectorderosto
    {
        private HaarObjectDetector detector = null;
        //   private HaarCascade _cascade = new Accord.Vision.Detection.Cascades.NoseHaarCascade();
        private HaarCascade _cascade = new Accord.Vision.Detection.Cascades.FaceHaarCascade();
        private HaarCascade cascade = HaarCascade.FromXml(Directory.GetCurrentDirectory() + @"\haarcascade_frontalface_alt_tree.xml");
        //  private ObjectDetectorScalingMode _scaleMode = ObjectDetectorScalingMode.SmallerToGreater; 
        float scaleFactor = 1.3f;
        int minSize = 70;
        private ObjectDetectorScalingMode _scaleMode = ObjectDetectorScalingMode.GreaterToSmaller;
        private static Detectorderosto _instance;
        public static Detectorderosto Instance()
        {
            if (_instance == null)
            {
                _instance = new Detectorderosto();
            }
            return _instance;
        }
        public static Detectorderosto Dispose()
        {
            if (_instance != null)
            {

                _instance = null;
            }
            return _instance;
        }

        private Detectorderosto()
        {

            detector = new HaarObjectDetector(cascade, minSize,
                ObjectDetectorSearchMode.NoOverlap, scaleFactor, _scaleMode);

            detector.UseParallelProcessing = true;
            //  cameras.Instance().Rostomudou += Detectorderosto_Rostomudou;
            cameras.Instance().Rostomudou += Detectorderosto_Rostomudoua;
        }

        private void Detectorderosto_Rostomudoua(object sender, EventArgs e)
        {

           

            Bitmap bmp = (Bitmap)sender;
            Bitmap bmp1 = new Bitmap(bmp);

            using ( var img = bmp1.ToArray2D<RgbPixel>())

          //  using (var img = bitmapp.ToArray2D(bmp1))

            //  BitmapData data = bmp1.LockBits(new System.Drawing.Rectangle(0, 0, bmp1.Width, bmp1.Height),
            //  System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp1.PixelFormat);
            //     byte[] array = new byte[data.Stride * data.Height];
            //     Marshal.Copy(data.Scan0, array, 0, array.Length);
            //  using (Array2D<BgrPixel> img = Dlib.LoadImageData<BgrPixel>(array, (uint)bmp1.Height, (uint)bmp1.Width, (uint)data.Stride))
            {
                using (FrontalFaceDetector fd = Dlib.GetFrontalFaceDetector())

                {
                    //    Array2D<RgbPixel> img = Dlib.LoadImage<RgbPixel>(imagePath);

                    // find all faces in the image
                    var faces = fd.Operator(img);
                    foreach (var face in faces)
                    {
                        Dlib.DrawRectangle(img, face, color: new RgbPixel(0, 255, 255), thickness: 4);
                        System.Windows.Forms.MessageBox.Show("ok");  
                        Dlib.SaveJpeg(img, AppDomain.CurrentDomain.BaseDirectory + "outpuht.jpg");
                        // draw a rectangle for each face
                        //     Dlib.DrawRectangle(img, face, color: new RgbPixel(0, 255, 255));
                    }
                    //    bmp1 = bitmapp.Array2DToBitmap(img);
                  
                    //   MessageBox.Show("ok");
                }
            }
           


            //Array2D < RgbPixel > cimg = Dlib.LoadImageData<RgbPixel>(array, (uint)frame.Height, (uint)frame.Width, (uint)(frame.Width * frame.ElemSize()));



        }

        private void Detectorderosto_Rostomudou(object sender, EventArgs e)
        {
            Bitmap bmp = (Bitmap)sender;
            System.Drawing.Rectangle[] rectangles = detector.ProcessFrame(bmp);

            if (rectangles.Length == 0) return;

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawRectangles(new System.Drawing.Pen(new SolidBrush(System.Drawing.Color.Red)), rectangles);
                Bitmap rosto = bmp.Clone(rectangles[0], bmp.PixelFormat);

                MainWindow.main.Statusa1 = rosto;
            }
        }

    }
    public static class bitmapp
    {
     

        public static Array2D<Byte> ToArray2D(Bitmap bitmap)
        {
            Int32 stride;
            Byte[] data;
            // Removes unnecessary getter calls.
            Int32 width = bitmap.Width;
            Int32 height = bitmap.Height;
            // 'using' block to properly dispose temp image.
            using (Bitmap grayImage = MakeGrayscale3(bitmap))
            {
                BitmapData bits = grayImage.LockBits(new System.Drawing.Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
                stride = bits.Stride;
                Int32 length = stride * height;
                data = new Byte[length];
                System.Runtime.InteropServices.Marshal.Copy(bits.Scan0, data, 0, length);
                grayImage.UnlockBits(bits);
            }
            // Constructor is (rows, columns), so (height, width)
            Array2D<Byte> array = new Array2D<Byte>(height, width);
            Int32 offset = 0;
            for (Int32 y = 0; y < height; y++)
            {
                // Offset variable for processing one line
                Int32 curOffset = offset;
                // Get row in advance
                Array2D<Byte>.Row<Byte> curRow = array[y];
                for (Int32 x = 0; x < width; x++)
                {
                    curRow[x] = data[curOffset]; // Should be the Blue component.
                    curOffset += 4;
                }
                // Stride is the actual data length of one line. No need to calculate that;
                // not only is it already given by the BitmapData object, but in some situations
                // it may differ from the actual data length. This also saves processing time
                // by avoiding multiplications inside each loop.
                offset += stride;
            }
            return array;
        }

        public static Bitmap MakeGrayscale3(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);

            //create the grayscale ColorMatrix
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
               {
         new float[] {.3f, .3f, .3f, 0, 0},
         new float[] {.59f, .59f, .59f, 0, 0},
         new float[] {.11f, .11f, .11f, 0, 0},
         new float[] {0, 0, 0, 1, 0},
         new float[] {0, 0, 0, 0, 1}
               });

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new System.Drawing.Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }
    }
}
