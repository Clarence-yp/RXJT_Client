using UnityEngine;
using DashFire;
using System;
using System.Collections;
using System.Collections.Generic;

public class UISkillGuide{

  public delegate void ShowGuideDelegate(SlotType slotType, int skillId);
  public delegate void GuidePointToUnlockDelegate(int skillId);
  public ShowGuideDelegate ShowGuideInSlotHandler = null;
  public GuidePointToUnlockDelegate HandleGuidePointToUnlock = null;
  private const int c_Steps = 3;
  private int steps = 0;
  public int m_UnlockSkillId = -1;
  public int m_CouldEquipSkillId = -1;
  public void SetSteps(int value)
  {
    PlayerPrefs.SetInt("Steps", value);
  }
  public int GetSteps()
  {
    return PlayerPrefs.GetInt("Steps");
  }
  public void ShowGuidePointing()
  {
    int step = GetSteps();
    if (step == 3) return;
    int skillId = -1;
    if (step == 1 || step ==0) {
      if (ExistCouldUnlockSkill(ref skillId)) {
        if (ShowGuideInSlotHandler != null) {
          ShowGuideInSlotHandler(SlotType.SkillStorage, skillId);
        }
      }
    }
    if (step == 2) {
      if (ExistUnlockSkill(ref skillId)) {
        if (ShowGuideInSlotHandler != null) {
          ShowGuideInSlotHandler(SlotType.SkillStorage, skillId);
        }
      }
    }
  }
  //判断是否有可解锁节能
  private bool ExistCouldUnlockSkill(ref int skillId)
  {
    skillId = -1;
    RoleInfo role_info = LobbyClient.Instance.CurrentRole;
    if (role_info != null && role_info.SkillInfos != null) {
      SkillInfo skill_info = null;
      int index = 0;
      for (index = 0; index < role_info.SkillInfos.Count; ++index) {
        skill_info = role_info.SkillInfos[index];
        if (skill_info != null && skill_info.SkillLevel <= 0) {
          if (skill_info.ConfigData != null && skill_info.ConfigData.ActivateLevel <= role_info.Level) {
            //有可解锁技能
            skillId= skill_info.SkillId;
            m_UnlockSkillId = skillId;
            return true;
          }
        }
      }
      if (index >= role_info.SkillInfos.Count)
        return false;//没有可解锁技能
    }
    return false;
  }
  //判断是否有可装备且未装备技能
  private bool ExistUnlockSkill(ref int skillId)
  {
    skillId = -1;
    RoleInfo role_info = LobbyClient.Instance.CurrentRole;
    if (role_info != null && role_info.SkillInfos != null) {
      SkillInfo skill_info = null;
      int index = 0;
      for (index = 0; index < role_info.SkillInfos.Count; ++index) {
        skill_info = role_info.SkillInfos[index];
        if (skill_info != null && skill_info.SkillLevel > 0 && skill_info.Postions.Presets[0] ==SlotPosition.SP_None) {
            skillId = skill_info.SkillId;
            m_CouldEquipSkillId = skillId;
            return true;
        }
      }
      if (index >= role_info.SkillInfos.Count)
        return false;//没有可解锁技能
    }
    return false;
  }
  //判断是否可解锁
  private bool CouldUnlockSkill(int skillId)
  {
    RoleInfo role_info = LobbyClient.Instance.CurrentRole;
    if (role_info != null && role_info.SkillInfos != null) {
      SkillInfo skill_info = null;
      int index = 0;
      for (index = 0; index < role_info.SkillInfos.Count; ++index) {
        skill_info = role_info.SkillInfos[index];
        if (skill_info != null && skill_info.SkillId == skillId && skill_info.SkillLevel<=0) {
          if (skill_info.ConfigData != null && skill_info.ConfigData.ActivateLevel <= role_info.Level) {
            //可解锁技能
            return true;
          }
        }
      }
      if (index >= role_info.SkillInfos.Count)
        return false;//不可解锁技能
    }
    return false;
  }
  //判断技能是否已经解锁且未装备
  private bool IsUnlockSkillAndUnEquiped(int skillId)
  {
    RoleInfo role_info = LobbyClient.Instance.CurrentRole;
    if (role_info != null && role_info.SkillInfos != null) {
      SkillInfo skill_info = null;
      int index = 0;
      for (index = 0; index < role_info.SkillInfos.Count; ++index) {
        skill_info = role_info.SkillInfos[index];
        if (skill_info != null && skill_info.SkillId == skillId && skill_info.SkillLevel > 0) {
            //可解锁技能
          if (skill_info.Postions.Presets[0] == SlotPosition.SP_None)
            return true;
          }
        }
      }
    return false;//不可解锁技能
  }
  public void OnUnlockSlotClick(int skillId)
  {
    
      if ((GetSteps() == 1|| GetSteps() == 0) &&CouldUnlockSkill(skillId)) {
        if (HandleGuidePointToUnlock != null)
          HandleGuidePointToUnlock(skillId);
      }
      if (GetSteps() == 2 && IsUnlockSkillAndUnEquiped(skillId)) {
        if (ShowGuideInSlotHandler != null)
          ShowGuideInSlotHandler(SlotType.SkillSetting, -1);
      }
  }
  private static UISkillGuide m_Instance = new UISkillGuide();
  public static UISkillGuide Instance
  {
    get
    {
      return m_Instance;
    }
  }
}
