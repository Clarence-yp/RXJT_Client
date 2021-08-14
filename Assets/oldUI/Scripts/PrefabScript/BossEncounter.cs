using UnityEngine;
using System.Collections;

public class BossEncounter : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {
    time = 0.0f;
    nowscale = 1.0f;
  }

  // Update is called once per frame
  void Update()
  {
    time += RealTime.deltaTime;
    int multiple = (int)System.Math.Round(time / 0.03f);
    switch (multiple) {
      case 0: Zoom(2.5f); break;
      case 1: Zoom(2.35f); break;
      case 2: Zoom(2.2f); break;
      case 3: Zoom(2.05f); break;
      case 4: Zoom(1.9f); break;
      case 5: Zoom(1.75f); break;
      case 6: Zoom(1.6f); break;
      case 7: Zoom(1.45f); break;
      case 8: Zoom(1.3f); break;
      case 9: Zoom(1.15f); break;
      case 10: Zoom(1.0f); break;
      case 11: Zoom(1.0f); PositionShake(new Vector2(-10, 10)); break;
      case 12: Zoom(1.0f); PositionShake(new Vector2(0, 0)); break;
      case 13: Zoom(1.0f); PositionShake(new Vector2(10, 10)); break;
      case 14: Zoom(1.0f); PositionShake(new Vector2(0, 0)); break;
      case 15: Zoom(1.0f); PositionShake(new Vector2(10, -10)); break;
      case 16: Zoom(1.0f); PositionShake(new Vector2(0, 0)); break;
      case 17: Zoom(1.0f); PositionShake(new Vector2(-10, -10)); break;
      case 18: Zoom(1.0f); PositionShake(new Vector2(0, 0)); break;
      case 19: Zoom(0.95f); PositionShake(new Vector2(0, 0)); break;
      case 20: Zoom(0.9f); PositionShake(new Vector2(0, 0)); break;
      case 21: Zoom(1.0f); PositionShake(new Vector2(0, 0)); break;
      case 22: Zoom(2.5f); PositionShake(new Vector2(0, 0)); SetAlpha(0.2f); break;
      case 23: Zoom(2.5f); PositionShake(new Vector2(0, 0)); SetAlpha(0.2f); break;
      case 24: Zoom(2.5f); PositionShake(new Vector2(0, 0)); SetAlpha(0.2f); break;
      default:
        NGUITools.Destroy(gameObject);
        break;
    }
  }
  void Zoom(float wantscale)
  {
    if (wantscale == nowscale) return;
    float scale = wantscale / nowscale;
    Transform tf = this.gameObject.transform.Find("Sprite");
    if (tf != null) {
      UISprite us = tf.gameObject.GetComponent<UISprite>();
      if (us != null) {
        us.width = (int)System.Math.Round(us.width * scale);
        us.height = (int)System.Math.Round(us.height * scale);
      }
    }
    nowscale = wantscale;
  }
  void SetAlpha(float alpha)
  {
    Transform tf = this.gameObject.transform.Find("Sprite");
    if (tf != null) {
      UISprite us = tf.gameObject.GetComponent<UISprite>();
      if (us != null) {
        us.alpha = alpha;
      }
    }
  }
  void PositionShake(Vector2 wantshake)
  {
    Vector2 shake = wantshake - alreadyshake;

    Vector3 nowpos = this.transform.localPosition;
    this.transform.localPosition = new Vector3(nowpos.x + shake.x, nowpos.y + shake.y, nowpos.z);

    alreadyshake = wantshake;
  }
  private float nowscale = 1.0f;
  private float time = 0.0f;
  private Vector2 alreadyshake = new Vector2(0f, 0f);
}
