using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Total_DMX_Control_WPF
{
    public delegate void FaderDoneCallback();

    public class Fader : LevelAction
    {
        #region Data Members
        public static int FADER_TIMER_TICK_INTERVAL = 15;
        protected int _start, _end;
        protected bool _up;
        protected double _increment, _current, _ms;
        protected int _level;
        protected FaderDoneCallback _callback;
        #endregion

        public Fader(string setterName, int channel, int start, int end, double fadeTimeSecs) : this(setterName, new List<Photon>() { new Photon(channel, 255) }, start, end, fadeTimeSecs) { }

        public Fader(string setterName, List<Photon> channels, int start, int end, double fadeTimeSecs) :base (setterName, channels, fadeTimeSecs)
        {
            _end = end;
            _start = start;
            _level = 255;
            _ms = fadeTimeSecs * 1000.0;
            _chans = new List<Photon>();
            _chans.AddRange(channels);
            if ((end - start) >= 0) _up = true;
            else _up = false;
            if (fadeTimeSecs <= 0) _increment = -1;
            else _increment = 1;
            _timer = new Timer();
            _timer.Elapsed += Tick;
        }

        public Fader(string setterName, int channel, int end, double fadeTimeSecs) :
            this(setterName, channel, DMXController.GetLevel(channel), end, fadeTimeSecs) { }

        public new virtual void Run()
        {
            if (_increment <= 0) Finish();
            else
            {
                _increment = Math.Abs(_start - _end) / Math.Ceiling((_ms / FADER_TIMER_TICK_INTERVAL) * (100 / Controller.Rate));
                _current = _start;
                _timer.Interval = FADER_TIMER_TICK_INTERVAL;
                _timer.Start();
            }
        }

        public void Run(int level)
        {
            _level = level;
            Run();
        }

        public void Run(FaderDoneCallback callback)
        {
            _callback = callback;
            Run();
        }

        protected virtual void Finish()
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
            foreach (Photon pair in _chans)
            {
                DMXController.SetLevel(_setterName, pair.Channel, (_level *_end * pair.Intensity) / 65025);
            }
            if (_callback != null)
            {
                _callback();
            }
        }

        public override void Kill()
        {
            if(_timer != null) _timer.Stop();
            foreach (Photon pair in _chans)
            {
                DMXController.SetLevel(_setterName, pair.Channel, 0);
            }
            if (_callback != null)
            {
                _callback();
            }
        }

        protected virtual void Tick(object sender, EventArgs e)
        {
            if (_up)
            {
                _current += _increment;
                if (_current >= _end) Finish();
                else
                {
                    foreach (Photon pair in _chans)
                        DMXController.SetLevel(_setterName, pair.Channel, Convert.ToInt32(Math.Floor((_level * _current * pair.Intensity) / 65025)));
                }
            }
            else
            {
                _current -= _increment;
                if (_current <= _end) Finish();
                else
                {
                    foreach (Photon pair in _chans)
                        DMXController.SetLevel(_setterName, pair.Channel, Convert.ToInt32(Math.Floor((_level * _current * pair.Intensity) / 65025)));
                }
            }
        }

    }

    public class PanTiltCoarseFineFader : Fader
    {
        private int _panCoarseChannel;
        private int _panFineChannel;
        private int _tiltCoarseChannel;
        private int _tiltFineChannel;
        private int _tiltStart;
        private int _tiltEnd;
        private double _tiltIncrement;
        private double _tiltCurrent;
        //The Pan will use the superclass _end and _start etc.
        public PanTiltCoarseFineFader(string setterName, int panCoarseChannel, int panFineChannel, int panBigEnd, int tiltCoarseChannel, int tiltFineChannel, int tiltBigEnd, double fadeTimeSecs)
            :base(setterName, panCoarseChannel, (DMXController.GetLevel(panCoarseChannel) * 256) + DMXController.GetLevel(panFineChannel), panBigEnd, fadeTimeSecs)
        {
            _panCoarseChannel = panCoarseChannel;
            _panFineChannel = panFineChannel;
            _tiltCoarseChannel = tiltCoarseChannel;
            _tiltFineChannel = tiltFineChannel;
            _tiltStart = (DMXController.GetLevel(tiltCoarseChannel) * 256) + DMXController.GetLevel(tiltFineChannel);
            _tiltEnd = tiltBigEnd;
            _tiltIncrement = Math.Abs(_tiltStart - _tiltEnd) / Math.Ceiling((_ms / FADER_TIMER_TICK_INTERVAL) * (100 / Controller.Rate));
            _tiltCurrent = _tiltStart;
        }

        protected override void Finish()
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
            //just to make sure it ends at the right values for sure.
            double fine = _end % 256;
            double coarse = _end / 256;
            DMXController.SetLevel(_setterName, _panFineChannel, Convert.ToInt32(Math.Floor((_level * fine) / 255)));
            DMXController.SetLevel(_setterName, _panCoarseChannel, Convert.ToInt32(Math.Floor((_level * coarse) / 255)));
            double tiltFine = _tiltEnd % 256;
            double tiltCoarse = _tiltEnd / 256;
            DMXController.SetLevel(_setterName, _tiltFineChannel, Convert.ToInt32(Math.Floor((_level * tiltFine) / 255)));
            DMXController.SetLevel(_setterName, _tiltCoarseChannel, Convert.ToInt32(Math.Floor((_level * tiltCoarse) / 255)));
            if (_callback != null)
            {
                _callback();
            }
        }

        protected override void Tick(object sender, EventArgs e)
        {
            if (_up)
            {
                //The normal "superclass" variables store the Pan values.
                _current += _increment;
                _tiltCurrent += _tiltIncrement;
                if (_current >= _end) Finish();
                else
                {
                    double fine = _current % 256;
                    double coarse = _current / 256;
                    DMXController.SetLevel(_setterName, _panFineChannel, Convert.ToInt32(Math.Floor((_level * fine) / 255)));
                    DMXController.SetLevel(_setterName, _panCoarseChannel, Convert.ToInt32(Math.Floor((_level * coarse) / 255)));
                    double tiltFine = _tiltCurrent % 256;
                    double tiltCoarse = _tiltCurrent / 256;
                    DMXController.SetLevel(_setterName, _tiltFineChannel, Convert.ToInt32(Math.Floor((_level * tiltFine) / 255)));
                    DMXController.SetLevel(_setterName, _tiltCoarseChannel, Convert.ToInt32(Math.Floor((_level * tiltCoarse) / 255)));
                }
            }
            else
            {
                _current -= _increment;
                _tiltCurrent -= _tiltIncrement;
                if (_current <= _end) Finish();
                else
                {
                    double fine = _current % 256;
                    double coarse = _current / 256;
                    DMXController.SetLevel(_setterName, _panFineChannel, Convert.ToInt32(Math.Floor((_level * fine) / 255)));
                    DMXController.SetLevel(_setterName, _panCoarseChannel, Convert.ToInt32(Math.Floor((_level * coarse) / 255)));
                    double tiltFine = _tiltCurrent % 256;
                    double tiltCoarse = _tiltCurrent / 256;
                    DMXController.SetLevel(_setterName, _tiltFineChannel, Convert.ToInt32(Math.Floor((_level * tiltFine) / 255)));
                    DMXController.SetLevel(_setterName, _tiltCoarseChannel, Convert.ToInt32(Math.Floor((_level * tiltCoarse) / 255)));
                }
            }
        }

    }
}
