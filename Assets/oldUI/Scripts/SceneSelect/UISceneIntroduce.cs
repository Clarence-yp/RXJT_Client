using UnityEngine;
using DashFire;
using System;
using System.Collections;
using System.Collections.Generic;

public class UISceneIntroduce : MonoBehaviour {
  private int m_SceneId = -1;
  public UILabel lblIntroduce1 = null;
  public UILabel lblSceneIndex = null;//章节序号
  public UILabel lblFightMy = null; //
  public UILabel lblFightRecommend = null;
  public UILabel lblCostPower = null;//体力消耗
  public UILabel lblTitle = null;
  public UILabel lblCurStamina = null;//当前体力
  public UILabel lblChallengeInfo = null;//剩余挑战次数

  public UILabel lblAwardExp = null;
  public UILabel lblAwardCoin = null;
  public UILabel lblAwardItem = null;
  public UILabel lblAwardItemName = null;
  public UITexture texAwardItem = null;
  public UISprite spRank = null;
  public UISprite[] starArr = new UISprite[3];
  public UISceneAward uiSceneAward = null;
  private const string c_AshStar = "da-xing-xing2";
  private const string c_BrightStar = "da-xing-xing1";
  private List<object> m_EventList = new List<object>();
  private int m_CompleteCount = 0;
  private SubSceneType m_SubSceneType = SubSceneType.UnKnown;
  public void UnSubscribe()
  {
    try {
      foreach (object obj in m_EventList) {
        if (null != obj) LogicSystem.EventChannelForGfx.Unsubscribe(obj);
      }
      m_EventList.Clear();
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  void Awake()
  {
   object obj = LogicSystem.EventChannelForGfx.Subscribe<int,int,SubSceneType>("ge_init_sceneintroduce","ui", this.InitSceneIntroduce);
   if (null != obj) m_EventList.Add(obj);
   obj = LogicSystem.EventChannelForGfx.Subscribe("ge_ui_unsubscribe", "ui", UnSubscribe);
   if (obj != null) m_EventList.Add(obj);
   
  }
	// Use this for initialization
	void Start () {
		UIManager.Instance.HideWindowByName("SceneIntroduce");
	}
	// Update is called once per frame
	void Update () {
	}
  public void InitSceneIntroduce(int sceneId,int grade, SubSceneType subType)
  {
    try {
      
      if (subType == SubSceneType.Common) {
        m_SubSceneType = SubSceneType.Common;
        for (int i = 0; i < starArr.Length; ++i) {
          if (starArr[i] != null) NGUITools.SetActive(starArr[i].gameObject, false);
        }
      } else {
        m_SubSceneType = SubSceneType.Master;
        for (int i = 0; i < starArr.Length; ++i) {
          if (starArr[i] != null) NGUITools.SetActive(starArr[i].gameObject,true);
          if (i < grade) {
            if (starArr[i] != null) starArr[i].spriteName = c_BrightStar;
          } else {
            if (starArr[i] != null) starArr[i].spriteName = c_AshStar;
          }
        }
      }
      if (uiSceneAward != null) uiSceneAward.ShowAwardInfo(sceneId, subType, grade);
      m_SceneId = sceneId;
      Data_SceneConfig sceneCfg = SceneConfigProvider.Instance.GetSceneConfigById(m_SceneId);
      RoleInfo role_info = LobbyClient.Instance.CurrentRole;
      if (sceneCfg != null) {
        //显示剩余次数
        if (subType == SubSceneType.Master) {
          if(lblChallengeInfo!=null && lblChallengeInfo.transform.parent!=null)
            NGUITools.SetActive(lblChallengeInfo.transform.parent.gameObject, true);
          int complete_count = role_info.GetCompletedSceneCount(m_SceneId);
		      m_CompleteCount =  complete_count;
          complete_count = complete_count > 3 ? 3 : complete_count;
          if (lblChallengeInfo != null) lblChallengeInfo.text = (3 - complete_count) + " / 3";
        } else {
          if (lblChallengeInfo != null && lblChallengeInfo.transform.parent != null)
            NGUITools.SetActive(lblChallengeInfo.transform.parent.gameObject,false);
        }
        SetName(sceneCfg.m_SceneName);
        SetRecommendFight(sceneCfg.m_RecommendFighting);
        SetCostStatima(sceneCfg.m_CostStamina);
        if (lblSceneIndex != null) lblSceneIndex.text = (1+sceneCfg.m_Order).ToString();
        string des = sceneCfg.m_SceneDescription.Replace("[\\n]","\n");
        if (lblIntroduce1 != null) lblIntroduce1.text = des;
        if (role_info != null)
          SetFightingScore((int)role_info.FightingScore);
        if (lblCurStamina != null) lblCurStamina.text = role_info.CurStamina.ToString();
        //设置掉落数据
        Data_SceneDropOut dropCfg = SceneConfigProvider.Instance.GetSceneDropOutById(sceneCfg.m_DropId);
        if (dropCfg != null) {
          if (lblAwardCoin != null) lblAwardCoin.text = dropCfg.m_GoldSum.ToString() ;
          //if (lblAwardDiamond != null) lblAwardDiamond.text = dropCfg.m_ItemCount.ToString();
          if (lblAwardExp != null) lblAwardExp.text = dropCfg.m_Exp.ToString();
          SetAwardItem(dropCfg.m_ItemIdList, dropCfg.m_ItemCountList);
        }
      }
    }
    catch (Exception ex) {
      LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  public void SetAwardItem(List<int> items, List<int> counts)
  {
    if (items == null || counts == null) return;
    if (items.Count <= 0 || counts.Count <= 0) return;
    int itemId = items[0];
    int itemCount = counts[0];
    ItemConfig itemCfg = ItemConfigProvider.Instance.GetDataById(itemId);
    if (itemCfg != null) {
      if(spRank!=null)spRank.spriteName = "EquipFrame" + itemCfg.m_PropertyRank;
      string path = itemCfg.m_ItemTrueName;
      Texture tex = ResourceSystem.GetSharedResource(path) as Texture;
      if (texAwardItem != null)
      {
        UISceneIntroduceSlot scriptSlot = texAwardItem.GetComponent<UISceneIntroduceSlot>();
        if (scriptSlot != null) scriptSlot.SetId(itemId);
        if (tex != null) {
          texAwardItem.mainTexture = tex;
        } else {
          DashFire.ResLoadAsyncHandler.LoadAsyncItem(path, texAwardItem);
        }
      }
      if (lblAwardItem != null) lblAwardItem.text = itemCount.ToString();
      if (lblAwardItemName != null) lblAwardItemName.text = itemCfg.m_ItemName;
    }
  }

  //进入副本
  public void OnChallengeBtnClick()
  {
	if(m_CompleteCount>=3 && m_SubSceneType == SubSceneType.Master)
	{
		    string chn_desc = StrDictionaryProvider.Instance.GetDictString(308);
        DashFire.LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", chn_desc, "OK", null, null, null, false);
		return;
	}
    RoleInfo role_info = LobbyClient.Instance.CurrentRole;
    Data_SceneConfig config_data = SceneConfigProvider.Instance.GetSceneConfigById(m_SceneId);
    if (null != role_info && -1 != m_SceneId && null != config_data) {
      if (role_info.CurStamina >= config_data.m_CostStamina) {
        LogicSystem.EventChannelForGfx.Publish("ge_ui_connect_hint", "ui", true, true);
      }
    }
    if (m_SceneId != -1) {
      DashFire.LogicSystem.PublishLogicEvent("ge_select_scene", "lobby", m_SceneId);
    } else {
      Debug.LogError("sceneId is -1!!");
    }
  }
  //扫荡事件
  public void OnWipeoutBtnClick()
  {

  }
  //
  public void OnCloseBtnClick()
  {
    UIManager.Instance.HideWindowByName("SceneIntroduce");
    //UIManager.Instance.ShowWindowByName("SceneSelect");
  }
  //
  public void OnBackgroundButtonClick()
  {
    UIManager.Instance.HideWindowByName("SceneIntroduce");
  }
  public void SetName(string name)
  {
    if (lblTitle != null)
      lblTitle.text = "" + name + "";
  }
  //设置玩家当前的战斗力
  private void SetFightingScore(int value)
  {
    if (lblFightMy != null) {
      lblFightMy.text = "" + value + "";
    }
  }
  //设置推荐战力
  private void SetRecommendFight(int value)
  {
    if (lblFightRecommend != null) {
      lblFightRecommend.text = "" + value + "";
    }
  }
  //设置体力消耗
  private void SetCostStatima(int value)
  {
    if (lblCostPower != null) {
      lblCostPower.text = "" + value + "";
    }
  }
  public int SceneId
  {
    get { return m_SceneId; }
    set { m_SceneId = value; }
  }
}
