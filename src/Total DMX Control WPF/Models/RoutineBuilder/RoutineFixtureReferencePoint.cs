using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF
{
    [Serializable()]
    public class RoutineFixtureReferencePoint : ISerializable
    {
        #region Data Members
        private string _name;
        private Ellipse _ellipse;
        private Label _label;
        private RoutineFixture _parent;
        private bool _visible;
        #endregion

        #region Properties
        public Point Location
        {
            get { return new Point(Canvas.GetLeft(_ellipse), Canvas.GetTop(_ellipse)); }
        }
        public Ellipse Ellipse
        {
            get { return _ellipse; }
        }
        public Label Label
        {
            get { return _label; }
            set { _label = value; }
        }
        public string Name
        {
            get { return _label.Content.ToString(); }
            set { _label.Content = value; }
        }
        public bool Visible
        {
            get { return _visible; }
            set 
            { 
                _visible = value;
                if (_visible)
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            }
        }
        #endregion

        #region Methods
        public RoutineFixtureReferencePoint(RoutineFixture parent, Point initialPosition)
        {
            _parent = parent;
            _visible = true;
            _parent.ReferencePoints.Add(this);
            Draw(initialPosition.X, initialPosition.Y);
        }

        public RoutineFixtureReferencePoint(SerializationInfo info, StreamingContext ctxt)
        {
            _name = info.GetString("_name");

            double ellX = info.GetDouble("_ellipseX");
            double ellY = info.GetDouble("_ellipseY");
            CreateEllipse(ellX, ellY);
            CreateLabel(info.GetString("_labelContent"), ellX,ellY);
            _parent = (RoutineFixture)info.GetValue("_parent", typeof(RoutineFixture));
            _visible = info.GetBoolean("_visible");
        }

        private void Draw(double X, double Y)
        {
            CreateEllipse(X, Y);
            CreateLabel("Reference Point " + _parent.ReferencePoints.Count.ToString(), X, Y);
        }

        private void CreateEllipse(double X, double Y)
        {
            Ellipse ell = new Ellipse();
            ell.Height = 800;
            ell.Width = 800;
            Canvas.SetTop(ell, Y);
            Canvas.SetLeft(ell, X);
            ell.StrokeThickness = 500;
            ell.Fill = Brushes.DarkGray;
            ell.Stroke = Brushes.DarkGray;
            _ellipse = ell;
        }

        private void CreateLabel(string content, double X, double Y)
        {
            Label lbl = new Label();
            lbl.FontSize = 1100;
            lbl.Foreground = Brushes.DarkGray;

            lbl.Content = content;
            Canvas.SetTop(lbl, Y + 1000);
            Canvas.SetLeft(lbl, X + 1000);
            _label = lbl;
        }

        public void Show()
        {
            ((RoutineBuilder)WindowManager.CurrentViews[typeof(RoutineBuilder)]).Canvas.Children.Add(Ellipse);
            ((RoutineBuilder)WindowManager.CurrentViews[typeof(RoutineBuilder)]).Canvas.Children.Add(Label);
        }

        public void Hide()
        {
            if (((RoutineBuilder)WindowManager.CurrentViews[typeof(RoutineBuilder)]).Canvas.Children.Contains(Ellipse))
            {
                ((RoutineBuilder)WindowManager.CurrentViews[typeof(RoutineBuilder)]).Canvas.Children.Remove(Ellipse);
                ((RoutineBuilder)WindowManager.CurrentViews[typeof(RoutineBuilder)]).Canvas.Children.Remove(Label);
            }
        }

        public void MoveFixtureTo()
        {
            _parent.Fixture.MoveTo(Location, 0);
        }

        public virtual void Move(double x_new, double y_new)
        {
            Canvas.SetLeft(_ellipse, x_new);
            Canvas.SetTop(_ellipse, y_new);
            Canvas.SetLeft(_label, x_new + 1000);
            Canvas.SetTop(_label, y_new + 1000);
        }

        // For serialization
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("_name", _name);
            info.AddValue("_ellipseX", Canvas.GetLeft(_ellipse));
            info.AddValue("_ellipseY", Canvas.GetTop(_ellipse));
            info.AddValue("_labelContent", _label.Content);
            info.AddValue("_parent", _parent);
            info.AddValue("_visible", _visible);
        }
        #endregion
    }
}
