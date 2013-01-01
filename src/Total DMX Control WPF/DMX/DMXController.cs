using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace Total_DMX_Control_WPF
{
    /*
     * This class controls all 512 of the DMX Channels.
     * Go here to get or set levels and whether Channels are suppressed.
     * In addition, you can simply blackout the entire show.
     */ 
    public static class DMXController
    {
        //Not for production, just so we can debug the program without the device.
        //To use the device this should be false.
        public static bool debugging = false;

        #region Data Members
        // Array where all the DMX Channels are stored.
        // Note the array is really 0-511, so any methods written in this class need to take heed of that.
        // However, any arguments to methods in this class should be one (not zero) based.
        private static DMXChannel[] dmxChannels;
        // Stores the blackout state.
        private static bool _blackout;
        // Holds the instance of the wrapper class.
        private static OpenDMXUSBWrapper _device;
        private static bool _stopped;
        private static bool _paused;
        #endregion

        #region Properties
        /*
         * Gets or sets the blackout state.
         */ 
        public static bool Blackout
        {
            get { return _blackout; }
            set { 
                _blackout = value;
                foreach (IAsyncChannelDisplayer asyncDisp in Controller.AsyncDisplayers)
                {
                    Application.Current.Dispatcher.BeginInvoke((Action)delegate
                    {
                        asyncDisp.SetBlackout(value);
                    });
                }
            }
        }

        public static bool Paused
        {
            get { return _paused; }
            set { _paused = value; }
        }
        #endregion

        static DMXController()
        {
            _stopped = false;
            _paused = false;
            dmxChannels = new DMXChannel[512];
            for (int i = 0; i < 512; i++)
            {
                dmxChannels[i] = new DMXChannel(i + 1);
            }
        }

        /*
         * This method needs to be called before anything in this class is accessed.
         * It creates all the DMX Channels and starts the connection to the device.
         */
        public static void StartDMX()
        {
            if (debugging)
            {
                MessageBox.Show("Debugging Mode Enabled, DMX Device will not function");
            }
            else
            {
                _device = new OpenDMXUSBWrapper();
                _device.Start();
            }
        }

        /*
         * Stop the DMX device.  This is called from Controller when the application is closing.
         */
        public static void StopDMX()
        {
            _stopped = true;
            if (!debugging) _device.Stop();
        }

        /*
         * Method called by the wrapper class.
         * Rather than the wrapper class being updated every time the level is set,
         * the wrapper class will poll the controller for information every 25 ms or so,
         * which is the minimum time between DMX sends.
         */
        public static string getDMXBuffer()
        {
            byte[] DMXArray = new byte[512];
            if (!_blackout)
                for (int i = 0; i < 512; i++)
                {
                    if (dmxChannels[i].Suppressed) DMXArray[i] = 0;
                    else DMXArray[i] = Convert.ToByte(dmxChannels[i].Level);
                }
            return System.Text.Encoding.Default.GetString(DMXArray);
        }

        public static int[] getDMXArray()
        {
            int[] array = new int[512];
            for (int i = 0; i < 512; i++) array[i] = dmxChannels[i].Level;
            return array;
        }

        public static bool[] getSuppressedArray()
        {
            bool[] array = new bool[512];
            for (int i = 0; i < 512; i++) array[i] = dmxChannels[i].Suppressed;
            return array;
        }

        #region Level Sets/Gets
        /*
         * ***ONE-BASED***
         * Sets a channel's level.
         * The setterName allows HTP to work.
         */
        public static void SetLevel(string setterName, int dmxchannel, int intensity)
        {
            if (intensity > 255)
            {
                Utilities.LogError("Trying to set level higher than 255! Channel: " + dmxchannel + "Level: " + intensity);
                return;
            }
            if (!_stopped)
            {
                dmxChannels[dmxchannel - 1].SetLevel(setterName, intensity);
                //foreach (IAsyncChannelDisplayer asyncDisp in Controller.AsyncDisplayers)
                //{
                //    asyncDisp.BeginInvoke((MethodInvoker)delegate
                //    {
                //        asyncDisp.UpdateIntensity(dmxchannel, dmxChannels[dmxchannel - 1].Level, setterName, dmxChannels[dmxchannel -1].Suppressed);
                //    });
                //}
            }
        }

        /*
         * ***ONE-BASED***
         * Allows setting of multiple levels at once.
         */ 
        public static void SetLevels(string setterName, List<Photon> levelsList, int level)
        {
            foreach (Photon pair in levelsList) SetLevel(setterName, pair.Channel, (pair.Intensity * level) / 255);
        }

        /*
         * ***ONE-BASED***
         * Gets the level of a channel.
         */ 
        public static int GetLevel(int dmxchannel)
        {
            return dmxChannels[dmxchannel - 1].Level;
        }

        public static void RemoveSetter(string setterName, int chan)
        {
            dmxChannels[chan].RemoveSetter(setterName);
        }

        public static void RemoveSetterFromAll(string setterName)
        {
            foreach (DMXChannel chan in dmxChannels) chan.RemoveSetter(setterName);
        }

        public static void FlushHTP(int chan)
        {
            dmxChannels[chan - 1].FlushHTP();
        }

        public static void SetOverride(int chan, bool overrideHTP)
        {
            dmxChannels[chan - 1].SetOverride(overrideHTP);
        }

        public static void FlushHTPAll()
        {
            foreach (DMXChannel chan in dmxChannels) chan.FlushHTP();
            Controller.HTPFlush();
        }
        #endregion

        #region Suppress Sets/Gets
        /*
         * ***ONE-BASED***
         * Suppresses a channel, forcing its level to be zero all the time until being unsuppressed.
         */ 
        public static void Suppress(int dmxchannel)
        {
            dmxChannels[dmxchannel - 1].Sticky = true;
            dmxChannels[dmxchannel - 1].Suppressed = true;
            //foreach (IAsyncChannelDisplayer asyncDisp in Controller.AsyncDisplayers)
            //{
            //    asyncDisp.BeginInvoke((MethodInvoker)delegate
            //    {
            //        asyncDisp.UpdateIntensity(dmxchannel, dmxChannels[dmxchannel - 1].Level, "suppress", dmxChannels[dmxchannel - 1].Suppressed);
            //    });
            //}
        }

        /*
         * ***ONE-BASED***
         * Suppresses a list of channels.
         */ 
        public static void Suppress(List<int> dmxchannels)
        {
            foreach (int chan in dmxchannels) Suppress(chan);
        }

        /*
         * ***ONE-BASED***
         * Unsupresses a channel, allowing it to operate normally.
         */
        public static void Unsuppress(int dmxchannel)
        {
            dmxChannels[dmxchannel - 1].Sticky = false;
            dmxChannels[dmxchannel - 1].Suppressed = false;
            //foreach (IAsyncChannelDisplayer asyncDisp in Controller.AsyncDisplayers)
            //{
            //    asyncDisp.BeginInvoke((MethodInvoker)delegate
            //    {
            //        asyncDisp.UpdateIntensity(dmxchannel, dmxChannels[dmxchannel - 1].Level, "unsuppress", dmxChannels[dmxchannel - 1].Suppressed);
            //    });
            //}
            
        }

        /*
        * ***ONE-BASED***
        * Unsuppresses a list of channels.
        */
        public static void Unsuppress(List<int> dmxchannels)
        {
            foreach (int chan in dmxchannels) Unsuppress(chan);
        }

        /*
         * Unsupresses all channels.
         */ 
        public static void UnsuppressAll()
        {
            for (int i = 1; i <= 512; i++)
            {
                Unsuppress(i);
            }
        }

        public static void Solo(List<int> dmxchannels)
        {
            int i = 0;
            do
            {
                dmxChannels[i].Suppressed = true;
                //foreach (IAsyncChannelDisplayer asyncDisp in Controller.AsyncDisplayers)
                //{
                //    asyncDisp.BeginInvoke((MethodInvoker)delegate
                //    {
                //        asyncDisp.UpdateIntensity(i + 1, dmxChannels[i].Level, "suppress", dmxChannels[i].Suppressed);
                //    });
                //}
                i++;
            } while (i < 512);
            foreach (int chan in dmxchannels)
            {
                Unsuppress(chan);
            }
        }

        public static void Unsolo()
        {
            int i = 0;
            do
            {
                if (!dmxChannels[i].Sticky)
                {
                    dmxChannels[i].Suppressed = false;
                    //foreach (IAsyncChannelDisplayer asyncDisp in Controller.AsyncDisplayers)
                    //{
                    //    asyncDisp.BeginInvoke((MethodInvoker)delegate
                    //    {
                    //        asyncDisp.UpdateIntensity(i + 1, dmxChannels[i].Level, "suppress", dmxChannels[i].Suppressed);
                    //    });
                    //}
                }
                i++;
            } while (i < 512);
        }

        /*
         * ***ONE-BASED***
         * Tells you if a channel is suppressed.
         */ 
        public static bool IsSuppressed(int dmxchannel)
        {
            return dmxChannels[dmxchannel - 1].Suppressed;
        }

        #endregion

    }
}
