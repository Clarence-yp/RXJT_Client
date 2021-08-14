using UnityEngine;
using DashFire;
using System;
using System.Collections;
using System.Collections.Generic;

public class UISceneAward : MonoBehaviour
{

  public GameObject goCommonAward = null;
  public GameObject goMasterAward = null;
  //星级挑战奖励
  public UILabel lblConditionNum = null;//几星条件
  public UILabel lblAwardCondition = null;//通过条件
  public UILabel lblRecommendFighting = null;
  public UILabel lblStarNum = null;
  public UILabel lblItemName = null;//普通关卡通关奖励物品名
  public UISprite spRank = null;
  public UISprite []spRankArr = new UISprite[3];
  public UISprite[] spArrowArr = new UISprite[2];
  public UISprite[] spFinishedArr = new UISprite[3];
  public UITexture[] texItemArr = new UITexture[3];
  public GameObject goGuide = null;
  //普通关卡奖励
  public GameObject goFinished = null;//已发放
  public UITexture texItem = null;
  public UISceneIntroduceSlot[] masterAwardSlotArr = new UISceneIntroduceSlot[3];
  public UISceneIntroduceSlot commonAwardSlot = null;

  private const string c_BrightArrow = "sheng-ji-jian-tou1";
  private const string c_AshArrow = "sheng-ji-jian-tou2";
  private Vector3[] m_GuidePosArr = new Vector3[3]{
    new Vector3(70,-132.05f,0),
    new Vector3(197.38f,-132f,0),
    new Vector3(328,-132f,0)
  };
  // Use this for initialization
  void Start()
  {
  }

  // Update is called once per frame
  void Update()
  {

  }
  //显示奖励信息
  public void ShowAwardInfo(int sceneId, SubSceneType subType, int grade)
  {
    Data_SceneConfig sceneCfg = SceneConfigProvider.Instance.GetSceneConfigById(sceneId);
    if (subType == SubSceneType.Common) {
      //普通副本
      NGUITools.SetActive(goCommonAward, true);
      NGUITools.SetActive(goMasterAward, false);
      if (goFinished != null) {
        if (grade > 0)
          NGUITools.SetActive(goFinished, true);
        else {
          NGUITools.SetActive(goFinished, false);
        }
      }
      int dropId = -1;
      if (sceneCfg == null) return;
      if (sceneCfg.m_CompletedRewards != null && sceneCfg.m_CompletedRewards.Count > 0)
        dropId = sceneCfg.m_CompletedRewards[0];
      Data_SceneDropOut dropCfg = SceneConfigProvider.Instance.GetSceneDropOutById(dropId);
      if (dropCfg != null && dropCfg.m_ItemIdList.Count > 0) {
        int itemId = dropCfg.m_ItemIdList[0];
        if (commonAwardSlot != null) commonAwardSlot.SetId(itemId);
        ItemConfig itemCfg = ItemConfigProvider.Instance.GetDataById(itemId);
        if (itemCfg == null) return;
        if (spRank != null) spRank.spriteName = "EquipFrame" + itemCfg.m_PropertyRank;
        if (lblItemName != null) lblItemName.text = itemCfg.m_ItemName;
        Texture tex = ResourceSystem.GetSharedResource(itemCfg.m_ItemTrueName) as Texture;
        if (texItem != null) {
          if (tex != null) {
            texItem.mainTexture = tex;
          } else {
            DashFire.ResLoadAsyncHandler.LoadAsyncItem(itemCfg.m_ItemTrueName, texItem);
          }
        }
      }
    } else {
      //精英副本
      NGUITools.SetActive(goCommonAward, false);
      NGUITools.SetActive(goMasterAward, true);
      if (sceneCfg == null || sceneCfg.m_CompletedRewards == null) return;
      for (int i = 0; i < sceneCfg.m_CompletedRewards.Count; ++i) {
        int dropId = sceneCfg.m_CompletedRewards[i];
        Data_SceneDropOut dropCfg = SceneConfigProvider.Instance.GetSceneDropOutById(dropId);
        if (dropCfg != null && dropCfg.m_ItemIdList.Count > 0) {
          int itemId = dropCfg.m_ItemIdList[0];
          if (i < masterAwardSlotArr.Length && masterAwardSlotArr[i] != null)
            masterAwardSlotArr[i].SetId(itemId);
          ItemConfig itemCfg = ItemConfigProvider.Instance.GetDataById(itemId);
          if (itemCfg == null) continue;
          if (i<spRankArr.Length &&  spRankArr[i]!= null) spRankArr[i].spriteName = "EquipFrame" + itemCfg.m_PropertyRank;
          Texture tex = ResourceSystem.GetSharedResource(itemCfg.m_ItemTrueName) as Texture;
          if (i < texItemArr.Length && texItemArr[i] != null)
            if (tex != null) {
              texItemArr[i].mainTexture = tex;
            } else {
              DashFire.ResLoadAsyncHandler.LoadAsyncItem(itemCfg.m_ItemTrueName, texItemArr[i]);
            }
        }
      }
      for (int i = 0; i < spArrowArr.Length; ++i) {
        if (i + 1 < grade) {
          if (spArrowArr[i] != null) spArrowArr[i].spriteName = c_BrightArrow;
        } else {
          if (spArrowArr[i] != null) spArrowArr[i].spriteName = c_AshArrow;
        }
      }
      //指示标志
      if (grade < 3 && grade >= 0 && lblStarNum != null) {
        NGUITools.SetActive(goGuide, true);
        lblStarNum.text = (grade + 1).ToString();
        if (grade < m_GuidePosArr.Length)
          goGuide.transform.localPosition = m_GuidePosArr[grade];
      } else {
        NGUITools.SetActive(goGuide, false);
      }
      for (int i = 0; i < spFinishedArr.Length; ++i) {
        if (i < grade) {
          if (spFinishedArr[i] != null) NGUITools.SetActive(spFinishedArr[i].gameObject, true);
        } else {
          if (spFinishedArr[i] != null) NGUITools.SetActive(spFinishedArr[i].gameObject, false);
        }
      }
      //推荐战力
      if (lblRecommendFighting != null) lblRecommendFighting.text = sceneCfg.m_RecommendFighting.ToString();
      SetAwardCondition(grade, sceneCfg);
    }
  }
  //设置通关条件
  private void SetAwardCondition(int grade, Data_SceneConfig sceneCfg)
  {
    if (sceneCfg == null) return;
    if (grade < 3 && grade >= 0) {
      string CHN = StrDictionaryProvider.Instance.GetDictString(302);
      CHN = string.Format(CHN, grade + 1);
      if (lblConditionNum != null) lblConditionNum.text = CHN;
      switch (grade) {
        case 0:
          CHN = StrDictionaryProvider.Instance.GetDictString(303);
          if (lblAwardCondition != null) lblAwardCondition.text = CHN;
          break;
        case 1:
          CHN = StrDictionaryProvider.Instance.GetDictString(304);
          CHN = string.Format(CHN, sceneCfg.m_CompletedTime);
          if (lblAwardCondition != null) lblAwardCondition.text = CHN;
          break;
        case 2:
          CHN = StrDictionaryProvider.Instance.GetDictString(305);
          CHN = string.Format(CHN, sceneCfg.m_CompletedHitCount);
          if (lblAwardCondition != null) lblAwardCondition.text = CHN;
          break;
      }
    } else {
      if (lblConditionNum != null) lblConditionNum.text = "";
      if (lblAwardCondition != null) lblAwardCondition.text = "";
    }
  }
}
