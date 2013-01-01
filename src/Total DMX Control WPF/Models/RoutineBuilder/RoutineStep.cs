using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF
{
    [Serializable()]
    public class RoutineStep : ISerializable
    {

        #region Data Members
        private string _name;
        private StepShape _stepShape;
        private List<AttributePoint> _attributePoints;
        private double _duration;
        private int _repeatCount;
        private bool _visible;

        private Routine _parentRoutine;
        #endregion

        #region Properties
        public StepShape StepShape
        {
            get { return _stepShape; }
            set { _stepShape = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public double Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }
        public int RepeatCount
        {
            get { return _repeatCount; }
            set { _repeatCount = value; }
        }
        public bool Visible
        {
            get { return _visible; }
            set {
                if (value)
                {
                    ShowDimmed();
                }
                else
                {
                    _visible = value; //looks weird, but trust me.
                    Hide();
                }
                _visible = value;
            }
        }
        public List<AttributePoint> AttrPointsOnlyUseForLoopingThroughNotAdding
        {
            get { return _attributePoints; }
        }
        #endregion

        #region Methods
        public RoutineStep(string name, StepShape stepShape, Routine parentRoutine)
        {
            _name = name;
            _stepShape = stepShape;
            //Default times
            _visible = false;
            _duration = 5;
            _repeatCount = 1;
            _attributePoints = new List<AttributePoint>();
            _parentRoutine = parentRoutine;
        }

        public RoutineStep(SerializationInfo info, StreamingContext ctxt)
        {
            _name = info.GetString("_name");
            _stepShape = (StepShape)info.GetValue("_stepShape", typeof(StepShape));
            _attributePoints = (List<AttributePoint>)info.GetValue("_attributePoints", typeof(List<AttributePoint>));
            _duration = info.GetDouble("_duration");
            _repeatCount = info.GetInt32("_repeatCount");
            _visible = info.GetBoolean("_visible");
            _parentRoutine = (Routine)info.GetValue("_parentRoutine", typeof(Routine));
        }

        public List<Point> GetPoints()
        {
            return StepShape.GetPoints(_duration);
        }

        public List<AttributePoint> GetAttributePoints(List<Point> movementPoints, Fixture fixture)
        {
            // Make a new list of attribute points, one per movement point
            List<AttributePoint> attrPoints = new List<AttributePoint>();
            foreach (Point pt in movementPoints)
            {
                attrPoints.Add(new AttributePoint(new Point(pt.X, pt.Y), fixture));
            }

            // Assign each attribute point to its closest movement point
            foreach (AttributePoint attrPt in _attributePoints)
            {
                int closestMovementPointIndex = Utilities.GetClosestPointIndexInList(attrPt.Point, movementPoints);
                attrPoints[closestMovementPointIndex] = attrPt;
            }

            // If there isn't a key attribute point assigned to the first movement point, we don't
            // know what value each fixture attribute should start at. So, we ask each fixture
            // attribute what its current DMX value is
            foreach (AttributePointSetting setting in attrPoints[0].AttributePointSettings)
            {
                if (!setting.Active)
                {
                    // If the setting isn't active, get the fixture attribute's current value
                    setting.Value = setting.Attribute.GetLevel();
                }
            }

            // Now fill in the intermediate values for the movement points that
            // don't have a corresponding attribute point.
            // First, we keep track of the last key point for each attribute, currently 0
            Dictionary<string, int> lastKeyPointForAttribute = new Dictionary<string, int>();
            foreach (FixtureAttribute attr in fixture.Attributes)
            {
                lastKeyPointForAttribute[attr.Name] = 0;
            }
            // Next, step through the attribute points; whenever we hit a "key" point
            // (that is, a point that has an active setting), if we were supposed to
            // fade to that value, go back and fill in the intermediate values
            for (int i = 0; i < attrPoints.Count; i++)
            {
                AttributePoint attrPoint = attrPoints[i];

                foreach (AttributePointSetting setting in attrPoint.AttributePointSettings)
                {
                    if (setting.Active)
                    {
                        if (!setting.Snap)
                        {
                            // If we aren't supposed to snap to this value, that means
                            // we were supposed to have been fading to it. So we have 
                            // to go back and fill in the intermediate values between
                            // this AttributePoint and this attribute's last key point.
                            int lastKeyPointIndex = lastKeyPointForAttribute[setting.AttributeName];
                            int startVal = attrPoints[lastKeyPointIndex].GetSettingForAttributeName(setting.AttributeName).Value;
                            int endVal = setting.Value;
                            int incrementPerIntermediatePoint = (endVal - startVal) / (i - lastKeyPointIndex);

                            for (int j = lastKeyPointIndex + 1; j < i; j++)
                            {
                                AttributePointSetting intermediateSetting = attrPoints[j].GetSettingForAttributeName(setting.AttributeName);
                                intermediateSetting.Value = startVal + incrementPerIntermediatePoint * (j - lastKeyPointIndex);
                                intermediateSetting.Active = true;
                            }
                        }

                        // Since the setting is active, this is this attribute's
                        // most recent "key" attributePoint
                        lastKeyPointForAttribute[setting.AttributeName] = i;
                    }
                    else
                    {
                        // Since this is not a "key" point for this attribute, copy
                        // in the value from the last key point. It might turn out that
                        // we're supposed to be fading to the next key point here, but
                        // if this turns out to be the case, we'll come back and
                        // correct this point when we get the next key point

                        AttributePoint lastKeyPoint = attrPoints[lastKeyPointForAttribute[setting.AttributeName]];
                        setting.Value = lastKeyPoint.GetSettingForAttributeName(setting.AttributeName).Value;
                    }
                }
            }

            return attrPoints;
        }

        public void ShowDimmed()
        {
            _stepShape.Shadow();
            AddToCanvas(_parentRoutine.RoutineBuilder.Canvas);
        }

        public void Hide()
        {
            _stepShape.Unshadow();
            RemoveFromCanvas(_parentRoutine.RoutineBuilder.Canvas);
        }

        // I made this method instead of making the list of attribute
        // points accessible through a Property because at some point
        // we might care about carefully maintaining the list in a 
        // particular order, and we don't want to let other people change
        // that order.   --David
        public void AddAttributePoint(AttributePoint attrPoint)
        {
            _attributePoints.Add(attrPoint);
        }

        // Used when you need to get it off of the canvas no matter
        // what, visible or not (like when the routine builder is closing)
        public void DetachFromCanvas(Canvas canvas)
        {
            canvas.Children.Remove(_stepShape.Shape);
            if (canvas.Children.Contains(_stepShape.BoundingBox))
            {
                canvas.Children.Remove(_stepShape.BoundingBox);
            }
            foreach (AttributePoint point in _attributePoints)
            {
                canvas.Children.Remove(point.Ellipse);
            }
        }

        public void RemoveFromCanvas(Canvas canvas)
        {
            if (_visible)
            {
                _stepShape.Shadow();
            }
            else
            {
                DetachFromCanvas(canvas);
            }
        }

        public void AddToCanvas(Canvas canvas)
        {
            if (_visible)
            {
                _stepShape.Unshadow();
            }
            else
            {
                if(canvas.Children.Contains(_stepShape.Shape)) return;
                canvas.Children.Add(_stepShape.Shape);
                foreach (AttributePoint point in _attributePoints)
                {
                    canvas.Children.Add(point.Ellipse);
                }
            }
        }

        // For serialization
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("_name", _name);
            info.AddValue("_stepShape", _stepShape);
            info.AddValue("_attributePoints", _attributePoints);
            info.AddValue("_duration", _duration);
            info.AddValue("_repeatCount", _repeatCount);
            info.AddValue("_visible", _visible);
            info.AddValue("_parentRoutine", _parentRoutine);
        }

        //Temp
        public void LogAttrs()
        {
            int i = 1;
            foreach (AttributePoint point in _attributePoints)
            {
                Debug.Write("These are the attributes for point #" + i + "\n");
                foreach (AttributePointSetting setting in point.AttributePointSettings)
                {
                    Debug.Write(setting.AttributeName + " Active: " + setting.Active.ToString() + " Value: " + setting.Value.ToString() + "\n");
                }
                i++;
            }
        }
        #endregion
    }
}
