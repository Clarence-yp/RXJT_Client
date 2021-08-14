using UnityEngine;
using DashFire;
using System.Collections;
using System.Collections.Generic;

public class GamePokeyButtonEvent : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {
    for (int i = 0; i < 6; ++i) {
      Transform tf = transform.Find("HeroEquip/Equipment/Slot" + i);
      if (tf != null) {
        UIEventListener.Get(tf.gameObject).onClick = SlotButtonClick;
      }
    }
  }

  // Update is called once per frame
  void Update()
  {

  }
  void SlotButtonClick(GameObject go)
  {
    Transform tf = transform.Find("RoleInfo");
    if (tf != null) {
      if (NGUITools.GetActive(tf.gameObject)) {
        return;
      }
    }

    if (go == null) return;
    int pos = 0;
    switch (go.transform.name) {
      case "Slot0":
        pos = 0;
        break;
      case "Slot1":
        pos = 1;
        break;
      case "Slot2":
        pos = 2;
        break;
      case "Slot3":
        pos = 3;
        break;
      case "Slot4":
        pos = 4;
        break;
      case "Slot5":
        pos = 5;
        break;
      default:
        return;
    }
    EquipmentInfo ei = GamePokeyManager.GetEquipmentInfo(pos);
    if (ei != null && ei.id != 0) {
      GameObject ipgo = UIManager.Instance.GetWindowGoByName("ItemProperty");
      if (ipgo != null && !NGUITools.GetActive(ipgo)) {
        ItemProperty ip = ipgo.GetComponent<ItemProperty>();
        ip.SetItemProperty(ei.id, pos, ei.level, ei.propertyid);
        UIManager.Instance.HideWindowByName("EntrancePanel");
        UIManager.Instance.ShowWindowByName("ItemProperty");
      }
    }
  }
  public void ArrangeButton()
  {
    //Debug.Log("ArrangeButton");
    Transform tf = gameObject.transform.Find("Container");
    if (tf != null) {
      GamePokeyManager gpm = tf.gameObject.GetComponent<GamePokeyManager>();
      if (gpm != null) {
        gpm.ArrangedItem();
      }
    }
  }
  public void BulksaleButton()
  {
    if (UIManager.CheckItemForDelete == null) return;
    List<int> li = new List<int>();
    li.Clear();
    List<int> propertys = new List<int>();
    propertys.Clear();
    int count = UIManager.CheckItemForDelete.Count;
    for (int i = 0; i < count; ++i) {
      GameObject go = UIManager.CheckItemForDelete[i];
      if (go != null) {
        ItemClick ic = go.GetComponent<ItemClick>();
        if (ic != null) {
          li.Add(ic.ID);
          propertys.Add(ic.PropertyId);
        }
      }
    }
    int[] sell = li.ToArray();
    int[] property = propertys.ToArray();
    GfxSystem.EventChannelForLogic.Publish("ge_discard_item", "lobby", sell, property);
  }
  public void ReturnButton()
  {
    UIManager.Instance.HideWindowByName("ItemProperty");
    UIManager.Instance.HideWindowByName("GamePokey");
    UIManager.Instance.ShowWindowByName("EntrancePanel");
  }
  public void DetailProperty()
  {
    Transform tf = transform.Find("RoleInfo");
    Transform tf1 = transform.Find("Container");
    Transform tf2 = transform.Find("ArrangeButton");
    Transform tf3 = transform.Find("BulksaleButton");
    if (tf != null) {
      if (NGUITools.GetActive(tf.gameObject)) {
        NGUITools.SetActive(tf.gameObject, false);
        if (tf1 != null && tf2 != null && tf3 != null) {
          NGUITools.SetActive(tf1.gameObject, true);
          NGUITools.SetActive(tf2.gameObject, true);
          NGUITools.SetActive(tf3.gameObject, true);
        }
      } else {
        NGUITools.SetActive(tf.gameObject, true);
        DashFire.GfxSystem.EventChannelForLogic.Publish("ge_request_role_property", "ui");
        if (tf1 != null && tf2 != null && tf3 != null) {
          NGUITools.SetActive(tf1.gameObject, false);
          NGUITools.SetActive(tf2.gameObject, false);
          NGUITools.SetActive(tf3.gameObject, false);
        }
      }
    }
  }
  public void BuyGold()
  {
    UIManager.Instance.ShowWindowByName("GoldBuy");
  }
}
