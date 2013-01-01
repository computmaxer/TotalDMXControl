using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF
{
    /*
     * Reason we have this class is so that you do not have to specify a channel for GetLevel() and SetLevel() methods like you would in the super class.
     * Otherwise we could just make an instance of Fixture and delete this class.
     * Yes, we could just create overload methods in Fixture that do this.  But currently I like this serparation.
     */
    [Serializable()]
    class DimmableFixture : Fixture, ISerializable
    {
        #region Data Members
        #endregion

        #region Properties
        #endregion

        public DimmableFixture(String name, int channel) :base(name, channel)
        {
            // Create attributes
            attributes.Add(new FixtureAttribute(ATTRIBUTE_TYPE.INTENSITY, "Intensity", startChannel, this));
        }

        // To restore one from a file
        public DimmableFixture(SerializationInfo info, StreamingContext ctxt)
            :base(info, ctxt)
        {

        }


        public void SetLevel(int intensity)
        {
            base.SetLevel(intensity, startChannel);
        }

        public int GetLevel()
        {
            return base.GetLevel(startChannel);
        }

        /*
         * On() and Off() used to be here, but now they are inherited from the superclass.
         * Can't really think of anything else to put here.
         */

        // For serialization
        public new void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            base.GetObjectData(info, ctxt);
        }
    }
}
