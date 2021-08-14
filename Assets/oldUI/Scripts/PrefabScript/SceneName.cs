using UnityEngine;
using System.Collections;

public class SceneName : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {
    time = -1.0f;
    SetAlpha(0.0f);
  }

  // Update is called once per frame
  void Update()
  {
    float dt = RealTime.deltaTime;
    if (dt < 0.08f) {
      time += dt;
    }
    if (time > 0.0f && time <= 0.6f) {
      SetAlpha(time / 0.6f);
    }
    if (time > 0.6f && time <= 2.3f) {
      SetAlpha(1.0f);
    }
    if (time > 2.3f && time <= 2.6) {
      SetAlpha(1.0f - (time - 2.3f) / 0.3f);
    }
    if (time > 2.6) {
      NGUITools.DestroyImmediate(gameObject);
    }
  }
  void SetAlpha(float alpha)
  {
      UIPanel us = gameObject.GetComponent<UIPanel>();
      if (us != null) {
        us.alpha = alpha;
      }
  }
  private float time = 0.0f;
}
