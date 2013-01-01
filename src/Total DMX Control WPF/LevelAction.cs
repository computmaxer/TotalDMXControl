using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Total_DMX_Control_WPF
{
    public abstract class LevelAction
    {
        protected Timer _timer;
        protected string _setterName;
        protected List<Photon> _chans;

        public LevelAction(string setterName, List<Photon> chans, double timeInSecs)
        {
            _setterName = setterName;
            _chans = new List<Photon>();
            _chans.AddRange(chans);
            if (timeInSecs <= 0) _timer = null;
            else _timer = new Timer(timeInSecs * 1000.0);
        }

        public void Pause()
        {
            _timer.Enabled = false;
        }

        public void Unpause()
        {
            _timer.Enabled = true;
        }

        public abstract void Kill();

        public void Run()
        {
            if (_timer != null) _timer.Start();
        }
    }
}
