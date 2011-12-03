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


        internal DirectAgentWindow(DirectEve directEve, PyObject pyWindow)
            : base(directEve, pyWindow)
        {
            var loading = pyWindow.Attribute("sr").Attribute("briefingBrowser").Attribute("_loading");
            IsReady = loading.IsValid && !(bool)loading;

            if (pyWindow.Attribute("sr").Attribute("briefingBrowser").IsValid)
            {
                loading = pyWindow.Attribute("sr").Attribute("objectiveBrowser").Attribute("_loading");
                IsReady &= loading.IsValid && !(bool)loading;
            }

            AgentId = (int)pyWindow.Attribute("sr").Attribute("agentID");
            AgentSays = (string)pyWindow.Attribute("sr").Attribute("agentSays");

            AgentResponses = new List<DirectAgentResponse>();            

            string[] responseButtonsPathRight = { "__maincontainer", "main", "rightPane", "rightPaneBottom", "btnsmainparent", "btns" };
            string[] responseButtonsPathLeft = { "__maincontainer", "main", "rightPaneBottom", "btnsmainparent", "btns" };

            string viewMode = (string)pyWindow.Attribute("viewMode");

            if (viewMode != "SinglePaneView")
            {
                var buttonsRight = FindChildWithPath(pyWindow, responseButtonsPathRight).Attribute("children").Attribute("_childrenObjects").ToList();
                

                foreach (var response in buttonsRight)
                {
                    var directResponse = new DirectAgentResponse(directEve, pyWindow);
                    directResponse.AgentId = AgentId;
                    directResponse.Text = (string)response.Attribute("text");
                    directResponse.Button = (string)response.Attribute("name");
                    directResponse.Right = true;
                    AgentResponses.Add(directResponse);
                }
            }
            else
            {
                var buttonsLeft = FindChildWithPath(pyWindow, responseButtonsPathLeft).Attribute("children").Attribute("_childrenObjects").ToList();                

                foreach (var response in buttonsLeft)
                {
                    var directResponse = new DirectAgentResponse(directEve, pyWindow);
                    directResponse.AgentId = AgentId;
                    directResponse.Text = (string)response.Attribute("text");
                    directResponse.Button = (string)response.Attribute("name");
                    directResponse.Right = false;
                    AgentResponses.Add(directResponse);
                }
            }            

            Briefing = (string)pyWindow.Attribute("sr").Attribute("briefingBrowser").Attribute("sr").Attribute("currentTXT");
            Objective = (string)pyWindow.Attribute("sr").Attribute("objectiveBrowser").Attribute("sr").Attribute("currentTXT");
        }

        public bool IsReady { get; internal set; }
        public long AgentId { get; internal set; }
        public string AgentSays { get; internal set; }

        public string Briefing { get; internal set; }
        public string Objective { get; internal set; }

        public List<DirectAgentResponse> AgentResponses { get; internal set; }
    }
}