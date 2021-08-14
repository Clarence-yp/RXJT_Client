using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Option : MonoBehaviour
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
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  // Use this for initialization
  void Start()
  {
    if (eventlist != null) { eventlist.Clear(); }
    object eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_return_login", "ui", ReturnLogin);
    if (eo != null) { eventlist.Add(eo); }
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_ui_unsubscribe", "ui", UnSubscribe);
    if (eo != null) eventlist.Add(eo);

    UIManager.Instance.HideWindowByName("Option");
  }

  // Update is called once per frame
  void Update()
  {

  }
  public void SwitchID()
  {
    DashFire.MyAction<int> fun = SwitchIDButtonWhich;
    DashFire.LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", DashFire.StrDictionaryProvider.Instance.GetDictString(18), null,
    DashFire.StrDictionaryProvider.Instance.GetDictString(140), DashFire.StrDictionaryProvider.Instance.GetDictString(157), fun, false);
  }
  void SwitchIDButtonWhich(int which)
  {
    if (which == 1) {
      DashFire.LogicSystem.QueueLogicAction(DashFire.WorldSystem.Instance.ReturnToLogin);
    }
  }
  void ReturnLogin()
  {
    GameObject go = GameObject.Find("GfxGameRoot");
    if (go != null) {
      GameLogic gameLogic = go.GetComponent<GameLogic>();
      if (gameLogic != null) gameLogic.RestartLocgic();
    }
  }
  public void Return()
  {
    UIManager.Instance.HideWindowByName("Option");
  }
}
