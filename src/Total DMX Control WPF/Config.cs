using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;

namespace Total_DMX_Control_WPF
{
    //*** DONT USE ME***
    // Use Properties.Settings.Default.xxx instead

    static class Config
    {
        private static string filePath = Utilities.PROGRAM_FILES_PATH + "config.txt";
        private static FileStream config_file;

        public static void Load()
        {
            if (CheckForConfig())
            {
                config_file = File.Open(filePath, FileMode.Open);
                StreamReader reader = new StreamReader(config_file);
                try
                {
                    Definitions.NUM_CHANNELS_DISP = Int32.Parse(reader.ReadLine());
                }
                catch
                {
                    MessageBox.Show("Failed to load config");
                }
                reader.Close();
                config_file.Close();
            }
            else
            {
                MessageBox.Show("No config file was found. Will use default configuration.");
                Save();
            }
        }

        public static void Save()
        {
            StreamWriter writer;
            
            if (CheckForConfig())
            {
                config_file = File.Open(filePath, FileMode.Open);
                writer = new StreamWriter(config_file);
            }
            else
            {
                config_file = File.Create(filePath);
                writer = new StreamWriter(config_file);
            }

            writer.WriteLine(Definitions.NUM_CHANNELS_DISP.ToString());
            writer.Close();
            config_file.Close();
        }

        private static bool CheckForConfig()
        {
            if (File.Exists(filePath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
