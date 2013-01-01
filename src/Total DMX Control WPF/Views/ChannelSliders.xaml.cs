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
using System.Timers;

namespace Total_DMX_Control_WPF.Views
{
    /// <summary>
    /// Interaction logic for ChannelSliders.xaml
    /// </summary>
    public partial class ChannelSliders : Window, IAsyncChannelDisplayer
    {
        #region Data Members
        //Default number of sliders to display
        private int _numSliders = Properties.Settings.Default.NumDisplayedChannels;
        //In case control was pressed/release between mousedown and mouseup
        private bool _shiftPressed = false;
        //List storing the sliders
        private List<Slider> _sliders;
        //List storing the bump buttons
        private List<SoloSuppressButton> _bumpButtons;

        //Trying something new
        //Timer for pull-based channel updating
        private Timer _timer;
        private const int REFRESH_INTERVAL_MS = 50;

        #endregion

        public ChannelSliders()
        {
            //Remember, arrays still start at 0
            _sliders = new List<Slider>();
            _bumpButtons = new List<SoloSuppressButton>();
            InitializeComponent();
            SetNumberOfSliders(_numSliders);
            _timer = new Timer();
            _timer.Interval = REFRESH_INTERVAL_MS;
            _timer.Elapsed += UpdateSuppressed;
        }

        private void ChannelSliders_Load(object sender, EventArgs e)
        {
            Controller.RegisterDisplayer(this);

            _timer.Start();
        }

        private void UpdateSuppressed(object sender, EventArgs e)
        {
            foreach (SoloSuppressButton btn in _bumpButtons)
            {
                btn.LitRight = DMXController.IsSuppressed(ParseTag(btn));
            }
        }

        #region Generating/Positioning stuff
        public void SetNumberOfSliders(int num)
        {
            _numSliders = num;

            // Generate new ones
            GenerateSliders();
        }

        private void GenerateSliders()
        {
            foreach (Slider s in _sliders) this.cnv_sliders.Children.Remove(s);
            foreach (SoloSuppressButton b in _bumpButtons) this.cnv_sliders.Children.Remove(b);

            //Remember, arrays still start at 0
            _sliders = new List<Slider>();
            _bumpButtons = new List<SoloSuppressButton>();

            for (int i = 1; i <= _numSliders; i++)
            {
                //Generate a trackbar
                Slider tbar = new Slider();
                //We will set location in a minute
                tbar.Margin = new Thickness();
                tbar.Name = "tbar_Chan" + i;
                //Zach discovered this
                //This property stores the associated channel
                tbar.Tag = i.ToString();
                tbar.LargeChange = 15;
                tbar.Maximum = 255;
                //tbar
                tbar.Width = 45;
                tbar.Height = 78;
                //We don't want to accidentally tab to this
                tbar.IsTabStop = false;
                tbar.Orientation = Orientation.Vertical;
                tbar.TickFrequency = 25;
                tbar.TickPlacement = System.Windows.Controls.Primitives.TickPlacement.Both;
                //Attach the scroll event handler
                tbar.ValueChanged += new RoutedPropertyChangedEventHandler<double>(this.tbr_Scroll);
                //Add it to the list
                _sliders.Add(tbar);
                //Actually add it to the form
                this.cnv_sliders.Children.Add(tbar);

                //Generate a button
                SoloSuppressButton btn = new SoloSuppressButton(45, 45);
                //We will set location in a minute
                btn.Margin = new Thickness();
                btn.Name = "btn_Chan" + i;
                //Zach discovered this
                //This property stores the associated channel
                btn.Tag = i.ToString();
                btn.IsTabStop = false;
                btn.Label = i.ToString();
                //Attach the event handlers
                btn.MouseDown += new MouseButtonEventHandler(this.bump_MouseDown);
                btn.MouseUp += new MouseButtonEventHandler(this.bump_MouseUp);
                //Add it to the list
                _bumpButtons.Add(btn);
                //Actually add it to the form
                this.cnv_sliders.Children.Add(btn);
                //Set the button's Green (bump) status to false
                btn.LitLeft = false;
                //Set the Red (suppressed) state
                btn.LitRight = DMXController.IsSuppressed(i);
            }

            PositionSliders(20, 10, 10, 5);
        }

        private void PositionSliders(int rowPadding, int colPadding, int borderPadding, int buttonPadding)
        {
            int currentX = borderPadding; // The upper left corner of the first label will be almost in the upper left
            int currentY = borderPadding + 25; // corner of the form but with "borderPadding" pixels of padding between it and the edges of the form

            for (int i = 0; i < _sliders.Count; i++)
            {
                //Position the slider
                _sliders[i].Margin = new Thickness(currentX, currentY, 0 , 0);
                //Position the corresponding button
                _bumpButtons[i].Margin = new Thickness(currentX, currentY + _sliders[i].Height + buttonPadding, 0, 0);

                //Calculate the position for the next slider
                currentX += Convert.ToInt32(_sliders[i].Width + colPadding);
                //Make sure another slider can fit on this row
                if (currentX + _bumpButtons[i].Width + borderPadding > this.Width)
                {
                    //If not, move it down to begin the next row
                    currentX = borderPadding;
                    currentY += Convert.ToInt32(_sliders[i].Height + buttonPadding + _bumpButtons[i].Height + rowPadding);
                }
            }
        }
        #endregion

        #region Event Handlers
        //Trackbars, sliders, etc are all grandchildren of the System.Windows.Forms.Control class
        //(although i don't belive labels or textboxes are)
        //Thus they all inherit the Tag property
        //If you have to cast a sender though, be sure to cast it as (Control) when calling this method
        private int ParseTag(Control sender)
        {
            string tag = sender.Tag.ToString();
            return Int32.Parse(tag);
        }

        private void tbr_Scroll(object sender, EventArgs e)
        {
            string tbarName = ((Slider)sender).Name;
            // Zach discovered Tag, which works great for this
            // Get the channel number from the tag
            int channelNum = ParseTag((Control)sender);
            int intensity = Convert.ToInt32(((Slider)sender).Value);
            DMXController.SetLevel(tbarName, channelNum, intensity);
        }

        private void bump_MouseDown(object sender, MouseEventArgs e)
        {
            SoloSuppressButton btn = (SoloSuppressButton)sender;

            // Zach discovered Tag, which works great for this
            // Get the channel number from the tag
            int channelNum = ParseTag((Control)sender);

            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                DMXController.FlushHTP(channelNum);
                if (btn.LitLeft) DMXController.SetLevel(btn.Name, channelNum, 255);
                DMXController.SetLevel(_sliders[channelNum - 1].Name, channelNum, Convert.ToInt32(_sliders[channelNum - 1].Value));
            }

            else if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
            {
                _shiftPressed = true;
                if (!DMXController.IsSuppressed(channelNum))
                {
                    DMXController.Suppress(channelNum);
                    btn.LitRight = true;
                }
                else
                {
                    DMXController.Unsuppress(channelNum);
                    btn.LitRight = false;
                }
            }

            else
            {
                // Set the level
                string buttonName = btn.Name;
                DMXController.SetLevel(buttonName, channelNum, 255);

                // If it was a right-click, toggle
                if (e.RightButton == MouseButtonState.Pressed)
                    btn.LitLeft = !btn.LitLeft;
            }
        }

        private void bump_MouseUp(object sender, MouseEventArgs e)
        {
            SoloSuppressButton btn = (SoloSuppressButton)sender;
            // Zach discovered Tag, which works great for this
            // Get the channel number from the tag
            int channelNum = ParseTag((Control)sender);

            if (_shiftPressed)
            {
                _shiftPressed = false;
            }

            else
            {
                // Turn the channel off if it wasn't right-clicked.
                string buttonName = btn.Name;
                if (!btn.LitLeft)
                    DMXController.SetLevel(buttonName, channelNum, 0);
            }
        }

        #endregion

        private void ClearAllSliders()
        {
            foreach (Slider slider in _sliders)
            {
                slider.Value = 0;
                DMXController.SetLevel(slider.Name, ParseTag(slider), 0);
            }
        }

        private void clearAllSlidersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearAllSliders();
        }

        private void ClearAllBumps()
        {
            foreach (SoloSuppressButton btn in _bumpButtons)
            {
                btn.LitLeft = false;
                DMXController.SetLevel(btn.Name, ParseTag(btn), 0);
            }
        }

        private void clearAllBumpsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearAllBumps();
        }

        public void UpdateNumChannelsDisplayed(int num)
        {
            SetNumberOfSliders(num);
        }

        public void SetBlackout(bool isBlackedOut)
        {

        }

        public void HTPFlush()
        {
            foreach (SoloSuppressButton btn in _bumpButtons)
            {
                if (btn.LitLeft) DMXController.SetLevel(btn.Name, ParseTag(btn), 255);
            }
            foreach (Slider tbar in _sliders)
            {
                DMXController.SetLevel(tbar.Name, ParseTag(tbar), Convert.ToInt32(tbar.Value));
            }
        }

        private void Window_Closing_1(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _timer.Stop();
            Controller.DeregisterDisplayer(this);
            ClearAllBumps();
            ClearAllSliders();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ClearAllBumps();
            ClearAllSliders();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            HTPFlush();
        }

        private void Window_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
            cnv_sliders.Height = this.ActualHeight - 38;
            cnv_sliders.Width = this.ActualWidth - 12;
            PositionSliders(20, 10, 10, 5);
        }
    }
}
