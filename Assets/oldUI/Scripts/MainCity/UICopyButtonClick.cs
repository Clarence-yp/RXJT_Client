using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DashFire;

public class UICopyButtonClick : MonoBehaviour {

  public GameObject goAwardHint = null;
  private List<object> eventlist = new List<object>();
  private bool m_IsCopy = true;
  public void UnSubscribe()
  {
    try {
      if (eventlist != null) {
        foreach (object eo in eventlist) {
          if (eo != null) {
            DashFire.LogicSystem.EventChannelForGfx.Unsubscribe(eo);
          }
        }
      }
      eventlist.Clear();
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
	// Use this for initialization
	void Start () {
    object eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<int, DashFire.MissionOperationType, string>("ge_about_task", "task", OnTaskFinished);
    if (eo != null) {eventlist.Add(eo); }
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<int>("ge_ui_award_finished", "ui", OnAwardFinished);
    if (eo != null) eventlist.Add(eo);
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_ui_unsubscribe", "ui", UnSubscribe);
    if (eo != null) eventlist.Add(eo);
  }
	
	// Update is called once per frame
	void Update () {
	
	}
  void OnClick()
  {
    if (m_IsCopy) {
      UIManager.Instance.ToggleWindowVisible("SceneSelect");
    } else {
      UIManager.Instance.ToggleWindowVisible("GameTask");
    }
  }
  private void OnTaskFinished(int taskId, DashFire.MissionOperationType opType, string schedule)
  {
    try {
      DashFire.MissionConfig missionconfig = DashFire.LogicSystem.GetMissionDataById(taskId);
      DashFire.RoleInfo ri = DashFire.LobbyClient.Instance.CurrentRole;
      if (missionconfig != null && ri != null && ri.Level < missionconfig.LevelLimit) {
        return;
      }
      if (opType == DashFire.MissionOperationType.FINISH) {
        if (missionconfig == null) return;
        if (missionconfig.MissionType == 1) {
          //主线任务完成
          if (goAwardHint != null) NGUITools.SetActive(goAwardHint, true);
          m_IsCopy = false;
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private void OnAwardFinished(int taskId)
  {
    DashFire.MissionConfig missionconfig = DashFire.LogicSystem.GetMissionDataById(taskId);
    if (missionconfig == null) return;
    if (missionconfig.MissionType == 1) {
      if (goAwardHint != null) {
        NGUITools.SetActive(goAwardHint, false);
        m_IsCopy = true;
      }
    }
  }
}
