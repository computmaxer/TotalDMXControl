using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF
{
    [Serializable()]
    public class AttributePreset : ISerializable
    {
        #region Data Members

        private string _name;
        private ObservableCollection<AttributePresetSetting> _attributePointSettings;

        #endregion

        #region Properties

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public ObservableCollection<AttributePresetSetting> AttributePointSettings
        {
            get { return _attributePointSettings; }
        }

        #endregion

        #region Methods

        public AttributePreset()
        {
            _attributePointSettings = new ObservableCollection<AttributePresetSetting>();
            _name = "Un-named preset";
        }

        public AttributePreset(SerializationInfo info, StreamingContext ctxt)
        {
            _name = info.GetString("_name");
            _attributePointSettings = (ObservableCollection<AttributePresetSetting>)info.GetValue("_attributePointSettings", typeof(ObservableCollection<AttributePresetSetting>));
        }

        // For serialization
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("_name", _name);
            info.AddValue("_attributePointSettings", _attributePointSettings);
        }

        #endregion
    }
}
