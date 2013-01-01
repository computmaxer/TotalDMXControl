using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;

namespace Total_DMX_Control_WPF
{
    class RoutineStepPlayer
    {
        #region Data Members

        #endregion

        #region Properties

        #endregion

        #region Methods
        public RoutineStepPlayer()
        {
            
        }
        
        public void PlayStep(Fixture fixture, RoutineStep step)
        {
            List<Point> movementPoints = step.GetPoints();
            List<AttributePoint> attributePoints = step.GetAttributePoints(movementPoints, fixture);
            double timePerPoint = step.Duration / movementPoints.Count;

            _playStep(fixture, step, movementPoints, attributePoints, timePerPoint);
        }

        public void PreviewStep(Fixture fixture, RoutineStep step)
        {
            List<Point> movementPoints = step.GetPoints();
            List<AttributePoint> attributePoints = step.GetAttributePoints(movementPoints, fixture);
            double timePerPoint = step.Duration / movementPoints.Count;

            PlayStepAsynchCaller caller = new PlayStepAsynchCaller(_playStep);
            caller.BeginInvoke(fixture, step, movementPoints, attributePoints, timePerPoint, null, null);
        }

        private void _playStep(Fixture fixture, RoutineStep step, List<Point> movementPoints, List<AttributePoint> attributePoints, double timePerPoint)
        {
            for (int i = 0; i < step.RepeatCount; i++)
            {
                for (int j = 0; j < movementPoints.Count; j++)
                {
                    // First set the attribute values at this point
                    foreach (AttributePointSetting setting in attributePoints[j].AttributePointSettings)
                    {
                        setting.Attribute.SetLevel(setting.Value);
                    }

                    // Then tell the light to move
                    fixture.MoveTo(movementPoints[j], timePerPoint);
                }
            }
        }

        #endregion
    }
    public delegate void PlayStepAsynchCaller(Fixture fixture, RoutineStep step, List<Point> movementPoints, List<AttributePoint> attributePoints, double timePerPoint);
}
