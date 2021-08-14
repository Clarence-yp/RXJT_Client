using UnityEngine;
using System;
using System.Collections;
using DashFire;

public class TouchCircle : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {
    OKAlpha = false;
    time = 0.0f;
    isUp = true;
    isSkillWant = false;

//     DashFire.LogicSystem.EventChannelForGfx.Subscribe<Vector2, SkillCategory, bool>("ge_ui_angle", "ui",
//       (Vector2 v, SkillCategory s, bool b) => { if (b) { SkillIsFalse(); } });
//     DashFire.LogicSystem.EventChannelForGfx.Subscribe<Vector2, bool>("ge_finger_event", "ui", FinentEvent);
//     DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_skill_false", "ui", SkillIsFalse);
//     DashFire.LogicSystem.EventChannelForGfx.Subscribe<bool>("ge_touch_dir", "ui", (bool want) => { isSkillWant = want; });
    UIManager.SkillDrectorColor = new Color(255, 255, 255);
    NGUITools.SetActive(gameObject.transform.parent.gameObject, false);
  }

  // Update is called once per frame
  void Update()
  {
    if (OKAlpha) {
      time += RealTime.deltaTime;
      int multiple = (int)System.Math.Round(time / 0.03f);
      switch (multiple) {
        case 0: SetAlpha(1.0f); break;
        case 1: SetAlpha(0.9f); break;
        case 2: SetAlpha(0.8f); break;
        case 3: SetAlpha(0.7f); break;
        case 4: SetAlpha(0.6f); break;
        case 5: SetAlpha(0.5f); break;
        case 6: SetAlpha(0.4f); break;
        case 7: SetAlpha(0.3f); break;
        case 8: SetAlpha(0.2f); break;
        case 9: SetAlpha(0.1f); break;
        case 10: SetAlpha(0.0f); break;
        default:
          NGUITools.SetActive(gameObject.transform.parent.gameObject, false);
          break;
      }

    }
  }
  void FinentEvent(Vector2 start, bool active)
  {
    try {
      if (active) {
        if (isUp || isSkillWant) {
          isUp = false;
          OKAlpha = false;
          time = 0.0f;
          UISprite us = this.gameObject.GetComponent<UISprite>();
          if (us != null) {
            UIManager.SkillDrectorColor = new Color(255, 255, 255);
            us.color = new Color(255, 255, 255);
          }
          Vector3 pos = UICamera.mainCamera.ScreenToWorldPoint(new Vector3(start.x, start.y, 0));
          this.transform.position = pos;
          NGUITools.SetActive(gameObject.transform.parent.gameObject, true);
        }
      } else {
        isUp = true;
        OKAlpha = true;
        time = 0.0f;
        //NGUITools.SetActive(gameObject, false);
      }
    }
    catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  void SkillIsFalse()
  {
    UISprite us = this.gameObject.GetComponent<UISprite>();
    if (us != null) {
      UIManager.SkillDrectorColor = new Color(255, 0, 0);
      us.color = new Color(255, 0, 0);
    }
  }
  void SetAlpha(float wantalpha)
  {
    UISprite us = this.gameObject.GetComponent<UISprite>();
    if (us != null) {
      us.alpha = wantalpha;
    }
  }
  private bool OKAlpha = false;
  private float time = 0.0f;

  private bool isUp = true;
  private bool isSkillWant = false;
}
