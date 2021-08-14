using UnityEngine;
using System.Collections;
using DashFire;

public class LoadingProgressBar : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {
  }

  // Update is called once per frame
  void LateUpdate()
  {
    if (JoyStickInputProvider.JoyStickEnable) {
      JoyStickInputProvider.JoyStickEnable = false;
    }
    UISlider us = this.GetComponent<UISlider>();
    float progressvalue = DashFire.LogicSystem.GetLoadingProgress();
    if (us != null) {
      us.value = progressvalue;
    }
    Transform tipObj = gameObject.transform.Find("Tip");
    if (tipObj != null) {
      UILabel tipLabel = tipObj.gameObject.GetComponent<UILabel>();
      if (tipLabel != null) {
        tipLabel.text = DashFire.LogicSystem.GetLoadingTip();
      }
    }
    if (progressvalue >= 0.9999f) {
      Transform tf = gameObject.transform.Find("Background/Panel/Icon");
      if (tf != null) {
        NGUITools.SetActive(tf.gameObject, false);
      }
    }

    string versionInfo = DashFire.LogicSystem.GetVersionInfo();
    Transform versionInfoObj = gameObject.transform.Find("VersionInfo");
    if (versionInfoObj != null && !string.IsNullOrEmpty(versionInfo)) {
      UILabel versionInfoLabel = versionInfoObj.gameObject.GetComponent<UILabel>();
      if (versionInfoLabel != null) {
        versionInfoLabel.text = versionInfo;
      }
    }
    if (!string.IsNullOrEmpty(versionInfo)) {
      us.value = progressvalue;
    }
  }
  void Update()
  {
    if (JoyStickInputProvider.JoyStickEnable) {
      JoyStickInputProvider.JoyStickEnable = false;
    }
    if (sign1 && DashFire.LogicSystem.GetLoadingProgress() > 0) {
      sign1 = false;
      Transform tf = gameObject.transform.Find("Background/Panel/Icon");
      if (tf != null) {
        NGUITools.SetActive(tf.gameObject, true);
      }
    }
    if (sign3) {
      if (!sign2) {
        time += RealTime.deltaTime;
        if (time >= 2.0f) {
          DestoryLoading();
        } else {
          if (GameObject.FindGameObjectsWithTag("Player").Length > 0) {
            //DestoryLoading();
            sign3 = false;
            time = 0f;
          }
        }
      }
    } else {
      time += RealTime.deltaTime;
      if (time >= 2.0f) {
        DestoryLoading();
      }
    }
  }
  void EndLoading()
  {
    sign2 = false;
    time = 0.0f;
  }
  void DestoryLoading()
  {
    if (InputType.Joystick == DFMUiRoot.InputMode) {
      JoyStickInputProvider.JoyStickEnable = UIManager.Instance.IsUIVisible;
    }
    GameObject go = UIManager.Instance.GetWindowGoByName("cangbaotu");
    if (go != null && NGUITools.GetActive(go)) {
      JoyStickInputProvider.JoyStickEnable = false;
    }
    sign1 = true;
    sign2 = true;
    sign3 = true;
    time = 0f;
    NGUITools.DestroyImmediate(this.transform.parent.gameObject);
  }
  private bool sign1 = true;
  private bool sign2 = true;
  private bool sign3 = true;
  private float time = 0f;
}
