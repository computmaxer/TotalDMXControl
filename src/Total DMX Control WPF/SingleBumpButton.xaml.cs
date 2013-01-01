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
    /// Interaction logic for SingleBumpButton.xaml
    /// </summary>
    public partial class SingleBumpButton : Button
    {
        private bool lit;
        private TextBlock label;
        private Rectangle bumpLight;

        public bool Lit
        {
            get { return lit; }
            set 
            {
                if (bumpLight != null)
                {
                    if (value) bumpLight.Fill = Brushes.LimeGreen;
                    else bumpLight.Fill = Brushes.Transparent;
                }
                lit = value;
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

        public SingleBumpButton()
        {
            InitializeComponent();

            base.Width = 60;
            base.Height = 50;

            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Vertical;
            panel.Margin = new Thickness(0);

            bumpLight = new Rectangle();
            bumpLight.HorizontalAlignment = HorizontalAlignment.Center;
            bumpLight.Width = 56;
            bumpLight.Height = 6;
            Lit = false;
            panel.Children.Add(bumpLight);

            label = new TextBlock();
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.Height = 44;
            label.Width = 56;
            label.TextWrapping = TextWrapping.Wrap;
            label.FontSize = 14;
            label.TextAlignment = TextAlignment.Center;
            panel.Children.Add(label);

            this.Content = panel;
        }
    }
}
