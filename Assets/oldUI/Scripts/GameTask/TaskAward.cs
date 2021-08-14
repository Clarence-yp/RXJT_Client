using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TaskAward : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }
  public void CloseTaskAward()
  {
    if (sign == TaskAwardOpenForWindow.W_GameTask) {
      //GameTask.awardtask.Remove(awardid);
      //if (GameTask.awardtask.Count == 0) {
      UIManager.Instance.HideWindowByName("TaskAward");
      UIManager.Instance.ShowWindowByName("GameTask");
      //} else {
      //SetAwardProperty(GameTask.awardtask[0]);
      //}
    }
    if (sign == TaskAwardOpenForWindow.W_MermanKing) {
      UIManager.Instance.HideWindowByName("TaskAward");
    }
    if (sign == TaskAwardOpenForWindow.W_SellGain) {
      UIManager.Instance.HideWindowByName("TaskAward");
    }
  }
  public void SetAwardProperty(int id)
  {
    sign = TaskAwardOpenForWindow.W_GameTask;
    awardid = id;
    DashFire.MissionConfig missionconfig = DashFire.LogicSystem.GetMissionDataById(id);
    if (missionconfig != null) {
      Transform tf = transform.Find("Back/Type/TypeDescription");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = missionconfig.Name;
        }
      }
      tf = transform.Find("Back/Challenge/ChallengeDescription");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = missionconfig.Description;
        }
      }
    }
    DashFire.Data_SceneDropOut dsdo = DashFire.SceneConfigProvider.Instance.GetSceneDropOutById(missionconfig.DropId);
    if (dsdo != null) {
      DashFire.RoleInfo ri = DashFire.LobbyClient.Instance.CurrentRole;
      if (ri != null && ri.GetMissionStateInfo() != null) {
        SetSomething(dsdo.m_GoldSum, dsdo.m_Diamond, ri.GetMissionStateInfo().GetMissionsExpReward(id, ri.Level), dsdo.m_ItemIdList, dsdo.m_ItemCountList);
      }
    }
  }
  public void SetSomething(int money, int diamond, int exp, List<int> itemlist, List<int> itemcount)
  {
    foreach (GameObject go in golist) {
      if (go != null) {
        NGUITools.DestroyImmediate(go);
      }
    }
    golist.Clear();

    Transform tfb = transform.Find("Back");
    if (tfb != null) {
      if (money > 0) {
        GameObject go = DashFire.ResourceSystem.GetSharedResource("UI/GameTask/AwardItem") as GameObject;
        if (go != null) {
          go = NGUITools.AddChild(tfb.gameObject, go);
          if (go != null) {
            golist.Add(go);
            Texture utt = GamePokeyManager.GetTextureByPicName("UI/GoodsPhoto/Money");
            UITexture ut = go.GetComponent<UITexture>();
            if (ut != null) {
              if (utt != null) {
                ut.mainTexture = utt;
              } else {
                DashFire.ResLoadAsyncHandler.LoadAsyncItem("UI/GoodsPhoto/Money", ut);
              }
            }
            Transform tf = go.transform.Find("Label");
            if (tf != null) {
              UILabel ul = tf.gameObject.GetComponent<UILabel>();
              if (ul != null) {
                ul.text = "X" + money;
              }
            }
          }
        }
      }
      if (diamond > 0) {
        GameObject go = DashFire.ResourceSystem.GetSharedResource("UI/GameTask/AwardItem") as GameObject;
        if (go != null) {
          go = NGUITools.AddChild(tfb.gameObject, go);
          if (go != null) {
            golist.Add(go);
            Texture utt = GamePokeyManager.GetTextureByPicName("UI/GoodsPhoto/Diamond");
            UITexture ut = go.GetComponent<UITexture>();
            if (ut != null) {
              if (utt != null) {
                ut.mainTexture = utt;
              } else {
                DashFire.ResLoadAsyncHandler.LoadAsyncItem("UI/GoodsPhoto/Diamond", ut);
              }
            }
            Transform tf = go.transform.Find("Label");
            if (tf != null) {
              UILabel ul = tf.gameObject.GetComponent<UILabel>();
              if (ul != null) {
                ul.text = "X" + diamond;
              }
            }
          }
        }
      }
      if (exp > 0) {
        GameObject go = DashFire.ResourceSystem.GetSharedResource("UI/GameTask/AwardItem") as GameObject;
        if (go != null) {
          go = NGUITools.AddChild(tfb.gameObject, go);
          if (go != null) {
            golist.Add(go);
            Texture utt = GamePokeyManager.GetTextureByPicName("UI/GoodsPhoto/Exp");
            UITexture ut = go.GetComponent<UITexture>();
            if (ut != null) {
              if (utt != null) {
                ut.mainTexture = utt;
              } else {
                DashFire.ResLoadAsyncHandler.LoadAsyncItem("UI/GoodsPhoto/Exp", ut);
              }
            }
            Transform tf = go.transform.Find("Label");
            if (tf != null) {
              UILabel ul = tf.gameObject.GetComponent<UILabel>();
              if (ul != null) {
                ul.text = "X" + exp;
              }
            }
          }
        }
      }
      int count = itemlist.Count;
      for (int i = 0; i < count; ++i) {
        DashFire.ItemConfig ic = DashFire.ItemConfigProvider.Instance.GetDataById(itemlist[i]);
        if (ic != null) {
          GameObject go = DashFire.ResourceSystem.GetSharedResource("UI/GameTask/AwardItem") as GameObject;
          if (go != null) {
            go = NGUITools.AddChild(tfb.gameObject, go);
            if (go != null) {
              golist.Add(go);
              Texture utt = GamePokeyManager.GetTextureByPicName(ic.m_ItemTrueName);
              UITexture ut = go.GetComponent<UITexture>();
              if (ut != null) {
                if (utt != null) {
                  ut.mainTexture = utt;
                } else {
                  DashFire.ResLoadAsyncHandler.LoadAsyncItem(ic.m_ItemTrueName, ut);
                }
              }
              Transform tf = go.transform.Find("Frame");
              if (tf != null) {
                UISprite us = tf.gameObject.GetComponent<UISprite>();
                if (us != null) {
                  us.spriteName = "EquipFrame" + ic.m_PropertyRank; ;
                }
              }
              tf = go.transform.Find("Label");
              if (tf != null) {
                UILabel ul = tf.gameObject.GetComponent<UILabel>();
                if (ul != null) {
                  ul.text = "X" + itemcount[i];
                }
              }
            }
          }
        }
      }
    }
    int number = golist.Count;
    if (number == 0) return;
    int offset = 0;
    int start = 0;
    if (number % 2 != 0) {
      GameObject go = golist[0];
      if (go != null) {
        go.transform.localPosition = new Vector3(0.0f, 25f, 0.0f);
        start = 1;
        offset = 50;
      }
    } else {
      offset = -60;
    }
    for (int i = start; i < number; ++i) {
      int j = i;
      if (number % 2 == 0) {
        j = i + 1;
      }
      GameObject go = golist[i];
      if (go != null) {
        if (j % 2 == 0) {
          go.transform.localPosition = new Vector3(j / 2 * (-120) - offset, 25, 0);
        } else {
          go.transform.localPosition = new Vector3((j / 2 + 1) * 120 + offset, 25f, 0.0f);
        }
      }
    }
  }
  public void SetAwardForMermanKing(int money, int diamond, int exp, List<int> itemid, List<int> itemcount)
  {
    sign = TaskAwardOpenForWindow.W_MermanKing;
    SetSomething(money, diamond, exp, itemid, itemcount);
  }
  public void SetSellGain(string money, string diamond)
  {
    sign = TaskAwardOpenForWindow.W_SellGain;
    foreach (GameObject go in golist) {
      if (go != null) {
        NGUITools.DestroyImmediate(go);
      }
    }
    golist.Clear();

    Transform tfb = transform.Find("Back");
    if (tfb != null) {
      if (money != null) {
        GameObject go = DashFire.ResourceSystem.GetSharedResource("UI/GameTask/AwardItem") as GameObject;
        if (go != null) {
          go = NGUITools.AddChild(tfb.gameObject, go);
          if (go != null) {
            golist.Add(go);
            Texture utt = GamePokeyManager.GetTextureByPicName("UI/GoodsPhoto/Money");
            UITexture ut = go.GetComponent<UITexture>();
            if (ut != null) {
              if (utt != null) {
                ut.mainTexture = utt;
              } else {
                DashFire.ResLoadAsyncHandler.LoadAsyncItem("UI/GoodsPhoto/Money", ut);
              }
            }
            Transform tf = go.transform.Find("Label");
            if (tf != null) {
              UILabel ul = tf.gameObject.GetComponent<UILabel>();
              if (ul != null) {
                ul.text = money;
              }
            }
          }
        }
      }
      if (diamond != null) {
        GameObject go = DashFire.ResourceSystem.GetSharedResource("UI/GameTask/AwardItem") as GameObject;
        if (go != null) {
          go = NGUITools.AddChild(tfb.gameObject, go);
          if (go != null) {
            golist.Add(go);
            Texture utt = GamePokeyManager.GetTextureByPicName("UI/GoodsPhoto/Diamond");
            UITexture ut = go.GetComponent<UITexture>();
            if (ut != null) {
              if (utt != null) {
                ut.mainTexture = utt;
              } else {
                DashFire.ResLoadAsyncHandler.LoadAsyncItem("UI/GoodsPhoto/Diamond", ut);
              }
            }
            Transform tf = go.transform.Find("Label");
            if (tf != null) {
              UILabel ul = tf.gameObject.GetComponent<UILabel>();
              if (ul != null) {
                ul.text = diamond;
              }
            }
          }
        }
      }
    }
    int number = golist.Count;
    if (number == 0) return;
    int offset = 0;
    int start = 0;
    if (number % 2 != 0) {
      GameObject go = golist[0];
      if (go != null) {
        go.transform.localPosition = new Vector3(0.0f, 25f, 0.0f);
        start = 1;
        offset = 50;
      }
    } else {
      offset = -60;
    }
    for (int i = start; i < number; ++i) {
      int j = i;
      if (number % 2 == 0) {
        j = i + 1;
      }
      GameObject go = golist[i];
      if (go != null) {
        if (j % 2 == 0) {
          go.transform.localPosition = new Vector3(j / 2 * (-120) - offset, 25, 0);
        } else {
          go.transform.localPosition = new Vector3((j / 2 + 1) * 120 + offset, 25, 0.0f);
        }
      }
    }
  }

  private int awardid = 0;
  private List<GameObject> golist = new List<GameObject>();
  private TaskAwardOpenForWindow sign = TaskAwardOpenForWindow.W_GameTask;
}
public enum TaskAwardOpenForWindow : int
{
  W_GameTask = 0,
  W_MermanKing = 1,
  W_SellGain = 2,
}
