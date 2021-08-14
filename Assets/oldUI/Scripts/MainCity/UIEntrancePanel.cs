using UnityEngine;
using DashFire;
using System;
using System.Collections;
using System.Collections.Generic;


public class UIEntrancePanel : MonoBehaviour
{
  //
  public UISprite entrance = null;
  private const int c_ButtonNumber = 8;
  public GameObject[] buttons = new GameObject[c_ButtonNumber];
  public GameObject table = null;
  private int m_RoleCurrentLevel = 0;
  public int C_TransOffset = 108;
  public int C_ButtonMax = 5;
  private bool hasAdded = false;
  //table的初始位置
  public Vector3 m_OriginalTablePos = new Vector3(-484, 109f, 0);
  //解锁顺序
  private Dictionary<int, string> m_UnlockDic = new Dictionary<int, string>()
  {
    {1,"05Mission"},
    {2,"02SkillPanel"},
    {3,"01Equipment"},
    {4,"04GodEquip"},
  };
  // Use this for initialization
  void Start()
  {
    //初始隐藏Table所有图标
    if (null != table) {
      for (int index = 0; index < table.transform.childCount; ++index) {
        Transform ts = table.transform.GetChild(index);
        if (ts != null) NGUITools.SetActive(ts.gameObject, false);
      }
    }
    foreach (GameObject go in buttons) {
      UIEventListener.Get(go).onClick += this.OnButtonClick;
    }
  }

  // Update is called once per frame
  void Update()
  {
    CheckUnlockSkill();
    RoleInfo role_info = LobbyClient.Instance.CurrentRole;
    if (null != role_info && role_info.Level > m_RoleCurrentLevel) {
      m_RoleCurrentLevel = role_info.Level;
      RepositionTable(role_info.Level);
    }
  }
  void OnEnable()
  {
    if (table != null) {
      UITable uiTable = table.GetComponent<UITable>();
      if (uiTable != null) uiTable.Reposition();
    }
  }
  public void OnEntranceBtnClick()
  {
    if (table == null)
      return;
    if (NGUITools.GetActive(table)) {
      NGUITools.SetActive(table, false);
    } else {
      NGUITools.SetActive(table, true);
      RoleInfo role_info = LobbyClient.Instance.CurrentRole;
      if (null != role_info) {
        RepositionTable(role_info.Level);
      }
    }
  }

  public void OnButtonClick(GameObject go)
  {
    if (go == null)
      return;
    switch (go.name) {
      case "01Equipment": OpenAndCloseWindow("GamePokey"); break;
      case "02SkillPanel": OpenAndCloseWindow("SkillPanel"); break;
      case "04GodEquip": OpenAndCloseWindow("ArtifactPanel"); break;
      case "05Mission": OpenAndCloseWindow("GameTask");
        JoyStickInputProvider.JoyStickEnable = false;
        DashFire.GfxSystem.EventChannelForLogic.Publish("ge_reload_missions", "lobby");
        break;
      case "06Match": /*EnterInScene(2);*/ OpenAndCloseWindow("Mars"); break;
      case "07Treasure":
        RoleInfo role_info = LobbyClient.Instance.CurrentRole;
        if (role_info != null && role_info.Level < 18) {
          string chn_desc = StrDictionaryProvider.Instance.GetDictString(456);
          string chn_confirm = StrDictionaryProvider.Instance.GetDictString(4);
          DashFire.LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", chn_desc, chn_confirm, null, null, null, false);
        } else {
          OpenAndCloseWindow("cangbaotu");
        }
        break;
      case "08Mail": OpenAndCloseWindow("Mail"); break;
      default: break;
    }
  }
  private void InputModeSwitch()
  {
    if (DFMUiRoot.InputMode == InputType.Joystick) {
      DFMUiRoot.InputMode = InputType.Touch;
    } else {
      DFMUiRoot.InputMode = InputType.Joystick;
    }
  }
  private void OpenAndCloseWindow(string window)
  {
    UIManager.Instance.ToggleWindowVisible(window);
  }
  //临时增加PvP与MPvE入口
  private void EnterInScene(int sceneId)
  {
    if (sceneId != -1) {
      //Debug.Log("ge_select_scene sceneId:" + sceneId);
      DashFire.LogicSystem.PublishLogicEvent("ge_select_scene", "lobby", sceneId);
    } else {
      Debug.Log("sceneId is -1!!");
    }
  }
  //根据图标显示的个数重新调整Table的位置
  private void RepositionTable(int roleLevel)
  {
    int assit_pos = 1;
    for (int index = 1; index <= roleLevel; ++index) {
      if (m_UnlockDic.ContainsKey(index)) {
        string name = m_UnlockDic[index];
        if (!string.IsNullOrEmpty(name) && table != null) {
          GameObject go = FindGameObjectByName(name);
          if (null != go) {
            NGUITools.SetActive(go, true);
            if (table != null) {
              Vector3 pos = table.transform.localPosition;
              table.transform.localPosition = new Vector3(m_OriginalTablePos.x + (C_ButtonMax - assit_pos) * C_TransOffset, pos.y, 0);
              UITable scriptTable = table.GetComponent<UITable>();
              if (scriptTable != null) scriptTable.Reposition();
              assit_pos++;
            }
          }
        }
      }
    }
  }
  private GameObject FindGameObjectByName(string name)
  {
    foreach (GameObject go in buttons) {
      if (null != go && go.name == name)
        return go;
    } return null;
  }
  //检查是否有可解锁技能
  private void CheckUnlockSkill()
  {
    RoleInfo role_info = LobbyClient.Instance.CurrentRole;
    if (role_info != null && role_info.SkillInfos != null) {
      SkillInfo skill_info = null;
      int index = 0;
      for (index = 0; index < role_info.SkillInfos.Count; ++index) {
        skill_info = role_info.SkillInfos[index];
        if (skill_info != null && skill_info.SkillLevel<=0) {
          if (skill_info.ConfigData != null && skill_info.ConfigData.ActivateLevel <= role_info.Level) {
            //有可解锁技能
            AddPointingToSkill(true);
            break;
          }
        }
      }
      if (index >= role_info.SkillInfos.Count)
        AddPointingToSkill(false);//没有可解锁技能
    }
  }
  //技能开关上添加、删除指引图标
  private void AddPointingToSkill(bool canUnlock)
  {
    if (canUnlock) {
      if (!hasAdded) {
        string path = UIManager.Instance.GetPathByName("Pointing");
        GameObject go = ResourceSystem.GetSharedResource(path) as GameObject;
        if (null != go) {
          GameObject goFather = GetBtnGameObject("02SkillPanel");
          if (goFather != null) {
            go = NGUITools.AddChild(goFather, go);
            UISprite sp = goFather.GetComponent<UISprite>();
            if (sp != null)
              go.transform.localPosition = new Vector3(sp.width / 3f, sp.height / 3f, 0);
            go.name = "Pointing";
            hasAdded = true;
			TweenPosition tween = go.GetComponent<TweenPosition>();
			if(tween!=null){
				tween.from = go.transform.localPosition;
				Vector3 pos = go.transform.localPosition;
				tween.to = new Vector3(pos.x,pos.y+5,0);
			tween.enabled = true;
			}
          }
        }
      }
    } else {
      GameObject goFather = GetBtnGameObject("02SkillPanel");
      if (goFather != null) {
        Transform ts = goFather.transform.Find("Pointing");
        if (ts != null) NGUITools.Destroy(ts.gameObject);
      }
    }
  }

  private GameObject GetBtnGameObject(string name)
  {
    for (int index = 0; index < buttons.Length; ++index) {
      if (buttons[index] != null && buttons[index].name == name)
        return buttons[index];
    }
    return null;
  }

}
