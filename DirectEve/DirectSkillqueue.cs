using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DirectEve
{
    class DirectSkillqueue
    {               

        /// <summary>
        ///   Gets the current skillqueue length.
        /// </summary>
        /// <param name = "directEve"></param>
        /// <returns>a TimeSpan indicating the length of the queue or MaxValue if the service isn't running</returns>
        internal static TimeSpan GetSkillQueueLenght(DirectEve directEve)
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
        internal static Dictionary<int, int> GetSkillLevelsById(DirectEve directEve)
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
        internal static IEnumerable<int> isInQueue(DirectEve directEve, int skillId)
        {
            var skillQueueService = directEve.GetLocalSvc("skillqueue");
            var skillQList = skillQueueService.Attribute("skillQueue").ToList();
            return skillQList.Where(s => s.ToList<int>().First() == skillId).Select(s => s.ToList<int>().Last());

        }       

        

        /// <summary>
        ///   Gets a list of QueueEntries to represent the current skillqueue.
        /// </summary>
        /// <param name = "directEve"></param>        
        /// <returns>a List of QueueEntry</returns>
        internal static List<DirectEve.QueueEntry> GetSkillsInQueue(DirectEve directEve)
        {
            var skillQueueService = directEve.GetLocalSvc("skillqueue");
            var skillQList = skillQueueService.Attribute("skillQueue").ToList();
            return (List<DirectEve.QueueEntry>)skillQList.Select(s => new DirectEve.QueueEntry(s.ToList<int>().First(), s.ToList<int>().Last()));
        }

        /// <summary>
        ///   Adds a skill to the end of the queue.
        /// </summary>
        /// <param name = "directEve"></param>   
        /// <param name = "skillId">typeId of the skill to add</param>   
        /// <param name = "currentLevel">current level of the skill</param>
        /// <param name = "skillLevel">level to add to the queue</param>
        /// <returns>true if the call was successfull, false otherwise</returns>
        internal static bool AddSkillToEndOfQueue(DirectEve directEve, int skillId, int currentLevel, int skillLevel)
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
        internal static bool TrainSkillNow(DirectEve directEve, int skillId, int skillLevel)
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
        internal static bool InjectSkill(DirectEve directEve, long skillItemId)
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
