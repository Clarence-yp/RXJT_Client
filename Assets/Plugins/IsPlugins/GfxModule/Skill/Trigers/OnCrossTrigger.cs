using System;
using System.Collections.Generic;
using UnityEngine;
using SkillSystem;

namespace GfxModule.Skill.Trigers
{
  public class OnCrossTrigger : AbstractSkillTriger
  {
    public override ISkillTriger Clone()
    {
      OnCrossTrigger copy = new OnCrossTrigger();
      copy.m_StartTime = m_StartTime;
      copy.m_RemainTime = m_RemainTime;
      copy.m_DamageInterval = m_DamageInterval;
      foreach(StateImpact si in m_StateImpacts.Values) {
        copy.m_StateImpacts.Add(si.m_State, si);
      }
      copy.m_MessageLoopMode = m_MessageLoopMode;
      copy.m_MessageConfig.AddRange(m_MessageConfig);
      copy.m_IsNeedDamage = m_IsNeedDamage;
      return copy;
    }

    public override void Reset()
    {
      m_IsInited = false;
      DashFire.LogicSystem.EventChannelForGfx.Publish("ex_skill_end", "skill");
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      if (callData.GetParamNum() >= 1) {
        m_StartTime = long.Parse(callData.GetParamId(0));
        m_RemainTime = long.Parse(callData.GetParamId(1));
        m_DamageInterval = long.Parse(callData.GetParamId(2));
      }
    }

    protected override void Load(ScriptableData.FunctionData funcData)
    {
      ScriptableData.CallData callData = funcData.Call;
      if (null != callData) {
        Load(callData);

        foreach (ScriptableData.ISyntaxComponent statement in funcData.Statements) {
          ScriptableData.CallData stCall = statement as ScriptableData.CallData;
          if (null == stCall || stCall.GetParamNum() <= 0) {
            continue;
          }
          string id = stCall.GetId();
          if (id == "stateimpact") {
            LoadStateImpactConfig(stCall);
          }
          if (id == "message") {
            LoadMessageConfig(stCall);
          }
        }
      }
    }

    public override bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime)
    {
      if (curSectionTime < m_StartTime) {
        return true;
      }
      if (curSectionTime > m_StartTime + m_RemainTime) {
        return false;
      }
      GameObject obj = sender as GameObject;
      if (obj == null) {
        return false;
      }
      if (!m_IsInited) {
        Init(obj);
      }
      if (!DashFire.GfxSystem.IsTouchPosChanged()) {
        return true;
      }
      Vector3 pos = GetTouchPos();
      Vector3 lastpos = GetLastPos();

      UpdateDamgedObjectCD(delta);
      bool is_damaged = DamageCrossObjects(obj, instance, pos);

      if (m_IsHaveMessage && (!m_IsNeedDamage || (m_IsNeedDamage && is_damaged))) {
        if (curSectionTime >= m_NextMessageTime) {
          MessageConfig mc = m_MessageConfig[m_CurMessageIndex];
          instance.SendMessage(mc.Message);
          m_NextMessageTime = curSectionTime + mc.NextMessageInterval;
          m_CurMessageIndex = m_CurMessageIndex + 1 >= m_MessageConfig.Count ? 0 : m_CurMessageIndex + 1;
        }
      }
      m_LastTouchPos = pos;
      return true;
    }

    private bool DamageCrossObjects(GameObject obj, SkillInstance instance, Vector3 pos)
    {
      bool is_damaged_somebody = false;
      SkillDamageManager damage_manager = instance.CustomDatas.GetData<SkillDamageManager>();
      if (damage_manager == null) {
        damage_manager = new SkillDamageManager(obj);
        instance.CustomDatas.AddData<SkillDamageManager>(damage_manager);
      }
      List<GameObject> hit_objes = TriggerUtil.GetRayHitObjects("Character", pos);
      List<GameObject> enimies = TriggerUtil.FiltEnimy(obj, hit_objes);
      int final_skill_id = -1;
      GameObject source_obj = TriggerUtil.GetFinalOwner(obj, final_skill_id, out final_skill_id);
      foreach (GameObject hit_obj in enimies) {
        if (!SkillDamageManager.IsEnemy(obj, hit_obj.gameObject)) {
          continue;
        }
        if (IsObjectCanDamage(hit_obj)) {
          damage_manager.SendImpactToObject(source_obj, hit_obj, m_StateImpacts, final_skill_id);
          DamageObjectInfo a = new DamageObjectInfo();
          a.obj = hit_obj;
          a.remainDamageCD = m_DamageInterval / 1000.0f;
          m_DamageCDObjects.Add(a);
          is_damaged_somebody = true;
        }
      }
      if (is_damaged_somebody) {
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
      if (m_LastTouchPos == Vector3.zero) {
        return GetTouchPos();
      }
      return m_LastTouchPos;
    }

    private void UpdateDamgedObjectCD(long delta)
    {
      float seconds = TriggerUtil.ConvertToSecond(delta);
      for (int i = m_DamageCDObjects.Count - 1; i >= 0; i--) {
        DamageObjectInfo info = m_DamageCDObjects[i];
        info.remainDamageCD -= seconds;
        if (info.remainDamageCD < 0) {
          m_DamageCDObjects.RemoveAt(i);
        }
      }
    }

    private void Init(GameObject obj)
    {
      m_IsInited = true;
      m_LastTouchPos = Vector3.zero;
      DashFire.LogicSystem.EventChannelForGfx.Publish("ex_skill_start", "skill");
      InitMessageConfig();
    }

    private void InitMessageConfig()
    {
      m_CurMessageIndex = 0;
      if (m_MessageConfig.Count > 0) {
        m_NextMessageTime = 0;
        m_IsHaveMessage = true;
      } else {
        m_IsHaveMessage = false;
      }
    }

    private void LoadStateImpactConfig(ScriptableData.CallData stCall)
    {
      StateImpact stateimpact = TriggerUtil.ParseStateImpact(stCall);
      m_StateImpacts[stateimpact.m_State] = stateimpact;
    }

    private void LoadMessageConfig(ScriptableData.CallData stCall)
    {
      if (stCall.GetParamNum() >= 4) {
        m_MessageLoopMode = stCall.GetParamId(0);
        m_IsNeedDamage = bool.Parse(stCall.GetParamId(1));
        for (int i = 2; i + 1 < stCall.GetParamNum(); i+=2) {
          MessageConfig mc = new MessageConfig();
          mc.Message = stCall.GetParamId(i);
          mc.NextMessageInterval = long.Parse(stCall.GetParamId(i+1));
          m_MessageConfig.Add(mc);
        }
      }
    }

    private bool IsObjectCanDamage(GameObject target)
    {
      foreach (DamageObjectInfo dam_info in m_DamageCDObjects) {
        if (target == dam_info.obj) {
          return false;
        }
      }
      return true;
    }

    private long m_RemainTime;
    private long m_DamageInterval;
    private Dictionary<BeHitState, StateImpact> m_StateImpacts = new Dictionary<BeHitState, StateImpact>();
    private string m_MessageLoopMode;
    private bool m_IsNeedDamage = false;
    private List<MessageConfig> m_MessageConfig = new List<MessageConfig>();

    private bool m_IsInited = false;
    private Vector3 m_LastTouchPos;
    private List<DamageObjectInfo> m_DamageCDObjects = new List<DamageObjectInfo>();
    private bool m_IsHaveMessage = false;
    private long m_NextMessageTime = 0;
    private int m_CurMessageIndex = 0;
  }

  public class MessageConfig
  {
    public string Message;
    public long NextMessageInterval;
  }
}
