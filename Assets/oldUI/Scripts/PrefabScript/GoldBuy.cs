using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GoldBuy : MonoBehaviour
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
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<DashFire.Network.GeneralOperationResult>("ge_midas_touch", "midastouch", Buyresult);
    if (eo != null) eventlist.Add(eo);

    SetGoldBuyInfo();
    UIManager.Instance.HideWindowByName("GoldBuy");
  }

  // Update is called once per frame
  void Update()
  {

  }
  void SetGoldBuyInfo()
  {
    DashFire.RoleInfo ri = DashFire.LobbyClient.Instance.CurrentRole;
    if (ri != null) {
      Transform tf = transform.Find("bk/tip");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          DashFire.VipConfig config_data = DashFire.VipConfigProvider.Instance.GetDataById(ri.Vip);
          ul.text = DashFire.StrDictionaryProvider.Instance.Format(145, ri.BuyMoneyCount, null == config_data ? (ri.Vip > 0 ? ri.Vip * 10 : 10) : config_data.m_BuyMoney);
        }
      }
      DashFire.BuyMoneyConfig bmc = DashFire.BuyMoneyConfigProvider.Instance.GetDataById(ri.BuyMoneyCount + 1);
      if (bmc != null) {
        tf = transform.Find("bk/money/mount");
        if (tf != null) {
          UILabel ul = tf.gameObject.GetComponent<UILabel>();
          if (ul != null) {
            ul.text = bmc.m_GainMoney.ToString();
          }
        }
        tf = transform.Find("bk/zuan/mount");
        if (tf != null) {
          UILabel ul = tf.gameObject.GetComponent<UILabel>();
          if (ul != null) {
            ul.text = bmc.m_CostGold.ToString();
          }
        }
      }
    }
  }
  public void BuyOne()
  {
    DashFire.GfxSystem.EventChannelForLogic.Publish("ge_midas_touch", "lobby");
  }
  public void BuyTen()
  {
    StartCoroutine(BuyTenDelay());
  }
  public IEnumerator BuyTenDelay()
  {
    int i = 0;
    while (i < 10) {
      BuyOne();
      ++i;
      yield return new WaitForSeconds(0.1f);
    }
  }
  public void CloseWindow()
  {
    UIManager.Instance.HideWindowByName("GoldBuy");
  }
  void Buyresult(DashFire.Network.GeneralOperationResult result)
  {
    try {
      if (result == DashFire.Network.GeneralOperationResult.LC_Succeed) {
        SetGoldBuyInfo();
        DashFire.RoleInfo ri = DashFire.LobbyClient.Instance.CurrentRole;
        if (ri != null) {
          DashFire.BuyMoneyConfig bmc = DashFire.BuyMoneyConfigProvider.Instance.GetDataById(ri.BuyMoneyCount);
          if (bmc != null) {
            BuyMoneyTip(bmc.m_GainMoney);
          }
        }
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
  void BuyMoneyTip(int num)
  {
    string path = UIManager.Instance.GetPathByName("GoldBuyDlg");
    UnityEngine.Object obj = DashFire.ResourceSystem.NewObject(path, 5f);
    GameObject go = obj as GameObject;
    if (null != go) {
      Transform tf = go.transform.Find("Label/value");
      if (tf != null) {
        UILabel bloodPanel = tf.gameObject.GetComponent<UILabel>();
        if (null != bloodPanel) {
          bloodPanel.text = num.ToString();
        }
      }
      GameObject cube = null;
      tf = transform.parent.Find("ScreenTipPanel");
      if (tf != null) {
        cube = NGUITools.AddChild(tf.gameObject, obj);
      }
      if (cube != null) {
        BloodAnimation ba = cube.GetComponent<BloodAnimation>();
        if (ba != null) {
          ba.PlayAnimation();
        }
        NGUITools.SetActive(cube, true);
      }
    }
  }
}
