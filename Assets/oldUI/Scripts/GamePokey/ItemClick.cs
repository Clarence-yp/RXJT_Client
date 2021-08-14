using UnityEngine;
using System.Collections;

public class ItemClick : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }
  public void OnButtonClick()
  {
    GameObject gogo = UIManager.Instance.GetWindowGoByName("GamePokey");
    if (gogo != null) {
      Transform tf = gogo.transform.Find("RoleInfo");
      if (tf != null) {
        if (NGUITools.GetActive(tf.gameObject)) {
          return;
        }
      }
    }
    DashFire.ItemConfig itemconfig = DashFire.LogicSystem.GetItemDataById(ID);
    if (itemconfig != null) {
      GameObject go = UIManager.Instance.GetWindowGoByName("GamePokey");
      if (go != null) {
        EquipmentInfo ei = GamePokeyManager.GetEquipmentInfo(itemconfig.m_WearParts);
        if (ei != null) {
          go = UIManager.Instance.GetWindowGoByName("ItemProperty");
          if (go != null && !NGUITools.GetActive(go)) {
            ItemProperty ip = go.GetComponent<ItemProperty>();
            if (ip != null) {
              ip.Compare(ei.id, ei.level, ei.propertyid, ID, ei.level, PropertyId, itemconfig.m_WearParts);
              UIManager.Instance.ShowWindowByName("ItemProperty");
            }
          }
        }
      }
    }
  }
  public void CheckBoxValueChange()
  {
    if (UIManager.CheckItemForDelete == null) return;

    Transform tf = transform.Find("CheckBox");
    if (tf != null) {
      UIToggle ut = tf.gameObject.GetComponent<UIToggle>();
      if (ut != null) {
        if (ut.value) {
          UIManager.CheckItemForDelete.Add(gameObject);
        } else {
          UIManager.CheckItemForDelete.Remove(gameObject);
        }
      }
    }
  }
  //   public int PropertyId
  //   {
  //     get { return item_property; }
  //     set { item_property = value; }
  //   }
  public int ID = 0;
  public int PropertyId = 0;
}
