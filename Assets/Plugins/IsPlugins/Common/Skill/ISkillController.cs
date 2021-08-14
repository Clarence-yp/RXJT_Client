using System;
using System.Collections;
using System.Collections.Generic;
using ScriptRuntime;

namespace DashFire
{

  public enum SkillCannotCastType
  {
    kUnknow,
    kNotFindSkill,
    kOwnerDead,
    kCannotCtrol,
    kInCD,
    kCostNotEnough,
  }

  public class SkillInputData
  {
    public float castRange;
    public float targetChooseRange;
  }

  public class SkillNode
  {
    public int SkillId;
    public SkillCategory Category;
    public SkillNode SkillQ = null;
    public SkillNode SkillE = null;
    public SkillNode NextSkillNode = null;
    public float StartTime;
    public Vector3 TargetPos;
    public bool IsLocked = false;
    public bool IsCDChecked = false;
  }

  public delegate void SkillQECanInputHandler(float remaintime, List<SkillNode> skills);
  public delegate void SkillStartHandler();
  public interface ISkillController
  {
    void Init();
    void OnTick();
    void LearnSkill(int skillid);
    SkillNode GetHead(SkillCategory category);
    SkillNode InitCategorySkillNode(List<SkillLogicData> skills, SkillLogicData ss);
    SkillNode ChangeCategorySkill(SkillCategory category, SkillNode new_head);
    void AddBreakSkillTask();
    void CancelBreakSkillTask();
    void StartAttack(Vector3 targetpos);
    void StopAttack();
    void PushSkill(SkillCategory category, Vector3 targetpos);
    List<SkillNode> QuerySkillQE(SkillCategory category, int times);
    void AddBreakSection(int skillid, int breaktype, int starttime, int endtime, bool isinterrupt);
    bool ForceStartSkill(int skillid);
    bool BreakCurSkill(int breaktype);
    void ForceInterruptCurSkill();
  }
}