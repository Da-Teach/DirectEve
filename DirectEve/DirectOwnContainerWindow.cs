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

    public class DirectOwnContainerWindow : DirectContainerWindow
    {
        internal DirectOwnContainerWindow(DirectEve directEve, PyObject pyWindow) : base(directEve, pyWindow)
        {
        }

        /// <summary>
        ///   Add bookmarks to this container window
        /// </summary>
        /// <param name = "bookmarkIds"></param>
        /// <returns></returns>
        public bool AddBookmarks(IEnumerable<long> bookmarkIds)
        {
            return DirectEve.ThreadedCall(PyWindow.Attribute("AddBookmarks"), bookmarkIds);
        }
    }
}