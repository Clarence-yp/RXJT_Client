using UnityEngine;
using System.Collections;
using DashFire;

public class LoadingSword : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {
    signforfake = false;
    time = 0.0f;
  }

  // Update is called once per frame
  void Update()
  {
    if (signforfake) {
      float dt = RealTime.deltaTime;
      if (dt <= 0.5f) {
        time += dt;
      }
      if (time <= 2.0f) {
        UISlider us = this.GetComponent<UISlider>();
        if (us != null) {
          us.value = time / 1.8f;
        }
      } else {
        signforfake = false;
        time = 0.0f;
        UISlider us = this.GetComponent<UISlider>();
        if (us != null) {
          us.value = 0.0f;
        }
        NGUITools.Destroy(this.transform.parent.gameObject);
      }
    }
  }
  void EndLoading()
  {
    signforfake = true;
    time = 0.0f;
    UISlider us = this.GetComponent<UISlider>();
    if (us != null) {
      us.value = 0.0f;
    }
    GameObject parentGO = GameObject.FindGameObjectWithTag("UI");
    if (parentGO != null) {
      Transform parentTransform = parentGO.transform;  //剧情对话框的父Transform
      Transform t = parentTransform.Find("StoryDlg");
      if (t != null) {
        NGUITools.SetActive(t.gameObject, false);
      }
    }
  }
  private bool signforfake = false;
  private float time = 0.0f;
}
