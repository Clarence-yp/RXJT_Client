using UnityEngine;
using System.Collections;

public class UIGuideStartGame : MonoBehaviour {
  
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
    //
    if (JoyStickInputProvider.JoyStickEnable) {
      JoyStickInputProvider.JoyStickEnable = false;
	}
	}
  void OnClick()
  {
    UIManager.Instance.HideWindowByName("GuideStartGame");
	UIManager.Instance.ShowWindowByName("HeroPanel");
    JoyStickInputProvider.JoyStickEnable = false;
	  UIManager.Instance.HideWindowByName("SkillBar");
    if (UIBeginnerGuide.Instance.onGuideGameStart != null) {
      UIBeginnerGuide.Instance.onGuideGameStart();
    }
  }
}
