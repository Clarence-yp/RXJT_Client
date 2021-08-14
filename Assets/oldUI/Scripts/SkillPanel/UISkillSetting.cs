using DashFire;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UISkillSetting : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {
    if (settingButtonGo != null) {
      UIEventListener.Get(settingButtonGo).onClick = OnSettingButtonClick;
    }
  }

  // Update is called once per frame
  void Update()
  {

  }

  public void InitSkillSetting(List<SkillInfo> skillInfoList)
  {
    if (skillInfoList == null) return;
    int currentPreset = UISkillSetting.presetIndex;
    foreach (SkillInfo info in skillInfoList) {
      if (info != null && info.Postions.Presets[currentPreset] != SlotPosition.SP_None) {
        int index = (int)info.Postions.Presets[currentPreset];
        if (index > 0 && index <= 4) {
          UISkillSlot slot = skillStorageArr[index - 1];
          if (slot == null || slot.SkillId != -1) continue;
          slot.SkillId = info.SkillId;
          //没初始化Atlas则初始化
          if (!m_IsAtlasInitialized) {
            InitSlotAtlas(info.SkillId);
            m_IsAtlasInitialized = true;
          }
          SkillLogicData skillCfg = SkillConfigProvider.Instance.ExtractData(SkillConfigType.SCT_SKILL, info.SkillId) as SkillLogicData;
          if (null != skillCfg) {
            slot.SetName(skillCfg.ShowName);
            slot.SetIcon(skillCfg.ShowIconName);
         }
        }
      }
    }
  }
  private void InitSlotAtlas(int skillId)
  {
    //同一个英雄对应的Atlas相同，随机选取其中一个技能
    foreach (UISkillSlot slot in skillStorageArr) {
      if (slot != null)
        slot.SetIconAtlas(skillId);
    }
  }
  public void ExchangeSlot(UISkillSlot draged, UISkillSlot surface)
  {
    if (draged == null || surface == null)
      return;
    UISkillSlot skillSlot = null;
    foreach (UISkillSlot slot in skillStorageArr) {
      if (slot != null && slot.SkillId == draged.SkillId) {
        skillSlot = slot;
        break;
      }
    }
    if (null == skillSlot) return;
    //如果surface与draged为同一个Slot,则不交换
    if (surface.SkillId == skillSlot.SkillId) {
      string iconName = draged.icon.spriteName;
      skillSlot.SetIcon(iconName);
      return;
    }
    if (surface.slotType == SlotType.SkillSetting) {
      LogicSystem.PublishLogicEvent("ge_swap_skill", "lobby", UISkillSetting.presetIndex, skillSlot.SkillId,
        skillSlot.slotIndex, surface.slotIndex);
    }
  }

  public void ExchangeSlot(SlotPosition sourcePos, SlotPosition targetPos)
  {
    int sourceIndex = (int)sourcePos;
    int targetIndex = (int)targetPos;
    if (sourceIndex <= 0 || sourceIndex > 4 || targetIndex <= 0 || targetIndex > 4) {
      return;
    }
    UISkillSlot draged = skillStorageArr[sourceIndex - 1];
    UISkillSlot surface = skillStorageArr[targetIndex - 1];
    if (draged == null || surface == null) {
      return;
    }
    int surfaceSkillId = surface.SkillId;
    surface.SkillId = draged.SkillId;
    surface.SetSlotIconById(surface.SkillId);
    draged.SkillId = surfaceSkillId;
    draged.SetSlotIconById(surfaceSkillId);
  }
  //卸下技能
  public void UnloadSkill(UISkillSlot skill)
  {
    if (skill == null) return;
    foreach (UISkillSlot slot in skillStorageArr) {
      if (slot != null && slot.SkillId == skill.SkillId) {
        LogicSystem.PublishLogicEvent("ge_unmount_skill", "lobby", presetIndex, slot.slotIndex);
      }
    }
  }
  public void OnLoadedSkill(int presetIndex, int skillId, int slotPositon)
  {
    if (slotPositon <= 0 || slotPositon > c_SkillSlotNum)
      return;
    UISkillSlot skillSlot = skillStorageArr[slotPositon - 1];
    if (skillSlot != null && skillSlot.SkillId == -1) {
      skillSlot.SkillId = skillId;
      skillSlot.SetSlotIconById(skillId);

    } else if (skillSlot != null && skillSlot.SkillId != -1) {
      UISkillPanel skillPanel = NGUITools.FindInParents<UISkillPanel>(this.gameObject);
      if (null != skillPanel) {
        skillPanel.OnUnloadedSkill(skillSlot.SkillId);
        skillSlot.SkillId = skillId;
        skillSlot.SetSlotIconById(skillId);
      }
    }

  }
  public void OnUnloadedSkill(int slotIndex)
  {
    if (slotIndex <= 0 || slotIndex > c_SkillSlotNum)
      return;
    UISkillSlot skillSlot = skillStorageArr[slotIndex - 1];
    if (skillSlot != null) {
      skillSlot.SkillId = -1;
      skillSlot.SetName("可装备");
      skillSlot.SetIcon("");
    }
  }
  //技能升阶
  public void OnLiftSkill(int sourceSkillId, int curSkillId)
  {
    foreach (UISkillSlot slot in skillStorageArr) {
      if (slot != null && slot.SkillId == sourceSkillId) {
        slot.UpgradeSkill(curSkillId);
        break;
      }
    }
  }
  public void ResetSkillSlot(UISkillSlot slot)
  {
    //根据已经装备的技能信息设置技能图标、技能名等属性
    slot.m_EquipedPos.Presets[UISkillSetting.presetIndex] = DashFire.SlotPosition.SP_None;
    slot.SkillId = -1;
    UISprite sp = slot.GetComponent<UISprite>();
    if (sp != null) {
      sp.spriteName = "";
    }
  }

  public int GetSkillId(int slotPos)
  {
    int index = slotPos;
    if (index > 4 || index < 0) return -1;
    if (skillStorageArr[index - 1] != null)
      return skillStorageArr[index - 1].SkillId;
    return -1;
  }

  public void OnSettingButtonClick(GameObject go)
  {
    //切换技能预设
  }
  public void HideSlotHighlight()
  {
    foreach (UISkillSlot slot in skillStorageArr) {
      if (null != slot) {
        slot.SetHighlight(false);
      }
    }
  }
  //设置可装备标志
  public void SetEquipFlag(bool canEquip)
  {
    foreach (UISkillSlot slot in skillStorageArr) {
      if (null != slot) {
        slot.SetEquipFlag(canEquip);
      }
    }
  }

  public void ShowSlotHighlight(int excludeId)
  {
    foreach (UISkillSlot slot in skillStorageArr) {
      if (null != slot && slot.SkillId == excludeId &&slot.SkillId!=-1) {
        slot.SetHighlight(true);
      } else {
        slot.SetHighlight(false);
      }
    }
  }
  //
  public void AddGuidePointing(GameObject goPointing, int skillId)
  {
    DelGuidePointing(skillId);
    UISkillSlot skillSlot = GetSlotById(skillId);
    if (skillSlot == null) {
      skillSlot =skillStorageArr[0];
    }
    if (skillSlot != null && goPointing != null) {
      goPointing = NGUITools.AddChild(skillSlot.gameObject, goPointing);
      goPointing.transform.position = skillSlot.transform.position;
    }
  }
  public void DelGuidePointing(int skillId)
  {
    UISkillSlot skillSlot = GetSlotById(skillId);
    if (skillSlot == null) {
      skillSlot = skillStorageArr[0];
    }
    if (skillSlot != null) {
      Transform tsPointing = skillSlot.transform.Find("GuideHand(Clone)");
      if (tsPointing != null) {
        NGUITools.Destroy(tsPointing.gameObject);
      } else {
        //已防玩家没有点击目标位置，导致小手不消失
        for(int i=0;i<skillStorageArr.Length;++i){
          if (skillStorageArr[i] != null) {
            Transform ts = skillStorageArr[i].transform.Find("GuideHand(Clone)");
            if (ts != null) NGUITools.Destroy(ts.gameObject);
          }
        }
      }
    }
  }
  //
  public UISkillSlot GetSlotById(int skillId)
  {
    for (int i = 0; i < skillStorageArr.Length; ++i) {
      if (skillStorageArr[i] != null && skillStorageArr[i].SkillId == skillId)
        return skillStorageArr[i];
    }
    return null;
  }

  //当前预设的序号
  public static int presetIndex = 0;
  public GameObject settingButtonGo = null;
  private const int c_SkillSlotNum = 4;
  public UISkillSlot[] skillStorageArr = new UISkillSlot[c_SkillSlotNum];
  private bool m_IsAtlasInitialized = false;
}
