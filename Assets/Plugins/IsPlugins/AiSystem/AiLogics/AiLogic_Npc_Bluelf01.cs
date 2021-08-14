using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptRuntime;

namespace DashFire {
  class AiLogic_Npc_Bluelf01 : AbstractNpcStateLogic {

    protected override void OnInitStateHandlers()
    {
      SetStateHandler((int)AiStateId.Idle, this.IdleHandler);
      SetStateHandler((int)AiStateId.Pursuit, this.PursuitHandler);
      SetStateHandler((int)AiStateId.Patrol, this.PatrolHandler);
      SetStateHandler((int)AiStateId.MoveCommand, this.MoveCommandHandler);
      SetStateHandler((int)AiStateId.PursuitCommand, this.PursuitCommandHandler);
      SetStateHandler((int)AiStateId.PatrolCommand, this.PatrolCommandHandler);
    }

    private void IdleHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      AiLogicUtility.CommonIdleHandler(npc, aiCmdDispatcher, deltaTime, this);
    }

    private void PursuitHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      if (!CanAiControl(npc)) {
        npc.GetMovementStateInfo().IsMoving = false;
        return;
      }
      NpcAiStateInfo info = npc.GetAiStateInfo();
      AiData_Npc_Bluelf data = GetAiData(npc);
      if (null != data) {
        CharacterInfo target = AiLogicUtility.GetLivingCharacterInfoHelper(npc, info.Target);
        if (null != target) {
          float dist = (float)npc.GetActualProperty().AttackRange;
          float distGoHome = (float)npc.GohomeRange;
          Vector3 targetPos = target.GetMovementStateInfo().GetPosition3D();
          ScriptRuntime.Vector3 srcPos = npc.GetMovementStateInfo().GetPosition3D();
          float powDist = Geometry.DistanceSquare(srcPos, targetPos);
          float powDistToHome = Geometry.DistanceSquare(srcPos, info.HomePos);
          // 遇敌是播放特效， 逗留两秒。
          if (data.WaitTime <= m_ResponseTime) {
            if (!data.HasMeetEnemy) {
              NotifyNpcMeetEnemy(npc, Animation_Type.AT_Attack);
              data.HasMeetEnemy = true;
            }
            TrySeeTarget(npc, target);
            data.WaitTime += deltaTime;
            return;
          }
          // 走向目标1.5秒
          if (data.MeetEnemyWalkTime < m_MeetWalkMaxTime) {
            data.MeetEnemyWalkTime += deltaTime;
            NotifyNpcWalk(npc);
            info.Time += deltaTime;
            if (info.Time > m_IntervalTime) {
              info.Time = 0;
              AiLogicUtility.PathToTargetWithoutObstacle(npc, data.FoundPath, targetPos, m_IntervalTime, true, this);
            }
            return;
          }

          // 大于攻击距离 跑向目标
          if (powDist > m_AttackRange * m_AttackRange && 0 == data.CurAiAction) {
            npc.IsTaunt = false;
            NotifyNpcRun(npc);
            info.Time += deltaTime;
            if (info.Time > m_IntervalTime) {
              info.Time = 0;
              AiLogicUtility.PathToTargetWithoutObstacle(npc, data.FoundPath, targetPos, m_IntervalTime, true, this);
            }
          } else {
            //小于攻击距离 
            if (data.CurAiAction == (int)AiAction.NONE) {
              npc.GetMovementStateInfo().IsMoving = false;
              NotifyNpcMove(npc);
              data.CurAiAction = (int)GetNextAction();
            }
            switch (data.CurAiAction) {
              case (int)AiAction.SKILL: 
                int skillId = GetCanCastSkillId(npc, (float)Math.Sqrt(powDist));
                if (-1 == skillId) {
                  info.Time += deltaTime;
                  if (info.Time > m_IntervalTime) {
                    info.Time = 0;
                    NotifyNpcRun(npc);
                    AiLogicUtility.PathToTargetWithoutObstacle(npc, data.FoundPath, targetPos, m_IntervalTime, true, this);
                  }
                } else {
                  npc.GetMovementStateInfo().IsMoving = false;
                  NotifyNpcMove(npc);
                  if (TryCastSkill(npc, skillId, target)) {
                    data.CurAiAction = 0;
                  }
                }
                break;
              case (int)AiAction.STAND:
                data.ChaseStandTime += deltaTime;
                TrySeeTarget(npc, target);
                if (data.ChaseStandTime > m_ChaseWalkMaxTime) {
                  data.ChaseStandTime = 0;
                  data.CurAiAction = 0;
                }
                break;
              case (int)AiAction.TAUNT:
                npc.GetMovementStateInfo().IsMoving = false;
                NotifyNpcMove(npc);
                npc.IsTaunt = true;
                data.TauntTime += deltaTime;
                TrySeeTarget(npc, target);
                if (data.TauntTime > m_TauntTime) {
                  npc.IsTaunt = false;
                  data.TauntTime = 0;
                  data.CurAiAction = 0;
                }
                break;
              case (int)AiAction.WALK:
                data.ChaseWalkTime += deltaTime;
                info.Time += deltaTime;
                if (info.Time > m_IntervalTime) {
                  info.Time = 0;
                  NotifyNpcWalk(npc);
                  AiLogicUtility.PathToTargetWithoutObstacle(npc, data.FoundPath, targetPos, m_IntervalTime, true, this);
                }
                if (data.ChaseWalkTime > m_ChaseWalkMaxTime) {
                  data.ChaseWalkTime = 0;
                  data.CurAiAction = 0;
                }
                break;
            }
          }
        }
      }
    }

    private int GetCanCastSkillId(NpcInfo npc, float dis) {
      List<SkillInfo> skills = npc.GetSkillStateInfo().GetAllSkill();
      List<int> canCastSkills = new List<int>();
      foreach (SkillInfo skill in skills) {
        if (skill.ConfigData.SkillRangeMin < dis && skill.ConfigData.SkillRangeMax > dis) {
          canCastSkills.Add(skill.SkillId);
        }
      }
      if (canCastSkills.Count > 0) {
        return canCastSkills[Helper.Random.Next(canCastSkills.Count)];
      } else {
        return -1;
      }
    }
    private AiAction GetNextAction() {
      int roll = Helper.Random.Next(100);
      if (roll <= 30) {
        return AiAction.WALK;
      } else if (roll <= 60) {
        return AiAction.TAUNT;
      } else if (roll <= 90) {
        return AiAction.SKILL;
      } else {
        return AiAction.STAND;
      }
    }

    private void PatrolHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      AiLogicUtility.CommonPatrolHandler(npc, aiCmdDispatcher, deltaTime, this);
    }
    private void MoveCommandHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      AiLogicUtility.DoMoveCommandState(npc, aiCmdDispatcher, deltaTime, this);
    }
    private void PursuitCommandHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      AiLogicUtility.DoPursuitCommandState(npc, aiCmdDispatcher, deltaTime, this);
    }
    private void PatrolCommandHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {      
      AiLogicUtility.DoPatrolCommandState(npc, aiCmdDispatcher, deltaTime, this);
    }

    private AiData_Npc_Bluelf GetAiData(NpcInfo npc)
    {
      AiData_Npc_Bluelf data = npc.GetAiStateInfo().AiDatas.GetData<AiData_Npc_Bluelf>();
      if (null == data) {
        data = new AiData_Npc_Bluelf();
        npc.GetAiStateInfo().AiDatas.AddData(data);
      }
      return data;
    }
    private const long m_IntervalTime = 100;
    private const long m_ResponseTime = 1000;       // 遇敌逗留时间
    private const long m_MeetWalkMaxTime = 1500;
    private const long m_ChaseWalkMaxTime = 1000;
    private const long m_TauntTime = 3000;

    private const float m_AttackRange = 8.0f;
  }
}



