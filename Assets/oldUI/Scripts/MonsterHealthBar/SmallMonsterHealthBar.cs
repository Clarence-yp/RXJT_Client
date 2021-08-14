using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DashFire;

public class SmallMonsterHealthBar : MonoBehaviour
{

  // Use this for initialization
  private GameObject goHealthBar = null;
  private List<object> m_EventList = new List<object>();
  void Awake()
  {
    Transform trans = this.transform.Find("HealthBar");
    if (trans == null)
      return;
    goHealthBar = trans.gameObject;
    if (null == goHealthBar) {
      Debug.LogError("Can Not Find HealthBar");
    }
    object obj = DashFire.LogicSystem.EventChannelForGfx.Subscribe<int, int, int>("ge_small_monster_healthbar", "ui", UpdateHealthBar);
    if (null != obj) m_EventList.Add(obj);
    obj = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_ui_unsubscribe", "ui", UnSubscribe);
    if (obj != null) m_EventList.Add(obj);
  }
  void Start()
  {
  }
  public void UnSubscribe()
  {
    try {
      foreach (object obj in m_EventList) {
        if (null != obj) LogicSystem.EventChannelForGfx.Unsubscribe(obj);
      }
      m_EventList.Clear();
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (m_CanCast)
      CastAnimation(goHealthBar);
    if (countdown > 0) {
      countdown -= RealTime.deltaTime;
      if (countdown <= 0)
        SetActive(false);
    }
  }

  public void SetActive(bool isActive)
  {
    NGUITools.SetActive(this.gameObject, isActive);
  }
  void SetHealthValueText(int current, int max)
  {
    SetActive(true);
    Transform trans = this.transform.Find("HealthBar/healthValue");
    if (null == trans)
      return;
    GameObject go = trans.gameObject;
    if (null != go) {
      UILabel label = go.GetComponent<UILabel>();
      if (null != label)
        label.text = current.ToString() + "/" + max.ToString();
    }
  }

  private void SetName(string name)
  {
    Transform trans = transform.Find("name");
    if (trans == null)
      return;
    GameObject go = trans.gameObject;
    UILabel label = null;
    if (go != null)
      label = go.GetComponent<UILabel>();
    if (null != label) {
      label.text = name.ToString();
    }
  }
  void ShakeHealthBar()
  {

    TweenPosition tweenPosition = this.gameObject.GetComponent<TweenPosition>();
    if (tweenPosition != null)
      Destroy(tweenPosition);
    tweenPosition = this.gameObject.AddComponent<TweenPosition>();
    tweenPosition.from = new Vector3(positionVec3.x + 25, positionVec3.y + 25, positionVec3.z);
    tweenPosition.to = positionVec3;
    tweenPosition.duration = 0.5f;
  }

  void UpdateHealthBar(int curValue, int maxValue, int hpDamage)
  {
    try {
      if (Vector3.zero == positionVec3) {
        positionVec3 = this.transform.localPosition;
      }
      SetActive(true);
      ShakeHealthBar();
      SetProgressValue(curValue, maxValue, hpDamage);
      countdown = waitTime;
      if (maxValue <= 0)
        return;
      SetHealthValueText(curValue, maxValue);
      float value = curValue / (float)maxValue;
      UIProgressBar progressBar = null;
      progressBar = goHealthBar.GetComponent<UIProgressBar>();
      if (null != progressBar) {
        progressBar.value = value;
        TweenSpriteAlpha(goHealthBar);
        if (value <= 0) {
          SetLable("Dead");
        } else {
          SetLable("x1");
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  void SetProgressValue(int curValue, int maxValue, int damage)
  {
    if (maxValue == 0)
      return;
    //因为damage是负值
    float percent = (curValue - damage) / (float)maxValue;
    if (goHealthBar != null) {
      Transform trans = goHealthBar.transform.Find("white");
      if (trans == null) return;
      GameObject go = trans.gameObject;
      UISprite sp = go.GetComponent<UISprite>();
      if (sp != null)
        sp.fillAmount = percent;
      trans = goHealthBar.transform.Find("forDel");
      if (trans == null) return;
      go = trans.gameObject;
      sp = go.GetComponent<UISprite>();
      if (sp != null)
        sp.fillAmount = percent;
    }
  }

  void TweenSpriteAlpha(GameObject father)
  {
    GameObject goBack = null;
    UIProgressBar progressBar = null;
    if (null != father) {
      progressBar = father.GetComponent<UIProgressBar>();
      Transform trans = father.transform.Find("white");
      if (trans != null)
        goBack = trans.gameObject;

    }
    if (goBack == null)
      return;
    UISprite spBack = null;
    if (null != goBack) {
      spBack = goBack.GetComponent<UISprite>();

    }
    if (null != spBack && null != progressBar) {
      if (spBack.fillAmount <= progressBar.value) {
        spBack.fillAmount = progressBar.value;
        SetCastFlag(true);
      } else {

        TweenAlpha tween = goBack.GetComponent<TweenAlpha>();
        if (null == tween)
          return;
        tween.enabled = true;
        tween.ResetToBeginning();
        tween.PlayForward();
      }
    }
  }

  void CastAnimation(GameObject father)
  {
    GameObject goBack = null;
    UIProgressBar progressBar = null;
    if (null != father) {
      Transform trans = father.transform.Find("forDel");
      if (trans != null)
        goBack = trans.gameObject;
      progressBar = father.GetComponent<UIProgressBar>();
    }

    UISprite spBack = null;
    if (null != goBack) {
      spBack = goBack.GetComponent<UISprite>();
    }
    if (null != spBack && null != progressBar) {
      if (spBack.fillAmount <= progressBar.value) {
        spBack.fillAmount = progressBar.value;
        SetCastFlag(false);
      } else {
        spBack.fillAmount -= RealTime.deltaTime * 0.2f;
      }
    }

  }

  private void SetLable(string state)
  {
    string path = "HealthBar/state";
    Transform trans = this.transform.Find(path);
    if (trans == null)
      return;
    GameObject go = trans.gameObject;
    if (null != go) {
      UILabel label = go.GetComponent<UILabel>();
      if (null != label)
        label.text = state;
    }
  }

  public void OnTweenAlphaFinished()
  {
    SetCastFlag(true);

    GameObject goBack = null;
    UIProgressBar progressBar = null;
    if (null != goHealthBar) {
      Transform trans = goHealthBar.transform.Find("white");
      if (trans != null)
        goBack = trans.gameObject;
      progressBar = goHealthBar.GetComponent<UIProgressBar>();
    }
    if (goBack == null)
      return;
    UISprite spBack = null;
    if (null != goBack) {
      spBack = goBack.GetComponent<UISprite>();
    }
    if (null != spBack && null != progressBar) {
      if (spBack.fillAmount >= progressBar.value) {
        spBack.fillAmount = progressBar.value;
        //Debug.Log("TweenAlpha is not null!!");
      }
    }

  }

  public void SetCastFlag(bool canCast)
  {
    m_CanCast = canCast;
  }

  public void TestButton()
  {
    if (goHealthBar != null) {
      ShakeHealthBar();
      testValue -= testDelta;
      UpdateHealthBar(testValue, 500, -100);
    }
  }

  private bool m_CanCast = false;
  public float countdown = -1f;
  public float waitTime = 3f;
  public int testDelta = 20;
  public int testValue = 1000;
  private Vector3 positionVec3 = Vector3.zero;
}