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
    using System.Linq;
    using global::DirectEve.PySharp;

    public class DirectChatWindow : DirectWindow
    {
        private List<DirectCharacter> _members;
        private List<DirectChatMessage> _messages;

        internal DirectChatWindow(DirectEve directEve, PyObject pyWindow) : base(directEve, pyWindow)
        {
            var id = pyWindow.Attribute("channelID");
            if (id.GetPyType() == PyType.TupleType)
                ChannelId = (string) id.Item(0).Item(0);
            if (id.GetPyType() == PyType.StringType)
                ChannelId = (string) id;
            if (id.GetPyType() == PyType.IntType)
                ChannelId = ((long) id).ToString();

            MemberCount = (int) pyWindow.Attribute("memberCount");      // deprecated by CCP
            Usermode = (int)pyWindow.Attribute("usermode");             // deprecated by CCP
            EveMemberCount = (int)pyWindow.Attribute("eveMemberCount");
            DustMemberCount = (int)pyWindow.Attribute("dustMemberCount");
            ShowUserList = (bool)pyWindow.Attribute("showUserList");

            
        }

        public string ChannelId { get; private set; }
        public int MemberCount { get; private set; }
        public int EveMemberCount { get; private set; }
        public int DustMemberCount { get; private set; }
        public int Usermode { get; private set; }
        public bool ShowUserList { get; private set; }

        public List<DirectChatMessage> Messages
        {
            get
            {
                if (_messages == null)
                    _messages = PyWindow.Attribute("messages").ToList().Select(m => new DirectChatMessage(DirectEve, m)).ToList();

                return _messages;
            }
        }

        public List<DirectCharacter> Members
        {
            get
            {
                if (_members == null)
                {
                    _members = new List<DirectCharacter>();

                    // Only do this if user list is shown
                    if (ShowUserList)
                    {
                        var channelId = PyWindow.Attribute("channelID");
                        var members = DirectEve.GetLocalSvc("LSC").Attribute("channels").DictionaryItem(channelId).Attribute("memberList");
                        foreach (var memberId in members.Call("keys").ToList())
                        {
                            var member = members.Call("__getitem__", memberId);
                            var character = new DirectCharacter(DirectEve);
                            character.CharacterId = (long) member.Attribute("charID");
                            character.CorporationId = (long) member.Attribute("corpID");
                            character.AllianceId = (long) member.Attribute("allianceID");
                            character.WarFactionId = (long) member.Attribute("warFactionID");
                            _members.Add(character);
                        }
                    }
                }
                return _members;
            }
        }

        public bool Speak(string message)
        {
            return DirectEve.ThreadedCall(PyWindow.Attribute("Speak"), message, PySharp.Import("__builtin__").Attribute("eve").Attribute("session").Attribute("charid"), true);
        }
    }
}