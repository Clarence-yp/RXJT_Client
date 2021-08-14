using UnityEngine;
using System.Collections;

public class UIChargeMoney : MonoBehaviour {

  public GameObject item = null;
  public GameObject gridGo = null;
	// Use this for initialization
	void Start () {
    InitChargeMoneyScrollBar();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
  //读取数据库中的数据初始化充值选项
  public void InitChargeMoneyScrollBar()
  {
    if (item == null || gridGo ==null) {
      Debug.LogError("!!Did not initialize Item or fatherGo.");
      return;
    }
    for (int index = 0; index < 5; index++) {
      NGUITools.AddChild(gridGo, item);
    }
    UIGrid grid = gridGo.GetComponent<UIGrid>();
    if (null != grid)
      grid.Reposition();
  }


}
