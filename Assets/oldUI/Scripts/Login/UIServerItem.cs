using UnityEngine;
using DashFire;
using System.Collections;

public class UIServerItem : MonoBehaviour {

  public UILabel lblId = null;
  public UILabel lblName = null;
  public UILabel lblState = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	  
	}
  void OnClick()
  {
    UIServerSelect serverSelect = NGUITools.FindInParents<UIServerSelect>(this.gameObject);
    if (serverSelect != null) serverSelect.TweenUpwards(ServerId);
  }
  public void SetServerInfo(int id,string serverName, string state)
  {
    ServerId = id;
    if (lblId != null) {
      StrDictionary strDic = DashFire.StrDictionaryProvider.Instance.GetDataById(201);
      if (strDic != null) {
        lblId.text = "" + id + strDic.m_String;
      } else {
        lblId.text = "" + id;
      }
    }
    if (lblName != null) {
      lblName.text =""+ serverName;
    }
    if (lblState != null) {
      lblState.text = state;
    }
  }

  public int ServerId
  {
    get { return m_ServerId; }
    set { m_ServerId = value; }
  }
  private int m_ServerId = 0;
}
