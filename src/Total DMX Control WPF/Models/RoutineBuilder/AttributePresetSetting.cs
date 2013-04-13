using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF
{
    /// <summary>
    /// Very similar to AttributePointSetting, but used generically and doesn't take in an actuall FixtureAttribute
    /// </summary>
    [Serializable()]
    public class AttributePresetSetting : ISerializable
    {
        #region Data Members

        private ATTRIBUTE_TYPE _type;
        private bool _active;
        private int _value;

        #endregion

        public ATTRIBUTE_TYPE Type
        {
            get { return _type; }
        }
        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }
        public int Value
        {
            get { return _value; }
            set { 
                _value = value;
                if (WindowManager.CurrentViews.Contains(typeof(RoutinePlayerWindow)))
                {
                    ((RoutinePlayerWindow)WindowManager.CurrentViews[typeof(RoutinePlayerWindow)]).ActivateCurrentPreset(
                        ((RoutinePlayerWindow)WindowManager.CurrentViews[typeof(RoutinePlayerWindow)]).CurrentPreset);
                }
            }
        }

        #region Methods

        public AttributePresetSetting(ATTRIBUTE_TYPE type)
        {
            _type = type;
            _active = false;
            _value = 0;
        }

        public AttributePresetSetting(SerializationInfo info, StreamingContext ctxt)
        {
            _type = (ATTRIBUTE_TYPE)info.GetValue("_type", typeof(ATTRIBUTE_TYPE));
            _active = info.GetBoolean("_active");
            _value = info.GetInt32("_value");
        }

        // For serialization
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("_type", _type);
            info.AddValue("_active", _active);
            info.AddValue("_value", _value);
        }

        #endregion
    }
}
