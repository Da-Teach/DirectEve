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
    using System.Collections.Generic;
    using global::DirectEve.PySharp;

    public class DirectWindow : DirectObject
    {
        private static WindowType[] _windowTypes = new[]
                                                   {
                                                       new WindowType("name", "marketsellaction", (directEve, pyWindow) => new DirectMarketActionWindow(directEve, pyWindow)),
                                                       new WindowType("name", "marketbuyaction", (directEve, pyWindow) => new DirectMarketActionWindow(directEve, pyWindow)),
                                                       new WindowType("__guid__", "form.AgentDialogueWindow", (directEve, pyWindow) => new DirectAgentWindow(directEve, pyWindow)),
                                                       new WindowType("__guid__", "form.VirtualInvWindow", (directEve, pyWindow) => new DirectContainerWindow(directEve, pyWindow)),
                                                       new WindowType("__guid__", "form.PVPOfferView", (directEve, pyWindow) => new DirectContainerWindow(directEve, pyWindow)),
                                                       new WindowType("__guid__", "form.SpyHangar", (directEve, pyWindow) => new DirectContainerWindow(directEve, pyWindow)),
                                                       new WindowType("__guid__", "form.ItemHangar", (directEve, pyWindow) => new DirectOwnContainerWindow(directEve, pyWindow)),
                                                       new WindowType("__guid__", "form.ShipHangar", (directEve, pyWindow) => new DirectContainerWindow(directEve, pyWindow)),
                                                       new WindowType("__guid__", "form.CorpHangar", (directEve, pyWindow) => new DirectContainerWindow(directEve, pyWindow)),
                                                       new WindowType("__guid__", "form.CorpHangarArray", (directEve, pyWindow) => new DirectContainerWindow(directEve, pyWindow)),
                                                       new WindowType("__guid__", "form.CorpMemberHangar", (directEve, pyWindow) => new DirectContainerWindow(directEve, pyWindow)),
                                                       new WindowType("__guid__", "form.CorpMarketHangar", (directEve, pyWindow) => new DirectContainerWindow(directEve, pyWindow)),
                                                       new WindowType("__guid__", "form.ShipCargoView", (directEve, pyWindow) => new DirectOwnContainerWindow(directEve, pyWindow)),
                                                       new WindowType("__guid__", "form.DockedCargoView", (directEve, pyWindow) => new DirectContainerWindow(directEve, pyWindow)),
                                                       new WindowType("__guid__", "form.InflightCargoView", (directEve, pyWindow) => new DirectContainerWindow(directEve, pyWindow)),
                                                       new WindowType("__guid__", "form.LootCargoView", (directEve, pyWindow) => new DirectContainerWindow(directEve, pyWindow)),
                                                       new WindowType("__guid__", "form.DroneBay", (directEve, pyWindow) => new DirectContainerWindow(directEve, pyWindow)),
                                                       new WindowType("__guid__", "form.RegionalMarket", (directEve, pyWindow) => new DirectMarketWindow(directEve, pyWindow)),
                                                       new WindowType("__guid__", "form.LSCChannel", (directEve, pyWindow) => new DirectChatWindow(directEve, pyWindow)),
                                                       new WindowType("name", "telecom", (directEve, pyWindow) => new DirectTelecomWindow(directEve, pyWindow)),
                                                   };

        internal PyObject PyWindow;

        internal DirectWindow(DirectEve directEve, PyObject pyWindow) : base(directEve)
        {
            PyWindow = pyWindow;

            Id = (int?) pyWindow.Attribute("windowID");
            Type = (string) pyWindow.Attribute("__guid__");
            Name = (string) pyWindow.Attribute("name");
            IsKillable = (bool) pyWindow.Attribute("_killable");
            IsDialog = (bool) pyWindow.Attribute("isDialog");
            IsModal = (bool) pyWindow.Attribute("isModal");
            Caption = (string) pyWindow.Call("GetCaption");

            var html = (string) pyWindow.Attribute("sr").Attribute("browser").Attribute("sr").Attribute("htmlstr");
            if (string.IsNullOrEmpty(html))
                html = (string) pyWindow.Attribute("edit").Attribute("sr").Attribute("currentTXT");
            if (string.IsNullOrEmpty(html))
                html = (string) pyWindow.Attribute("sr").Attribute("messageArea").Attribute("sr").Attribute("currentTXT");
            Html = html;

            ViewMode = (string) pyWindow.Attribute("viewMode");
        }

        public int? Id { get; internal set; }
        public string Type { get; internal set; }
        public string Name { get; internal set; }
        public string Caption { get; internal set; }
        public string Html { get; internal set; }
        public string ViewMode { get; internal set; }

        public bool IsKillable { get; internal set; }
        public bool IsDialog { get; internal set; }
        public bool IsModal { get; internal set; }

        internal static List<DirectWindow> GetWindows(DirectEve directEve)
        {
            var windows = new List<DirectWindow>();

            var pySharp = directEve.PySharp;
            var builtin = pySharp.Import("__builtin__");
            var pyWindows = builtin.Attribute("eve").Call("LocalSvc", "window").Call("GetWindows").ToList();
            foreach (var pyWindow in pyWindows)
            {
                // Ignore destroyed windows
                if ((bool) pyWindow.Attribute("destroyed"))
                    continue;

                DirectWindow window = null;
                foreach (var windowType in _windowTypes)
                {
                    if ((string) pyWindow.Attribute(windowType.Attribute) != windowType.Value)
                        continue;

                    window = windowType.Creator(directEve, pyWindow);
                }

                if (window == null)
                    window = new DirectWindow(directEve, pyWindow);

                windows.Add(window);
            }

            return windows;
        }

        public virtual bool Close()
        {
            if (!IsKillable)
                return false;

            return DirectEve.ThreadedCall(PyWindow.Attribute("CloseX"));
        }

        #region Nested type: WindowType

        private class WindowType
        {
            public WindowType(string attribute, string value, Func<DirectEve, PyObject, DirectWindow> creator)
            {
                Attribute = attribute;
                Value = value;
                Creator = creator;
            }

            public string Attribute { get; set; }
            public string Value { get; set; }
            public Func<DirectEve, PyObject, DirectWindow> Creator { get; set; }
        }

        #endregion
    }
}