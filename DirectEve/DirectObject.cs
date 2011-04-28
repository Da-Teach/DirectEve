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
    public class DirectObject
    {
        internal DirectObject(DirectEve directEve)
        {
            DirectEve = directEve;
            PySharp = directEve.PySharp;
        }

        internal DirectEve DirectEve { get; private set; }
        internal PySharp.PySharp PySharp { get; private set; }
    }
}