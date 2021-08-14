using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatWin : MonoBehaviour
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
    } catch (System.Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  // Use this for initialization
  void Start()
  {
    if (eventlist != null) { eventlist.Clear(); }
    object eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<int, int>("ge_turnover_card", "ui", GetPrize);
    if (eo != null) { eventlist.Add(eo); }
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_ui_unsubscribe", "ui", UnSubscribe);
    if (eo != null) eventlist.Add(eo);


    Transform tf = transform.Find("Time/Label");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        timelabel = ul;
        ul.text = "16:00";
      }
    }
    for (int i = 0; i < 4; ++i) {
      Transform tfcard = transform.Find(i.ToString());
      if (tfcard != null) {
        UIEventListener.Get(tfcard.gameObject).onClick = CardClick;
        tfcard.localPosition = new Vector3(0f, -40f, 0f);
      }
    }
    UIManager.Instance.HideWindowByName("CombatWin");
  }

  // Update is called once per frame
  void Update()
  {
    if (Clickwhich != -1) { return; }
    time += RealTime.deltaTime;
    int second = (int)(16 - time);

    if (second > 15.0f) {
      second = 15;
    } else if (second <= 0.0f) {
      second = 0;
      if (Clickwhich == -1) {
        Transform tf = transform.Find("0");
        if (tf != null) {
          CardClick(tf.gameObject);
        }
      }
    }


    if (timelabel != null) {
      string str1 = (second / 60).ToString();
      if (str1.Length == 1) {
        str1 = "0" + str1;
      }
      string str2 = (second % 60).ToString();
      if (str2.Length == 1) {
        str2 = "0" + str2;
      }
      timelabel.text = str1 + ":" + str2;
    }
  }
  public void ButtonClick()
  {
    //发送消息
    if (Clickwhich >= 0 && Clickwhich < 4) {
      UIManager.Instance.HideWindowByName("CombatWin");
      DashFire.GfxSystem.EventChannelForLogic.Publish("ge_return_maincity", "lobby");
    } else {
      Transform tf = transform.Find("0");
      if (tf != null) {
        CardClick(tf.gameObject);
      }
    }
  }
  public void CardClick(GameObject go)
  {
    if (!firstclicked) {
      firstclicked = true;
      if (positionOK) {
        positionOK = false;
        int i;
        if (int.TryParse(go.transform.name, out i)) {
          Clickwhich = i;
          //           Transform tf = transform.Find(i.ToString());
          //           if (tf != null) {
          TweenRotation tr = /*tf.gameObject*/go.GetComponents<TweenRotation>()[0];
          if (tr != null) {
            tr.PlayForward();
          }
          //}
        }
        Transform tfc = transform.Find("ExtractNum");
        if (tfc != null) {
          UILabel ul = tfc.gameObject.GetComponent<UILabel>();
          if (ul != null) {
            ul.text = DashFire.StrDictionaryProvider.Instance.Format(136, 0);
          }
        }
      }
      for (int m = 0; m < 4; ++m) {
        Transform tf = transform.Find(m.ToString());
        if (tf != null) {
          BoxCollider bc = tf.gameObject.GetComponent<BoxCollider>();
          if (bc != null) {
            bc.enabled = false;
          }
        }
      }
    } else {
      if (go != null) {
        TweenRotation tr = go.GetComponents<TweenRotation>()[0];
        if (tr != null) {
          tr.PlayForward();
        }
      }
    }
  }
  public void SetPositionOk()
  {
    positionOK = true;
    Transform tf = transform.Find("Container");
    if (tf != null) {
      NGUITools.SetActive(tf.gameObject, false);
    }
  }
  public void TweenPotationHalfOK()
  {
    try {
      if (!firsthalfed) {
        SetInfo(Clickwhich, prizeid);
        firsthalfed = true;
        StartCoroutine(DelayForTurnedCard());
        Transform tf = transform.Find(Clickwhich.ToString() + "/Light");
        if (tf != null) {
          NGUITools.SetActive(tf.gameObject, true);
        }
        for (int i = 0; i < 4; ++i) {
          Transform tfcard = transform.Find(i.ToString());
          if (tfcard != null) {
            UIEventListener.Get(tfcard.gameObject).onClick -= CardClick;
          }
        }
      } else {
        if (firstsetall) {
          Transform tf = transform.Find("Button");
          if (tf != null) {
            NGUITools.SetActive(tf.gameObject, true);
          }
          firstsetall = false;
          DashFire.Data_SceneConfig dsc = DashFire.SceneConfigProvider.Instance.GetSceneConfigById(DFMUiRoot.NowSceneID);
          if (dsc != null) {
            DashFire.Data_SceneDropOut dsdo = DashFire.SceneConfigProvider.Instance.GetSceneDropOutById(dsc.m_DropId);
            if (dsdo != null) {
              if (dsdo.m_ItemCount == 4 && dsdo.m_ItemIdList != null && dsdo.m_ItemIdList.Count == 4) {
                bool sign = true;
                for (int j = 0; j < 4; ++j) {
                  if (j != Clickwhich) {
                    if (dsdo.m_ItemIdList[j] == prizeid) {
                      if (sign) {
                        sign = false;
                        SetInfo(j, dsdo.m_ItemIdList[Clickwhich]);
                      } else {
                        SetInfo(j, dsdo.m_ItemIdList[j]);
                      }
                    } else {
                      SetInfo(j, dsdo.m_ItemIdList[j]);
                    }
                  }
                }
              }
            }
          }
        }
      }
    } catch (System.Exception) {
      //
    }
  }
  public IEnumerator DelayForTurnedCard()
  {
    yield return new WaitForSeconds(0.8f);
    for (int j = 0; j < 4; ++j) {
      if (Clickwhich != j) {
        Transform tf = transform.Find(j.ToString());
        if (tf != null && tf.gameObject != null) {
          CardClick(tf.gameObject);
        }
      }
    }
  }
  void SetInfo(int which, int id)
  {
    Transform tf = transform.Find(which.ToString());
    if (tf != null) {
      TweenRotation tr = tf.gameObject.GetComponents<TweenRotation>()[1];
      if (tr != null) {
        tr.PlayForward();
      }
    }
    tf = transform.Find(which.ToString());
    if (tf != null) {
      UISprite us = tf.gameObject.GetComponent<UISprite>();
      if (us != null) {
        us.spriteName = "pai2";
      }
    }
    DashFire.ItemConfig ic = DashFire.LogicSystem.GetItemDataById(id);
    tf = transform.Find(which.ToString() + "/Texture");
    if (tf != null) {
      NGUITools.SetActive(tf.gameObject, true);
      UITexture ut = tf.gameObject.GetComponent<UITexture>();
      if (ic != null && ut != null) {
        Texture tt = GamePokeyManager.GetTextureByPicName(ic.m_ItemTrueName);
        if (tt != null) {
          ut.mainTexture = tt;
        } else {
          DashFire.ResLoadAsyncHandler.LoadAsyncItem(ic.m_ItemTrueName, ut);
        }
      }
    }
    tf = transform.Find(which.ToString() + "/Sprite");
    if (tf != null) {
      NGUITools.SetActive(tf.gameObject, true);
      if (ic != null) {
        UISprite us = tf.gameObject.GetComponent<UISprite>();
        if (us != null) {
          us.spriteName = "EquipFrame" + ic.m_PropertyRank;
        }
      }
    }
    tf = transform.Find(which.ToString() + "/name");
    if (tf != null) {
      NGUITools.SetActive(tf.gameObject, true);
      if (ic != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = ic.m_ItemName;
          Color col = new Color();
          switch (ic.m_PropertyRank) {
            case 1:
              col = new Color(1.0f, 1.0f, 1.0f);
              break;
            case 2:
              col = new Color(0.0f, 1.0f, 0.0f);
              break;
            case 3:
              col = new Color(0.0f, 0.0f, 1.0f);
              break;
            case 4:
              col = new Color(0.824f, 0.0f, 0.91f);
              break;
            case 5:
              col = new Color(1.0f, 0.54f, 0.19f);
              break;
            default:
              col = new Color(1.0f, 1.0f, 1.0f);
              break;
          }
          ul.color = col;
        }
      }
    }
  }
  private void GetPrize(int id, int num)
  {
    try {
      prizeid = id;
      prizecount = num;
    } catch (System.Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private bool positionOK = false;
  private int Clickwhich = -1;
  private float time = 0.0f;
  private UILabel timelabel = null;
  private int prizeid = 0;
  private int prizecount = 0;
  private bool firstclicked = false;
  private bool firsthalfed = false;
  private bool firstsetall = true;
}
