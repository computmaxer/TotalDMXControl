using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Total_DMX_Control_WPF
{
    public class Sequence
    {
        #region Data Members
        private string _name;
        private List<SequenceStep> steps;
        private bool _useGlobalRate;
        private int _currentStep;

        #endregion

        #region Properties
        public SequenceStep[] Steps
        {
            get { return steps.ToArray(); }
        }

        public bool UsesGlobalRate
        {
            get { return _useGlobalRate; }
        }
        #endregion

        public Sequence(string name)
        {
            _name = name;
            steps = new List<SequenceStep>();
            _useGlobalRate = true;
            _currentStep = 0;
        }

        public Sequence(string name, bool useGlobalRate) :this(name)
        {
            _useGlobalRate = useGlobalRate;
        }

        public void AddStep(List<LevelAction> actions, double timeInSecs)
        {
            steps.Add(new SequenceStep(this, actions, timeInSecs));
        }

        public void RunNext()
        {
            _currentStep++;
            if (steps.ElementAt(_currentStep) != null) steps.ElementAt(_currentStep).Run();
            else _currentStep = 0;
        }
    }
}
