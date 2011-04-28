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

    public class DirectAgentWindow : DirectWindow
    {
        internal DirectAgentWindow(DirectEve directEve, PyObject pyWindow) : base(directEve, pyWindow)
        {
            var loading = pyWindow.Attribute("sr").Attribute("briefingBrowser").Attribute("_loading");
            IsReady = loading.IsValid && !(bool) loading;

            if (pyWindow.Attribute("sr").Attribute("briefingBrowser").IsValid)
            {
                loading = pyWindow.Attribute("sr").Attribute("objectiveBrowser").Attribute("_loading");
                IsReady &= loading.IsValid && !(bool) loading;
            }

            AgentId = (int) pyWindow.Attribute("sr").Attribute("agentID");
            AgentSays = (string) pyWindow.Attribute("sr").Attribute("agentSays");

            AgentResponses = new List<DirectAgentResponse>();
            foreach (var response in pyWindow.Attribute("sr").Attribute("dialogue").ToList())
            {
                var directResponse = new DirectAgentResponse(directEve);
                directResponse.AgentId = AgentId;
                directResponse.ActionId = (int) response.Item(0);
                directResponse.Text = (string) response.Item(1);
                AgentResponses.Add(directResponse);
            }

            Briefing = (string) pyWindow.Attribute("sr").Attribute("briefingBrowser").Attribute("sr").Attribute("currentTXT");
            Objective = (string) pyWindow.Attribute("sr").Attribute("objectiveBrowser").Attribute("sr").Attribute("currentTXT");
        }

        public bool IsReady { get; internal set; }
        public long AgentId { get; internal set; }
        public string AgentSays { get; internal set; }

        public string Briefing { get; internal set; }
        public string Objective { get; internal set; }

        public List<DirectAgentResponse> AgentResponses { get; internal set; }
    }
}