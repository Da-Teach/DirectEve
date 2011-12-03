namespace DirectEve
{
    using System.Collections.Generic;
    using System.Linq;
    using global::DirectEve.PySharp;

    public class DirectBookmarkFolder : DirectObject
    {
        internal DirectBookmarkFolder(DirectEve directEve, PyObject pyFolder)
            : base(directEve)
        {
            Id = (long)pyFolder.Attribute("folderID");
            Name = (string)pyFolder.Attribute("folderName");
            OwnerId = (long)pyFolder.Attribute("ownerID");
            CreatorId = (long?)pyFolder.Attribute("creatorID");
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
