using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Total_DMX_Control_WPF
{
    public static class WindowManager
    {
        #region Data Members
        private static Hashtable _currentViews;
        #endregion

        #region Properties
        static Window MainWindow
        {
            get { return Application.Current.MainWindow; }
        }
        public static Hashtable CurrentViews
        {
            get { return _currentViews; }
        }
        #endregion

        static WindowManager()
        {
            _currentViews = new Hashtable();
        }

        public static void ShowMainWindow()
        {
            Application.Current.MainWindow = new MainStatusScreen();
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.Closing += new System.ComponentModel.CancelEventHandler(MainWindow_Closing);
        }

        public static void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Controller.EndProgram();
        }

        public static void ShowWindow<W>() where W : Window, new()
        {
            if (Application.Current.MainWindow == null)
            {
                ShowMainWindow();
            }
            else
            {
                List<W> open = Application.Current.Windows.OfType<W>().ToList<W>();
                if (open.Count == 0)
                {
                    Window newWindow = new W();
                    Application.Current.MainWindow.Dispatcher.Invoke((Action)delegate
                    {
                        newWindow.Show();
                        newWindow.Activate();
                    });
                    _currentViews[newWindow.GetType()] =newWindow;
                }
                else open[0].Activate();
            }
        }

        public static void CloseAll()
        {
            foreach (Window w in _currentViews)
            {
                if (!w.Dispatcher.HasShutdownStarted)
                {
                    w.Dispatcher.InvokeShutdown();
                }
            }
            Application.Current.MainWindow.Dispatcher.InvokeShutdown();
        }

    }
}
