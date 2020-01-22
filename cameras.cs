using AForge.Video;
using AForge.Video.DirectShow;
using Accord.Vision.Motion;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Accord.Imaging;
using Accord;
using System.Windows;
using System.Windows.Threading;
using Accord.Controls;
using System.ComponentModel;
namespace LFM_CAM_FACE
{
    public class cameras 
    {
      
        private string videoDevice = null;
        private int deviceint = 0;
        private Bitmap rosto = null;

        public delegate void RostoEventHandler(object sender, EventArgs e);

        public event RostoEventHandler Rostomudou;


        public int Deviceint { get => deviceint; set => deviceint = value; }
        public string VideoDevice { get => videoDevice; set => videoDevice = value; }
        public Bitmap Rosto {
            get{ return rosto; }
            set
            {
                if (rosto != value)
                {
                    rosto = value;
                  
                    OnPropertyChanged(new EventArgs());
                }
            }
        }

        private void OnPropertyChanged(EventArgs e)
        {
            RostoEventHandler handler = Rostomudou;
            if (handler != null)
            {
                handler(Rosto,  e);
            }
        }

      
        VideoCaptureDevice videoSource = null;
        AsyncVideoSource asyncVideoSource = null;
        MotionDetector detector = new MotionDetector(new SimpleBackgroundModelingDetector(), new BlobCountingObjectsProcessing());
      
        private float motionAlarmLevel = 0.2f;

        DispatcherTimer dispatcherTimer = new DispatcherTimer();


        private static cameras _instance;

        public static cameras Instance()
        {
            if (_instance == null)
            {
                _instance = new cameras();
            }
            return _instance;
        }

        public cameras()
        {
           
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
      
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (asyncVideoSource != null)
            {
                if (asyncVideoSource.IsRunning == true) { Variaveis.Camerafechada = false; }
                if (asyncVideoSource.IsRunning == false) { Variaveis.Camerafechada = true; }
            }
        }

        public void selecionarcameras()
        {
            Detectorderosto.Instance();
            try
            {
                VideoCaptureDeviceForm videoCaptureDeviceForm = new VideoCaptureDeviceForm();
                videoCaptureDeviceForm.Owner = Variaveis.Principal();
                videoCaptureDeviceForm.ShowDialog();
                if (videoCaptureDeviceForm.DialogResult == true)
                {
                    videoSource = new VideoCaptureDevice(VideoDevice);
                    videoSource.VideoResolution = selectResolution(videoSource);
                    OpenVideoSource(videoSource);
                }
                else if (videoCaptureDeviceForm.DialogResult == false)
                {
                    Variaveis.modomensagem = ("O usuário não selecionou o dispositivo de video");
                    Variaveis.modomensagemaberto = true;
                    var esperawindow = new Esperawindow();
                    esperawindow.Owner = Variaveis.Principal();
                    esperawindow.Show();

                    Variaveis.modomensagemaberto = false;
                }
            }
            catch { }
        
        }
        private static VideoCapabilities selectResolution(VideoCaptureDevice device)
        {
            foreach (var cap in device.VideoCapabilities)
            {
                if (cap.FrameSize.Width == 1280)
                    return cap;
                if (cap.FrameSize.Height == 800)
                    return cap;
            }
            return device.VideoCapabilities.Last();
        }

        private void OpenVideoSource(VideoCaptureDevice source)
        {
            Variaveis.Principal().Cursor = System.Windows.Input.Cursors.Wait;

            CloseVideoSource();
            asyncVideoSource = new AsyncVideoSource(source);
            asyncVideoSource.NewFrame += AsyncVideoSource_NewFrame;
            asyncVideoSource.Start();
            videoSource = source;
            Variaveis.Principal().Cursor = System.Windows.Input.Cursors.Arrow;
        }

        private void AsyncVideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            lock (this)
            {
                if (detector != null)
                {
                    Bitmap temp = (Bitmap)eventArgs.Frame.Clone();
                    Bitmap templ = (Bitmap)eventArgs.Frame.Clone();
                    float motionLevel = detector.ProcessFrame(templ);
                    MainWindow.main.Status = motionLevel.ToString();
                    if (motionLevel > motionAlarmLevel)
                    {
                        BlobCounter blobCounter1 = new BlobCounter();
                        blobCounter1.ProcessImage(temp);
                        Rectangle[] rects = blobCounter1.GetObjectsRectangles();
                        foreach (Rectangle recs in rects)
                            if (rects.Length > 0)
                            {
                                if (recs.Width > 100)
                                {
                                    if (recs.Height > 100)
                                    {
                                        Graphics g = Graphics.FromImage(temp);
                                        using (Pen pen = new Pen(Color.FromArgb(160, 255, 160), 3))
                                        {
                                            g.DrawRectangle(pen, recs);
                                            Rosto = temp.Clone(recs, temp.PixelFormat);
                                    //      Detectorderosto.Instance().detectarrosto(Rosto);
                                            MainWindow.main.Statusa = Rosto;
                                        }
                                        g.Dispose();
                                    }
                                }
                            }
                    }
                    else
                    {
                        MainWindow.main.Statusa = temp;
                    }
                }
            }
        }
        public void CloseVideoSource()
        {
            try
            {
                if (asyncVideoSource != null)
                {
                    if (asyncVideoSource.IsRunning == true)
                    {
                        asyncVideoSource.SignalToStop();
                        asyncVideoSource.NewFrame -= AsyncVideoSource_NewFrame;
                        var esperawindow = new Esperawindow();
                        esperawindow.Owner = Variaveis.Principal();
                        esperawindow.ShowDialog();
                        asyncVideoSource.WaitForStop();
                    }
                    if (asyncVideoSource.IsRunning == true)
                    {
                        asyncVideoSource.WaitForStop();
                    }
                    if (asyncVideoSource.IsRunning == true)
                    {
                        for (int i = 0; (i < 50) && (asyncVideoSource.IsRunning == true); i++)
                        {
                            Thread.Sleep(100);
                            if (asyncVideoSource.IsRunning != true) { break; }
                        }
                    }
                    if (asyncVideoSource.IsRunning == true) { asyncVideoSource.Stop(); }
                }
            }
            catch
            {
                Variaveis.modomensagem = ("Ocorreu erro ao manipular o dispositivo");
                Variaveis.modomensagemaberto = true;
                var esperawindow = new Esperawindow();
                esperawindow.Owner = Variaveis.Principal();
                esperawindow.Show();
                                Variaveis.modomensagemaberto = false;
            }

        }
    }
}
