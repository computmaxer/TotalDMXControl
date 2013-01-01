using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Timers;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF
{
    [Serializable()]
    public class EffectStep : ISerializable
    {
        #region Data Members

        private Effect _parentEffect;
        private Timer _stepTimer;
        private Timer _downTimer;
        private List<Photon> _photons;
        private BindingList<BigPhoton> _bigPhotons;
        //private BindingList<FixtureAttribute> _attribs;
        private Fader downFader, upFader;
        private double _stepTime;
        private double _upFadeTime;
        private double _dwellTime;
        private double _downFadeTime;
        private int _lowLevel;
        private int _highLevel;
        private List<Photon> _allPhotons;
        private int _stepID;
        #endregion

        #region Properties
        public BindingList<BigPhoton> BigPhotons
        {
            get { return _bigPhotons; }
        }
        public double Step
        {
            get { return _stepTime; }
            set { _stepTime = value; }
        }
        public double Up
        {
            get { return _upFadeTime; }
            set { _upFadeTime = value; }
        }
        public double Dwell
        {
            get { return _dwellTime; }
            set { _dwellTime = value; }
        }
        public double Down
        {
            get { return _downFadeTime; }
            set { _downFadeTime = value; }
        }
        public int Low
        {
            get { return _lowLevel; }
            set { _lowLevel = value; }
        }
        public int High
        {
            get { return _highLevel; }
            set { _highLevel = value; }
        }
        #endregion

        public EffectStep(Effect parent, List<Photon> photons, double stepTime, double upFadeTime, double dwellTime, double downFadeTime, int low, int high, int stepID)
            :this(parent, stepTime, upFadeTime, dwellTime, downFadeTime, low, high, stepID)
        {
            _photons.AddRange(photons);
            //_attribs = null;
        }

        public EffectStep(Effect parent, List<BigPhoton> bigPhotons, double stepTime, double upFadeTime, double dwellTime, double downFadeTime, int low, int high, int stepID)
            : this(parent, stepTime, upFadeTime, dwellTime, downFadeTime, low, high, stepID)
        {
            //TODO: Better way to do this?
            foreach (BigPhoton p in bigPhotons)
            {
                _bigPhotons.Add(p);
            }

            //_pairs = null;
        }

        public EffectStep(Effect parent, List<Photon> pairs, int stepID) : this(parent, pairs, 0.2, 0, 0.2, 0, 0, 255, stepID) { }
        
        public EffectStep(Effect parent, int stepID) : this(parent, 0.2, 0, 0.2, 0, 0, 255, stepID) { }

        public EffectStep(Effect parent, List<Photon> photons, List<BigPhoton> bigPhotons, double stepTime, double upFadeTime, double dwellTime, double downFadeTime, int low, int high, int stepID)
            : this(parent, stepTime, upFadeTime, dwellTime, downFadeTime, low, high, stepID)
        {
            //TODO: Better way to do this?
            foreach (BigPhoton p in bigPhotons)
            {
                _bigPhotons.Add(p);
            }

            _photons.AddRange(photons);
        }

        private EffectStep(Effect parent, double stepTime, double upFadeTime, double dwellTime, double downFadeTime, int low, int high, int stepID) 
        {
            _parentEffect = parent;
            _photons = new List<Photon>();
            _bigPhotons = new BindingList<BigPhoton>();
            _allPhotons = new List<Photon>();
            _stepTimer = new Timer();
            _stepTimer.AutoReset = false;
            _stepTimer.Elapsed += Finished;
            _downTimer = new Timer();
            _downTimer.AutoReset = false;
            _downTimer.Elapsed += StartDownFade;
            _stepTime = stepTime;
            _upFadeTime = upFadeTime;
            _dwellTime = dwellTime;
            _downFadeTime = downFadeTime;
            _lowLevel = low;
            _highLevel = high;
            _stepID = stepID;
            if (upFadeTime == 0 && dwellTime == 0 && downFadeTime == 0) _dwellTime = stepTime;
        }

        // To restore one from a file
        public EffectStep(SerializationInfo info, StreamingContext ctxt)
        {
            _parentEffect = (Effect)info.GetValue("parent", typeof(Effect));
            _photons = (List<Photon>)info.GetValue("pairs", typeof(List<Photon>));
            _bigPhotons = (BindingList<BigPhoton>)info.GetValue("bigPhotons", typeof(BindingList<BigPhoton>));
            _stepTime = info.GetDouble("stepTime");
            _upFadeTime = info.GetDouble("upFadeTime");
            _dwellTime = info.GetDouble("dwellTime");
            _downFadeTime = info.GetDouble("downFadeTime");
            _lowLevel = info.GetInt32("lowLevel");
            _highLevel = info.GetInt32("highLevel");
            _stepID = info.GetInt32("stepID");
            _allPhotons = new List<Photon>();
            _stepTimer = new Timer();
            _stepTimer.AutoReset = false;
            _stepTimer.Elapsed += Finished;
            _downTimer = new Timer();
            _downTimer.AutoReset = false;
            _downTimer.Elapsed += StartDownFade;
        }

        public void InitializeStep()
        {
            _allPhotons.Clear();
            if (_photons.Count > 0)
            {
                _allPhotons.AddRange(_photons);
            }
            if (_bigPhotons.Count > 0)
            {
                foreach (BigPhoton p in _bigPhotons)
                {
                    _allPhotons.AddRange(p.Attribute.GetPhotonsAtLevel(p.Level));
                }
            }

            upFader = new Fader(_parentEffect.ToString() + _stepID.ToString(), _allPhotons, _lowLevel, _highLevel, (100.0 / Controller.Rate) * _upFadeTime);
            downFader = new Fader(_parentEffect.ToString() + _stepID.ToString(), _allPhotons, _highLevel, _lowLevel, (100.0 / Controller.Rate) * _downFadeTime);

        }

        public void Run()
        {
            if (_parentEffect.Running)
            {
                upFader.Run(_parentEffect.Level);
                if (_stepTimer != null)
                {
                    _stepTimer.Interval = ((100.0 / Controller.Rate) * _stepTime * 1000.0);
                    _stepTimer.Start();
                }
                if (_downTimer != null)
                {
                    _downTimer.Interval = ((100.0 / Controller.Rate) * ((_upFadeTime + _dwellTime) * 1000.0));
                    _downTimer.Start();
                }
            }
        }

        private void StartDownFade(object sender, EventArgs e)
        {
            _downTimer.Stop();
            downFader.Run(_parentEffect.Level);
        }

        private void Finished(object sender, EventArgs e)
        {
            _stepTimer.Stop();
            _parentEffect.RunNext();
        }

        public void Kill()
        {
            _stepTimer.Stop();
            _downTimer.Stop();
            if (downFader != null) downFader.Kill();
            if (upFader != null) upFader.Kill();
        }

        // For serialization
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("parent", _parentEffect);
            info.AddValue("pairs", _photons);
            info.AddValue("bigPhotons", _bigPhotons);
            info.AddValue("stepTime", _stepTime);
            info.AddValue("upFadeTime", _upFadeTime);
            info.AddValue("dwellTime", _dwellTime);
            info.AddValue("downFadeTime", _downFadeTime);
            info.AddValue("lowLevel", _lowLevel);
            info.AddValue("highLevel", _highLevel);
            info.AddValue("stepID", _stepID);
        }
    }
}
