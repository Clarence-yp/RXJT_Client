using UnityEngine;
using DashFire;
using System;
using System.Collections;
using System.Collections.Generic;

public class MarsManager : MonoBehaviour
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
      }
      eventlist.Clear();
      golist.Clear();
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
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<List<GowDataForMsg>>("ge_sync_gowstar_list", "gowstar", SyncMars);
    if (eo != null) eventlist.Add(eo);
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_update_role_dynamic_property", "ui", UpdateDynamicProperty);
    if (eo != null) { eventlist.Add(eo); }
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_pvpmatch_result", "lobby", PvpMatchResult);
    if (eo != null) { eventlist.Add(eo); }

    DashFire.GfxSystem.EventChannelForLogic.Publish("ge_get_gowstar_list", "lobby", 0, 100);
    SetStaticProperty();
    RecordSomething();
    SetDanamicProperty();
    UIManager.Instance.HideWindowByName("Mars");
  }

  // Update is called once per frame
  void Update()
  {
    if (ismatching) {
      time += RealTime.deltaTime;

      if (timelabel != null) {
        string str1 = ((int)time / 60).ToString();
        if (str1.Length == 1) {
          str1 = "0" + str1;
        }
        string str2 = ((int)time % 60).ToString();
        if (str2.Length == 1) {
          str2 = "0" + str2;
        }
        timelabel.text = str1 + ":" + str2;
      }
      if (matchlabel != null) {
        System.Text.StringBuilder sb = new System.Text.StringBuilder(DashFire.StrDictionaryProvider.Instance.GetDictString(137));
        sb.Append('.', (int)(time * 10 % 7));
        if (matchlabel != null) {
          matchlabel.text = sb.ToString();
        }
      }
    }
    UpdateDynamicProperty();
  }
  void PvpMatchResult()
  {
    DashFire.LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", DashFire.StrDictionaryProvider.Instance.GetDictString(156),
                                                      DashFire.StrDictionaryProvider.Instance.GetDictString(140), null, null, null, false);
    if (ismatching) {
      Matching();
    }
  }
  private void SyncMars(List<GowDataForMsg> marslist)
  {
    try {
      if (marslist != null) {
        int mlcount = marslist.Count;
        int glcount = golist.Count;
        Transform tfr = transform.Find("Right1/ScrollView/Grid");
        if (tfr == null) return;
        for (int i = 0; i < mlcount; ++i) {
          if (i < glcount) {
            GameObject go = golist[i];
            if (go != null) {
              GowDataForMsg gdfm = marslist[i];
              if (gdfm != null) {
                SetMarsCellInfo(go, gdfm, i);
              }
            }
          } else {
            GameObject go = DashFire.ResourceSystem.GetSharedResource("UI/Mars/MarsCell") as GameObject;
            if (go != null) {
              go = NGUITools.AddChild(tfr.gameObject, go);
              if (go != null) {
                golist.Add(go);
                GowDataForMsg gdfm = marslist[i];
                if (gdfm != null) {
                  SetMarsCellInfo(go, gdfm, i);
                }
              }
            }
          }
        }
        if (glcount > mlcount) {
          for (int j = mlcount; j < glcount; ++j) {
            GameObject go = golist[j];
            if (go != null) {
              NGUITools.DestroyImmediate(go);
            }
          }
          golist.RemoveRange(mlcount, glcount - mlcount);
        }
        UIGrid ug = tfr.gameObject.GetComponent<UIGrid>();
        if (ug != null) {
          ug.repositionNow = true;
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private void SetMarsCellInfo(GameObject go, GowDataForMsg gdfm, int order)
  {
    if (go != null && gdfm != null) {
      Transform tf = go.transform.Find("Label4/Sprite");
      if (tf != null) {
        UISprite us = tf.gameObject.GetComponent<UISprite>();
        if (us != null) {
          us.spriteName = "no" + (order + 1);
        }
      }
      tf = go.transform.Find("Head");
      if (tf != null) {
        UISprite us = tf.gameObject.GetComponent<UISprite>();
        if (us != null) {
          if (gdfm.m_Heroid == 1) {
            us.spriteName = "kuang-zhan-shi-tou-xiang2";
          } else {
            us.spriteName = "ci-ke-tou-xiang2";
          }
        }
      }
      tf = go.transform.Find("Label0");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = "Lv." + gdfm.m_Level;
        }
      }
      tf = go.transform.Find("Label1");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = gdfm.m_FightingScore.ToString();
        }
      }
      tf = go.transform.Find("Label2");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = gdfm.m_GowElo.ToString();
        }
      }
      tf = go.transform.Find("Label3");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = gdfm.m_Nick;
        }
      }
      tf = go.transform.Find("Label4");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          if (order + 1 < 4) {
            ul.text = "NOD" + (char)('A' + order);
          } else {
            ul.text = (order + 1).ToString() + "ETH";
          }
        }
      }
    }
  }
  private void RecordSomething()
  {
    Transform tf = transform.Find("Right0/Time/Label");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        timelabel = ul;
      }
    }
    tf = transform.Find("Right0/Label");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        matchlabel = ul;
        ul.text = "";
      }
    }
    tf = transform.Find("Head/level");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        lvlabel = ul;
      }
    }
    tf = transform.Find("LeftFrame/Top/Label3");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        fightscorelabel = ul;
      }
    }
    tf = transform.Find("LeftFrame/Bottom/Label1");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        myscorelabel = ul;
      }
    }
    tf = transform.Find("Money/Label");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        mymoney = ul;
      }
    }
    tf = transform.Find("Diamond/Label");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        mydiamond = ul;
      }
    }
  }
  private void SetStaticProperty()
  {
    DashFire.RoleInfo player = DashFire.LobbyClient.Instance.CurrentRole;
    if (player != null) {
      Transform tf = transform.Find("Head/name_label");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = player.Nickname;
        }
      }
      tf = transform.Find("LeftFrame/Top/Label1");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          Data_PlayerConfig dpc = PlayerConfigProvider.Instance.GetPlayerConfigById(player.HeroId);
          if (dpc != null) {
            ul.text = dpc.m_Name;
          }
        }
      }
      tf = transform.Find("Head/headPic");
      if (tf != null) {
        UISprite us = tf.gameObject.GetComponent<UISprite>();
        if (us != null) {
          if (player.HeroId == 1) {
            us.spriteName = "jianshi";
          } else {
            us.spriteName = "cike";
          }
        }
      }
      tf = transform.Find("Right0/Button/Label");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = DashFire.StrDictionaryProvider.Instance.GetDictString(138);
        }
      }
    }
  }
  private void SetDanamicProperty()
  {
    DashFire.RoleInfo player = DashFire.LobbyClient.Instance.CurrentRole;
    if (player != null) {
      if (lvlabel != null) {
        lvlabel.text = player.Level.ToString();
      }
      if (fightscorelabel != null) {
        fightscorelabel.text = Mathf.FloorToInt(player.FightingScore).ToString();
      }
      if (myscorelabel != null) {
        if (UIManager.Instance.MarsIntegral == 0) {
          myscorelabel.text = player.Gow.GowElo.ToString();
        } else {
          myscorelabel.text = UIManager.Instance.MarsIntegral.ToString();
        }
      }
    }
    if (mymoney != null) {
      mymoney.text = player.Money.ToString();
    }
    if (mydiamond != null) {
      mydiamond.text = player.Gold.ToString();
    }
  }
  private void UpdateDynamicProperty()
  {
    try {
      SetDanamicProperty();
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  public void Return()
  {
    if (ismatching) {
      DashFire.LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", DashFire.StrDictionaryProvider.Instance.GetDictString(139), DashFire.StrDictionaryProvider.Instance.GetDictString(140), null, null, null, false);
      return;
    }
    UIManager.Instance.HideWindowByName("Mars");
  }
  public void ChangeShow()
  {
    if (ismatching) {
      DashFire.LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", DashFire.StrDictionaryProvider.Instance.GetDictString(139), DashFire.StrDictionaryProvider.Instance.GetDictString(140), null, null, null, false);
      return;
    }
    ismarsranking = !ismarsranking;
    Transform tf = transform.Find("Button/Label");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        if (ismarsranking) {
          ul.text = DashFire.StrDictionaryProvider.Instance.GetDictString(141);
        } else {
          ul.text = DashFire.StrDictionaryProvider.Instance.GetDictString(142);
        }
      }
    }
    tf = transform.Find("TitleBack/Title");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        if (!ismarsranking) {
          ul.text = DashFire.StrDictionaryProvider.Instance.GetDictString(141);
        } else {
          ul.text = DashFire.StrDictionaryProvider.Instance.GetDictString(142);
        }
      }
    }
    tf = transform.Find("Right0");
    if (tf != null) {
      NGUITools.SetActive(tf.gameObject, !ismarsranking);
    }
    tf = transform.Find("Right1");
    if (tf != null) {
      NGUITools.SetActive(tf.gameObject, ismarsranking);
    }
  }

  public void ShowSkill()
  {
    if (ismatching) {
      DashFire.LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", DashFire.StrDictionaryProvider.Instance.GetDictString(139), DashFire.StrDictionaryProvider.Instance.GetDictString(140), null, null, null, false);
      return;
    }
    UIManager.Instance.HideWindowByName("Mars");
    UIManager.Instance.ShowWindowByName("SkillPanel");
  }

  public void Matching()
  {
    string str = "";
    if (ismatching) {
      //停止匹配
      str = DashFire.StrDictionaryProvider.Instance.GetDictString(138);
      DashFire.LogicSystem.PublishLogicEvent("ge_cancel_match", "lobby");
      if (matchlabel != null) {
        timelabel.text = "00:00";
      }
      if (matchlabel != null) {
        matchlabel.text = "";
      }
    } else {
      //开始匹配
      str = DashFire.StrDictionaryProvider.Instance.GetDictString(143);
      DashFire.LogicSystem.PublishLogicEvent("ge_select_scene", "lobby", 3001);
    }
    time = 0.0f;
    ismatching = !ismatching;
    Transform tf = transform.Find("Right0/Button/Label");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        ul.text = str;
      }
    }
    tf = transform.Find("Right0/Time");
    if (tf != null) {
      NGUITools.SetActive(tf.gameObject, ismatching);
    }
  }
  public void BuyGold()
  {
    UIManager.Instance.ShowWindowByName("GoldBuy");
  }
  private bool ismarsranking = false;
  private bool ismatching = false;
  private float time = 0.0f;
  private UILabel timelabel = null;
  private UILabel matchlabel = null;
  private UILabel fightscorelabel = null;
  private UILabel lvlabel = null;
  private UILabel myscorelabel = null;
  private UILabel mymoney = null;
  private UILabel mydiamond = null;
  private List<GameObject> golist = new List<GameObject>();
}
