using System;
using System.Collections.Generic;
using UnityEngine;
using SkillSystem;

namespace GfxModule.Skill.Trigers
{
    public class CrossSummonMoveTrigger : AbstractSkillTriger
    {
        public override ISkillTriger Clone()
        {
            CrossSummonMoveTrigger copy = new CrossSummonMoveTrigger();
            copy.m_StartTime = m_StartTime;
            copy.m_IsSummon = m_IsSummon;
            copy.m_StartIndex = m_StartIndex;
            copy.m_EndIndex = m_EndIndex;
            copy.m_MoveTime = m_MoveTime;
            return copy;
        }

        public override void Reset()
        {
            m_IsInited = false;
        }

        protected override void Load(ScriptableData.CallData callData)
        {
            if (callData.GetParamNum() >= 5)
            {
                m_StartTime = long.Parse(callData.GetParamId(0));
                m_IsSummon = bool.Parse(callData.GetParamId(1));
                m_StartIndex = int.Parse(callData.GetParamId(2));
                m_EndIndex = int.Parse(callData.GetParamId(3));
                m_MoveTime = long.Parse(callData.GetParamId(4));
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
            if (!m_IsInited)
            {
                Init(obj);
            }
            if (m_CurTargetSummon != null)
            {
                long passed_time = curSectionTime - (m_NextSummonBeginTime - m_MoveTime);
                if (passed_time > m_MoveTime)
                {
                    passed_time = m_MoveTime;
                }
                Vector3 motion_pos = Vector3.Lerp(m_StartPos, m_CurTargetSummon.transform.position,
                                                  passed_time * 1.0f / m_MoveTime);
                TriggerUtil.MoveObjTo(obj, motion_pos);
                TriggerUtil.UpdateObjPosition(obj);
            }
            if (m_CurTargetSummon == null || curSectionTime > m_NextSummonBeginTime)
            {
                m_CurIndex++;
                if (m_CurIndex >= m_SummonList.Count || (m_EndIndex >= m_StartIndex && m_CurIndex > m_EndIndex))
                {
                    return false;
                }
                InitNextSummonInfo(obj);
            }
            return true;
        }

        private void Init(GameObject obj)
        {
            m_IsInited = true;
            m_SummonList = GetSummonList(obj);
            m_CurIndex = m_StartIndex;
            m_NextSummonBeginTime = m_StartTime;
            if (m_CurIndex < 0)
            {
                m_CurIndex = 0;
            }
            else
            {
                GameObject target = GetGameObjectByIndex(m_CurIndex);
                if (target != null)
                {
                    TriggerUtil.MoveObjTo(obj, target.transform.position);
                }
                m_CurIndex++;
            }
            InitNextSummonInfo(obj);
        }

        private void InitNextSummonInfo(GameObject obj)
        {
            m_CurTargetSummon = GetGameObjectByIndex(m_CurIndex);
            m_StartPos = obj.transform.position;
            m_NextSummonBeginTime += m_MoveTime;
            if (m_CurTargetSummon != null)
            {
                Vector3 dir = m_CurTargetSummon.transform.position - m_StartPos;
                GfxSkillSystem.ChangeDir(obj, dir);
            }
        }

        private GameObject GetGameObjectByIndex(int index)
        {
            GameObject result = null;
            if (0 <= index && index < m_SummonList.Count)
            {
                result = DashFire.LogicSystem.GetGameObject(m_SummonList[index]);
            }
            return result;
        }

        private List<int> GetSummonList(GameObject obj)
        {
            List<int> result = new List<int>();
            DashFire.SharedGameObjectInfo share_info = DashFire.LogicSystem.GetSharedGameObjectInfo(obj);
            if (share_info == null)
            {
                return result;
            }
            if (!m_IsSummon)
            {
                result.AddRange(share_info.Summons);
            }
            else if (share_info.SummonOwnerActorId >= 0)
            {
                DashFire.SharedGameObjectInfo owner_info = DashFire.LogicSystem.GetSharedGameObjectInfo(share_info.SummonOwnerActorId);
                if (owner_info != null)
                {
                    for (int i = 0; i < owner_info.Summons.Count; ++i)
                    {
                        int actorid = owner_info.Summons[i];
                        if (actorid != share_info.m_ActorId)
                        {
                            result.Add(actorid);
                        }
                    }
                }
            }
            return result;
        }

        private bool m_IsSummon;
        private int m_StartIndex;
        private int m_EndIndex;
        private long m_MoveTime;

        private bool m_IsInited = false;
        private List<int> m_SummonList = new List<int>();
        private int m_CurIndex;
        private long m_NextSummonBeginTime;
        private Vector3 m_StartPos;
        private GameObject m_CurTargetSummon;
    }
}
