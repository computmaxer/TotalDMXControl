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
    class StepEllipse : StepShape, ISerializable
    {
        
        #region Data Members

        #endregion

        #region Properties
        public Point Center
        {
            get
            {
                return new Point(Position.X + (Width / 2.0), Position.Y + (Height / 2.0));
            }
        }
        #endregion

        #region Methods

        public StepEllipse(Ellipse shape)
            :base(shape)
        {

        }

        public StepEllipse(SerializationInfo info, StreamingContext ctxt)
         : base(info, ctxt) 
        {
            Ellipse ell = new Ellipse();

            ell.Height = info.GetInt32("Height");
            ell.Width = info.GetInt32("Width");
            
            ell.StrokeThickness = 200;
            ell.Stroke = Brushes.Black;

            Point position = (Point)info.GetValue("Position", typeof(Point));
            Canvas.SetLeft(ell, position.X);
            Canvas.SetTop(ell, position.Y);

            _shape = ell;

            CreateBoundingBox();
        }

        public static Shape Draw(double X, double Y)
        {
            Ellipse ell = new Ellipse();
            ell.Height = 2000;
            ell.Width = 2000;
            Canvas.SetTop(ell, Y);
            Canvas.SetLeft(ell, X);
            ell.StrokeThickness = 200;
            //ell.Fill = Brushes.White;
            ell.Stroke = Brushes.Black;
            return ell;
        }

        /// <summary>
        /// Get a single point in the shape with the given t value.
        /// </summary>
        /// <param name="t">double</param>
        /// <returns>Point</returns>
        private Point _getPoint(double t)
        {
            double x = (Width / 2.0) * Math.Cos(t) + Center.X;
            double y = (Height / 2.0) * Math.Sin(t) + Center.Y;
            return new Point(x, y);
        }

        /// <summary>
        /// Get a list of points representing the ellipse.
        /// The number of points in the list is equal to RESOLUTION
        /// </summary>
        /// <returns>List of points on the ellipse</returns>
        protected override List<Point> GetPointsHelper(double duration)
        {
            List<Point> points = new List<Point>();

            int res = GetResolution(duration);
            double currentRadian = 0;  //TODO: Change when we know the starting point.
            for (int i = 0; i < res; i++)
            {
                points.Add(_getPoint(currentRadian += ((2 * Math.PI) / res)));
            }

            return points;
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Width", Width);
            info.AddValue("Height", Height);
        }
        #endregion
    }
}
