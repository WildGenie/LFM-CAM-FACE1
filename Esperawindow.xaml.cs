using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
namespace LFM_CAM_FACE
{

    /// <summary>
    /// Lógica interna para Esperawindow.xaml
    /// </summary>
    public partial class Esperawindow : Window
    {
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        public Esperawindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = Application.Current.MainWindow;
         
           
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            textblockmensagem.Text = Variaveis.modomensagem;
            if (Variaveis.Camerafechada == true)
            {
                if (Variaveis.modomensagemaberto == true)
                {
                    Thread.Sleep(5000);
                    return; }
                else {  dispatcherTimer.Stop();
                    dispatcherTimer.Tick -= DispatcherTimer_Tick;
                  
                    gifplayer.Stop();
                    this.Close(); }

            }
            if (Variaveis.Camerafechada == false)
            {
                if (Variaveis.modomensagemaberto == true)
                {
                    Thread.Sleep(5000);
                    return;
                }
                else
                {  dispatcherTimer.Stop();
                    dispatcherTimer.Tick -= DispatcherTimer_Tick;
                  
                    gifplayer.Stop();
                    this.Close();
                }

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
   gifplayer.Play();
            gifplayer.MediaOpened += Gifplayer_MediaOpened;
         
        }

        private void Gifplayer_MediaOpened(object sender, RoutedEventArgs e)
        {  bordaespera.Visibility = Visibility.Visible;
             dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
         
            textblockmensagem.Text = Variaveis.modomensagem;
        }

        private void gifplayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            gifplayer.Stop();
            gifplayer.Position = new TimeSpan(0, 0, 1);
            gifplayer.Play();
        }
    }
}
