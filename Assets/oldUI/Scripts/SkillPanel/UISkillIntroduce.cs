using UnityEngine;
using System;
using System.Collections;
using DashFire;


public class UISkillIntroduce : MonoBehaviour {

  public UITextList textList = null;
  public UIScrollBar scrollBar = null;
  // Use this for initialization
  void Start()
  {
	}
  // Update is called once per frame
  void Update()
  {
	}
  public void SetSkillIntroduce(int skillId)
  {
    if (scrollBar == null || textList == null) return;
    ClearChildren();
    //m_SkillId = skillId;
    //通过读取表格获取技能信息，初始化预设
    int skillLevel = 0;
    foreach (SkillInfo info in LobbyClient.Instance.CurrentRole.SkillInfos) {
      if (info.SkillId == skillId){  
        skillLevel = info.SkillLevel; 
        break;
      }
    }
    SkillLogicData skillCfg = SkillConfigProvider.Instance.ExtractData(SkillConfigType.SCT_SKILL, skillId) as SkillLogicData;
    if (skillCfg != null) {
      textList.Add("                       [ffff00]" +skillCfg.ShowName+"[-]");
      textList.Add(" ");
      textList.Add(skillCfg.ShowDescription);
      if (skillCfg.ShowSteps > 1) {
        for(int index =2; index<=skillCfg.ShowSteps; ++index){
          switch(index){
            case 2:textList.Add("[00ff00]" + skillCfg.ShowSteps2Des + "[-]");break;
            case 3:textList.Add("[00ff00]" + skillCfg.ShowSteps3Des + "[-]");break;
            case 4:textList.Add("[00ff00]" + skillCfg.ShowSteps4Des + "[-]");break;
            default:break;
          }
        }
      }
      textList.Add(" ");
      textList.Add(FormatString("技能CD",skillCfg.ShowCd.ToString(), "s"));
      textList.Add(FormatString("技能消耗", skillCfg.ShowCostEnergy.ToString(), ""));
      textList.Add(FormatString("技能等级", skillLevel.ToString(), "级"));
      float totalDamage =(skillCfg.ShowBaseDamage + skillCfg.DamagePerLevel * skillLevel)*100;
      textList.Add(FormatString("技能总伤害", totalDamage.ToString("F1"), "%"));
      textList.Add(" ");
      //最大阶数为4
      const int MaxSteps = 4;
      for (int index = skillCfg.ShowSteps + 1; index <= MaxSteps; ++index) {
        switch (index) {
          case 2: textList.Add("[00ff00]" + skillCfg.ShowSteps2Des + "[-]"); break;
          case 3: textList.Add("[00ff00]" + skillCfg.ShowSteps3Des + "[-]"); break;
          case 4: textList.Add("[00ff00]" + skillCfg.ShowSteps4Des + "[-]"); break;
          default: break;
        }
      }
    }

    scrollBar.value = 0;
  }
  private void ClearChildren()
  {
    textList.Clear();
  }
  private string FormatString(string name, string value, string ending)
  {
    string ret = "";
    ret = name + ": [ffff00]" + value + "[-] " + ending;
    return ret;
  }
  public GameObject labelPrefab = null;
  //private int m_SkillId = -1;
}
