using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Total_DMX_Control_WPF
{
    public class RoutinePlayer
    {

        #region Data Members

        private Routine _routine;
        private Thread _masterThread;
        private List<Thread> _threads;
        private bool _preview;
        private bool _playing;

        #endregion

        #region Properties

        public string Name
        {
            get
            {
                if (_routine != null)
                {
                    return _routine.Name;
                }
                else return "";
            }
        }

        public bool Play
        {
            get
            {
                return _playing;
            }
            set
            {
                _playing = value;
                PlayOrCancel();
            }
        }

        public Routine Routine
        {
            get { return _routine; }
        }
        #endregion

        #region Methods
        public RoutinePlayer()
        {

        }

        public RoutinePlayer(Routine routine)
        {
            _routine = routine;
        }

        private void PlayOrCancel()
        {
            if (_playing)
            {
                PlayRoutine();
            }
            else
            {
                Cancel();
            }
        }

        public void PlayRoutine(Routine routine)
        {
            _routine = routine;
            _playing = true;
            Cancel();  // Maybe?
            _threads = new List<Thread>();

            _masterThread = new Thread(new ParameterizedThreadStart(PlayRoutineHelper));
            _masterThread.Start(routine);
            
        }

        public void PlayRoutine()
        {
            if (_routine != null)
            {
                PlayRoutine(_routine);
            }
            else
            {
                Utilities.LogError("There is no routine to play.");
                return;
            }
        }

        public void PreviewRoutine(Routine routine)
        {
            _preview = true;
            PlayRoutine(routine);
        }

        public void Cancel()
        {
            if (_masterThread != null)
            {
                _masterThread.Abort();
            }
            if (_threads != null)
            {
                foreach (Thread t in _threads)
                {
                    t.Abort();
                }
            }
            _routine.ClearFixtures();
        }

        private void PlayRoutineHelper(object parameter)
        {
            Routine routine = (Routine)parameter;

            while (true)  // For now the default is to loop the routine forever
            {
                // Start a new thread to play back each fixture's list of steps
                foreach (RoutineFixture rf in routine.RoutineFixtures)
                {
                    Thread t = new Thread(new ParameterizedThreadStart(PlayStepList));
                    t.SetApartmentState(ApartmentState.STA);
                    _threads.Add(t);
                    t.Start(rf);
                }

                // Wait until all of the threads finish
                foreach (Thread t in _threads)
                {
                    t.Join();
                }
                if (_preview)
                {
                    _preview = false;
                    break;
                }
            }
        }

        private void PlayStepList(object parameters)
        {
            RoutineFixture routineFixture = (RoutineFixture)parameters;

            foreach (RoutineStep step in routineFixture.RoutineSteps)
            {
                RoutineStepPlayer player = new RoutineStepPlayer();
                player.PlayStep(routineFixture.Fixture, step);
            }
        }

        #endregion
    }
}
