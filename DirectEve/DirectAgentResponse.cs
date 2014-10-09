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
    using System.Linq;
    using global::DirectEve.PySharp;

    public class DirectAgentResponse : DirectObject
    {
        private PyObject _container;

        private string[] _responseButtonsPathLeft = {"__maincontainer", "main", "rightPaneBottom"};
        private string[] _responseButtonsPathRight = {"__maincontainer", "main", "rightPane", "rightPaneBottom"};

        internal DirectAgentResponse(DirectEve directEve, PyObject container)
            : base(directEve)
        {
            _container = container;
        }

        public long AgentId { get; internal set; }
        public string Text { get; internal set; }
        public string Button { get; internal set; }
        public bool Right { get; internal set; }

        public bool Say()
        {
            var btn = DirectWindow.FindChildWithPath(_container, (Right ? _responseButtonsPathRight : _responseButtonsPathLeft).Concat(new[] {Button}));
            return DirectEve.ThreadedCall(btn.Attribute("OnClick"));
        }
    }
}