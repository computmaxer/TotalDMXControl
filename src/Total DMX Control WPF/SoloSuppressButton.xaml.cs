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

namespace Total_DMX_Control_WPF
{
    /// <summary>
    /// Interaction logic for DoubleBumpButton.xaml
    /// </summary>
    public partial class SoloSuppressButton : Button
    {
        private bool litLeft, litRight;
        private TextBlock label;
        private Rectangle bumpLightLeft, bumpLightRight;

        public bool LitLeft
        {
            get { return litLeft; }
            set
            {
                if (bumpLightLeft != null)
                {
                    if (value) bumpLightLeft.Fill = Brushes.LimeGreen;
                    else bumpLightLeft.Fill = Brushes.Transparent;
                }
                litLeft = value; 
            }
        }

        public bool LitRight
        {
            get { return litRight; }
            set
            {
                if (bumpLightRight != null)
                {
                    if (value) bumpLightRight.Fill = Brushes.Red;
                    else bumpLightRight.Fill = Brushes.Transparent;
                }
                litRight = value;
            }
        }

        public string Label
        {
            get
            {
                if (label != null) return label.Text;
                else return String.Empty;
            }
            set
            {
                if (label != null) label.Text = value;
            }
        }

        public SoloSuppressButton() : this(192, 50)
        {

        }

        public SoloSuppressButton(int width, int height)
        {
            InitializeComponent();

            base.Width = width;
            base.Height = height;

            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Vertical;
            panel.Margin = new Thickness(0);

            StackPanel bumps = new StackPanel();
            bumps.Orientation = Orientation.Horizontal;
            bumps.Margin = new Thickness(0);

            bumpLightLeft = new Rectangle();
            bumpLightLeft.HorizontalAlignment = HorizontalAlignment.Center;
            bumpLightLeft.Width = base.Width / 2;
            bumpLightLeft.Height = 6;
            LitLeft = false;
            bumps.Children.Add(bumpLightLeft);

            bumpLightRight = new Rectangle();
            bumpLightRight.HorizontalAlignment = HorizontalAlignment.Center;
            bumpLightRight.Width = base.Height / 2;
            bumpLightRight.Height = 6;
            LitRight = false;
            bumps.Children.Add(bumpLightRight);

            panel.Children.Add(bumps);

            label = new TextBlock();
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.Height = base.Height - 6;
            label.Width = base.Width;
            label.TextWrapping = TextWrapping.Wrap;
            label.FontSize = 14;
            label.TextAlignment = TextAlignment.Center;
            panel.Children.Add(label);

            this.Content = panel;
        }
    }
}
