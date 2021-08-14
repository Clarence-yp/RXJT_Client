using UnityEngine;
using System.Collections;

public class CreateSelectCreateHero : MonoBehaviour {

  // Use this for initialization
  void Start()
  {
    GameObject go = DashFire.ResourceSystem.GetSharedResource("UI/SelectCreateHero") as GameObject;
    if (go != null) {
      MonoBehaviour.Instantiate(go);
    }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
