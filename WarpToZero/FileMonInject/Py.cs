// ------------------------------------------------------------------------------
//   <copyright from='2010' to='2015' company='THEHACKERWITHIN.COM'>
//     Copyright (c) TheHackerWithin.COM. All Rights Reserved.
// 
//     Please look in the accompanying license.htm file for the license that 
//     applies to this source code. (a copy can also be found at: 
//     http://www.thehackerwithin.com/license.htm)
//   </copyright>
// -------------------------------------------------------------------------------

namespace AphackInject
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public static class Py
    {
        [DllImport("python27.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int PyLong_AsLong(IntPtr op);

        [DllImport("python27.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyImport_ImportModule(string module);

        [DllImport("python27.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Py_BuildValue(string format);

        [DllImport("python27.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr Py_BuildValue(string format, IntPtr parm1);

        [DllImport("python27.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyDict_GetItem(IntPtr op, IntPtr key);

        [DllImport("python27.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyObject_GetAttrString(IntPtr op, string attr);

        [DllImport("python27.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyTuple_GetItem(IntPtr op, int index);

        [DllImport("python27.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int PyTuple_Size(IntPtr op);

        [DllImport("python27.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern double PyFloat_AsDouble(IntPtr op);

        [DllImport("python27.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyString_FromString(string s);

        [DllImport("python27.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int PyUnicodeUCS2_GetSize(IntPtr op);

        [DllImport("python27.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyUnicodeUCS2_AsUnicode(IntPtr op);

        [DllImport("python27.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int PyString_Size(IntPtr op);

        [DllImport("python27.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyString_AsString(IntPtr op);

        [DllImport("python27.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern int PyList_Size(IntPtr op);

        [DllImport("python27.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyList_GetItem(IntPtr op, int index);

        [DllImport("python27.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern void PyErr_Clear();

        [DllImport("python27.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr PyErr_Occurred();

        [DllImport("python27.dll", CallingConvention = CallingConvention.Cdecl)]
        internal static extern long PyLong_AsLongLong(IntPtr op);

        internal static PyType GetPyType(IntPtr po)
        {
            return GetPyType(po, false);
        }

        internal static PyType GetPyType(IntPtr po, bool derived)
        {
            if (!_initialized)
                InitializeStructures();

            if (po == IntPtr.Zero)
                return PyType.Invalid;

            PyType type;
            var pyType = Marshal.ReadIntPtr(IntPtr.Add(po, 4));
            if (!_types.TryGetValue(pyType, out type))
            {
                type = GetPyType(pyType, true);
                _types.Add(pyType, type);
                return type;
            }

            if (derived)
            {
                try
                {
                    var s = type.ToString();
                    type = (PyType) Enum.Parse(typeof (PyType), "Derived" + s);
                }
                catch
                {
                    return PyType.Unknown;
                }
            }

            return type;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr LoadLibrary(string lpszLib);


        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        private static IntPtr GetProcAddresFunc(string dllname, string function)
        {
            var hDLL = GetModuleHandle(dllname);
            return GetProcAddress(hDLL, function);
        }

        internal static IntPtr GetStruct(string name)
        {
            IntPtr structure;
            if (!_structures.TryGetValue(name, out structure))
            {
                if (_moduleHandle == IntPtr.Zero)
                    _moduleHandle = LoadLibrary("python27.dll");

                structure = GetProcAddress(_moduleHandle, name);
                if (structure == IntPtr.Zero)
                    throw new Exception("Structure " + name + " not found!");

                if (structure != IntPtr.Zero)
                    _structures[name] = structure;
            }
            return structure;
        }

        private static bool _initialized;
        private static IntPtr _moduleHandle;
        private static Dictionary<IntPtr, PyType> _types = new Dictionary<IntPtr, PyType>();
        private static Dictionary<string, IntPtr> _structures = new Dictionary<string, IntPtr>();

        public enum PyType
        {
            Invalid,
            Unknown,
            NoneType,
            BaseObjectType,
            BaseStringType,
            BombType,
            BoolType,
            BufferType,
            ByteArrayIterType,
            ByteArrayType,
            CFrameType,
            CFunctionType,
            CObjectType,
            CStackType,
            CallIterType,
            CapsuleType,
            CellType,
            ChannelTypePtr,
            ClassMethodDescrType,
            ClassMethodType,
            ClassType,
            CodeType,
            ComplexType,
            DictItemsType,
            DictIterItemType,
            DictIterKeyType,
            DictIterValueType,
            DictKeysType,
            DictProxyType,
            DictValuesType,
            DictType,
            EllipsisType,
            EnumType,
            ExcTypeError,
            FileType,
            FlexTypeTypePtr,
            FloatType,
            FrameType,
            FrozenSetType,
            FunctionType,
            GenType,
            GetSetDescrType,
            InstanceType,
            IntType,
            ListType,
            LongType,
            MemberDescrType,
            MemoryViewType,
            MethodDescrType,
            MethodWrapperType,
            MethodType,
            ModuleType,
            NullImporterType,
            ObjectType,
            PropertyType,
            RangeType,
            ReversedType,
            STEntryType,
            SeqIterType,
            SetType,
            SliceType,
            StaticMethodType,
            StringType,
            SuperType,
            TaskletTypePtr,
            TraceBackType,
            TupleType,
            TypeType,
            UnicodeType,
            WrapperDescrType,
            DerivedNoneType,
            DerivedBaseObjectType,
            DerivedBaseStringType,
            DerivedBombType,
            DerivedBoolType,
            DerivedBufferType,
            DerivedByteArrayIterType,
            DerivedByteArrayType,
            DerivedCFrameType,
            DerivedCFunctionType,
            DerivedCObjectType,
            DerivedCStackType,
            DerivedCallIterType,
            DerivedCapsuleType,
            DerivedCellType,
            DerivedChannelTypePtr,
            DerivedClassMethodDescrType,
            DerivedClassMethodType,
            DerivedClassType,
            DerivedCodeType,
            DerivedComplexType,
            DerivedDictItemsType,
            DerivedDictIterItemType,
            DerivedDictIterKeyType,
            DerivedDictIterValueType,
            DerivedDictKeysType,
            DerivedDictProxyType,
            DerivedDictValuesType,
            DerivedDictType,
            DerivedEllipsisType,
            DerivedEnumType,
            DerivedExcTypeError,
            DerivedFileType,
            DerivedFlexTypeTypePtr,
            DerivedFloatType,
            DerivedFrameType,
            DerivedFrozenSetType,
            DerivedFunctionType,
            DerivedGenType,
            DerivedGetSetDescrType,
            DerivedInstanceType,
            DerivedIntType,
            DerivedListType,
            DerivedLongType,
            DerivedMemberDescrType,
            DerivedMemoryViewType,
            DerivedMethodDescrType,
            DerivedMethodWrapperType,
            DerivedMethodType,
            DerivedModuleType,
            DerivedNullImporterType,
            DerivedObjectType,
            DerivedPropertyType,
            DerivedRangeType,
            DerivedReversedType,
            DerivedSTEntryType,
            DerivedSeqIterType,
            DerivedSetType,
            DerivedSliceType,
            DerivedStaticMethodType,
            DerivedStringType,
            DerivedSuperType,
            DerivedTaskletTypePtr,
            DerivedTraceBackType,
            DerivedTupleType,
            DerivedTypeType,
            DerivedUnicodeType,
            DerivedWrapperDescrType,
        }

        private static void InitializeStructures()
        {
            _initialized = true;

            _types.Add(GetStruct("PyBaseObject_Type"), PyType.BaseObjectType);
            _types.Add(GetStruct("PyBaseString_Type"), PyType.BaseStringType);
            _types.Add(GetStruct("PyBomb_Type"), PyType.BombType);
            _types.Add(GetStruct("PyBool_Type"), PyType.BoolType);
            _types.Add(GetStruct("PyBuffer_Type"), PyType.BufferType);
            _types.Add(GetStruct("PyByteArrayIter_Type"), PyType.ByteArrayIterType);
            _types.Add(GetStruct("PyByteArray_Type"), PyType.ByteArrayType);
            _types.Add(GetStruct("PyCFrame_Type"), PyType.CFrameType);
            _types.Add(GetStruct("PyCFunction_Type"), PyType.CFunctionType);
            _types.Add(GetStruct("PyCObject_Type"), PyType.CObjectType);
            _types.Add(GetStruct("PyCStack_Type"), PyType.CStackType);
            _types.Add(GetStruct("PyCallIter_Type"), PyType.CallIterType);
            _types.Add(GetStruct("PyCapsule_Type"), PyType.CapsuleType);
            _types.Add(GetStruct("PyCell_Type"), PyType.CellType);
            _types.Add(GetStruct("PyChannel_TypePtr"), PyType.ChannelTypePtr);
            _types.Add(GetStruct("PyClassMethod_Type"), PyType.ClassMethodType);
            _types.Add(GetStruct("PyClass_Type"), PyType.ClassType);
            _types.Add(GetStruct("PyCode_Type"), PyType.CodeType);
            _types.Add(GetStruct("PyComplex_Type"), PyType.ComplexType);
            _types.Add(GetStruct("PyDictItems_Type"), PyType.DictItemsType);
            _types.Add(GetStruct("PyDictIterItem_Type"), PyType.DictIterItemType);
            _types.Add(GetStruct("PyDictIterKey_Type"), PyType.DictIterKeyType);
            _types.Add(GetStruct("PyDictIterValue_Type"), PyType.DictIterValueType);
            _types.Add(GetStruct("PyDictKeys_Type"), PyType.DictKeysType);
            _types.Add(GetStruct("PyDictProxy_Type"), PyType.DictProxyType);
            _types.Add(GetStruct("PyDictValues_Type"), PyType.DictValuesType);
            _types.Add(GetStruct("PyDict_Type"), PyType.DictType);
            _types.Add(GetStruct("PyEllipsis_Type"), PyType.EllipsisType);
            _types.Add(GetStruct("PyEnum_Type"), PyType.EnumType);
            _types.Add(GetStruct("PyExc_TypeError"), PyType.ExcTypeError);
            _types.Add(GetStruct("PyFile_Type"), PyType.FileType);
            _types.Add(GetStruct("PyFlexType_TypePtr"), PyType.FlexTypeTypePtr);
            _types.Add(GetStruct("PyFloat_Type"), PyType.FloatType);
            _types.Add(GetStruct("PyFrame_Type"), PyType.FrameType);
            _types.Add(GetStruct("PyFrozenSet_Type"), PyType.FrozenSetType);
            _types.Add(GetStruct("PyFunction_Type"), PyType.FunctionType);
            _types.Add(GetStruct("PyGen_Type"), PyType.GenType);
            _types.Add(GetStruct("PyGetSetDescr_Type"), PyType.GetSetDescrType);
            _types.Add(GetStruct("PyInstance_Type"), PyType.InstanceType);
            _types.Add(GetStruct("PyInt_Type"), PyType.IntType);
            _types.Add(GetStruct("PyList_Type"), PyType.ListType);
            _types.Add(GetStruct("PyLong_Type"), PyType.LongType);
            _types.Add(GetStruct("PyMemberDescr_Type"), PyType.MemberDescrType);
            _types.Add(GetStruct("PyMemoryView_Type"), PyType.MemoryViewType);
            _types.Add(GetStruct("PyMethod_Type"), PyType.MethodType);
            _types.Add(GetStruct("PyModule_Type"), PyType.ModuleType);
            _types.Add(GetStruct("PyNullImporter_Type"), PyType.NullImporterType);
            _types.Add(GetStruct("PyObject_Type"), PyType.ObjectType);
            _types.Add(GetStruct("PyProperty_Type"), PyType.PropertyType);
            _types.Add(GetStruct("PyRange_Type"), PyType.RangeType);
            _types.Add(GetStruct("PyReversed_Type"), PyType.ReversedType);
            _types.Add(GetStruct("PySTEntry_Type"), PyType.STEntryType);
            _types.Add(GetStruct("PySeqIter_Type"), PyType.SeqIterType);
            _types.Add(GetStruct("PySet_Type"), PyType.SetType);
            _types.Add(GetStruct("PySlice_Type"), PyType.SliceType);
            _types.Add(GetStruct("PyStaticMethod_Type"), PyType.StaticMethodType);
            _types.Add(GetStruct("PyString_Type"), PyType.StringType);
            _types.Add(GetStruct("PySuper_Type"), PyType.SuperType);
            _types.Add(GetStruct("PyTasklet_TypePtr"), PyType.TaskletTypePtr);
            _types.Add(GetStruct("PyTraceBack_Type"), PyType.TraceBackType);
            _types.Add(GetStruct("PyTuple_Type"), PyType.TupleType);
            _types.Add(GetStruct("PyType_Type"), PyType.TypeType);
            _types.Add(GetStruct("PyUnicode_Type"), PyType.UnicodeType);
            _types.Add(GetStruct("PyWrapperDescr_Type"), PyType.WrapperDescrType);
        }


        private static void HandlePythonError()
        {
            if (PyErr_Occurred() != IntPtr.Zero)
                PyErr_Clear();

            return;
        }

        public static string ToUnicodeString(IntPtr intptr)
        {
            try
            {
                if (intptr == IntPtr.Zero)
                    return null;

                // Manually convert from buffers to string
                if (GetPyType(intptr) == PyType.UnicodeType)
                {
                    var size = PyUnicodeUCS2_GetSize(intptr);
                    if (size <= 0)
                        return null;

                    var ptr = PyUnicodeUCS2_AsUnicode(intptr);
                    if (ptr == IntPtr.Zero)
                        return null;

                    return Marshal.PtrToStringUni(ptr, size);
                }
                else
                {
                    var size = PyString_Size(intptr);
                    if (size <= 0)
                        return null;

                    var ptr = PyString_AsString(intptr);
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
    }
}