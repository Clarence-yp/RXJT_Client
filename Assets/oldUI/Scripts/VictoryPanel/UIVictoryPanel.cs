using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DashFire;

public class UIVictoryPanel : MonoBehaviour
{
  public UISprite[] spStarsArr = new UISprite[3];
  public GameObject goItemLabel = null;//prefab
  public UIGrid gridForInfo = null;//用于组织击杀数、战斗用时等信息
  public GameObject goForAward = null;//用于组织奖励信息
  public GameObject goMasterAward = null;
  public UILabel lblPlayerCurrentExp = null;
  public UILabel lblPlayerLevel = null;
  public UILabel lblAwardMoney = null;//获得的奖励
  public UILabel lblAwardExp = null;
  public UILabel lblMasterHint = null;
  public UILabel lblFirstComplete = null;
  public UILabel lblAwardName = null;
  public UISprite spAwardType = null;
  public UITexture texAward = null;
  public UIProgressBar progressBar = null;//经验条
  public float m_DeltaForInfo = 0.6f;//每隔0.6s显示一条信息
  private float m_CountDown = 0.0f;
  private int m_CountForInfo = 0;//已经显示信息的条数
  private int m_FinishedCount = 0;//完成Item的次数
  private bool m_CanPlayStar = false;//是否开始播放星星动画
  private int m_StarCount = 0;//显示几个星星
  public AnimationCurve CurveForStar;//星星动画曲线
  public Vector3 ScaleOfToForStar = new Vector3(1.2f, 1.2f, 1);
  public float DurationForStar = 1.0f;//每个星星动画的播放时间
  public float DeltaForStar = 0.4f;//两个星星播放的时间间隔
  private int m_StarFinishedCount = 0;

  private bool m_CanProgress = false;
  private List<object> m_EventList = new List<object>();
  private int m_PlayerCurrentLevel = 1;
  private int m_PlayerExp = 0;
  private int m_DropExp = 0;
  private int m_SceneId = -1;
  private const int m_MaxLevel = 100;
  public int OffsetV = -60;
  public int TransOffsetInfoItem = 400;
  public int TransOffsetStar = 300;
  public int TransOffsetAward = 273;

  private bool m_IsFirstComplete = true;
  private SceneSubTypeEnum subType = SceneSubTypeEnum.TYPE_ELITE;
  //public 
  // Use this for initialization
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
    object obj = null;
    obj = LogicSystem.EventChannelForGfx.Subscribe<int, int, int, int, int, int, int, bool>("ge_victory_panel", "ui", InitVictoryPanel);
    if (obj != null) m_EventList.Add(obj);
    obj = LogicSystem.EventChannelForGfx.Subscribe("ge_ui_unsubscribe", "ui", UnSubscribe);
    if (obj != null) m_EventList.Add(obj);
  }
  void Start()
  {
  }
  void OnEnable()
  {
    //结束画面开始前结束普通攻击
    GameObject go = UIManager.Instance.GetWindowGoByName("SkillBar");
    if (go != null) {
      SkillBar sBar = go.GetComponent<SkillBar>();
      if (sBar != null) {
        sBar.StopAttack();
        if (DashFire.LogicSystem.PlayerSelf != null)
          GfxModule.Skill.GfxSkillSystem.Instance.StopAttack(DashFire.LogicSystem.PlayerSelf);
      }
    }
  }
  // Update is called once per frame
  void Update()
  {
    if (m_CountForInfo < 4) {
      if (m_CountDown <= 0.0f) {
        ShowCombatIntoItem(m_CountForInfo++);
        m_CountDown = m_DeltaForInfo;
      } else {
        m_CountDown -= RealTime.deltaTime;
      }
    }
    if (m_CanPlayStar) {
      StartCoroutine(ShowStars(DeltaForStar));
      m_CanPlayStar = false;
    }
    if (m_CanProgress) {
      m_CanProgress = false;
      StartCoroutine(SetPlayerExp(m_PlayerExp - m_DropExp, m_PlayerExp));
    }
  }
  private void InitVictoryPanel(int sceneId, int maxHit, int beHittTimes, int diedTimes, int time, int exp, int gold, bool isFirstComplete)
  {
    try {
      if (texAward != null) NGUITools.SetActive(texAward.gameObject, false);
      m_IsFirstComplete = isFirstComplete;
      m_SceneId = sceneId;
      if (lblAwardExp != null) lblAwardExp.text = "+" + exp.ToString();
      if (lblAwardMoney != null) lblAwardMoney.text = "+" + gold.ToString();
      values[0] = maxHit;
      values[1] = beHittTimes;
      values[2] = diedTimes;
      values[3] = time;
      RoleInfo role_info = LobbyClient.Instance.CurrentRole;
      if (role_info != null) {
        m_PlayerCurrentLevel = role_info.Level;
        m_PlayerExp = role_info.Exp;
        Data_SceneDropOut dropCfg = null;
        Data_SceneConfig sceneCfg = SceneConfigProvider.Instance.GetSceneConfigById(sceneId);
        if (sceneCfg != null) {
          if (sceneCfg.m_SubType == (int)SceneSubTypeEnum.TYPE_ELITE)
            subType = SceneSubTypeEnum.TYPE_ELITE;
          if (sceneCfg.m_SubType == (int)SceneSubTypeEnum.TYPE_STORY)
            subType = SceneSubTypeEnum.TYPE_STORY;
          dropCfg = SceneConfigProvider.Instance.GetSceneDropOutById(sceneCfg.m_DropId);
        }
        if (dropCfg != null) m_DropExp = dropCfg.m_Exp;
      }
      UIManager.Instance.ShowWindowByName("VictoryPanel");
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  //服务器已经算完了等级，所以要算出升级之前的级数，用于UI表现
  private int GetLevelByExp(int curExp, int addedExp)
  {
    int lastExp = curExp - addedExp;
    int level = 1;
    while (level <= m_MaxLevel) {
      PlayerLevelupExpConfig expCfg = PlayerConfigProvider.Instance.GetPlayerLevelupExpConfigById(level);
      if (expCfg != null && expCfg.m_ConsumeExp > lastExp) {
        return level;
      }
      level++;
    }
    return m_MaxLevel;
  }
  //战斗胜利后显示击杀数等
  public void ShowCombatIntoItem(int index)
  {
    if (index >= values.Length) return;
    if (goItemLabel != null && gridForInfo != null) {
      GameObject item = NGUITools.AddChild(gridForInfo.gameObject, goItemLabel);
      UIVictoryItem lbl = item.GetComponent<UIVictoryItem>();
      if (lbl != null) {
        //共四条属性
        if (index == 3)
          lbl.SetValue(values[index], UIItemType.ForTime);
        else
          lbl.SetValue(values[index], UIItemType.Common);
        lbl.SetItemName(nameArr[index]);
      }
      TweenPosition tweenPos = item.GetComponent<TweenPosition>();
      if (tweenPos != null) {
        //-182是itemlabel的长度、
        tweenPos.from = new Vector3(-422, OffsetV * index, 0);
        tweenPos.to = new Vector3(0, OffsetV * index, 0);
        tweenPos.enabled = true;
      }
      EventDelegate.Add(tweenPos.onFinished, OnShowInfoFinished);
    }
  }
  public void OnShowInfoFinished()
  {
    m_FinishedCount++;
    if (m_FinishedCount == c_InfoItemNum) {
      TransLeftToRight();
      m_FinishedCount = 0;
    }
  }
  //从左向右移动Award
  public void TransLeftToRight()
  {
    if (goForAward != null) {
      if (lblPlayerCurrentExp != null) NGUITools.SetActive(lblPlayerCurrentExp.gameObject, false);
      Transform tsAward = goForAward.transform;
      Vector3 pos = tsAward.localPosition;
      Vector3 targetPos = new Vector3(pos.x + 426, pos.y, 0);
      TweenPosition tweenPos = TweenPosition.Begin(tsAward.gameObject, 0.3f, targetPos);
      if (tweenPos != null) EventDelegate.Add(tweenPos.onFinished, OnLeft2RightFinished);
    }
  }
  //左--右移动结束
  public void OnLeft2RightFinished()
  {
    if (goForAward != null) {
      TweenPosition tweenPos = goForAward.GetComponent<TweenPosition>();
      if (tweenPos != null) Destroy(tweenPos);
    }
    m_CanProgress = true;
  }

  //显示小星星=、=
  public IEnumerator ShowStars(float delta)
  {
    for (int index = 0; index < m_StarCount; ++index) {
      if (index < spStarsArr.Length && spStarsArr[index] != null) {
        UISprite spStar = spStarsArr[index];
        spStar.spriteName = "da-xing-xing1";
        GameObject go = spStar.gameObject;
        TweenScale scale = go.AddComponent<TweenScale>();
        scale.to = ScaleOfToForStar;
        scale.duration = DurationForStar;
        scale.animationCurve = CurveForStar;
        EventDelegate.Add(scale.onFinished, OnShowStarFinished);
        yield return new WaitForSeconds(delta);
      }
    }
    yield return new WaitForSeconds(0f);
  }
  public void OnShowStarFinished()
  {
    if (++m_StarFinishedCount == m_StarCount) {
      StartCoroutine(ShowMasterAward());
    }
  }
  public IEnumerator ShowMasterAward()
  {
    yield return new WaitForSeconds(0.2f);
    if (m_IsFirstComplete) {
      if (lblMasterHint != null) NGUITools.SetActive(lblMasterHint.gameObject, true);
      if (lblFirstComplete != null) NGUITools.SetActive(lblFirstComplete.gameObject, false);
      SetAwardItem();
    }
    yield return new WaitForSeconds(1.5f);
    UIManager.Instance.HideWindowByName("VictoryPanel");
    UIManager.Instance.ShowWindowByName("CombatWin");
  }
  //UI上移
  public void MoveChildGameObject()
  {
    SetAwardVisible(false);
    //Transform tsStar = this.transform.Find("ScrollView/Stars");
    Transform tsAward = this.transform.Find("ScrollView/Award");
    Transform tsCombatInfo = this.transform.Find("ScrollView/CombatInfo");
    if (tsAward == null || tsCombatInfo == null) {
      Debug.Log("Something is null in UI.");
    } else {
      Vector3 pos = tsCombatInfo.localPosition;
      Vector3 targetPos = new Vector3(pos.x, pos.y + TransOffsetInfoItem, 0);
      TweenPosition.Begin(tsCombatInfo.gameObject, 0.3f, targetPos);
      //pos = tsStar.localPosition;
      //targetPos = new Vector3(pos.x, pos.y + TransOffsetStar, 0);
      //TweenPosition.Begin(tsStar.gameObject, 0.3f, targetPos);
      //NGUITools.SetActive(tsAward.gameObject, true);
      pos = tsAward.localPosition;
      targetPos = new Vector3(pos.x, pos.y + TransOffsetAward, 0);
      TweenPosition tweenPos = TweenPosition.Begin(tsAward.gameObject, 0.3f, targetPos);
      if (tweenPos != null)
        EventDelegate.Add(tweenPos.onFinished, OnMoveFinished);
    }
  }
  //上移结束
  public void OnMoveFinished()
  {
    RoleInfo role_info = LobbyClient.Instance.CurrentRole;
    if (role_info != null) {
      if (role_info.SceneInfo != null && role_info.SceneInfo.ContainsKey(m_SceneId))
        m_StarCount = role_info.SceneInfo[m_SceneId];//星级
    }
    if (!m_IsFirstComplete) {
      for (int i = 0; i < spStarsArr.Length; ++i) {
        if (spStarsArr[i] != null)
          NGUITools.SetActive(spStarsArr[i].gameObject, true);
      }
      m_CanPlayStar = true;
    } else {
      //星级奖励
      if (subType == SceneSubTypeEnum.TYPE_ELITE) {
        for (int i = 0; i < spStarsArr.Length; ++i) {
          if (spStarsArr[i] != null)
            NGUITools.SetActive(spStarsArr[i].gameObject, true);
        }
        m_CanPlayStar = true;
      } else {
        //剧情首次通关奖励
        if (lblMasterHint != null) NGUITools.SetActive(lblMasterHint.gameObject, false);
        if (lblFirstComplete != null) NGUITools.SetActive(lblFirstComplete.gameObject, true);
        SetAwardItem();
        StartCoroutine(ReturnToMainCity());
      }
    }
    //todo：星级如何确定？
  }
  public void SetPlayerLevel(int level)
  {
    if (lblPlayerLevel != null) {
      lblPlayerLevel.text = "[b]Lv." + level.ToString();
    }
  }
  //经验值动画
  public IEnumerator SetPlayerExp(int fromExp, int toExp)
  {
    if (lblPlayerCurrentExp != null) NGUITools.SetActive(lblPlayerCurrentExp.gameObject, true);
    int startLevel = GetLevelByExp(m_PlayerExp, m_DropExp);
    SetPlayerLevel(startLevel);
    //if (maxExp <= 0) yield break;
    int baseExp = GetTotleExpByLevel(startLevel - 1);
    int maxExp = GetTotleExpByLevel(startLevel) - baseExp;
    for (int exp = fromExp; exp <= toExp; ) {
      int level = GetLevelByExp(exp, 0);
      if (level > startLevel) {
        startLevel = level;
        SetPlayerLevel(level);
        baseExp = GetTotleExpByLevel(level - 1);
        maxExp = GetLevelUpExpById(level);
      }
      if (maxExp > 0) {
        if (lblPlayerCurrentExp != null) lblPlayerCurrentExp.text = "[b]" + (exp - baseExp) + "/" + maxExp;
        if (progressBar != null && maxExp != 0) progressBar.value = (exp - baseExp) / (float)maxExp;
      }
      //以20贞为基准,保证最长时间为5s
      if (maxExp > 100) {
        exp += maxExp / 100;
      } else {
        exp++;
      }
      yield return new WaitForSeconds(0.0001f);
    }
    if (startLevel < m_PlayerCurrentLevel) {
      SetPlayerLevel(m_PlayerCurrentLevel);
      baseExp = GetTotleExpByLevel(m_PlayerCurrentLevel - 1);
      maxExp = GetTotleExpByLevel(m_PlayerCurrentLevel) - baseExp;
      if (maxExp > 0) {
        if (lblPlayerCurrentExp != null) lblPlayerCurrentExp.text = "[b]" + (toExp - baseExp) + "/" + maxExp;
        if (progressBar != null && maxExp != 0) progressBar.value = (toExp - baseExp) / (float)maxExp;
      }
    }
    if (m_IsFirstComplete == true || subType == SceneSubTypeEnum.TYPE_ELITE) {
      yield return new WaitForSeconds(1f);
      m_CanProgress = false;
      MoveChildGameObject();
    } else {
      yield return new WaitForSeconds(1f);
      UIManager.Instance.HideWindowByName("VictoryPanel");
      UIManager.Instance.ShowWindowByName("CombatWin");
    }
  }
  private int GetLevelUpExpById(int level)
  {
    PlayerLevelupExpConfig expCfgHigh = PlayerConfigProvider.Instance.GetPlayerLevelupExpConfigById(level);
    if (level == 1 && expCfgHigh != null)
      return expCfgHigh.m_ConsumeExp;
    PlayerLevelupExpConfig expCfgLow = PlayerConfigProvider.Instance.GetPlayerLevelupExpConfigById(level - 1);
    if (expCfgHigh != null && expCfgLow != null) {
      return expCfgHigh.m_ConsumeExp - expCfgLow.m_ConsumeExp;
    }
    return 0;
  }
  private int GetTotleExpByLevel(int level)
  {
    if (level == 0) return 0;
    int exp = 0;
    PlayerLevelupExpConfig expCfg = PlayerConfigProvider.Instance.GetPlayerLevelupExpConfigById(level);
    if (expCfg != null) exp = expCfg.m_ConsumeExp;
    return exp;
  }
  private void SetAwardVisible(bool visible)
  {
    if (goMasterAward != null)
      NGUITools.SetActive(goMasterAward, visible);
    for (int i = 0; i < spStarsArr.Length; ++i) {
      if (spStarsArr[i] != null)
        NGUITools.SetActive(spStarsArr[i].gameObject, visible);
    }
  }
  private void SetAwardItem()
  {
    RoleInfo role_info = LobbyClient.Instance.CurrentRole;
    if (role_info != null) {
      if (role_info.SceneInfo != null && role_info.SceneInfo.ContainsKey(m_SceneId)) {
        int level = role_info.SceneInfo[m_SceneId];//星级
        Data_SceneConfig sceneCfg = SceneConfigProvider.Instance.GetSceneConfigById(m_SceneId);
        if (sceneCfg == null) return;
        int dropId = sceneCfg.GetCompletedRewardId(level);
        Data_SceneDropOut dropCfg = SceneConfigProvider.Instance.GetSceneDropOutById(dropId);
        if (dropCfg != null && dropCfg.m_ItemIdList.Count > 0) {
          int itemId = dropCfg.m_ItemIdList[0];
          ItemConfig itemCfg = ItemConfigProvider.Instance.GetDataById(itemId);
          if (itemCfg == null) return;
          if (spAwardType != null) spAwardType.spriteName = "EquipFrame" + itemCfg.m_PropertyRank;
          if (lblAwardName != null) lblAwardName.text = itemCfg.m_ItemName;
          Texture tex = ResourceSystem.GetSharedResource(itemCfg.m_ItemTrueName) as Texture;
          if (texAward != null) {
            if (tex != null) {
              texAward.mainTexture = tex;
            } else {
              DashFire.ResLoadAsyncHandler.LoadAsyncItem(itemCfg.m_ItemTrueName, texAward);
            }
            NGUITools.SetActive(texAward.gameObject, true);
          }
        }
      }
    }
    if (goMasterAward != null) NGUITools.SetActive(goMasterAward, true);

  }
  private IEnumerator ReturnToMainCity()
  {
    yield return new WaitForSeconds(1f);
    UIManager.Instance.HideWindowByName("VictoryPanel");
    UIManager.Instance.ShowWindowByName("CombatWin");
  }
  private const int c_InfoItemNum = 4;
  private string[] nameArr = new string[c_InfoItemNum]{
    "最高连击", 
    "被击次数",
    "死亡次数",
    "通关时间"
  };
  private int[] values = new int[c_InfoItemNum];
}
public enum UIItemType
{
  Common = 0,
  ForTime = 1,
}
