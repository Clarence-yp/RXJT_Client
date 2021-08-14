using System;
using System.Collections.Generic;
using UnityEngine;
using SkillSystem;

namespace GfxModule.Skill.Trigers
{
  /// <summary>
  /// lockframe(startime, anim_name, is_need_collide, lock_speed, locktime, after_lock_speed, restore_time);
  /// </summary>
  internal class LockFrameTriger : AbstractSkillTriger
  {
    public override ISkillTriger Clone()
    {
      LockFrameTriger triger = new LockFrameTriger();
      triger.m_StartTime = m_StartTime;
      triger.m_AnimName = m_AnimName;
      triger.m_IsNeedCollide = m_IsNeedCollide;
      triger.m_LockSpeed = m_LockSpeed;
      triger.m_LockTime = m_LockTime;
      triger.m_AfterLockAnimSpeed = m_AfterLockAnimSpeed;
      triger.m_AfterLockEffectSpeed = m_AfterLockEffectSpeed;
      triger.m_AfterLockMoveSpeed = m_AfterLockMoveSpeed;
      triger.m_AfterLockSkillSpeed = m_AfterLockSkillSpeed;
      triger.m_RestoreTime = m_RestoreTime;
      triger.m_IsEffectSkillTime = m_IsEffectSkillTime;
      triger.m_Curve = m_Curve;
      return triger;
    }

    public override void Reset()
    {
      m_IsInited = false;
    }

    public override bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime)
    {
      if (curSectionTime < m_StartTime) {
        return true;
      }
      GameObject obj = sender as GameObject;
      if (null == obj || null == obj.GetComponent<Animation>()) {
        return false;
      }
      Animation animation = obj.GetComponent<Animation>();
      AnimationState state = animation[m_AnimName];
      if (state == null) {
        return false;
      }

      if (!m_IsInited) {
        Init();
      }

      if (m_IsNeedCollide) {
        SkillDamageManager damage_manager = instance.CustomDatas.GetData<SkillDamageManager>();
        if (damage_manager == null || !damage_manager.IsDamagedEnemy) {
          return false;
        }
      }
      if (Time.time <= m_WorldStartTime + m_LockTime / 1000.0f) {
        if (state.speed != m_LockSpeed) {
          state.speed = m_LockSpeed;
          SetSkillTimeScale(instance, state.speed, state.speed, state.speed);
        }
      } else {
        if (m_RestoreTime > 0 && Time.time < m_WorldStartTime + (m_LockTime + m_RestoreTime) / 1000.0f) {
          float time_percent = (Time.time - m_WorldStartTime - m_LockTime / 1000.0f) / (m_RestoreTime / 1000.0f);
          if (m_Curve != null && m_Curve.keys.Length > 0) {
            float scale = m_Curve.Evaluate(time_percent);
            float anim_speed = scale * m_AfterLockAnimSpeed;
            float effect_speed = scale * m_AfterLockEffectSpeed;
            float move_speed = scale * m_AfterLockMoveSpeed;
            float skill_speed = scale * m_AfterLockSkillSpeed;
            state.speed = anim_speed;
            SetSkillTimeScale(instance, effect_speed, move_speed, skill_speed);
          } else {
            float anim_speed = m_LockSpeed + time_percent * (m_AfterLockAnimSpeed - m_LockSpeed);
            float effect_speed = m_LockSpeed + time_percent * (m_AfterLockEffectSpeed - m_LockSpeed);
            float move_speed = m_LockSpeed + time_percent * (m_AfterLockMoveSpeed - m_LockSpeed);
            float skill_speed = m_LockSpeed + time_percent * (m_AfterLockSkillSpeed - m_LockSpeed);
            SetSkillTimeScale(instance, effect_speed, move_speed, skill_speed);
          }
        } else {
          state.speed = m_AfterLockAnimSpeed;
          SetSkillTimeScale(instance, m_AfterLockEffectSpeed, m_AfterLockMoveSpeed, m_AfterLockSkillSpeed);
          return false;
        }
      }
      return true;
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      int num = callData.GetParamNum();
      int index = 0;
      if (num >= 7) {
        m_StartTime = long.Parse(callData.GetParamId(index++));
        m_AnimName = callData.GetParamId(index++);
        m_IsNeedCollide = bool.Parse(callData.GetParamId(index++));
        m_LockSpeed = float.Parse(callData.GetParamId(index++));
        m_LockTime = long.Parse(callData.GetParamId(index++));
        m_AfterLockAnimSpeed = float.Parse(callData.GetParamId(index++));
        m_RestoreTime = long.Parse(callData.GetParamId(index++));
      }
      if (num >= 11) {
        m_IsEffectSkillTime = bool.Parse(callData.GetParamId(index++));
        m_AfterLockEffectSpeed = float.Parse(callData.GetParamId(index++));
        m_AfterLockMoveSpeed = float.Parse(callData.GetParamId(index++));
        m_AfterLockSkillSpeed = float.Parse(callData.GetParamId(index++));
      }
    }

    protected override void Load(ScriptableData.FunctionData funcData)
    {
      ScriptableData.CallData callData = funcData.Call;
      if (null == callData) {
        return;
      }
      int num = callData.GetParamNum();
      int index = 0;
      if (num >= 7) {
        m_StartTime = long.Parse(callData.GetParamId(index++));
        m_AnimName = callData.GetParamId(index++);
        m_IsNeedCollide = bool.Parse(callData.GetParamId(index++));
        m_LockSpeed = float.Parse(callData.GetParamId(index++));
        m_LockTime = long.Parse(callData.GetParamId(index++));
        m_AfterLockAnimSpeed = float.Parse(callData.GetParamId(index++));
        m_RestoreTime = long.Parse(callData.GetParamId(index++));
      }
      if (num >= 11) {
        m_IsEffectSkillTime = bool.Parse(callData.GetParamId(index++));
        m_AfterLockEffectSpeed = float.Parse(callData.GetParamId(index++));
        m_AfterLockMoveSpeed = float.Parse(callData.GetParamId(index++));
        m_AfterLockSkillSpeed = float.Parse(callData.GetParamId(index++));
      }
      LoadKeyFrames(funcData.Statements);
    }

    private void LoadKeyFrames(List<ScriptableData.ISyntaxComponent> statements)
    {
      m_Curve = new AnimationCurve();
      foreach (ScriptableData.ISyntaxComponent statement in statements) {
        ScriptableData.CallData stCall = statement as ScriptableData.CallData;
        if (stCall.GetId() == "keyframe") {
          if (stCall.GetParamNum() >= 4) {
            float time = float.Parse(stCall.GetParamId(0));
            float value = float.Parse(stCall.GetParamId(1));
            float inTangent = float.Parse(stCall.GetParamId(2));
            float outTangent = float.Parse(stCall.GetParamId(3));
            Keyframe keyframe = new Keyframe(time, value, inTangent, outTangent);
            m_Curve.AddKey(keyframe);
          }
        }
      }
    }

    private void SetSkillTimeScale(SkillInstance instance, float effect_speed, float move_speed, float skill_speed)
    {
      if (m_IsEffectSkillTime) {
        instance.TimeScale = skill_speed;
        instance.EffectScale = effect_speed;
        instance.MoveScale = move_speed;
        EffectManager em = instance.CustomDatas.GetData<EffectManager>();
        if (em != null) {
          em.SetParticleSpeed(effect_speed);
        }
      }
    }

    private void Init()
    {
      m_WorldStartTime = Time.time;
      m_IsInited = true;
    }

    private string m_AnimName = "";
    private bool m_IsNeedCollide = false;
    private float m_LockSpeed = 0;
    private long m_LockTime = 0;
    private float m_AfterLockAnimSpeed = 1;
    private float m_AfterLockEffectSpeed = 1;
    private float m_AfterLockMoveSpeed = 1;
    private float m_AfterLockSkillSpeed = 1;
    private long m_RestoreTime = 0;
    private bool m_IsEffectSkillTime = false;

    private bool m_IsInited = false;
    private float m_WorldStartTime = 0;
    private AnimationCurve m_Curve;
  }
}
