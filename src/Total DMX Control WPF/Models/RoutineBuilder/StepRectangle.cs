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
    class StepRectangle:StepShape, ISerializable
    {
        #region Data Members

        #endregion
        public delegate double GetShapePerimeterDelegate(Shape shape);
        #region Properties
        public double Perimeter
        {
            get
            {
                GetShapeLengthDelegate caller = new GetShapeLengthDelegate(GetShapePerimeter);
                return (double)this.Shape.Dispatcher.Invoke(caller, new object[] { this.Shape });
            }
        }

        private double GetShapePerimeter(Shape shape)
        {
            return (2 * shape.Width) + (2 * shape.Height);
        }
        #endregion

        #region Methods

        public StepRectangle(Rectangle shape)
            :base(shape)
        {

        }

        public StepRectangle(SerializationInfo info, StreamingContext ctxt)
         : base(info, ctxt) 
        {
            Rectangle rect = new Rectangle();

            rect.Width = info.GetInt32("Width");
            rect.Height = info.GetInt32("Height");

            rect.StrokeThickness = 200;
            rect.Stroke = Brushes.Black;

            Point position = (Point)info.GetValue("Position", typeof(Point));
            Canvas.SetLeft(rect, position.X);
            Canvas.SetTop(rect, position.Y);

            _shape = rect;

            CreateBoundingBox();
        }

        public static Rectangle Draw(double X, double Y)
        {
            Rectangle rect = new Rectangle();
            rect.Height = 2000;
            rect.Width = 2000;
            Canvas.SetTop(rect, Y);
            Canvas.SetLeft(rect, X);
            rect.StrokeThickness = 200;
            //rect.Fill = Brushes.White;
            rect.Stroke = Brushes.Black;

            return rect;
        }


        /// <summary>
        /// Get a list of points representing the rectangle.
        /// The number of points in the list is equal to RESOLUTION.
        /// </summary>
        /// <returns>List of points on the rectangle</returns>
        protected override List<Point> GetPointsHelper(double duration)
        {
            List<Point> points = new List<Point>();

            double pointsPerUnit = GetResolution(duration) / Perimeter;
            double distanceBetweenPoints = Perimeter / GetResolution(duration);
            double pointsOnWidth = pointsPerUnit * Width;
            double pointsOnHeight = pointsPerUnit * Height;

            //Add points for top edge of rectangle
            for (int i = 0; i < pointsOnWidth; i++)
            {
                points.Add(new Point(Position.X + (i * distanceBetweenPoints), Position.Y));
            }
            //Add points for right edge of rectangle
            for (int i = 0; i < pointsOnHeight; i++)
            {
                points.Add(new Point(Position.X + Width, Position.Y + (i * distanceBetweenPoints)));
            }
            //Add points for the bottom of rectangle
            for (int i = 0; i < pointsOnWidth; i++)
            {
                points.Add(new Point((Position.X + Width) - (i * distanceBetweenPoints), Position.Y + Height));
            }
            //Add points for the left edge of the rectangle
            for (int i = 0; i < pointsOnHeight; i++)
            {
                points.Add(new Point(Position.X, (Position.Y + Height) - (i * distanceBetweenPoints)));
            }

            return points;
        }
        #endregion

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Width", Width);
            info.AddValue("Height", Height);
        }
    }
}
