using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DashFire;
public class UISkillStorage : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ResetSlot(UISkillSlot draged)
    {
        foreach (UISkillSlot slot in skillStorageArr)
        {
            if (slot != null && slot.SkillId == draged.SkillId)
            {
                if (slot.icon != null && draged.icon != null) { slot.SetIcon(draged.icon.spriteName); }
                break;
            }
        }
    }
    public void ExchangeSlot(UISkillSlot draged, UISkillSlot surface)
    {
        //NGUIDebug.Log(surface.name);
        if (draged == null || surface == null)
        {
            return;
        }
        UISkillSlot skillSlot = null;
        foreach (UISkillSlot slot in skillStorageArr)
        {
            if (slot != null && slot.SkillId == draged.SkillId)
            {
                skillSlot = slot;
                break;
            }
        }
        if (null == skillSlot) return;
        //被拖拽的技能之前没有被装备上
        if (skillSlot.EquipedPos != null && skillSlot.EquipedPos.Presets[UISkillSetting.presetIndex] == SlotPosition.SP_None)
        {
            //todo:发送消息通知装载操作
            LogicSystem.PublishLogicEvent("ge_mount_skill", "lobby", UISkillSetting.presetIndex, skillSlot.SkillId,
             surface.slotIndex);
        }
        else
        {
            LogicSystem.PublishLogicEvent("ge_swap_skill", "lobby", UISkillSetting.presetIndex, skillSlot.SkillId,
              (SlotPosition)skillSlot.EquipedPos.Presets[UISkillSetting.presetIndex], surface.slotIndex);
        }
    }
    //
    public void OnExchangeSkill(int sourceSkillId, int targetSkillId, SlotPosition sourcePos, SlotPosition targetPos)
    {
        int presetIndex = UISkillSetting.presetIndex;
        foreach (UISkillSlot slot in skillStorageArr)
        {
            if (slot != null && slot.SkillId == -1) continue;
            if (slot != null && slot.SkillId == sourceSkillId && slot.EquipedPos != null)
            {
                slot.EquipedPos.Presets[presetIndex] = targetPos;
            }
            if (slot != null && slot.SkillId == targetSkillId && slot.EquipedPos != null)
            {
                slot.EquipedPos.Presets[presetIndex] = sourcePos;
            }
        }
    }

    //
    public void OnLoadedSkill(int presetIndex, int skillId, int slotPositon)
    {
        //
        foreach (UISkillSlot slot in skillStorageArr)
        {
            if (slot != null && slot.SkillId == skillId && slot.EquipedPos != null)
            {
                slot.EquipedPos.Presets[presetIndex] = (SlotPosition)slotPositon;
            }
        }
    }

    //
    public void OnUnloadedSkill(int presetIndex, int skillId)
    {
        foreach (UISkillSlot slot in skillStorageArr)
        {
            if (slot != null && slot.SkillId == skillId)
            {
                slot.m_EquipedPos.Presets[UISkillSetting.presetIndex] = SlotPosition.SP_None;
                //设置技能图标状态：装备、可装备等
            }
        }
    }
    //


    //初始化每个英雄技能背包里的技能
    public void InitSkillStorage(List<SkillInfo> skillPresets)
    {
        if (skillPresets == null || skillPresets.Count <= 0)
            return;
        //初始化滑动
        m_TransLeft = 0;
        m_TransRight = skillPresets.Count - c_ShowNum;
        if (btnLeft != null) NGUITools.SetActive(btnLeft.gameObject, false);
        if (m_TransRight <= 0)
        {
            m_TransRight = 0;
            if (btnRight != null) NGUITools.SetActive(btnRight.gameObject, false);
        }

        List<SkillInfo> skillList = skillPresets;
        for (int index = 0; index < skillList.Count; ++index)
        {
            SkillInfo info = skillList[index];
            if (info != null && goGrid != null && goSlot != null)
            {
                GameObject goChild = NGUITools.AddChild(goGrid, goSlot);
                if (goChild != null)
                {
                    UISkillSlot slotScript = goChild.GetComponent<UISkillSlot>();
                    if (slotScript != null)
                    {
                        slotScript.InitSlot(info);
                        if (index < c_SkillSlotNum) skillStorageArr[index] = slotScript;
                    }
                }
            }
        }
        if (goGrid != null)
        {
            UIGrid grid = goGrid.GetComponent<UIGrid>();
            if (grid != null) grid.Reposition();
        }
    }
    //通过Id获取Slot
    public UISkillSlot GetSlot(int skillId)
    {
        foreach (UISkillSlot slot in skillStorageArr)
        {
            if (null != slot && slot.SkillId == skillId)
                return slot;
        }
        return null;
    }
    //解锁技能
    public void OnUnlockSkill(int skillId)
    {
        UISkillSlot slot = GetSlot(skillId);
        if (slot != null)
        {
            slot.Unlock(true);
        }
    }
    //升级技能
    public void OnUpgradeSkill(int curSkillId)
    {
        UISkillSlot slot = GetSlot(curSkillId);
        if (slot != null)
        {
            slot.UpgradeSkill(curSkillId);
        }
    }
    //升阶技能
    public void OnLiftSkill(int sourceSkillId, int curSkillId)
    {
        UISkillSlot slot = GetSlot(sourceSkillId);
        if (slot != null)
        {
            slot.UpgradeSkill(curSkillId);
        }
    }
    //技能图标左右移动效果
    public void OnLeftButtonClick()
    {
        if (goGrid != null)
        {
            StartCoroutine(TransSkillStorage(m_TransOffsetX));
            //goGrid.transform.localPosition -= TransOffset;
        }
    }
    public void OnRightButtonClick()
    {
        if (goGrid != null)
        {
            StartCoroutine(TransSkillStorage(-m_TransOffsetX));
            //goGrid.transform.localPosition += TransOffset;
        }
    }
    //模拟UI滑动效果
    public IEnumerator TransSkillStorage(int offsetX)
    {
        m_ClickNum++;
        m_TotleOffset += offsetX;
        int transNum = 0;
        if (offsetX < 0)
        {
            transNum = m_TransRight >= 3 ? 3 : m_TransRight;
            m_TransRight -= transNum;
            m_TransLeft += transNum;
        }
        else
        {
            transNum = m_TransLeft >= 3 ? 3 : m_TransLeft;
            m_TransLeft -= transNum;
            m_TransRight += transNum;
        }
        if (btnLeft != null) btnLeft.enabled = false;
        if (btnRight != null) btnRight.enabled = false;
        while (Mathf.Abs(m_TotleOffset) <= transNum * TransOffset * m_ClickNum)
        {
            if (goGrid != null)
            {
                goGrid.transform.localPosition += new Vector3(offsetX, 0, 0);
            }
            yield return new WaitForSeconds(m_TransDelta);
            m_TotleOffset += offsetX;
        }
        if (btnLeft != null) btnLeft.enabled = true;
        if (btnRight != null) btnRight.enabled = true;
        m_ClickNum = 0;
        m_TotleOffset = 0f;
        if (m_TransLeft == 0)
        {
            if (btnLeft != null) NGUITools.SetActive(btnLeft.gameObject, false);
        }
        else
        {
            if (btnLeft != null) NGUITools.SetActive(btnLeft.gameObject, true);
        }
        if (m_TransRight == 0)
        {
            if (btnRight != null) NGUITools.SetActive(btnRight.gameObject, false);
        }
        else
        {
            if (btnRight != null) NGUITools.SetActive(btnRight.gameObject, true);
        }

    }
    //点击某个技能图标，高亮
    public void ShowHighLight(int skillId)
    {
        for (int i = 0; i < skillStorageArr.Length; ++i)
        {
            if (skillStorageArr[i] != null)
            {
                if (skillStorageArr[i].SkillId == skillId)
                {
                    skillStorageArr[i].SetHighlight(true);
                }
                else
                {
                    skillStorageArr[i].SetHighlight(false);
                }
            }
        }
    }

    public void AddGuidePointing(GameObject goPointing, int skillId)
    {
        DelGuidePointing(skillId);
        UISkillSlot skillSlot = GetSlot(skillId);
        if (skillSlot != null && goPointing != null)
        {
            goPointing = NGUITools.AddChild(skillSlot.gameObject, goPointing);
            goPointing.transform.position = skillSlot.transform.position;
        }
    }
    public void DelGuidePointing(int skillId)
    {
        UISkillSlot skillSlot = GetSlot(skillId);
        if (skillSlot != null)
        {
            Transform tsPointing = skillSlot.transform.Find("GuideHand(Clone)");
            if (tsPointing != null)
            {
                Destroy(tsPointing.gameObject);
            }
            else
            {
                //防止玩家没按要求操作
                for (int i = 0; i < skillStorageArr.Length; ++i)
                {
                    if (skillStorageArr[i] != null)
                    {
                        tsPointing = skillStorageArr[i].transform.Find("GuideHand(Clone)");
                        if (tsPointing != null)
                        {
                            Destroy(tsPointing.gameObject);
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            //防止玩家没按要求操作
            Transform tsPointing = null;
            for (int i = 0; i < skillStorageArr.Length; ++i)
            {
                if (skillStorageArr[i] != null)
                {
                    tsPointing = skillStorageArr[i].transform.Find("GuideHand(Clone)");
                    if (tsPointing != null)
                    {
                        Destroy(tsPointing.gameObject);
                        break;
                    }
                }
            }
        }
    }
    private const int c_SkillSlotNum = 15;
    public GameObject goSlot = null;
    public GameObject goGrid = null;
    public UIButton btnLeft = null;
    public UIButton btnRight = null;
    public UISkillSlot[] skillStorageArr = new UISkillSlot[c_SkillSlotNum];
    //滑动数据
    private const int c_ShowNum = 7;
    private int m_TransLeft = 0;
    private int m_TransRight = 3;
    private int m_ClickNum = 0;
    private float m_TotleOffset = 0f;
    public float m_TransDelta = 0.001f;
    public float TransOffset = 100f;
    public int m_TransOffsetX = 15;
}
