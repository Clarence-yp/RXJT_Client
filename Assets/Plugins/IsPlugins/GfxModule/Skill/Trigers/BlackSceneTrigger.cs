using System;
using System.Collections.Generic;
using UnityEngine;
using SkillSystem;

namespace GfxModule.Skill.Trigers
{
  public class BlackSceneTrigger : AbstractSkillTriger
  {
    public override ISkillTriger Clone()
    {
      BlackSceneTrigger copy = new BlackSceneTrigger();
      copy.m_StartTime = m_StartTime;
      copy.m_CameraPerfab = m_CameraPerfab;
      copy.m_BlackPercent = m_BlackPercent;
      copy.m_FadeInTime = m_FadeInTime;
      copy.m_RemainTime = m_RemainTime;
      copy.m_FadeOutTime = m_FadeOutTime;
      copy.m_IgnoreLayers.AddRange(m_IgnoreLayers);
      return copy;
    }

    public override void Reset()
    {
      m_IsInited = false;
      SetBlackPercent(0);
      Camera.main.cullingMask = m_OldMask;
      Camera.main.clearFlags = CameraClearFlags.Skybox;
      DashFire.ResourceSystem.RecycleObject(m_BlackCameraObj);
      m_BlackCameraObj = null;
      m_BlackCamera = null;
    }

    protected override void Load(ScriptableData.CallData callData)
    {
      if (callData.GetParamNum() >= 6) {
        m_StartTime = long.Parse(callData.GetParamId(0));
        m_CameraPerfab = callData.GetParamId(1);
        m_BlackPercent = float.Parse(callData.GetParamId(2));
        m_FadeInTime = long.Parse(callData.GetParamId(3));
        m_RemainTime = long.Parse(callData.GetParamId(4));
        m_FadeOutTime = long.Parse(callData.GetParamId(5));
      }
      for (int i = 6; i < callData.GetParamNum(); ++i) {
        m_IgnoreLayers.Add(callData.GetParamId(i));
      }
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
      if (!m_IsInited) {
        if (!Init()) {
          return false;
        }
      }
      if (curSectionTime <= m_FadeInEndTime) {
        UpdateFadeIn(curSectionTime - m_StartTime);
      } else if (curSectionTime < m_RemainEndTime){
      } else if (curSectionTime < m_FadeOutEndTime) {
        UpdateFadeOut(curSectionTime - m_RemainEndTime);
      } else {
        Reset();
        return false;
      }
      CopyCameraProperty();
      return true;
    }

    private bool Init()
    {
      m_IsInited = true;
      m_FadeInEndTime = m_StartTime + m_FadeInTime;
      m_RemainEndTime = m_FadeInEndTime + m_RemainTime;
      m_FadeOutEndTime = m_RemainEndTime + m_FadeOutTime;
      m_BlackCameraObj = DashFire.ResourceSystem.NewObject(m_CameraPerfab) as GameObject;
      if (m_BlackCameraObj == null) {
        return false;
      }
      m_BlackCamera = m_BlackCameraObj.GetComponent<Camera>();
      if (m_BlackCamera == null) {
        return false;
      }
      m_BlackCamera.CopyFrom(Camera.main);
      m_BlackCamera.clearFlags = CameraClearFlags.Skybox;
      m_BlackCamera.depth = Camera.main.depth - 1;
      //m_BlackCamera.cullingMask = 0;
      m_OldMask = Camera.main.cullingMask;
      Camera.main.cullingMask = 0;
      Camera.main.clearFlags = CameraClearFlags.Nothing;
      foreach(string layer_str in m_IgnoreLayers) {
        int layer = LayerMask.NameToLayer(layer_str);
        int layer_mask = 1 << layer;
        m_BlackCamera.cullingMask = m_BlackCamera.cullingMask & (~layer_mask);
        Camera.main.cullingMask |= layer_mask;
      }
      if (m_FadeInTime == 0) {
        SetBlackPercent(1);
      } else {
        SetBlackPercent(0);
      }
      return true;
    }

    private void CopyCameraProperty()
    {
      m_BlackCamera.transform.position = Camera.main.transform.position;
      m_BlackCamera.transform.rotation = Camera.main.transform.rotation;
    }

    private void UpdateFadeIn(float passed_time)
    {
      if (m_FadeInTime != 0) {
        float dark_percent = passed_time / m_FadeInTime;
        SetBlackPercent(dark_percent);
      }
    }

    private void UpdateFadeOut(float passed_time)
    {
      if (m_FadeOutTime != 0) {
        float dark_percent = 1 - passed_time / m_FadeOutTime;
        SetBlackPercent(dark_percent);
      }
    }

    private void SetBlackPercent(float dark_percent)
    {
      if (m_BlackCameraObj != null) {
        m_BlackCameraObj.SendMessage("SetBlackPercent", dark_percent * m_BlackPercent);
      }
    }

    private string m_CameraPerfab;
    private float m_BlackPercent;
    private long m_FadeInTime;
    private long m_RemainTime;
    private long m_FadeOutTime;
    private List<string> m_IgnoreLayers = new List<string>();

    private GameObject m_BlackCameraObj;
    private Camera m_BlackCamera;
    private bool m_IsInited = false;
    private long m_FadeInEndTime;
    private long m_RemainEndTime;
    private long m_FadeOutEndTime;
    private int m_OldMask;
  }
}
