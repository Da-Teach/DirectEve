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
    using PyS = PySharp.PySharp;

    public class DirectCharacter : DirectObject
    {
        internal DirectCharacter(DirectEve directEve) : base(directEve)
        {
        }

        public long CharacterId { get; internal set; }
        public long CorporationId { get; internal set; }
        public long AllianceId { get; internal set; }
        public long WarFactionId { get; internal set; }

        public string Name
        {
            get { return DirectEve.GetOwner(CharacterId).Name; }
        }
    }
}