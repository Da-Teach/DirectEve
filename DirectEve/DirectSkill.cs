using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirectEve
{
    /// <summary>
    /// Skill in the game
    /// </summary>
    public class DirectSkill : DirectObject
    {
        long itemId, locationId;
        int typeId, flagId;

        /// <summary>
        /// item id
        /// </summary>
        public long ItemId
        {
            get
            {
                return itemId;
            }
        }

        /// <summary>
        /// typeID of the skill
        /// </summary>
        public int TypeId
        {
            get
            {
                return typeId;
            }
        }

        /// <summary>
        /// flag of skill changes to show if it's beaing trained
        /// </summary>
        public int FlagId
        {
            get
            {
                return flagId;
            }
        }

        /// <summary>
        /// no idea what this is
        /// </summary>
        public long LocationId
        {
            get
            {
                return locationId;
            }
        }

        /// <summary>
        /// True if this skill is currently training
        /// </summary>
        public bool InTraining
        {
            get
            {
                return FlagId == new DirectConst(DirectEve).FlagSkillInTraining.ToInt();
            }
        }

        /// <summary>
        /// Level of skill
        /// </summary>
        public int Level
        {
            get
            {
                // CharStartTrainingSkill
                return DirectEve.GetLocalSvc("godma").Call("GetItem", ItemId).Attribute("skillLevel").ToInt();
                //sm.GetService('godma').GetItem(skillID)
                //return 0;
            }
        }

        /// <summary>
        /// skill name
        /// </summary>
        public string Name
        {
            get
            {
                return FindName(DirectEve, TypeId);
            }
        }

        /// <summary>
        /// Number of points in this skill
        /// </summary>
        public int SkillPoints
        {
            get
            {
                return DirectEve.GetLocalSvc("godma").Call("GetItem", ItemId).Attribute("skillPoints").ToInt();
            }
        }

        /// <summary>
        /// Time multiplier to indicate relative training time
        /// </summary>
        public int SkillTimeConstant
        {
            get
            {
                return FindSkillTimeConstant(DirectEve, ItemId);
            }
        }

        /// <summary>
        /// Start training this skill - I think this is from before queues
        /// </summary>
        //public void Train()
        //{
        //    // sm.GetService('godma').GetSkillHandler().CharStartTrainingSkill(skillX.itemID, skillX.locationID)
        //    DirectEve.GetLocalSvc("godma").Call("GetSkillHandler").Call("CharStartTrainingSkill", ItemId, locationId);
        //}

        static Dictionary<int, string> skillNames = new Dictionary<int, string>();

        internal static string FindName(DirectEve directEve, int typeId)
        {
            string name;
            if (!skillNames.TryGetValue(typeId, out name))
            {
                name = directEve.PySharp.Import("__builtin__").Attribute("cfg").Attribute("invtypes").Call("Get", typeId).Attribute("name").ToUnicodeString();
                skillNames.Add(typeId, name);
            }
            return name;
        }

        static Dictionary<long, int> skillTimeConstants = new Dictionary<long, int>();

        internal static int FindSkillTimeConstant(DirectEve directEve, long itemId)
        {
            int skillTimeConstant;
            if (!skillTimeConstants.TryGetValue(itemId, out skillTimeConstant))
            {
                skillTimeConstant = directEve.GetLocalSvc("godma").Call("GetItem", itemId).Attribute("skillTimeConstant").ToInt();
                skillTimeConstants.Add(itemId, skillTimeConstant);
            }
            return skillTimeConstant;
        }

        internal DirectSkill(DirectEve directEve, long itemId, int typeId, int flagId, long locationId ) : base(directEve)
        {
            this.itemId = itemId;
            this.typeId = typeId;
            this.flagId = flagId;
            this.locationId = locationId;
        }

        internal static Dictionary<int, string> GetAllSkills(DirectEve directEve)
        {
            // __builtin__.sm.services["skills"].allskills[0]
            var allskills = directEve.PySharp.Import("__builtin__").Attribute("sm").Attribute("services").DictionaryItem("skills").Call("GetAllSkills").ToList();

            var skills = new Dictionary<int, string>();

            foreach (var skill in allskills) {
                var typeId = skill.Attribute("typeID").ToInt();
                skills.Add(typeId, FindName(directEve, typeId));
            }
            return skills;
        }

        internal static List<DirectSkill> GetMySkills(DirectEve directEve)
        {
            var myskills = directEve.GetLocalSvc("skills").Call("MySkills").ToList();

            return myskills.Select(skill => new DirectSkill(directEve,
                skill.Attribute("itemID").ToLong(),
                skill.Attribute("typeID").ToInt(),
                skill.Attribute("flagID").ToInt(),
                skill.Attribute("locationID").ToLong()
                )).ToList();
        }

        
    }
}
