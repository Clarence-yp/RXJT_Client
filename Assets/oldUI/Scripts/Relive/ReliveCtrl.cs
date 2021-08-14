using UnityEngine;
using System.Collections;

public class ReliveCtrl{

  
  public void Init(GameObject father)
  {
    m_FatherGo = father;
  }
  public void ShowReliveUi()
  {
    GameObject go = DashFire.ResourceSystem.GetSharedResource("UI/Relive") as GameObject;
    if (null != go) {
      go = NGUITools.AddChild(m_FatherGo, go);
    }
  }

  static private ReliveCtrl m_Instance = new ReliveCtrl();
  static public  ReliveCtrl Instance
  {
    get { return m_Instance;}
  }
  private GameObject m_FatherGo = null;

}
