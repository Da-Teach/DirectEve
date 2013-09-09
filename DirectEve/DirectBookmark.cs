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
    using System.Linq;
    using global::DirectEve.PySharp;

    public class DirectBookmark : DirectInvType
    {
        /// <summary>
        ///   Entity cache
        /// </summary>
        private DirectEntity _entity;

        internal DirectBookmark(DirectEve directEve, PyObject pyBookmark)
            : base(directEve)
        {
            PyBookmark = pyBookmark;
            BookmarkId = (long?) pyBookmark.Attribute("bookmarkID");
            CreatedOn = (DateTime?) pyBookmark.Attribute("created");
            ItemId = (long?) pyBookmark.Attribute("itemID");
            LocationId = (long?) pyBookmark.Attribute("locationID");
            FolderId = (long?) pyBookmark.Attribute("folderID");
            Title = (string) pyBookmark.Attribute("memo");
            if (!String.IsNullOrEmpty(Title) && Title.Contains("\t"))
            {
                Memo = Title.Substring(Title.IndexOf("\t") + 1);
                Title = Title.Substring(0, Title.IndexOf("\t"));
            }
            Note = (string) pyBookmark.Attribute("note");
            OwnerId = (int?) pyBookmark.Attribute("ownerID");
            TypeId = (int) pyBookmark.Attribute("typeID");
            X = (double?) pyBookmark.Attribute("x");
            Y = (double?) pyBookmark.Attribute("y");
            Z = (double?) pyBookmark.Attribute("z");
        }

        internal PyObject PyBookmark { get; set; }

        public long? BookmarkId { get; internal set; }
        public DateTime? CreatedOn { get; internal set; }

        /// <summary>
        /// If this is a bookmark of a station, this is the StationId
        /// </summary>
        public long? ItemId { get; internal set; }

        /// <summary>
        /// Matches SolarSystemId
        /// </summary>
        public long? LocationId { get; internal set; }
        public long? FolderId { get; internal set; }
        public string Title { get; internal set; }
        public string Memo { get; internal set; }
        public string Note { get; internal set; }
        public int? OwnerId { get; internal set; }
        public double? X { get; internal set; }
        public double? Y { get; internal set; }
        public double? Z { get; internal set; }

        /// <summary>
        ///   The entity associated with this bookmark
        /// </summary>
        /// <remarks>
        ///   This property will be null if no entity can be found
        /// </remarks>
        public DirectEntity Entity
        {
            get { return _entity ?? (_entity = DirectEve.GetEntityById(ItemId ?? -1)); }
        }

        internal static bool RefreshBookmarks(DirectEve directEve)
        {
            // If the bookmarks need to be refreshed, then this will do it
            return directEve.ThreadedLocalSvcCall("bookmarkSvc", "GetBookmarks");
        }

        internal static DateTime? GetLastBookmarksUpdate(DirectEve directEve)
        {
            // Get the bookmark-last-update-time
            return (DateTime?) directEve.GetLocalSvc("bookmarkSvc").Attribute("lastUpdateTime");
        }

        internal static List<DirectBookmark> GetBookmarks(DirectEve directEve)
        {
            // List the bookmarks from cache
            var bookmarks = directEve.GetLocalSvc("bookmarkSvc").Attribute("bookmarkCache").ToDictionary<long>();
            return bookmarks.Values.Select(pyBookmark => new DirectBookmark(directEve, pyBookmark)).ToList();
        }

        internal static List<DirectBookmarkFolder> GetFolders(DirectEve directEve)
        {
            // List the bookmark folders from cache
            var folders = directEve.GetLocalSvc("bookmarkSvc").Attribute("folders").ToDictionary<long>();
            return folders.Values.Select(pyFolder => new DirectBookmarkFolder(directEve, pyFolder)).ToList();
        }

        internal static bool CreateBookmarkFolder(DirectEve directEve, long ownerId, string name)
        {
            return directEve.ThreadedLocalSvcCall("bookmarkSvc", "CreateFolder", ownerId, name);
        }

        internal static bool BookmarkLocation(DirectEve directEve, long ownerId, long itemId, string name, string comment, int typeId, long? locationId, long? folderId)
        {
            var bookmarkLocation = directEve.GetLocalSvc("bookmarkSvc").Attribute("BookmarkLocation");
            var keywords = new Dictionary<string, object>();
            if (locationId.HasValue)
                keywords.Add("locationID", locationId.Value);
            if (folderId.HasValue)
                keywords.Add("folderID", folderId.Value);
            return directEve.ThreadedCallWithKeywords(bookmarkLocation, keywords, itemId, ownerId, name, comment, typeId);
        }

        internal static bool RefreshPnPWindow(DirectEve directEve)
        {
            return directEve.ThreadedLocalSvcCall("bookmarkSvc", "RefreshWindow"); ;
        }

        public bool CopyBookmarksToCorpFolder()
        {

            if ((!BookmarkId.HasValue) || (DirectEve.Session.CorporationId == null))
                return false;

            return DirectEve.ThreadedLocalSvcCall("bookmarkSvc", "MoveBookmarksToFolder", DirectEve.Session.CorporationId, DirectEve.Session.CorporationId, PyBookmark.Attribute("bookmarkID"));
        }

        public bool UpdateBookmark(string name, string comment)
        {
            if (!BookmarkId.HasValue)
                return false;

            return DirectEve.ThreadedLocalSvcCall("bookmarkSvc", "UpdateBookmark", PyBookmark.Attribute("bookmarkID"), PyBookmark.Attribute("ownerID"), name, comment, PyBookmark.Attribute("folderID"));
        }

        public bool WarpTo()
        {
            return WarpTo(0);
        }

        public bool WarpTo(double distance)
        {
            PyObject warpToBookmark = PySharp.Import("eve.client.script.ui.services.menuSvcExtras.movementFunctions").Attribute("WarpToBookmark");
            return DirectEve.ThreadedCall(warpToBookmark, PyBookmark, distance);
        }

        public bool Approach()
        {
            PyObject approachLocation = PySharp.Import("eve.client.script.ui.services.menuSvcExtras.movementFunctions").Attribute("ApproachLocation");
            return DirectEve.ThreadedCall(approachLocation, PyBookmark);
        }

        public bool Delete()
        {
            if (!BookmarkId.HasValue)
                return false;

            return DirectEve.ThreadedLocalSvcCall("addressbook", "DeleteBookmarks", new List<PyObject> {PyBookmark.Attribute("bookmarkID")});
        }
    }
}