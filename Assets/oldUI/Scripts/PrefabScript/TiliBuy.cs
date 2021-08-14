using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TiliBuy : MonoBehaviour
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
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<DashFire.Network.GeneralOperationResult>("ge_buy_stamina", "stamina", Buyresult);
    if (eo != null) eventlist.Add(eo);
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<DashFire.Network.GeneralOperationResult>("ge_startgame_failure", "game", StartGameFailure);
    if (eo != null) eventlist.Add(eo);

    SetBuyStaminaInfo();
    UIManager.Instance.HideWindowByName("TiliBuy");
  }

  // Update is called once per frame
  void Update()
  {

  }

  void SetBuyStaminaInfo()
  {
    DashFire.RoleInfo ri = DashFire.LobbyClient.Instance.CurrentRole;
    if (ri != null) {
      Transform tf = transform.Find("bk/tip");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          DashFire.VipConfig config_data = DashFire.VipConfigProvider.Instance.GetDataById(ri.Vip);
          ul.text = DashFire.StrDictionaryProvider.Instance.Format(146, ri.BuyStaminaCount, null == config_data ? ri.Vip + 1 : config_data.m_Stamina);
        }
      }
      DashFire.BuyStaminaConfig bsc = DashFire.BuyStaminaConfigProvider.Instance.GetDataById(ri.BuyStaminaCount + 1);
      if (bsc != null) {
        tf = transform.Find("bk/zuan/mount");
        if (tf != null) {
          UILabel ul = tf.gameObject.GetComponent<UILabel>();
          if (ul != null) {
            ul.text = bsc.m_CostGold.ToString();
          }
        }
        tf = transform.Find("bk/money/mount");
        if (tf != null) {
          UILabel ul = tf.gameObject.GetComponent<UILabel>();
          if (ul != null) {
            ul.text = bsc.m_GainStamina.ToString();
          }
        }
      }
    }
  }

  private void StartGameFailure(DashFire.Network.GeneralOperationResult result)
  {
    try {
      if (result == DashFire.Network.GeneralOperationResult.LC_Failure_CostError) {
        DashFire.LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", DashFire.StrDictionaryProvider.Instance.GetDictString(306),
        DashFire.StrDictionaryProvider.Instance.GetDictString(140), null, null, null, false);
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  void Buyresult(DashFire.Network.GeneralOperationResult result)
  {
    try {
      if (result == DashFire.Network.GeneralOperationResult.LC_Succeed) {
        SetBuyStaminaInfo();
      } else {
        int i = 0;
        if (result == DashFire.Network.GeneralOperationResult.LC_Failure_CostError) {
          i = 123;
        }
        if (result == DashFire.Network.GeneralOperationResult.LC_Failure_Overflow) {
          i = 150;
        }
        DashFire.LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", DashFire.StrDictionaryProvider.Instance.GetDictString(i),
        DashFire.StrDictionaryProvider.Instance.GetDictString(140), null, null, null, false);
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  public void ButtonForYes()
  {
    DashFire.GfxSystem.EventChannelForLogic.Publish("ge_buy_stamina", "lobby");
  }

  public void CloseWindow()
  {
    UIManager.Instance.HideWindowByName("TiliBuy");
  }
}
