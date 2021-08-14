using UnityEngine;
using System.Collections;

public class UIChargePanel : MonoBehaviour {

  public UILabel noticeLabel = null;
  public UIChargeMoney chargeMoney = null;
	// Use this for initialization
	void Start () {
    SetNotice("1");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
  
  //通过读配置表来设置文本信息
  public void SetNotice(string msg)
  {
    msg = "好消息：\n    [ffff00]首次充值将获得2000钻[-]";
    if (noticeLabel == null)
      return;
    noticeLabel.text = msg;
  }

}
