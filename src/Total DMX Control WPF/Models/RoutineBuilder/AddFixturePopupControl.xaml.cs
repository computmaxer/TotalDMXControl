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
    /// Interaction logic for AddFixturePopupControl.xaml
    /// </summary>
    public partial class AddFixturePopupControl : UserControl
    {
        public AddFixturePopupControl()
        {
            InitializeComponent();
            lbxFixturesToAdd.ItemsSource = Controller.Fixtures;
        }

        public event RoutedEventHandler AddSelectedClick
        {
            add { btnAddSelectedFixtures.Click += value; }
            remove { btnAddSelectedFixtures.Click -= value; }
        }

        public event RoutedEventHandler CancelAddFixtureClick
        {
            add { btnCancelAddFixture.Click += value; }
            remove { btnCancelAddFixture.Click += value; }
        }

        public void Reset()
        {
            tbxFixtureFilter.Text = null;
            lbxFixturesToAdd.ItemsSource = null;
            lbxFixturesToAdd.ItemsSource = Controller.Fixtures;
        }
    }
}
