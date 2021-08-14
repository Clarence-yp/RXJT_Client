using UnityEngine;
using DashFire;
using System.Collections;

public class UIReturnToMaincity : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnClick(){
    LogicSystem.SendStoryMessage("returntomaincity");
   
  }
  public int offset = 10;
}
