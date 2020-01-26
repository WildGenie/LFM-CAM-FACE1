using System;
using System.Drawing;
using System.IO;
using DlibDotNet;

using DlibDotNet.Extensions;
using Dlib = DlibDotNet.Dlib;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Windows.Forms;

namespace LFM_CAM_FACE
{
    class Detectorderosto
    {
        private  ShapePredictor _ShapePredictor;
        string shapePredictorFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "shape_predictor_68_face_landmarks.dat");

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
            _ShapePredictor = ShapePredictor.Deserialize(shapePredictorFilePath);
     
            cameras.Instance().Rostomudou += Detectorderosto_Rostomudoua;
        }
     
        private void Detectorderosto_Rostomudoua(object sender, EventArgs e)
        {
            Bitmap frame = (Bitmap)sender;
            buscarrosto(frame);
          


        }
        private void buscarrosto(Bitmap frame)
        {
  Image<Rgb, Byte> imageCV = new Image<Rgb, byte>(frame); 
            Emgu.CV.Mat mat = imageCV.Mat;
            var array = new byte[mat.Width * mat.Height * mat.ElementSize];
            mat.CopyTo(array);

            using (Array2D<RgbPixel> image = Dlib.LoadImageData<RgbPixel>(array, (uint)mat.Height, (uint)mat.Width, (uint)(mat.Width * mat.ElementSize)))
            {
                using (FrontalFaceDetector fd = Dlib.GetFrontalFaceDetector())

                {
                 
                    var faces = fd.Operator(image);
                    foreach (DlibDotNet.Rectangle face in faces)
                    {
                              
                        FullObjectDetection shape = _ShapePredictor.Detect(image, face);
                        ChipDetails faceChipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.25);
                        Array2D<RgbPixel> faceChip = Dlib.ExtractImageChip<RgbPixel>(image, faceChipDetail);
                        Bitmap bitmap1 = faceChip.ToBitmap<RgbPixel>();
                        MainWindow.main.Statusa1 = bitmap1;
                        Dlib.DrawRectangle(image, face, color: new RgbPixel(0, 255, 255), thickness: 4);
                    }
              

                }
             frame = image.ToBitmap<RgbPixel>();
                MainWindow.main.Statusa = frame;
            }
        }
        public void GetImage(string imagePath)
        {
                Array2D<RgbPixel> image = Dlib.LoadImage<RgbPixel>(imagePath);
            using (FrontalFaceDetector fd = Dlib.GetFrontalFaceDetector())

            {

                var faces = fd.Operator(image);
                foreach (DlibDotNet.Rectangle face in faces)
                {

                    FullObjectDetection shape = _ShapePredictor.Detect(image, face);
                    ChipDetails faceChipDetail = Dlib.GetFaceChipDetails(shape, 150, 0.25);
                    Array2D<RgbPixel> faceChip = Dlib.ExtractImageChip<RgbPixel>(image, faceChipDetail);
                    Bitmap bitmap1 = faceChip.ToBitmap<RgbPixel>();
                    MainWindow.main.Statusa1 = bitmap1;
                    Dlib.DrawRectangle(image, face, color: new RgbPixel(0, 255, 255), thickness: 4);
                }


            }
          Bitmap  frame = image.ToBitmap<RgbPixel>();
            MainWindow.main.Statusa = frame;
        }
    

    }

}
