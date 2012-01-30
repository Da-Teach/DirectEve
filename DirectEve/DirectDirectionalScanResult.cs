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
    using global::DirectEve.PySharp;

    // a scan result is like an entity but we cannot base directly of DirectEntity
    // as it stands today.  Maybe in the future DirectEntity can handle scan results
    // directly.
    public class DirectDirectionalScanResult : DirectInvType
    {
        private PyObject _ball;
        private PyObject _celestialRec;
        private PyObject _slimItem;
        private string _name;
        private int? _itemId;

        internal DirectDirectionalScanResult(DirectEve directEve, PyObject slimItem, PyObject ball, PyObject celestialRec)
            : base(directEve)
        {
            _slimItem = slimItem;
            _ball = ball;
            _celestialRec = celestialRec;

            if(_slimItem.IsValid)
            {
                TypeId = (int) _slimItem.Attribute("typeID");
            }
            else if (_celestialRec.IsValid)
            {
                TypeId = (int)_celestialRec.Attribute("typeID");
            }
        }

        public int ItemID 
        { 
            get
            {
                if (!_itemId.HasValue)
                {
                    if (_slimItem.IsValid)
                    {
                        _itemId = (int?)_slimItem.Attribute("itemID");
                    }
                    else if (_celestialRec.IsValid)
                    {
                        _itemId = (int?)_celestialRec.Attribute("id");
                    }

                }
                return _itemId.Value;
            }
        }

        public string Name
        {
            get
            {
                if (string.IsNullOrEmpty(_name))
                {
                    if (_slimItem.IsValid)
                    {
                        _name = (string)PySharp.Import("uix").Call("GetSlimItemName", _slimItem);
                    }
                    else if (_celestialRec.IsValid)
                    {
                        var c = new DirectConst(DirectEve);
                        _name = (string) PyInvType.Attribute("name");
                        if (this.GroupId == (int) c["groupHarvestableCloud"])
                        {
                            _name = (string)PySharp.Import("localization").Call("GetByLabel", "UI/Inventory/SlimItemNames/SlimHarvestableCloud", _name);
                        }
                        else if (this.CategoryId == (int) c["categoryAsteroid"])
                        {
                            _name = (string)PySharp.Import("localization").Call("GetByLabel", "UI/Inventory/SlimItemNames/SlimAsteroid", _name);
                        }
                        else
                        {
                            _name = DirectEve.GetLocationName(this.ItemID);
                        }
                    }
                }
                return _name;
            }
        }

        public DirectEntity Entity
        {
            get
            {
                DirectEntity entity = null;
                if (_celestialRec.IsValid && _ball.IsValid)
                {
                    var ballpark = DirectEve.GetLocalSvc("michelle").Call("GetBallpark");
                    var slimItem = ballpark.Call("GetInvItem", this.ItemID);
                    entity = new DirectEntity(DirectEve, ballpark, _ball, slimItem, this.ItemID);
                }
                return entity;
            }
        }
    }
}
