using UnityEngine;
using System.Collections;
using DashFire;

public class GunChange : MonoBehaviour {
  void Start() {
    NGUITools.SetActive(this.gameObject, false);
  }
  void Update() {
  }
  void OnClick() {
  }
  public void SetActive(bool active) {
    NGUITools.SetActive(this.gameObject, active);
  }
}
