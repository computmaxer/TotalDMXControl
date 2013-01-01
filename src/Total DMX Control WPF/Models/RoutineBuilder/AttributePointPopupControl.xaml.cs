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
using Xceed.Wpf.Toolkit;

namespace Total_DMX_Control_WPF
{
    /// <summary>
    /// Interaction logic for AttributePointPopupControl.xaml
    /// </summary>
    public partial class AttributePointPopupControl : UserControl
    {
        public AttributePointPopupControl()
        {
            InitializeComponent();
        }

        public event RoutedEventHandler DoneClick
        {
            add { btnAttrPointPopupDone.Click += value; }
            remove { btnAttrPointPopupDone.Click -= value; }
        }

    }
}
