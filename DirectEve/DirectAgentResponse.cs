// ------------------------------------------------------------------------------
//   <copyright from='2010' to='2015' company='THEHACKERWITHIN.COM'>
//     Copyright (c) TheHackerWithin.COM. All Rights Reserved.
// 
//     Please look in the accompanying license.htm file for the license that 
//     applies to this source code. (a copy can also be found at: 
//     http://www.thehackerwithin.com/license.htm)
//   </copyright>
// -------------------------------------------------------------------------------
using global::DirectEve.PySharp;
namespace DirectEve
{
    public class DirectAgentResponse : DirectObject
    {
        private PyObject container;
        internal DirectAgentResponse(DirectEve directEve, PyObject _container)
            : base(directEve)
        {
            container = _container;
        }        

        string[] responseButtonsPathRight = { "__maincontainer", "main", "rightPane", "rightPaneBottom", "btnsmainparent", "btns", "" };
        string[] responseButtonsPathLeft = { "__maincontainer", "main", "rightPaneBottom", "btnsmainparent", "btns", "" };

        public long AgentId { get; internal set; }
        //public int ActionId { get; internal set; }
        public string Text { get; internal set; }
        public string button;
        public bool right;

        public bool Say()
        {
            /*InnerSpace.Echo("doing say, agentId is "+AgentId+"actionId is "+ActionId);
            return DirectEve.ThreadedLocalSvcCall("agents", "DoAction", AgentId, ActionId);*/
            //InnerSpace.Echo("doing say, button is"+button);
            string[] responseButtonsPath;
            if (right)
            {
                responseButtonsPath = responseButtonsPathRight;
                responseButtonsPath[6] = button;
            }
            else
            {
                responseButtonsPath = responseButtonsPathLeft;
                responseButtonsPath[5] = button;
            }

            var btn = DirectEve.findChildWithPath(container, responseButtonsPath);
            //InnerSpace.Echo("btn name is"+btn.Attribute("name"));
            return DirectEve.ThreadedCall(btn.Attribute("OnClick"));
        }
    }
}