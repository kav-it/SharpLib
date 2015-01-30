using System;
using System.Collections.Generic;
using System.Text;

namespace SharpLib.Audio.Dmo
{
    [Flags]
    enum DmoInputStatusFlags
    {
        None,
        DMO_INPUT_STATUSF_ACCEPT_DATA	= 0x1
    }
}
