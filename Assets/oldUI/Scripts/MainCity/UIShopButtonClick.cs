
using UnityEngine;
using System.Collections;

public class UIShopButtonClick : MonoBehaviour {

	// Use this for initialization
	void Start () {  
	}
	
	// Update is called once per frame
	void Update () {
	
	}
  public void OnClick()
  {
    GameObject go = UIManager.Instance.GetWindowGoByName("GoldBuy");
    if (go != null) {
      if (NGUITools.GetActive(go)) {
        UIManager.Instance.HideWindowByName("GoldBuy");
      } else {
        UIManager.Instance.ShowWindowByName("GoldBuy");
      }
    }
  }
}
