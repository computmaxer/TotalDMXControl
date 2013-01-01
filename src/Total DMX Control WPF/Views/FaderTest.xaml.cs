using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Total_DMX_Control_WPF
{

    public delegate void UpdateTestFaderLabelDelegate(TimeSpan timespan);
    /// <summary>
    /// Interaction logic for FaderTest.xaml
    /// </summary>
    public partial class FaderTest : Window
    {
        TimeSpan _startTime;
        TimeSpan _total;

        public FaderTest()
        {
            InitializeComponent();
        }

        private void startFader1_Click(object sender, RoutedEventArgs e)
        {
            double time = Double.Parse(tbxFadeTime.Text);
            PanTiltCoarseFineFader fader1 = new PanTiltCoarseFineFader("TestFader", 1, 2, 20000, 3,4, 10000, time);
            _startTime = DateTime.Now.TimeOfDay;
            fader1.Run(new FaderDoneCallback(Fader1Finished));
        }

        private void Fader1Finished(){
            _total = DateTime.Now.TimeOfDay - _startTime;
            lblFader1Time.Dispatcher.Invoke(new UpdateTestFaderLabelDelegate(UpdateLabel), new object[] { _total });
        }

        private void UpdateLabel(TimeSpan timespan)
        {
            lblFader1Time.Content = timespan.ToString();
        }

        private void btnClearHTP_Click(object sender, RoutedEventArgs e)
        {
            DMXController.FlushHTPAll();
        }

    }
}
