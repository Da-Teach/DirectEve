namespace DirectEve
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::DirectEve.PySharp;

    public class DirectAgentMission : DirectObject
    {
        internal DirectAgentMission(DirectEve directEve) : base(directEve)
        {
        }

        private PyObject _pyAgentId;

        public int State { get; internal set; }
        public bool Important { get; internal set; }
        public string Type { get; internal set; }
        public string Name { get; internal set; }
        public long AgentId { get; internal set; }
        public DateTime ExpiresOn { get; internal set; }
        public List<DirectAgentMissionBookmark> Bookmarks { get; internal set; }

        internal static List<DirectAgentMission> GetAgentMissions(DirectEve directEve)
        {
            var missions = new List<DirectAgentMission>();

            var pyMissions = directEve.GetLocalSvc("journal").Attribute("agentjournal").Item(0).ToList();
            foreach (var pyMission in pyMissions)
            {
                var mission = new DirectAgentMission(directEve);
                mission.State = (int)pyMission.Item(0);
                mission.Important = (bool)pyMission.Item(1);
                mission.Type = (string)pyMission.Item(2);
                mission.Name = (string)pyMission.Item(3);

                mission._pyAgentId = pyMission.Item(4);
                mission.AgentId = (long)pyMission.Item(4);

                mission.ExpiresOn = (DateTime)pyMission.Item(5);
                mission.Bookmarks = pyMission.Item(6).ToList().Select(b => new DirectAgentMissionBookmark(directEve, b)).ToList();
                missions.Add(mission);
            }

            return missions;
        }

        public bool RemoveOffer()
        {
            if (State != (int)PySharp.Import("__builtin__").Attribute("const").Attribute("agentMissionStateOffered"))
                return false;

            return DirectEve.ThreadedLocalSvcCall("agents", "RemoveOfferFromJournal", _pyAgentId);
        }
    }
}
