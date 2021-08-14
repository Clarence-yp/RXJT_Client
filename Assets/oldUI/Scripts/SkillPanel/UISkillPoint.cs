/*UI类
 * 技能点相关UI方法
 */ 
using UnityEngine;
using System.Collections;

public class UISkillPoint : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
  //根据技能Id获取不同的二段、三段、Q、E技能段
  public void SetSkillPoint(int skillId)
  {

  }

  private const int c_SkillPointNum = 5;
  public UISkillSlot[] SkillPoint = new UISkillSlot[c_SkillPointNum];
}
