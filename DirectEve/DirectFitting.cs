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

    public class DirectFitting : DirectObject
    {
        private PyObject _pyFitting;

        internal DirectFitting(DirectEve directEve, long ownerId, int fittingId, PyObject pyFitting) : base(directEve)
        {
            _pyFitting = pyFitting;

            OwnerId = ownerId;
            FittingId = fittingId;
            Name = (string) pyFitting.Attribute("name");
            ShipTypeId = (int) pyFitting.Attribute("shipTypeID");
            Modules = new List<DirectItem>();
            foreach (var module in pyFitting.Attribute("fitData").ToList())
            {
                var item = new DirectItem(directEve);
                item.TypeId = (int) module.Item(0);
                item.FlagId = (int) module.Item(1);
                item.Quantity = (int) module.Item(2);
                item.OwnerId = (int) OwnerId;
                Modules.Add(item);
            }
        }

        public long OwnerId { get; private set; }
        public int FittingId { get; private set; }
        public int ShipTypeId { get; private set; }
        public string Name { get; private set; }
        public List<DirectItem> Modules { get; private set; }

        /// <summary>
        ///     Try to fit this fitting
        /// </summary>
        /// <returns></returns>
        public bool Fit()
        {
            var charId = DirectEve.Session.CharacterId;
            if (charId == null)
                return false;

            return DirectEve.ThreadedLocalSvcCall("fittingSvc", "LoadFitting", charId.Value, _pyFitting.Attribute("fittingID"));
        }
    }
}