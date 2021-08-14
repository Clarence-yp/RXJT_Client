using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CombatFalse : MonoBehaviour
{
  private List<object> m_EventList = new List<object>();
  public void UnSubscribe()
  {
    try {
      foreach (object obj in m_EventList) {
        if (null != obj) {
          DashFire.LogicSystem.EventChannelForGfx.Unsubscribe(obj);
        }
      }
      m_EventList.Clear();
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  // Use this for initialization
  void Start()
  {
    m_EventList.Clear();
    object obj = null;
    obj = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_ui_unsubscribe", "ui", UnSubscribe);
    if (obj != null) m_EventList.Add(obj);
    obj = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_role_dead", "ui", RoleDead);
    if (obj != null) m_EventList.Add(obj);

    Transform tf = transform.Find("Time/Label");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        timelabel = ul;
        ul.text = "16:00";
      }
    }
    time = 0.0f;
    UIManager.Instance.HideWindowByName("CombatFalse");
  }

  // Update is called once per frame
  void Update()
  {
    time += RealTime.deltaTime;

    int second = (int)(16 - time);
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
    if (second <= 0.0f) {
      //退回主城
      GoBack();
    }
  }
  private void RoleDead()
  {
    try {
      UIManager.Instance.ShowWindowByName("CombatFalse");
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  public void GoBack()
  {
    DashFire.GfxSystem.EventChannelForLogic.Publish("ge_request_relive", "lobby", false);
    UIManager.Instance.HideWindowByName("CombatFalse");
  }
  public void HeroRelive()
  {
    DashFire.RoleInfo ri = DashFire.LobbyClient.Instance.CurrentRole;
    if (ri != null) {
      if (ri.Gold < 20) {
        DashFire.LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui",
        DashFire.StrDictionaryProvider.Instance.GetDictString(149), DashFire.StrDictionaryProvider.Instance.GetDictString(140), null, null, null, false);
      } else {
        if (DFMUiRoot.PveFightInfo != null) {
          PveFightInfo pfi = DFMUiRoot.PveFightInfo.GetComponent<PveFightInfo>();
          if (pfi != null) {
            pfi.AboutHeroDead();
          }
        }
        time = 0.0f;
        DashFire.GfxSystem.EventChannelForLogic.Publish("ge_request_relive", "lobby", true);
        UIManager.Instance.HideWindowByName("CombatFalse");
      }
    }
  }
  private float time = 0.0f;
  private UILabel timelabel = null;
}
