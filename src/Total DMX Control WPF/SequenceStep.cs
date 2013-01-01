using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Total_DMX_Control_WPF
{
    public class SequenceStep
    {
        #region Data Members
        private Sequence _parent;
        private List<LevelAction> Actions;
        private Timer timer;
        private double timeinms;
        #endregion

        public SequenceStep(Sequence parent, List<LevelAction> actions, double timeInSecs)
        {
            _parent = parent;
            Actions = new List<LevelAction>();
            Actions.AddRange(actions);
            timeinms = timeInSecs * 1000.0;
            timer = new Timer(timeinms);
            timer.AutoReset = false;
            timer.Elapsed += Finish;
        }

        public void Run()
        {
            if (_parent.UsesGlobalRate) timer.Interval = timeinms * (Controller.Rate / 100.0);
            foreach (LevelAction action in Actions)
            {
                action.Run();
            }
            timer.Start();
        }

        private void Finish(object sender, EventArgs e)
        {
            timer.Stop();
            _parent.RunNext();
        }
    }
}
