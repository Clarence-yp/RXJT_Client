using System;
using System.Collections.Generic;
using UnityEngine;
using SkillSystem;

namespace GfxModule.Skill.Trigers
{
  public class RotateCameraTrigger : AbstractSkillTriger
  {
    public override ISkillTriger Clone()
    {
      RotateCameraTrigger copy = new RotateCameraTrigger();
      copy.m_StartTime = m_StartTime;
      copy.m_RemainTime = m_RemainTime;
      copy.m_RotateSpeed = m_RotateSpeed;
      return copy;
    }

    public override void Reset()
    {
      m_IsInited = false;
      if (TriggerUtil.IsControledCamera(m_CameraControlId)) {
        TriggerUtil.ControlCamera(false, true);
      }
      m_CameraControlId = TriggerUtil.CAMERA_CONTROL_FAILED;
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      if (callData.GetParamNum() >= 3) {
        m_StartTime = long.Parse(callData.GetParamId(0));
        m_RemainTime = long.Parse(callData.GetParamId(1));
        m_RotateSpeed = ScriptableDataUtility.CalcVector3(callData.GetParam(2) as ScriptableData.CallData);
      }
    }

    public override bool Execute(object sender, SkillInstance instance, long delta, long curSectionTime)
    {
      if (curSectionTime < m_StartTime) {
        return true;
      }
      if (curSectionTime > (m_StartTime + m_RemainTime)) {
        return false;
      }
      GameObject obj = sender as GameObject;
      if (obj == null) {
        return false;
      }
      if (!TriggerUtil.IsPlayerSelf(obj)) {
        return false;
      }
      if (!m_IsInited) {
        if (!Init()) {
          return false;
        }
      }
      //m_CameraObj.SendMessage("SetUpPosition");
      if (Camera.main != null) {
        Camera.main.transform.Rotate(m_RotateSpeed * TriggerUtil.ConvertToSecond(delta));
      }
      return true;
    }

    private bool Init()
    {
      m_IsInited = true;
      m_CameraControlId = TriggerUtil.ControlCamera(true, true);
      if (m_CameraControlId < TriggerUtil.CAMERA_CONTROL_START_ID) {
        return false;
      }
      m_CameraObj = TriggerUtil.GetCameraObj();
      return true;
    }

    private long m_RemainTime;
    private Vector3 m_RotateSpeed;
    private bool m_IsInited = false;
    private GameObject m_CameraObj;
    private int m_CameraControlId = TriggerUtil.CAMERA_CONTROL_FAILED;
  }
}
