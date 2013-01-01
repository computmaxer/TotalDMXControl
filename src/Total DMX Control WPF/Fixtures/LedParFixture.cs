using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF
{
    /*
     * This subclass of Fixture is meant for the 5-channel Stellar Labs LED PAR 56 can.
     * The personnality is as follows:
     * Channel 1 is a mode control.
     * Channel 2 is Red intensity.
     * Channel 3 is Green intensity.
     * Channel 4 is Blue intensity.
     * Channel 5 is speed select for built in effects or allows you to choose sound activation.
     */
    [Serializable()]
    class LedParFixture : Fixture, ISerializable
    {
        #region Data Members

        private const int NUM_CHANNELS = 3;

        #endregion

        #region Properties

        #endregion

        /*
         * Constructs a Fixture with the given information.  The IntensityChannel in the superclass is
         * getting set as the second channel, which controls Red intensity. It's unlikely that we will
         * ever use that on this fixture anyway.
         */
        public LedParFixture(string name, int startChannel)
            : base(name, startChannel, startChannel + NUM_CHANNELS - 1)
        {
            // Create attributes
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Red", startChannel, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Green", startChannel + 1, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Blue", startChannel + 2, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Pink", new List<int>() { startChannel, startChannel + 2 }, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Teal", new List<int>() { startChannel + 2, startChannel + 2 }, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Orange", new List<int>() { startChannel, startChannel + 1 }, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "White", new List<int>() { startChannel, startChannel + 1, startChannel + 2 }, this));
        }

        // To restore one from a file
        public LedParFixture(SerializationInfo info, StreamingContext ctxt)
            :base(info, ctxt)
        {

        }

        /**
         * These methods allow you to suppress each channel individually instead of all of them together.
         * To suppress all, use the Suppress() and Unsuppress() found in the superclass.
         **/
        #region Suppression

        public void SuppressRed()
        {
            DMXController.Suppress(startChannel);
        }

        public void SuppressGreen()
        {
            DMXController.Suppress(startChannel + 1);
        }

        public void SuppressBlue()
        {
            DMXController.Suppress(startChannel + 2);
        }

        public void UnsuppressRed()
        {
            DMXController.Unsuppress(startChannel);
        }

        public void UnsuppressGreen()
        {
            DMXController.Unsuppress(startChannel + 1);
        }

        public void UnsuppressBlue()
        {
            DMXController.Unsuppress(startChannel + 2);
        }

        #endregion

        /**
         * Simple methods for setting each color channel. If no intensity is supplied, full is assumed.
         **/
        #region Color Setters

        public void Red(int intensity)
        {
            SetLevel(intensity, (startChannel));
        }

        public void Red()
        {
            SetLevel(255, (startChannel));
        }

        public void Green(int intensity)
        {
            SetLevel(intensity, (startChannel + 1));
        }

        public void Green()
        {
            SetLevel(255, (startChannel + 1));
        }

        public void Blue(int intensity)
        {
            SetLevel(intensity, (startChannel + 2));
        }

        public void Blue()
        {
            SetLevel(255, (startChannel + 2));
        }

        public void Pink(int intensity)
        {
            Red(intensity);
            Blue(intensity);
        }

        public void Pink()
        {
            Red();
            Blue();
        }

        public void Teal(int intensity)
        {
            Blue(intensity);
            Green(intensity);
        }

        public void Teal()
        {
            Blue();
            Green();
        }

        public void Orange(int intensity)
        {
            Red(intensity);
            Green(intensity);
        }

        public void Orange()
        {
            Red();
            Green();
        }

        public void White(int intensity)
        {
            Red(intensity);
            Green(intensity);
            Blue(intensity);
        }

        public void White()
        {
            Red();
            Green();
            Blue();
        }

        public void AllOff()
        {
            White(0);
        }
        #endregion

        // For serialization
        public new void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            base.GetObjectData(info, ctxt);
        }
    }
}
