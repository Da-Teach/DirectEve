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

    public class DirectSkills : DirectObject
    {
        private List<DirectInvType> _allSkills;
        private List<DirectSkill> _mySkillQueue;
        private List<DirectSkill> _mySkills;

        private TimeSpan? _skillQueueLength;

        internal DirectSkills(DirectEve directEve) : base(directEve)
        {
        }

        /// <summary>
        ///   Return the skill queue length
        /// </summary>
        public TimeSpan SkillQueueLength
        {
            get { return (TimeSpan) (_skillQueueLength ?? (_skillQueueLength = new TimeSpan((long) DirectEve.GetLocalSvc("skillqueue").Call("GetTrainingLengthOfQueue")))); }
        }

        /// <summary>
        ///   Return all skills in the game
        /// </summary>
        public List<DirectInvType> AllSkills
        {
            get
            {
                if (_allSkills == null)
                {
                    _allSkills = new List<DirectInvType>();
                    var pySkills = DirectEve.GetLocalSvc("skills").Call("GetAllSkills").ToList();
                    foreach (var pySkill in pySkills)
                    {
                        var skill = new DirectInvType(DirectEve);
                        skill.TypeId = (int) pySkill.Attribute("typeID");
                        _allSkills.Add(skill);
                    }
                }

                return _allSkills;
            }
        }

        /// <summary>
        ///   Returns if MySkills is valid
        /// </summary>
        public bool AreMySkillsReady
        {
            get { return DirectEve.GetLocalSvc("skills").Attribute("myskills").IsValid; }
        }

        /// <summary>
        ///   Return my skills
        /// </summary>
        public List<DirectSkill> MySkills
        {
            get
            {
                if (_mySkills == null)
                    _mySkills = DirectEve.GetLocalSvc("skills").Attribute("myskills").ToList().Select(s => new DirectSkill(DirectEve, s)).ToList();

                return _mySkills;
            }
        }

        /// <summary>
        ///   Return the current skill queue
        /// </summary>
        public List<DirectSkill> MySkillQueue
        {
            get
            {
                if (_mySkillQueue == null)
                {
                    var pySkills = DirectEve.GetLocalSvc("skillqueue").Attribute("skillQueue").ToList();

                    _mySkillQueue = new List<DirectSkill>();
                    foreach (var pySkill in pySkills)
                    {
                        var skill = new DirectSkill(DirectEve, global::DirectEve.PySharp.PySharp.PyZero);
                        skill.TypeId = (int) pySkill.Item(0);
                        skill.Level = (int) pySkill.Item(1);
                        _mySkillQueue.Add(skill);
                    }
                }

                return _mySkillQueue;
            }
        }

        /// <summary>
        ///   Is the skill data ready?
        /// </summary>
        public bool IsReady
        {
            get { return DirectEve.GetLocalSvc("skillqueue").IsValid && DirectEve.GetLocalSvc("skills").IsValid; }
        }

        /// <summary>
        ///   Refresh MySkills
        /// </summary>
        /// <returns></returns>
        public bool RefreshMySkills()
        {
            if (!DirectEve.HasSupportInstances())
            {
                DirectEve.Log("DirectEve: Error: This method requires a support instance.");
                return false;
            }
            
            var mySkills = DirectEve.GetLocalSvc("skills").Attribute("MySkills");

            var keywords = new Dictionary<string, object>();
            keywords.Add("renew", 1);
            return DirectEve.ThreadedCallWithKeywords(mySkills, keywords);
        }

        /// <summary>
        ///   Add a skill to the end of the queue
        /// </summary>
        /// <param name = "skill"></param>
        /// <returns></returns>
        public bool AddSkillToEndOfQueue(DirectInvType skill)
        {
            if (!DirectEve.HasSupportInstances())
            {
                DirectEve.Log("DirectEve: Error: This method requires a support instance.");
                return false;
            }
            
            if (!AreMySkillsReady)
                return false;

            // Assume level 1
            var currentLevel = 1;

            // Get the skill from 'MySkills'
            var mySkill = MySkills.FirstOrDefault(s => s.TypeId == skill.TypeId);
            if (mySkill != null)
                currentLevel = mySkill.Level;

            // Assume 1 level higher then current
            var nextLevel = currentLevel + 1;

            // Check if the skill is already in the queue
            // due to the OrderByDescending on Level, this will 
            // result in the highest version of this skill in the queue
            mySkill = MySkillQueue.OrderByDescending(s => s.Level).FirstOrDefault(s => s.TypeId == skill.TypeId);
            if (mySkill != null)
                nextLevel = mySkill.Level + 1;

            return DirectEve.ThreadedLocalSvcCall("skillqueue", "AddSkillToEnd", skill.TypeId, currentLevel, nextLevel);
        }

        /// <summary>
        ///   Add a skill to the start of the queue
        /// </summary>
        /// <param name = "skill"></param>
        /// <returns></returns>
        public bool TrainSkillNow(DirectInvType skill)
        {
            if (!DirectEve.HasSupportInstances())
            {
                DirectEve.Log("DirectEve: Error: This method requires a support instance.");
                return false;
            }
            
            if (!AreMySkillsReady)
                return false;

            // Assume level 1
            var currentLevel = 1;

            // Get the skill from 'MySkills'
            var mySkill = MySkills.FirstOrDefault(s => s.TypeId == skill.TypeId);
            if (mySkill != null)
                currentLevel = mySkill.Level;

            return DirectEve.ThreadedLocalSvcCall("skillqueue", "TrainSkillNow", skill.TypeId, currentLevel);
        }

        // Doesn't work
        ///// <summary>
        /////   Remove's a skill from the queue (note only use DirectSkill's from the MySkillQueue list!)
        ///// </summary>
        ///// <param name="skill"></param>
        ///// <returns></returns>
        //public bool RemoveSkillFromQueue(DirectSkill skill)
        //{
        //    if (skill.PyItem.IsValid)
        //        return false;

        //    DirectEve.GetLocalSvc("skillqueue").Call("RemoveSkillFromQueue", skill.TypeId, skill.Level);
        //    if (!DirectEve.GetLocalSvc("skillqueue").Attribute("cachedSkillQueue").IsValid)
        //        return false;

        //    return DirectEve.ThreadedLocalSvcCall("skillqueue", "CommitTransaction");
        //}
        
        // This only pauses
        ///// <summary>
        /////   Abort the current skill in training
        ///// </summary>
        ///// <returns></returns>
        //public bool AbortTraining()
        //{
        //    var godma = DirectEve.GetLocalSvc("godma");
        //    if (!godma.Attribute("skillHandler").IsValid)
        //    {
        //        DirectEve.ThreadedCall(godma.Attribute("GetSkillHandler"));
        //        return false;
        //    }

        //    return DirectEve.ThreadedCall(godma.Attribute("skillHandler").Attribute("CharStopTrainingSkill"));
        //}
    }
}