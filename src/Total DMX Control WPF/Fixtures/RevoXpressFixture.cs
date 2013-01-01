using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF
{
    [Serializable()]
    class RevoXpressFixture : Fixture, ISerializable
    {
        #region Data Members

        private const int NUM_CHANNELS = 4;

        #endregion

        #region Properties

        #endregion

        public RevoXpressFixture(String name, int startChannel)
            : base(name, startChannel, startChannel + NUM_CHANNELS - 1)
        {
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.MODE, "Sound Mode", new List<Photon>() {new Photon(startChannel, 255)}, this));
        }

        // To restore one from a file
        public RevoXpressFixture(SerializationInfo info, StreamingContext ctxt)
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
