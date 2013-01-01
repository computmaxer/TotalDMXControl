using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Runtime.Serialization;
using System.Threading;
using System.Diagnostics;

namespace Total_DMX_Control_WPF
{
    public delegate List<Point> GetPointCollectionDelegate(Polyline shape);

    [Serializable()]
    public class StepFreeForm : StepShape, ISerializable
    {
        #region Data Members

        private double xDiff;
        private double yDiff;

        #endregion

        #region Properties

        public List<Point> PolyPoints
        {
            get
            {
                if (_shape.Dispatcher.Thread == Thread.CurrentThread)
                {
                    return GetPointCollection((Polyline)_shape);
                }
                else
                {
                    GetPointCollectionDelegate caller = new GetPointCollectionDelegate(GetPointCollection);
                    return (List<Point>)_shape.Dispatcher.Invoke(caller, new object[] { _shape });
                }
            }
        }

        private List<Point> GetPointCollection(Polyline polyline)
        {
            return polyline.Points.ToList<Point>();
        }

        #endregion

        #region Methods

        public StepFreeForm(double X, double Y)
        {
            _shape = Draw(X, Y);
            xDiff = 0;
            yDiff = 0;
        }

        public StepFreeForm(SerializationInfo info, StreamingContext ctxt)
         : base(info, ctxt) 
        {
            Polyline line = new Polyline();

            line.Points = new PointCollection((List<Point>)info.GetValue("Points", typeof(List<Point>)));

            line.StrokeThickness = 200;
            line.Fill = Brushes.Transparent;
            line.Stroke = Brushes.Black;

            _shape = line;

            CreateBoundingBox();
        }

        public Polyline Draw(double X, double Y)
        {
            Polyline line = new Polyline();
            //Canvas.SetTop(line, Y);
            //Canvas.SetLeft(line, X);
            line.Points.Add(new Point(X, Y));
            line.StrokeThickness = 200;
            line.Fill = Brushes.Transparent;
            line.Stroke = Brushes.Black;
            return line;
        }

        public override void DrawMouseMove(double x_difference, double y_difference)
        {
            
            Point previous = ((Polyline)_shape).Points.Last<Point>();
            if (Utilities.GetDistanceBetweenPoints(previous, new Point(previous.X + x_difference + xDiff, previous.Y + y_difference + yDiff)) > 1000)
            {
                ((Polyline)_shape).Points.Add(new Point(previous.X + x_difference + xDiff, previous.Y + y_difference + yDiff));
                xDiff = 0;
                yDiff = 0;
            }
            else
            {
                xDiff += x_difference;
                yDiff += y_difference;
            }
            
        }

        public override void UpdateBoundingBox()
        {
            //_boundingBox.Height = _shape.Height + 1000;
            //_boundingBox.Width = _shape.Width + 1000;
            //Canvas.SetTop(_boundingBox, Canvas.GetTop(_shape) - 500);
            //Canvas.SetLeft(_boundingBox, Canvas.GetLeft(_shape) - 500);
        }

        /// <summary>
        /// Get a list of points representing the rectangle.
        /// The number of points in the list is equal to RESOLUTION.
        /// </summary>
        /// <returns>List of points on the rectangle</returns>
        protected override List<Point> GetPointsHelper(double duration)
        {
            return PolyPoints;
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Points", ((Polyline)_shape).Points.ToList<Point>());
        }
        #endregion
    }
}
