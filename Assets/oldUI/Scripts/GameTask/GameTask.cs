using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DashFire;

public class GameTask : MonoBehaviour
{
  private List<object> eventlist = new List<object>();
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
  void Start()
  {
    if (taskDic != null) { taskDic.Clear(); }
    if (finishtask != null) { finishtask.Clear(); }
    //if (awardtask != null) { awardtask.Clear(); }

    if (eventlist != null) { eventlist.Clear(); }
    object eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<int, DashFire.MissionOperationType, string>("ge_about_task", "task", GetTaskIdAndOperator);
    if (eo != null) { eventlist.Add(eo); }
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_ui_unsubscribe", "ui", UnSubscribe);
    if (eo != null) eventlist.Add(eo);


    DashFire.GfxSystem.EventChannelForLogic.Publish("ge_reload_missions", "lobby");
    UIManager.Instance.HideWindowByName("GameTask");
  }

  // Update is called once per frame
  void Update()
  {

  }

  private void GetTaskIdAndOperator(int id, DashFire.MissionOperationType oper, string schedule)
  {
    try {
      DashFire.MissionConfig missionconfig = DashFire.LogicSystem.GetMissionDataById(id);
      DashFire.RoleInfo ri = DashFire.LobbyClient.Instance.CurrentRole;
      if (missionconfig != null && ri != null && ri.Level < missionconfig.LevelLimit) {
        return;
      }
      switch (oper) {
        case DashFire.MissionOperationType.ADD:
          AddTask(id, schedule);
          break;
        case DashFire.MissionOperationType.FINISH:
          if (!finishtask.Contains(id)) {
            if (!taskDic.ContainsKey(id)) {
              AddTask(id, schedule);
            } else {
              SetTaskInfo(taskDic[id], id, schedule);
            }

            Transform tf = taskDic[id].transform.Find("New");
            if (tf != null) {
              UISprite us = tf.gameObject.GetComponent<UISprite>();
              if (us != null) {
                us.spriteName = "lingj";
              }
            }

            finishtask.Add(id);
          }
          //           GameObject go = UIManager.Instance.GetWindowGoByName("TaskAward");
          //           if (!NGUITools.GetActive(go)) {
          //             DashFire.GfxSystem.EventChannelForLogic.Publish("ge_read_finish", "lobby", id);
          //             if (go != null) {
          //               TaskAward ta = go.GetComponent<TaskAward>();
          //               if (ta != null) {
          //                 ta.SetAwardProperty(id);
          //               }
          //             }
          //             UIManager.Instance.HideWindowByName("GameTask");
          //             UIManager.Instance.ShowWindowByName("TaskAward");
          //           } else {
          //             awardtask.Add(id);
          //           }
          break;
        case DashFire.MissionOperationType.DELETE:
          DeleteTask(id);
          break;
        case DashFire.MissionOperationType.UPDATA:
          break;
        default:
          break;
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  private void AddTask(int taskid, string schedule)
  {
    if (!taskDic.ContainsKey(taskid)) {
      GameObject go = DashFire.ResourceSystem.GetSharedResource("UI/GameTask/TaskItem") as GameObject;
      if (go != null) {
        Transform tf = gameObject.transform.Find("Container/Scroll View/Grid");
        if (tf != null) {
          go = NGUITools.AddChild(tf.gameObject, go);
          if (go != null) {
            UIEventListener.Get(go).onClick = TaskItemClick;
            NGUITools.SetActive(go, true);
            taskDic.Add(taskid, go);
            SetTaskInfo(go, taskid, schedule, true);
          }
        }
        if (tf != null) {
          UIGrid ug = tf.gameObject.GetComponent<UIGrid>();
          if (ug != null) {
            ug.repositionNow = true;
          }
          if (tf.parent != null) {
            tf.parent.localPosition = new Vector3(0, 0, 0);
          }
        }
      }
    } else {
      SetTaskInfo(taskDic[taskid], taskid, schedule);
    }
  }

  private void DeleteTask(int id)
  {
    if (finishtask.Contains(id)) {
      finishtask.Remove(id);
      LogicSystem.EventChannelForGfx.Publish("ge_ui_award_finished", "ui",id);//通关副本按钮
      GfxSystem.PublishGfxEvent("ge_ui_connect_hint", "ui", false, false);
      UIManager.Instance.ShowWindowByName("TaskAward");
      GameObject god = UIManager.Instance.GetWindowGoByName("TaskAward");
      if (god != null) {
        TaskAward ta = god.GetComponent<TaskAward>();
        if (ta != null) {
          ta.SetAwardProperty(id);
        }
      }
    }
    if (taskDic.ContainsKey(id)) {
      if (taskDic[id] != null) {
        NGUITools.DestroyImmediate(taskDic[id]);
      }
      taskDic.Remove(id);
    }
    Transform tf = gameObject.transform.Find("Container/Scroll View/Grid");
    if (tf != null) {
      UIGrid ug = tf.gameObject.GetComponent<UIGrid>();
      if (ug != null) {
        ug.repositionNow = true;
      }
      if (tf.parent != null) {
        tf.parent.localPosition = new Vector3(0, 0, 0);
      }
    }
  }

  private void SetTaskInfo(GameObject go, int taskid, string schedule, bool sign = false)
  {
    DashFire.MissionConfig missionconfig = DashFire.LogicSystem.GetMissionDataById(taskid);
    if (missionconfig != null && go != null) {
      Transform tf = go.transform.Find("Name");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = missionconfig.Name + "：" + missionconfig.Description;
        }
      }
      tf = go.transform.Find("TaskType");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          if (missionconfig.MissionType == 1) {
            ul.text = DashFire.StrDictionaryProvider.Instance.GetDictString(124);
            go.transform.name = "0";
          }
          if (missionconfig.MissionType == 2) {
            ul.text = DashFire.StrDictionaryProvider.Instance.GetDictString(125);
            go.transform.name = "1";
          }
          if (missionconfig.MissionType == 3) {
            ul.text = DashFire.StrDictionaryProvider.Instance.GetDictString(148);
            go.transform.name = "2";
          }
        }
      }
      tf = go.transform.Find("Schedule");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          if (schedule != null) {
            ul.text = schedule;//DashFire.StrDictionaryProvider.Instance.Format(126, schedule);
          } else {
            ul.text = "";
          }
        }
      }
      tf = go.transform.Find("Award");
      if (sign) {
        SetAwardAndPosition(tf, missionconfig.DropId, missionconfig.Id);
      }
    }
    //     DashFire.RoleInfo ri = DashFire.LobbyClient.Instance.CurrentRole;
    //     if (ri != null && missionconfig != null) {
    //       if (ri.Level < missionconfig.LevelLimit) {
    //         if (NGUITools.GetActive(go)) {
    //           NGUITools.SetActive(go, false);
    //           Transform tf = gameObject.transform.Find("Container/Scroll View/Grid");
    //           if (tf != null) {
    //             UIGrid ug = tf.gameObject.GetComponent<UIGrid>();
    //             if (ug != null) {
    //               ug.repositionNow = true;
    //             }
    //           }
    //         }
    //       } else {
    //         if (!NGUITools.GetActive(go)) {
    //           NGUITools.SetActive(go, true);
    //           Transform tf = gameObject.transform.Find("Container/Scroll View/Grid");
    //           if (tf != null) {
    //             UIGrid ug = tf.gameObject.GetComponent<UIGrid>();
    //             if (ug != null) {
    //               ug.repositionNow = true;
    //             }
    //           }
    //         }
    //       }
    //     }
  }

  private void SetAwardAndPosition(Transform tf, int dropid, int missionId)
  {
    DashFire.Data_SceneDropOut dsdo = DashFire.SceneConfigProvider.Instance.GetSceneDropOutById(dropid);
    Vector3 pos = new Vector3(80.0f, 0.0f, 0.0f);
    if (tf != null && dsdo != null) {
      if (dsdo.m_GoldSum > 0) {
        Transform tt = tf.Find("Money");
        if (tt != null) {
          //NGUITools.SetActive(tt.gameObject, true);
          pos = tt.localPosition;
          pos = new Vector3(pos.x + 55, pos.y, 0.0f);

          tt = tt.Find("Label");
          if (tt != null) {
            UILabel ul = tt.gameObject.GetComponent<UILabel>();
            if (ul != null) {
              ul.text = "X" + dsdo.m_GoldSum;
            }

            pos = new Vector3(pos.x + ul.localSize.x, pos.y, 0.0f);
          }
        }
      } else {
        Transform tt = tf.Find("Money");
        if (tt != null) {
          NGUITools.SetActive(tt.gameObject, false);
        }
      }
      if (dsdo.m_Exp > 0) {
        Transform tt = tf.Find("Exp");
        RoleInfo roleInfo = LobbyClient.Instance.CurrentRole;
        if (tt != null && roleInfo != null) {
          //NGUITools.SetActive(tt.gameObject, true);
          tt.localPosition = pos;
          pos = tt.localPosition;
          pos = new Vector3(pos.x + 55, pos.y, 0.0f);

          tt = tt.Find("Label");
          if (tt != null) {
            UILabel ul = tt.gameObject.GetComponent<UILabel>();
            if (ul != null) {
              ul.text = "X" + roleInfo.GetMissionStateInfo().GetMissionsExpReward(missionId, roleInfo.Level);
            }

            pos = new Vector3(pos.x + ul.localSize.x, pos.y, 0.0f);
          }
        }
      } else {
        Transform tt = tf.Find("Exp");
        if (tt != null) {
          NGUITools.SetActive(tt.gameObject, false);
        }
      }
      if (dsdo.m_Diamond > 0) {
        Transform tt = tf.Find("Diamond");
        if (tt != null) {
          //NGUITools.SetActive(tt.gameObject, true);
          tt.localPosition = pos;
          pos = tt.localPosition;
          pos = new Vector3(pos.x + 55, pos.y, 0.0f);

          tt = tt.Find("Label");
          if (tt != null) {
            UILabel ul = tt.gameObject.GetComponent<UILabel>();
            if (ul != null) {
              ul.text = "X" + dsdo.m_Diamond;
            }

            pos = new Vector3(pos.x + ul.localSize.x, pos.y, 0.0f);
          }
        }
      } else {
        Transform tt = tf.Find("Diamond");
        if (tt != null) {
          NGUITools.SetActive(tt.gameObject, false);
        }
      }
      int count = dsdo.m_ItemIdList.Count;
      pos = new Vector3(pos.x + 30, pos.y, 0f);
      for (int i = 0; i < count; ++i) {
        DashFire.ItemConfig ic = DashFire.ItemConfigProvider.Instance.GetDataById(dsdo.m_ItemIdList[i]);
        if (ic != null) {
          GameObject go = DashFire.ResourceSystem.GetSharedResource("UI/GameTask/Item") as GameObject;
          if (go != null) {
            go = NGUITools.AddChild(tf.gameObject, go);
            if (go != null) {
              go.transform.localPosition = pos;
              pos = go.transform.localPosition;
              pos = new Vector3(pos.x + 65, pos.y, 0.0f);
              Texture utt = GamePokeyManager.GetTextureByPicName(ic.m_ItemTrueName);
              UITexture ut = go.GetComponent<UITexture>();
              if (ut != null) {
                if (utt != null) {
                  ut.mainTexture = utt;
                } else {
                  DashFire.ResLoadAsyncHandler.LoadAsyncItem(ic.m_ItemTrueName, ut);
                }
              }
              Transform tt = go.transform.Find("Frame");
              if (tt != null) {
                UISprite us = tt.gameObject.GetComponent<UISprite>();
                if (us != null) {
                  us.spriteName = "EquipFrame" + ic.m_PropertyRank;
                }
              }
              tt = go.transform.Find("Label");
              if (tt != null) {
                UILabel ul = tt.gameObject.GetComponent<UILabel>();
                if (ul != null) {
                  ul.text = "X" + dsdo.m_ItemCountList[i];
                }
                pos = new Vector3(pos.x + ul.localSize.x, pos.y, 0.0f);
              }
            }
          }
        }
      }
    }
  }

  private void TaskItemClick(GameObject go)
  {
    if (go != null) {
      foreach (int id in finishtask) {
        if (taskDic.ContainsKey(id)) {
          GameObject godic = taskDic[id];
          if (godic != null && godic == go) {
            //发送已读消息
            DashFire.GfxSystem.EventChannelForLogic.Publish("ge_read_finish", "lobby", id);
            UIManager.Instance.HideWindowByName("GameTask");
            GfxSystem.PublishGfxEvent("ge_ui_connect_hint", "ui", false, true);
            return;
          }
        }
      }
    }
    UIManager.Instance.HideWindowByName("GameTask");
    GameObject goc = UIManager.Instance.GetWindowGoByName("SceneSelect");
    if (goc != null) {
      UISceneSelect uss = goc.GetComponent<UISceneSelect>();
      if (uss != null) {
        foreach (int key in taskDic.Keys) {
          if (taskDic[key] == go) {
            DashFire.MissionConfig missionconfig = DashFire.LogicSystem.GetMissionDataById(key);
            if (missionconfig != null) {
              if (missionconfig.SceneId == 3001) {
                UIManager.Instance.ShowWindowByName("Mars");
              } else if (missionconfig.SceneId == 4001) {
                UIManager.Instance.ShowWindowByName("cangbaotu");
              } else {
                uss.StartChapter(missionconfig.SceneId);
              }
            }
            break;
          }
        }
      }
    }
  }

  public void CloseGameTask()
  {
    UIManager.Instance.HideWindowByName("GameTask");
    JoyStickInputProvider.JoyStickEnable = true;
  }
  private List<int> finishtask = new List<int>();
  private Dictionary<int, GameObject> taskDic = new Dictionary<int, GameObject>();
  //static public List<int> awardtask = new List<int>();
}
