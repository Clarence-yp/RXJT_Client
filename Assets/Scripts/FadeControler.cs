using UnityEngine;
using System.Collections;

public class FadeControler : MonoBehaviour {
  //public float m_DarkPercent;
  public void SetBlackPercent(float dark_percent)
  {
    if (screen_bloom_ != null) {
      screen_bloom_.m_Color = Color.Lerp(Color.white, Color.black, dark_percent);
    }
  }

  /*void Update ()
  {
    if (m_LastDarkPercent != m_DarkPercent) {
      SetDarkPercent(m_DarkPercent);
      m_LastDarkPercent = m_DarkPercent;
    }
  }*/

  void Start()
  {
    screen_bloom_ = gameObject.GetComponent<ScreenBloom>();
  }

  private ScreenBloom screen_bloom_;
  //private float m_LastDarkPercent = 0;
}
