using System.Windows;
using AForge.Video.DirectShow;
using AForge.Video;
using System;
using System.Drawing;
using System.Threading;
using System.Collections.Generic;
using AForge.Vision.Motion;
using Accord.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
using System.IO;
using DlibDotNet.Dnn;
using Dlib = DlibDotNet.Dlib;
using Rectangle = DlibDotNet.Rectangle;
namespace LFM_CAM_FACE
{

    public partial class MainWindow : Window
    { 
     
        int i = 0;

        public MainWindow()
        {
      
            InitializeComponent();
            imageBox1.Width = (int)primeirohost.Width;
            imageBox1.Height = (int)primeirohost.Height;
            imageBox2.Width = (int)segundohost.Width;
            imageBox2.Height = (int)segundohost.Height;
            main = this;
            picturebox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            
        }

 
        internal static MainWindow main;
        internal string Status
        {
            get { return textBox1_Copy.Content.ToString(); }
            set { Dispatcher.Invoke(new Action(() => { textBox1_Copy.Content = value; })); }
        }
        internal System.Drawing.Image Statusa
        {
            get { return picturebox1.Image; }
            set { Dispatcher.Invoke(new Action(() => { picturebox1.Image = value;
           })); }
        }

        internal System.Drawing.Image Statusa1
        {
            get { return picturebox11.Image; }
            set
            {
                Dispatcher.Invoke(new Action(() => {
                    picturebox11.Image = value;
                }));
            }
        }

        private void Iniciar_Click(object sender, RoutedEventArgs e)
        {
          
            CameraCaptura.Instance().Iniciar(i);
        }


        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptura.Instance().Salvar();
        }

        private void Treinar_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptura.Instance().Treinar();
        }

        private void Parar_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptura.Instance().Parar();
        }

        private void Imagem_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptura.Instance().Buscarimagemarquivo();
        }

        private void Salvar_imagem_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptura.Instance().Salvarimagem(Variaveis.IdentificationNumber); ;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cameras.Instance().CloseVideoSource();
        }
        private void btnselecionarcamer_Click(object sender, RoutedEventArgs e)
        {
            cameras.Instance().selecionarcameras();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var d = new Microsoft.Win32.OpenFileDialog();
            if (d.ShowDialog(this) == true)
            {

                var image_files = d.FileName;
                GetImage(image_files);

            }
        }
        public void GetImage(string imagePath)
        {
            using (FrontalFaceDetector fd = Dlib.GetFrontalFaceDetector())

            {
                Array2D<RgbPixel> img = Dlib.LoadImage<RgbPixel>(imagePath);

                // find all faces in the image
                var faces = fd.Operator(img);
                foreach (var face in faces)
                {
                    // draw a rectangle for each face
                    Dlib.DrawRectangle(img, face, color: new RgbPixel(0, 255, 255), thickness: 4);
                }
                Dlib.SaveJpeg(img, AppDomain.CurrentDomain.BaseDirectory + "outpuht.jpg");
                MessageBox.Show("ok");
            }
        }
       
    }
}




