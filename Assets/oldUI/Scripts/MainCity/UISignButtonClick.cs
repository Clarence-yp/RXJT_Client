using UnityEngine;
using System.Collections;
using DashFire;

public class UISignButtonClick : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
  void OnClick()
  {
    //Debug.Log("Open ths sign panel.");
    if (DFMUiRoot.InputMode == InputType.Joystick) {
      DFMUiRoot.InputMode = InputType.Touch;
    } else {
      DFMUiRoot.InputMode = InputType.Joystick;
    }
    GameObject go = UIManager.Instance.GetWindowGoByName("TiliBuy");
    if (go != null) {
      if (NGUITools.GetActive(go)) {
        UIManager.Instance.HideWindowByName("TiliBuy");
      } else {
        UIManager.Instance.ShowWindowByName("TiliBuy");
      }
    }
  }
}
