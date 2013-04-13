using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Total_DMX_Control_WPF
{
    class FixtureWrapperFAOverride
    {

        private Fixture _fixture;
        public bool _isAffected;

        public string Name
        {
            get { return _fixture.Name; }
        }

        public Fixture Fixture
        {
            get { return _fixture; }
        }
        public bool IsAffected
        {
            get { return _isAffected; }
            set { 
                _isAffected = value;
                //if (!_isAffected)
                //{
                //    ((RoutinePlayerWindow)WindowManager.CurrentViews[typeof(RoutinePlayerWindow)]).DeactivateCurrentPreset();
                //}
                //else
                //{
                //    ((RoutinePlayerWindow)WindowManager.CurrentViews[typeof(RoutinePlayerWindow)]).ActivateCurrentPreset();
                //}
            }
        }

        public FixtureWrapperFAOverride(Fixture fixture)
        {
            _fixture = fixture;
            _isAffected = false;
        }
    }
}
