using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF
{
    public struct RoboColor
    {
        public const int WHITE = 240;
        public const int FLAME_RED = 237;
        public const int LIGHT_BLUE = 234;
        public const int FRN_GREEN = 231;
        public const int YELLOW = 228;
        public const int GREEN = 225;
        public const int MAUVE = 222;
        public const int DARK_BLUE = 219;
        public const int CYAN_BLUE = 216;
        public const int RED = 213;
        public const int LIGHT_ORANGE = 210;
        public const int LIGHT_GREEN = 207;
        public const int AMBER = 204;
        public const int PINK = 201;
        public const int DARK_LAVENDER = 198;
        public const int DARK_ORANGE = 195;
        public const int MULTI_1 = 192;
        public const int MULTI_2 = 189;
    }

    public struct RoboGobo
    {
        public const int OPEN = 240;
        public const int HALF = 237;
        public const int DOT = 234;
        public const int PIN = 231;
        public const int VERT_BAR = 228;
        public const int HORIZ_BAR = 225;
        public const int CROSS = 222;
        public const int ARROW = 219;
        public const int TRIANGLES = 216;
        public const int STAR = 213;
        public const int HOLES = 210;
        public const int BELLS = 207;
        public const int CONE = 204;
        public const int CONES = 201;
        public const int PHONE = 198;
        public const int THIN_BARS = 195;
        public const int WINDOW = 192;
        public const int TURBINE = 189;
    }

    public struct RoboPreset
    {
        private int _pan, _tilt, _color, _gobo;

        public int Pan
        {
            get { return _pan; }
        }
        public int Tilt
        {
            get { return _tilt; }
        }
        public int Color
        {
            get { return _color; }
        }
        public int Gobo
        {
            get { return _gobo; }
        }

        public RoboPreset(int pan, int tilt, int color, int gobo)
        {
            _pan = pan;
            _tilt = tilt;
            _color = color;
            _gobo = gobo;
        }
    }

    [Serializable()]
    public class Roboscan218Fixture : Fixture, ISerializable
    {
        #region Data Members
        private const int NUM_CHANNELS = 8;

        private double currentPan = 0;
        private double currentTilt = 0;
        private int currentColor = RoboColor.WHITE;
        private int currentGobo = RoboGobo.OPEN;
        private RoboPreset[] presets = new RoboPreset[6];
        private bool fineMovement = false;

        private BindingList<FixtureAttribute> _focusPoints;
        #endregion

        #region Properties
        public bool FineMovement
        {
            get { return fineMovement; }
            set { fineMovement = value; }
        }

        public BindingList<FixtureAttribute> FocusPoints
        {
            get { return _focusPoints; }
        }
        #endregion

        public Roboscan218Fixture(string name, int startChannel)
            : base(name, startChannel, startChannel + NUM_CHANNELS - 1)
        {
            SetColor(RoboColor.WHITE);
            SetGobo(RoboGobo.OPEN);
            _focusPoints = new BindingList<FixtureAttribute>();
            // Create attributes
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.INTENSITY, "Intensity", startChannel + 1, this));
        }

        // To restore one from a file
        public Roboscan218Fixture(SerializationInfo info, StreamingContext ctxt)
            :base(info, ctxt)
        {
            _focusPoints = (BindingList<FixtureAttribute>)info.GetValue("FocusPoints", typeof(BindingList<FixtureAttribute>));
        }

        public void SetIntensity(int level)
        {
            DMXController.SetLevels(Name, OnOffChannels, level);
        }

        public void SetControl(int level)
        {
            SetLevel(level, startChannel);
        }

        public void SetColor(int level)
        {
            SetLevel(level, startChannel + 2);
            currentColor = level;
        }

        public void SetGobo(int level)
        {
            SetLevel(level, startChannel + 3);
            currentGobo = level;
        }

        public void Pan(int level)
        {
            int high = level / 256;
            int low = level % 256;
            SetLevel(high, startChannel + 4);
            SetLevel(low, startChannel + 5);
            currentPan = level;
        }

        public void Tilt(int level)
        {
            int high = level / 256;
            int low = level % 256;
            SetLevel(high, startChannel + 6);
            SetLevel(low, startChannel + 7);
            currentTilt = level;
        }

        public void NextColor()
        {
            if (currentColor <= RoboColor.MULTI_2)
            {
                currentColor = RoboColor.WHITE;
            }
            else
            {
                currentColor = currentColor - 3;
            }
            SetColor(currentColor);
        }

        public void PreviousColor()
        {
            if (currentColor >= RoboColor.WHITE)
            {
                currentColor = RoboColor.MULTI_2;
            }
            else currentColor = currentColor + 3;
            SetColor(currentColor);
        }

        public void NextGobo()
        {
            if (currentGobo <= RoboGobo.TURBINE)
            {
                currentGobo = RoboGobo.OPEN;
            }
            else currentGobo = currentGobo - 3;
            SetGobo(currentGobo);
        }

        public void PreviousGobo()
        {
            if (currentGobo >= RoboGobo.OPEN)
            {
                currentGobo = RoboGobo.TURBINE;
            }
            else currentGobo = currentGobo + 3;
            SetGobo(currentGobo);
        }

        public void SaveFocusPoint(string Name)
        {
            // Save a focus point as a new attribute
            // (and also add it to _focusPoints)

            List<Photon> thePhotons = new List<Photon>();
            int panCoarse = (int)currentPan / 256;
            int panFine = (int)currentPan % 256;
            int tiltCoarse = (int)currentTilt / 256;
            int tiltFine = (int)currentTilt % 256;

            thePhotons.Add(new Photon(startChannel + 4, panCoarse));
            thePhotons.Add(new Photon(startChannel + 5, panFine));
            thePhotons.Add(new Photon(startChannel + 6, tiltCoarse));
            thePhotons.Add(new Photon(startChannel + 7, tiltFine));
            thePhotons.Add(new Photon(startChannel + 2, currentColor));
            thePhotons.Add(new Photon(startChannel + 3, currentGobo));

            FixtureAttribute newFocusPoint = new FixtureAttribute(ATTRIBUTE_TYPE.OTHER, Name, thePhotons, this);
            attributes.Add(newFocusPoint);
            _focusPoints.Add(newFocusPoint);
        }

        public void ToggleFineMovement()
        {
            fineMovement = !fineMovement;
        }


        // For serialization
        public new void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            base.GetObjectData(info, ctxt);
            info.AddValue("FocusPoints", _focusPoints);
        }
    }
}
