using System;
using System.Collections.Generic;
using UnityEngine;
using SkillSystem;

namespace GfxModule.Skill.Trigers
{
    public enum CrossDirection
    {
        kUnknown,
        kTopLeft,
        kTopRight,
        kBottomLeft,
        kBottomRight
    }

    public class FruitNinjiaTrigger : AbstractSkillTriger
    {
        public override ISkillTriger Clone()
        {
            FruitNinjiaTrigger copy = new FruitNinjiaTrigger();
            copy.m_StartTime = m_StartTime;
            copy.m_RemainTime = m_RemainTime;
            copy.m_DamageInterval = m_DamageInterval;
            foreach (StateImpact si in m_StateImpacts.Values)
            {
                copy.m_StateImpacts.Add(si.m_State, si);
            }
            foreach (DirectionAnimInfo anim_info in m_DirectionAnims.Values)
            {
                copy.m_DirectionAnims.Add(anim_info.direction, (DirectionAnimInfo)anim_info.Clone());
            }
            return copy;
        }

        public override void Reset()
        {
            m_IsInited = false;
            m_CurDirectionAnimInfo = null;
            DashFire.LogicSystem.EventChannelForGfx.Publish("ex_skill_end", "skill");
        }

        protected override void Load(ScriptableData.CallData callData)
        {
            if (callData.GetParamNum() >= 1)
            {
                m_StartTime = long.Parse(callData.GetParamId(0));
                m_RemainTime = long.Parse(callData.GetParamId(1));
                m_DamageInterval = long.Parse(callData.GetParamId(2));
            }
        }

        protected override void Load(ScriptableData.FunctionData funcData)
        {
            ScriptableData.CallData callData = funcData.Call;
            if (null != callData)
            {
                Load(callData);

                foreach (ScriptableData.ISyntaxComponent statement in funcData.Statements)
                {
                    ScriptableData.CallData stCall = statement as ScriptableData.CallData;
                    if (null == stCall || stCall.GetParamNum() <= 0)
                    {
                        continue;
                    }
                    string id = stCall.GetId();
                    if (id == "stateimpact")
                    {
                        LoadStateImpactConfig(stCall);
                    }
                    if (id == "directionanim")
                    {
                        LoadDirectionAnimConfig(stCall);
                    }
                }
            }
        }

        public override bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime)
        {
            if (curSectionTime < m_StartTime)
            {
                return true;
            }
            if (curSectionTime > m_StartTime + m_RemainTime)
            {
                return false;
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

            if (m_CurDirectionAnimInfo != null)
            {
                m_CurDirectionAnimInfo.Update();
            }

            if (!DashFire.GfxSystem.IsTouchPosChanged())
            {
                return true;
            }
            Vector3 pos = GetTouchPos();
            Vector3 lastpos = GetLastPos();

            UpdateDamgedObjectCD(delta);
            bool is_damaged = DamageCrossObjects(obj, instance, pos);

            UpdateDirectionAnim(pos, lastpos, is_damaged);

            m_LastTouchPos = pos;
            return true;
        }

        private void UpdateDirectionAnim(Vector3 touchpos, Vector3 lastpos, bool is_damaged)
        {
            if (m_CurDirectionAnimInfo == null)
            {
                CrossDirection cur_direction = GetDirection(touchpos, lastpos);
                if (m_DirectionAnims.TryGetValue(cur_direction, out m_CurDirectionAnimInfo))
                {
                    m_CurDirectionAnimInfo.Start();
                }
            }
            else
            {
                if (m_CurDirectionAnimInfo.IsCrossOver())
                {
                    m_CurDirectionAnimInfo.Stop();
                    m_CurDirectionAnimInfo = null;
                }
            }
        }

        private CrossDirection GetDirection(Vector3 touchpos, Vector3 lastpos)
        {
            float delta_x = touchpos.x - lastpos.x;
            float delta_y = touchpos.y - lastpos.y;
            if (delta_x >= 0)
            {
                if (delta_y >= 0)
                {
                    return CrossDirection.kBottomLeft;
                }
                else
                {
                    return CrossDirection.kTopLeft;
                }
            }
            else
            {
                if (delta_y >= 0)
                {
                    return CrossDirection.kBottomRight;
                }
                else
                {
                    return CrossDirection.kTopRight;
                }
            }
        }

        private bool DamageCrossObjects(GameObject obj, SkillInstance instance, Vector3 pos)
        {
            bool is_damaged_somebody = false;
            SkillDamageManager damage_manager = instance.CustomDatas.GetData<SkillDamageManager>();
            if (damage_manager == null)
            {
                damage_manager = new SkillDamageManager(obj);
                instance.CustomDatas.AddData<SkillDamageManager>(damage_manager);
            }
            List<GameObject> hit_objes = TriggerUtil.GetRayHitObjects("Character", pos);
            List<GameObject> enimies = TriggerUtil.FiltEnimy(obj, hit_objes);
            int final_skill_id = -1;
            GameObject source_obj = TriggerUtil.GetFinalOwner(obj, final_skill_id, out final_skill_id);
            foreach (GameObject hit_obj in enimies)
            {
                if (!SkillDamageManager.IsEnemy(obj, hit_obj.gameObject))
                {
                    continue;
                }
                if (IsObjectCanDamage(hit_obj))
                {
                    damage_manager.SendImpactToObject(source_obj, hit_obj, m_StateImpacts, final_skill_id);
                    DamageObjectInfo a = new DamageObjectInfo();
                    a.obj = hit_obj;
                    a.remainDamageCD = m_DamageInterval / 1000.0f;
                    m_DamageCDObjects.Add(a);
                    is_damaged_somebody = true;
                }
            }
            if (is_damaged_somebody)
            {
                damage_manager.IsDamagedEnemy = true;
                instance.SendMessage("oncollide");
            }
            return is_damaged_somebody;
        }

        private Vector3 GetTouchPos()
        {
            Vector3 pos = Vector3.zero;
            pos.x = DashFire.GfxSystem.GetTouchPointX();
            pos.y = DashFire.GfxSystem.GetTouchPointY();
            pos.z = DashFire.GfxSystem.GetTouchPointZ();
            return pos;
        }

        private Vector3 GetLastPos()
        {
            if (m_LastTouchPos == Vector3.zero)
            {
                return GetTouchPos();
            }
            return m_LastTouchPos;
        }

        private void UpdateDamgedObjectCD(long delta)
        {
            float seconds = TriggerUtil.ConvertToSecond(delta);
            for (int i = m_DamageCDObjects.Count - 1; i >= 0; i--)
            {
                DamageObjectInfo info = m_DamageCDObjects[i];
                info.remainDamageCD -= seconds;
                if (info.remainDamageCD < 0)
                {
                    m_DamageCDObjects.RemoveAt(i);
                }
            }
        }

        private void Init(GameObject obj)
        {
            m_IsInited = true;
            m_LastTouchPos = Vector3.zero;
            DashFire.LogicSystem.EventChannelForGfx.Publish("ex_skill_start", "skill");
            foreach (DirectionAnimInfo dai in m_DirectionAnims.Values)
            {
                dai.Init(obj);
            }
        }

        private void LoadStateImpactConfig(ScriptableData.CallData stCall)
        {
            StateImpact stateimpact = TriggerUtil.ParseStateImpact(stCall);
            m_StateImpacts[stateimpact.m_State] = stateimpact;
        }

        private void LoadDirectionAnimConfig(ScriptableData.CallData stCall)
        {
            if (stCall.GetParamNum() >= 3)
            {
                DirectionAnimInfo dir_anim_info = new DirectionAnimInfo();
                dir_anim_info.direction = CrossDirectionFromStr(stCall.GetParamId(0));
                dir_anim_info.onceAnimationName = stCall.GetParamId(1);
                dir_anim_info.loopAnimationName = stCall.GetParamId(2);
                m_DirectionAnims.Add(dir_anim_info.direction, dir_anim_info);
            }
        }

        private CrossDirection CrossDirectionFromStr(string str)
        {
            CrossDirection result = CrossDirection.kUnknown;
            switch (str)
            {
                case "TopLeft":
                    result = CrossDirection.kTopLeft;
                    break;
                case "TopRight":
                    result = CrossDirection.kTopRight;
                    break;
                case "BottomLeft":
                    result = CrossDirection.kBottomLeft;
                    break;
                case "BottomRight":
                    result = CrossDirection.kBottomRight;
                    break;
            }
            return result;
        }

        private bool IsObjectCanDamage(GameObject target)
        {
            foreach (DamageObjectInfo dam_info in m_DamageCDObjects)
            {
                if (target == dam_info.obj)
                {
                    return false;
                }
            }
            return true;
        }

        private long m_RemainTime;
        private long m_DamageInterval;
        private Dictionary<BeHitState, StateImpact> m_StateImpacts = new Dictionary<BeHitState, StateImpact>();
        private Dictionary<CrossDirection, DirectionAnimInfo> m_DirectionAnims = new Dictionary<CrossDirection, DirectionAnimInfo>();

        private bool m_IsInited = false;
        private Vector3 m_LastTouchPos;
        private List<DamageObjectInfo> m_DamageCDObjects = new List<DamageObjectInfo>();
        private DirectionAnimInfo m_CurDirectionAnimInfo = null;
    }

    public class DirectionAnimInfo : ICloneable
    {
        enum AnimSection
        {
            kNone,
            kCrossSection,
            kWaitSection,
        }
        public CrossDirection direction;
        public string onceAnimationName;
        public string loopAnimationName;

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public void Init(GameObject owner)
        {
            m_Owner = owner;
            m_Animation = m_Owner.GetComponent<Animation>();
        }

        public void Start()
        {
            PlayAnimation(onceAnimationName, WrapMode.Once);
            m_CurAnimSection = AnimSection.kCrossSection;
        }

        public void Update()
        {
            if (m_CurAnimSection == AnimSection.kCrossSection && !m_Owner.GetComponent<Animation>().IsPlaying(onceAnimationName))
            {
                PlayAnimation(loopAnimationName, WrapMode.Loop);
                m_CurAnimSection = AnimSection.kWaitSection;
            }
        }

        public bool IsCrossOver()
        {
            return m_CurAnimSection == AnimSection.kWaitSection;
        }

        public void Stop()
        {
            m_CurAnimSection = AnimSection.kNone;
        }

        private void PlayAnimation(string animname, WrapMode mode)
        {
            AnimationState once_anim = m_Owner.GetComponent<Animation>()[animname];
            if (once_anim != null)
            {
                once_anim.wrapMode = mode;
                m_Owner.GetComponent<Animation>().CrossFade(animname, 0.1f);
            }
        }

        private GameObject m_Owner;
        private Animation m_Animation;
        private AnimSection m_CurAnimSection = AnimSection.kNone;
    }

    public class DamageObjectInfo
    {
        public GameObject obj;
        public float remainDamageCD;
    }
}
