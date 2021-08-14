using UnityEngine;
using System.Collections;
using DashFire;

public class Quit : MonoBehaviour
{
  // Use this for initialization
  internal void Start()
  {

  }
  // Update is called once per frame
  internal void Update()
  {

  }

  public void OnClick()
  {
    LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", Dict.Get(8), null, Dict.Get(4), Dict.Get(9), (MyAction<int>)((int btn) => {
      if (btn == 1) {
        LogicSystem.PublishLogicEvent("ge_quit_battle", "lobby");
      }
    }), false);
  }
}
