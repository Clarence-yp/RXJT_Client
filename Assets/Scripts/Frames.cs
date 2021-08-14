using UnityEngine;
using System.Collections;

public class Frames : MonoBehaviour
{
  public UnityEngine.UI.Text m_Target;
  public float m_UpdateInterval = 0.5f;

	// Use this for initialization
  internal void Start()
  {
    if (!m_Target) {
      enabled = false;
    } else {
      m_TimeLeft = m_UpdateInterval;
    }
	}
	
	// Update is called once per frame
	internal void Update ()
  {
    m_TimeLeft -= Time.deltaTime;
    m_Accum += Time.timeScale / Time.deltaTime;
    ++m_Frames;

    if (m_TimeLeft <= 0) {
      m_Target.text = "" + (m_Accum / m_Frames);
      m_TimeLeft = m_UpdateInterval;
      m_Accum = 0;
      m_Frames = 0;
    }
	}

  private float m_Accum = 0;
  private float m_Frames = 0;
  private float m_TimeLeft = 0;
}
