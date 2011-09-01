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
    public class DirectOwner : DirectObject
    {
        internal DirectOwner(DirectEve directEve) : base(directEve)
        {
        }

        public long OwnerId { get; private set; }
        public string Name { get; private set; }
        public int TypeId { get; private set; }

        internal static DirectOwner GetOwner(DirectEve directEve, long ownerId)
        {
            var pyOwner = directEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("eveowners").Attribute("data").DictionaryItem(ownerId);

            var owner = new DirectOwner(directEve);
            owner.OwnerId = (long) pyOwner.Attribute("ownerID");
            owner.Name = (string) pyOwner.Attribute("ownerName");
            owner.TypeId = (int) pyOwner.Attribute("typeID");
            return owner;
        }
    }
}