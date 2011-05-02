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
    using global::DirectEve.PySharp;

    public class DirectStandings : DirectObject
    {
        internal DirectStandings(DirectEve directEve) : base(directEve)
        {
        }

        private PyObject AddressBook
        {
            get { return DirectEve.GetLocalSvc("addressbook"); }
        }

        public bool IsReady
        {
            get
            {
                var isReady = AddressBook.Attribute("contacts").IsValid;
                isReady &= AddressBook.Attribute("corporateContacts").IsValid;
                isReady &= AddressBook.Attribute("allianceContacts").IsValid;
                return isReady;
            }
        }

        public bool LoadStandings()
        {
            return DirectEve.ThreadedLocalSvcCall("addressbook", "GetContacts");
        }

        public float GetPersonalRelationship(long id)
        {
            if (!IsReady)
                return 0;

            return (float) AddressBook.Attribute("contacts").Call("get", id, global::DirectEve.PySharp.PySharp.PyNone).Attribute("relationshipID");
        }

        public float GetCorporationRelationship(long id)
        {
            if (!IsReady)
                return 0;

            return (float) AddressBook.Attribute("corporateContacts").Call("get", id, global::DirectEve.PySharp.PySharp.PyNone).Attribute("relationshipID");
        }

        public float GetAllianceRelationship(long id)
        {
            if (!IsReady)
                return 0;

            return (float) AddressBook.Attribute("allianceContacts").Call("get", id, global::DirectEve.PySharp.PySharp.PyNone).Attribute("relationshipID");
        }
    }
}