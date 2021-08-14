using System;
using System.Collections.Generic;
using UnityEngine;
using SkillSystem;

namespace GfxModule.Skill.Trigers
{
  public class OnInputTrigger : AbstractSkillTriger
  {
    public override ISkillTriger Clone()
    {
      OnInputTrigger copy = new OnInputTrigger();
      copy.m_StartTime = m_StartTime;
      copy.m_RemainTime = m_RemainTime;
      copy.m_TriggerInterval = m_TriggerInterval;
      copy.m_MaxCount = m_MaxCount;
      copy.m_RequireCount = m_RequireCount;
      copy.m_Message = m_Message;
      copy.m_RequireMessage = m_RequireMessage;
      return copy;
    }

    public override void Reset()
    {
      m_CurCount = 0;
      m_NextTriggerTime = 0;
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      if (callData.GetParamNum() >= 7) {
        m_StartTime = long.Parse(callData.GetParamId(0));
        m_RemainTime = long.Parse(callData.GetParamId(1));
        m_TriggerInterval = long.Parse(callData.GetParamId(2));
        m_MaxCount = int.Parse(callData.GetParamId(3));
        m_RequireCount = int.Parse(callData.GetParamId(4));
        m_Message = callData.GetParamId(5);
        m_RequireMessage = callData.GetParamId(6);
      }
    }

    public override bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime)
    {
      if (curSectionTime < m_StartTime) {
        return true;
      }
      if (curSectionTime >= m_StartTime + m_RemainTime) {
        SupplementCount(instance);
        return false;
      }
      GameObject obj = sender as GameObject;
      if (obj == null) {
        return false;
      }
      if (!TriggerUtil.IsTouching(obj)) {
        return true;
      }
      if (curSectionTime >= m_NextTriggerTime) {
        m_NextTriggerTime = curSectionTime + m_TriggerInterval;
        m_CurCount++;
        StoreTouchPos(obj, instance);
        instance.SendMessage(m_Message);
        if (m_CurCount >= m_MaxCount) {
          return false;
        }
      }
      return true;
    }

    private void SupplementCount(SkillInstance instance)
    {
      for (int i = 0; i < m_RequireCount - m_CurCount; ++i) {
        instance.SendMessage(m_RequireMessage);
      }
    }

    private void StoreTouchPos(GameObject obj, SkillInstance instance)
    {
      Vector3 touchpos = TriggerUtil.GetTouchPos(obj);
      Vector3 scene_pos = CalcScenePos(obj, touchpos);
      DashFire.SharedGameObjectInfo obj_info = DashFire.LogicSystem.GetSharedGameObjectInfo(obj);
      if (obj_info != null) {
        obj_info.LastTouchX = scene_pos.x;
        obj_info.LastTouchY = scene_pos.y;
        obj_info.LastTouchZ = scene_pos.z;
      }
    }

    private Vector3 CalcScenePos(GameObject obj, Vector3 touch_pos)
    {
      Vector3 obj_pos = obj.transform.position;
      Vector3 end_pos = touch_pos;
      end_pos.y += 3;
      obj_pos.y = end_pos.y;
      RaycastHit hitinfo;
      int layer = 1 << LayerMask.NameToLayer("AirWall");
      if (Physics.Linecast(obj_pos, end_pos, out hitinfo, layer)) {
        return TriggerUtil.GetGroundPos(hitinfo.point);
      }
      return touch_pos;
    }

    private long m_RemainTime;
    private long m_TriggerInterval;
    private int m_MaxCount;
    private int m_RequireCount;
    private string m_Message;
    private string m_RequireMessage;

    private int m_CurCount = 0;
    private long m_NextTriggerTime = 0;
  }
}
