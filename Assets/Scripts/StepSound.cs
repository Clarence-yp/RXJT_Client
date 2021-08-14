using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class StepAnimInfo
{
  public AudioClip m_StepSound;
  public string m_AnimName;
  public float[] m_StepTimes;
}

public class StepSound : MonoBehaviour {
  public AudioSource m_AudioSource;
  public StepAnimInfo[] m_StepAnimInfos;

  public bool m_IsDebug = false;
  public int m_DebugIndex = 0;
  public float m_AnimSpeed = 1;

  private string m_DebugAnimName;
	// Use this for initialization
	void Start () {
    foreach(StepAnimInfo info in m_StepAnimInfos) {
      AnimationClip animclip = GetComponent<Animation>()[info.m_AnimName].clip;
      if (animclip == null) {
        continue;
      }
      foreach(float time in info.m_StepTimes) {
        AnimationEvent ae = new AnimationEvent();
        ae.time = time;
        ae.functionName = "PlayStepSound";
        ae.objectReferenceParameter = info.m_StepSound;
        animclip.AddEvent(ae);
      }
    }
    if (m_IsDebug && m_DebugIndex >= 0 && m_DebugIndex < m_StepAnimInfos.Length) {
      StepAnimInfo debuginfo = m_StepAnimInfos[m_DebugIndex];
      m_DebugAnimName = debuginfo.m_AnimName;
      GetComponent<Animation>()[debuginfo.m_AnimName].speed = m_AnimSpeed;
      GetComponent<Animation>().Play(debuginfo.m_AnimName);
    }
	}

  void Update()
  {
    if (m_IsDebug) {
      Debug.Log("anim-time=" + GetComponent<Animation>()[m_DebugAnimName].time);
    }
  }

  void PlayStepSound(AudioClip stepsound)
  {
    if (m_AudioSource != null) {
      m_AudioSource.PlayOneShot(stepsound);
    }
  }
}