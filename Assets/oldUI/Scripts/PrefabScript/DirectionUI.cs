using UnityEngine;
using System.Collections;
using DashFire;

public class DirectionUI : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {
    time = 0.0f;
    nowscale = 1.0f;
    isSkillWant = false;
    isHaveSkill = false;
    lastStart = new Vector2();
    lastSkill = SkillCategory.kSkillA;
//     DashFire.LogicSystem.EventChannelForGfx.Subscribe<Vector2, SkillCategory, bool>("ge_ui_angle", "ui", DirectionAngle);
//     DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_skill_false", "ui", SkillIsFalse);
//     DashFire.LogicSystem.EventChannelForGfx.Subscribe<bool>("ge_touch_dir", "ui", (bool want) => { isSkillWant = want; });
    NGUITools.SetActive(gameObject.transform.parent.gameObject, false);
  }

  // Update is called once per frame
  void Update()
  {
    time += RealTime.deltaTime;
    int multiple = (int)System.Math.Round(time / 0.03f);
    switch (multiple) {
      case 0: Zoom(1.4f, 1.0f); break;
      case 1: Zoom(1.8f, 1.0f); break;
      case 2: Zoom(1.4f, 1.0f); break;
      case 3: Zoom(1.2f, 1.0f); break;
      case 4: Zoom(1.0f, 1.0f); break;
      case 5: Zoom(1.0f, 0.9f); break;
      case 6: Zoom(1.0f, 0.8f); break;
      case 7: Zoom(1.0f, 0.7f); break;
      case 8: Zoom(1.0f, 0.6f); break;
      case 9: Zoom(1.0f, 0.5f); break;
      case 10: Zoom(1.0f, 0.4f); break;
      case 11: Zoom(1.0f, 0.3f); break;
      case 12: Zoom(1.0f, 0.2f); break;
      case 13: Zoom(1.0f, 0.1f); break;
      case 14: Zoom(1.0f, 0.0f); break;
      default: break;
    }
    if (time > 0.42f) {
      Zoom(1.0f, 0.0f);
      NGUITools.SetActive(gameObject.transform.parent.gameObject, false);
    }
  }
  void DirectionAngle(Vector2 start, SkillCategory direct, bool isInSkillCD)
  {
    if ((lastSkill == SkillCategory.kSkillE || lastSkill == SkillCategory.kSkillQ)
      && (direct == SkillCategory.kSkillE || direct == SkillCategory.kSkillQ)) {
      return;
    }
    if (isSkillWant) return;
    if (isInSkillCD) {
      SkillIsFalse();
    }
    UISprite usc = this.gameObject.GetComponent<UISprite>();
    if (usc != null) {
      usc.color = UIManager.SkillDrectorColor;
    }
    if (direct != SkillCategory.kSkillE && direct != SkillCategory.kSkillQ) {
      lastStart = start;
    }
    Vector3 pos = UICamera.mainCamera.ScreenToWorldPoint(new Vector3(lastStart.x, lastStart.y, 0));
    this.transform.parent.transform.position = pos;

    Quaternion qua = new Quaternion();
    switch (direct) {
      case SkillCategory.kSkillD:
        isHaveSkill = true;
        ActiveDirectionUI(false);
        lastSkill = SkillCategory.kSkillD;
        qua = new Quaternion(0, 0, 0.7f, 0.7f);
        break;
      case SkillCategory.kSkillB:
        isHaveSkill = true;
        ActiveDirectionUI(false);
        lastSkill = SkillCategory.kSkillB;
        qua = new Quaternion(0, 0, 0.7f, -0.7f);
        break;
      case SkillCategory.kSkillC:
        isHaveSkill = true;
        ActiveDirectionUI(false);
        lastSkill = SkillCategory.kSkillC;
        qua = new Quaternion(0, 0, 1.0f, 0f);
        break;
      case SkillCategory.kSkillA:
        isHaveSkill = true;
        ActiveDirectionUI(false);
        lastSkill = SkillCategory.kSkillA;
        qua = new Quaternion(0, 0, 0.0f, 1.0f);
        break;
      case SkillCategory.kSkillQ:
        qua = Direction(SkillCategory.kSkillQ);
        lastSkill = SkillCategory.kSkillQ;
        ActiveDirectionUI(true);
        NGUITools.SetActive(gameObject.transform.parent.gameObject, true);
        break;
      case SkillCategory.kSkillE:
        qua = Direction(SkillCategory.kSkillE);
        lastSkill = SkillCategory.kSkillE;
        ActiveDirectionUI(true);
        NGUITools.SetActive(gameObject.transform.parent.gameObject, true);
        break;
      default: break;
    }

    this.transform.parent.transform.rotation = qua;
    NGUITools.SetActive(gameObject.transform.parent.gameObject, true);
    if (direct != SkillCategory.kSkillE && direct != SkillCategory.kSkillQ) {
      time = 0.0f;
    }

  }
  void Zoom(float wantscale, float wantalpha)
  {
    if (wantscale == nowscale) return;
    float scale = wantscale / nowscale;

    UISprite us = this.gameObject.GetComponent<UISprite>();
    if (us != null) {
      us.alpha = wantalpha;
      us.width = (int)System.Math.Round(us.width * scale);
      us.height = (int)System.Math.Round(us.height * scale);

    }

    nowscale = wantscale;
  }
  Quaternion Direction(SkillCategory nowskill)
  {
    Quaternion dir = new Quaternion();
    if (nowskill == SkillCategory.kSkillQ) {
      switch (lastSkill) {
        case SkillCategory.kSkillD: dir = new Quaternion(0, 0, 0.7f, 0.7f); break;
        case SkillCategory.kSkillB: dir = new Quaternion(0.7f, -0.7f, 0f, 0f); break;
        case SkillCategory.kSkillC: dir = new Quaternion(0, 1.0f, 0, 0); break;
        case SkillCategory.kSkillA: dir = new Quaternion(0, 0, 0f, 1.0f); break;
        default: break;
      }
      return dir;
    }
    if (nowskill == SkillCategory.kSkillE) {
      switch (lastSkill) {
        case SkillCategory.kSkillB: dir = new Quaternion(0, 0, -0.7f, 0.7f); break;
        case SkillCategory.kSkillD: dir = new Quaternion(0.7f, 0.7f, 0.0f, 0.0f); break;
        case SkillCategory.kSkillA: dir = new Quaternion(1.0f, 0, 0, 0); break;
        case SkillCategory.kSkillC: dir = new Quaternion(0, 0, -1.0f, 0); break;
        default: break;
      }
      return dir;
    }
    return dir;
  }
  void SkillIsFalse()
  {
    UISprite us = this.gameObject.GetComponent<UISprite>();
    if (us != null) {
      UIManager.SkillDrectorColor = new Color(255, 0, 0);
      us.color = new Color(255, 0, 0);
      //us.spriteName = "StraightArrow";
    }
    isHaveSkill = false;
  }

  void ActiveDirectionUI(bool wantactive)
  {
    UISprite us = this.gameObject.GetComponent<UISprite>();
    if (us != null) {
      if (wantactive && isHaveSkill) {
        us.spriteName = "BentArrow";
        us.color = UIManager.SkillDrectorColor;
        if (NGUITools.GetActive(gameObject) == false) {
          NGUITools.SetActive(gameObject.transform.parent.gameObject, true);
        }
        time = 0.12f;
      } else {
        us.spriteName = "StraightArrow";
        us.color = UIManager.SkillDrectorColor;
      }
    }

  }
  private float nowscale = 1.0f;
  private float time = 0.0f;
  private bool isSkillWant = false;
  private bool isHaveSkill = true;
  private DashFire.SkillCategory lastSkill = DashFire.SkillCategory.kSkillA;
  private Vector2 lastStart;
}
