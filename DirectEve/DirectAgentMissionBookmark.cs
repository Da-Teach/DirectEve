namespace DirectEve
{
    using global::DirectEve.PySharp;

    public class DirectAgentMissionBookmark : DirectBookmark
    {
        internal DirectAgentMissionBookmark(DirectEve directEve, PyObject pyBookmark) : base(directEve, pyBookmark)
        {
            AgentId = (long?) pyBookmark.Attribute("agentID");
            IsDeadspace = (bool?)pyBookmark.Attribute("deadspace");
            Flag = (int?)pyBookmark.Attribute("flag");
            LocationNumber = (int?)pyBookmark.Attribute("locationNumber");
            LocationType = (string)pyBookmark.Attribute("locationType");
            Title = (string)pyBookmark.Attribute("hint");
            SolarSystemId = (long?)pyBookmark.Attribute("solarsystemID");
        }

        public long? AgentId;
        public bool? IsDeadspace;
        public int? Flag;
        public int? LocationNumber;
        public string LocationType;
        public long? SolarSystemId;
    }
}
