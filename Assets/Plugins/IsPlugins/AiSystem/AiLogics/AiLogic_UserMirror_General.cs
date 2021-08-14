using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptRuntime;

namespace DashFire {
  class AiLogic_UserMirror_General : AbstractUserStateLogic {

    protected override void OnInitStateHandlers()
    {
      SetStateHandler((int)AiStateId.Idle, this.IdleHandler);
      SetStateHandler((int)AiStateId.Pursuit, this.PursuitHandler);
      SetStateHandler((int)AiStateId.MoveCommand, this.MoveCommandHandler);
      SetStateHandler((int)AiStateId.PursuitCommand, this.PursuitCommandHandler);
      SetStateHandler((int)AiStateId.PatrolCommand, this.PatrolCommandHandler);
    }

    protected override void OnStateLogicInit(UserInfo user, AiCommandDispatcher aiCmdDispatcher, long deltaTime) {
      UserAiStateInfo info = user.GetAiStateInfo();
      info.HomePos = user.GetMovementStateInfo().GetPosition3D();
      info.Time = 0;
      user.GetMovementStateInfo().IsMoving = false;
    }
    private void IdleHandler(UserInfo user, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      AiLogicUtility.CommonIdleHandler(user, aiCmdDispatcher, deltaTime, this);
    }

    private void PursuitHandler(UserInfo user, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      if (user.IsDead()) {
        user.GetMovementStateInfo().IsMoving = false;
        return;
      }
      if (user.GetSkillStateInfo().IsImpactControl()) {
        user.GetMovementStateInfo().IsMoving = false;
        return;
      }
      if (user.GetSkillStateInfo().IsSkillActivated()) {
        user.GetMovementStateInfo().IsMoving = false;
        return;
      }
      UserAiStateInfo info = user.GetAiStateInfo();
      AiData_UserSelf_General data = GetAiData(user);
      if (null != data) {
        CharacterInfo target = AiLogicUtility.GetLivingCharacterInfoHelper(user, info.Target);
        if (null != target) {
          float dist = (float)user.GetActualProperty().AttackRange;
          float distGoHome = (float)user.GohomeRange;
          Vector3 targetPos = target.GetMovementStateInfo().GetPosition3D();
          ScriptRuntime.Vector3 srcPos = user.GetMovementStateInfo().GetPosition3D();
          float powDist = Geometry.DistanceSquare(srcPos, targetPos);
          float powDistToHome = Geometry.DistanceSquare(srcPos, info.HomePos);
          TryCastSkill(user, target, powDist);
          // 大于攻击距离 跑向目标
          if (powDist > m_AttackRange * m_AttackRange) {
            info.Time += deltaTime;
            if (info.Time > m_IntervalTime) {
              info.Time = 0;
              AiLogicUtility.PathToTargetWithoutObstacle(user, data.FoundPath, targetPos, m_IntervalTime, true, this);
            }
          } else {
            user.GetMovementStateInfo().IsMoving = false;
            NotifyUserMove(user);
          }
        }
      }
    }


    private bool TryCastSkill(UserInfo user, CharacterInfo target, float powDis)
    {
      float dis = (float)Math.Sqrt(powDis);
      int category = GetCanCastSkillId(user, dis);
      if (-1 != category) {
        if (user.GetSkillStateInfo().IsSkillActivated()) {
          SkillInfo curSkill = user.GetSkillStateInfo().GetCurSkillInfo();
          if (null != curSkill) {
            if (TimeUtility.GetServerMilliseconds() - curSkill.StartTime > curSkill.ConfigData.LockInputTime) {
              user.SkillController.PushSkill((SkillCategory)category, target.GetMovementStateInfo().GetPosition3D());
            }
          }
        } else {
          user.SkillController.PushSkill((SkillCategory)category, target.GetMovementStateInfo().GetPosition3D());
        }
        return true;
      }
      return false;
    }
    private int GetCanCastSkillId(UserInfo user, float dis) {
      if (null == user) return -1;
      List<int> skillsCanCast = new List<int>();
      int start = (int)SkillCategory.kAttack;
      int end = (int)SkillCategory.kSkillD;
      for (int i = start; i <= end; i++) {
        SkillNode node = user.SkillController.GetHead((SkillCategory)i);
        if (null != node) {
          SkillInfo skillInfo = user.GetSkillStateInfo().GetSkillInfoById(node.SkillId);
          if (null != skillInfo) {
            if (skillInfo.ConfigData.SkillRangeMax >= dis && skillInfo.ConfigData.SkillRangeMin <= dis) {
              if (!skillInfo.IsInCd(TimeUtility.GetServerMilliseconds())) {
                skillsCanCast.Add(i);
              }
            }
          }
        }
      }
      int count = skillsCanCast.Count;
      if (count > 0) {
        return skillsCanCast[Helper.Random.Next(count)];
      }
      return -1;
    }
    private void MoveCommandHandler(UserInfo user, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      AiLogicUtility.DoMoveCommandState(user, aiCmdDispatcher, deltaTime, this);
    }
    private void PursuitCommandHandler(UserInfo user, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      AiLogicUtility.DoPursuitCommandState(user, aiCmdDispatcher, deltaTime, this);
    }
    private void PatrolCommandHandler(UserInfo user, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {      
    }

    private AiData_UserSelf_General GetAiData(UserInfo user) {
      AiData_UserSelf_General data = user.GetAiStateInfo().AiDatas.GetData<AiData_UserSelf_General>();
      if (null == data) {
        data = new AiData_UserSelf_General();
        user.GetAiStateInfo().AiDatas.AddData(data);
      }
      return data;
    }
    private const long m_IntervalTime = 100;
    private const long m_ResponseTime = 1000;       // 遇敌逗留时间
    private const long m_MeetWalkMaxTime = 1500;
    private const long m_ChaseWalkMaxTime = 1000;
    private const long m_ChaseStandMaxTime = 1000;
    private const long m_TauntTime = 3000;

    private const float m_AttackRange = 2.0f;
  }
}





