using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF
{
    [Serializable()]
    public class BigPhoton : ISerializable
    {
        // DATA MEMBERS
        #region Data Members
        private FixtureAttribute _attribute;
        private int _level;
        #endregion


        // PROPERTIES
        #region Properties

        public FixtureAttribute Attribute
        {
            get { return _attribute; }
        }

        public int Level
        {
            get { return _level; }
            set { 
                if(0 <= value && value <= 255)
                    _level = value; 
            }
        }

        #endregion


        // CONSTRUCTORS
        public BigPhoton(FixtureAttribute fixtureAttribute, int level)
        {
            _attribute = fixtureAttribute;
            _level = level;
        }

        // To restore one from a file
        public BigPhoton(SerializationInfo info, StreamingContext ctxt)
        {
            _attribute = (FixtureAttribute)info.GetValue("FixtureAttribute", typeof(FixtureAttribute));
            _level = info.GetInt32("Level");
        }


        // For serialization
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("FixtureAttribute", _attribute);
            info.AddValue("Level", _level);
        }
    }
}
