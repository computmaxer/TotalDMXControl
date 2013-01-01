using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF
{
    [Serializable()]
    public class RoutineFixture : ISerializable
    {

        #region Data Members
        private Fixture _fixture;
        private ObservableCollection<RoutineStep> _routineSteps;
        private ObservableCollection<RoutineFixtureReferencePoint> _referencePoints;
        #endregion

        #region Properties
        public Fixture Fixture
        {
            get { return _fixture; }
            set { _fixture = value; }
        }
        public string Name
        {
            get { return _fixture.Name; }
        }
        public ObservableCollection<RoutineStep> RoutineSteps
        {
            get { return _routineSteps; }
            set { _routineSteps = value; }
        }
        public ObservableCollection<RoutineFixtureReferencePoint> ReferencePoints
        {
            get { return _referencePoints; }
            set { _referencePoints = value; }
        }
        #endregion

        #region Methods
        public RoutineFixture(Fixture fixture)
        {
            _fixture = fixture;
            _routineSteps = new ObservableCollection<RoutineStep>();
            _referencePoints = new ObservableCollection<RoutineFixtureReferencePoint>();
        }

        public RoutineFixture(SerializationInfo info, StreamingContext ctxt)
        {
            _fixture = (Fixture)info.GetValue("_fixture", typeof(Fixture));
            _routineSteps = (ObservableCollection<RoutineStep>)info.GetValue("_routineSteps", typeof(ObservableCollection<RoutineStep>)); // Will this preserve the order? Do collections have order?
            _referencePoints = (ObservableCollection<RoutineFixtureReferencePoint>)info.GetValue("_referencePoints", typeof(ObservableCollection<RoutineFixtureReferencePoint>));
        }

        public void RemoveFromCanvas(Canvas canvas)
        {
            foreach (RoutineFixtureReferencePoint rp in ReferencePoints)
            {
                if (canvas.Children.Contains(rp.Ellipse))
                {
                    canvas.Children.Remove(rp.Ellipse);
                    canvas.Children.Remove(rp.Label);
                }
            }
        }

        public void AddToCanvas(Canvas canvas)
        {
            foreach (RoutineFixtureReferencePoint rp in ReferencePoints)
            {
                if (!canvas.Children.Contains(rp.Ellipse))
                {
                    canvas.Children.Add(rp.Ellipse);
                    canvas.Children.Add(rp.Label);
                }
            }

            foreach (RoutineStep step in RoutineSteps)
            {
                if (step.Visible) // looks weird, but trust me :P
                {
                    step.Visible = false;
                    step.Visible = true;
                }
            }
        }

        public void ClearFixture()
        {
            _fixture.Reset();
        }

        // For serialization
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("_fixture", _fixture);
            info.AddValue("_routineSteps", _routineSteps);
            info.AddValue("_referencePoints", _referencePoints);
        }
        #endregion

    }
}
