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

    public class DirectItemAttributes : DirectObject
    {
        private Dictionary<string, PyObject> _attributes;

        /// <summary>
        ///   Attribute cache
        /// </summary>
        private Dictionary<string, PyObject> _chargedAttributes;

        internal DirectItemAttributes(DirectEve directEve, long itemId)
            : this(directEve, directEve.PySharp.From(itemId))
        {
        }

        internal DirectItemAttributes(DirectEve directEve, PyObject itemId)
            : base(directEve)
        {
            _chargedAttributes = directEve.GetLocalSvc("godma").Attribute("stateManager").Attribute("chargedAttributesByItemAttribute").DictionaryItem(itemId).ToDictionary<string>();
            _attributes = directEve.GetLocalSvc("godma").Attribute("stateManager").Attribute("attributesByItemAttribute").DictionaryItem(itemId).ToDictionary<string>();

            if (_attributes.Keys.Count > 0)
                return;

            // Apparently we did not find any attributes, try through dogmaLocation.dogmaItems
            var dogmaLocation = directEve.GetLocalSvc("clientDogmaIM").Attribute("dogmaLocation");
            var dogmaItem = dogmaLocation.Attribute("dogmaItems").DictionaryItem(itemId);
            if (!dogmaItem.IsValid)
                return;

            // Get the attribute-names dictionary
            var attributeNames = dogmaLocation.Attribute("dogmaStaticMgr").Attribute("attributes");

            // Convert new-style to old-style attributes
            foreach (var item in dogmaItem.Attribute("attributes").ToDictionary<int>())
            {
                var attributeName = (string) attributeNames.DictionaryItem(item.Key).Attribute("attributeName");
                var cachedValue = dogmaItem.Attribute("attributeCache").DictionaryItem(item.Key);
                _attributes.Add(attributeName, cachedValue.IsValid ? cachedValue : item.Value);
            }
        }

        /// <summary>
        ///   Returns a list of all the attributes associated with the item
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///   object-types are unknown types
        /// </remarks>
        public Dictionary<string, Type> GetAttributes()
        {
            var result = new Dictionary<string, Type>();
            foreach (var attribute in _attributes)
            {
                var type = typeof (object);

                switch (attribute.Value.GetPyType())
                {
                    case PyType.BoolType:
                        type = typeof (bool);
                        break;
                    case PyType.IntType:
                        type = typeof (int);
                        break;
                    case PyType.LongType:
                        type = typeof (long);
                        break;
                    case PyType.FloatType:
                        type = typeof (double);
                        break;
                    case PyType.StringType:
                    case PyType.UnicodeType:
                        type = typeof (string);
                        break;
                }

                result[attribute.Key] = type;
            }

            return result;
        }

        /// <summary>
        ///   Get an attribute
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "key"></param>
        /// <returns></returns>
        public T TryGet<T>(string key)
        {
            if (_chargedAttributes.ContainsKey(key))
            {
                var value = _chargedAttributes[key];
                var charge = DirectEve.GetLocalSvc("godma").Attribute("stateManager").Call("GetChargeValue", value.Item(0), value.Item(1), value.Item(2), value.Item(3));
                if (typeof (T) == typeof (bool))
                    return (T) (object) charge.ToBool();
                if (typeof (T) == typeof (string))
                    return (T) (object) charge.ToUnicodeString();
                if (typeof (T) == typeof (int))
                    return (T) (object) charge.ToInt();
                if (typeof (T) == typeof (long))
                    return (T) (object) charge.ToLong();
                if (typeof (T) == typeof (float))
                    return (T) (object) charge.ToFloat();
                if (typeof (T) == typeof (double))
                    return (T) (object) charge.ToDouble();
                if (typeof (T) == typeof (DateTime))
                    return (T) (object) charge.ToDateTime();
            }

            if (_attributes.ContainsKey(key))
            {
                if (typeof (T) == typeof (bool))
                    return (T) (object) _attributes[key].ToBool();
                if (typeof (T) == typeof (string))
                    return (T) (object) _attributes[key].ToUnicodeString();
                if (typeof (T) == typeof (int))
                    return (T) (object) _attributes[key].ToInt();
                if (typeof (T) == typeof (long))
                    return (T) (object) _attributes[key].ToLong();
                if (typeof (T) == typeof (float))
                    return (T) (object) _attributes[key].ToFloat();
                if (typeof (T) == typeof (double))
                    return (T) (object) _attributes[key].ToDouble();
                if (typeof (T) == typeof (DateTime))
                    return (T) (object) _attributes[key].ToDateTime();
            }
            return default(T);
        }
    }
}