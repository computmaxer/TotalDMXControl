using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF
{
    [Serializable()]
    class StepPath : StepShape, ISerializable
    {

        #region Data Members

        private ArcSegment _arcSeg;
        private PathFigure _pathFig;

        #endregion

        #region Properties


        #endregion


        #region Methods

        public StepPath(double X, double Y)
            : base()
        {
            _shape = Draw(X, Y);
        }

        public StepPath(SerializationInfo info, StreamingContext ctxt)
         : base(info, ctxt) 
        {
            //TODO
        }

        public Path Draw(double X, double Y)
        {
            Path path = new Path();
            Canvas.SetTop(path, Y);
            Canvas.SetLeft(path, X);
            //Make Geometry
            _arcSeg = new ArcSegment(new Point(X+800, Y+800), new Size(1000, 2000), 0.0, true, SweepDirection.Clockwise, true);
            PathSegmentCollection psc = new PathSegmentCollection(){_arcSeg};
            _pathFig = new PathFigure(new Point(X, Y), psc, false);
            PathFigureCollection pfc = new PathFigureCollection(){_pathFig};
            PathGeometry pgeo = new PathGeometry(pfc);
            path.Data = pgeo;
            
            path.StrokeThickness = 200;
            path.Fill = Brushes.White;
            path.Stroke = Brushes.Black;

            return path;
        }

        //Used when drawing line originally while mouse is held down. Not to move the object once it has been drawn.
        public override void DrawMouseMove(double x_difference, double y_difference)
        {
            _arcSeg.Point = new Point(_arcSeg.Point.X + x_difference, _arcSeg.Point.Y + y_difference);
        }

        public override void DrawStep2MouseMove(double x_difference, double y_difference)
        {
            if ((_arcSeg.Size.Width - (x_difference * 10)) < 32000 && (_arcSeg.Size.Width - (x_difference * 10)) > 0)
            {
                _arcSeg.Size = new Size(_arcSeg.Size.Width - (x_difference * 10), _arcSeg.Size.Height);
            }
            if ((_arcSeg.Size.Height - (y_difference * 10)) < 32000 && (_arcSeg.Size.Height - (y_difference * 10)) > 0)
            {
                _arcSeg.Size = new Size(_arcSeg.Size.Width, _arcSeg.Size.Height - (y_difference*10));
            }
            Debug.WriteLine(_arcSeg.Size);
        }

        public override void UpdateBoundingBox()
        {
            //_boundingBox.Height = _shape.Height + 1000;
            //_boundingBox.Width = _shape.Width + 1000;
            //Canvas.SetTop(_boundingBox, Canvas.GetTop(_shape) - 500);
            //Canvas.SetLeft(_boundingBox, Canvas.GetLeft(_shape) - 500);
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            //TODO
        }
        #endregion
    }
}
