using UnityEngine;
using System.Collections;

public class UISceneIntroduceSlot : MonoBehaviour {

  private int m_ItemId = -1;
  // Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
  void OnClick()
  {
    GameObject ipgo = UIManager.Instance.GetWindowGoByName("ItemProperty");
    if (ipgo != null && !NGUITools.GetActive(ipgo)) {
      ItemProperty ip = ipgo.GetComponent<ItemProperty>();
      ip.ShowItemProperty(m_ItemId, 1);
      //UIManager.Instance.HideWindowByName("EntrancePanel");
      UIManager.Instance.ShowWindowByName("ItemProperty");
    }
  }
  public void SetId(int itemId)
  {
    m_ItemId = itemId;
  }
}
