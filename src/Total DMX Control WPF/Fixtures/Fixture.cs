using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows;
using System.Threading;
using System.Diagnostics;

namespace Total_DMX_Control_WPF
{
    [Serializable()]
    public class Fixture : ISerializable
    {

        public static string NextFixtureName()
        {
            // Default effect names are "Fixture #"
            // Note the following properties useful for recognizing default names:
            // 1) Contain a space
            // 2) Splitting string over spaces results in 2 substrings
            // 3) First substring must be "Fixture"
            // 4) Second substring must be an integer

            int biggestExistingDefaultFixture = 0;

            foreach (Fixture fixture in Controller.Fixtures)
            {
                string[] nameArray = fixture.Name.Split(' ');
                if (nameArray.Length == 2 && nameArray[0] == "Fixture")
                {
                    // If this really is a default name, figure out what number
                    // it is and save in effectNumber
                    int fixtureNumber = -1;
                    if (Int32.TryParse(nameArray[1], out fixtureNumber)
                        && fixtureNumber > biggestExistingDefaultFixture)
                    {
                        biggestExistingDefaultFixture = fixtureNumber;
                    }
                }
            }

            return "Fixture " + (biggestExistingDefaultFixture + 1);
        }

        public static int NextOpenChannel()
        {
            int biggestUsedChannel = 0;

            foreach (Fixture fixture in Controller.Fixtures)
            {
                if (fixture.EndChannel > biggestUsedChannel)
                {
                    biggestUsedChannel = fixture.EndChannel;
                }
            }

            return biggestUsedChannel + 1;
        }
        
        #region DataMembers
        //The first channel assigned to this fixture.
        protected int startChannel;
        //The last channel that this fixture uses. For fixture with 1 channel, this will be the same as startChannel.
        protected int endChannel;
        //The name of the fixture, used to refer to the object when it is added to the hashtable.
        protected string name;
        //The fixtures attributes; it's up to each subclass to create these
        protected List<FixtureAttribute> attributes;
        protected List<FixtureAttribute> _movingAttributes;
        //New stuff

        #endregion

        #region Properties
        //Get the name
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        //Get the starting channel of the fixture.
        public int StartChannel
        {
            get { return startChannel; }
           // set { startChannel = value; }
        }
        //Get the ending channel of the fixture.
        public int EndChannel
        {
            get { return endChannel; }
        }
        //Get a list of the fixture's attributes
        public List<FixtureAttribute> Attributes
        {
            get { return attributes; }
        }
        //
        public List<FixtureAttribute> MovingAttributes
        {
            get { return _movingAttributes; }
        }
        public bool HasPan
        {
            get
            {
                foreach (FixtureAttribute attr in MovingAttributes)
                {
                    if (attr.Type == ATTRIBUTE_TYPE.PAN_COARSE || attr.Type == ATTRIBUTE_TYPE.PAN_FINE)
                        return true;
                }
                return false;
            }
        }

        public bool HasTilt
        {
            get
            {
                foreach (FixtureAttribute attr in MovingAttributes)
                {
                    if (attr.Type == ATTRIBUTE_TYPE.TILT_COARSE || attr.Type == ATTRIBUTE_TYPE.TILT_FINE)
                        return true;
                }
                return false;
            }
        }

        public bool IsDimmable
        {
            get
            {
                foreach (FixtureAttribute attr in attributes)
                {
                    if (attr.Type == ATTRIBUTE_TYPE.INTENSITY && !attr.SnapOn)
                        return true;
                }
                return false;
            }
        }

        public List<Photon> OnOffChannels
        {
            get
            {
                List<Photon> photons = new List<Photon>();
                foreach (FixtureAttribute attr in attributes)
                {
                    if (attr.Type == ATTRIBUTE_TYPE.INTENSITY)
                        photons.AddRange(attr.Photons);
                    if (attr.Type == ATTRIBUTE_TYPE.SHUTTER)
                        photons.AddRange(attr.Photons);
                }
                return photons;
            }
        }

        public FixtureAttribute GetTiltCoarseAttr
        {
            get
            {
                foreach (FixtureAttribute attr in MovingAttributes)
                {
                    if (attr.Type == ATTRIBUTE_TYPE.TILT_COARSE)
                        return attr;
                }
                return null;
            }
        }

        public FixtureAttribute GetTiltFineAttr
        {
            get
            {
                foreach (FixtureAttribute attr in MovingAttributes)
                {
                    if (attr.Type == ATTRIBUTE_TYPE.TILT_FINE)
                        return attr;
                }
                return null;
            }
        }

        public FixtureAttribute GetPanCoarseAttr
        {
            get
            {
                foreach (FixtureAttribute attr in MovingAttributes)
                {
                    if (attr.Type == ATTRIBUTE_TYPE.PAN_COARSE)
                        return attr;
                }
                return null;
            }
        }

        public FixtureAttribute GetPanFineAttr
        {
            get
            {
                foreach (FixtureAttribute attr in MovingAttributes)
                {
                    if (attr.Type == ATTRIBUTE_TYPE.PAN_FINE)
                        return attr;
                }
                return null;
            }
        }
        #endregion

        /*
         * CONSTRUCTORS
         * Allows you to define fixture with a range of channels or just one channel.
         * Also allows you to specify an intensity channel, which is the one that will
         * be turned on by using this classes On() and Off() methods.  If no intensity
         * channel is defined, the startChannel will be assumed.
         */
        public Fixture(string name, int startChannel, int endChannel)
        {
            this.name = name;
            this.startChannel = startChannel;
            this.endChannel = endChannel;
            attributes = new List<FixtureAttribute>();
            _movingAttributes = new List<FixtureAttribute>();
        }

        public Fixture(string name, int channel)
            :this(name, channel, channel)
        {

        }

        // To restore one from a file
        public Fixture(SerializationInfo info, StreamingContext ctxt)
        {
            name = info.GetString("name");
            startChannel = info.GetInt32("startChannel");
            endChannel = info.GetInt32("endChannel");
            attributes = (List<FixtureAttribute>)info.GetValue("attributes", typeof(List<FixtureAttribute>));
            _movingAttributes = (List<FixtureAttribute>)info.GetValue("movingAttrs", typeof(List<FixtureAttribute>));
            Controller.Fixtures.Add(this);
        }

        public void AddAttribute(FixtureAttribute attr)
        {
            if (attr.Type == ATTRIBUTE_TYPE.PAN_COARSE || attr.Type == ATTRIBUTE_TYPE.PAN_FINE || attr.Type == ATTRIBUTE_TYPE.TILT_COARSE || attr.Type == ATTRIBUTE_TYPE.TILT_FINE)
            {
                MovingAttributes.Add(attr);
            }
            else
            {
                Attributes.Add(attr);
            }
        }

        /*
         * Generic level setter. Gets called from subclasses. 
         * Appends "fix." to the beginning of the senderName for HTP purposes.
         */
        public void SetLevel(int intensity, int channel)
        {
            DMXController.SetLevel(("fix." + name), channel, intensity);
        }

        /*
         * Get the level of the specified channel.
         */
        public int GetLevel(int channel)
        {
            return DMXController.GetLevel(channel);
        }

        /*
         * Suppress all channels that belong to a fixture.
         */
        public void Suppress()
        {
            for (int i = startChannel; i <= endChannel; i++)
            {
                DMXController.Suppress(i);
            }
        }

        /*
         * Unsuppress all channels that belong to a fixture.
         */
        public void Unsuppress()
        {
            for (int i = startChannel; i <= endChannel; i++)
            {
                DMXController.Unsuppress(i);
            }
        }

        /*
         * Sets all the Fixture Attributes to 0.
         */
        public void Reset()
        {
            foreach (FixtureAttribute attr in Attributes)
            {
                attr.SetLevel(0);
            }
            foreach (FixtureAttribute attr in MovingAttributes)
            {
                attr.SetLevel(0);
                attr.SetLevel(0, Name);
            }
        }

        /*
         * Basic BUMP methods.  Sets the Fixture's intensity channel to full or zero.
         */
        public virtual void On()
        {
            On(Name);
        }
        
        public virtual void On(string sender)
        {
            DMXController.SetLevels(sender, OnOffChannels, 255);
        }

        public virtual void Off()
        {
            Off(Name);
        }

        public virtual void Off(string sender)
        {
            DMXController.SetLevels(sender, OnOffChannels, 0);
        }

        public void MoveTo(Point point, double fadeTime)
        {
            MoveTo(point, fadeTime, Name);
        }

        private readonly object m_lock = new object();
        public void MoveTo(Point point, double fadeTime, string sender)
        {
            if (!HasPan || !HasTilt)
                throw new FixtureNotMovableException();

            if (fadeTime == 0)
            {
                int tiltCoarse = (int)point.Y / 256;
                int tiltFine = (int)point.Y % 256;
                int panCoarse = (int)point.X / 256;
                int panFine = (int)point.X % 256;
                DMXController.SetLevel(Name, GetTiltCoarseAttr.Photons[0].Channel, tiltCoarse);
                DMXController.SetLevel(Name, GetTiltFineAttr.Photons[0].Channel, tiltFine);
                DMXController.SetLevel(Name, GetPanCoarseAttr.Photons[0].Channel, panCoarse);
                DMXController.SetLevel(Name, GetPanFineAttr.Photons[0].Channel, panFine);
            }
            else
            {

                //Tilt
                PanTiltCoarseFineFader fader = new PanTiltCoarseFineFader(sender, GetTiltCoarseAttr.Photons[0].Channel, GetTiltFineAttr.Photons[0].Channel, (int)point.Y, GetPanCoarseAttr.Photons[0].Channel, GetPanFineAttr.Photons[0].Channel, (int)point.X, fadeTime);

                lock (m_lock)
                {
                    fader.Run(new FaderDoneCallback(DoneMoving));
                    Monitor.Wait(m_lock);
                }
            }
        }

        public void DoneMoving()
        {
            lock (m_lock)
                Monitor.Pulse(m_lock);
        }

        public override string ToString()
        {
            return this.name;
        }

        // For serialization
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("startChannel", startChannel);
            info.AddValue("endChannel", endChannel);
            info.AddValue("name", name);
            info.AddValue("attributes", attributes);
            info.AddValue("movingAttrs", _movingAttributes);
        }
    }

    public class FixtureAttributeException : Exception { }
    public class FixtureNotMovableException : FixtureAttributeException { }
    public class FixtureNotDimmable : FixtureAttributeException { }
}
