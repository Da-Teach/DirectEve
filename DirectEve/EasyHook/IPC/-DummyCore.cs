// ------------------------------------------------------------------------------
//   <copyright from='2010' to='2015' company='THEHACKERWITHIN.COM'>
//     Copyright (c) TheHackerWithin.COM. All Rights Reserved.
// 
//     Please look in the accompanying license.htm file for the license that 
//     applies to this source code. (a copy can also be found at: 
//     http://www.thehackerwithin.com/license.htm)
//   </copyright>
// -------------------------------------------------------------------------------

namespace EasyHook.IPC
{
    using System.Diagnostics;

    /// <summary>
    ///     Represents EasyHook's (future) CoreClass/DomainManager/...
    /// </summary>
    public static class DummyCore
    {
        public static ConnectionManager ConnectionManager { get; set; }

        static DummyCore()
        {
            ConnectionManager = new ConnectionManager();
        }

        public static void StartRemoteProcess(string exe)
        {
            var channelUrl = ConnectionManager.InitializeInterDomainConnection();
            Process.Start(exe, channelUrl);
        }

        public static void InitializeAsRemoteProcess(string channelUrl)
        {
            ConnectionManager.ConnectInterDomainConnection(channelUrl);
        }
    }
}