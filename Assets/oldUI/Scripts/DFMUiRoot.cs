using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using StoryDlg;
using DashFire;

public class DFMUiRoot : MonoBehaviour
{
  // Use this for initialization
  void Start()
  {
    DontDestroyOnLoad(this.gameObject.transform.parent);
    UIManager.Instance.Init(this.gameObject);
    UIManager.Instance.OnAllUiLoadedDelegate += AfterAllUiLoaded;
    DashFire.LogicSystem.EventChannelForGfx.Subscribe<float, float, float, int>("ge_hero_blood", "ui", ShowAddHPForHero);
    DashFire.LogicSystem.EventChannelForGfx.Subscribe<float, float, float, int>("ge_hero_energy", "ui", ShowAddMPForHero);
    DashFire.LogicSystem.EventChannelForGfx.Subscribe<float, float, float, int, bool>("ge_npc_odamage", "ui", ShowbloodFlyTemplate);
    DashFire.LogicSystem.EventChannelForGfx.Subscribe<float, float, float, int, bool>("ge_npc_cdamage", "ui", ShowCriticalDamage);
    DashFire.LogicSystem.EventChannelForGfx.Subscribe<float, float, float, int>("ge_gain_money", "ui", ShowGainMoney);
    //hit
    DashFire.LogicSystem.EventChannelForGfx.Subscribe<int>("ge_hitcount", "ui", this.OnChainBeat);
    //蓄力
    DashFire.LogicSystem.EventChannelForGfx.Subscribe<float, float, float, float, int>("ge_monster_power", "ui", ShowMonsterPrePower);
    DashFire.LogicSystem.EventChannelForGfx.Subscribe<int>("ge_enter_scene", "ui", EnterInScene);
    DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_loading_start", "ui", StartLoading);
    DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_loading_finish", "ui", EndLoading);
    DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_show_login", "ui", ShowLogin);
    DashFire.LogicSystem.EventChannelForGfx.Subscribe<int>("ge_pvp_counttime", "ui", PvpCountTime);
    DashFire.LogicSystem.EventChannelForGfx.Subscribe<GfxUserInfo>("ge_show_name_plate", "ui", CreateHeroNickName);
    DashFire.LogicSystem.EventChannelForGfx.Subscribe<List<GfxUserInfo>>("ge_show_name_plates", "ui", CreateHeroNickName);
    DashFire.LogicSystem.EventChannelForGfx.Subscribe<GfxUserInfo>("ge_show_npc_name_plate", "ui", CreateNpcNickName);
    DashFire.LogicSystem.EventChannelForGfx.Subscribe<string, Action<bool>>("ge_show_yesornot", "ui", ShowYesOrNot);
    DashFire.LogicSystem.EventChannelForGfx.Subscribe<List<int>>("ge_show_newbieguide", "ui", ShowNewbieGuide);
    DashFire.LogicSystem.EventChannelForGfx.Subscribe<float, float, float, bool, string>("ge_screen_tip", "ui", ShowScreenTip);
    DashFire.LogicSystem.EventChannelForGfx.Subscribe<int, int>("ge_sell_item_income", "bag", SellAward);
    DashFire.LogicSystem.EventChannelForGfx.Subscribe<int, int, int, int>("ge_pve_fightinfo", "ui", SetPveFightInfo);
    DashFire.LogicSystem.EventChannelForGfx.Subscribe<int, int>("ge_start_monster_sheild", "ui", CreateMonestSheildBlood);
    DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_hide_input_ui", "ui", HideInputUi);

    DynamicWidgetPanel = transform.Find("DynamicWidget").gameObject;
    ScreenTipPanel = transform.Find("ScreenTipPanel").gameObject;
  }
  // Update is called once per frame
  void Update()
  {
  }

  void SellAward(int money, int gold)
  {
    try {
      string path = UIManager.Instance.GetPathByName("SellAward");
      UnityEngine.Object obj = DashFire.ResourceSystem.NewObject(path, timeRecycle);
      GameObject go = obj as GameObject;
      if (null != go) {
        Transform tf = go.transform.Find("Label/Money/MLabel");
        if (tf != null) {
          UILabel ul = tf.gameObject.GetComponent<UILabel>();
          if (null != ul) {
            ul.text = money.ToString();
          }
        }
        tf = go.transform.Find("Label/Diamond/DLabel");
        if (tf != null) {
          UILabel ul = tf.gameObject.GetComponent<UILabel>();
          if (null != ul) {
            ul.text = gold.ToString();
          }
        }
        if (ScreenTipPanel != null) {
          go = NGUITools.AddChild(ScreenTipPanel, obj);
        }
        if (go != null) {
          tf = go.transform.Find("Label/Money");
          if (tf != null) {
            if (money == 0) {
              NGUITools.SetActive(tf.gameObject, false);
            } else {
              NGUITools.SetActive(tf.gameObject, true);
            }
          }
          tf = go.transform.Find("Label/Diamond");
          if (tf != null) {
            if (gold == 0) {
              NGUITools.SetActive(tf.gameObject, false);
            } else {
              NGUITools.SetActive(tf.gameObject, true);
            }
          }
          BloodAnimation ba = go.GetComponent<BloodAnimation>();
          if (ba != null) {
            ba.PlayAnimation();
          }
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  void ShowScreenTip(float x, float y, float z, bool is3d, string tip)
  {
    try {
      string path = UIManager.Instance.GetPathByName("ScreenTip");
      UnityEngine.Object obj = DashFire.ResourceSystem.NewObject(path, timeRecycle);
      GameObject go = obj as GameObject;
      if (null != go) {
        Transform tf = go.transform.Find("Label");
        if (tf != null) {
          UILabel bloodPanel = tf.gameObject.GetComponent<UILabel>();
          if (null != bloodPanel) {
            bloodPanel.text = tip;
          }
        }
        GameObject cube = null;
        if (ScreenTipPanel != null) {
          cube = NGUITools.AddChild(ScreenTipPanel, obj);
        }
        if (cube != null) {
          if (is3d) {
            Vector3 pos = new Vector3(x, y, z);
            pos = Camera.main.WorldToScreenPoint(pos);
            pos.z = 0; pos.y += 100;
            pos = UICamera.mainCamera.ScreenToWorldPoint(pos);
            cube.transform.position = pos;
          } else {
            cube.transform.position = new Vector3(x, y, z);
          }
          BloodAnimation ba = cube.GetComponent<BloodAnimation>();
          if (ba != null) {
            ba.PlayAnimation();
          }
          NGUITools.SetActive(cube, true);
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  public void PvpCountTime(int counttime)
  {
    try {
      string path = UIManager.Instance.GetPathByName("PVPTime");
      GameObject go = DashFire.ResourceSystem.GetSharedResource(path) as GameObject;
      if (go != null) {
        Transform tf = go.transform.Find("Label");
        if (tf != null) {
          UILabel ul = tf.gameObject.GetComponent<UILabel>();
          if (null != ul) {
            ul.text = "00:00";
          }
        }
        go = NGUITools.AddChild(gameObject, go);
        if (go != null) {
          PVPTime pt = go.GetComponent<PVPTime>();
          if (pt != null) {
            pt.SetCountDownTime(counttime);
          }
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  public void HidePanelCreateUser()
  {
    Transform ts = this.transform.Find("PanelCreateUser");
    if (ts != null) {
      if (null != ts.gameObject) {
        NGUITools.SetActive(ts.gameObject, false);
      }
    }
  }

  public void ShowLogin()
  {
    try {
      UIManager.Instance.LoadAllWindows(0, UICamera.mainCamera);
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  public void LoadUiInGame(int sceneId)
  {
    UIManager.Instance.LoadAllWindows(-1, UICamera.mainCamera);
    //初始化剧情配置数据
    InitStoryDlg();
  }

  //进入场景
  public void EnterInScene(int sceneId)
  {
    try {
      NowSceneID = sceneId;
      SceneSubTypeEnum prevSceneType = m_SubSceneType;//上一场景类型
      m_EnemyNum = 0;
      Data_SceneConfig dsc = SceneConfigProvider.Instance.GetSceneConfigById(sceneId);
      if (dsc != null) {
        if (dsc.m_SubType == (int)SceneSubTypeEnum.TYPE_EXPEDITION)
          m_SubSceneType = SceneSubTypeEnum.TYPE_EXPEDITION;
        else {
          m_SubSceneType = SceneSubTypeEnum.TYPE_UNKNOWN;
        }

        if (dsc.m_Type == (int)SceneTypeEnum.TYPE_SERVER_SELECT) {
          m_SceneType = SceneTypeEnum.TYPE_SERVER_SELECT;
          UIManager.Instance.LoadAllWindows(0, UICamera.mainCamera);
        }

        if (dsc.m_Type == (int)SceneTypeEnum.TYPE_PURE_CLIENT_SCENE) {
          m_SceneType = SceneTypeEnum.TYPE_PURE_CLIENT_SCENE;
          UIManager.Instance.LoadAllWindows(1, UICamera.mainCamera);
          StartCoroutine(DelayForNewbieGuide());
        }
        if (dsc.m_Type == (int)SceneTypeEnum.TYPE_PVP) {
          m_SceneType = SceneTypeEnum.TYPE_PVP;
          LoadUiInGame(sceneId);
          if (UIManager.Instance.OnAllUiLoadedDelegate != null) {
            UIManager.Instance.OnAllUiLoadedDelegate();
          }
        }
        if (dsc.m_Type == (int)SceneTypeEnum.TYPE_PVE) {
          m_SceneType = SceneTypeEnum.TYPE_PVE;
          LoadUiInGame(sceneId);
          if (UIManager.Instance.OnAllUiLoadedDelegate != null) {
            UIManager.Instance.OnAllUiLoadedDelegate();
          }
          SetPveFightInfo(4, 0, 0, 0);
        }
        //如果刚打完远征，则打开远征界面
        if (prevSceneType == SceneSubTypeEnum.TYPE_EXPEDITION && m_SceneType == SceneTypeEnum.TYPE_PURE_CLIENT_SCENE) {
          UIManager.Instance.ShowWindowByName("cangbaotu");
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  //
  public void AfterAllUiLoaded()
  {
    if (m_SceneType == SceneTypeEnum.TYPE_PVP) {
      GameObject goSkill = UIManager.Instance.GetWindowGoByName("SkillBar");
      if (goSkill != null) {
        SkillBar skillBar = goSkill.GetComponent<SkillBar>();
        if (skillBar != null) skillBar.SetExButtonVisble(false);
      }
    }
  }

  public bool IsCombatWithGhost(List<GfxUserInfo> gfxUsers)
  {
    if (gfxUsers == null) return false;
    RoleInfo role_info = LobbyClient.Instance.CurrentRole;
    if (role_info == null) return false;
    UserInfo user_info = role_info.GetPlayerSelfInfo();
    if (user_info == null) return false;
    for (int i = 0; i < gfxUsers.Count; ++i) {
      if (gfxUsers[i] != null) {
        SharedGameObjectInfo share_info = LogicSystem.GetSharedGameObjectInfo(gfxUsers[i].m_ActorId);
        if (share_info != null && user_info.GetCampId() != share_info.CampId)
          return true;
      }
    }
    return false;
  }

  //pve创建
  public void CreatePvPHeroPanel(List<GfxUserInfo> gfxUsers)
  {
    m_EnemyNum = 0;
    if (gfxUsers != null) {
      if (m_SubSceneType == SceneSubTypeEnum.TYPE_EXPEDITION && IsCombatWithGhost(gfxUsers)) {
        if (PveFightInfo != null) {
          PveFightInfo pfi = PveFightInfo.GetComponent<PveFightInfo>();
          if (pfi != null) {
            pfi.UnSubscribe();
          }
        }
        //远征中怪为镜像
        UIManager.Instance.UnLoadWindowByName("HeroPanel");
        RoleInfo role_info = LobbyClient.Instance.CurrentRole;
        if (role_info == null) {
          Debug.Log("!!RoleInfo is null.");
          return;
        }
        UserInfo user_info = role_info.GetPlayerSelfInfo();
        if (user_info == null) {
          Debug.Log("!!UserInfo is null");
          return;
        }
        int self_campId = user_info.GetCampId();
        for (int index = 0; index < gfxUsers.Count; ++index) {
          if (gfxUsers[index] != null) {
            SharedGameObjectInfo shared_info = LogicSystem.GetSharedGameObjectInfo(gfxUsers[index].m_ActorId);
            if (shared_info != null) {
              if (self_campId == shared_info.CampId) {
                //如果是自己阵营，因为只有单人Pvp所以就是自己
                GameObject go = UIManager.Instance.LoadWindowByName("PVPmyHero", UICamera.mainCamera);
                if (go == null) {
                  Debug.Log("!!!PVPmyHero is null.");
                  continue;
                }
                HeroPanel scriptHp = go.GetComponent<HeroPanel>();
                if (scriptHp != null) scriptHp.SetUserInfo(gfxUsers[index]);
              } else {
                //敌人
                GameObject go = UIManager.Instance.LoadWindowByName("PVPmyEnemy", UICamera.mainCamera);
                if (go == null) {
                  Debug.Log("!!!PVPmyEnemy is null."); continue;
                }
                go.transform.localPosition += new Vector3(0, -90f * (m_EnemyNum), 0);
                HeroPanel scriptHp = go.GetComponent<HeroPanel>();
                if (scriptHp != null) scriptHp.SetUserInfo(gfxUsers[index]);
                m_EnemyNum++;//敌人数加1
              }
            }
          }
        }

      } else {
        //目前普通PVE只有玩家自己
        if (gfxUsers.Count > 0 && gfxUsers[0] != null) {
          GameObject go = UIManager.Instance.GetWindowGoByName("HeroPanel");
          if (go == null) {
            Debug.Log("!!!HeroPanel is null.");
            return;
          }
          HeroPanel scriptHp = go.GetComponent<HeroPanel>();
          if (scriptHp != null) scriptHp.SetUserInfo(gfxUsers[0]);
        }
      }
    }
  }
  //Pvp下创建
  public void CreatePvPHeroPanel(GfxUserInfo gfxUser)
  {
    if (gfxUser != null && m_SceneType == SceneTypeEnum.TYPE_PVP) {
      UIManager.Instance.UnLoadWindowByName("HeroPanel");
      RoleInfo role_info = LobbyClient.Instance.CurrentRole;
      if (role_info != null) {
        UserInfo user_info = role_info.GetPlayerSelfInfo();
        if (user_info == null) {
          Debug.Log("!!!user_info is null.");
          return;
        }
        SharedGameObjectInfo share_info = LogicSystem.GetSharedGameObjectInfo(gfxUser.m_ActorId);
        if (share_info == null) {
          if (user_info == null) {
            Debug.Log("!!!share_info is null.");
            return;
          }
        }
        if (user_info.GetCampId() == share_info.CampId) {
          //如果是自己阵营，因为只有单人Pvp所以就是自己
          GameObject go = UIManager.Instance.LoadWindowByName("PVPmyHero", UICamera.mainCamera);
          if (go == null) {
            Debug.Log("!!!PVPmyHero is null.");
            return;
          }
          HeroPanel scriptHp = go.GetComponent<HeroPanel>();
          if (scriptHp != null) scriptHp.SetUserInfo(gfxUser);
        } else {
          //敌人
          GameObject go = UIManager.Instance.LoadWindowByName("PVPmyEnemy", UICamera.mainCamera);
          if (go == null) {
            Debug.Log("!!!PVPmyEnemy is null.");
            return;
          }
          go.transform.localPosition += new Vector3(0, -90f * (m_EnemyNum), 0);
          HeroPanel scriptHp = go.GetComponent<HeroPanel>();
          if (scriptHp != null) scriptHp.SetUserInfo(gfxUser);
          m_EnemyNum++;//敌人数加1
        }
      }
    }
  }
  public void CreateHeroNickName(GfxUserInfo gfxUser)
  {
    try {
      CreatePvPHeroPanel(gfxUser);
      RoleInfo ri = DashFire.LobbyClient.Instance.CurrentRole;
      if (ri != null) {
        UserInfo ui = ri.GetPlayerSelfInfo();
        if (ui != null) {
          AboutHeroNickName(gfxUser, ui.GetCampId());
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  public void CreateHeroNickName(List<GfxUserInfo> gfxUsers)
  {
    try {
      UserInfoForUI.Clear();
      CreatePvPHeroPanel(gfxUsers);
      RoleInfo ri = DashFire.LobbyClient.Instance.CurrentRole;
      if (ri != null) {
        UserInfo ui = ri.GetPlayerSelfInfo();
        if (ui != null) {
          foreach (GfxUserInfo gui in gfxUsers) {
            AboutHeroNickName(gui, ui.GetCampId());
          }
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private void AboutHeroNickName(GfxUserInfo gui, int cmpid)
  {
    if (gui != null) {
      UserInfoForUI.Add(gui);
      GameObject pargo = DashFire.LogicSystem.GetGameObject(gui.m_ActorId);
      SharedGameObjectInfo sgoi = DashFire.LogicSystem.GetSharedGameObjectInfo(gui.m_ActorId);
      if (pargo != null && sgoi != null) {
        string path = UIManager.Instance.GetPathByName("NickName");
        GameObject go = DashFire.ResourceSystem.GetSharedResource(path) as GameObject;
        if (go != null && DynamicWidgetPanel != null) {
          go = NGUITools.AddChild(DynamicWidgetPanel, go);
          if (go != null) {
            NickName nn = go.GetComponent<NickName>();
            if (nn != null) {
              nn.SetPlayerGameObjectAndNickName(pargo, gui.m_Nick, sgoi.CampId == cmpid ? Color.white : Color.red);
            }
          }
        }
      }
    }
  }
  private void CreateNpcNickName(GfxUserInfo gfxNpc)
  {
    try {
      AboutNpcNickName(gfxNpc);
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  private void AboutNpcNickName(GfxUserInfo gui)
  {
    if (gui != null) {
      Data_NpcConfig dnc = NpcConfigProvider.Instance.GetNpcConfigById(gui.m_HeroId);
      if (dnc != null && dnc.m_ShowName) {
        GameObject pargo = DashFire.LogicSystem.GetGameObject(gui.m_ActorId);
        if (!NpcGameObjectS.ContainsKey(pargo)) {
          SharedGameObjectInfo sgoi = DashFire.LogicSystem.GetSharedGameObjectInfo(gui.m_ActorId);
          if (pargo != null && sgoi != null) {
            string path = UIManager.Instance.GetPathByName("NickName");
            GameObject go = DashFire.ResourceSystem.GetSharedResource(path) as GameObject;
            if (go != null && DynamicWidgetPanel != null) {
              go = NGUITools.AddChild(DynamicWidgetPanel, go);
              if (go != null) {
                NpcGameObjectS.Add(pargo, go);
                NickName nn = go.GetComponent<NickName>();
                if (nn != null) {
                  nn.SetPlayerGameObjectAndNickName(pargo, dnc.m_Name, Color.white);
                }
              }
            }
          }
        } else {
          GameObject go = NpcGameObjectS[pargo];
          if (go != null) {
            UILabel ul = go.GetComponent<UILabel>();
            if (ul != null) {
              ul.text = dnc.m_Name;
            }
          }
        }
      }
    }
  }
  public IEnumerator DelayForNewbieGuide()
  {
    yield return new WaitForSeconds(0.5f);
    NewbieGuideManager nbm = gameObject.GetComponent<NewbieGuideManager>();
    if (nbm != null) {
      NGUITools.DestroyImmediate(nbm);
    }
    RoleInfo role = LobbyClient.Instance.CurrentRole;
    if (null != role && role.NewbieGuides.Count > 0) {
      SystemGuideConfig config = SystemGuideConfigProvider.Instance.GetDataById(role.NewbieGuides[0]);
      if (null != config) {
        ShowNewbieGuide(config.Operations);
      }
    }
  }
  public void OnChainBeat(int number)
  {
    try {
      if (number <= 0) {
        beatnum = 0;
        UIManager.Instance.HideWindowByName("ChainBeat");
      } else {
        UIManager.Instance.ShowWindowByName("ChainBeat");
        GameObject go = UIManager.Instance.GetWindowGoByName("ChainBeat");
        if (go != null) {
          Transform tf = go.transform.Find("Scaling");
          if (tf != null) {
            ChainBeat cb = tf.gameObject.GetComponent<ChainBeat>();
            cb.SetInitTime();
            cb.enabled = true;
          }
          int num = System.Math.Abs(number) % 100;
          if (num > beatnum) {
            beatnum = num;
          } else {
            num = beatnum;
          }
          tf = go.transform.Find("EvaluateImage");
          if (tf != null) {
            UISprite us = tf.gameObject.GetComponent<UISprite>();
            if (us != null) {
              if (beatnum <= 33) {
                us.spriteName = "good";
              } else if (beatnum <= 66) {
                us.spriteName = "great";
              } else if (beatnum <= 99) {
                us.spriteName = "perfect";
              }
            }
          }
          string chainstr = num.ToString();
          int i = chainstr.Length;
          if (i == 1) {
            chainstr = "0" + chainstr;
            i = 2;
          }
          for (int j = 0; j < 2; j++) {
            Transform CB = go.transform.Find("Scaling/" + "Number" + j);
            if (CB != null) {
              UISprite SP = CB.gameObject.GetComponent<UISprite>();
              if (SP != null) {
                SP.spriteName = "b" + chainstr.ToCharArray()[j].ToString();
              }
            }
          }
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private void InitStoryDlg()
  {
    StoryDlgManager.Instance.Init();
  }
  private void OnEnterNewScene()
  {
    //隐藏剧情对话框
    if (m_StoryDlgSmallGO != null) {
      NGUITools.SetActive(m_StoryDlgSmallGO, false);
    }
    if (m_StoryDlgBigGO != null) {
      NGUITools.SetActive(m_StoryDlgBigGO, false);
    }
  }
  //蓄力
  public void ShowMonsterPrePower(float x, float y, float z, float duration, int monsterId)
  {
    try {
      if (duration <= 0)
        return;
      Vector3 pos = new Vector3(x, y, z);
      if (Camera.main != null)
        pos = Camera.main.WorldToScreenPoint(pos);
      pos.z = 0;
      Vector3 nguiPos = Vector3.zero;
      if (UICamera.mainCamera != null) {
        nguiPos = UICamera.mainCamera.ScreenToWorldPoint(pos);
      }

      GameObject go = DashFire.ResourceSystem.GetSharedResource("UI/MonsterPrePower") as GameObject;
      GameObject prePowerGo = NGUITools.AddChild(this.gameObject, go);
      if (prePowerGo == null)
        return;
      prePowerGo.transform.position = nguiPos;
      MonsterPrePower power = prePowerGo.GetComponent<MonsterPrePower>();
      if (power != null) {
        power.Duration = duration;
        power.PowerId = monsterId;
        power.Position = new Vector3(x, y, z);
      } else {
        NGUITools.SetActive(prePowerGo, false);
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  //打断蓄力
  public void BreakPrePower(int monsterId)
  {
    for (int index = 0; index < this.transform.childCount; ++index) {
      Transform trans = this.transform.GetChild(index);
      GameObject go = null;
      if (trans != null)
        go = trans.gameObject;
      if (go != null && go.name == "MonsterPrePower(Clone)") {
        MonsterPrePower power = go.GetComponent<MonsterPrePower>();
        if (power == null)
          return;
        if (power.PowerId == monsterId) {
          NGUITools.SetActive(go, false);
          NGUITools.Destroy(go);
        }
      }
    }
  }
  //********************************************************************
  public void ShowbloodFlyTemplate(float x, float y, float z, int blood, bool isOrdinaryDamage)
  {
    try {
      string path = UIManager.Instance.GetPathByName("AttackEffect");
      UnityEngine.Object obj = DashFire.ResourceSystem.NewObject(path, timeRecycle);
      GameObject go = obj as GameObject;
      if (null != go) {
        Transform tf = go.transform.Find("Label");
        if (tf != null) {
          UILabel bloodPanel = tf.gameObject.GetComponent<UILabel>();
          if (null != bloodPanel) {
            bloodPanel.text = blood.ToString();
            if (isOrdinaryDamage) {
              bloodPanel.color = new Color(1.0f, 1.0f, 1.0f);
            } else {
              bloodPanel.color = new Color(0.92549f, 0.7098f, 0.0f);
            }
          }
        }
        Vector3 pos = new Vector3(x, y, z);
        pos = Camera.main.WorldToScreenPoint(pos);
        pos.z = 0; pos.y += (100 + offsetheight);
        offsetheight *= -1;
        pos = UICamera.mainCamera.ScreenToWorldPoint(pos);
        GameObject cube = null;
        if (DynamicWidgetPanel != null) {
          cube = NGUITools.AddChild(DynamicWidgetPanel, obj);
        }
        if (cube != null) {
          cube.transform.position = pos;
          BloodAnimation ba = cube.GetComponent<BloodAnimation>();
          if (ba != null) {
            ba.PlayAnimation();
          }
          NGUITools.SetActive(cube, true);
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private int offsetheight = 20;
  //**************************************************************
  public void ShowAddHPForHero(float x, float y, float z, int blood)
  {
    try {
      UnityEngine.Object obj = null;
      GameObject go = null;
      string path = "";
      int offset = 0;
      if (blood > 0) {
        path = "DamageForAddHero";
        offset = 100;
      } else {
        path = "DamageForCutHero";
        offset = -50;
      }
      path = UIManager.Instance.GetPathByName(path);
      obj = DashFire.ResourceSystem.NewObject(path, timeRecycle);
      go = obj as GameObject;
      if (null != go) {
        Transform tf = go.transform.Find("Label");
        if (tf != null) {
          UILabel bloodPanel = tf.gameObject.GetComponent<UILabel>();
          if (null != bloodPanel)
            bloodPanel.text = blood.ToString();
        }
        Vector3 pos = new Vector3(x, y, z);
        pos = Camera.main.WorldToScreenPoint(pos);
        pos.z = 0; pos.y += offset;
        pos = UICamera.mainCamera.ScreenToWorldPoint(pos);
        GameObject cube = null;
        if (DynamicWidgetPanel != null) {
          cube = NGUITools.AddChild(DynamicWidgetPanel, obj);
        }
        if (cube != null) {
          cube.transform.position = pos;
          BloodAnimation ba = cube.GetComponent<BloodAnimation>();
          if (ba != null) {
            ba.PlayAnimation();
          }
          NGUITools.SetActive(cube, true);
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  public void ShowAddMPForHero(float x, float y, float z, int energy)
  {
    try {
      UnityEngine.Object obj = null;
      GameObject go = null;
      string path = "";
      int offset = 0;
      if (energy > 0) {
        path = "EnergyAdd";
        offset = 100;
      } else {
        path = "EnergyCut";
        offset = -50;
      }
      path = UIManager.Instance.GetPathByName(path);
      obj = DashFire.ResourceSystem.NewObject(path, timeRecycle);
      go = obj as GameObject;
      if (null != go) {
        Transform tf = go.transform.Find("Label");
        if (tf != null) {
          UILabel bloodPanel = tf.gameObject.GetComponent<UILabel>();
          if (null != bloodPanel)
            bloodPanel.text = energy.ToString();
        }
        Vector3 pos = new Vector3(x, y, z);
        pos = Camera.main.WorldToScreenPoint(pos);
        pos.z = 0; pos.y += offset;
        pos = UICamera.mainCamera.ScreenToWorldPoint(pos);
        GameObject cube = null;
        if (DynamicWidgetPanel != null) {
          cube = NGUITools.AddChild(DynamicWidgetPanel, obj);
        }
        if (cube != null) {
          cube.transform.position = pos;
          BloodAnimation ba = cube.GetComponent<BloodAnimation>();
          if (ba != null) {
            ba.PlayAnimation();
          }
          NGUITools.SetActive(cube, true);
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  //*********************************************
  public void ShowCriticalDamage(float x, float y, float z, int blood, bool isOrdinaryDamage)
  {
    try {
      string path = UIManager.Instance.GetPathByName("CriticalDamage");
      UnityEngine.Object obj = DashFire.ResourceSystem.NewObject(path, timeRecycle);
      GameObject go = obj as GameObject;
      if (null != go) {
        string damage = System.Math.Abs(blood).ToString();
        Transform tf = go.transform.Find("Label");
        if (tf != null) {
          UILabel bloodPanel = tf.gameObject.GetComponent<UILabel>();
          if (null != bloodPanel) {
            bloodPanel.text = blood.ToString();
            if (isOrdinaryDamage) {
              bloodPanel.color = new Color(1.0f, 1.0f, 1.0f);
            } else {
              bloodPanel.color = new Color(0.92549f, 0.7098f, 0.0f);
            }
          }
        }
        Vector3 pos = new Vector3(x, y, z);
        pos = Camera.main.WorldToScreenPoint(pos);
        pos.z = 0; pos.y += 100;
        pos = UICamera.mainCamera.ScreenToWorldPoint(pos);
        GameObject cube = null;
        if (DynamicWidgetPanel != null) {
          cube = NGUITools.AddChild(DynamicWidgetPanel, obj);
        }
        if (cube != null) {
          cube.transform.position = pos;
          BloodAnimation ba = cube.GetComponent<BloodAnimation>();
          if (ba != null) {
            ba.PlayAnimation();
          }
          NGUITools.SetActive(cube, true);
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  public void ShowGainMoney(float x, float y, float z, int num)
  {
    try {
      string path = UIManager.Instance.GetPathByName("GainMoney");
      UnityEngine.Object obj = DashFire.ResourceSystem.NewObject(path, timeRecycle);
      GameObject go = obj as GameObject;
      if (null != go) {
        Transform tf = go.transform.Find("Label");
        if (tf != null) {
          UILabel bloodPanel = tf.gameObject.GetComponent<UILabel>();
          if (null != bloodPanel) {
            bloodPanel.text = num > 0 ? "+" + num : num.ToString();
          }
        }
        Vector3 pos = new Vector3(x, y, z);
        pos = Camera.main.WorldToScreenPoint(pos);
        pos.z = 0; pos.y += 100;
        pos = UICamera.mainCamera.ScreenToWorldPoint(pos);
        GameObject cube = null;
        if (DynamicWidgetPanel != null) {
          cube = NGUITools.AddChild(DynamicWidgetPanel, obj);
        }
        if (cube != null) {
          cube.transform.position = pos;
          BloodAnimation ba = cube.GetComponent<BloodAnimation>();
          if (ba != null) {
            ba.PlayAnimation();
          }
          NGUITools.SetActive(cube, true);
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  public void ShowStageClear(string type)
  {
    Transform existTrans = this.transform.Find("StageClear(Clone)");
    if (null != existTrans) {
      GameObject existGo = existTrans.gameObject;
      if (null != existGo)
        Destroy(existGo);
    }

    GameObject go = DashFire.ResourceSystem.GetSharedResource("UI/StageClear") as GameObject;
    if (null == go)
      return;
    go = NGUITools.AddChild(this.gameObject, go);
    if (go == null)
      return;
    StageClear stageClear = go.GetComponent<StageClear>();
    if (stageClear != null)
      stageClear.SetClearType(type);
  }

  //*******************************************
  void StartLoading()
  {
    try {
      //清理记录npcactorid的list
      NpcGameObjectS.Clear();
      //开始加载Loading条时，卸载所有UI
      UIManager.Instance.UnLoadAllWindow();
      GameObject goConnect = UIManager.Instance.GetWindowGoByName("Connect");
      if (goConnect != null) {
        GfxSystem.PublishGfxEvent("ge_ui_connect_hint", "ui", false, false);
      }
      if (InputType.Joystick == DFMUiRoot.InputMode) {
        JoyStickInputProvider.JoyStickEnable = false;
      }
      GameObject mgo = UIManager.Instance.GetWindowGoByName("Mars");
      if (mgo != null) {
        UIManager.Instance.HideWindowByName("Mars");
      }
      if (loading != null) return;
      GameObject go = DashFire.ResourceSystem.GetSharedResource("Loading/Loading2") as GameObject;
      if (go != null) {
        loading = NGUITools.AddChild(gameObject, go);
        if (loading != null) {
          loading.transform.localPosition = new Vector3(0, 0, 0);
          NGUITools.SetActive(loading, true);
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  void EndLoading()
  {
    try {
//       if (InputType.Joystick == DFMUiRoot.InputMode) {
//         JoyStickInputProvider.JoyStickEnable = true;
//       }
      if (loading != null) {
        loading.transform.Find("ProgressBar").SendMessage("EndLoading");
        loading = null;
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }

    if (InputType.Joystick == DFMUiRoot.InputMode) {
      DFMUiRoot.InputMode = InputType.Joystick;
    } else {
      DFMUiRoot.InputMode = InputType.Touch;
    }
  }
  private void HideInputUi()
  {
    try {
      UIManager.Instance.HideWindowByName("SkillBar");
      JoyStickInputProvider.JoyStickEnable = false;
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private static InputType inputMode = InputType.Joystick;
  public static InputType InputMode
  {
    get
    {
      return inputMode;
    }
    set
    {
      inputMode = value;
      if (InputType.Joystick == inputMode) {
        JoyStickInputProvider.JoyStickEnable = true;
        TouchManager.GestureEnable = false;
      } else {
        TouchManager.GestureEnable = true;
        JoyStickInputProvider.JoyStickEnable = false;
      }
    }
  }
  //***********************
  private void ShowYesOrNot(string message, Action<bool> dofunction)
  {
    try {
      if (message != null && dofunction != null) {
        GameObject go = DashFire.ResourceSystem.GetSharedResource("UI/ConfirmDlg/ConfirmDlg") as GameObject;
        if (go != null) {
          go = NGUITools.AddChild(gameObject, go);
          if (go != null) {
            YesOrNot yon = go.GetComponent<YesOrNot>();
            yon.SetMessageAndDO(message, dofunction);
          }
          NGUITools.SetActive(go, true);
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  private void ShowNewbieGuide(List<int> idlist)
  {
    if (idlist != null) {
      NewbieGuideManager ngm = gameObject.AddComponent<NewbieGuideManager>();
      if (ngm != null) {
        ngm.SetMySelf(ngm, transform);
        ngm.DoInitGuid(idlist);
      }
    }
  }

  private int beatnum = 0;
  private GameObject loading = null;
  private float timeRecycle = 10.0f;
  private GameObject m_StoryDlgSmallGO = null;
  private GameObject m_StoryDlgBigGO = null;
  private GameObject DynamicWidgetPanel = null;
  private GameObject ScreenTipPanel = null;
  public SceneTypeEnum m_SceneType = SceneTypeEnum.TYPE_UNKNOWN;
  public SceneSubTypeEnum m_SubSceneType = SceneSubTypeEnum.TYPE_UNKNOWN;
  private int m_EnemyNum = 0;
  static public List<GfxUserInfo> UserInfoForUI = new List<GfxUserInfo>();
  static public Dictionary<GameObject, GameObject> NpcGameObjectS = new Dictionary<GameObject, GameObject>();
  static public int NowSceneID = 0;
  //pve战斗信息单独处理,type = 0,1,2,3(被击，防御，挑战，突袭)
  static public GameObject PveFightInfo = null;
  private void SetPveFightInfo(int type, int num0, int num1, int num2)
  {
    if (PveFightInfo == null) {
      string path = UIManager.Instance.GetPathByName("PveFightInfo");
      GameObject go = DashFire.ResourceSystem.GetSharedResource(path) as GameObject;
      if (go != null) {
        go = NGUITools.AddChild(gameObject, go);
        if (go != null) {
          PveFightInfo = go;
          PveFightInfo pfi = go.GetComponent<PveFightInfo>();
          if (pfi != null) {
            pfi.SetInitInfo(type, num0, num1, num2);
          }
        }
      }
    } else {
      //if (type == 0 || type == 1) {
      PveFightInfo pfi = PveFightInfo.GetComponent<PveFightInfo>();
      if (pfi != null) {
        pfi.SetInitInfo(type, num0, num1, num2);
      }
      //}
      Transform tf = PveFightInfo.transform.Find("TimeOrSome");
      if (tf != null) {
        NGUITools.SetActive(tf.gameObject, true);
      }
    }
  }

  //小怪盾条
  private void CreateMonestSheildBlood(int actorid, int type)
  {
    try {
      GameObject pargo = DashFire.LogicSystem.GetGameObject(actorid);
      if (pargo != null) {
        string path = UIManager.Instance.GetPathByName("Sheild");
        GameObject go = DashFire.ResourceSystem.GetSharedResource(path) as GameObject;
        if (go != null && DynamicWidgetPanel != null) {
          go = NGUITools.AddChild(DynamicWidgetPanel, go);
          if (go != null) {
            Sheild dun = go.GetComponent<Sheild>();
            if (dun != null) {
              dun.InitSheild(actorid, type, pargo);
            }
          }
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
}

