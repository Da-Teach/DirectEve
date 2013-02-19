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
    using System.Collections.Generic;
    using System.Linq;

    public class DirectContainerWindow : DirectWindow
    {
        //private DirectEve DirectEve;

        internal DirectContainerWindow(DirectEve directEve, PyObject pyWindow)
            : base(directEve, pyWindow)
        {
            //IsReady = (bool) pyWindow.Attribute("startedUp");
            IsOneWay = (bool)pyWindow.Attribute("oneWay");
            ItemId = (long)pyWindow.Attribute("itemID");
            LocationFlag = (int)pyWindow.Attribute("locationFlag");
            HasCapacity = (bool)pyWindow.Attribute("hasCapacity");
            currInvIdName = (string)PyWindow.Attribute("currInvID").ToList().First();
            currInvIdItem = (long)PyWindow.Attribute("currInvID").ToList().Last();
        }

        public bool IsReady
        {
            get
            {
                return !((bool)PyWindow.Attribute("startingup")) && !((bool)PyWindow.Attribute("loadingInvCont")) && !((bool)PyWindow.Attribute("loadingTreeView"));
            }
        }
        public bool IsOneWay { get; private set; }

        public long ItemId { get; private set; }
        public int LocationFlag { get; private set; }
        public bool HasCapacity { get; private set; }
        public string currInvIdName { get; private set; }
        public long currInvIdItem { get; private set; }


        /// <summary>
        ///   Open the current container view in a new window
        /// </summary>        
        /// <returns></returns>        
        public bool OpenAsSecondary()
        {
            PyObject invID = PyWindow.Attribute("currInvID");
            //check if it's already open, in that case do nothing.
            var windows = DirectEve.Windows.OfType<DirectContainerWindow>();
            string lookForInvID = (string)invID.ToList().First();
            var alreadyOpened = windows.FirstOrDefault(w => w.Name.Contains(lookForInvID) && !w.IsPrimary());
            if (alreadyOpened != null)
                return true;


            var form = PySharp.Import("form");
            var keywords = new Dictionary<string, object>();
            keywords.Add("invID", invID);
            keywords.Add("usePrimary", false);
            return DirectEve.ThreadedCallWithKeywords(form.Attribute("Inventory").Attribute("OpenOrShow"), keywords);
        }


        /// <summary>
        ///   Get a list of IDs as longs associated with each entry of a inventory tree
        /// </summary>
        /// <param name = "itemsOnly">only look for wrecks and containers</param>
        /// <returns>List of IDs of the inventory tree</returns>
        public List<long> GetIdsFromTree(bool itemsOnly = true)
        {
            var dict = PyWindow.Attribute("treeEntryByID").ToDictionary();
            List<long> result = new List<long>();
            foreach (var treeItem in dict.Keys)
            {
                string invIDName = (string)treeItem.ToList().First();
                long invID = (long)treeItem.ToList().Last();
                if (itemsOnly)
                {
                    if (invIDName.Contains("Item"))
                        result.Add(invID);
                }
                else result.Add(invID);
            }
            return result;
        }


        /// <summary>
        ///   Close the tree entry with the given ID
        /// </summary>
        /// <param name = "entryID"></param>
        /// <returns></returns>
        public bool CloseTreeEntry(long entryID)
        {
            var dict = PyWindow.Attribute("treeEntryByID").ToDictionary();
            var entry = dict.Where(d => (long)d.Key.ToList().Last() == entryID);
            if (entry == null) return false;
            if (entry.Count() != 1) return false;
            

            //can NOT do threaded calls because these need to be executed in order
            PyWindow.Call("RemoveTreeEntry", entry.First().Value);
            PyWindow.Call("RefreshTree");
            return true;
        }

        /// <summary>
        ///   Select the tree entry with the given ID
        /// </summary>
        /// <param name = "entryID"></param>
        /// <returns></returns>
        public bool SelectTreeEntryByID(long entryID)
        {
            var dict = PyWindow.Attribute("treeEntryByID").ToDictionary();
            var entry = dict.Where(d => (long)d.Key.ToList().Last() == entryID);
            if (entry == null) return false;
            if (entry.Count() != 1) return false;

            return DirectEve.ThreadedCall(PyWindow.Attribute("OpenOrShow"), entry.First().Key);

        }

        /// <summary>
        ///   Select the first tree entry with the given name
        /// </summary>
        /// <param name = "entryName"></param>
        /// <returns></returns>
        public bool SelectTreeEntryByName(string entryName)
        {
            var dict = PyWindow.Attribute("treeEntryByID").ToDictionary();
            var entry = dict.Where(d => (string)d.Key.ToList().First() == entryName);
            if (entry == null) return false;
            if (entry.Count() <= 0) return false;

            return DirectEve.ThreadedCall(PyWindow.Attribute("OpenOrShow"), entry.First().Key);

        }

        /// <summary>
        ///   Select the tree entry with the given name and ID
        /// </summary>
        /// <param name = "entryName"></param>
        /// <param name = "entryID"></param>
        /// <returns></returns>
        public bool SelectTreeEntry(string entryName, long entryID)
        {
            var dict = PyWindow.Attribute("treeEntryByID").ToDictionary();
            var entry = dict.Where(d => (string)d.Key.ToList().First() == entryName && (long)d.Key.ToList().Last() == entryID);
            if (entry == null) return false;
            if (entry.Count() != 1) return false;

            return DirectEve.ThreadedCall(PyWindow.Attribute("OpenOrShow"), entry.First().Key);

        }

        /// <summary>
        ///   Expand the tree entry with the given name and ID
        /// </summary>
        /// <param name = "entryName"></param>
        /// <returns></returns>
        internal bool ExpandTreeEntry(string entryName)
        {
            var dict = PyWindow.Attribute("treeEntryByID").ToDictionary();
            var entry = dict.Where(d => (string)d.Key.ToList().First() == entryName);
            if (entry == null) return false;
            if (entry.Count() != 1) return false;

            entry.First().Value.Call("ExpandFromRoot");
            PyWindow.Call("RefreshTree");

            return true;
        }

        public bool ExpandCorpHangarView()
        {
            return ExpandTreeEntry("Corporation hangars");
        }

        public bool IsPrimary()
        {
            return this.Type == "form.InventoryPrimary";
        }

    }
}