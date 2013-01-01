using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Total_DMX_Control_WPF
{
    /*
     * A way to refer to something that can be turned on or off, in simplest terms.
     */
    public enum Element
    {
        CHANNEL,
        PAR,
        LED,
        STROBE,
        MINE,
        ROPE,
        REVO,
        DISCO,
        CANCEL,
        NULL
    }

    public enum FixtureType
    {
        COLOR_STRIP,
        DIMMABLE,
        LED_PAR,
        REVO,
        ROBOSCAN,
        STROBE,
        CUSTOM
    }

    public struct Definitions
    {
        public static int NUM_CHANNELS_DISP = 128;
    }
}