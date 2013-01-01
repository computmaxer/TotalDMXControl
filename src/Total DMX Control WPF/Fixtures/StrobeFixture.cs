using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF
{
    [Serializable()]
    class StrobeFixture : Fixture, ISerializable
    {
        #region Data Members

        private const int NUM_CHANNELS = 2;

        #endregion

        #region Properties

        #endregion

        public StrobeFixture(String name, int startChannel)
            : base(name, startChannel, startChannel + NUM_CHANNELS - 1)
        {
            // Create attributes
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.OTHER, "Speed", startChannel, this));
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.INTENSITY, "Intensity", startChannel + 1, this));
        }

        // To restore one from a file
        public StrobeFixture(SerializationInfo info, StreamingContext ctxt)
            :base(info, ctxt)
        {

        }


        // For serialization
        public new void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            base.GetObjectData(info, ctxt);
        }
    }
}
