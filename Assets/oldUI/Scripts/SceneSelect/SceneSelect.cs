using UnityEngine;
using System.Collections;

public class SceneSelect : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
  public void OnPveBtnClick()
  {
    ChangeToScene(1);
  }
  public void OnPvpBtnClick()
  {
    ChangeToScene(2);
  }
  public void OnMpveBtnClick()
  {
    ChangeToScene(3);
  }
  public void ChangeToScene(int sceneId)
  {
     DashFire.LogicSystem.PublishLogicEvent("ge_select_scene", "lobby", sceneId);
  }
}
