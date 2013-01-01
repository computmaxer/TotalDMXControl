using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace Total_DMX_Control_WPF
{
    interface IAsyncChannelDisplayer
    {
        void UpdateNumChannelsDisplayed(int num);
        void SetBlackout(bool isBlackedOut);
        void HTPFlush();
    }
}
