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
    using System;
    using global::DirectEve.PySharp;

    public class DirectChatMessage : DirectObject
    {
        internal DirectChatMessage(DirectEve directEve, PyObject message) : base(directEve)
        {
            Name = (string) message.Item(0);
            Message = (string)message.Item(1);
            CharacterId = -1;
            if (message.Item(2).GetPyType() == PyType.IntType)
                CharacterId = (long)message.Item(2);
            Time = (DateTime)message.Item(3);
            ColorKey = (int)message.Item(4);
        }

        public string Name { get; internal set; }
        public string Message { get; internal set; }
        public long CharacterId { get; internal set; }
        public DateTime Time { get; internal set; }
        public int ColorKey { get; internal set; }
    }
}