using System;
using System.Collections.Generic;
using DashFire;
using SkillSystem;
using UnityEngine;
using GfxModule.Skill.Trigers;
using GfxModule.Skill;

namespace GfxModule.SkillResourceAnalysis.Trigers
{
    /// <summary>
    /// areadamage(start_time,center_x, center_y, center_z, range, is_clear_when_finish[,impact_id,...]) {
    ///   showtip(time, color_r, color_g, color_b);
    ///   stateimpact(statename, impact_id[,impact_id...]); 
    /// };
    /// </summary>
    internal class AreaDamageTriger : AbstractSkillTriger
    {
        public override ISkillTriger Clone()
        {
            AreaDamageTriger triger = new AreaDamageTriger();
            triger.m_Impacts = m_Impacts;
            return triger;
        }

        public override void Analyze(object sender, SkillInstance instance)
        {
            instance.EnableImpactsToOther.AddRange(m_Impacts);
        }

        protected override void Load(ScriptableData.FunctionData funcData)
        {
            ScriptableData.CallData callData = funcData.Call;
            if (null != callData)
            {
                foreach (ScriptableData.ISyntaxComponent statement in funcData.Statements)
                {
                    ScriptableData.CallData stCall = statement as ScriptableData.CallData;
                    if (null != stCall)
                    {
                        string id = stCall.GetId();
                        if (id == "stateimpact")
                        {
                            if (stCall.GetParamNum() > 1)
                            {
                                m_Impacts.Add(int.Parse(stCall.GetParamId(1)));
                            }
                        }
                    }
                }
            }
        }

        private List<int> m_Impacts = new List<int>();
    }

    public class SummonObjectTrigger : AbstractSkillTriger
    {
        public override ISkillTriger Clone()
        {
            SummonObjectTrigger copy = new SummonObjectTrigger();
            copy.m_StartTime = m_StartTime;
            copy.m_NpcTypeId = m_NpcTypeId;
            copy.m_ModelPrefab = m_ModelPrefab;
            copy.m_SkillId = m_SkillId;
            return copy;
        }

        public override void Analyze(object sender, SkillInstance instance)
        {
            instance.SummonNpcSkills.Add(m_SkillId);
            instance.Resources.Add(m_ModelPrefab);
        }

        protected override void Load(ScriptableData.CallData callData)
        {
            if (callData.GetParamNum() >= 4)
            {
                m_StartTime = long.Parse(callData.GetParamId(0));
                m_NpcTypeId = int.Parse(callData.GetParamId(1));
                m_ModelPrefab = callData.GetParamId(2);
                if (m_ModelPrefab == " ")
                {
                    m_ModelPrefab = "";
                }
                m_SkillId = int.Parse(callData.GetParamId(3));
            }
        }

        private int m_NpcTypeId;
        private string m_ModelPrefab;
        private int m_SkillId;
    }

    public class AddImpactToSelfTrigger : AbstractSkillTriger
    {
        public override ISkillTriger Clone()
        {
            AddImpactToSelfTrigger copy = new AddImpactToSelfTrigger();
            copy.m_StartTime = m_StartTime;
            copy.m_ImpactId = m_ImpactId;
            copy.m_RemainTime = m_RemainTime;
            return copy;
        }

        public override void Analyze(object sender, SkillInstance instance)
        {
            instance.EnableImpactsToMyself.Add(m_ImpactId);
        }

        protected override void Load(ScriptableData.CallData callData)
        {
            if (callData.GetParamNum() >= 2)
            {
                m_StartTime = long.Parse(callData.GetParamId(0));
                m_ImpactId = int.Parse(callData.GetParamId(1));
            }
            if (callData.GetParamNum() >= 3)
            {
                m_RemainTime = long.Parse(callData.GetParamId(2));
            }
        }

        private int m_ImpactId;
        private long m_RemainTime = -1;
    }

    /// <summary>
    /// charactereffect(effect_path,delete_time,attach_bone[,start_time]);
    /// 
    /// or
    /// 
    /// charactereffect(effect_path,delete_time,attach_bone[,start_time])
    /// {
    ///   transform(vector3(0,1,0)[,eular(0,0,0)[,vector3(1,1,1)]]);
    /// };
    /// </summary>
    internal class CharacterEffectTriger : AbstractSkillTriger
    {
        public override ISkillTriger Clone()
        {
            CharacterEffectTriger triger = new CharacterEffectTriger();
            triger.m_EffectPath = m_EffectPath;
            triger.m_AttachPath = m_AttachPath;
            triger.m_DeleteTime = m_DeleteTime;
            triger.m_StartTime = m_StartTime;
            triger.m_IsAttach = m_IsAttach;
            triger.m_Pos = m_Pos;
            triger.m_Dir = m_Dir;
            triger.m_Scale = m_Scale;
            return triger;
        }

        public override void Analyze(object sender, SkillInstance instance)
        {
            instance.Resources.Add(m_EffectPath);
        }

        protected override void Load(ScriptableData.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0)
            {
                m_EffectPath = callData.GetParamId(0);
            }
            if (num > 1)
            {
                m_DeleteTime = float.Parse(callData.GetParamId(1)) / 1000.0f;
            }
            if (num > 2)
            {
                m_AttachPath = callData.GetParamId(2);
            }
            if (num > 3)
            {
                m_StartTime = long.Parse(callData.GetParamId(3));
            }
            if (num > 4)
            {
                m_IsAttach = bool.Parse(callData.GetParamId(4));
            }
        }

        protected override void Load(ScriptableData.FunctionData funcData)
        {
            ScriptableData.CallData callData = funcData.Call;
            if (null != callData)
            {
                Load(callData);

                ScriptableData.ISyntaxComponent statement = funcData.Statements.Find(st => st.GetId() == "transform");
                if (null != statement)
                {
                    ScriptableData.CallData stCall = statement as ScriptableData.CallData;
                    if (null != stCall)
                    {
                        if (stCall.GetParamNum() > 0)
                        {
                            ScriptableData.CallData param0 = stCall.GetParam(0) as ScriptableData.CallData;
                            if (null != param0)
                                m_Pos = ScriptableDataUtility.CalcVector3(param0);
                        }
                        if (stCall.GetParamNum() > 1)
                        {
                            ScriptableData.CallData param1 = stCall.GetParam(1) as ScriptableData.CallData;
                            if (null != param1)
                                m_Dir = ScriptableDataUtility.CalcEularRotation(param1);
                        }
                        if (stCall.GetParamNum() > 2)
                        {
                            ScriptableData.CallData param2 = stCall.GetParam(2) as ScriptableData.CallData;
                            if (null != param2)
                                m_Scale = ScriptableDataUtility.CalcVector3(param2);
                        }
                    }
                }
            }
        }

        private string m_EffectPath = "";
        private string m_AttachPath = "";
        private float m_DeleteTime = 0;
        private bool m_IsAttach = true;

        private Vector3 m_Pos = Vector3.zero;
        private Quaternion m_Dir = Quaternion.identity;
        private Vector3 m_Scale = Vector3.one;
    }
    /// <summary>
    /// sceneeffect(effect_path,delete_time[,vector3(x,y,z)[,start_time[,eular(rx,ry,rz)[,vector3(sx,sy,sz)]]]]);
    /// </summary>
    internal class SceneEffectTriger : AbstractSkillTriger
    {
        public override ISkillTriger Clone()
        {
            SceneEffectTriger triger = new SceneEffectTriger();
            triger.m_EffectPath = m_EffectPath;
            triger.m_Pos = m_Pos;
            triger.m_Dir = m_Dir;
            triger.m_Scale = m_Scale;
            triger.m_DeleteTime = m_DeleteTime;
            triger.m_StartTime = m_StartTime;
            triger.m_IsRotateRelativeUser = m_IsRotateRelativeUser;
            return triger;
        }

        public override void Analyze(object sender, SkillInstance instance)
        {
            instance.Resources.Add(m_EffectPath);
        }

        protected override void Load(ScriptableData.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num > 0)
            {
                m_EffectPath = callData.GetParamId(0);
            }
            if (num > 1)
            {
                m_DeleteTime = float.Parse(callData.GetParamId(1)) / 1000.0f;
            }
            if (num > 2)
            {
                m_Pos = ScriptableDataUtility.CalcVector3(callData.GetParam(2) as ScriptableData.CallData);
            }
            if (num > 3)
            {
                m_StartTime = long.Parse(callData.GetParamId(3));
            }
            if (num > 4)
            {
                m_Dir = ScriptableDataUtility.CalcEularRotation(callData.GetParam(4) as ScriptableData.CallData);
            }
            if (num > 5)
            {
                m_Scale = ScriptableDataUtility.CalcVector3(callData.GetParam(5) as ScriptableData.CallData);
            }
            if (num > 6)
            {
                m_IsRotateRelativeUser = bool.Parse(callData.GetParamId(6));
            }
        }

        private string m_EffectPath = "";
        private Vector3 m_Pos = Vector3.zero;
        private Quaternion m_Dir = Quaternion.identity;
        private Vector3 m_Scale = Vector3.one;
        private float m_DeleteTime = 0;
        private long m_StartTime = 0;
        private bool m_IsRotateRelativeUser = false;
    }
    public class PlaySoundTriger : AbstractSkillTriger
    {
        public override ISkillTriger Clone()
        {
            PlaySoundTriger triger = new PlaySoundTriger();
            triger.m_StartTime = m_StartTime;
            triger.m_Name = m_Name;
            triger.m_AudioSource = m_AudioSource;
            triger.m_AudioSourceLifeTime = m_AudioSourceLifeTime;
            triger.m_AudioGroup.AddRange(m_AudioGroup);
            triger.m_IsNeedCollide = m_IsNeedCollide;
            triger.m_IsBoneSound = m_IsBoneSound;
            triger.m_Position = m_Position;
            triger.m_BoneName = m_BoneName;
            triger.m_IsAttach = m_IsAttach;
            return triger;
        }
        public override void Analyze(object sender, SkillInstance instance)
        {
            instance.Resources.Add(m_AudioSource);
            foreach (string audio in m_AudioGroup)
            {
                instance.Resources.Add(audio);
            }
        }
        private string GetRandomAudio()
        {
            int random_index = m_Random.Next(0, m_AudioGroup.Count);
            if (0 <= random_index && random_index < m_AudioGroup.Count)
            {
                return m_AudioGroup[random_index];
            }
            return "";
        }

        private AudioSource CreateNewAudioSource(GameObject obj)
        {
            if (string.IsNullOrEmpty(m_AudioSource))
            {
                return null;
            }
            GameObject audiosource_obj = DashFire.ResourceSystem.NewObject(
                                                   m_AudioSource,
                                                   m_AudioSourceLifeTime / 1000.0f) as GameObject;
            if (audiosource_obj == null)
            {
                return null;
            }
            AudioSource audiosource = audiosource_obj.GetComponent<AudioSource>();
            if (m_IsBoneSound)
            {
                Transform attach_node = TriggerUtil.GetChildNodeByName(obj, m_BoneName);
                if (attach_node != null)
                {
                    audiosource.transform.parent = attach_node;
                    audiosource.transform.rotation = Quaternion.identity;
                    audiosource.transform.position = Vector3.zero;
                    if (!m_IsAttach)
                    {
                        audiosource.transform.parent = null;
                    }
                }
                else
                {
                    audiosource.transform.position = obj.transform.TransformPoint(m_Position);
                    if (m_IsAttach)
                    {
                        audiosource.transform.parent = obj.transform;
                    }
                }
            }
            else
            {
                audiosource.transform.position = obj.transform.TransformPoint(m_Position);
                if (m_IsAttach)
                {
                    audiosource.transform.parent = obj.transform;
                }
            }
            return audiosource;
        }

        protected override void Load(ScriptableData.CallData callData)
        {
            int num = callData.GetParamNum();
            if (num >= 6)
            {
                m_StartTime = long.Parse(callData.GetParamId(0));
                m_Name = callData.GetParamId(1);
                m_AudioSource = callData.GetParamId(2);
                m_AudioSourceLifeTime = long.Parse(callData.GetParamId(3));
                m_AudioGroup.Add(callData.GetParamId(4));
                m_IsNeedCollide = bool.Parse(callData.GetParamId(5));
            }
        }

        protected override void Load(ScriptableData.FunctionData funcData)
        {
            ScriptableData.CallData callData = funcData.Call;
            if (null == callData)
            {
                return;
            }
            Load(callData);
            foreach (ScriptableData.ISyntaxComponent statement in funcData.Statements)
            {
                ScriptableData.CallData stCall = statement as ScriptableData.CallData;
                if (null == stCall)
                {
                    continue;
                }
                if (stCall.GetId() == "position")
                {
                    LoadPositionConfig(stCall);
                }
                else if (stCall.GetId() == "bone")
                {
                    LoadBoneConfig(stCall);
                }
                else if (stCall.GetId() == "audiogroup")
                {
                    LoadAudioGroup(stCall);
                }
            }
        }

        private void LoadAudioGroup(ScriptableData.CallData stCall)
        {
            for (int i = 0; i < stCall.GetParamNum(); i++)
            {
                m_AudioGroup.Add(stCall.GetParamId(i));
            }
        }

        private void LoadPositionConfig(ScriptableData.CallData stCall)
        {
            if (stCall.GetParamNum() >= 2)
            {
                m_IsBoneSound = false;
                m_Position = ScriptableDataUtility.CalcVector3(stCall.GetParam(0) as ScriptableData.CallData);
                m_IsAttach = bool.Parse(stCall.GetParamId(1));
            }
        }

        private void LoadBoneConfig(ScriptableData.CallData stCall)
        {
            if (stCall.GetParamNum() >= 2)
            {
                m_IsBoneSound = true;
                m_Position = ScriptableDataUtility.CalcVector3(stCall.GetParam(0) as ScriptableData.CallData); ;
                m_IsAttach = bool.Parse(stCall.GetParamId(1));
            }
        }

        public static string DefaultAudioName = "default";

        private string m_Name;
        private string m_AudioSource;
        private long m_AudioSourceLifeTime;
        private List<string> m_AudioGroup = new List<string>();
        private bool m_IsNeedCollide = false;

        private System.Random m_Random = new System.Random();
        private bool m_IsBoneSound = false;
        private Vector3 m_Position = new Vector3(0, 0, 0);
        private string m_BoneName = "";
        private bool m_IsAttach = true;
    }
    public class CreateShadowTrigger : AbstractSkillTriger
    {
        public override ISkillTriger Clone()
        {
            CreateShadowTrigger copy = new CreateShadowTrigger();
            copy.m_StartTime = m_StartTime;
            copy.m_CreateInterval = m_CreateInterval;
            copy.m_MaxShadowCount = m_MaxShadowCount;
            copy.m_HoldTime = m_HoldTime;
            copy.m_FadeOutTime = m_FadeOutTime;
            copy.m_ShadowMaterial = m_ShadowMaterial;
            copy.m_ShaderName = m_ShaderName;
            copy.m_StartAlpha = m_StartAlpha;
            copy.m_IgnoreList.AddRange(m_IgnoreList);
            return copy;
        }

        public override void Reset()
        {
            m_IsInited = false;
            m_IsCreateOver = false;
            m_CreatedCount = 0;
        }
        public override void Analyze(object sender, SkillInstance instance)
        {
            instance.Resources.Add(m_ShaderName);
            instance.Resources.Add(m_ShadowMaterial);
        }
        protected override void Load(ScriptableData.CallData callData)
        {
            if (callData.GetParamNum() >= 7)
            {
                m_StartTime = long.Parse(callData.GetParamId(0));
                m_CreateInterval = long.Parse(callData.GetParamId(1));
                m_MaxShadowCount = int.Parse(callData.GetParamId(2));
                m_HoldTime = long.Parse(callData.GetParamId(3));
                m_FadeOutTime = long.Parse(callData.GetParamId(4));
                m_ShadowMaterial = callData.GetParamId(5);
                m_ShaderName = callData.GetParamId(6);
                m_StartAlpha = float.Parse(callData.GetParamId(7));
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
                    if (id == "ignorelist")
                    {
                        m_IgnoreList.Add(stCall.GetParamId(0));
                    }
                }
            }
        }

        public void Init(SkillInstance instance, long curSectionTime)
        {
            m_IsInited = true;
            m_NextCreateTime = curSectionTime;
        }

        private void CreateShadow(GameObject obj, SkillInstance instance, long curSectionTime)
        {
            ShadowProduct shadow = new ShadowProduct(obj, m_ShadowMaterial, m_ShaderName,
                                                     m_HoldTime, m_FadeOutTime,
                                                     m_StartAlpha, m_IgnoreList);
            GfxSkillSystem.Instance.AddSkillProduct(shadow);
            m_CreatedCount++;
        }

        private long m_CreateInterval;
        private int m_MaxShadowCount;
        private long m_HoldTime;
        private long m_FadeOutTime;
        private string m_ShadowMaterial;
        private string m_ShaderName;
        private float m_StartAlpha;

        private int m_CreatedCount = 0;
        private long m_NextCreateTime;
        private bool m_IsInited = false;
        private bool m_IsCreateOver = false;
        private List<ShadowInfo> m_ShadowList = new List<ShadowInfo>();
        private List<String> m_IgnoreList = new List<String>();
    }
    internal class ColliderDamageTriger : AbstractSkillTriger
    {
        public override ISkillTriger Clone()
        {
            ColliderDamageTriger copy = new ColliderDamageTriger();
            copy.m_StartTime = m_StartTime;
            copy.m_RemainTime = m_RemainTime;
            copy.m_IsClearWhenFinish = m_IsClearWhenFinish;
            copy.m_IsAlwaysEnterDamage = m_IsAlwaysEnterDamage;
            copy.m_DamageInterval = m_DamageInterval;
            copy.m_MaxDamageTimes = m_MaxDamageTimes;
            copy.m_ColliderInfo = m_ColliderInfo;
            copy.m_ColliderCreated = m_ColliderCreated;
            return copy;
        }

        public override void Analyze(object sender, SkillInstance instance)
        {
            AttachConfig attach_config = m_ColliderInfo.GetAttachConfig();
            instance.EnableImpactsToOther.Add(attach_config.AttachImpact);
            instance.EnableImpactsToOther.Add(attach_config.FallImpact);
            instance.EnableImpactsToOther.Add(attach_config.FinalImpact);
            instance.Resources.Add(m_ColliderInfo.Prefab);
        }

        public override void Reset()
        {
            SendFinalImpact();
            foreach (AttachTargetInfo ati in m_AttachedObjects)
            {
                DashFire.LogicSystem.NotifyGfxMoveControlFinish(ati.TargetObj, m_OwnSkill.SkillId, true);
            }
            ClearDamagedObjIfNeed();
            m_LeaveDelObjects.Clear();
            m_MoreTimesEffectObjects.Clear();
            m_AttachedObjects.Clear();
            m_ColliderCreated = false;
            m_Owner = null;
            m_OwnSkill = null;
            m_DamageManager = null;
        }

        private void UpdateMoreTimeEffectObjects(SkillInstance instance)
        {
            float now = instance.CurTime / 1000.0f;
            foreach (EffectObjectInfo obj_info in m_MoreTimesEffectObjects)
            {
                if (obj_info.NextEffectTime <= now)
                {
                    //Debug.Log("-----------more time damage obj " + obj_info.TargetObj.name + " time=" + obj_info.NextEffectTime);
                    obj_info.NextEffectTime += m_DamageInterval / 1000.0f;
                    m_DamageManager.SendImpactToObject(m_DamageManager.GetOwner(), obj_info.TargetObj, m_ColliderInfo.GetStateImpacts(), instance.SkillId);
                }
            }
        }

        private void UpdateAttachObjects()
        {
            AttachConfig attach_config = m_ColliderInfo.GetAttachConfig();
            for (int i = m_AttachedObjects.Count - 1; i >= 0; --i)
            {
                AttachTargetInfo ati = m_AttachedObjects[i];
                if (ati.ParentObj != null)
                {
                    if (!UpdateAttachTargetPos(ati))
                    {
                        m_AttachedObjects.RemoveAt(i);
                        //Debug.Log("---FallImpact: send to " + ati.TargetObj.name);
                        m_DamageManager.SendImpactToObject(m_DamageManager.GetOwner(),
                                                           ati.TargetObj, attach_config.FallImpact, attach_config.FallImpactTime,
                                                           m_OwnSkill.SkillId);
                    }
                }
            }
        }

        protected override void Load(ScriptableData.FunctionData funcData)
        {
            ScriptableData.CallData callData = funcData.Call;
            if (null == callData)
            {
                return;
            }
            int num = callData.GetParamNum();
            if (num >= 6)
            {
                m_StartTime = long.Parse(callData.GetParamId(0));
                m_RemainTime = long.Parse(callData.GetParamId(1));
                m_IsClearWhenFinish = bool.Parse(callData.GetParamId(2));
                m_IsAlwaysEnterDamage = bool.Parse(callData.GetParamId(3));
                m_DamageInterval = long.Parse(callData.GetParamId(4));
                m_MaxDamageTimes = int.Parse(callData.GetParamId(5));
            }
            //碰撞体数据
            m_ColliderInfo.Load(funcData.Statements);
        }

        public void OnTriggerEnter(Collider collider)
        {
            if (m_DamageManager == null)
            {
                return;
            }
            string message = m_ColliderInfo.GetCollideLayerMessage(collider.gameObject.layer);
            if (!string.IsNullOrEmpty(message))
            {
                m_OwnSkill.SendMessage(message);
            }
            if (!m_OwnSkill.IsDamageEnable)
            {
                return;
            }

            if (m_DamageManager.AddDamagedObject(collider.gameObject))
            {
                if (m_ColliderInfo.GetAttachConfig().IsAttachEnemy)
                {
                    AddAttachObject(collider);
                }
                else
                {
                    m_DamageManager.SendImpactToObject(m_DamageManager.GetOwner(), collider.gameObject, m_ColliderInfo.GetStateImpacts(), m_OwnSkill.SkillId);
                    if (m_DamageInterval > 0)
                    {
                        Debug.Log("----------add more times obj " + collider.gameObject.name + " time=" + m_OwnSkill.CurTime);
                        AddMoreTimeEffectObject(collider.gameObject);
                    }
                    if (m_IsAlwaysEnterDamage)
                    {
                        m_LeaveDelObjects.Add(collider.gameObject);
                    }
                    m_DamagedObjects.Add(collider.gameObject);
                }
            }
            if (m_DamageManager.IsDamagedEnemy)
            {
                m_OwnSkill.SendMessage("oncollide");
            }
        }

        public void OnTriggerExit(Collider collider)
        {
            if (m_IsAlwaysEnterDamage)
            {
                m_LeaveDelObjects.Remove(collider.gameObject);
                if (m_DamageManager != null)
                {
                    m_DamageManager.RemoveGameObject(collider.gameObject);
                }
            }
        }

        public void OnDestroy()
        {
        }

        public void AddMoreTimeEffectObject(GameObject obj)
        {
            if (FindEffectInfoByObj(obj) != null)
            {
                return;
            }
            EffectObjectInfo effectinfo = new EffectObjectInfo();
            effectinfo.TargetObj = obj;
            effectinfo.StartEffectTime = m_OwnSkill.CurTime / 1000.0f;
            effectinfo.NextEffectTime = m_OwnSkill.CurTime / 1000.0f + m_DamageInterval / 1000.0f;
            m_MoreTimesEffectObjects.Add(effectinfo);
        }

        public void RemoveMoreTimeEffectObject(GameObject obj)
        {
            EffectObjectInfo ei = FindEffectInfoByObj(obj);
            if (ei != null)
            {
                m_MoreTimesEffectObjects.Remove(ei);
            }
        }

        public EffectObjectInfo FindEffectInfoByObj(GameObject obj)
        {
            foreach (EffectObjectInfo ei in m_MoreTimesEffectObjects)
            {
                if (ei.TargetObj == obj)
                {
                    return ei;
                }
            }
            return null;
        }

        private void AddAttachObject(Collider collider)
        {
            GameObject parent = m_ColliderInfo.GetCollider();
            if (parent == null)
            {
                return;
            }
            AttachConfig attach_config = m_ColliderInfo.GetAttachConfig();
            AttachTargetInfo attach_info = new AttachTargetInfo();
            attach_info.ParentObj = parent;
            attach_info.TargetObj = collider.gameObject;
            attach_info.AttachNode = TriggerUtil.GetChildNodeByName(collider.gameObject,
                                                                    attach_config.AttachNodeName);

            Vector3 hit_pos = parent.GetComponent<Collider>().ClosestPointOnBounds(attach_info.AttachNode.position);
            attach_info.ParentPos = attach_info.ParentObj.transform.InverseTransformPoint(hit_pos);
            attach_info.Rotate = attach_config.AttachRotation;
            attach_info.MoveControler = attach_info.TargetObj.GetComponent<CharacterController>();
            m_AttachedObjects.Add(attach_info);
            UpdateAttachTargetPos(attach_info);
            DashFire.LogicSystem.NotifyGfxMoveControlStart(attach_info.TargetObj, m_OwnSkill.SkillId, true);
            //Debug.Log("---AttachImpact: send " + attach_config.AttachImpact + " to " + attach_info.TargetObj.name);
            m_DamageManager.SendImpactToObject(m_DamageManager.GetOwner(),
                                               collider.gameObject, attach_config.AttachImpact,
                                               attach_config.AttachImpactTime,
                                               m_OwnSkill.SkillId);
        }

        private bool UpdateAttachTargetPos(AttachTargetInfo ati)
        {
            if (ati.AttachNode == null)
            {
                return false;
            }
            ati.TargetObj.transform.rotation = ati.ParentObj.transform.rotation;
            ati.TargetObj.transform.Rotate(ati.Rotate);
            Vector3 relative_motion = (ati.TargetObj.transform.position - ati.AttachNode.position);
            Vector3 target_pos = ati.ParentObj.transform.TransformPoint(ati.ParentPos) + relative_motion;
            CollisionFlags flag;
            if (ati.MoveControler != null)
            {
                flag = ati.MoveControler.Move(target_pos - ati.TargetObj.transform.position);
            }
            else
            {
                return false;
            }
            Vector3 cur_pos = ati.TargetObj.transform.position;
            //if (/*Math.Abs(cur_pos.x - target_pos.x) <= 0.5 && Math.Abs(cur_pos.z - target_pos.z) <= 0.5*/) {
            if ((flag & CollisionFlags.CollidedSides) <= 0)
            {
                ati.TargetObj.transform.position = target_pos;
                return true;
            }
            else
            {
                Debug.Log("----can't move to " + target_pos + "  just move to " + cur_pos);
                return false;
            }
        }

        private void ClearDamagedObjIfNeed()
        {
            if (m_IsClearWhenFinish)
            {
                foreach (GameObject damaged_obj in m_DamagedObjects)
                {
                    m_DamageManager.RemoveGameObject(damaged_obj);
                }
            }
            m_DamagedObjects.Clear();
        }

        private void SendFinalImpact()
        {
            AttachConfig attach_config = m_ColliderInfo.GetAttachConfig();
            for (int i = m_AttachedObjects.Count - 1; i >= 0; --i)
            {
                AttachTargetInfo ati = m_AttachedObjects[i];
                if (ati.ParentObj != null)
                {
                    //Debug.Log("---FinalImpact: send " + attach_config.FinalImpact + " to " + ati.TargetObj.name);
                    m_DamageManager.SendImpactToObject(m_DamageManager.GetOwner(),
                                                        ati.TargetObj, attach_config.FinalImpact,
                                                        attach_config.FinalImpactTime,
                                                        m_OwnSkill.SkillId);
                }
            }
        }

        private long m_RemainTime = 0;
        private bool m_IsClearWhenFinish = false;
        private bool m_IsAlwaysEnterDamage = false;
        private long m_DamageInterval = 0;
        private int m_MaxDamageTimes = 0;
        private ColliderTriggerInfo m_ColliderInfo = new ColliderTriggerInfo();

        private bool m_ColliderCreated = false;
        private SkillInstance m_OwnSkill = null;
        private GameObject m_Owner = null;
        private SkillDamageManager m_DamageManager = null;
        private List<EffectObjectInfo> m_MoreTimesEffectObjects = new List<EffectObjectInfo>();
        private List<AttachTargetInfo> m_AttachedObjects = new List<AttachTargetInfo>();
        private List<GameObject> m_LeaveDelObjects = new List<GameObject>();
        private List<GameObject> m_DamagedObjects = new List<GameObject>();
    }
}
