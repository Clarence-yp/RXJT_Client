using System;
using System.Collections.Generic;
using UnityEngine;
using SkillSystem;

namespace GfxModule.Skill.Trigers
{
    public class SetUIVisibleTrigger : AbstractSkillTriger
    {
        public override ISkillTriger Clone()
        {
            SetUIVisibleTrigger copy = new SetUIVisibleTrigger();
            copy.m_StartTime = m_StartTime;
            copy.m_UIWindowName = m_UIWindowName;
            copy.m_IsShow = m_IsShow;
            return copy;
        }

        public override void Reset()
        {
        }

        protected override void Load(ScriptableData.CallData callData)
        {
            if (callData.GetParamNum() >= 3)
            {
                m_StartTime = long.Parse(callData.GetParamId(0));
                m_UIWindowName = callData.GetParamId(1);
                m_IsShow = bool.Parse(callData.GetParamId(2));
            }
        }

        public override bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime)
        {
            if (curSectionTime < m_StartTime)
            {
                return true;
            }
            GameObject obj = sender as GameObject;
            if (obj == null)
            {
                return false;
            }
            string message = m_IsShow ? "show_ui" : "hide_ui";
            DashFire.LogicSystem.EventChannelForGfx.Publish(message, "ui", m_UIWindowName);
            return false;
        }

        private string m_UIWindowName;
        private bool m_IsShow;
    }
}
