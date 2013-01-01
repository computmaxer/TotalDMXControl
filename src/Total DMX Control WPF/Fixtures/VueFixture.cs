using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF.FixtureClasses
{
    public static class Vue_6
    {
        public const int BLACKOUT = 0;
        public const int RED = 7;
        public const int YELLOW = 12;
        public const int GREEN = 17;
        public const int BLUE = 22;
        public const int PURPLE = 27;
        public const int CYAN = 32;
        public const int WHITE = 37;

        /// <summary>
        /// Vue stand-alone modes
        /// </summary>
        /// <param name="num">Range: 1-9</param>
        /// <returns>DMX Value</returns>
        public static int StandAlone(int num)
        {
            return (25 * num + 10);
        }

        /// <summary>
        /// Vue auto sequences
        /// </summary>
        /// <param name="num">Range: 1 - 27</param>
        /// <returns>DMX Value for Ch 2-4</returns>
        public static int Auto(int num)
        {
            return (5 * num + 37);
        }
    }

    public enum VuePod
    {
        ONE,
        TWO,
        THREE
    }

    public enum VuePodColor
    {
        BLACKOUT,
        RED,
        YELLOW,
        GREEN,
        BLUE,
        PURPLE,
        CYAN,
        WHITE
    }

    public class VueFixture : Fixture, ISerializable
    {
        private const int NUM_CHANNELS = 6;

        public const int SOUND_ACTIVE_MODE = 10;
        public const int MANUAL_MODE = 0;

        protected int pod1, pod2, pod3;

        #region Properties
        public int Pod1
        {
            get { return pod1; }
        }
        public int Pod2
        {
            get { return pod2; }
        }
        public int Pod3
        {
            get { return pod3; }
        }
        #endregion

        public VueFixture(string name, int startChannel)
            : base(name, startChannel, startChannel + NUM_CHANNELS - 1)
        {
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.MODE, "Control", startChannel, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.OTHER, "Pod1", startChannel + 1, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.OTHER, "Pod2", startChannel + 2, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.OTHER, "Pod3", startChannel + 3, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.OTHER, "Strobe", startChannel + 4, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.OTHER, "Motor", startChannel + 5, this));

            pod1 = startChannel + 1;
            pod2 = startChannel + 2;
            pod3 = startChannel + 3;
        }

        public VueFixture(SerializationInfo info, StreamingContext ctxt)
            : base(info, ctxt)
        {

        }

        public new void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            base.GetObjectData(info, ctxt);
        }

        public void SetMode(int mode)
        {
            if (mode <= 0) SetLevel(0, startChannel);
            else if (mode >= 10) SetLevel(255, startChannel);
            else SetLevel((25 * mode) + 5, startChannel);
        }

        public void SetPod(VuePod pod, int automode)
        {
            int podval = 0;
            switch (pod)
            {
                case VuePod.ONE:
                    podval = 1;
                    break;
                case VuePod.TWO:
                    podval = 2;
                    break;
                case VuePod.THREE:
                    podval = 3;
                    break;
                default:
                    break;
            }
            if (podval > 0)
            {
                if (automode <= 0) SetLevel(0, startChannel + podval);
                else if (automode > 27) SetLevel(255, startChannel + podval);
                else SetLevel((5 * automode) + 37, startChannel + podval);
            }
        }

        public void SetPod(VuePod pod, VuePodColor color)
        {
            int podval = 0;
            switch (pod)
            {
                case VuePod.ONE:
                    podval = 1;
                    break;
                case VuePod.TWO:
                    podval = 2;
                    break;
                case VuePod.THREE:
                    podval = 3;
                    break;
                default:
                    break;
            }
            if (podval > 0)
            {
                switch (color)
                {
                    case VuePodColor.BLACKOUT:
                        SetLevel(0, startChannel + podval);
                        break;
                    case VuePodColor.RED:
                        SetLevel(7, startChannel + podval);
                        break;
                    case VuePodColor.YELLOW:
                        SetLevel(12, startChannel + podval);
                        break;
                    case VuePodColor.GREEN:
                        SetLevel(17, startChannel + podval);
                        break;
                    case VuePodColor.BLUE:
                        SetLevel(22, startChannel + podval);
                        break;
                    case VuePodColor.PURPLE:
                        SetLevel(27, startChannel + podval);
                        break;
                    case VuePodColor.CYAN:
                        SetLevel(32, startChannel + podval);
                        break;
                    case VuePodColor.WHITE:
                        SetLevel(37, startChannel + podval);
                        break;
                    default:
                        break;
                }
            }
        }

        public void SetStrobeSpeed(int speed)
        {
            if (speed <= 0) SetLevel(0, startChannel + 4);
            else if (speed > 255) SetLevel(255, startChannel + 4);
            else SetLevel(speed, startChannel + 4);
        }

        public void SetMotorSpeed(int speed)
        {
            if (speed <= 0) SetLevel(0, startChannel + 5);
            else if (speed > 255) SetLevel(255, startChannel + 5);
            else SetLevel(speed, startChannel + 5);
        }

        public override void On()
        {
            SetLevel(255, startChannel);
        }

        public override void Off()
        {
            for (int i = 0; i < NUM_CHANNELS; i++) SetLevel(0, StartChannel + i);
        }
    }
}
