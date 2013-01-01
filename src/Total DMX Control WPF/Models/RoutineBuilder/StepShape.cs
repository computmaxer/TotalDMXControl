using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Windows.Threading;
using System.Threading;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF
{
    public delegate double GetShapeHeightWidthDelegate(Shape shape);
    public delegate Point GetShapePointDelegate(Shape shape);

    [Serializable()]
    public abstract class StepShape : ISerializable
    {
        #region
        protected static int RESOLUTION = 32;
        #endregion

        #region Data Members

        protected Shape _shape;
        protected bool _isSelected;
        protected Rectangle _boundingBox;
        protected bool _moving;
        protected bool _moveBackwardsHeight;
        protected bool _moveBackwardsWidth;
        
        #endregion

        #region Properties
        public Shape Shape
        {
            get { return _shape; }
        }
        public Rectangle BoundingBox
        {
            get { return _boundingBox; }
        }

        public double Width
        {
            get
            {
                if (this.Shape.Dispatcher.Thread == Thread.CurrentThread)
                {
                    return GetShapeWidth(this.Shape);
                }
                else
                {
                    GetShapeHeightWidthDelegate caller = new GetShapeHeightWidthDelegate(GetShapeWidth);
                    return (double)Shape.Dispatcher.Invoke(caller, new object[] { Shape });
                }
            }
        }

        private double GetShapeWidth(Shape shape)
        {
            return shape.Width;
        }

        public double Height
        {
            get
            {
                if (this.Shape.Dispatcher.Thread == Thread.CurrentThread)
                {
                    return GetShapeHeight(this.Shape);
                }
                else
                {
                    GetShapeHeightWidthDelegate caller = new GetShapeHeightWidthDelegate(GetShapeHeight);
                    return (double)Shape.Dispatcher.Invoke(caller, new object[] { Shape });
                }
            }
        }

        private double GetShapeHeight(Shape shape)
        {
            return shape.Height;
        }

        public Point Position
        {
            get
            {
                if (this.Shape.Dispatcher.Thread == Thread.CurrentThread)
                {
                    return GetShapePosition(this.Shape);
                }
                else {
                    GetShapePointDelegate caller = new GetShapePointDelegate(GetShapePosition);
                    return (Point)this.Shape.Dispatcher.Invoke(caller, new object[] { this.Shape });
                }
            }
        }

        private Point GetShapePosition(Shape shape)
        {
            return new Point(Canvas.GetLeft(shape), Canvas.GetTop(shape));
        }
        #endregion

        #region Methods

        public StepShape(Shape shape)
        {
            _shape = shape;
            //_moving = false;
            _moveBackwardsHeight = false;
            _moveBackwardsWidth = false;

            CreateBoundingBox();

            //BindEvents();
        }

        public StepShape()
        {
            _moveBackwardsHeight = false;
            _moveBackwardsWidth = false;
            //CreateBoundingBox();
        }

        public StepShape(SerializationInfo info, StreamingContext ctxt)
        {
            _isSelected = info.GetBoolean("_isSelected");
            //_moving = info.GetBoolean("_moving");
            _moveBackwardsHeight = false;
            _moveBackwardsWidth = false;
        }

        protected void CreateBoundingBox()
        {
            //Setup Bounding Box
            _boundingBox = new Rectangle();
            _boundingBox.Height = _shape.Height + 1000;
            _boundingBox.Width = _shape.Width + 1000;
            _boundingBox.StrokeDashArray = new DoubleCollection(new double[2] { 30.0, 20.0 });
            Canvas.SetTop(_boundingBox, Canvas.GetTop(_shape) - 500);
            Canvas.SetLeft(_boundingBox, Canvas.GetLeft(_shape) - 500);
            _boundingBox.StrokeThickness = 50;
            _boundingBox.Stroke = Brushes.Black;
            //_boundingBox.MouseEnter += new System.Windows.Input.MouseEventHandler(_boundingBox_MouseEnter);
        }

        public void Shadow()
        {
            Shape.Stroke = Brushes.DarkGray;
        }

        public void Unshadow()
        {
            Shape.Stroke = Brushes.Black;
        }

        //void _boundingBox_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        //{
        //    Debugger.Log(1, "debug", "getting here");
        //    Mouse.OverrideCursor = Cursors.SizeAll;
        //}

        //void _shape_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        //{
        //    _moving = false;
        //}

        //protected Point _last_mouse_location;
        public virtual void MoveShape(double x_difference, double y_difference)
        {
            double new_left = Canvas.GetLeft(_shape) + (x_difference);
            double new_top = Canvas.GetTop(_shape) + (y_difference);
            if (new_left <= ((Canvas)_shape.Parent).Width - _shape.Width && new_left >= 0)
            {
                Canvas.SetLeft(_shape, new_left);
                Canvas.SetLeft(_boundingBox, Canvas.GetLeft(_boundingBox) + (x_difference));
            }
            if (new_top <= ((Canvas)_shape.Parent).Height - _shape.Height && new_top >= 0)
            {
                Canvas.SetTop(_shape, new_top);
                Canvas.SetTop(_boundingBox, Canvas.GetTop(_boundingBox) + (y_difference));
            }
        }

        //protected void _shape_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    Select(); //Need to deal with dragging and stuff.
        //    _moving = true;
        //    _last_mouse_location = e.GetPosition((Canvas)((Shape)sender).Parent);
        //}

        public virtual void UpdateBoundingBox()
        {
            _boundingBox.Height = _shape.Height + 1000;
            _boundingBox.Width = _shape.Width + 1000;
            Canvas.SetTop(_boundingBox, Canvas.GetTop(_shape) - 500);
            Canvas.SetLeft(_boundingBox, Canvas.GetLeft(_shape) - 500);
        }

        public void Select()
        {
            if (!_isSelected)
            {
                _isSelected = true;
                ((Canvas)_shape.Parent).Children.Add(_boundingBox);
            }
        }

        public void Deselect()
        {
            if(_isSelected)
                ((Canvas)_shape.Parent).Children.Remove(_boundingBox);
            _isSelected = false;
        }

        public virtual void DrawMouseMove(double x_difference, double y_difference)
        {
            if (_shape.Height + y_difference > 0 && !_moveBackwardsHeight)
            {
                _shape.Height = _shape.Height + y_difference;
            }
            else
            {
                _moveBackwardsHeight = true;
                if (y_difference > 0)
                {
                    if (_shape.Height - y_difference < 100)
                    {
                        _moveBackwardsHeight = false;
                        return;
                    }
                    _shape.Height = _shape.Height - y_difference;
                    double currentTop = Canvas.GetTop(_shape);
                    Canvas.SetTop(_shape, currentTop + y_difference);
                    
                }
                else
                {
                    _shape.Height = _shape.Height + Math.Abs(y_difference);
                    double currentTop = Canvas.GetTop(_shape);
                    Canvas.SetTop(_shape, currentTop + y_difference);
                }
            }
            if (_shape.Width + x_difference > 0 && !_moveBackwardsWidth)
            {
                _shape.Width = _shape.Width + x_difference;
            }
            else
            {
                _moveBackwardsWidth = true;
                if (x_difference > 0)
                {
                    if (_shape.Width - x_difference < 100)
                    {
                        _moveBackwardsWidth = false;
                        return;
                    }
                    _shape.Width = _shape.Width - x_difference;
                    double currentLeft = Canvas.GetLeft(_shape);
                    Canvas.SetLeft(_shape, currentLeft + x_difference);
                }
                else
                {
                    _shape.Width = _shape.Width + Math.Abs(x_difference);
                    double currentLeft = Canvas.GetLeft(_shape);
                    Canvas.SetLeft(_shape, currentLeft + x_difference);
                }
            }
        }

        public virtual void DrawStep2MouseMove(double x_difference, double y_difference)
        {
            return;
        }

        public List<Point> GetPoints(double duration)
        {
            List<Point> points = new List<Point>();
            //Application.Current.MainWindow.Dispatcher.Invoke((Action)delegate
            //{
                points = GetPointsHelper(duration);
            //});
            return points;
        }

        protected virtual List<Point> GetPointsHelper(double duration)
        {
            return new List<Point>();
        }

        public int GetResolution(double duration)
        {
            // The maximum resolution is the number of timer ticks
            // it will take to play the step
            int numTicksForStep = (int)Math.Ceiling(duration * 1000.0 / (Fader.FADER_TIMER_TICK_INTERVAL*1.0));

            return numTicksForStep;
            // The minimum resolution is something we pick to ensure
            // that the shape looks decent (but no matter what it can't
            // be bigger than the number of timer ticks
            ////int minResolution = Math.Min(255, numTicksForStep);

            // Now we set the resolution to be the bigger of the following:
            // 1) 10 * timer tick interval
            // 2) the minimum resolution
            ////return Math.Max((int)Math.Ceiling((duration * 1000.0) / (10 * Fader.FADER_TIMER_TICK_INTERVAL)), minResolution);
        }

        // For serialization
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("_isSelected", _isSelected);
            //info.AddValue("_moving", _moving);
            info.AddValue("Position", Position);
        }

        #endregion
    }
}
