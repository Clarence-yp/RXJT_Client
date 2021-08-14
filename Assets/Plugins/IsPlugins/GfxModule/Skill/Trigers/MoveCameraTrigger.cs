using System;
using System.Collections.Generic;
using UnityEngine;
using SkillSystem;

namespace GfxModule.Skill.Trigers
{
  public class MoveCameraTrigger : AbstractSkillTriger
  {
    public override ISkillTriger Clone()
    {
      MoveCameraTrigger copy = new MoveCameraTrigger();
      copy.m_StartTime = m_StartTime;
      copy.m_IsLookAt = m_IsLookAt;
      copy.m_SectionList.AddRange(m_SectionList);
      return copy;
    }

    public override void Reset()
    {
      m_IsCurveMoving = true;
      m_IsInited = false;
      if (TriggerUtil.IsControledCamera(m_CameraControlId)) {
        TriggerUtil.ControlCamera(false, true);
      }
      m_CameraControlId = TriggerUtil.CAMERA_CONTROL_FAILED;
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      if (callData.GetParamNum() > 1) {
        m_StartTime = int.Parse(callData.GetParamId(0));
        m_IsLookAt = bool.Parse(callData.GetParamId(1));
      }
      m_SectionList.Clear();
      int section_num = 0;
      while (callData.GetParamNum() >= 7 * (section_num + 1) + 2) {
        MoveSectionInfo section = new MoveSectionInfo();
        section.moveTime = (float)System.Convert.ToDouble(callData.GetParamId((section_num * 7) + 2));
        section.speedVect.x = (float)System.Convert.ToDouble(callData.GetParamId((section_num * 7) + 3));
        section.speedVect.y = (float)System.Convert.ToDouble(callData.GetParamId((section_num * 7) + 4));
        section.speedVect.z = (float)System.Convert.ToDouble(callData.GetParamId((section_num * 7) + 5));
        section.accelVect.x = (float)System.Convert.ToDouble(callData.GetParamId((section_num * 7) + 6));
        section.accelVect.y = (float)System.Convert.ToDouble(callData.GetParamId((section_num * 7) + 7));
        section.accelVect.z = (float)System.Convert.ToDouble(callData.GetParamId((section_num * 7) + 8));
        m_SectionList.Add(section);
        section_num++;
      }
      if (m_SectionList.Count == 0) {
        return;
      }
      m_IsCurveMoving = true;
    }

    public override bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime)
    {
      if (curSectionTime < m_StartTime) {
        return true;
      }
      GameObject obj = sender as GameObject;
      if (obj == null) {
        return false;
      }
      if (!m_IsCurveMoving) {
        return false;
      }
      if (!TriggerUtil.IsPlayerSelf(obj)) {
        return false;
      }
      if (!m_IsInited) {
        if (!Init(instance)) {
          return false;
        }
      }
      if (m_SectionListCopy.Count == 0 || !instance.IsCurveMoveEnable) {
        m_IsCurveMoving = false;
        return false;
      }
      if (!TriggerUtil.IsControledCamera(m_CameraControlId)) {
        return false;
      }

      m_Now += TriggerUtil.ConvertToSecond((long)(instance.OrigDelta * instance.MoveScale));
      MoveSectionInfo cur_section = m_SectionListCopy[0];
      if (m_Now - cur_section.startTime > cur_section.moveTime) {
        float end_time = cur_section.startTime + cur_section.moveTime;
        float used_time = end_time - cur_section.lastUpdateTime;
        cur_section.curSpeedVect = Move(cur_section.curSpeedVect, cur_section.accelVect, used_time);
        m_SectionListCopy.RemoveAt(0);
        if (m_SectionListCopy.Count > 0) {
          cur_section = m_SectionListCopy[0];
          cur_section.startTime = end_time;
          cur_section.lastUpdateTime = end_time;
          cur_section.curSpeedVect = cur_section.speedVect;
        } else {
          m_IsCurveMoving = false;
        }
      } else {
        cur_section.curSpeedVect = Move(cur_section.curSpeedVect, cur_section.accelVect, m_Now - cur_section.lastUpdateTime);
        cur_section.lastUpdateTime = m_Now;
      }
      if (m_IsLookAt) {
        m_CameraObj.SendMessage("LookAtTarget");
      }
      return true;
    }

    private bool Init(SkillInstance instance)
    {
      m_CameraControlId = TriggerUtil.ControlCamera(true, true);
      if (m_CameraControlId < TriggerUtil.CAMERA_CONTROL_START_ID) {
        return false;
      }
      m_CameraObj = TriggerUtil.GetCameraObj();
      CopySectionList();
      m_Now = instance.CurTime / 1000.0f;
      m_SectionListCopy[0].startTime = m_Now;
      m_SectionListCopy[0].lastUpdateTime = m_Now;
      m_SectionListCopy[0].curSpeedVect = m_SectionListCopy[0].speedVect;
      m_IsInited = true;
      return true;
    }

    private void CopySectionList()
    {
      m_SectionListCopy.Clear();
      foreach (MoveSectionInfo sect in m_SectionList) {
        m_SectionListCopy.Add(sect.Clone());
      }
    }

    private Vector3 Move(Vector3 speed_vect, Vector3 accel_vect, float time)
    {
      Vector3 local_motion = speed_vect * time + accel_vect * time * time / 2;
      Camera.main.transform.position += local_motion;
      return (speed_vect + accel_vect * time);
    }

    private bool m_IsLookAt = true;
    private List<MoveSectionInfo> m_SectionList = new List<MoveSectionInfo>();
    private List<MoveSectionInfo> m_SectionListCopy = new List<MoveSectionInfo>();
    private bool m_IsCurveMoving = true;

    private GameObject m_CameraObj;
    private bool m_IsInited = false;
    private float m_Now;
    private int m_CameraControlId = TriggerUtil.CAMERA_CONTROL_FAILED;
  }
}
