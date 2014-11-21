// ------------------------------------------------------------------------------
//   <copyright from='2010' to='2015' company='THEHACKERWITHIN.COM'>
//     Copyright (c) TheHackerWithin.COM. All Rights Reserved.
// 
//     Please look in the accompanying license.htm file for the license that 
//     applies to this source code. (a copy can also be found at: 
//     http://www.thehackerwithin.com/license.htm)
//   </copyright>
// -------------------------------------------------------------------------------
namespace QuestorDLL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Runtime.InteropServices;
    using EasyHook;
    using System.Threading;

    public class Main : IEntryPoint
    {
        public Main(EasyHook.RemoteHooking.IContext InContext, string questorParameters)
        {

        }

        public void Run(EasyHook.RemoteHooking.IContext InContext, string questorParameters)
        {
            //Initialize here, hook onframe

            while (true)
                Thread.Sleep(50);
        }   
    }
}
