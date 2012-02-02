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

        //static library of methods by Ganondorf 02/02/2012 for skillplan execution support
        //try to keep the interface when moving/integrating them


        /// <summary>
        ///   Gets the current skillqueue length.
        /// </summary>
        /// <param name = "directEve"></param>
        /// <returns>a TimeSpan indicating the length of the queue or MaxValue if the service isn't running</returns>
        public static TimeSpan GetSkillQueueLenght(DirectEve directEve)
        {
            var skillQueueService = directEve.GetLocalSvc("skillqueue");
            if (skillQueueService == null || skillQueueService == global::DirectEve.PySharp.PySharp.PyZero)
                return TimeSpan.MaxValue;
            long skillSpan = (long)skillQueueService.Call("GetTrainingLengthOfQueue");
            return new TimeSpan(skillSpan);
        }

        /// <summary>
        ///   Gets the currently trained skill levels by id.
        /// </summary>
        /// <param name = "directEve"></param>
        /// <returns>a Dictionary with the skill typeId as key, and its level as value</returns>
        public static Dictionary<int, int> GetSkillLevelsById(DirectEve directEve)
        {
            var skillService = directEve.GetLocalSvc("skills");
            var skillDic = skillService.Call("MySkillLevelsByID").ToDictionary();

            return skillDic.ToDictionary(s => (int)s.Key, s => (int)s.Value);
        }

        /// <summary>
        ///   Checks if a skill is in the current skillqueue and returns which levels are queued.
        /// </summary>
        /// <param name = "directEve"></param>
        /// <param name = "skillId">TypeId of the skill to look for in the queue.</param>
        /// <returns>a List of ints with all the levels queued for skillId</returns>
        public static IEnumerable<int> isInQueue(DirectEve directEve, int skillId)
        {
            var skillQueueService = directEve.GetLocalSvc("skillqueue");
            var skillQList = skillQueueService.Attribute("skillQueue").ToList();
            return skillQList.Where(s => s.ToList<int>().First() == skillId).Select(s => s.ToList<int>().Last());

        }       

        /// <summary>
        ///   Basically a IntPair.
        /// </summary>
        public class QueueEntry
        {
            int skill;
            int level;
            public QueueEntry(int _skill, int _level)
            {
                skill = _skill;
                level = _level;
            }
        }

        /// <summary>
        ///   Gets a list of QueueEntries to represent the current skillqueue.
        /// </summary>
        /// <param name = "directEve"></param>        
        /// <returns>a List of QueueEntry</returns>
        public static List<QueueEntry> GetSkillsInQueue(DirectEve directEve)
        {
            var skillQueueService = directEve.GetLocalSvc("skillqueue");
            var skillQList = skillQueueService.Attribute("skillQueue").ToList();
            return (List<QueueEntry>)skillQList.Select(s => new QueueEntry(s.ToList<int>().First(), s.ToList<int>().Last()));
        }

        /// <summary>
        ///   Adds a skill to the end of the queue.
        /// </summary>
        /// <param name = "directEve"></param>   
        /// <param name = "skillId">typeId of the skill to add</param>   
        /// <param name = "currentLevel">current level of the skill</param>
        /// <param name = "skillLevel">level to add to the queue</param>
        /// <returns>true if the call was successfull, false otherwise</returns>
        public static bool AddSkillToEndOfQueue(DirectEve directEve, int skillId, int currentLevel, int skillLevel)
        {
            return directEve.ThreadedLocalSvcCall("skillqueue", "AddSkillToEnd", skillId, currentLevel, skillLevel);
        }


        /// <summary>
        ///   Adds a skill to the front of the queue and starts training it.
        /// </summary>
        /// <param name = "directEve"></param>   
        /// <param name = "skillId">typeId of the skill to add</param>           
        /// <param name = "skillLevel">level to add to the queue</param>
        /// <returns>true if the call was successfull, false otherwise</returns>
        public static bool TrainSkillNow(DirectEve directEve, int skillId, int skillLevel)
        {
            if (skillLevel < 1 || skillLevel > 5) return false;
            return directEve.ThreadedLocalSvcCall("skillqueue", "TrainSkillNow", skillId, skillLevel);
        }

        /// <summary>
        ///   Injects a skill into the character's brain.
        /// </summary>
        /// <param name = "directEve"></param>   
        /// <param name = "skillItemId">itemId of the skill to add</param>          
        /// <returns>true if the call was successfull, false otherwise</returns>
        public static bool InjectSkill(DirectEve directEve, long skillItemId)
        {
            DirectContainer hangar = directEve.GetItemHangar();
            var skillList = hangar.Items.Where(i => i.ItemId == skillItemId).Select(i => i.PyItem);
            if (skillList == null)
            {                
                return false;
            }
            if (skillList.Count() == 0)
            {               
                return false;
            }
            return directEve.ThreadedLocalSvcCall("menu", "InjectSkillIntoBrain", skillList);
        }
    }
}
