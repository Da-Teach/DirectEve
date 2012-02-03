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

    public class DirectBookmarkFolder : DirectObject
    {
        internal DirectBookmarkFolder(DirectEve directEve, PyObject pyFolder)
            : base(directEve)
        {
            Id = (long) pyFolder.Attribute("folderID");
            Name = (string) pyFolder.Attribute("folderName");
            OwnerId = (long) pyFolder.Attribute("ownerID");
            CreatorId = (long?) pyFolder.Attribute("creatorID");
        }

        public long Id { get; internal set; }
        public string Name { get; internal set; }
        public long OwnerId { get; internal set; }
        public long? CreatorId { get; internal set; }

        public bool Delete()
        {
            return DirectEve.ThreadedLocalSvcCall("bookmarkSvc", "DeleteFolder", Id);
        }
    }
}