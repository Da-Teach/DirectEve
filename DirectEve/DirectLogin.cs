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
    using System.Collections.Generic;
    using global::DirectEve.PySharp;

    public class DirectLogin : DirectObject
    {
        /// <summary>
        ///   Character slot cache
        /// </summary>
        private List<DirectLoginSlot> _slots;

        internal DirectLogin(DirectEve directEve) : base(directEve)
        {
        }

        private PyObject LoginLayer
        {
            get { return PySharp.Import("__builtin__").Attribute("uicore").Attribute("layer").Attribute("login"); }
        }

        private PyObject CharSelectLayer
        {
            get { return PySharp.Import("__builtin__").Attribute("uicore").Attribute("layer").Attribute("charsel"); }
        }

        /// <summary>
        ///   The login screen is open
        /// </summary>
        public bool AtLogin
        {
            
            get { return (bool) LoginLayer.Attribute("isopen") || (bool) LoginLayer.Attribute("isopening"); }
        }

        /// <summary>
        ///   EVE is connecting/logging in
        /// </summary>
        public bool IsConnecting
        {
            get { return (bool) LoginLayer.Attribute("connecting"); }
        }

        /// <summary>
        ///   Either the character selection screen or login screen is loading
        /// </summary>
        public bool IsLoading
        {
            get { return (bool) LoginLayer.Attribute("isopening") || (bool) CharSelectLayer.Attribute("isopening"); }
        }

        /// <summary>
        ///   The character selection screen is open
        /// </summary>
        public bool AtCharacterSelection
        {
            get { return (bool) CharSelectLayer.Attribute("isopen") || (bool) CharSelectLayer.Attribute("isopening"); }
        }

        /// <summary>
        ///   Is the character selection screen ready
        /// </summary>
        public bool IsCharacterSelectionReady
        {
            get { return (bool) CharSelectLayer.Attribute("ready"); }
        }

        /// <summary>
        ///   The server status string
        /// </summary>
        public string ServerStatus
        {

            get { return (string)LoginLayer.Attribute("serverStatusTextControl").Attribute("text"); }
        }

        /// <summary>
        ///   Return the 3 character slots
        /// </summary>
        public List<DirectLoginSlot> CharacterSlots
        {
            get
            {
                if (_slots == null)
                {
                    _slots = new List<DirectLoginSlot>();
                    foreach (var slot in CharSelectLayer.Attribute("chars").ToList())
                        _slots.Add(new DirectLoginSlot(DirectEve, slot));
                }

                return _slots;
            }
        }

        /// <summary>
        ///   Login
        /// </summary>
        /// <param name = "username"></param>
        /// <param name = "password"></param>
        /// <returns></returns>
        public bool Login(string username, string password)
        {
            if (!DirectEve.HasSupportInstances())
            {
                DirectEve.Log("DirectEve: Error: This method requires a support instance.");
                return false;
            }
            
            LoginLayer.Attribute("usernameEditCtrl").Call("SetValue", username);
            LoginLayer.Attribute("passwordEditCtrl").Call("SetValue", password);
            return DirectEve.ThreadedCall(LoginLayer.Attribute("_Connect"));
        }
    }
}