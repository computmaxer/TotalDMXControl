using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF
{
    [Serializable()]
    public class AttributePoint : ISerializable
    {
        #region Data Members
        private Point _point;
        private ObservableCollection<AttributePointSetting> _attributePointSettings;
        private AttributePointPopupControl _popup;
        private Ellipse _ellipse;
        #endregion

        #region Properties
        public AttributePointPopupControl Popup
        {
            get { return _popup; }
        }

        public Ellipse Ellipse
        {
            get { return _ellipse; }
        }

        public Point Point
        {
            get { return _point; }
            set { _point = value; }
        }

        public ObservableCollection<AttributePointSetting> AttributePointSettings
        {
            get { return _attributePointSettings; }
        }

        #endregion

        public AttributePoint(Point point, Fixture fixture)
        {
            _point = point;
            _popup = new AttributePointPopupControl();
            Draw(point.X, point.Y);

            _attributePointSettings = new ObservableCollection<AttributePointSetting>();
            foreach (FixtureAttribute attribute in fixture.Attributes)
            {
                _attributePointSettings.Add(new AttributePointSetting(attribute));
            }
        }

        public AttributePoint(SerializationInfo info, StreamingContext ctxt)
        {
            _point = (Point)info.GetValue("_point", typeof(Point));
            _attributePointSettings = (ObservableCollection<AttributePointSetting>)info.GetValue("_attributePointSettings", typeof(ObservableCollection<AttributePointSetting>));

            Draw(_point.X, _point.Y);

            _popup = new AttributePointPopupControl();
        }

        public AttributePointSetting GetSettingForAttributeName(string attributeName)
        {
            // TODO: Maybe store these in a hash table to make this more efficient?

            AttributePointSetting theSetting = null;

            foreach (AttributePointSetting setting in _attributePointSettings)
            {
                if (setting.AttributeName == attributeName)
                {
                    theSetting = setting;
                    break;
                }
            }

            return theSetting;
        }

        private void Draw(double X, double Y)
        {
            Ellipse ell = new Ellipse();
            ell.Height = 900;
            ell.Width = 900;
            Canvas.SetTop(ell, Y - 450);
            Canvas.SetLeft(ell, X -450);
            ell.StrokeThickness = 310;
            ell.Fill = Brushes.Black;
            ell.Stroke = Brushes.Red;
            _ellipse = ell;
            _ellipse.Name += "_attrPoint";
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_point", _point);
            info.AddValue("_attributePointSettings", _attributePointSettings);
        }
    }
}
