// ------------------------------------------------------------------------------
//   <copyright from='2010' to='2015' company='THEHACKERWITHIN.COM'>
//     Copyright (c) TheHackerWithin.COM. All Rights Reserved.
// 
//     Please look in the accompanying license.htm file for the license that 
//     applies to this source code. (a copy can also be found at: 
//     http://www.thehackerwithin.com/license.htm)
//   </copyright>
// -------------------------------------------------------------------------------
namespace DirectEve.PySharp
{
    using System;

    internal static class IntPtrExtension
    {
        /// <summary>
        ///   Add an offset to a IntPtr
        /// </summary>
        /// <param name = "basePtr">Base IntPtr</param>
        /// <param name = "offset">Offset</param>
        /// <returns>New IntPtr</returns>
        /// <remarks>
        ///   This is unsafe because there are no managed checks preventing this to read invalid memory
        /// </remarks>
        public static IntPtr Add(this IntPtr basePtr, int offset)
        {
            return (IntPtr) (basePtr.ToInt64() + offset);
        }
    }
}