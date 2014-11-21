// ------------------------------------------------------------------------------
//   <copyright from='2010' to='2015' company='THEHACKERWITHIN.COM'>
//     Copyright (c) TheHackerWithin.COM. All Rights Reserved.
// 
//     Please look in the accompanying license.htm file for the license that 
//     applies to this source code. (a copy can also be found at: 
//     http://www.thehackerwithin.com/license.htm)
//   </copyright>
// -------------------------------------------------------------------------------

namespace DirectEve
{
    using PySharp;

    public class DirectReprocessingQuoteRecoverable : DirectInvType
    {
        internal DirectReprocessingQuoteRecoverable(DirectEve directEve, PyObject recoverable) : base(directEve)
        {
            TypeId = (int) recoverable.Item(0);
            YouReceive = (long) recoverable.Item(1);
            WeTake = (long) recoverable.Item(2);
            Unrecoverable = (long) recoverable.Item(3);
        }

        public long YouReceive { get; private set; }
        public long WeTake { get; private set; }
        public long Unrecoverable { get; private set; }
    }
}