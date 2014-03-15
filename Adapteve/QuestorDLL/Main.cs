using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using EasyHook;
using System.Threading;

namespace QuestorDLL
{
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
