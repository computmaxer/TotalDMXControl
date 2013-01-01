using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows.Controls;

namespace Total_DMX_Control_WPF
{
    [Serializable()]
	public class Routine : ISerializable
    {
        #region Static

        public static string NextRoutineName()
        {
            // Default routine names are "Routine #"
            // Note the following properties useful for recognizing default names:
            // 1) Contain a space
            // 2) Splitting string over spaces results in 2 substrings
            // 3) First substring must be "Routine"
            // 4) Second substring must be an integer

            int biggestExistingDefaultRoutine = 0;

            foreach (Routine routine in Controller.Routines)
            {
                string[] nameArray = routine.Name.Split(' ');
                if (nameArray.Length == 2 && nameArray[0] == "Routine")
                {
                    // If this really is a default name, figure out what number
                    // it is and save in effectNumber
                    int fixtureNumber = -1;
                    if (Int32.TryParse(nameArray[1], out fixtureNumber)
                        && fixtureNumber > biggestExistingDefaultRoutine)
                    {
                        biggestExistingDefaultRoutine = fixtureNumber;
                    }
                }
            }

            return "Routine " + (biggestExistingDefaultRoutine + 1);
        }

        #endregion

        #region Data Members
        private string _name;
        private BindingList<RoutineFixture> _routineFixtures;
        private RoutineBuilder _routineBuilder;
        #endregion

        #region Properties
        public BindingList<RoutineFixture> RoutineFixtures
        {
            get { return _routineFixtures; }
            set { _routineFixtures = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public RoutineBuilder RoutineBuilder
        {
            get { return _routineBuilder; }
            set { _routineBuilder = value; }
        }
        #endregion

        #region Methods
        public Routine()
        {
            _name = Routine.NextRoutineName();
            _routineFixtures = new BindingList<RoutineFixture>();
        }

        public Routine(SerializationInfo info, StreamingContext ctxt)
        {
            _name = info.GetString("_name");
            _routineFixtures = (BindingList<RoutineFixture>)info.GetValue("_routineFixtures", typeof(BindingList<RoutineFixture>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_name", _name);
            info.AddValue("_routineFixtures", _routineFixtures);
        }

        public void ClearFixtures()
        {
            foreach (RoutineFixture fixt in _routineFixtures)
            {
                fixt.ClearFixture();
            }
        }

        public void DetachStepsFromCanvas(Canvas canvas)
        {
            foreach (RoutineFixture rf in _routineFixtures)
            {
                foreach (RoutineStep step in rf.RoutineSteps)
                {
                    step.DetachFromCanvas(canvas);
                }
            }
        }
        #endregion


    }
}
