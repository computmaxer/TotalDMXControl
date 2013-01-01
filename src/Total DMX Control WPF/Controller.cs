using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Total_DMX_Control_WPF
{
    /*
     * This class deals with program-level stuff, such as closing the app and handling windows.
     */
    static class Controller
    {
        #region Data Members
        // Is the app currently in the process of closing?
        private static bool _appClosing = false;

        // List of anything needing to know when a channel's properties change.
        private static List<IAsyncChannelDisplayer> asyncDisplayers = new List<IAsyncChannelDisplayer>();

        // The current lighting system configuration
        private static LightingSystemConfiguration _lightingSystemConfiguration = new LightingSystemConfiguration("Default");

        //List of all fader instances.
        public static List<Fader> Faders = new List<Fader>();

        //Global Rates
        private static double rate = 100;
        private static double background_rate = 100;

        //iTunes Jukebox
        //private static LightingCommunication _lightingCommunication = new LightingCommunication();
        #endregion

        #region Properties
        //Get whether or not the application is closing.
        public static bool AppClosing
        {
            get { return _appClosing; }
        }
        //Get the list of Channel Displayers
        public static List<IAsyncChannelDisplayer> AsyncDisplayers
        {
            get { return asyncDisplayers; }
        }
        //Global Rates
        public static double Rate
        {
            get { return rate; }
            set { rate = value; }
        }
        public static double BackgroundRate
        {
            get { return background_rate; }
            set { background_rate = value; }
        }
        
        // Access to the lighting system configuration
        public static BindingList<Effect> Effects
        {
            get { return _lightingSystemConfiguration.Effects; }
        }
        public static BindingList<Fixture> Fixtures
        {
            get { return _lightingSystemConfiguration.Fixtures; }
        }
        public static BindingList<Routine> Routines
        {
            get { return _lightingSystemConfiguration.Routines; }
        }
        public static BindingList<AttributePreset> AttributePresets
        {
            get { return _lightingSystemConfiguration.AttributePresets; }
        }
        #endregion

        public static void SwitchRates()
        {
            double temp = rate;
            rate = background_rate;
            background_rate = temp;
        }

        public static void StartProgram()
        {
            //Load the default lighting config.  If the directory does not exist, create it.
            if (!Directory.Exists(Utilities.PROGRAM_FILES_PATH)) Directory.CreateDirectory(Utilities.PROGRAM_FILES_PATH);
            //If the file does not exist, create it.
            if (!File.Exists(Utilities.PROGRAM_FILES_PATH + "\\Default.lsc"))
            {
                FileStream f = File.Create(Utilities.PROGRAM_FILES_PATH + "\\Default.lsc");
                f.Close();
            }
            Stream inputStream = File.OpenRead(Utilities.PROGRAM_FILES_PATH + "\\Default.lsc");

            try
            {
                //Attempt to deserialize from the lighting config file.
                BinaryFormatter deserializer = new BinaryFormatter();
                _lightingSystemConfiguration = (LightingSystemConfiguration)deserializer.Deserialize(inputStream);
            }
            catch (System.Runtime.Serialization.SerializationException e)
            {
                MessageBox.Show("The default configuration file could not be found.  The system will create a new one.");
            }
            catch (Exception e)
            {
                Utilities.ReportError(e);
            }
            finally
            {
                inputStream.Close();
            }

            //Start DMX
            DMXController.StartDMX();

            //Start other static classes
            Total_DMX_Control_WPF.Fixtures.Start();

            ////Connect to iTunes Jukebox Server
            //if (Properties.Settings.Default.EnableItunesJukeboxSupport)
            //{
            //    _lightingCommunication.ConnectToServer(Properties.Settings.Default.ItunesJukeboxServerIdentifier, 
            //        Properties.Settings.Default.ItunesJukeboxPort,
            //        "Total DMX Control", ClientType.LIGHTING, new Version());
            //}
            //DMXController.SetLevel("START", 62, 255);
            //DMXController.SetLevel("START", 63, 255);
            WindowManager.ShowMainWindow();
        }

        /*
         * Everything that should happen when the program is closing.
         */
        public static void EndProgram()
        {
            DMXController.StopDMX();
            //TODO: this isn't quite right; make sure that if someone disables
            //support while the program is running that we disconnect from
            //the iTunes jukebox server then
            //if (_lightingCommunication != null && Properties.Settings.Default.EnableItunesJukeboxSupport)
            //{
            //    _lightingCommunication.DisconnectFromServer();
            //}

            _lightingSystemConfiguration.Serialize(Utilities.PROGRAM_FILES_PATH);

            
            DeregisterAllDisplayers();
            _appClosing = true;
            Application.Current.Shutdown();
        }

        //public static void UpdateCurrentSong(Song currentSong)
        //{
        //    MessageBox.Show("New song:\n" + currentSong.ToString());
        //}

        public static void UpdateNumDisplayedChannels()
        {
            foreach (IAsyncChannelDisplayer disp in Controller.AsyncDisplayers)
            {
                disp.UpdateNumChannelsDisplayed(Properties.Settings.Default.NumDisplayedChannels);
            }
        }

        public static void HTPFlush()
        {
            foreach (IAsyncChannelDisplayer disp in Controller.AsyncDisplayers)
            {
                disp.HTPFlush();
            }
        }

        #region Displayer Stuff
        /*
         * Adds a channel displayer to the list of displayers to notifiy when a channel's state changes.
         */
        public static void RegisterDisplayer(IAsyncChannelDisplayer displayer)
        {
            asyncDisplayers.Add(displayer);
        }

        /*
         * Removes a channel displayer from the list when it no longer needs to know things.
         */
        public static void DeregisterDisplayer(IAsyncChannelDisplayer displayer)
        {
            asyncDisplayers.Remove(displayer);
        }

        /*
         * Removes ALL channel displayers from the list (use with caution).
         */
        public static void DeregisterAllDisplayers()
        {
            asyncDisplayers.Clear();
        }
        #endregion
    }
}