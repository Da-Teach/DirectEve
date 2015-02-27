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
    using PySharp;

    /// <summary>
    ///     Skill in the game
    /// </summary>
    public class DirectSkill : DirectItem
    {
        private int? _level;
        private PyObject _pyGodmaItem;
        private int? _skillPoints;
        private int? _skillTimeConstant;

        internal DirectSkill(DirectEve directEve, PyObject pySkill) : base(directEve)
        {
            PyItem = pySkill;
        }

        internal PyObject PyGodmaItem
        {
            get
            {
                if (!PyItem.IsValid)
                    return global::DirectEve.PySharp.PySharp.PyZero;

                return _pyGodmaItem ?? (_pyGodmaItem = DirectEve.GetLocalSvc("godma").Call("GetItem", ItemId));
            }
        }

        /// <summary>
        ///     Are we currently training this skill?
        /// </summary>
        public bool InTraining
        {
            get { return FlagId == DirectEve.Const.FlagSkillInTraining; }
        }

        /// <summary>
        ///     Level of skill
        /// </summary>
        public int Level
        {
            get { return (int) (_level ?? (_level = (int) PyGodmaItem.Attribute("skillLevel"))); }
            set { _level = value; }
        }

        /// <summary>
        ///     Number of points in this skill
        /// </summary>
        public int SkillPoints
        {
            get { return (int) (_skillPoints ?? (_skillPoints = (int) PyGodmaItem.Attribute("skillPoints"))); }
        }

        /// <summary>
        ///     Time multiplier to indicate relative training time
        /// </summary>
        public int SkillTimeConstant
        {
            get { return (int) (_skillTimeConstant ?? (_skillTimeConstant = (int) PyGodmaItem.Attribute("skillTimeConstant"))); }
        }

        /// <summary>
        ///     Enqueue this skill at the end of the queue
        /// </summary>
        /// <returns></returns>
        public bool AddToEndOfQueue()
        {
            //if (!DirectEve.HasSupportInstances())
            //{
            //    DirectEve.Log("DirectEve: Error: This method requires a support instance.");
            //    return false;
            //}

            return DirectEve.Skills.AddSkillToEndOfQueue(this);
        }

        /// <summary>
        ///     Train this skill right now (add it at the start of the queue)
        /// </summary>
        /// <returns></returns>
        public bool TrainNow()
        {
            //if (!DirectEve.HasSupportInstances())
            //{
            //    DirectEve.Log("DirectEve: Error: This method requires a support instance.");
            //    return false;
            //}

            return DirectEve.Skills.TrainSkillNow(this);
        }

        // TODO: This doesnt work :(
        ///// <summary>
        /////   Remove this skill from the queue (only use on skills from the MySkillQueue list)
        ///// </summary>
        ///// <returns></returns>
        //public bool RemoveFromQueue()
        //{
        //    return DirectEve.Skills.RemoveSkillFromQueue(this);
        //}
    }
}