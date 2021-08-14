using UnityEngine;
using System.Collections;

public class EndShieldScore : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {
    m_Particle = DashFire.ResourceSystem.GetSharedResource("Hero_FX/1_Swordman/ui_FX_01") as GameObject;
    Transform tf = gameObject.transform.Find("Label");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        System.Int64.TryParse(ul.text, out num);
        //maxgrade = Grade(num);
      }
    }
    time = -1.0f;
    nowgrade = "";
    tf = gameObject.transform.Find("Sprite0");
    if (tf != null) {
      UISprite us = tf.gameObject.GetComponent<UISprite>();
      if (us != null) {
        width = us.width;
        us.spriteName = null;
      }
    }
  }

  // Update is called once per frame
  void Update()
  {
    time += RealTime.deltaTime;
    if (time < 0.00001) return;
    if (time < (tweenscalespritecell * 7)) {
      if (!isplayparticle) {
        isplayparticle = true;
      }
      SetNum(((long)(time / (tweenscalespritecell * 7) * num)));
    } else {
      if (!m_IsRebackShow) {
        m_IsRebackShow = true;
        GameObject returnBack = DashFire.ResourceSystem.GetSharedResource("UI/ReturnBack") as GameObject;
        if (returnBack != null) {
          if (this.transform.parent != null)
            NGUITools.AddChild(this.transform.parent.gameObject, returnBack);
        }
      }
      SetNum(num);
      ChangeGrade(Grade(num));
      Destroy(this.GetComponent("EndShieldScored"));
    }
  }
  void SetNum(long nownum)
  {
    Transform tf = gameObject.transform.Find("Label");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        ul.text = "得分："+nownum.ToString();
        ul.alpha = 1.0f;
      }
    }
    tf = gameObject.transform.Find("WShield");
    if (tf != null) {
      UIProgressBar up = tf.gameObject.GetComponent<UIProgressBar>();
      if (up != null) {
        up.value = nownum / 100000000f;
      }
    }
  }
  string Grade(long gradenum)
  {
    string grade = "";
    switch (gradenum / 10000000L) {
      case 1:
      case 2: grade = "D"; break;
      case 3:
      case 4: grade = "C"; break;
      case 5:
      case 6: grade = "B"; break;
      case 7:
      case 8: grade = "A"; break;
      case 9:
      case 10: grade = "S"; break;
      default: break;
    }
    return grade;
  }
  void ChangeGrade(string changegrade)
  {
    Transform tf = gameObject.transform.Find("LabelCopy");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        ul.alpha = 1.0f;
      }
    }
    if (changegrade != nowgrade && changegrade != null) {
      PlayParticle(this.transform.position);

      if (changegrade == "SS") {
        SetAll("S", "S", null, new Vector3((0 - width / 2), 0, 0), new Vector3(width / 2, 0, 0), new Vector3(0, 0, 0));
      } else if (changegrade == "SSS") {
        SetAll("S", "S", "S", new Vector3(0, 0, 0), new Vector3(width, 0, 0), new Vector3(0 - width, 0, 0));
      } else {
        SetAll(changegrade, null, null, new Vector3(0, 0, 0), new Vector3(width, 0, 0), new Vector3(0 - width, 0, 0));
      }

      nowgrade = changegrade;
    }
  }
  void SetAll(string one, string two, string three, Vector3 vone, Vector3 vtwo, Vector3 vthree)
  {
    Transform sstf = gameObject.transform.Find("Sprite0");
    if (sstf != null) {
      UISprite ssus0 = sstf.gameObject.GetComponent<UISprite>();
      if (ssus0 != null) {
        ssus0.spriteName = one;
        ssus0.transform.localPosition = vone;
      }
      TweenScale ts0 = sstf.gameObject.GetComponent<TweenScale>();
      if (ts0 != null)
        Destroy(ts0);
      ts0 = sstf.gameObject.AddComponent<TweenScale>();
      if (ts0 != null) {
        ts0.animationCurve = ac;
        ts0.from = new Vector3(1, 1, 1);
        ts0.to = new Vector3(2, 2, 2);
        TweenScale.Begin(sstf.gameObject, tweenscalespritecell, new Vector3(1, 1, 1));
      }
    }
    sstf = gameObject.transform.Find("Sprite1");
    if (sstf != null) {
      UISprite ssus1 = sstf.gameObject.GetComponent<UISprite>();
      if (ssus1 != null) {
        ssus1.spriteName = two;
        ssus1.transform.localPosition = vtwo;
      }
      TweenScale ts1 = sstf.gameObject.GetComponent<TweenScale>();
      if (ts1 != null)
        Destroy(ts1);
      ts1 = sstf.gameObject.AddComponent<TweenScale>();
      if (ts1 != null) {
        ts1.animationCurve = ac;
        ts1.from = new Vector3(1, 1, 1);
        ts1.to = new Vector3(2, 2, 2);
        TweenScale.Begin(sstf.gameObject, tweenscalespritecell, new Vector3(1, 1, 1));
      }
    }
    sstf = gameObject.transform.Find("Sprite2");
    if (sstf != null) {
      UISprite ssus2 = sstf.gameObject.GetComponent<UISprite>();
      if (ssus2 != null) {
        ssus2.spriteName = three;
        ssus2.transform.localPosition = vthree;
      }
      TweenScale ts2 = sstf.gameObject.GetComponent<TweenScale>();
      if (ts2 != null)
        Destroy(ts2);
      ts2 = sstf.gameObject.AddComponent<TweenScale>();
      if (ts2 != null) {
        ts2.animationCurve = ac;
        ts2.from = new Vector3(1, 1, 1);
        ts2.to = new Vector3(2, 2, 2);
        TweenScale.Begin(sstf.gameObject, tweenscalespritecell, new Vector3(1, 1, 1));
      }
    }
  }
  private void PlayParticle(Vector3 nguiPos)
  {
    if (null != m_Particle) {
      Vector3 curPos = UICamera.mainCamera.WorldToScreenPoint(nguiPos);
      curPos = Camera.main.ScreenToWorldPoint(new Vector3(curPos.x, curPos.y, 1));
      GameObject effect = GameObject.Instantiate(m_Particle, curPos, Quaternion.identity) as GameObject;
      if (null != effect)
        effect.transform.position = curPos;
      Destroy(effect, duration);
    }
  }
  private string nowgrade = "";
  //private string maxgrade = "";
  private int width = 0;
  private long num = 0;
  private float time = 0.0f;
  public AnimationCurve ac = null;
  private float duration = 1.0f;
  private GameObject m_Particle = null;
  private float tweenscalespritecell = 0.3f;
  private bool isplayparticle = false;
  private bool m_IsRebackShow = false;
}
