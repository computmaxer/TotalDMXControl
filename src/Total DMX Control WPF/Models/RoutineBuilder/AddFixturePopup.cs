using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows;

namespace Total_DMX_Control_WPF
{
    class AddFixturePopup : Popup
    {

        public event RoutedEventHandler AddSelectedClick
        {
            add { ((AddFixturePopupControl)((Border)this.Child).Child).AddSelectedClick += value; }
            remove { ((AddFixturePopupControl)((Border)this.Child).Child).AddSelectedClick -= value; }
        }

        public event RoutedEventHandler CancelAddFixtureClick
        {
            add { ((AddFixturePopupControl)((Border)this.Child).Child).CancelAddFixtureClick += value; }
            remove { ((AddFixturePopupControl)((Border)this.Child).Child).CancelAddFixtureClick -= value; }
        }

        public AddFixturePopup()
            : base()
        {

            //Establish Border object. Everything will go inside the border.
            Border brd = new Border();
            brd.BorderThickness = new System.Windows.Thickness(1);
            AddFixturePopupControl ctrl = new AddFixturePopupControl();
            brd.Child = ctrl;
            this.Child = brd;
            this.CancelAddFixtureClick += new RoutedEventHandler(AddFixturePopup_CancelAddFixtureClick);
        }

        void AddFixturePopup_CancelAddFixtureClick(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        public void Show()
        {
            this.IsOpen = true;
        }

        public void Show(PlacementMode placement)
        {
            this.IsOpen = true;
            this.Placement = placement;
        }

        public void Hide()
        {
            this.IsOpen = false;
            ((AddFixturePopupControl)((Border)this.Child).Child).Reset();
        }
    }
}
