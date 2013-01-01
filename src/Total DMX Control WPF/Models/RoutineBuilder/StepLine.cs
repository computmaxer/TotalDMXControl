using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;
using System.Threading;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF
{
    public delegate double GetShapeLengthDelegate(Shape shape);
    
    [Serializable()]
    public class StepLine : StepShape, ISerializable
    {
        #region Data Members

        #endregion

        #region Properties

        private double Length
        {
            get
            {
                if (this.Shape.Dispatcher.Thread == Thread.CurrentThread)
                {
                    return GetShapeLength(this.Shape);
                }
                else
                {
                    GetShapeLengthDelegate caller = new GetShapeLengthDelegate(GetShapeLength);
                    return (double)this.Shape.Dispatcher.Invoke(caller, new object[] { this.Shape });
                }
            }
        }

        private double GetShapeLength(Shape shape)
        {
            Line line = ((Line)shape);
            double xDelta = Math.Abs(line.X1 - line.X2);
            double yDelta = Math.Abs(line.Y1 - line.Y2);

            return Math.Sqrt(xDelta * xDelta + yDelta * yDelta);
        }

        public Point Point1
        {
            get
            {
                if (this.Shape.Dispatcher.Thread == Thread.CurrentThread)
                {
                    return GetShapePoint1(this.Shape);
                }
                else
                {
                    GetShapePointDelegate caller = new GetShapePointDelegate(GetShapePoint1);
                    return (Point)this.Shape.Dispatcher.Invoke(caller, new object[] { this.Shape });
                }
            }
        }

        private Point GetShapePoint1(Shape shape)
        {
            return new Point(((Line)shape).X1, ((Line)shape).Y1);
        }

        public Point Point2
        {
            get
            {
                if (this.Shape.Dispatcher.Thread == Thread.CurrentThread)
                {
                    return GetShapePoint2(this.Shape);
                }
                else
                {
                    GetShapePointDelegate caller = new GetShapePointDelegate(GetShapePoint2);
                    return (Point)this.Shape.Dispatcher.Invoke(caller, new object[] { this.Shape });
                }
            }
        }

        private Point GetShapePoint2(Shape shape)
        {
            return new Point(((Line)shape).X2, ((Line)shape).Y2);
        }
        #endregion

        #region Methods

        public StepLine(Line shape)
            :base(shape)
        {

        }

        public StepLine(SerializationInfo info, StreamingContext ctxt)
         : base(info, ctxt) 
        {
            Line line = new Line();

            line.X1 = info.GetInt32("X1");
            line.X2 = info.GetInt32("X2");
            line.Y1 = info.GetInt32("Y1");
            line.Y2 = info.GetInt32("Y2");

            line.StrokeThickness = 200;
            line.Fill = Brushes.White;
            line.Stroke = Brushes.Black;

            Point position = (Point)info.GetValue("Position", typeof(Point));
            Canvas.SetLeft(line, position.X);
            Canvas.SetTop(line, position.Y);

            _shape = line;

            CreateBoundingBox();
        }

        //Used when drawing line originally while mouse is held down. Not to move the object once it has been drawn.
        public override void DrawMouseMove(double x_difference, double y_difference)
        {
            ((Line)_shape).Y2 = ((Line)_shape).Y2 + y_difference;
            ((Line)_shape).X2 = ((Line)_shape).X2 + x_difference;
        }

        public static Line Draw(double X, double Y)
        {
            Line line = new Line();
            Canvas.SetTop(line, Y);
            Canvas.SetLeft(line, X);
            line.X1 = 0;
            line.Y1 = 0;
            line.StrokeThickness = 200;
            line.Fill = Brushes.White;
            line.Stroke = Brushes.Black;

            return line;
        }

        public virtual void MoveShape(double x_difference, double y_difference)
        {
            Line line = (Line)_shape;
            double new_left = Canvas.GetLeft(_shape) + (x_difference);
            double new_top = Canvas.GetTop(_shape) + (y_difference);
            if (new_left <= ((Canvas)_shape.Parent).Width - (line.X2 - line.X1) && new_left >= 0)
            {
                Canvas.SetLeft(_shape, new_left);
                Canvas.SetLeft(_boundingBox, Canvas.GetLeft(_boundingBox) + (x_difference));
            }
            if (new_top <= ((Canvas)_shape.Parent).Height - (line.Y2 - line.Y2) && new_top >= 0)
            {
                Canvas.SetTop(_shape, new_top);
                Canvas.SetTop(_boundingBox, Canvas.GetTop(_boundingBox) + (y_difference));
            }
        }

        public override void UpdateBoundingBox()
        {
            Line line = (Line)_shape;
            _boundingBox.Height = Math.Abs(line.Y2 - line.Y1) + 1000;
            _boundingBox.Width = Math.Abs(line.X2 - line.X1) + 1000;
            if(line.Y2 - line.Y1 > 0)
                Canvas.SetTop(_boundingBox, Canvas.GetTop(_shape) - 500);
            else
                Canvas.SetTop(_boundingBox, Canvas.GetTop(_shape) - 500 - Math.Abs(line.Y2 - line.Y1));
            if(line.X2 - line.X1 > 0)
                Canvas.SetLeft(_boundingBox, Canvas.GetLeft(_shape) - 500);
            else
                Canvas.SetLeft(_boundingBox, Canvas.GetLeft(_shape) - 500 - Math.Abs(line.X2 - line.X1));
        }

        /// <summary>
        /// Get a list of points representing the rectangle.
        /// The number of points in the list is equal to RESOLUTION.
        /// </summary>
        /// <returns>List of points on the rectangle</returns>
        protected override List<Point> GetPointsHelper(double duration)
        {
            List<Point> points = new List<Point>();
            //double pointsPerUnit;
            double distanceBetweenPoints;

            //pointsPerUnit = GetResolution(duration) / Length;
            distanceBetweenPoints = Length / (GetResolution(duration) - 1);
            double pointsOnLine = GetResolution(duration); // pointsPerUnit* Length;
            
            double xDelta = Point2.X - Point1.X;
            double yDelta = Point2.Y - Point1.Y;

            for (int i = 0; i < pointsOnLine; i++)
            {
                double x = Position.X + (i * distanceBetweenPoints) * (xDelta / Length);
                double y = Position.Y + (i * distanceBetweenPoints) * (yDelta / Length);
                points.Add(new Point(x, y));
            }

            return points;
        }

        #endregion

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            
            info.AddValue("X1", ((Line)_shape).X1);
            info.AddValue("X2", ((Line)_shape).X2);
            info.AddValue("Y1", ((Line)_shape).Y1);
            info.AddValue("Y2", ((Line)_shape).Y2);
        }
    }
}
