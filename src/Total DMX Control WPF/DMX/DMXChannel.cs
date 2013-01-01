using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Total_DMX_Control_WPF
{
    /*
     * A single Highest Takes Precedence (HTP) DMX Channel.
     * DMXController stores 512 of them in its array.
     */
    public class DMXChannel
    {
        #region Data Members
        // The channel's number
        private int _num;
        // Stores the channel's current level (after HTP).
        private int _level;
        // Stores all the HTP entries.
        private Hashtable _HTPLevels;
        // Whether this channel has been suppressed.
        private bool _suppressed, _sticky, _override;
        #endregion

        #region Properties
        /*
         * Get the level of this channel.
         */
        public int Level
        {
            get { return _level; }
        }
        /*
         * Get or set whether this channel is suppressed.
         */ 
        public bool Suppressed
        {
            get { return _suppressed; }
            set { _suppressed = value; }
        }

        public bool Sticky
        {
            get { return _sticky; }
            set { _suppressed = value; }
        }
        #endregion

        /*
         * Constructor
         * Initializes the HTP hashtable.
         * Sets the initial level to 0 and unsupresses.
         */ 
        public DMXChannel(int num)
        {
            _num = num;
            _HTPLevels = new Hashtable();
            _suppressed = false;
            _override = false;
            SetLevel("startup", 0);
        }

        /*
         * Sets the channel's level.
         * This method should only ever be used by DMXController.
         * HTP will look at all other level settings and decide the highest one.
         * If identical setterNames are used, the most recent call applies.
         */ 
        public void SetLevel(string setterName, int level)
        {
            if (setterName.ToLower().Contains("nohtp") || setterName.ToLower().Contains("override"))
            {
                _level = level;
            }
            else
            {
                _HTPLevels[setterName] = level;
                UpdateHTP();
            }
        }

        //Ability to override HTP
        public void SetOverride(bool overrideHTP)
        {
            _override = overrideHTP;
            if (!_override)
            {
                UpdateHTP();
            }
        }

        public void RemoveSetter(string setterName)
        {
            _HTPLevels.Remove(setterName);
            UpdateHTP();
        }

        public void FlushHTP()
        {
            _HTPLevels = new Hashtable();
            SetLevel("startup", 0);
        }

        private void UpdateHTP()
        {
            if (_override)
            {
                return;
            }
            int highestSoFar = 0;
            Hashtable levels = (Hashtable) _HTPLevels.Clone();
            foreach (int general in levels.Values)
                if (general > highestSoFar) highestSoFar = general;
            _level = highestSoFar;
        }
    }
}
