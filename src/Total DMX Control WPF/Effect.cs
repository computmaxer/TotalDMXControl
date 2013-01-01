using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF
{
    [Serializable()]
    public class Effect : ISerializable
    {
        #region Data Members
        private string _name;
        private int _level;
        public BindingList<EffectStep> Steps;
        public HashSet<EffectAttribute> Attributes;
        private Random _ran;
        private int _currentStep;
        private bool _bouncing;
        public bool _running;
        private int _stepID;
        #endregion

        #region Properties
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }
        public bool Running
        {
            get { return _running; }
        }
        public int CurrentStep
        {
            get { return _currentStep; }
        }
        #endregion

        public Effect(string name)
        {
            _name = name;
            _level = 255;
            Steps = new BindingList<EffectStep>();
            Attributes = new HashSet<EffectAttribute>();
            _ran = new Random();
            _bouncing = false;
            _currentStep = 0;
            _running = false;
            _stepID = 0;
        }

        // To restore one from a file
        public Effect(SerializationInfo info, StreamingContext ctxt)
        {
            _name = info.GetString("Name");
            _level = info.GetInt32("Level");
            Steps = (BindingList<EffectStep>)info.GetValue("Steps", typeof(BindingList<EffectStep>));
            Attributes = (HashSet<EffectAttribute>)info.GetValue("Attributes", typeof(HashSet<EffectAttribute>));
            _stepID = info.GetInt32("StepID");
            _ran = new Random();
            _bouncing = false;
            _currentStep = 0;
            _running = false;
        }

        public override string ToString()
        {
            return _name;
        }

        public void Start()
        {
            foreach (EffectStep step in Steps)
            {
                step.InitializeStep();
            }
            _running = true;
            _currentStep = 0;
            if(Steps.Count > 0) Steps.ElementAt(0).Run();
        }

        public void Stop()
        {
            _running = false;
            foreach (EffectStep step in Steps) step.Kill();
        }

        /****ADD STEP METHODS ****/
        #region AddStep Methods
        public void AddStep(List<Photon> chan_pairs, double stepTime, double upFadeTime, double dwellTime, double downFadeTime, int low, int high)
        {
            Steps.Add(new EffectStep(this, chan_pairs, stepTime, upFadeTime, dwellTime, downFadeTime, low, high, _stepID));
            _stepID++;
        }

        public void AddStep(List<BigPhoton> bigPhotons, double stepTime, double upFadeTime, double dwellTime, double downFadeTime, int low, int high)
        {
            Steps.Add(new EffectStep(this, bigPhotons, stepTime, upFadeTime, dwellTime, downFadeTime, low, high, _stepID));
            _stepID++;
        }

        public void AddStep(int[] chans, double stepTime, double upFadeTime, double dwellTime, double downFadeTime, int low, int high)
        {
            List<Photon> temp = new List<Photon>();
            foreach (int chan in chans)
            {
                temp.Add(new Photon(chan, 255));
            }
            AddStep(temp, stepTime, upFadeTime, dwellTime, downFadeTime, 0, 255);
        }

        public void AddStep(Photon chan_pair, double stepTime, double upFadeTime, double dwellTime, double downFadeTime, int low, int high)
        {
            AddStep(new List<Photon>() { chan_pair }, stepTime, upFadeTime, dwellTime, downFadeTime, 0, 255);
        }

        public void AddStep(int chan, double stepTime, double upFadeTime, double dwellTime, double downFadeTime, int low, int high)
        {
            AddStep(new Photon(chan, 255), stepTime, upFadeTime, dwellTime, downFadeTime, 0, 255);
        }

        public void AddStep(List<Photon> chan_pairs)
        {
            AddStep(chan_pairs, 0.2, 0, 0.2, 0, 0, 255);
        }

        public void AddStep(Photon chan_pair)
        {
            List<Photon> chan_pairs = new List<Photon>();
            chan_pairs.Add(chan_pair);
            AddStep(chan_pairs);
        }

        public void AddStep(int chan)
        {
            AddStep(new Photon(chan, 255));            
        }

        public void AddStep(int[] chans)
        {
            List<Photon> p = new List<Photon>();
            foreach (int chan in chans)
            {
                p.Add(new Photon(chan, 255));
            }
            AddStep(p);
        }

        public void AddStep()
        {
            Steps.Add(new EffectStep(this, _stepID));
            _stepID++;
        }
        #endregion

        private void GoBackwards()
        {
            if (_currentStep == 0)
            {
                _currentStep = Steps.Count - 1;
            }
            else
            {
                _currentStep--;
            }
            Steps.ElementAt(_currentStep).Run();
        }

        private void GoForwards()
        {
            if (_currentStep == Steps.Count - 1)
            {
                _currentStep = 0;
            }
            else _currentStep++;
            Steps.ElementAt(_currentStep).Run();
        }

        public void Pause()
        {
            _running = false;
        }

        public void Unpause()
        {
            _running = true;
            RunNext();
        }

        public void RunNext()
        {
            if (_running)
            {
                if (Attributes.Contains(EffectAttribute.RANDOM)) Steps[_ran.Next(Steps.Count)].Run();
                else
                {
                    if (Attributes.Contains(EffectAttribute.REVERSE))
                    {
                        if (Attributes.Contains(EffectAttribute.BOUNCE))
                        {
                            if (_currentStep == 0) _bouncing = true;
                            else if (_currentStep == Steps.Count - 1) _bouncing = false;
                            if (_bouncing) GoForwards();
                            else GoBackwards();
                        }
                        else GoBackwards();
                    }
                    else
                    {
                        if (Attributes.Contains(EffectAttribute.BOUNCE))
                        {
                            if (_currentStep == Steps.Count - 1) _bouncing = true;
                            else if (_currentStep == 0) _bouncing = false;
                            if (_bouncing) GoBackwards();
                            else GoForwards();
                        }
                        else GoForwards();
                    }
                }
            }
        }

        // For serialization
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Name", _name);
            info.AddValue("Level", _level);
            info.AddValue("Steps", Steps);
            info.AddValue("Attributes", Attributes);
            info.AddValue("StepID", _stepID);
        }
    }

    public enum EffectAttribute
    {
        REVERSE,
        BOUNCE,
        RANDOM
    }
}
