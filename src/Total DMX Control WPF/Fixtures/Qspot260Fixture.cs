using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF
{
    public struct QspotColor
    {
        public const int WHITE = 0;
        public const int RED = 25;
        public const int YELLOW = 42;
        public const int MAGENTA = 59;
        public const int GREEN = 76;
        public const int ORANGE = 93;
        public const int BLUE = 110;
        public const int LIGHTBLUE = 127;
        public const int LIGHTGREEN = 144;
    }

    public struct QspotFixedGobo
    {
        public const int NONE = 0;
        public const int G1 = 15;
        public const int G2 = 25;
        public const int G3 = 35;
        public const int G4 = 45;
        public const int G5 = 55;
        public const int G6 = 65;
        public const int G7 = 75;
        public const int G8 = 85;
        public const int G9 = 95;
        public const int S9 = 107; 
        public const int S8 = 122;
        public const int S7 = 137;
        public const int S6 = 152;
        public const int S5 = 167;
        public const int S4 = 182;
        public const int S3 = 197;
        public const int S2 = 212;
        public const int S1 = 227;
    }

    public struct QspotRotatingGobo
    {
        public const int NONE = 0;
        public const int G1 = 15;
        public const int G2 = 25;
        public const int G3 = 35;
        public const int G4 = 45;
        public const int G5 = 55;
        public const int G6 = 65;
        public const int G7 = 75;
        public const int S7 = 90;
        public const int S6 = 110;
        public const int S5 = 130;
        public const int S4 = 150;
        public const int S3 = 170;
        public const int S2 = 190;
        public const int S1 = 210;
    }

    public struct QspotControl
    {
        public const int DMX = 0;
        public const int BLACKACTIVATION = 30;
        public const int BLACKDEACTIVATION = 50;
        public const int AUTO1 = 70;
        public const int AUTO2 = 90;
        public const int SOUND1 = 110;
        public const int SOUND2 = 130;
        public const int CUSTOM = 150;
        public const int TEST = 170;
        public const int RESET = 210;
    }

    [Serializable()]
    public class Qspot260Fixture : Fixture, ISerializable
    {
        #region Data Members
        private const int NUM_CHANNELS = 14;
        private double currentPan = 0;
        private double currentTilt = 0;
        private int currentColor = QspotColor.WHITE;
        private int currentFixedGobo = QspotFixedGobo.NONE;
        private int currentRotatingGobo = QspotRotatingGobo.NONE;
        private RoboPreset[] presets = new RoboPreset[6];
        private bool fineMovement = false;
        //private Wheel colorWheel = new Wheel(new QspotColor());

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

        public Qspot260Fixture(string name, int startChannel)
            : base(name, startChannel, startChannel + NUM_CHANNELS - 1)
        {
            //SetColor(QspotColor.WHITE);
            //SetFixedGobo(QspotFixedGobo.NONE);

            _focusPoints = new BindingList<FixtureAttribute>();
            // Create attributes
            AddAttribute(new FixtureAttribute(ATTRIBUTE_TYPE.PAN_COARSE, "Pan Coarse", false, startChannel, this));
            AddAttribute(new FixtureAttribute(ATTRIBUTE_TYPE.PAN_FINE, "Pan Fine", false, startChannel + 1, this));
            AddAttribute(new FixtureAttribute(ATTRIBUTE_TYPE.TILT_COARSE, "Tilt Coarse", false, startChannel + 2, this));
            AddAttribute(new FixtureAttribute(ATTRIBUTE_TYPE.TILT_FINE, "Tilt Fine", false, startChannel + 3, this));
            //speed +4
            AddAttribute(new FixtureAttribute(ATTRIBUTE_TYPE.COLOR, "Color Wheel", false, startChannel + 5, this));
            AddAttribute(new FixtureAttribute(ATTRIBUTE_TYPE.GOBO_WHEEL, "Fixed Gobo Wheel", false, startChannel + 6, this));
            AddAttribute(new FixtureAttribute(ATTRIBUTE_TYPE.ROTATING_GOBO_WHEEL, "Rotating Gobo Wheel", false, startChannel + 7, this));
            AddAttribute(new FixtureAttribute(ATTRIBUTE_TYPE.GOBO_ROTATION, "Gobo Rotation", false, startChannel + 8, this));
            AddAttribute(new FixtureAttribute(ATTRIBUTE_TYPE.PRISM, "Prism", false, startChannel + 9, this));
            AddAttribute(new FixtureAttribute(ATTRIBUTE_TYPE.FOCUS, "Focus", false, startChannel + 10, this));
            AddAttribute(new FixtureAttribute(ATTRIBUTE_TYPE.INTENSITY, "Intensity", false, startChannel + 11, this));
            AddAttribute(new FixtureAttribute(ATTRIBUTE_TYPE.SHUTTER, "Shutter", false, startChannel + 12, this));
            AddAttribute(new FixtureAttribute(ATTRIBUTE_TYPE.MODE, "Control", false, startChannel + 13, this));
        }

        // To restore one from a file
        public Qspot260Fixture(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
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

        //public void SetColor(QspotColor color)
        //{
        //    colorWheel.Current = QspotColor.WHITE;
        //}

        //public void SetFixedGobo(QspotFixedGobo gobo)
        //{
        //    SetLevel((int)gobo, startChannel + 6);
        //    currentFixedGobo = gobo;
        //}

        //public void SetRotatingGobo(QspotRotatingGobo gobo)
        //{
        //    SetLevel((int)gobo, startChannel + 7);
        //    currentRotatingGobo = gobo;
        //}

        public void Pan(int level)
        {
            int high = level / 256;
            int low = level % 256;
            SetLevel(high, startChannel);
            SetLevel(low, startChannel + 1);
            currentPan = level;
        }

        public void Tilt(int level)
        {
            int high = level / 256;
            int low = level % 256;
            SetLevel(high, startChannel + 2);
            SetLevel(low, startChannel + 3);
            currentTilt = level;
        }

        //public void NextColor()
        //{
        //    Array values = Enum.GetValues(typeof(QspotColor));
        //    if (currentColor == QspotColor.LIGHTBLUE)
        //    {
        //        currentColor = QspotColor.WHITE;
        //    }
        //    else
        //    {
        //        (QspotColor)0;
        //    }
        //    SetColor(currentColor);
        //}

        //public void PreviousColor()
        //{
        //    if (currentColor >= RoboColor.WHITE)
        //    {
        //        currentColor = RoboColor.MULTI_2;
        //    }
        //    else currentColor = currentColor + 3;
        //    SetColor(currentColor);
        //}

        //public void NextGobo()
        //{
        //    if (currentGobo <= RoboGobo.TURBINE)
        //    {
        //        currentGobo = RoboGobo.OPEN;
        //    }
        //    else currentGobo = currentGobo - 3;
        //    SetGobo(currentGobo);
        //}

        //public void PreviousGobo()
        //{
        //    if (currentGobo >= RoboGobo.OPEN)
        //    {
        //        currentGobo = RoboGobo.TURBINE;
        //    }
        //    else currentGobo = currentGobo + 3;
        //    SetGobo(currentGobo);
        //}

        //public void SaveFocusPoint(string Name)
        //{
        //    // Save a focus point as a new attribute
        //    // (and also add it to _focusPoints)

        //    List<Photon> thePhotons = new List<Photon>();
        //    int panCoarse = (int)currentPan / 256;
        //    int panFine = (int)currentPan % 256;
        //    int tiltCoarse = (int)currentTilt / 256;
        //    int tiltFine = (int)currentTilt % 256;

        //    thePhotons.Add(new Photon(startChannel + 4, panCoarse));
        //    thePhotons.Add(new Photon(startChannel + 5, panFine));
        //    thePhotons.Add(new Photon(startChannel + 6, tiltCoarse));
        //    thePhotons.Add(new Photon(startChannel + 7, tiltFine));
        //    thePhotons.Add(new Photon(startChannel + 2, currentColor));
        //    thePhotons.Add(new Photon(startChannel + 3, currentGobo));

        //    FixtureAttribute newFocusPoint = new FixtureAttribute(Name, thePhotons, this);
        //    attributes.Add(newFocusPoint);
        //    _focusPoints.Add(newFocusPoint);
        //}

        //public void RecordPreset(int num)
        //{
        //    switch (num)
        //    {
        //        case JoyButtons.PRESET7:
        //            presets[0] = new RoboPreset((int)currentPan, (int)currentTilt, currentColor, currentGobo);
        //            break;
        //        case JoyButtons.PRESET8:
        //            presets[1] = new RoboPreset((int)currentPan, (int)currentTilt, currentColor, currentGobo);
        //            break;
        //        case JoyButtons.PRESET9:
        //            presets[2] = new RoboPreset((int)currentPan, (int)currentTilt, currentColor, currentGobo);
        //            break;
        //        case JoyButtons.PRESET10:
        //            presets[3] = new RoboPreset((int)currentPan, (int)currentTilt, currentColor, currentGobo);
        //            break;
        //        case JoyButtons.PRESET11:
        //            presets[4] = new RoboPreset((int)currentPan, (int)currentTilt, currentColor, currentGobo);
        //            break;
        //        case JoyButtons.PRESET12:
        //            presets[5] = new RoboPreset((int)currentPan, (int)currentTilt, currentColor, currentGobo);
        //            break;
        //        default:
        //            break;
        //    }
        //}

        //public void PresetSelect(int num)
        //{
        //    switch (num)
        //    {
        //        case JoyButtons.PRESET7:
        //            Pan(presets[0].Pan);
        //            Tilt(presets[0].Tilt);
        //            SetColor(presets[0].Color);
        //            SetGobo(presets[0].Gobo);
        //            break;
        //        case JoyButtons.PRESET8:
        //            Pan(presets[1].Pan);
        //            Tilt(presets[1].Tilt);
        //            SetColor(presets[1].Color);
        //            SetGobo(presets[1].Gobo);
        //            break;
        //        case JoyButtons.PRESET9:
        //            Pan(presets[2].Pan);
        //            Tilt(presets[2].Tilt);
        //            SetColor(presets[2].Color);
        //            SetGobo(presets[2].Gobo);
        //            break;
        //        case JoyButtons.PRESET10:
        //            Pan(presets[3].Pan);
        //            Tilt(presets[3].Tilt);
        //            SetColor(presets[3].Color);
        //            SetGobo(presets[3].Gobo);
        //            break;
        //        case JoyButtons.PRESET11:
        //            Pan(presets[4].Pan);
        //            Tilt(presets[4].Tilt);
        //            SetColor(presets[4].Color);
        //            SetGobo(presets[4].Gobo);
        //            break;
        //        case JoyButtons.PRESET12:
        //            Pan(presets[5].Pan);
        //            Tilt(presets[5].Tilt);
        //            SetColor(presets[5].Color);
        //            SetGobo(presets[5].Gobo);
        //            break;
        //        default:
        //            break;
        //    }
        //}

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
