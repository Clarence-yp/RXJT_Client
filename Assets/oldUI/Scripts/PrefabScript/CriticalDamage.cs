using UnityEngine;
using System.Collections;

public class CriticalDamage : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {
    livetime = 0.0f;
  }

  // Update is called once per frame
  void Update()
  {
    livetime += RealTime.deltaTime;
    float deltatime = 0.015f;

    int multiple = (int)System.Math.Round(livetime / deltatime);
    switch (multiple) {
      case 0: break;
      case 1: Zoom(1.5f); break;
      case 2: Zoom(2.0f); break;
      case 3: break;
      case 4: Zoom(1.8f); break;
      case 5: break;
      case 6: Zoom(1.6f); break;
      case 7: break;
      case 8: break;
      case 9: Zoom(1.65f); break;
      case 10: break;
      case 11: break;
      case 12: Zoom(1.7f); break;
      case 13: Zoom(1.75f); break;
      case 14: break;
      case 15: Zoom(1.8f); break;
      case 16: break;
      case 17: break;
      case 18: Zoom(1.9f); break;
      case 19: break;
      case 20: Zoom(1.7f); break;
      case 21: break;
      case 22: Zoom(1.5f); break;
      case 23: break;//
      case 24: Zoom(1.4f); break;
      case 25: Zoom(1.3f); break;
      case 26: Zoom(1.2f); break;
      case 27: Zoom(1.1f); break;
      case 28: Zoom(1.0f); break;
      case 29: break;
      case 30:
        PositionRise(50);
        SetAlpha(0.9f);
        break;
      case 31: break;
      case 32:
        PositionRise(100);
        SetAlpha(0.8f);
        break;
      case 33: break;
      case 34:
        PositionRise(150);
        SetAlpha(0.7f);
        break;
      case 35: break;
      case 36:
        PositionRise(200);
        SetAlpha(0.6f);
        break;
      case 37: break;
      case 38:
        PositionRise(250);
        SetAlpha(0.5f);
        break;
      case 39: break;
      case 40:
        PositionRise(300);
        SetAlpha(0.4f);
        break;
      case 41: break;
      case 42:
        PositionRise(350);
        SetAlpha(0.3f);
        break;
      case 43: break;
      case 44:
        PositionRise(400);
        SetAlpha(0.2f);
        break;
      case 45: break;
      case 46:
        PositionRise(450);
        SetAlpha(0.1f);
        break;
      case 47: break;
      case 48:
        PositionRise(500);
        SetAlpha(0.0f);
        break;
      default: break;
    }
    if (multiple > 48) {
      DashFire.ResourceSystem.RecycleObject(gameObject);
      livetime = 0.0f;
      PositionRise(0);
      Zoom(1.0f);
      SetAlpha(1.0f);
    }
  }
  void Zoom(float wantscale)
  {
    if (wantscale == nowscale) return;
    float scale = wantscale / nowscale;
    foreach (string widget in widgets) {
      Transform tf = this.gameObject.transform.Find(widget);
      if (tf != null) {
        {
          UISprite us = tf.gameObject.GetComponent<UISprite>();
          if (us != null) {
            Vector3 lp = us.transform.localPosition;
            us.transform.localPosition = new Vector3(lp.x * scale, lp.y * scale, lp.z);

            us.width = (int)System.Math.Round(us.width * scale);
            us.height = (int)System.Math.Round(us.height * scale);
          }
        }
      }
    }
    nowscale = wantscale;
  }
  void PositionRise(int rise)
  {
    int needrise = rise - alreadyrise;

    Vector3 nowpos = this.transform.localPosition;
    this.transform.localPosition = new Vector3(nowpos.x, nowpos.y + needrise, nowpos.z);

    alreadyrise = rise;
  }
  void SetAlpha(float alpha)
  {
    foreach (string widget in widgets) {
      Transform tf = this.gameObject.transform.Find(widget);
      if (tf != null) {
        UISprite us = tf.gameObject.GetComponent<UISprite>();
        if (us != null) {
          us.alpha = alpha;
        }
      }
    }
  }
  private float livetime = 0.0f;
  private string[] widgets = new string[]{
      "baoji",
      "Number0",
      "Number1",
      "Number2",
      "Number3",
      "Number4",
      "Number5",
    };
  private float nowscale = 1.0f;
  private int alreadyrise = 0;
}
