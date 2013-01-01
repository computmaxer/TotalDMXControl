using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF
{
    [Serializable()]
    class ColorStripFixture : Fixture, ISerializable
    {
        private const int NUM_CHANNELS = 4;

        public ColorStripFixture(string name, int startChannel)
            :base(name, startChannel, startChannel + NUM_CHANNELS - 1)
        {
            
            // Create attributes
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Red", startChannel + 1, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Green", startChannel + 2, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Blue", startChannel + 3, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Pink", new List<int>() { startChannel + 1, startChannel + 3 }, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Teal", new List<int>() { startChannel + 2, startChannel + 3 }, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Orange", new List<int>() { startChannel + 1, startChannel + 2 }, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "White", new List<int>() { startChannel + 1, startChannel + 2, startChannel + 3 }, this));
        }

        // To restore one from a file
        public ColorStripFixture(SerializationInfo info, StreamingContext ctxt)
            :base(info, ctxt)
        {
            
        }

        public void StartColorStrip()
        {
            DMXController.SetLevel("static", startChannel, 215);
        }

        public void StopColorStrip()
        {
            DMXController.SetLevel("static", startChannel, 0);
        }

        /**
        * These methods allow you to suppress each channel individually instead of all of them together.
        * To suppress all, use the Suppress() and Unsuppress() found in the superclass.
        **/
        #region Suppression

        public void SuppressRed()
        {
            DMXController.Suppress(startChannel + 1);
        }

        public void SuppressGreen()
        {
            DMXController.Suppress(startChannel + 2);
        }

        public void SuppressBlue()
        {
            DMXController.Suppress(startChannel + 3);
        }

        public void UnsuppressRed()
        {
            DMXController.Unsuppress(startChannel + 1);
        }

        public void UnsuppressGreen()
        {
            DMXController.Unsuppress(startChannel + 2);
        }

        public void UnsuppressBlue()
        {
            DMXController.Unsuppress(startChannel + 3);
        }

        #endregion

        /**
         * Simple methods for setting each color channel. If no intensity is supplied, full is assumed.
         **/
        #region Color Setters

        public void Red(int intensity)
        {
            SetLevel(intensity, (startChannel + 1));
        }

        public void Red()
        {
            SetLevel(255, (startChannel + 1));
        }

        public void Green(int intensity)
        {
            SetLevel(intensity, (startChannel + 2));
        }

        public void Green()
        {
            SetLevel(255, (startChannel + 2));
        }

        public void Blue(int intensity)
        {
            SetLevel(intensity, (startChannel + 3));
        }

        public void Blue()
        {
            SetLevel(255, (startChannel +3));
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
