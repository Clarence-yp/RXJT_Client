using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MailManage : MonoBehaviour
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
      MailDic.Clear();
      golist.Clear();
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  // Use this for initialization
  void Start()
  {
    if (MailDic != null) { MailDic.Clear(); }
    if (golist != null) { golist.Clear(); }
    if (eventlist != null) { eventlist.Clear(); }
    object eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<List<DashFire.MailInfo>>("ge_sync_mail_list", "mail", SyncMailList);
    if (eo != null) { eventlist.Add(eo); }
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_notify_new_mail", "mail", NewMail);
    if (eo != null) { eventlist.Add(eo); }
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_ui_unsubscribe", "ui", UnSubscribe);
    if (eo != null) eventlist.Add(eo);

    DashFire.GfxSystem.EventChannelForLogic.Publish("ge_get_mail_list", "lobby");
    UIManager.Instance.HideWindowByName("Mail");
  }

  // Update is called once per frame
  void Update()
  {

  }

  void NewMail()
  {
    try {
      DashFire.GfxSystem.EventChannelForLogic.Publish("ge_get_mail_list", "lobby");
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  public void CloseMail()
  {
    UIManager.Instance.HideWindowByName("Mail");
  }

  void SyncMailList(List<DashFire.MailInfo> maillist)
  {
    try {
      if (maillist != null) {
        foreach (DashFire.MailInfo mi in maillist) {
          if (!HaveThisMail(mi.m_MailGuid)) {
            AddMail(mi);
          }
        }
      }
      Transform tf = gameObject.transform.Find("MetalFrame/Container/ScrollView/Grid");
      if (tf != null) {
        UIGrid ug = tf.gameObject.GetComponent<UIGrid>();
        if (ug != null) {
          ug.repositionNow = true;
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  bool HaveThisMail(ulong mailguid)
  {
    foreach (DashFire.MailInfo mi in MailDic.Values) {
      if (mailguid == mi.m_MailGuid) {
        return true;
      }
    }
    return false;
  }

  void AddMail(DashFire.MailInfo mailinfo)
  {
    if (mailinfo != null) {
      GameObject go = DashFire.ResourceSystem.GetSharedResource("UI/Mail/MailItem") as GameObject;
      if (go != null) {
        Transform tf = gameObject.transform.Find("MetalFrame/Container/ScrollView/Grid");
        if (tf != null) {
          go = NGUITools.AddChild(tf.gameObject, go);
          if (go != null) {
            UIEventListener.Get(go).onClick = MailItemClick;
            MailDic.Add(go, mailinfo);
            SetMailItemInfo(go, mailinfo);
          }
        }
      }
    }
  }

  void SetMailItemInfo(GameObject go, DashFire.MailInfo mailinfo)
  {
    if (go != null && mailinfo != null) {
      Transform tf = go.transform.Find("Date");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = mailinfo.m_SendTime.ToString("MM/dd/HH/ss");
        }
      }
      tf = go.transform.Find("Name");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = mailinfo.m_Title;
        }
      }
      tf = go.transform.Find("Sender");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = mailinfo.m_Sender;
        }
      }
      tf = go.transform.Find("MailImage");
      if (tf != null) {
        UISprite us = tf.gameObject.GetComponent<UISprite>();
        if (us != null) {
          if (mailinfo.m_AlreadyRead) {
            us.spriteName = "mail2";
          } else {
//             if (mailinfo.m_Gold == 0 && mailinfo.m_Money == 0 && (mailinfo.m_Items == null || mailinfo.m_Items.Count == 0)) {
//               us.spriteName = "xingfeng";
//             } else {
              us.spriteName = "mail1";
            //}
          }
        }
      }
    }
  }

  void SetMailIntroduceInfo(DashFire.MailInfo mi)
  {
    Transform tfo = transform.Find("MetalFrame/RoleInfo/DragThing");
    if (tfo != null) {
      tfo.localPosition = new Vector3(0.0f, 11.0f, 0.0f);
    } else {
      return;
    }
    Transform tf = tfo.Find("Label");
    if (tf != null) {
      if (mi != null) {
        nowread = mi.m_MailGuid;
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          string str = "";
          //str += (mi.m_Title + "\n");
          str += (mi.m_SendTime.ToString("yyyy/MM/dd/HH/ss") + "\n");
          str += mi.m_Text;
          ul.text = str;
        }
        bool sign = false;
        Vector3 pos = tf.localPosition;
        pos = new Vector3(pos.x, pos.y - ul.localSize.y - 15, 0.0f);
        if (mi.m_Money != 0) {
          sign = true;
          tf = tfo.Find("Money");
          if (tf != null) {
            GameObject go = tf.gameObject;
            if (go != null) {
              UISprite us = go.GetComponent<UISprite>();

              Transform tf2 = go.transform.Find("Amount");
              if (tf2 != null) {
                UILabel ul1 = tf2.gameObject.GetComponent<UILabel>();
                if (ul1 != null) {
                  ul1.text = "X " + mi.m_Money;
                }
              }
              go.transform.localPosition = pos;
              NGUITools.SetActive(go, true);
              if (us != null) {
                pos = new Vector3(pos.x, pos.y - us.localSize.y - 15, 0.0f);
              }
            }
          }
        }
        if (mi.m_Gold != 0) {
          sign = true;
          tf = tfo.Find("Diamond");
          if (tf != null) {
            GameObject go = tf.gameObject;
            if (go != null) {
              UISprite us = go.GetComponent<UISprite>();

              Transform tf2 = go.transform.Find("Amount");
              if (tf2 != null) {
                UILabel ul1 = tf2.gameObject.GetComponent<UILabel>();
                if (ul1 != null) {
                  ul1.text = "X " + mi.m_Gold;
                }
              }
              go.transform.localPosition = pos;
              NGUITools.SetActive(go, true);
              if (us != null) {
                pos = new Vector3(pos.x, pos.y - us.localSize.y - 15, 0.0f);
              }
            }
          }
        }
//         if (mi.m_Gold != 0) {
//           sign = true;
//           tf = tfo.Find("Exp");
//           if (tf != null) {
//             GameObject go = tf.gameObject;
//             if (go != null) {
//               UISprite us = go.GetComponent<UISprite>();
// 
//               Transform tf2 = go.transform.Find("Amount");
//               if (tf2 != null) {
//                 UILabel ul1 = tf2.gameObject.GetComponent<UILabel>();
//                 if (ul1 != null) {
//                   ul1.text = "X " + mi.m_Gold;
//                 }
//               }
//               go.transform.localPosition = pos;
//               NGUITools.SetActive(go, true);
//               if (us != null) {
//                 pos = new Vector3(pos.x, pos.y - us.localSize.y - 15, 0.0f);
//               }
//             }
//           }
//         }
        if (mi.m_Items != null) {
          foreach (DashFire.MailItem mailitem in mi.m_Items) {
            if (mailitem != null) {
              sign = true;
              DashFire.ItemConfig ic = DashFire.LogicSystem.GetItemDataById(mailitem.m_ItemId);
              if (ic != null) {
                GameObject go = DashFire.ResourceSystem.GetSharedResource("UI/Mail/MailAward") as GameObject;
                if (go != null) {
                  UITexture ut = go.GetComponent<UITexture>();
                  if (ut != null) {
                    Texture tt = GamePokeyManager.GetTextureByPicName(ic.m_ItemTrueName);
                    if (tt != null) {
                      ut.mainTexture = tt;
                    } else {
                      DashFire.ResLoadAsyncHandler.LoadAsyncItem(ic.m_ItemTrueName, ut);
                    }
                  }
                  Transform tf2 = go.transform.Find("Amount");
                  if (tf2 != null) {
                    UILabel ul1 = tf2.gameObject.GetComponent<UILabel>();
                    if (ul1 != null) {
                      ul1.text = "X " + mailitem.m_ItemNum;
                    }
                  }
                  go = NGUITools.AddChild(tfo.gameObject, go);
                  if (go != null) {
                    go.transform.localPosition = pos;
                    golist.Add(go);
                  }
                  pos = new Vector3(pos.x, pos.y - ut.localSize.y - 15, 0.0f);
                }
              }
            }
          }
        }
        if (sign) {
          tf = transform.Find("MetalFrame/RoleInfo/DragThing/ReceiveButton");
          if (tf != null) {
            tf.localPosition = new Vector3(0.0f, pos.y, 0.0f);
            NGUITools.SetActive(tf.gameObject, true);
          }
        }
      }
    }
    tf = transform.Find("sp_hongdi1/Label");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        ul.text = mi.m_Sender;
      }
    }
    tf = transform.Find("sp_hongdi2/Label");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        ul.text = mi.m_Title;
      }
    }
  }

  void DeleteMail(ulong mailid)
  {
    foreach (GameObject go in MailDic.Keys) {
      DashFire.MailInfo minfo = MailDic[go];
      if (minfo != null && mailid == minfo.m_MailGuid) {
        MailDic.Remove(go);
        NGUITools.DestroyImmediate(go);
        break;
      }
    }
    Transform tf = gameObject.transform.Find("MetalFrame/Container/ScrollView/Grid");
    if (tf != null) {
      UIGrid ug = tf.gameObject.GetComponent<UIGrid>();
      if (ug != null) {
        ug.repositionNow = true;
      }
    }
  }

  void MailItemClick(GameObject go)
  {
    if (go != null && MailDic.ContainsKey(go)) {
      ClearIntroduce();
      DashFire.MailInfo minfo = MailDic[go];
      SetMailIntroduceInfo(minfo);

      DashFire.GfxSystem.EventChannelForLogic.Publish("ge_read_mail", "lobby", minfo.m_MailGuid);

      Transform tf = go.transform.Find("MailImage");
      if (tf != null) {
        UISprite us = tf.gameObject.GetComponent<UISprite>();
        if (us != null) {
          us.spriteName = "mail2";
        }
      }
    }
  }

  public void ReceiveButton()
  {
    DashFire.GfxSystem.EventChannelForLogic.Publish("ge_receive_mail", "lobby", nowread);
    DeleteMail(nowread);
    ClearIntroduce();
  }

  void ClearIntroduce()
  {
    Transform tf = transform.Find("MetalFrame/RoleInfo/DragThing/Label");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        ul.text = "";
      }
    }
    tf = transform.Find("sp_hongdi1/Label");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        ul.text = "";
      }
    }
    tf = transform.Find("sp_hongdi2/Label");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        ul.text = "";
      }
    }
    foreach (GameObject go in golist) {
      if (go != null) {
        NGUITools.DestroyImmediate(go);
      }
    }
    golist.Clear();
    tf = transform.Find("MetalFrame/RoleInfo/DragThing/ReceiveButton");
    if (tf != null) {
      NGUITools.SetActive(tf.gameObject, false);
    }
    tf = transform.Find("MetalFrame/RoleInfo/DragThing/Money");
    if (tf != null) {
      NGUITools.SetActive(tf.gameObject, false);
    }
    tf = transform.Find("MetalFrame/RoleInfo/DragThing/Diamond");
    if (tf != null) {
      NGUITools.SetActive(tf.gameObject, false);
    }
    tf = transform.Find("MetalFrame/RoleInfo/DragThing/Exp");
    if (tf != null) {
      NGUITools.SetActive(tf.gameObject, false);
    }
  }
  private Dictionary<GameObject, DashFire.MailInfo> MailDic = new Dictionary<GameObject, DashFire.MailInfo>();
  private List<GameObject> golist = new List<GameObject>();
  private ulong nowread = 0;
}
