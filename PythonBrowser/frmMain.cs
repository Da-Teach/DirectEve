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
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;
    using InnerSpaceAPI;
    using LavishScriptAPI;
    using PythonBrowser.PySharp;

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

        private PyObject Evaluate(PySharp.PySharp pySharp)
        {
            pySharp.Run("import __builtin__\n__builtin__.evalresult = " + _evaluate);
            return pySharp.Import("__builtin__").Attribute("evalresult");
        }

        private void OnFrame(object sender, LSEventArgs e)
        {
            try
            {
                using (var pySharp = new PySharp.PySharp())
                {
                    PyObject pyObject = null;

                    if (_doTest)
                    {
                        _doTest = false;

                        //pyObject = pySharp.Import("__builtin__").Attribute("sm").Attribute("services").DictionaryItem("invCache").Attribute("inventories").Call("keys");

                        //var shipid = (long)pySharp.Import("__builtin__").Attribute("eve").Attribute("session").Attribute("shipid");
                        //pyObject = pySharp.Import("__builtin__").Attribute("eve").Call("GetInventoryFromId", shipid);

                        //pySharp.Import("uthread").Call("new", pySharp.Import("__builtin__").Attribute("eve").Attribute("GetInventoryFromId"), shipid);
                        //pyObject = pySharp.Import("blue").Attribute("os").Call("GetTime");
                        /*var shipid = (long)pySharp.Import("__builtin__").Attribute("eve").Attribute("session").Attribute("shipid");
                        pyObject = pySharp.Import("__builtin__").Attribute("eve").Call("GetInventoryFromId", shipid);

                        var items = pyObject.Call("List").ToList();
                        foreach(var item in items)
                        {
                            if ((int)item.Attribute("quantity") < 1000)
                                continue;

                            InnerSpace.Echo("TypeId " + (int)item.Attribute("typeID") + " ; Quantity " + (int)item.Attribute("quantity"));

                            var keywords = new Dictionary<string, object>
                                           {
                                               {"qty", 1}
                                           };
                            pySharp.Import("__builtin__").Attribute("eve").Call("GetInventory", 10004).CallWithKeywords("Add", keywords, item.Attribute("itemID"), item.Attribute("locationID"));
                        }*/
                        //pyObject = pySharp.Import("__builtin__").Attribute("sm").Attribute("services").DictionaryItem("godma").Attribute("stateManager").Attribute("chargedAttributesByItemAttribute").DictionaryItem(shipid).DictionaryItem("shieldCharge");
                        //pyObject = pySharp.Import("__builtin__").Attribute("sm").Attribute("services").DictionaryItem("godma").Attribute("stateManager").Call("GetChargeValue", pyObject.Item(0), pyObject.Item(1), pyObject.Item(2), pyObject.Item(3));

                        //pyObject = pySharp.Import("__builtin__").Attribute("sm").Attribute("services").DictionaryItem("addressbook").Attribute("allianceContacts").Call("get", 481995776, PySharp.PyNone).Attribute("relationshipID");
                        //pyObject = pySharp.Import("__builtin__").Attribute("uicore").Attribute("registry").Attribute("windows").Item(0).Attribute("channelID");
                        //pyObject = pySharp.Import("__builtin__").Attribute("sm").Attribute("services").DictionaryItem("LSC").Attribute("channels").DictionaryItem(pyObject);

                        //pyObject = pySharp.Import("__builtin__").Attribute("uicore").Attribute("registry").Attribute("windows").Item(8);
                        //pySharp.Import("uthread").Call("new", pyObject.Attribute("LoadTypeID_Ext"), 20);
                        pySharp.Run(File.ReadAllText("C:/Downloads/tmp/dump.py"));
                        InnerSpace.Echo("Done");
                    }

                    if (_doEvaluate)
                    {
                        _doEvaluate = false;
                        pyObject = Evaluate(pySharp);
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