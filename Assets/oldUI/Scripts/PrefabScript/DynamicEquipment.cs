using UnityEngine;
using DashFire;
using System;
using System.Collections;
using System.Collections.Generic;

public class DynamicEquipment : MonoBehaviour
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
    object eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<int, int, int, int, DashFire.Network.GeneralOperationResult>("ge_fiton_equipment", "equipment", HeroPutOnEquipment);
    if (eo != null) { eventlist.Add(eo); }

    UIManager.Instance.HideWindowByName("DynamicEquipment");
  }

  // Update is called once per frame
  void Update()
  {

  }
  public void SetEquipment(ChangeNewEquip cne)
  {
    if (cne != null) {
      id = cne.id;
      propertyid = cne.propertyid;
      ItemConfig ic = ItemConfigProvider.Instance.GetDataById(id);
      if (ic != null) {
        Transform tf = transform.Find("bc/goods/Texture");
        if (tf != null) {
          UITexture ut = tf.gameObject.GetComponent<UITexture>();
          if (ut != null) {
            Texture tt = GamePokeyManager.GetTextureByPicName(ic.m_ItemTrueName);
            if (tt != null) {
              ut.mainTexture = tt;
            } else {
              DashFire.ResLoadAsyncHandler.LoadAsyncItem(ic.m_ItemTrueName, ut);
            }
          }
        }
        tf = transform.Find("bc/goods");
        if (tf != null) {
          UISprite us = tf.gameObject.GetComponent<UISprite>();
          if (us != null) {
            us.spriteName = "EquipFrame" + ic.m_PropertyRank;
          }
        }
      }
      UIManager.Instance.ShowWindowByName("DynamicEquipment");
    }
  }
  void DeleteNowCheckAnother()
  {
    ItemConfig ic = ItemConfigProvider.Instance.GetDataById(id);
    if (GamePokeyManager.changeitemDic != null&&ic!=null) {
      if (GamePokeyManager.changeitemDic.ContainsKey(ic.m_WearParts)) {
        GamePokeyManager.changeitemDic.Remove(ic.m_WearParts);
      }
      foreach (int pos in GamePokeyManager.changeitemDic.Keys) {
        SetEquipment(GamePokeyManager.changeitemDic[pos]);
        break;
      }
    }
  }
  void HeroPutOnEquipment(int id, int pos, int itemLevel, int itemRandomProperty, DashFire.Network.GeneralOperationResult result)
  {
    try {
      UIManager.Instance.HideWindowByName("DynamicEquipment");
      DeleteNowCheckAnother();
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  public void WearButton()
  {
    DashFire.ItemConfig itemconfig = DashFire.LogicSystem.GetItemDataById(id);
    if (itemconfig != null) {
      DashFire.GfxSystem.EventChannelForLogic.Publish("ge_mount_equipment", "lobby", id, propertyid, itemconfig.m_WearParts);
    }
    UIManager.Instance.HideWindowByName("DynamicEquipment");
  }
  public void CloseButton()
  {
    UIManager.Instance.HideWindowByName("DynamicEquipment");
    DeleteNowCheckAnother();
  }
  public void ItemButton()
  {
    GameObject go = UIManager.Instance.GetWindowGoByName("GamePokey");
    if (go != null) {
      if (!NGUITools.GetActive(go)) {
        DashFire.ItemConfig itemconfig = DashFire.LogicSystem.GetItemDataById(id);
        if (itemconfig != null) {
          EquipmentInfo ei = GamePokeyManager.GetEquipmentInfo(itemconfig.m_WearParts);
          if (ei != null) {
            go = UIManager.Instance.GetWindowGoByName("ItemProperty");
            if (go != null && !NGUITools.GetActive(go)) {
              ItemProperty ip = go.GetComponent<ItemProperty>();
              if (ip != null) {
                ip.Compare(ei.id, ei.level, ei.propertyid, id, ei.level, propertyid, itemconfig.m_WearParts);
                UIManager.Instance.ShowWindowByName("ItemProperty");
              }
            }
          }
        }
      } else {
        UIManager.Instance.HideWindowByName("ItemProperty");
      }
    }
  }
  private int id = 0;
  private int propertyid = 0;
}
