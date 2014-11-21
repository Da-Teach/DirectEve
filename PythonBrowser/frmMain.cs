// ------------------------------------------------------------------------------
//   <copyright from='2010' to='2015' company='THEHACKERWITHIN.COM'>
//     Copyright (c) TheHackerWithin.COM. All Rights Reserved.
// 
//     Please look in the accompanying license.htm file for the license that 
//     applies to this source code. (a copy can also be found at: 
//     http://www.thehackerwithin.com/license.htm)
//   </copyright>
// -------------------------------------------------------------------------------

namespace PythonBrowser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
    using InnerSpaceAPI;
    using LavishScriptAPI;
    using PySharp;

    public partial class frmMain : Form
    {
        #region TypeMode enum

        public enum TypeMode
        {
            Auto,
            Value,
            Class,
            List,
            Tuple,
            Dictionary
        }

        #endregion

        private bool _doEvaluate;
        private bool _doTest;
        private bool _done;
        private string _evaluate;
        private TypeMode _typeMode;

        private List<PyValue> _values;

        public frmMain()
        {
            InitializeComponent();

            _values = new List<PyValue>();
            LavishScript.Events.AttachEventTarget(LavishScript.Events.RegisterEvent("OnFrame"), OnFrame);
        }

        private PyObject Evaluate(PySharp.PySharp pySharp, PyObject pyObject)
        {
            // TODO: Better part splitting (e.g. bla('bla', const.bla)
            var parts = _evaluate.Split('.');
            if (parts.Length == 0)
            {
                // TODO: List imports
                return null;
            }

            if (pyObject == null)
                pyObject = pySharp.Import(parts[0]);

            for (var i = 1; i < parts.Length; i++)
            {
                if (parts[i].Contains("("))
                {
                    // TODO: Call
                }
                else if (parts[i].Contains("["))
                {
                    var attr = parts[i].Substring(0, parts[i].IndexOf('['));

                    var key = parts[i].Substring(parts[i].IndexOf('[') + 1, parts[i].IndexOf(']') - parts[i].IndexOf('[') - 1);
                    if (key.StartsWith("'") || key.StartsWith("\""))
                        key = key.Substring(1, key.Length - 2);

                    if (!string.IsNullOrEmpty(attr))
                        pyObject = pyObject.Attribute(attr);

                    if (pyObject.GetPyType() == PyType.DictType ||
                        pyObject.GetPyType() == PyType.DerivedDictType ||
                        pyObject.GetPyType() == PyType.DictProxyType ||
                        pyObject.GetPyType() == PyType.DerivedDictProxyType)
                    {
                        var dict = pyObject.ToDictionary();

                        pyObject = PySharp.PySharp.PyZero;
                        foreach (var dictItem in dict)
                        {
                            if (GetPyValue(dictItem.Key) == key)
                                pyObject = dictItem.Value;
                        }
                    }
                    else
                    {
                        int index;
                        pyObject = int.TryParse(key, out index) ? pyObject.Item(index) : PySharp.PySharp.PyZero;
                    }
                }
                else
                {
                    pyObject = pyObject.Attribute(parts[i]);
                }
            }

            return pyObject;
        }

        private void OnFrame(object sender, LSEventArgs e)
        {
            try
            {
                using (dynamic pySharp = new PySharp.PySharp())
                {
                    dynamic pyObject = null;

                    if (_doTest)
                    {
                        _doTest = false;

                        pyObject = pySharp.__builtin__.eve.session;

                        var file = pySharp.__builtin__.open("c:/blaat.txt", "wb");
                        file.write("hello world");
                        file.close();

                        //// Make eve reload the compiled code file (stupid DiscardCode function :)
                        //pySharp.Import("nasty").Attribute("nasty").Attribute("compiler").Call("Load", pySharp.Import("nasty").Attribute("nasty").Attribute("compiledCodeFile"));

                        //// Get a reference to all code files
                        //var dict = pySharp.Import("nasty").Attribute("nasty").Attribute("compiler").Attribute("code").ToDictionary();

                        //// Get the magic once
                        //var magic = pySharp.Import("imp").Call("get_magic");

                        //foreach (var item in dict)
                        //{
                        //    // Clean up the path
                        //    var path = (string)item.Key.Item(0);
                        //    if (path.IndexOf(":") >= 0)
                        //        path = path.Substring(path.IndexOf(":") + 1);
                        //    while (path.StartsWith("/.."))
                        //        path = path.Substring(3);
                        //    path = "c:/dump/code" + path + "c";

                        //    // Create the directory
                        //    Directory.CreateDirectory(Path.GetDirectoryName(path));

                        //    var file = pySharp.Import("__builtin__").Call("open", path, "wb");
                        //    var time = pySharp.Import("os").Attribute("path").Call("getmtime", path);

                        //    // Write the magic
                        //    file.Call("write", magic);
                        //    // Write the time
                        //    file.Call("write", pySharp.Import("struct").Call("pack", "<i", time));
                        //    // Write the code
                        //    pySharp.Import("marshal").Call("dump", item.Value.Item(0), file);
                        //    // Close the file
                        //    file.Call("close");
                        //}
                        InnerSpace.Echo("Done");
                    }

                    if (_doEvaluate)
                    {
                        _doEvaluate = false;
                        pyObject = Evaluate(pySharp, pyObject);
                    }

                    if (pyObject != null)
                        ListPyObject(pyObject);
                }
            }
            finally
            {
                _done = true;
            }
        }

        private void ListPyObject(PyObject pyObject)
        {
            PyType type;
            switch (_typeMode)
            {
                case TypeMode.Value:
                    // Fill the value here
                    var value = new PyValue();
                    value.Attribute = "(this)";
                    value.Value = GetPyValue(pyObject);
                    value.Type = pyObject.GetPyType().ToString();
                    _values.Add(value);
                    return;

                case TypeMode.Class:
                    // Force it to unknown
                    type = PyType.Unknown;
                    break;

                case TypeMode.List:
                    // Force it to a list
                    type = PyType.ListType;
                    break;

                case TypeMode.Tuple:
                    // Force it to a tuple
                    type = PyType.TupleType;
                    break;

                case TypeMode.Dictionary:
                    // Force it to a dictionary
                    type = PyType.DictType;
                    break;

                default:
                    // Let the type decide
                    type = pyObject.GetPyType();
                    break;
            }

            switch (type)
            {
                case PyType.DictType:
                case PyType.DictProxyType:
                case PyType.DerivedDictType:
                case PyType.DerivedDictProxyType:
                    foreach (var attribute in pyObject.ToDictionary())
                    {
                        var item = new PyValue();
                        item.Attribute = GetPyValue(attribute.Key);
                        item.Value = GetPyValue(attribute.Value);
                        item.Type = attribute.Value.GetPyType().ToString();
                        item.Eval = _evaluate + "[" + item.Attribute + "]";
                        _values.Add(item);
                    }
                    break;

                case PyType.ListType:
                case PyType.TupleType:
                case PyType.DerivedListType:
                case PyType.DerivedTupleType:
                    var length = pyObject.Size(type);
                    for (var i = 0; i < length; i++)
                    {
                        var item = new PyValue();
                        item.Attribute = i.ToString();
                        item.Value = GetPyValue(pyObject.Item(i, type));
                        item.Type = pyObject.Item(i, type).GetPyType().ToString();
                        item.Eval = _evaluate + "[" + i + "]";
                        _values.Add(item);
                    }
                    break;

                default:
                    foreach (var attribute in pyObject.Attributes())
                    {
                        var item = new PyValue();
                        item.Attribute = attribute.Key;
                        item.Value = GetPyValue(attribute.Value);
                        item.Type = attribute.Value.GetPyType().ToString();
                        item.Eval = _evaluate + "." + attribute.Key;
                        _values.Add(item);
                    }
                    break;
            }
        }

        private string GetPyValue(PyObject attr)
        {
            switch (attr.GetPyType())
            {
                case PyType.FloatType:
                case PyType.DerivedFloatType:
                    return ((double) attr).ToString();

                case PyType.IntType:
                case PyType.LongType:
                case PyType.DerivedIntType:
                case PyType.DerivedLongType:
                    return ((long) attr).ToString();

                case PyType.BoolType:
                case PyType.DerivedBoolType:
                    return ((bool) attr).ToString();

                case PyType.StringType:
                case PyType.UnicodeType:
                case PyType.DerivedStringType:
                case PyType.DerivedUnicodeType:
                    return (string) attr;

                case PyType.MethodType:
                    var x = attr.Attribute("im_func").Attribute("func_code");
                    var name = (string) x.Attribute("co_name");
                    var argCount = (int) x.Attribute("co_argcount");
                    var args = string.Join(",", x.Attribute("co_varnames").ToList<string>().Take(argCount).ToArray());
                    return name + "(" + args + ")";

                default:
                    return attr.ToString();
            }
        }

        private void PreEvalutate()
        {
            _values.Clear();

            _typeMode = TypeMode.Auto;
            _typeMode = ValueButton.Checked ? TypeMode.Value : _typeMode;
            _typeMode = ClassButton.Checked ? TypeMode.Class : _typeMode;
            _typeMode = ListButton.Checked ? TypeMode.List : _typeMode;
            _typeMode = TupleButton.Checked ? TypeMode.Tuple : _typeMode;
            _typeMode = DictionaryButton.Checked ? TypeMode.Dictionary : _typeMode;

            _evaluate = EvaluateBox.Text;

            var found = false;
            foreach (var item in EvaluateBox.Items)
            {
                if (item as string == _evaluate)
                    found = true;
            }

            if (!found)
                EvaluateBox.Items.Insert(0, EvaluateBox.Text);

            _done = false;
        }

        private void PostEvaluate()
        {
            var timeout = 0;
            while (!_done)
            {
                Thread.Sleep(50);
                timeout++;
                if (timeout > 300) // 15 sec
                    break;
            }

            AttributesList.Items.Clear();
            foreach (var value in _values)
            {
                var item = new ListViewItem(value.Attribute);
                item.SubItems.Add(value.Value);
                item.SubItems.Add(value.Type);
                item.Tag = value.Eval;
                AttributesList.Items.Add(item);
            }
        }

        private void EvaluateButton_Click(object sender, EventArgs e)
        {
            PreEvalutate();

            _doEvaluate = true;

            PostEvaluate();
        }

        private void StaticTestButton_Click(object sender, EventArgs e)
        {
            PreEvalutate();

            _doTest = true;
            _doEvaluate = true;

            PostEvaluate();
        }

        private void EvaluateBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                EvaluateButton_Click(null, null);
                e.Handled = true;
            }
        }

        private void RadioButton_Click(object sender, EventArgs e)
        {
            EvaluateButton_Click(null, null);
        }

        private void AttributesList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (AttributesList.SelectedItems.Count != 1)
                return;

            var tag = AttributesList.SelectedItems[0].Tag as string;
            if (string.IsNullOrEmpty(tag))
                return;

            EvaluateBox.Text = tag;
            EvaluateButton_Click(null, null);
        }

        #region Nested type: PyValue

        public class PyValue
        {
            public string Attribute { get; set; }
            public string Value { get; set; }
            public string Type { get; set; }
            public string Eval { get; set; }
        }

        #endregion
    }
}