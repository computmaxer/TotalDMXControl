using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;

namespace Total_DMX_Control_WPF
{
    /*
     * Class copied from David Naylor's other programs. 
     * A collection of useful methods.
     */
    public static class Utilities
    {
        private static bool logWritable;

        public static string PROGRAM_FILES_PATH = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Total DMX Control\";

        static Utilities()
        {
            logWritable = true;
        }

        public static void ReportError(Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            LogError(ex.ToString());
        }

        public static void ReportError(Window owner, Exception ex)
        {
            MessageBox.Show(owner, ex.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            LogError(ex.ToString());
        }        

        public static void LogError(string message)
        {
            if (logWritable)
            {
                try
                {
                    File.AppendAllText(PROGRAM_FILES_PATH + "log.txt", DateTime.Now.ToString() + ":  " + message + Environment.NewLine);
                }
                catch (Exception ex)
                {
                    logWritable = false;
                }
            }
        }

        public static double GetDistanceBetweenPoints(Point p1, Point p2)
        {
            double x = Math.Pow(p2.X - p1.X, 2);
            double y = Math.Pow(p2.Y - p1.Y, 2);
            return Math.Sqrt(x + y);
        }


        public static int GetClosestPointIndexInList(Point pt, List<Point> points)
        {
            int closestSoFarIndex = 0;
            double shortestDistanceSoFar = Utilities.GetDistanceBetweenPoints(points[closestSoFarIndex], pt);

            for (int i = 0; i < points.Count; i++)
            {
                double thisDistance = Utilities.GetDistanceBetweenPoints(points[i], pt);

                if (thisDistance < shortestDistanceSoFar)
                {
                    closestSoFarIndex = i;
                    shortestDistanceSoFar = thisDistance;
                }
            }

            return closestSoFarIndex;
        }

        public static Point GetClosestPointInList(Point pt, List<Point> points)
        {
            return points[GetClosestPointIndexInList(pt, points)];
        }







        /*
         * Get a color when given a string of the setter name. This is used by the 
         * UpdateIntensity method in all ChannelDisplayers.  We put it here cause 
         * where else would have we put it?
         */
        //public static System.Windows.Media.Colors GetIntensityColor(string setterName)
        //{

        //    //if (setterName.ToLower().Contains("effect"))
        //    //{
        //    //    return System.Drawing.Color.Aqua;
        //    //}
        //    //Color for when a channel is fading up.
        //    if (setterName.ToLower().Contains("fadeup"))
        //    {
        //        return System.Windows.Media.Colors.SpringGreen;
        //    }
        //    //Color for when a channel is fading down.
        //    else if (setterName.ToLower().Contains("fadedown"))
        //    {
        //        return System.Windows.Media.Colors.SpringGreen;
        //    }
        //    ////Color for when a channel has finished fading.
        //    //else if (setterName.ToLower().Contains("fadedone"))
        //    //{
        //    //    return System.Drawing.Color.MediumPurple;
        //    //}
        //    else if (setterName.ToLower().Contains("static"))
        //    {
        //        return System.Windows.Media.Colors.Gray;
        //    }

        //    //DEFAULT COLOR
        //    else
        //    {
        //        return System.Windows.Media.Colors.Yellow;
        //    }
        //}

        /*
         * Concat all strings in a string list into one and return the new concatonated string.
         */
        public static string StringListToString(List<String> StringList)
        {
            if (StringList.Count > 0)
            {
                string temp = string.Empty;

                //Add each string to the string, separated by commas
                for (int i = 0; i < StringList.Count - 1; i++)
                {
                    temp += StringList[i].ToString() + ",";
                }
                //Add the last string to the string with no following comma
                temp += StringList[StringList.Count - 1];

                return temp;
            }
            else
            {
                return String.Empty;
            }
        }

        /*
         * Convert the given string into a list of strings delimited by the
         * given delimiter.
         */
        public static List<string> StringToStringList(string String, char delimiter)
        {
            List<string> temp = new List<string>();

            if (String != string.Empty)
            {
                string[] input = String.Split(delimiter);

                foreach (string s in input)
                {
                    temp.Add(s);
                }
            }

            return temp;
        }

        /*
         * Return a string containing all ints from the given int list separated by commas
         */
        public static string IntListToString(List<int> IntList)
        {
            if (IntList.Count > 0)
            {
                string temp = string.Empty;

                //Add each int to the string, separated by commas
                for (int i = 0; i < IntList.Count - 1; i++)
                {
                    temp += IntList[i].ToString() + ",";
                }
                //Add the last string to the string with no following comma
                temp += IntList[IntList.Count - 1];

                return temp;
            }
            else
            {
                return String.Empty;
            }
        }

        /*
         * Convert the given String delimited by commas into an int list
         */
        public static List<int> StringToIntList(string String)
        {
            List<int> temp = new List<int>();

            if (String != string.Empty)
            {
                string[] input = String.Split(',');

                foreach (string s in input)
                {
                    temp.Add(Convert.ToInt32(s.Trim()));
                }
            }

            return temp;
        }

        public static string GetFileNameFromPath(string FilePath)
        {
            string[] filePath = FilePath.Split('\\');
            string[] fileName = filePath[filePath.Length - 1].Split('.');

            return fileName[0];
        }

        public static Point StringToPoint(string String)
        {
            string data = String.Trim().TrimStart('{').TrimEnd('}');
            string[] values = data.Split(',');
            string[] moreValues = values[0].Split('=');
            string[] moreValues2 = values[1].Split('=');
            return new Point(Convert.ToInt32(moreValues[1]), Convert.ToInt32(moreValues2[1]));
        }

        //public static Song StringToSong(string theString)
        //{
        //    string[] separatorArray = { Communication.SONG_SEPARATOR };
        //    string[] songInfo = theString.Split(separatorArray, StringSplitOptions.None);

        //    Song theSong = new Song(songInfo[0], songInfo[1], songInfo[2], songInfo[3],
        //        Convert.ToInt32(songInfo[4]), StringToTimeSpan(songInfo[5]));

        //    return theSong;
        //}

        public static TimeSpan StringToTimeSpan(string theString)
        {
            string[] stringInfo = theString.Split(':');
            return new TimeSpan(0, Convert.ToInt32(stringInfo[0]), Convert.ToInt32(stringInfo[1]));
        }
    }
}
