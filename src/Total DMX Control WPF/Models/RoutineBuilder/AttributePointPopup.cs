using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Media;

namespace Total_DMX_Control_WPF
{
    class AttributePointPopup : Popup
    {
        #region Data Members
        private AttributePointPopupControl _control;
        #endregion

        #region Properties
        public AttributePointPopupControl Control
        {
            get { return _control; }
        }
        #endregion

        #region Methods
        public AttributePointPopup()
            : base()
        {
            this.AllowsTransparency = true;
            _control = new AttributePointPopupControl();
            this.Child = _control;
            _control.DoneClick += new System.Windows.RoutedEventHandler(_control_DoneClick);
        }

        void _control_DoneClick(object sender, System.Windows.RoutedEventArgs e)
        {
            foreach (AttributePointSetting setting in Control.lbxAttributePoint.Items)
            {
                setting.Attribute.Off("LIVEPREVIEW");
            }
            Hide();
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
        }
        #endregion
    }
}
