using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PveFightInfo : MonoBehaviour
{
  private List<object> eventlist = new List<object>();
  public void UnSubscribe()
  {
    try {
      if (eventlist != null) {
        foreach (object eo in eventlist) {
          if (eo != null) {
            DashFire.LogicSystem.EventChannelForGfx.Unsubscribe(eo);
          }
        }
        eventlist.Clear();
      }
      NGUITools.DestroyImmediate(gameObject);
      DFMUiRoot.PveFightInfo = null;
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  // Use this for initialization
  void Start()
  {
    if (eventlist != null) { eventlist.Clear(); }
    object eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_ui_unsubscribe", "ui", UnSubscribe);
    if (eo != null) eventlist.Add(eo);
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<long>("ge_send_scenestart_time", "ui", PveSceneStartTime);
    if (eo != null) eventlist.Add(eo);
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<float, float, float, int>("ge_gain_money", "ui", PlusMoney);
    if (eo != null) eventlist.Add(eo);
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<int, int, int, int, int, int, int, bool>("ge_victory_panel", "ui", VictoryEnd);
    if (eo != null) eventlist.Add(eo);
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_role_dead", "ui", AboutHeroDead);
    if (eo != null) eventlist.Add(eo);
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_loading_start", "ui", UnSubscribe);
    if (eo != null) eventlist.Add(eo);

    Transform tf = transform.Find("jinbi/Label");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        moneylabel = ul;
        moneylabel.text = "0";
      }
    }
    tf = transform.Find("");

    DashFire.GfxSystem.EventChannelForLogic.Publish("ge_get_scenestart_time", "lobby");
  }

  // Update is called once per frame
  void Update()
  {
    if (signtime) {
      if (timeorsomelabel != null) {
        long residuetime = (long)countDownTime - (DashFire.TimeUtility.GetServerMilliseconds() - StartTime) / 1000;

        string str1 = (residuetime / 60).ToString();
        if (str1.Length == 1) {
          str1 = "0" + str1;
        }
        string str2 = (residuetime % 60).ToString();
        if (str2.Length == 1) {
          str2 = "0" + str2;
        }
        timeorsomelabel.text = str1 + ":" + str2;
        if (residuetime <= 0) {
          signtime = false;
          SetDefeat();
        }
      }
    }
  }
  void SetDefeat()
  {
    Transform tf = transform.Find("TimeOrSome/Sprite");
    if (tf != null) {
      UISprite us = tf.gameObject.GetComponent<UISprite>();
      if (us != null) {
        us.spriteName = "tzshibai";
        us.width = 135;
        us.height = 42;
      }
    }
  }
  public void SetInitInfo(int type, int num0, int num1, int num2)
  {
    count++;
    Transform tf = transform.Find("TimeOrSome/Sprite");
    UISprite us = null;
    if (tf != null) {
      us = tf.gameObject.GetComponent<UISprite>();
    }
    switch (type) {
      case 0:
        if (us != null) {
          us.spriteName = "zd_bjan";
        }
        tf = transform.Find("TimeOrSome/Back");
        if (tf != null) {
          NGUITools.SetActive(tf.gameObject, false);
        }
        tf = transform.Find("TimeOrSome/Label");
        if (tf != null) {
          UILabel ul = tf.gameObject.GetComponent<UILabel>();
          if (ul != null) {
            timeorsomelabel = ul;
          }
        }
        break;
      case 1:
        if (us != null) {
          us.spriteName = "zd_fsan";
        }
        tf = transform.Find("TimeOrSome/Label");
        if (tf != null) {
          NGUITools.SetActive(tf.gameObject, false);
        }
        tf = transform.Find("TimeOrSome/Back/Label");
        if (tf != null) {
          UILabel ul = tf.gameObject.GetComponent<UILabel>();
          if (ul != null) {
            defenselabel = ul;
          }
        }
        tf = transform.Find("TimeOrSome/Back/Front");
        if (tf != null) {
          UISprite uss = tf.gameObject.GetComponent<UISprite>();
          if (uss != null) {
            scrollus = uss;
          }
        }
        break;
      case 2:
        if (us != null) {
          us.spriteName = "zd_tzan";
        }
        tf = transform.Find("TimeOrSome/Back");
        if (tf != null) {
          NGUITools.SetActive(tf.gameObject, false);
        }
        tf = transform.Find("TimeOrSome/Label");
        if (tf != null) {
          UILabel ul = tf.gameObject.GetComponent<UILabel>();
          if (ul != null) {
            timeorsomelabel = ul;
          }
        }
        SetTimeCountDownTime(num0);
        break;
      case 3:
        if (count == 2) {
          StartTime = DashFire.TimeUtility.GetServerMilliseconds();
        }
        if (us != null) {
          us.spriteName = "zd_txan";
        }
        tf = transform.Find("TimeOrSome/Back");
        if (tf != null) {
          NGUITools.SetActive(tf.gameObject, false);
        }
        tf = transform.Find("TimeOrSome/Label");
        if (tf != null) {
          UILabel ul = tf.gameObject.GetComponent<UILabel>();
          if (ul != null) {
            timeorsomelabel = ul;
          }
        }
        SetTimeCountDownTime(num0);
        break;
      case 4:
        tf = transform.Find("TimeOrSome");
        if (tf != null) {
          NGUITools.SetActive(tf.gameObject, false);
        }
        break;
    }
    SetUpdateInfo(type, num0, num1, num2);
  }
  public void SetUpdateInfo(int type, int num0, int num1, int num2)
  {
    switch (type) {
      case 0:
        if (num0 > num1) { num0 = num1; }
        if (timeorsomelabel != null) {
          timeorsomelabel.text = num0.ToString() + "/" + num1;
        }
        if (num0 == num1) {
          SetDefeat();
        }
        break;
      case 1:
        if (num1 > num2) { num1 = num2; }
        if (defenselabel != null) {
          defenselabel.text = DashFire.StrDictionaryProvider.Instance.Format(307, num0);
        }
        if (scrollus != null) {
          scrollus.fillAmount = (float)(num1 * 1.0f / num2);
        }
//         if (num1 == num2) {
//           SetDefeat();
//         }
        break;
      case 2:
        if (!signtime) {
          SetTimeCountDownTime(num0);
        }
        break;
      case 3:
        if (!signtime) {
          SetTimeCountDownTime(num0);
        }
        break;
    }
  }
  private void SetTimeCountDownTime(int time)
  {
    countDownTime = time;
    signtime = true;
  }
  private void PveSceneStartTime(long starttime)
  {
    try {
      StartTime = starttime;
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private void PlusMoney(float x, float y, float z, int moneynum)
  {
    try {
      moneyall += moneynum;
      if (moneylabel != null) {
        moneylabel.text = moneyall.ToString();
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private void VictoryEnd(int one, int two, int three, int four, int five, int six, int seven, bool nine)
  {
    try {
      UnSubscribe();
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  public void AboutHeroDead()
  {
    try {
      NGUITools.SetActive(gameObject, !NGUITools.GetActive(gameObject));
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private bool signtime = false;
  private long StartTime = 0;
  private int countDownTime = 0;
  private UILabel timeorsomelabel = null;
  private UISprite scrollus = null;
  private UILabel moneylabel = null;
  private UILabel defenselabel = null;
  private int moneyall = 0;
  private int count = 0;
}