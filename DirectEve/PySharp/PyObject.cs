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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;

    public partial class PyObject
    {
        /// <summary>
        ///   Attribute cache
        /// </summary>
        private Dictionary<string, PyObject> _attributeCache;

        /// <summary>
        ///   Dictionary cache (used by DictionaryItem)
        /// </summary>
        private Dictionary<PyObject, PyObject> _dictionaryCache;

        /// <summary>
        ///   Item cache (used by Item)
        /// </summary>
        private Dictionary<int, PyObject> _itemCache;

        /// <summary>
        ///   Store if its a new reference
        /// </summary>
        private bool _newReference;

        /// <summary>
        ///   Reference to the actual python object
        /// </summary>
        private IntPtr _pyReference;

        /// <summary>
        ///   Reference to the overall PySharp object
        /// </summary>
        private PySharp _pySharp;

        /// <summary>
        ///   PyType cache
        /// </summary>
        private PyType? _pyType;

        /// <summary>
        ///   Create a PyObject
        /// </summary>
        /// <param name = "pySharp">The main PySharp object</param>
        /// <param name = "pyReference">The Python Reference</param>
        /// <param name = "newReference">Is this a new reference? (e.g. did the reference counter get increased?)</param>
        internal PyObject(PySharp pySharp, IntPtr pyReference, bool newReference)
        {
            _pyReference = pyReference;
            _newReference = newReference;
            _pySharp = pySharp;

            if (pySharp != null && _newReference)
                pySharp.AddReference(this);

            HandlePythonError();

            if (!IsValid)
                return;

            // Only build up cache if it actually a valid object
            _attributeCache = new Dictionary<string, PyObject>();
            _dictionaryCache = new Dictionary<PyObject, PyObject>();
            _itemCache = new Dictionary<int, PyObject>();
        }

        /// <summary>
        ///   Is this PyObject valid?
        /// </summary>
        /// <remarks>
        ///   Both null and none values are considered invalid
        /// </remarks>
        public bool IsValid
        {
            get { return !IsNull && !IsNone; }
        }

        /// <summary>
        ///   Is this PyObject Null?
        /// </summary>
        public bool IsNull
        {
            get { return _pyReference == IntPtr.Zero; }
        }

        /// <summary>
        ///   Is this PyObject a PyNone?
        /// </summary>
        public bool IsNone
        {
            get { return _pyReference == Py.PyNoneStruct; }
        }

        /// <summary>
        ///   Return the Python Reference Count
        /// </summary>
        public int ReferenceCount
        {
            get { return Py.GetRefCnt(this); }
        }

        /// <summary>
        ///   Cast a PyObject to a IntPtr
        /// </summary>
        /// <param name = "pyObject"></param>
        /// <returns></returns>
        public static implicit operator IntPtr(PyObject pyObject)
        {
            return pyObject._pyReference;
        }

        /// <summary>
        ///   Release the PyObject's internal reference
        /// </summary>
        public void Release()
        {
            if (!IsNull && _newReference)
                Py.Py_DecRef(_pyReference);

            _pyReference = IntPtr.Zero;
        }

        /// <summary>
        ///   Attach the PyObject to a new PySharp object
        /// </summary>
        /// <param name = "pySharp">New PySharp object</param>
        /// <returns>A new copy of itself</returns>
        public PyObject Attach(PySharp pySharp)
        {
            if (_newReference)
                Py.Py_IncRef(_pyReference);

            return new PyObject(pySharp, _pyReference, _newReference);
        }

        /// <summary>
        ///   Return python type
        /// </summary>
        /// <returns></returns>
        public PyType GetPyType()
        {
            if (IsNull)
                _pyType = PyType.Invalid;

            if (IsNone)
                _pyType = PyType.NoneType;

            if (!_pyType.HasValue)
                _pyType = Py.GetPyType(this);

            return _pyType.Value;
        }

        /// <summary>
        ///   Returns an attribute from the current Python object
        /// </summary>
        /// <param name = "attribute"></param>
        /// <returns></returns>
        public PyObject Attribute(string attribute)
        {
            if (!IsValid || string.IsNullOrEmpty(attribute))
                return PySharp.PyZero;

            PyObject result;
            if (!_attributeCache.TryGetValue(attribute, out result))
            {
                result = new PyObject(_pySharp, Py.PyObject_GetAttrString(this, attribute), true);
                _attributeCache[attribute] = result;
            }
            return result;
        }

        /// <summary>
        ///   Returns a dictionary of all attributes within the current Python object
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, PyObject> Attributes()
        {
            if (!IsValid)
                return new Dictionary<string, PyObject>();

            return new PyObject(_pySharp, Py.PyObject_Dir(this), true).ToList<string>().ToDictionary(attr => attr, Attribute);
        }

        /// <summary>
        /// Logs debug information about this PyObject
        /// </summary>
        public void LogObject()
        {
            Debugger.Log(0, "", string.Format("\nDumping attributes of {0}...\n",this.Repr));
            foreach (KeyValuePair<string, PyObject> pair in this.Attributes())
            {
                Debugger.Log(0, "", string.Format("  {0} : {1}\n", pair.Key, pair.Value.Repr));
            }
        }

        /// <summary>
        ///   Returns a dictionary item from the current Python object
        /// </summary>
        /// <param name = "key"></param>
        /// <returns></returns>
        public PyObject DictionaryItem(int key)
        {
            if (_pySharp == null)
                return PySharp.PyZero;

            return DictionaryItem(_pySharp.From(key));
        }

        /// <summary>
        ///   Returns a dictionary item from the current Python object
        /// </summary>
        /// <param name = "key"></param>
        /// <returns></returns>
        public PyObject DictionaryItem(long key)
        {
            if (_pySharp == null)
                return PySharp.PyZero;

            return DictionaryItem(_pySharp.From(key));
        }

        /// <summary>
        ///   Returns a dictionary item from the current Python object
        /// </summary>
        /// <param name = "key"></param>
        /// <returns></returns>
        public PyObject DictionaryItem(string key)
        {
            if (_pySharp == null)
                return PySharp.PyZero;

            return DictionaryItem(_pySharp.From(key));
        }

        /// <summary>
        ///   Returns a dictionary item from the current Python object
        /// </summary>
        /// <param name = "key"></param>
        /// <returns></returns>
        public PyObject DictionaryItem(PyObject key)
        {
            if (!IsValid || key.IsNull)
                return PySharp.PyZero;

            PyObject result;
            if (!_dictionaryCache.TryGetValue(key, out result))
            {
                PyType thisType = this.GetPyType();
                if (thisType == PyType.DerivedDictType || thisType == PyType.DictType)
                {
                    // This only works for objects derived from PyDict_Type
                    result = new PyObject(_pySharp, Py.PyDict_GetItem(this, key), false);
                }
                else
                {
                    // This works for any container object that defines __getitem__
                    // If __getitem__ is not defined then result will be PySharp.PyZero
                    result = new PyObject(_pySharp, this.Call("__getitem__", key), false);
                }
                _dictionaryCache[key] = result;
            }
            return result;
        }

        /// <summary>
        ///   Returns a list item from the current Python object
        /// </summary>
        /// <param name = "index"></param>
        /// <returns></returns>
        public PyObject Item(int index)
        {
            return Item(index, GetPyType());
        }

        /// <summary>
        ///   Returns a list item from the current Python object
        /// </summary>
        /// <param name = "index"></param>
        /// <param name = "type">Force the PyType to List or Tuple</param>
        /// <returns></returns>
        public PyObject Item(int index, PyType type)
        {
            if (!IsValid)
                return PySharp.PyZero;

            var getItem = (type == PyType.TupleType || type == PyType.DerivedTupleType) ? (Func<IntPtr, int, IntPtr>) Py.PyTuple_GetItem : Py.PyList_GetItem;

            PyObject result;
            if (!_itemCache.TryGetValue(index, out result))
            {
                result = new PyObject(_pySharp, getItem(this, index), false);
                _itemCache[index] = result;
            }
            return result;
        }

        /// <summary>
        ///   Returns the size of the list or tuple
        /// </summary>
        /// <returns></returns>
        public int Size()
        {
            return Size(GetPyType());
        }

        /// <summary>
        ///   Returns the size of the given type (tuple, otherwise list)
        /// </summary>
        /// <param name = "type"></param>
        /// <returns></returns>
        public int Size(PyType type)
        {
            if (!IsValid)
                return -1;

            try
            {
                var getSize = (type == PyType.TupleType || type == PyType.DerivedTupleType) ? (Func<IntPtr, int>) Py.PyTuple_Size : Py.PyList_Size;
                return getSize(this);
            }
            finally
            {
                HandlePythonError();
            }
        }

        /// <summary>
        ///   Call a python function
        /// </summary>
        /// <param name = "function"></param>
        /// <param name = "parms"></param>
        /// <returns></returns>
        public PyObject Call(string function, params object[] parms)
        {
            var func = Attribute(function);
            return func.CallThis(parms);
        }

        /// <summary>
        ///   Call a python function
        /// </summary>
        /// <param name = "function"></param>
        /// <param name = "keywords"></param>
        /// <param name = "parms"></param>
        /// <returns></returns>
        public PyObject CallWithKeywords(string function, Dictionary<string, object> keywords, params object[] parms)
        {
            var func = Attribute(function);
            return func.CallThisWithKeywords(keywords, parms);
        }

        /// <summary>
        ///   Call this PyObject as a python function
        /// </summary>
        /// <param name = "parms"></param>
        /// <returns></returns>
        public PyObject CallThis(params object[] parms)
        {
            return CallThisWithKeywords(null, parms);
        }

        /// <summary>
        ///   Call this PyObject as a python function
        /// </summary>
        /// <param name = "keywords"></param>
        /// <param name = "parms"></param>
        /// <returns></returns>
        public PyObject CallThisWithKeywords(Dictionary<string, object> keywords, params object[] parms)
        {
            if (!IsValid)
                return PySharp.PyZero;

            if (_pySharp == null)
                throw new NotImplementedException();

            var pyKeywords = PySharp.PyZero;
            if (keywords != null && keywords.Keys.Any())
            {
                pyKeywords = new PyObject(_pySharp, Py.PyDict_New(), true);
                foreach (var item in keywords)
                {
                    var pyValue = _pySharp.From(item.Value);

                    if (pyValue == null || pyValue.IsNull)
                        throw new NotImplementedException();

                    Py.PyDict_SetItem(pyKeywords, _pySharp.From(item.Key), pyValue);
                }
            }

            var pyParms = new List<PyObject>();
            foreach (var parm in parms)
            {
                var pyParm = _pySharp.From(parm);

                if (pyParm == null || pyParm.IsNull)
                    throw new NotImplementedException();

                // Fail if any parameter is invalid (PyNone is a valid parameter)
                if (pyParm.IsNull)
                    return PySharp.PyZero;

                pyParms.Add(pyParm);
            }

            var format = "(" + string.Join("", pyParms.Select(dummy => "O").ToArray()) + ")";
            PyObject pyArgs = null;
            if (pyParms.Count == 0)
                pyArgs = new PyObject(_pySharp, Py.Py_BuildValue(format), true);
            if (pyParms.Count == 1)
                pyArgs = new PyObject(_pySharp, Py.Py_BuildValue(format, pyParms[0]), true);
            if (pyParms.Count == 2)
                pyArgs = new PyObject(_pySharp, Py.Py_BuildValue(format, pyParms[0], pyParms[1]), true);
            if (pyParms.Count == 3)
                pyArgs = new PyObject(_pySharp, Py.Py_BuildValue(format, pyParms[0], pyParms[1], pyParms[2]), true);
            if (pyParms.Count == 4)
                pyArgs = new PyObject(_pySharp, Py.Py_BuildValue(format, pyParms[0], pyParms[1], pyParms[2], pyParms[3]), true);
            if (pyParms.Count == 5)
                pyArgs = new PyObject(_pySharp, Py.Py_BuildValue(format, pyParms[0], pyParms[1], pyParms[2], pyParms[3], pyParms[4]), true);
            if (pyParms.Count == 6)
                pyArgs = new PyObject(_pySharp, Py.Py_BuildValue(format, pyParms[0], pyParms[1], pyParms[2], pyParms[3], pyParms[4], pyParms[5]), true);
            if (pyParms.Count == 7)
                pyArgs = new PyObject(_pySharp, Py.Py_BuildValue(format, pyParms[0], pyParms[1], pyParms[2], pyParms[3], pyParms[4], pyParms[5], pyParms[6]), true);
            if (pyParms.Count == 8)
                pyArgs = new PyObject(_pySharp, Py.Py_BuildValue(format, pyParms[0], pyParms[1], pyParms[2], pyParms[3], pyParms[4], pyParms[5], pyParms[6], pyParms[7]), true);

            if (pyArgs == null)
                throw new NotImplementedException();

            return new PyObject(_pySharp, Py.PyEval_CallObjectWithKeywords(this, pyArgs, pyKeywords), true);
        }

        /// <summary>
        ///   Return the PyObject as a string
        /// </summary>
        /// <returns></returns>
        public string ToUnicodeString()
        {
            try
            {
                if (!IsValid)
                    return null;

                // Manually convert from buffers to string
                if (GetPyType() == PyType.UnicodeType)
                {
                    var size = Py.PyUnicodeUCS2_GetSize(this);
                    if (size <= 0)
                        return null;

                    var ptr = Py.PyUnicodeUCS2_AsUnicode(this);
                    if (ptr == IntPtr.Zero)
                        return null;

                    return Marshal.PtrToStringUni(ptr, size);
                }
                else
                {
                    var size = Py.PyString_Size(this);
                    if (size <= 0)
                        return null;

                    var ptr = Py.PyString_AsString(this);
                    if (ptr == IntPtr.Zero)
                        return null;

                    return Marshal.PtrToStringAnsi(ptr, size);
                }
            }
            finally
            {
                HandlePythonError();
            }
        }

        /// <summary>
        ///   Cast a PyObject to a string
        /// </summary>
        /// <param name = "pyObject"></param>
        /// <returns></returns>
        public static explicit operator string(PyObject pyObject)
        {
            return pyObject.ToUnicodeString();
        }

        /// <summary>
        ///   Returns the PyObject as a bool
        /// </summary>
        /// <returns></returns>
        public bool ToBool()
        {
            return ToInt() == 1;
        }

        /// <summary>
        ///   Cast a PyObject to an bool
        /// </summary>
        /// <param name = "pyObject"></param>
        /// <returns></returns>
        public static explicit operator bool(PyObject pyObject)
        {
            return pyObject.ToBool();
        }

        /// <summary>
        ///   Cast a PyObject to a nullable bool
        /// </summary>
        /// <param name = "pyObject"></param>
        /// <returns></returns>
        public static explicit operator bool?(PyObject pyObject)
        {
            return pyObject.IsValid ? pyObject.ToBool() : (bool?) null;
        }

        /// <summary>
        ///   Returns the PyObject as an integer
        /// </summary>
        /// <returns></returns>
        public int ToInt()
        {
            try
            {
                return IsValid ? Py.PyLong_AsLong(this) : 0;
            }
            finally
            {
                HandlePythonError();
            }
        }

        /// <summary>
        ///   Cast a PyObject to an integer
        /// </summary>
        /// <param name = "pyObject"></param>
        /// <returns></returns>
        public static explicit operator int(PyObject pyObject)
        {
            return pyObject.ToInt();
        }

        /// <summary>
        ///   Cast a PyObject to a nullable integer
        /// </summary>
        /// <param name = "pyObject"></param>
        /// <returns></returns>
        public static explicit operator int?(PyObject pyObject)
        {
            return pyObject.IsValid ? pyObject.ToInt() : (int?) null;
        }

        /// <summary>
        ///   Returns the PyObject as a long
        /// </summary>
        /// <returns></returns>
        public long ToLong()
        {
            try
            {
                return IsValid ? Py.PyLong_AsLongLong(this) : 0;
            }
            finally
            {
                HandlePythonError();
            }
        }

        /// <summary>
        ///   Cast a PyObject to a nullable long
        /// </summary>
        /// <param name = "pyObject"></param>
        /// <returns></returns>
        public static explicit operator long?(PyObject pyObject)
        {
            return pyObject.IsValid ? pyObject.ToLong() : (long?) null;
        }

        /// <summary>
        ///   Cast a PyObject to an integer
        /// </summary>
        /// <param name = "pyObject"></param>
        /// <returns></returns>
        public static explicit operator long(PyObject pyObject)
        {
            return pyObject.ToLong();
        }

        /// <summary>
        ///   Returns the PyObject as a double
        /// </summary>
        /// <returns></returns>
        public double ToDouble()
        {
            try
            {
                return IsValid ? Py.PyFloat_AsDouble(this) : 0;
            }
            finally
            {
                HandlePythonError();
            }
        }

        /// <summary>
        ///   Returns the PyObject as a float
        /// </summary>
        /// <returns></returns>
        public float ToFloat()
        {
            try
            {
                return IsValid ? (float) Py.PyFloat_AsDouble(this) : 0;
            }
            finally
            {
                HandlePythonError();
            }
        }

        /// <summary>
        ///   Cast a PyObject to an integer
        /// </summary>
        /// <param name = "pyObject"></param>
        /// <returns></returns>
        public static explicit operator double(PyObject pyObject)
        {
            return pyObject.ToDouble();
        }

        /// <summary>
        ///   Cast a PyObject to a nullable long
        /// </summary>
        /// <param name = "pyObject"></param>
        /// <returns></returns>
        public static explicit operator double?(PyObject pyObject)
        {
            return pyObject.IsValid ? pyObject.ToDouble() : (double?) null;
        }

        /// <summary>
        ///   Cast a PyObject to an integer
        /// </summary>
        /// <param name = "pyObject"></param>
        /// <returns></returns>
        public static explicit operator float(PyObject pyObject)
        {
            return pyObject.ToFloat();
        }

        /// <summary>
        ///   Cast a PyObject to a nullable long
        /// </summary>
        /// <param name = "pyObject"></param>
        /// <returns></returns>
        public static explicit operator float?(PyObject pyObject)
        {
            return pyObject.IsValid ? pyObject.ToFloat() : (float?) null;
        }

        /// <summary>
        ///   Returns the PyObject as a DateTime
        /// </summary>
        /// <returns></returns>
        public DateTime ToDateTime()
        {
            return new DateTime(1601, 1, 1).AddMilliseconds(ToLong()/10000d);
        }

        /// <summary>
        ///   Cast a PyObject to an integer
        /// </summary>
        /// <param name = "pyObject"></param>
        /// <returns></returns>
        public static explicit operator DateTime(PyObject pyObject)
        {
            return pyObject.ToDateTime();
        }

        /// <summary>
        ///   Cast a PyObject to a nullable long
        /// </summary>
        /// <param name = "pyObject"></param>
        /// <returns></returns>
        public static explicit operator DateTime?(PyObject pyObject)
        {
            return pyObject.IsValid ? pyObject.ToDateTime() : (DateTime?) null;
        }

        /// <summary>
        ///   Returns the PyObject as a list
        /// </summary>
        /// <returns></returns>
        public List<PyObject> ToList()
        {
            return ToList<PyObject>();
        }

        /// <summary>
        ///   Cast a PyObject to a List
        /// </summary>
        /// <param name = "pyObject"></param>
        /// <returns></returns>
        public static explicit operator List<PyObject>(PyObject pyObject)
        {
            return pyObject.ToList();
        }

        /// <summary>
        ///   Cast a PyObject to a List
        /// </summary>
        /// <param name = "pyObject"></param>
        /// <returns></returns>
        public static explicit operator List<int>(PyObject pyObject)
        {
            return pyObject.ToList<int>();
        }

        /// <summary>
        ///   Cast a PyObject to a List
        /// </summary>
        /// <param name = "pyObject"></param>
        /// <returns></returns>
        public static explicit operator List<long>(PyObject pyObject)
        {
            return pyObject.ToList<long>();
        }

        /// <summary>
        ///   Cast a PyObject to a List
        /// </summary>
        /// <param name = "pyObject"></param>
        /// <returns></returns>
        public static explicit operator List<string>(PyObject pyObject)
        {
            return pyObject.ToList<string>();
        }

        /// <summary>
        ///   Returns the PyObject as a list
        /// </summary>
        /// <returns></returns>
        public List<T> ToList<T>()
        {
            var result = new List<T>();
            if (!IsValid)
                return result;

            var size = Size();
            for (var i = 0; i < size; i++)
            {
                var item = Item(i);

                object oItem = null;
                if (typeof (T) == typeof (int))
                    oItem = item.ToInt();
                if (typeof (T) == typeof (long))
                    oItem = item.ToLong();
                if (typeof (T) == typeof (float))
                    oItem = item.ToFloat();
                if (typeof (T) == typeof (double))
                    oItem = item.ToDouble();
                if (typeof (T) == typeof (string))
                    oItem = item.ToUnicodeString();
                if (typeof (T) == typeof (PyObject))
                    oItem = item;

                if (oItem == null)
                    continue;

                result.Add((T) oItem);
            }

            return result;
        }

        /// <summary>
        ///   Returns the PyObject as a dictionary
        /// </summary>
        /// <returns></returns>
        public Dictionary<PyObject, PyObject> ToDictionary()
        {
            return ToDictionary<PyObject>();
        }

        /// <summary>
        ///   Cast a PyObject to a dictionary
        /// </summary>
        /// <returns></returns>
        public Dictionary<TKey, PyObject> ToDictionary<TKey>()
        {
            var result = new Dictionary<TKey, PyObject>();
            if (!IsValid)
                return result;

            var keys = Call("keys").ToList();
            foreach (var key in keys)
            {
                object oKey = null;

                if (typeof (TKey) == typeof (int))
                    oKey = key.ToInt();
                if (typeof (TKey) == typeof (long))
                    oKey = key.ToLong();
                if (typeof (TKey) == typeof (float))
                    oKey = key.ToFloat();
                if (typeof (TKey) == typeof (double))
                    oKey = key.ToDouble();
                if (typeof (TKey) == typeof (string))
                    oKey = key.ToUnicodeString();
                if (typeof (TKey) == typeof (PyObject))
                    oKey = key;

                if (oKey == null)
                    continue;

                result[(TKey) oKey] = DictionaryItem(key);
            }

            return result;
        }

        /// <summary>
        ///   Cast a PyObject to a dictionary
        /// </summary>
        /// <param name = "pyObject"></param>
        /// <returns></returns>
        public static explicit operator Dictionary<PyObject, PyObject>(PyObject pyObject)
        {
            return pyObject.ToDictionary();
        }

        /// <summary>
        ///   Cast a PyObject to a dictionary
        /// </summary>
        /// <param name = "pyObject"></param>
        /// <returns></returns>
        public static explicit operator Dictionary<int, PyObject>(PyObject pyObject)
        {
            return pyObject.ToDictionary<int>();
        }

        /// <summary>
        ///   Cast a PyObject to a dictionary
        /// </summary>
        /// <param name = "pyObject"></param>
        /// <returns></returns>
        public static explicit operator Dictionary<long, PyObject>(PyObject pyObject)
        {
            return pyObject.ToDictionary<long>();
        }

        /// <summary>
        ///   Cast a PyObject to a dictionary
        /// </summary>
        /// <param name = "pyObject"></param>
        /// <returns></returns>
        public static explicit operator Dictionary<string, PyObject>(PyObject pyObject)
        {
            return pyObject.ToDictionary<string>();
        }

        /// <summary>
        ///   Set an attribute value
        /// </summary>
        /// <typeparam name = "T"></typeparam>
        /// <param name = "attribute"></param>
        /// <param name = "value"></param>
        /// <returns></returns>
        public bool SetAttribute<T>(string attribute, T value)
        {
            var pyValue = PySharp.PyZero;

            var oValue = (object) value;
            if (value is PyObject)
                pyValue = (PyObject) oValue;

            // Only allow type conversions if we have a PySharp object reference
            if (_pySharp != null)
            {
                if (oValue is bool)
                    pyValue = _pySharp.From((bool) oValue);
                if (oValue is int)
                    pyValue = _pySharp.From((int) oValue);
                if (oValue is long)
                    pyValue = _pySharp.From((long) oValue);
                if (oValue is float)
                    pyValue = _pySharp.From((float) oValue);
                if (oValue is double)
                    pyValue = _pySharp.From((double) oValue);
                if (oValue is string)
                    pyValue = _pySharp.From((string) oValue);
                if (oValue is IEnumerable<PyObject>)
                    pyValue = _pySharp.From((IEnumerable<PyObject>) oValue);
                if (oValue is IEnumerable<int>)
                    pyValue = _pySharp.From((IEnumerable<int>) oValue);
                if (oValue is IEnumerable<long>)
                    pyValue = _pySharp.From((IEnumerable<long>) oValue);
                if (oValue is IEnumerable<float>)
                    pyValue = _pySharp.From((IEnumerable<float>) oValue);
                if (oValue is IEnumerable<double>)
                    pyValue = _pySharp.From((IEnumerable<double>) oValue);
                if (oValue is IEnumerable<string>)
                    pyValue = _pySharp.From((IEnumerable<string>) oValue);
            }

            try
            {
                if (IsValid && !pyValue.IsNull)
                    return Py.PyObject_SetAttrString(this, attribute, pyValue) != -1;

                return false;
            }
            finally
            {
                HandlePythonError();
            }
        }

        /// <summary>
        ///   Clear this PyObject (the PyObject must be a List, Tuple or Dictionary)
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        ///   For a list attribute, the list is cleared
        ///   For a Dictionary attribute, the dictionary is cleared
        /// </remarks>
        public bool Clear()
        {
            return Clear(GetPyType());
        }

        /// <summary>
        ///   Clear this PyObject (the PyObject must be a List or Dictionary)
        /// </summary>
        /// <param name = "pyType">Force this Python Type</param>
        /// <returns></returns>
        /// <remarks>
        ///   For a list attribute, the list is cleared
        ///   For a Dictionary attribute, the dictionary is cleared
        /// </remarks>
        public bool Clear(PyType pyType)
        {
            try
            {
                switch (pyType)
                {
                    case PyType.ListType:
                    case PyType.DerivedListType:
                        return Py.PyList_SetSlice(this, 0, Size() - 1, PySharp.PyZero) == 0;

                    case PyType.DictType:
                    case PyType.DictProxyType:
                    case PyType.DerivedDictType:
                    case PyType.DerivedDictProxyType:
                        return ToDictionary().All(item => Py.PyDict_DelItem(this, item.Key) == 0);
                }

                return false;
            }
            finally
            {
                HandlePythonError();
            }
        }

        /// <summary>
        ///   Handle a python error (e.g. clear error)
        /// </summary>
        /// <remarks>
        ///   This checks if an error actually occured and clears the error
        /// </remarks>
        private void HandlePythonError()
        {
            // TODO: Save the python error to a log file?
            if (Py.PyErr_Occurred() != IntPtr.Zero)
                Py.PyErr_Clear();

            return;
        }

        private string Repr
        {
            get
            {
                return (string)new PyObject(_pySharp, Py.PyObject_Repr(this), true);
            }
        }
    }
}