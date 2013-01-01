using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Total_DMX_Control_WPF
{
    [Serializable()]
    public class LightingSystemConfiguration : ISerializable
    {
        //***** DATA MEMBERS *****//
        #region Data Members
        
        private string _name;

        private BindingList<Effect> _effects;
        private BindingList<Fixture> _fixtures;
        private BindingList<Routine> _routines;
        private BindingList<AttributePreset> _attributePresets;
        #endregion


        //***** PROPERTIES *****//
        #region Properties

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public BindingList<Effect> Effects
        {
            get { return _effects; }
        }

        public BindingList<Fixture> Fixtures
        {
            get { return _fixtures; }
        }

        public BindingList<Routine> Routines
        {
            get { return _routines; }
        }

        public BindingList<AttributePreset> AttributePresets
        {
            get { return _attributePresets; }
        }
        #endregion


        //***** CONSTRUCTORS *****//
        #region Constructors

        // To create a configuration from scratch
        public LightingSystemConfiguration(string Name)
        {
            _name = Name;

            _effects = new BindingList<Effect>();
            _fixtures = new BindingList<Fixture>();
            _routines = new BindingList<Routine>();
            _attributePresets = new BindingList<AttributePreset>();
        }

        // To restore one from a file
        public LightingSystemConfiguration(SerializationInfo info, StreamingContext ctxt)
        {
            _name = (string)info.GetValue("Name", typeof(string));
            _effects = (BindingList<Effect>)info.GetValue("EffectsList", typeof(BindingList<Effect>));
            _fixtures = (BindingList<Fixture>)info.GetValue("Fixtures", typeof(BindingList<Fixture>));
            _routines = (BindingList<Routine>)info.GetValue("Routines", typeof(BindingList<Routine>));
            _attributePresets = (BindingList<AttributePreset>)info.GetValue("AttributePresets", typeof(BindingList<AttributePreset>));
        }

        #endregion


        //***** METHODS *****//
        #region Methods

        // This saves the configuration to a file
        public void Serialize(string DirPath)
        {
            try
            {
                Stream stream = File.Open(DirPath + @"\" + _name + ".lsc", FileMode.Create);
                BinaryFormatter bFormatter = new BinaryFormatter();
                bFormatter.Serialize(stream, this);
                stream.Close();
            }
            catch (Exception ex)
            {
                Utilities.ReportError(ex);
            }
        }

        // For serialization
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Name", _name);
            info.AddValue("EffectsList", _effects);
            info.AddValue("Fixtures", _fixtures);
            info.AddValue("Routines", _routines);
            info.AddValue("AttributePresets", _attributePresets);
        }

        #endregion
    }
}
