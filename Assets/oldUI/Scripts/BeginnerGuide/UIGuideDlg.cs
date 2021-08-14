using UnityEngine;
using System;
using System.Collections;
using DashFire;

public class UIGuideDlg : MonoBehaviour {

  public UILabel lblDesc = null;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
  public void SetDescription(int descId)
  {
    string chn_des = StrDictionaryProvider.Instance.GetDictString(descId);
    if (lblDesc != null)
      lblDesc.text = chn_des;
  }
}
