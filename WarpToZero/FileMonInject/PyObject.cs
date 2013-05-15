using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AphackInject
{
    public class PyObject
    {
        private IntPtr _pyReference;
        private Py.PyType _type = Py.PyType.Unknown;
        private List<PyObject> _list = new List<PyObject>();
        private List<PyObject> _tuple = new List<PyObject>();
        private string _string = "";
        private int? _integer = null;
        private long? _long = null;
        private double? _float = null;
        private int? _size = null;

        internal PyObject(IntPtr pyReference)
        {
            _pyReference = pyReference;

        }

        public int? Size
        {
            get
            {
                if (Type != Py.PyType.ListType && Type != Py.PyType.TupleType)
                    return null;

                if (_size == null)
                {
                    switch (Type)
                    {
                        case Py.PyType.ListType:
                            _size = Py.PyList_Size(_pyReference);
                            break;

                        case Py.PyType.TupleType:
                            _size = Py.PyTuple_Size(_pyReference);
                            break;
                    }
                }
                return _size;
            }
        }
        public Py.PyType Type
        {
            get
            {
                if (_type != Py.PyType.Unknown)
                    return _type;

                return _type = Py.GetPyType(_pyReference);
            }
        }

        public List<PyObject> List
        {
            get
            {
                if (_list.Count != 0)
                    return _list;

                if (Type == Py.PyType.ListType)
                {
                    _list = new List<PyObject>();
                    int size = Py.PyList_Size(_pyReference);
                    if (size > 0)
                        for (int i = 0; i < size; i++)
                            _list.Add(new PyObject(Py.PyTuple_GetItem(_pyReference, i)));
                }
                return _list;
            }
        }

        public List<PyObject> Tuple
        {
            get
            {
                if (_tuple.Count != 0)
                    return _tuple;

                if (Type == Py.PyType.TupleType)
                {
                    _tuple = new List<PyObject>();
                    int size = Py.PyTuple_Size(_pyReference);
                    if (size > 0)
                        for (int i = 0; i < size; i++)
                            _tuple.Add(new PyObject(Py.PyTuple_GetItem(_pyReference, i)));
                }
                return _tuple;
            }
        }

        public string String
        {
            get
            {
                return _string = Py.ToUnicodeString(_pyReference);
            }
        }

        public int? Int
        {
            get
            {
                if (_integer != null)
                    return _integer;

                if (Type == Py.PyType.LongType)
                    _integer = Py.PyLong_AsLong(_pyReference);

                return _integer;
            }
        }

        public long? Long
        {
            get
            {
                if (_long != null)
                    return _long;

                if (Type == Py.PyType.StringType)
                    _long = Py.PyLong_AsLongLong(_pyReference);

                return _long;
            }
        }

        public double? Float
        {
            get
            {
                if (_float != null)
                    return _float;

                if (Type == Py.PyType.FloatType)
                    _float = Py.PyFloat_AsDouble(_pyReference);

                return _float;
            }
        }
    }
}