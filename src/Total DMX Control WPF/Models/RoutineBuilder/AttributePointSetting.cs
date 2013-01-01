using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF
{
    public delegate bool GetLivePreviewDelegate();

    [Serializable()]
    public class AttributePointSetting : ISerializable
    {
        #region Data Members

        public FixtureAttribute Attribute { get; private set; }
        public int _value;
        public bool Snap { get; set; }
        public bool _active;

        #endregion

        #region Properties

        public string AttributeName
        {
            get { return Attribute.Name; }
        }

        public ATTRIBUTE_TYPE AttributeType
        {
            get { return Attribute.Type; }
        }

        public int Value
        {
            get { return _value; }
            set {
                _value = value;
                if (WindowManager.CurrentViews.Contains(typeof(RoutineBuilder)))
                {
                    GetLivePreviewDelegate caller = new GetLivePreviewDelegate(GetLivePreview);
                    if ((bool)((RoutineBuilder)WindowManager.CurrentViews[typeof(RoutineBuilder)]).Dispatcher.Invoke(caller, null))
                    {
                        Attribute.SetLevel(_value, "LIVEPREVIEW");
                    }
                }
            }
        }
        private bool GetLivePreview()
        {
            try
            {
                return ((RoutineBuilder)WindowManager.CurrentViews[typeof(RoutineBuilder)]).LivePreview;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        public bool Fade
        {
            get { return !Snap; }
            set { Snap = !value; }
        }
                    
        #endregion

        #region Methods

        public AttributePointSetting(FixtureAttribute attribute) 
        {
            this.Attribute = attribute;
            this._value = 0;
            this.Snap = true;  // TODO: Should snap be on by default?
            this._active = false;
        }

        public AttributePointSetting(SerializationInfo info, StreamingContext ctxt)
        {
            this.Attribute = (FixtureAttribute)info.GetValue("Attribute", typeof(FixtureAttribute));
            _value = info.GetInt32("_value");
            this.Snap = info.GetBoolean("Snap");
            _active = info.GetBoolean("_active");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Attribute", this.Attribute);
            info.AddValue("_value", _value);
            info.AddValue("Snap", this.Snap);
            info.AddValue("_active", _active);
        }
        #endregion

        
    }
}
