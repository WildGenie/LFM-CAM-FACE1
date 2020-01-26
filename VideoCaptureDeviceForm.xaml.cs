using System;
using System.Windows;
using System.Windows.Controls;

using Accord.Video.DirectShow;
namespace LFM_CAM_FACE
{
 
    public partial class VideoCaptureDeviceForm : Window
    {
        public FilterInfoCollection videoDevices = null;

        public VideoCaptureDeviceForm()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = Application.Current.MainWindow;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            try
            {
            

                if (videoDevices.Count == 0)
                    throw new ApplicationException();
                foreach (FilterInfo device in videoDevices)
                {
                    devicesCombo.Items.Add(device.Name);
                }
            }
            catch (ApplicationException)
            {
                devicesCombo.Items.Add("No local capture devices");
                devicesCombo.IsEnabled = false;
                okButton.IsEnabled = false;
            }

            devicesCombo.SelectedIndex = 0;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            cameras.Instance().VideoDevice = videoDevices[devicesCombo.SelectedIndex].MonikerString;
            DialogResult = true;
        }

        private void devicesCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cameras.Instance().VideoDevice = videoDevices[devicesCombo.SelectedIndex].MonikerString;
            cameras.Instance().Deviceint = devicesCombo.SelectedIndex;
        }
    }
}
