using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF
{
    [Serializable()]
    public class FixtureAttribute : ISerializable
    {
        #region Data Members


        private string _name;
        private List<Photon> _photons;
        private Fixture _parentFixture;
        private ATTRIBUTE_TYPE _type;
        private bool _snapOn;

        #endregion

        #region Properties
        // Get the name of the attribute
        public string Name
        {
            get { return _name; }
        }

        //Get a list of channels controlled by the attribute
        public List<Photon> Photons
        {
            get { return _photons; }
        }

        public ATTRIBUTE_TYPE Type
        {
            get { return _type; }
        }

        public bool SnapOn
        {
            get { return _snapOn; }
        }
        #endregion

        /*
         * CONSTRUCTORS
         * Allows you to define an attribute controlling either a single
         * channel, a list of channels, or a list of photons
         */
        public FixtureAttribute(ATTRIBUTE_TYPE Type, string Name, bool SnapOn, List<Photon> Photons, Fixture ParentFixture)
        {
            _type = Type;
            _name = Name;
            _snapOn = SnapOn;
            _photons = Photons;
            _parentFixture = ParentFixture;
        }

        public FixtureAttribute(ATTRIBUTE_TYPE Type, string Name, List<Photon> Photons, Fixture ParentFixture)
            : this(Type, Name, false, Photons, ParentFixture)
        { }

        public FixtureAttribute(ATTRIBUTE_TYPE Type, string Name, bool SnapOn, List<int> Channels, Fixture ParentFixture)
        {
            _type = Type;
            _name = Name;
            _snapOn = SnapOn;
            _photons = new List<Photon>();
            _parentFixture = ParentFixture;

            foreach (int chan in Channels)
            {
                _photons.Add(new Photon(chan, 255));
            }
        }

        public FixtureAttribute(ATTRIBUTE_TYPE Type, string Name, List<int> Channels, Fixture ParentFixture)
            : this(Type, Name, false, Channels, ParentFixture)
        { }

        public FixtureAttribute(ATTRIBUTE_TYPE Type, string Name, bool SnapOn, int Channel, Fixture ParentFixture)
        {
            _type = Type;
            _name = Name;
            _snapOn = SnapOn;
            _photons = new List<Photon>();
            _parentFixture = ParentFixture;

            _photons.Add(new Photon(Channel, 255));
        }

        public FixtureAttribute(ATTRIBUTE_TYPE Type, string Name, int Channel, Fixture ParentFixture)
            : this(Type, Name, false, Channel, ParentFixture)
        { }
        
        // To restore one from a file
        public FixtureAttribute(SerializationInfo info, StreamingContext ctxt)
        {
            _type = (ATTRIBUTE_TYPE)info.GetValue("Type", typeof(ATTRIBUTE_TYPE));
            _name = info.GetString("Name");
            _snapOn = info.GetBoolean("SnapOn");
            _photons = (List<Photon>)info.GetValue("Photons", typeof(List<Photon>));
            _parentFixture = (Fixture)info.GetValue("parent", typeof(Fixture));
        }

        public void SetLevel(int intensity, string sender)
        {
            if (_snapOn && intensity > 0)
                intensity = 255;

            foreach (Photon photon in _photons)
            {
                DMXController.SetLevel(sender, photon.Channel, (int)(intensity * photon.Intensity / 255.0));
            }
        }

        public void SetLevel(int intensity)
        {
            SetLevel(intensity, ("fix." + _parentFixture.Name + " (" + _name + ")"));
        }

        public void SetOverride(bool overrideHTP)
        {
            foreach (Photon photon in _photons)
            {
                DMXController.SetOverride(photon.Channel, overrideHTP);
            }
        }

        public int GetLevel()
        {
            // A bit of a hack; we reverse engineer the level based on one photon
            // (might be weird if some of these channels were set by a different source)
            
            // Get channel's level
            int dmxValue = DMXController.GetLevel(_photons[0].Channel);

            // Return the level scaled by the photon's intensity
            // (the inverse of what we did in SetLevel)
            return (dmxValue * 255) / _photons[0].Intensity;
        }

        public List<Photon> GetPhotonsAtLevel(int level)
        {
            List<Photon> newPhotons = new List<Photon>();

            foreach (Photon p in _photons)
            {
                newPhotons.Add(new Photon(p.Channel, p.Intensity * (level / 255)));
            }

            return newPhotons;
        }


        /*
         * Basic BUMP methods.  Sets the Attribute's channels to full or zero.
         */
        public void On()
        {
            SetLevel(255);
        }

        public void Off()
        {
            SetLevel(0);
        }

        public void Off(string sender)
        {
            SetLevel(0, sender);
        }

        public override string ToString()
        {
            return _name + " (" + _parentFixture.Name + ")";
        }

        // For serialization
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Type", _type);
            info.AddValue("Name", _name);
            info.AddValue("SnapOn", _snapOn);
            info.AddValue("Photons", _photons);
            info.AddValue("parent", _parentFixture);
        }
    }

    public enum ATTRIBUTE_TYPE
    {
        PAN_COARSE,
        PAN_FINE,
        TILT_COARSE,
        TILT_FINE,
        COLOR,
        GOBO_WHEEL,
        ROTATING_GOBO_WHEEL,
        GOBO_ROTATION,
        INTENSITY,
        LAMP,
        MODE,
        SHUTTER,
        FOCUS,
        PRISM,
        OTHER
    }
}
