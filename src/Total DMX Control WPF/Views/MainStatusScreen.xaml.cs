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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Total_DMX_Control_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainStatusScreen : Window, IAsyncChannelDisplayer
    {
        #region Data Members
        //Default number of channels displayed
        private int numChannelsDisplayed = Properties.Settings.Default.NumDisplayedChannels;

        //Label lists
        private List<Label> channelNumberLabels;
        private List<Label> channelIntensityLabels;

        //Define default font for Number Labels
        private FontFamily numberFont = new FontFamily("Arial Black");
        private Color numberColor = Colors.DarkGray;
        private double numberSize = 16;

        //Define default font for Intensity Labels
        private FontFamily intensityFont = new FontFamily("Arial Black");
        private Color intensityColor = Colors.Yellow;
        private double intensitySize = 16;
        private Color suppressedColor = Colors.Red;

        //Trying something new
        //Timer for pull-based channel updating
        private DispatcherTimer timer;
        private const int REFRESH_INTERVAL_MS = 25;

        #endregion

        public MainStatusScreen()
        {
            InitializeComponent();
            channelIntensityLabels = new List<Label>();
            channelNumberLabels = new List<Label>();
            SetNumberOfLabels(numChannelsDisplayed);
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(REFRESH_INTERVAL_MS * 10);
            timer.Tick += UpdateIntensitiesAndSuppressed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Controller.RegisterDisplayer(this);
            timer.Start();
        }

        private void UpdateIntensitiesAndSuppressed(object sender, EventArgs e)
        {
            for (int i = 1; i <= numChannelsDisplayed; i++)
                UpdateIntensity(i, DMXController.GetLevel(i), DMXController.IsSuppressed(i));
        }

        private void GenerateLabels()
        {
            channelNumberLabels = new List<Label>();
            channelIntensityLabels = new List<Label>();

            for (int i = 1; i <= numChannelsDisplayed; i++) // Notice we start the loop at 1 so that the first channel number will be 1, not 0
            {
                //Make the channel number label
                Label numberLabel = new Label();
                numberLabel.FontFamily = numberFont;
                numberLabel.FontWeight = FontWeights.Bold;
                numberLabel.Foreground = new SolidColorBrush(numberColor);
                numberLabel.FontSize = numberSize;
                numberLabel.Content = i;
                numberLabel.ContentStringFormat = "{0:00}";
                numberLabel.Margin = new Thickness();
                channelNumberLabels.Add(numberLabel); // Add the label to the list so we can keep track of it
                cnv_Levels.Children.Add(numberLabel); // This actually adds the label to the form so it shows up when the form draws

                //Make the channel intensity label
                Label intensityLabel = new Label();
                intensityLabel.FontFamily = intensityFont;
                intensityLabel.FontWeight = FontWeights.Bold;
                intensityLabel.Foreground = new SolidColorBrush(intensityColor);
                intensityLabel.FontSize = intensitySize;
                intensityLabel.Content = "";
                intensityLabel.ContentStringFormat = "{0:00}";
                intensityLabel.Margin = new Thickness();
                channelIntensityLabels.Add(intensityLabel);
                cnv_Levels.Children.Add(intensityLabel);
            }
            PositionLabels(20, 10, 10);
        }

        private void PositionLabels(int rowPadding, int colPadding, int borderPadding)
        {
            double currentX = borderPadding; // The upper left corner of the first label will be almost in the upper left
            double currentY = borderPadding + 25; // corner of the form but with "borderPadding" pixels of padding between it and the edges of the form

            for (int i = 0; i < channelNumberLabels.Count; i++)
            {
                // Position the channel number label
                channelNumberLabels[i].Margin = new Thickness(currentX, currentY, 0, 0);

                // Position the corresponding intensity label
                channelIntensityLabels[i].Margin = new Thickness(currentX, currentY + channelNumberLabels[i].ActualHeight, 0, 0);

                // Calcualte the position for the next channel
                currentX += channelNumberLabels[i].ActualWidth + colPadding;
                // Make sure another channel can fit on this row
                if (currentX + channelNumberLabels[i].ActualWidth + borderPadding > cnv_Levels.ActualWidth)
                {
                    // If another channel is too wide to fit on the current row,
                    // move down to the next row
                    currentX = borderPadding;
                    currentY += channelNumberLabels[i].ActualHeight + channelIntensityLabels[i].ActualHeight + rowPadding;
                }
            }
        }

        private void SetNumberOfLabels(int num)
        {
            // Get rid of existing labels
            foreach (Label label in channelNumberLabels) cnv_Levels.Children.Remove(label);
            foreach (Label label in channelIntensityLabels) cnv_Levels.Children.Remove(label);

            numChannelsDisplayed = num;

            // Generate new ones
            GenerateLabels();
        }

        public void UpdateNumChannelsDisplayed(int num)
        {
            SetNumberOfLabels(num);
        }

        private void UpdateIntensity(int channelNumber, int intensity, bool suppressed)
        {
            if (channelNumber < channelIntensityLabels.Count && channelNumber < channelNumberLabels.Count)
            {
                //Sets the label to either blank for 0, 01-99 for in between, or FL for 255
                if (intensity == 255) channelIntensityLabels[channelNumber - 1].Content = "FL";
                else if (intensity == 0) channelIntensityLabels[channelNumber - 1].Content = "";
                else channelIntensityLabels[channelNumber - 1].Content = String.Format("{0:00}", (intensity * 100) / 255);
                //If the channel is suppressed the label becomes red
                if (suppressed) channelIntensityLabels[channelNumber - 1].Foreground = new SolidColorBrush(suppressedColor);
                else channelIntensityLabels[channelNumber - 1].Foreground = new SolidColorBrush(intensityColor);
            }

        }

        public void SetBlackout(bool isBlackedOut)
        {
            //if (isBlackedOut) lbl_blackout.Text = "BLACKOUT";
            //else lbl_blackout.Text = "";
        }

        public void HTPFlush()
        {

        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            cnv_Levels.Height = this.ActualHeight - 38;
            cnv_Levels.Width = this.ActualWidth - 12;
            PositionLabels(20, 10, 10);
        }

        private void mbrWindowRoutine_Click(object sender, RoutedEventArgs e)
        {
            //WindowManager.ShowWindow<RoutineBuilder>();
        }

        private void mbrWindowFaderTest_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowWindow<FaderTest>();
        }

        private void mbrWindowRoutinePlayer_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowWindow<RoutinePlayerWindow>();
        }

        private void mbrWindowEffectsDesk_Click(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowWindow<EffectsDesk>();
        }

        private void mbrWindowChannelSliders_Click_1(object sender, RoutedEventArgs e)
        {
            WindowManager.ShowWindow<Views.ChannelSliders>();
        }

    }
}
