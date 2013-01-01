using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Threading;

namespace Total_DMX_Control_WPF
{
    /*
     * This subclass of Fixture is meant for 10 Stellar Labs LED PAR 56 cans, in a line.
     * The personnality is as follows:
     * Channel 1 is a mode control.
     * Channel 2 is Red intensity.
     * Channel 3 is Green intensity.
     * Channel 4 is Blue intensity.
     * Channel 5 is speed select for built in effects or allows you to choose sound activation.
     */
    [Serializable()]
    class LedBackdropFixture : Fixture, ISerializable
    {
        #region Data Members

        private const int NUM_CHANNELS = 30;
        private List<List<Photon>> colors;
        #endregion

        #region Properties

        #endregion

        /*
         * Constructs a Fixture with the given information.  The IntensityChannel in the superclass is
         * getting set as the second channel, which controls Red intensity. It's unlikely that we will
         * ever use that on this fixture anyway.
         */
        public LedBackdropFixture(string name, int startChannel)
            : base(name, startChannel, startChannel + NUM_CHANNELS - 1)
        {
            // Create attributes
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Red", startChannel, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Green", startChannel + 1, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Blue", startChannel + 2, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Pink", new List<int>() { startChannel, startChannel + 2 }, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Teal", new List<int>() { startChannel + 1, startChannel + 2 }, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Orange", new List<int>() { startChannel, startChannel + 1 }, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "White", new List<int>() { startChannel, startChannel + 1, startChannel + 2 }, this));

            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Red2", startChannel +3, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Green2", startChannel + 4, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Blue2", startChannel + 5, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Pink2", new List<int>() { startChannel + 3, startChannel + 5 }, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Teal2", new List<int>() { startChannel + 4, startChannel + 5 }, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Orange2", new List<int>() { startChannel + 3, startChannel + 4 }, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "White2", new List<int>() { startChannel + 3, startChannel + 4, startChannel + 5 }, this));

            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Red3", startChannel + 6, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Green3", startChannel + 7, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Blue3", startChannel + 8, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Pink3", new List<int>() { startChannel + 6, startChannel + 8 }, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Teal3", new List<int>() { startChannel + 7, startChannel + 8 }, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Orange3", new List<int>() { startChannel + 6, startChannel + 7 }, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "White3", new List<int>() { startChannel + 6, startChannel + 7, startChannel + 8 }, this));

            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Red4", startChannel + 9, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Green4", startChannel + 10, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Blue4", startChannel + 11, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Pink4", new List<int>() { startChannel + 9, startChannel + 11 }, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Teal4", new List<int>() { startChannel + 10, startChannel + 11 }, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Orange4", new List<int>() { startChannel + 9, startChannel + 10 }, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "White4", new List<int>() { startChannel + 9, startChannel + 10, startChannel + 11 }, this));

            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Red5", startChannel + 12, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Green5", startChannel + 13, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Blue5", startChannel + 14, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Pink5", new List<int>() { startChannel + 12, startChannel + 14 }, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Teal5", new List<int>() { startChannel + 13, startChannel + 14 }, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Orange5", new List<int>() { startChannel + 12, startChannel + 13 }, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "White5", new List<int>() { startChannel + 12, startChannel + 13, startChannel + 14 }, this));

            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Red6", startChannel + 15, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Green6", startChannel + 16, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Blue6", startChannel + 17, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Pink6", new List<int>() { startChannel + 15, startChannel + 17 }, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Teal6", new List<int>() { startChannel + 16, startChannel + 17 }, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Orange6", new List<int>() { startChannel + 15, startChannel + 16 }, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "White6", new List<int>() { startChannel + 15, startChannel + 16, startChannel + 17 }, this));

            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Red7", startChannel + 18, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Green7", startChannel + 19, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Blue7", startChannel + 20, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Pink7", new List<int>() { startChannel + 18, startChannel + 20 }, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Teal7", new List<int>() { startChannel + 19, startChannel + 20 }, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Orange7", new List<int>() { startChannel + 18, startChannel + 19 }, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "White7", new List<int>() { startChannel + 18, startChannel + 19, startChannel + 20 }, this));

            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Red8", startChannel + 21, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Green8", startChannel + 22, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Blue8", startChannel + 23, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Pink8", new List<int>() { startChannel + 21, startChannel + 23 }, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Teal8", new List<int>() { startChannel + 22, startChannel + 23 }, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Orange8", new List<int>() { startChannel + 21, startChannel + 22 }, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "White8", new List<int>() { startChannel + 21, startChannel + 22, startChannel + 23 }, this));

            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Red9", startChannel + 24, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Green9", startChannel + 25, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Blue9", startChannel + 26, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Pink9", new List<int>() { startChannel + 24, startChannel + 26 }, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Teal9", new List<int>() { startChannel + 25, startChannel + 26 }, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Orange9", new List<int>() { startChannel + 24, startChannel + 25 }, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "White9", new List<int>() { startChannel + 24, startChannel + 25, startChannel + 26 }, this));

            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Red10", startChannel + 27, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Green10", startChannel + 28, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Blue10", startChannel + 29, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Pink10", new List<int>() { startChannel + 27, startChannel + 29 }, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Teal10", new List<int>() { startChannel + 28, startChannel + 29 }, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Orange10", new List<int>() { startChannel + 27, startChannel + 28 }, this));
            //attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "White10", new List<int>() { startChannel + 27, startChannel + 28, startChannel + 29 }, this));

            List<Photon> red = new List<Photon>();
            List<Photon> orange = new List<Photon>();
            List<Photon> green = new List<Photon>();
            List<Photon> teal = new List<Photon>();
            List<Photon> blue = new List<Photon>();
            List<Photon> pink = new List<Photon>();

            foreach (FixtureAttribute attr in Attributes)
            {
                if (attr.Type == ATTRIBUTE_TYPE.COLOR)
                {
                    if (attr.Name.Contains("Red")) red.Add(attr.Photons[0]);
                    if (attr.Name.Contains("Orange")) orange.Add(attr.Photons[0]);
                    if (attr.Name.Contains("Green")) green.Add(attr.Photons[0]);
                    if (attr.Name.Contains("Teal")) teal.Add(attr.Photons[0]);
                    if (attr.Name.Contains("Blue")) blue.Add(attr.Photons[0]);
                    if (attr.Name.Contains("Pink")) pink.Add(attr.Photons[0]);
                }
            }

            colors = new List<List<Photon>>();
            colors.Add(red);
            //colors.Add(orange);
            colors.Add(green);
            //colors.Add(teal);
            colors.Add(blue);
            //colors.Add(pink);
        }

        // To restore one from a file
        public LedBackdropFixture(SerializationInfo info, StreamingContext ctxt)
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


        List<Fader> colorRotationFaders = new List<Fader>();
        Thread _colorRotationThread;

        public void colorRotation(double fadeTime){
            _colorRotationThread = new Thread(new ParameterizedThreadStart(colorRotationHelper));
            _colorRotationThread.Start(fadeTime);
        }

        private void colorRotationHelper(object parameter)
        {
            double fadeTime = (double)parameter;
            bool firstTime = true;

            while (true)
            {
                for (int i = 0; i < colors.Count; i++)
                {
                    List<Photon> prev;
                    if (i == 0)
                        prev = colors[colors.Count - 1];
                    else
                        prev = colors[i - 1];

                    Fader colorFader = new Fader(Name + "colorRotation", colors[i], 0, 255, fadeTime);
                    colorRotationFaders.Add(colorFader);
                    lock (c_lock)
                    {
                        colorFader.Run(new FaderDoneCallback(DoneFading));
                        Monitor.Wait(c_lock);
                    }

                    if (firstTime)
                    {
                        firstTime = false;
                        continue;
                    }
                    
                    Fader prevFader = new Fader(Name + "colorRotation", prev, 255, 0, fadeTime);
                    colorRotationFaders.Add(prevFader);
                    lock (c_lock)
                    {
                        prevFader.Run(new FaderDoneCallback(DoneFading));
                        Monitor.Wait(c_lock);
                    }

                }
            }
        }

        public void StopColorRotation()
        {
            _colorRotationThread.Abort();
            foreach (Fader f in colorRotationFaders)
            {
                f.Kill();
            }
        }
        
        private readonly object c_lock = new object();
        public void DoneFading()
        {
            lock (c_lock)
                Monitor.Pulse(c_lock);
        }


        // For serialization
        public new void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            base.GetObjectData(info, ctxt);
        }
    }
}
