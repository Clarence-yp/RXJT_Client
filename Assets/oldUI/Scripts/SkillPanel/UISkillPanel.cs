using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DashFire;

public class UISkillPanel : MonoBehaviour
{
  public UISkillTitle uiSkillTitle = null;
  public UISkillInfo uiSkillInfo = null;
  public UISkillSetting uiSkillSetting = null;
  public UISkillStorage uiSkillStorage = null;

  public GameObject goSkillInfo = null;
  public GameObject goSkillLiftInfo = null;
  public GameObject goSkillLevelUpInfo = null;
  public GameObject goSkillInfoButton = null;
  public GameObject goSkillLevelUpButton = null;
  public GameObject goSkillLiftUpButton = null;
  private int m_CurrentClickSkillId = -1;
  //按钮
  public UIButton equipButton = null;

  private const string C_ConFirm = "OK";
  //
  private List<object> m_EventList = new List<object>();
  //
  public enum PanelType:int
  {
    SkillInfo =1,
    SkillLevelUpInfo =2,
    SkillLiftUpInfo=3
  }
  public void UnSubscribe()
  {
    try {
      foreach (object obj in m_EventList) {
        if (null != obj) LogicSystem.EventChannelForGfx.Unsubscribe(obj);
      }
      m_EventList.Clear();
    }
    catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  void Awake()
  {
    m_EventList.Clear();
    object obj = null;
    obj = LogicSystem.EventChannelForGfx.Subscribe<List<SkillInfo>>("ge_init_skills", "skill", InitSkills);
    if (null != obj) m_EventList.Add(obj);
    obj = LogicSystem.EventChannelForGfx.Subscribe<int, int, int, DashFire.Network.GeneralOperationResult>("ge_mount_skill", "skill", HandleLoadSkill);
    if (null != obj) m_EventList.Add(obj);
    obj = LogicSystem.EventChannelForGfx.Subscribe<int, int, SlotPosition, SlotPosition, DashFire.Network.GeneralOperationResult>("ge_swap_skill", "skill", HandleSwapSkill);
    if (null != obj) m_EventList.Add(obj);
    obj = LogicSystem.EventChannelForGfx.Subscribe<int, int, DashFire.Network.GeneralOperationResult>("ge_unmount_skill", "skill", OnUnloadedSkill);
    if (null != obj) m_EventList.Add(obj);
    obj = LogicSystem.EventChannelForGfx.Subscribe<int, int, int, DashFire.Network.GeneralOperationResult>("ge_upgrade_skill", "skill", HandleUpgradeSkill);
    if (null != obj) m_EventList.Add(obj);
    obj = LogicSystem.EventChannelForGfx.Subscribe<int, int, int, DashFire.Network.GeneralOperationResult>("ge_unlock_skill", "skill", HandleUnLockSkill);
    if (null != obj) m_EventList.Add(obj);
    obj = LogicSystem.EventChannelForGfx.Subscribe<int, int, DashFire.Network.GeneralOperationResult>("ge_lift_skill","skill",HandleLiftSkill);
    if (obj != null) m_EventList.Add(obj);
    obj = LogicSystem.EventChannelForGfx.Subscribe("ge_ui_unsubscribe", "ui", UnSubscribe);
    if (obj != null) m_EventList.Add(obj);
    LogicSystem.PublishLogicEvent("ge_request_skills", "ui");
  }

  // Use this for initialization
  void Start()
  {
    //装备按钮的初始状态不可点击
    SetEquipButtonState(false);
    //ShowInfoPanel(PanelType.SkillLevelUpInfo);
	  if(uiSkillStorage!=null && uiSkillStorage.skillStorageArr.Length>0){
			SetSkillInfo(uiSkillStorage.skillStorageArr[0].SkillId);
    }

    //暂定5级之后不再教学
    RoleInfo role_info = LobbyClient.Instance.CurrentRole;
    int level = 5;
    if (role_info != null) level = role_info.Level;
    if (UISkillGuide.Instance.GetSteps() < 3 && level<=5) {
      UISkillGuide.Instance.ShowGuideInSlotHandler = this.AddGuidePointing;
      UISkillGuide.Instance.HandleGuidePointToUnlock = this.GuidePointToUnlock;
      UISkillGuide.Instance.ShowGuidePointing();
    }
  }
  // Update is called once per frame
  void Update()
  {
    RoleInfo roleInfo = LobbyClient.Instance.CurrentRole;
    if (roleInfo != null) {
      SetMoneyCoin(Convert.ToInt32(roleInfo.Money));
      SetDiamond(Convert.ToInt32(roleInfo.Gold));
      SetHeroLevel(roleInfo.Level);
    }
  }

  //
  public void ShowInfoPanel(PanelType index)
  {
    if (goSkillInfo == null || goSkillLevelUpInfo == null || goSkillLiftInfo == null)
      return;
    switch (index) {
      case PanelType.SkillInfo:
        NGUITools.SetActive(goSkillInfo, true);
        NGUITools.SetActive(goSkillLevelUpInfo, false);
        NGUITools.SetActive(goSkillLiftInfo, false);
        NGUITools.SetActive(goSkillInfoButton, true);
        NGUITools.SetActive(goSkillLevelUpButton, false);
        NGUITools.SetActive(goSkillLiftUpButton, false);
        break;
     case PanelType.SkillLevelUpInfo:
        NGUITools.SetActive(goSkillInfo, false);
        NGUITools.SetActive(goSkillLevelUpInfo, true);
        NGUITools.SetActive(goSkillLiftInfo, false);
        NGUITools.SetActive(goSkillInfoButton, false);
        NGUITools.SetActive(goSkillLevelUpButton, true);
        NGUITools.SetActive(goSkillLiftUpButton, false);break;
      case PanelType.SkillLiftUpInfo:
        NGUITools.SetActive(goSkillInfo, false);
        NGUITools.SetActive(goSkillLevelUpInfo, false);
        NGUITools.SetActive(goSkillLiftInfo, true);
        NGUITools.SetActive(goSkillInfoButton, false);
        NGUITools.SetActive(goSkillLevelUpButton, false);
        NGUITools.SetActive(goSkillLiftUpButton, true);break;
    }
  }
  //装备技能返回的消息
  public void HandleLoadSkill(int presetIndex, int skillId, int slotPositon, DashFire.Network.GeneralOperationResult result)
  {
    try {
      if (DashFire.Network.GeneralOperationResult.LC_Succeed == result) {
        string msg = string.Format("LoadSkill:{0},{1},{2}", presetIndex, skillId, slotPositon);
        Debug.LogError(msg);
        uiSkillSetting.OnLoadedSkill(presetIndex, skillId, slotPositon);
        uiSkillStorage.OnLoadedSkill(presetIndex, skillId, slotPositon);
        if(UISkillGuide.Instance.GetSteps()<3)
          GuideStepComplete(3,skillId);
      } else {
        string chn_desc = StrDictionaryProvider.Instance.GetDictString(363);
        DashFire.LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", chn_desc, C_ConFirm, null, null, null, false);
      }
    }
    catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  //
  public void HandleSwapSkill(int presetIndex, int skillId, SlotPosition sourcePos, SlotPosition targetPos, DashFire.Network.GeneralOperationResult result)
  {
    try {
      if (DashFire.Network.GeneralOperationResult.LC_Succeed == result) {
        string msg = string.Format("OnSwapSkill:{0},{1}", sourcePos, targetPos);
        Debug.LogError(msg);
        int targetSkillId = uiSkillSetting.GetSkillId((int)targetPos);
        uiSkillSetting.ExchangeSlot(sourcePos, targetPos);
        uiSkillStorage.OnExchangeSkill(skillId, targetSkillId, sourcePos, targetPos);
      } else {
        string chn_desc = StrDictionaryProvider.Instance.GetDictString(364);
        DashFire.LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", chn_desc, C_ConFirm, null, null, null, false);
      }
    }
    catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  //
  public void HandleUpgradeSkill(int presetIndex, int curSkillId, int curSkillLevel, DashFire.Network.GeneralOperationResult result)
  {
    try {
      if (DashFire.Network.GeneralOperationResult.LC_Succeed == result) {
        //string msg = string.Format("HandleUpGradeSkill in UI presetIndex:{0}, skillId:{1}", presetIndex, curSkillId);
        //Debug.Log(msg);
        SetSkillInfo(curSkillId);
      } else {
        string chn_desc = StrDictionaryProvider.Instance.GetDictString(365);
        switch (result) {
          case DashFire.Network.GeneralOperationResult.LC_Failure_CostError:
            chn_desc = StrDictionaryProvider.Instance.GetDictString(369);
            break;
          case DashFire.Network.GeneralOperationResult.LC_Failure_LevelError:
            chn_desc = StrDictionaryProvider.Instance.GetDictString(370);
            break;
          case DashFire.Network.GeneralOperationResult.LC_Failure_Unknown:
            chn_desc = StrDictionaryProvider.Instance.GetDictString(365);
            break;
        }
        DashFire.LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", chn_desc, C_ConFirm, null, null, null, false);
      }
    }
    catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  //
  public void HandleUnLockSkill(int presetIndex, int skillId, int userLevel, DashFire.Network.GeneralOperationResult result)
  {
    try {
      if (DashFire.Network.GeneralOperationResult.LC_Succeed == result) {
        //string msg = string.Format("HandleUnlockSkill in UI presetIndex:{0}, skillId:{1}", presetIndex, skillId);
        //Debug.Log(msg);
        uiSkillStorage.OnUnlockSkill(skillId);
        SetSkillInfo(skillId);
        //可装备、可升级
        SetEquipButtonState(true);
        SetActionButtonState(true);
        SetEquipFlag(true);
        if(UISkillGuide.Instance.GetSteps()<2)
          GuideStepComplete(2);
      } else {
        string chn_desc = StrDictionaryProvider.Instance.GetDictString(366);
        DashFire.LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", chn_desc, C_ConFirm, null, null, null, false);
      }
    }
    catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  //处理升阶之后的信息
  public void HandleLiftSkill(int sourceId, int targetId, DashFire.Network.GeneralOperationResult result)
  {
    try {
      if (DashFire.Network.GeneralOperationResult.LC_Succeed == result) {
        //string str = string.Format("HandleLiftSkill,sourceId:{0},targetId:{1}", sourceId, targetId);
        //Debug.Log(str);
        SetSkillInfo(targetId);
        uiSkillStorage.OnLiftSkill(sourceId, targetId);
        uiSkillSetting.OnLiftSkill(sourceId, targetId);
      } else {
        string chn_desc = StrDictionaryProvider.Instance.GetDictString(367);
        DashFire.LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", chn_desc, C_ConFirm, null, null, null, false);
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  public void SetHeroLevel(int level)
  {
    if (null != uiSkillTitle) {
      uiSkillTitle.SetLevel(level);
    }
  }
  //设置金币数量
  public void SetMoneyCoin(int amount)
  {
    if (null != uiSkillTitle) {
      uiSkillTitle.SetMoneyCoin(amount);
    }
  }
  //设置钻石数量
  public void SetDiamond(int amount)
  {
    if (null != uiSkillTitle) {
      uiSkillTitle.SetDiamond(amount);
    }
  }
  public void SetSkillInfo(int skillId)
  {
    m_CurrentClickSkillId = skillId;
    if (uiSkillInfo == null) {
      Debug.LogError("!!Did not initialize uiSkillInfo");
      return;
    }
    if (uiSkillInfo != null) {
      uiSkillInfo.SetLiftSkillInfo(skillId);
      uiSkillInfo.SetSkillLevelUpInfo(skillId);
      uiSkillInfo.SetSkillPanelInfo(skillId);
    }
    if(uiSkillStorage!=null)
    uiSkillStorage.OnUpgradeSkill(skillId);
    //uiSkillInfo.SetText(LabelType.MsgCd, 2.ToString(), "3");
  }

  //根据玩家Id设置技能包
  public void InitSkills(List<SkillInfo> skillPresets)
  {
    try {
      if (uiSkillStorage == null) return;
      if (skillPresets.Count > 0)
        CurrentClickSkillId = skillPresets[0].SkillId;
      uiSkillStorage.InitSkillStorage(skillPresets);
      uiSkillSetting.InitSkillSetting(skillPresets);
    }
    catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  //设置技能段
  public void SkillPoint(int skillId)
  {
  }
  public void OnUnloadedSkill(int presetIndex, int slotPos, DashFire.Network.GeneralOperationResult result)
  {
    try {
      if (DashFire.Network.GeneralOperationResult.LC_Succeed == result) {
        int skillId = uiSkillSetting.GetSkillId(slotPos);
        uiSkillStorage.OnUnloadedSkill(presetIndex, skillId);
        uiSkillSetting.OnUnloadedSkill(slotPos);
      } else {
        DashFire.LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", result.ToString(), C_ConFirm, null, null, null, false);
      }
    }
    catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  public void OnUnloadedSkill(int skillId)
  {
    uiSkillStorage.OnUnloadedSkill(UISkillSetting.presetIndex, skillId);
  }
  //点击操作装备技能
  public void SendMsg(int targetId, SlotPosition targetPos)
  {
    if (uiSkillInfo == null) return;
    //uiSkillInfo.SkillId为SourceId,==-1则返回
    if (uiSkillInfo.SkillId == -1) return;
    int sourceId = uiSkillInfo.SkillId;
    UISkillSlot sourceSlot = uiSkillStorage.GetSlot(sourceId);
    int currentPreset = UISkillSetting.presetIndex;
    if (sourceSlot != null) {
      if (sourceSlot.EquipedPos != null && sourceSlot.EquipedPos.Presets[currentPreset] == SlotPosition.SP_None) {
        LogicSystem.PublishLogicEvent("ge_mount_skill", "lobby", currentPreset, sourceId, targetPos);
      } else {
        LogicSystem.PublishLogicEvent("ge_swap_skill", "lobby", currentPreset, sourceId, sourceSlot.EquipedPos.Presets[UISkillSetting.presetIndex], targetPos);
      }
    }
  }
  //用于点击装备技能
  public void ShowSlotHighlight(SlotType slotType)
  {
    if (uiSkillSetting != null && uiSkillStorage!=null) {
      if (slotType == SlotType.SkillSetting){
        uiSkillSetting.ShowSlotHighlight(CurrentClickSkillId);
        uiSkillStorage.ShowHighLight(int.MinValue);
      }
      else {
        uiSkillSetting.ShowSlotHighlight(int.MinValue);
        uiSkillStorage.ShowHighLight(CurrentClickSkillId);
      }
    }
  }
  //设置可装备
  public void SetEquipFlag(bool canEquip)
  {
    if (uiSkillSetting != null)
      uiSkillSetting.SetEquipFlag(canEquip);
  }

  //设置按钮状态为解锁或者升级
  public void SetActionButtonState(bool isUnlock)
  {
    string chn_levelUp = StrDictionaryProvider.Instance.GetDictString(404);
    string chn_unlock = StrDictionaryProvider.Instance.GetDictString(368);
    if (isUnlock) {
      //todo:如果已经解锁，还应该判断是否可升级
      uiSkillInfo.SetActionText(chn_levelUp);
    } else {
      uiSkillInfo.SetActionText(chn_unlock);
    }
  }
  //
  public void UpdateProperty(float money, float gold, int curStamina, int exp, int level)
  {
    //NGUIDebug.Log("UpdateProperty");
    try {
      SetMoneyCoin(Convert.ToInt32(money));
      SetDiamond(Convert.ToInt32(gold));
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  public void OnEquipPageButtonClick()
  {
    ShowInfoPanel(PanelType.SkillInfo);
    // ShowSlotHighlight(CurrentClickSkillId);
    if (uiSkillInfo != null) {
      uiSkillInfo.SetSkillPanelInfo(CurrentClickSkillId);

    }
  }
  public void OnLevelUpPageButtonClick()
  {
    ShowInfoPanel(PanelType.SkillLevelUpInfo);
    if (uiSkillInfo != null) {
      uiSkillInfo.SetSkillLevelUpInfo(CurrentClickSkillId);
    }
  }
  public void OnLiftUpPageButtonClick()
  {
    ShowInfoPanel(PanelType.SkillLiftUpInfo);
    if (uiSkillInfo != null) {
      uiSkillInfo.SetLiftSkillInfo(CurrentClickSkillId);
    }
  }

  //根据技能状态设置装备按钮状态
  public void SetEquipButtonState(bool enable)
  {
//     if(equipButton!=null)
//     equipButton.isEnabled = enable;
  }
  //当前点击技能ID
  public int CurrentClickSkillId
  {
    get
    {
      return m_CurrentClickSkillId;
    }
    set
    {
      m_CurrentClickSkillId = CurrentClickSkillId;
    }
  }
  //添加新手指引指示
  public void AddGuidePointing(SlotType slotType, int skillId)
  {
    string path = UIManager.Instance.GetPathByName("GuideHand");
    GameObject go = ResourceSystem.GetSharedResource(path) as GameObject;
    if (go == null) {
      Debug.Log("!!!Load " + path +" failed.");
      return;
    }
    if (slotType == SlotType.SkillStorage) {
      if (uiSkillStorage != null) {
        uiSkillStorage.AddGuidePointing(go, skillId);
      }
    } else {
      if (uiSkillSetting != null) {
        uiSkillSetting.AddGuidePointing(go, -1);
        //如果没有第二步，只存在第一步到第三步则需要去掉Storage中的指示
        if (uiSkillStorage != null) uiSkillStorage.DelGuidePointing(skillId);
      }
    }
  }
  public void GuidePointToUnlock(int skillId)
  {
    if (uiSkillStorage != null) uiSkillStorage.DelGuidePointing(skillId);
    string path = UIManager.Instance.GetPathByName("GuideHand");
    GameObject go = ResourceSystem.GetSharedResource(path) as GameObject;
    if (go == null) {
      Debug.Log("!!!Load " + path + " failed.");
      return;
    }
    DelGuidePointOnUnlock();
    Transform transUnlock = this.transform.Find("SkillInfo/02LevelUpInfo/bt");
    if (transUnlock != null) {
      NGUITools.AddChild(transUnlock.gameObject, go);
    }
  }
  public void DelGuidePointOnUnlock()
  {
    Transform transUnlock = this.transform.Find("SkillInfo/02LevelUpInfo/bt/GuideHand(Clone)");
    if (transUnlock != null) {
      NGUITools.Destroy(transUnlock.gameObject);
    }
  }
  public void GuideStepComplete(int stepIndex,int skillId=-1)
  {
    if (stepIndex == 2) {
      UISkillGuide.Instance.SetSteps(2);
      DelGuidePointOnUnlock();
      AddGuidePointing(SlotType.SkillSetting, -1);
    }
    if (stepIndex == 3) {
      UISkillGuide.Instance.SetSteps(3);
      if (uiSkillSetting != null)
        uiSkillSetting.DelGuidePointing(skillId);
      if (uiSkillStorage != null)
        uiSkillStorage.DelGuidePointing(UISkillGuide.Instance.m_CouldEquipSkillId);
    }
  }

}
