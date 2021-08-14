using System;
using UnityEngine;
using SkillSystem;
using DashFire;

namespace GfxModule.Skill.Trigers
{
    public class AddLockInputTimeTrigger : AbstractSkillTriger
    {
        public override ISkillTriger Clone()
        {
            AddLockInputTimeTrigger copy = new AddLockInputTimeTrigger();
            copy.m_SkillCategory = m_SkillCategory;
            copy.m_LockInputTime = m_LockInputTime;
            return copy;
        }

        public override void Reset()
        {
        }

        protected override void Load(ScriptableData.CallData callData)
        {
            if (callData.GetParamNum() >= 2)
            {
                m_SkillCategory = (SkillCategory)int.Parse(callData.GetParamId(0));
                m_LockInputTime = long.Parse(callData.GetParamId(1)) / 1000.0f;
            }
        }

        public override bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime)
        {
            GameObject obj = sender as GameObject;
            if (obj == null)
            {
                return false;
            }
            LogicSystem.NotifyGfxAddLockInputTime(obj, m_SkillCategory, m_LockInputTime);
            return false;
        }

        private SkillCategory m_SkillCategory;
        private float m_LockInputTime;
    }
}
