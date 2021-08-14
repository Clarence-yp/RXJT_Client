using UnityEngine;
using System.Collections;
using DashFire;
public enum SlotType
{
    SkillSetting,
    SkillStorage,
}
public class UISkillSlot : MonoBehaviour
{

    public UILabel lblName = null;
    public UILabel lblSection = null;
    public UILabel lblUnLock = null;
    public UISprite icon = null;
    public UISprite circleSp = null;
    //锁标志
    public UISprite lockSp = null;
    public UISprite[] sectionHintSpArr = new UISprite[4];//阶数标志
    public PresetInfo m_EquipedPos = null;
    public SlotType slotType = SlotType.SkillStorage;
    public SlotPosition slotIndex = SlotPosition.SP_None;

    public bool m_IsUnlock = false;
    //
    private int m_SkillId = -1;
    private bool m_IsHighlight = false;
    private const int c_SectionNum = 4;
    private const string ASHHINT = "skilllevel";
    private const string LIGHTHINT = "skilllevel2";
    //private SkillInfo m_SkillInfo = null;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnClick()
    {
        UISkillPanel skillPanel = NGUITools.FindInParents<UISkillPanel>(this.gameObject);
        if (skillPanel == null) return;
        if (slotType == SlotType.SkillSetting)
        {
            if (m_IsHighlight)
            {
                skillPanel.SendMsg(SkillId, slotIndex);
            }
            skillPanel.SetActionButtonState(true);
            skillPanel.SetEquipFlag(false);
        }
        else
        {
            skillPanel.SetActionButtonState(m_IsUnlock);
            skillPanel.SetEquipFlag(m_IsUnlock);
            UISkillGuide.Instance.OnUnlockSlotClick(SkillId);
        }
        if (circleSp != null) NGUITools.SetActive(circleSp.gameObject, true);
        skillPanel.SetSkillInfo(SkillId);
        skillPanel.ShowSlotHighlight(slotType);
    }
    public void SetIconAtlas(int skillId)
    {
        SkillLogicData skillCfg = SkillConfigProvider.Instance.ExtractData(SkillConfigType.SCT_SKILL, skillId) as SkillLogicData;
        //m_SkillInfo = info;
        if (null != skillCfg)
        {
            GameObject goAtlas = ResourceSystem.GetSharedResource(skillCfg.ShowAtlasPath) as GameObject;
            if (goAtlas != null)
            {
                UIAtlas atlas = goAtlas.GetComponent<UIAtlas>();
                if (atlas != null && icon != null)
                {
                    icon.atlas = atlas;
                }
            }
            else
            {
                Debug.LogError("!!!Load atlas failed.");
            }
        }
    }
    public void SetIcon(string name)
    {
        if (name == null) return;
        if (icon != null)
        {
            icon.spriteName = name;
            UIButton button = this.GetComponent<UIButton>();
            if (button != null) button.normalSprite = name;
        }
        else
        {
            Debug.LogError("!! Icon did not init.");
        }
    }

    //设置等级
    public void SetSkillLevel(int level)
    {
        //在没解锁时会隐藏掉、所以这里要重新设置为可见
        ShowSection(true);
        if (lblSection != null)
        {
            lblSection.text = "Lv." + level.ToString();
        }
        for (int index = 0; index < c_SectionNum; ++index)
        {
            if (index < level && index < sectionHintSpArr.Length && sectionHintSpArr[index] != null)
            {
                sectionHintSpArr[index].spriteName = LIGHTHINT;
            }
            else
            {
                if (index < sectionHintSpArr.Length && sectionHintSpArr[index] != null)
                    sectionHintSpArr[index].spriteName = ASHHINT;
            }
        }
    }
    //设置技能名
    public void SetName(string name)
    {
        if (lblName != null)
        {
            lblName.text = name;
        }
        else
        {
            //Debug.Log("!!NameLabel did not init.");
        }
    }

    public void Unlock(bool unLock)
    {
        m_IsUnlock = unLock;
        if (lockSp == null)
        {
            Debug.LogError("!!Did not init sprite:lockSprite.");
            return;
        }
        if (unLock)
        {
            if (lblSection != null) NGUITools.SetActive(lblSection.gameObject, true);
            NGUITools.SetActive(lockSp.gameObject, false);
            if (lblSection != null)
            {
                //SkillLogicData skillCfg = SkillConfigProvider.Instance.ExtractData(SkillConfigType.SCT_SKILL, SkillId) as SkillLogicData;
                SkillInfo skill_info = GetSkillInfoById(SkillId);
                SetSkillLevel(skill_info.SkillLevel);
            }
            //lockSp.spriteName = "UnLock";
        }
        else
        {
            NGUITools.SetActive(lockSp.gameObject, true);
            if (lblUnLock != null)
            {
                string CHN = StrDictionaryProvider.Instance.GetDictString(360);
                SkillLogicData skillCfg = SkillConfigProvider.Instance.ExtractData(SkillConfigType.SCT_SKILL, SkillId) as SkillLogicData;
                if (skillCfg != null) lblUnLock.text = skillCfg.ActivateLevel + CHN;
            }
            if (lblSection != null) NGUITools.SetActive(lblSection.gameObject, false);
            ShowSection(false);
        }
    }
    public void ShowSection(bool visible)
    {
        for (int index = 0; index < sectionHintSpArr.Length; ++index)
            if (sectionHintSpArr[index] != null)
                NGUITools.SetActive(sectionHintSpArr[index].gameObject, visible);
    }
    //设置可装备标志
    public void SetEquipFlag(bool equip)
    {
        //
        m_IsHighlight = equip;
    }

    //设置高亮
    public void SetHighlight(bool visible)
    {
        if (circleSp != null)
        {
            NGUITools.SetActive(circleSp.gameObject, visible);
        }
    }
    public void SetSlotIconById(int skillId)
    {
        SkillLogicData skillCfg = SkillConfigProvider.Instance.ExtractData(SkillConfigType.SCT_SKILL, skillId) as SkillLogicData;
        if (null != skillCfg)
        {
            SetIcon(skillCfg.ShowIconName);
            SetName(skillCfg.ShowName);
            SkillInfo skill_info = GetSkillInfoById(skillId);
            SetSkillLevel(skill_info.SkillLevel);
        }
        else
        {
            SetIcon("");
            SetName("可装备");
        }
    }
    public void InitSlot(SkillInfo info)
    {
        SkillLogicData skillCfg = SkillConfigProvider.Instance.ExtractData(SkillConfigType.SCT_SKILL, info.SkillId) as SkillLogicData;
        //m_SkillInfo = info;
        if (null != skillCfg)
        {
            SetIconAtlas(info.SkillId);
            SetIcon(skillCfg.ShowIconName);
            SetName(skillCfg.ShowName);
            SetSkillLevel(info.SkillLevel);
        }
        SkillId = info.SkillId;
        EquipedPos = info.Postions;
        Unlock(info.SkillLevel > 0);

    }
    public void UpgradeSkill(int skillId)
    {
        SkillLogicData skillCfg = SkillConfigProvider.Instance.ExtractData(SkillConfigType.SCT_SKILL, skillId) as SkillLogicData;
        //m_SkillInfo = info;
        if (null != skillCfg)
        {
            SetIcon(skillCfg.ShowIconName);
            SetName(skillCfg.ShowName);
            SkillInfo skill_info = GetSkillInfoById(skillId);
            SetSkillLevel(skill_info.SkillLevel);
        }
        SkillId = skillId;
    }

    public int SkillId
    {
        get
        {
            return m_SkillId;
        }
        set
        {
            m_SkillId = value;
        }
    }

    public PresetInfo EquipedPos
    {
        get
        {
            return m_EquipedPos;
        }
        set
        {
            m_EquipedPos = value;
        }
    }
    private SkillInfo GetSkillInfoById(int skillId)
    {
        RoleInfo role_info = LobbyClient.Instance.CurrentRole;
        if (role_info != null)
        {
            for (int i = 0; i < role_info.SkillInfos.Count; ++i)
            {
                if (role_info.SkillInfos[i] != null && role_info.SkillInfos[i].SkillId == skillId)
                {
                    return role_info.SkillInfos[i];
                }
            }
        }
        return null;
    }
}
