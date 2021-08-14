using UnityEngine;
using System.Collections;

public class ReXueJunTuan : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {
    livetime = -0.2f;
  }

  // Update is called once per frame
  void Update()
  {
    if (livetime < 1.5f) {
      livetime += RealTime.deltaTime;
      if (livetime < 0.0f) return;
      UISprite us = this.gameObject.GetComponent<UISprite>();
      if (us != null) {
        us.fillAmount = livetime/1.5f;
      }
    }
  }
  private float livetime = 0.0f;
}
