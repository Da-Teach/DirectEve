// ------------------------------------------------------------------------------
//   <copyright from='2010' to='2015' company='THEHACKERWITHIN.COM'>
//     Copyright (c) TheHackerWithin.COM. All Rights Reserved.
// 
//     Please look in the accompanying license.htm file for the license that 
//     applies to this source code. (a copy can also be found at: 
//     http://www.thehackerwithin.com/license.htm)
//   </copyright>
// -------------------------------------------------------------------------------

namespace PythonBrowser.PySharp
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;

    internal partial class PySharp : DynamicObject
    {
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = Import(binder.Name);
            return true;
        }
    }

    internal partial class PyObject : DynamicObject
    {
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = Attribute(binder.Name);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return SetAttribute(binder.Name, value);
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            Dictionary<string, object> keywords = null;
            if (args.Length > 0 && args[0] is Dictionary<string, object>)
            {
                keywords = (Dictionary<string, object>) args[0];
                args = args.Skip(1).ToArray();
            }

            result = CallThisWithKeywords(keywords, args);
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            Dictionary<string, object> keywords = null;
            if (args.Length > 0 && args[0] is Dictionary<string, object>)
            {
                keywords = (Dictionary<string, object>) args[0];
                args = args.Skip(1).ToArray();
            }

            result = CallWithKeywords(binder.Name, keywords, args);
            return true;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = null;
            if (binder.ReturnType == typeof (bool))
                result = (bool) this;
            if (binder.ReturnType == typeof (bool?))
                result = (bool?) this;
            if (binder.ReturnType == typeof (int))
                result = (int) this;
            if (binder.ReturnType == typeof (int?))
                result = (int?) this;
            if (binder.ReturnType == typeof (long))
                result = (long) this;
            if (binder.ReturnType == typeof (long?))
                result = (long?) this;
            if (binder.ReturnType == typeof (float))
                result = (float) this;
            if (binder.ReturnType == typeof (float?))
                result = (float?) this;
            if (binder.ReturnType == typeof (double))
                result = (double) this;
            if (binder.ReturnType == typeof (double?))
                result = (double?) this;
            if (binder.ReturnType == typeof (DateTime))
                result = (DateTime) this;
            if (binder.ReturnType == typeof (DateTime?))
                result = (DateTime?) this;
            if (binder.ReturnType == typeof (List<PyObject>))
                result = (List<PyObject>) this;
            if (binder.ReturnType == typeof (List<int>))
                result = (List<int>) this;
            if (binder.ReturnType == typeof (List<long>))
                result = (List<long>) this;
            if (binder.ReturnType == typeof (List<string>))
                result = (List<string>) this;
            if (binder.ReturnType == typeof (List<float>))
                result = ToList<float>();
            if (binder.ReturnType == typeof (List<double>))
                result = ToList<double>();
            if (binder.ReturnType == typeof (Dictionary<PyObject, PyObject>))
                result = ToDictionary<PyObject>();
            if (binder.ReturnType == typeof (Dictionary<int, PyObject>))
                result = ToDictionary<int>();
            if (binder.ReturnType == typeof (Dictionary<long, PyObject>))
                result = ToDictionary<long>();
            if (binder.ReturnType == typeof (Dictionary<string, PyObject>))
                result = ToDictionary<string>();
            if (binder.ReturnType == typeof (Dictionary<float, PyObject>))
                result = ToDictionary<float>();
            if (binder.ReturnType == typeof (Dictionary<double, PyObject>))
                result = ToDictionary<double>();
            return result != null;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = PySharp.PyZero;
            if (indexes.Length != 1 && indexes.Length != 2)
                return false;

            var pyType = GetPyType();
            if (indexes.Length == 2 && indexes[1] is PyType)
                pyType = (PyType) indexes[1];

            if (pyType == PyType.DictType)
            {
                if (indexes[0] is int)
                    result = DictionaryItem((int) indexes[0]);
                if (indexes[0] is long)
                    result = DictionaryItem((long) indexes[0]);
                if (indexes[0] is string)
                    result = DictionaryItem((string) indexes[0]);
                if (indexes[0] is PyObject)
                    result = DictionaryItem((PyObject) indexes[0]);
            }
            else
            {
                // First index has be an int
                if (!(indexes[0] is int))
                    return false;

                var index = (int) indexes[0];
                if (pyType == PyType.TupleType || pyType == PyType.DerivedTupleType)
                    result = Item(index, pyType);
                else
                    result = Item(index);
            }
            return true;
        }
    }
}