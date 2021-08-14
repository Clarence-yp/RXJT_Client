using UnityEngine;
using System.Collections;

public class ReturnBack : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {
  }

  // Update is called once per frame
  void Update()
  {

  }
  public void OnPress()
  {
    /*DashFire.LogicSystem.PublishLogicEvent("ge_change_scene", "game", m_SceneId);*/
    NGUITools.DestroyImmediate(this.gameObject);
  }

  public void BackToBegin()
  {
    /*DashFire.LogicSystem.PublishLogicEvent("ge_change_scene", "game", m_SceneId);*/
  }
  public int m_SceneId = 6;
}
