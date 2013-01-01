using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Total_DMX_Control_WPF
{
    [Serializable()]
    public struct Photon : ISerializable
    {
        private int _chan, _intensity;

        public int Channel
        {
            get { return _chan; }
            set { _chan = value; }
        }

        public int Intensity
        {
            get { return _intensity; }
            set { _intensity = value; }
        }

        public Photon(int channel, int intensity)
        {
            _chan = channel;
            _intensity = intensity;
        }
        
        // To restore one from a file
        public Photon(SerializationInfo info, StreamingContext ctxt)
        {
            _chan = info.GetInt32("Channel");
            _intensity = info.GetInt32("Intensity");
        }


        // For serialization
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Channel", _chan);
            info.AddValue("Intensity", _intensity);
        }
    }
}
