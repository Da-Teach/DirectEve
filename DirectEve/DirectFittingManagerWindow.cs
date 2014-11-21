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
    using PySharp;

    public class DirectFittingManagerWindow : DirectWindow
    {
        private List<DirectFitting> _fittings;

        internal DirectFittingManagerWindow(DirectEve directEve, PyObject pyWindow)
            : base(directEve, pyWindow)
        {
            var charId = DirectEve.Session.CharacterId;
            IsReady = charId != null && DirectEve.GetLocalSvc("fittingSvc").Attribute("fittings").DictionaryItem(charId.Value).IsValid;
        }

        public bool IsReady { get; internal set; }

        /// <summary>
        ///     List all your saved fittings
        /// </summary>
        /// <remarks>
        ///     Only personal fittings are listed
        /// </remarks>
        public List<DirectFitting> Fittings
        {
            get
            {
                var charId = DirectEve.Session.CharacterId;
                if (_fittings == null && charId != null)
                {
                    _fittings = new List<DirectFitting>();
                    foreach (var fitting in DirectEve.GetLocalSvc("fittingSvc").Attribute("fittings").DictionaryItem(charId.Value).ToDictionary<int>())
                    {
                        _fittings.Add(new DirectFitting(DirectEve, charId.Value, fitting.Key, fitting.Value));
                    }
                }

                return _fittings;
            }
        }
    }
}