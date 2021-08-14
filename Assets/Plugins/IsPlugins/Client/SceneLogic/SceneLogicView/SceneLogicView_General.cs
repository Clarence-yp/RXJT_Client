using System;
using System.Collections.Generic;

namespace DashFire
{
  public class SceneLogicView_General
  {
    public SceneLogicView_General()
    {
      AbstractSceneLogic.OnSceneLogicSendStoryMessage += this.OnSceneLogicSendStoryMessage;
    }

    public void OnSceneLogicSendStoryMessage(SceneLogicInfo info, string msgId, object[] args)
    {
      if (WorldSystem.Instance.IsPveScene() || WorldSystem.Instance.IsPureClientScene()) {
        ClientStorySystem.Instance.SendMessage(msgId, args);
      }
    }
  }
}
