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
    public class DirectAgentResponse : DirectObject
    {
        internal DirectAgentResponse(DirectEve directEve) : base(directEve)
        {
        }

        public long AgentId { get; internal set; }
        public int ActionId { get; internal set; }
        public string Text { get; internal set; }

        public bool Say()
        {
            return DirectEve.ThreadedLocalSvcCall("agents", "DoAction", AgentId, ActionId);
        }
    }
}