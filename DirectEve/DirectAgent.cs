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
    using System.Linq;
    using global::DirectEve.PySharp;

    public class DirectAgent : DirectObject
    {
        private int? _loyaltyPoints;

        internal DirectAgent(DirectEve directEve) : base(directEve)
        {
        }

        private PyObject PyAgentId { get; set; }

        public bool IsValid { get; private set; }
        public long AgentId { get; private set; }
        public long AgentTypeId { get; private set; }
        public long DivisionId { get; private set; }
        public int Level { get; private set; }
        public long StationId { get; private set; }
        public long BloodlineId { get; private set; }
        public int Quality { get; private set; }
        public long CorpId { get; private set; }
        public bool Gender { get; private set; }
        public long FactionId { get; private set; }
        public long SolarSystemId { get; private set; }

        public string Name
        {
            get
            {
                var owner = DirectOwner.GetOwner(DirectEve, AgentId);
                if (owner == null)
                    return string.Empty;

                return owner.Name;
            }
        }

        public int LoyaltyPoints
        {
            get
            {
                if (!_loyaltyPoints.HasValue)
                {
                    if ((bool)DirectEve.GetLocalSvc("journal").Attribute("outdatedCorpLP"))
                    {
                        // Update loyalty points if their outdated
                        DirectEve.ThreadedLocalSvcCall("journal", "GetMyLoyaltyPoints");
                    }
                    else
                    {
                        var mappings = DirectEve.GetLocalSvc("journal").Attribute("lpMapping").ToList();
                        foreach (var mapping in mappings)
                        {
                            if ((int) mapping.Item(0) != CorpId)
                                continue;

                            _loyaltyPoints = (int) mapping.Item(1);
                        }
                    }
                    _loyaltyPoints = _loyaltyPoints ?? -1;
                }
                return _loyaltyPoints.Value;
            }
        }

        public DirectAgentWindow Window
        {
            get { return DirectEve.Windows.OfType<DirectAgentWindow>().FirstOrDefault(w => w.AgentId == AgentId); }
        }

        public bool InteractWith()
        {
            // Use the actual agent id gotten from the allAgentsByID attribute, long/int crashed...
            return DirectEve.ThreadedLocalSvcCall("agents", "InteractWith", PyAgentId);
        }

        internal static DirectAgent GetAgentById(DirectEve directEve, long id)
        {
            var pyAgent = directEve.GetLocalSvc("agents").Attribute("allAgentsByID").Attribute("items").DictionaryItem(id);

            var agent = new DirectAgent(directEve);
            agent.IsValid = pyAgent.IsValid;
            agent.PyAgentId = pyAgent.Item(0);
            agent.AgentId = (long) pyAgent.Item(0);
            agent.AgentTypeId = (long) pyAgent.Item(1);
            agent.DivisionId = (long) pyAgent.Item(2);
            agent.Level = (int) pyAgent.Item(3);
            agent.StationId = (long) pyAgent.Item(4);
            agent.BloodlineId = (long) pyAgent.Item(5);
            agent.Quality = (int) pyAgent.Item(6);
            agent.CorpId = (long) pyAgent.Item(7);
            agent.Gender = (bool) pyAgent.Item(8);
            agent.FactionId = (long) pyAgent.Item(9);
            agent.SolarSystemId = (long) pyAgent.Item(10);
            return agent;
        }

        internal static DirectAgent GetAgentByName(DirectEve directEve, string name)
        {
            var agentsById = directEve.GetLocalSvc("agents").Attribute("allAgentsByID").Attribute("items").ToDictionary<long>();
            foreach (var agent in agentsById)
            {
                var owner = DirectOwner.GetOwner(directEve, agent.Key);
                if (owner.Name != name)
                    continue;

                return GetAgentById(directEve, agent.Key);
            }

            return null;
        }
    }
}