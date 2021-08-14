using System;
using System.Collections.Generic;
using ScriptRuntime;

namespace DashFire {
  class AiLogic_Npc_BossHulun : AbstractNpcStateLogic {

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
      if (npc.IsDead()){
          npc.GetMovementStateInfo().IsMoving = false;
          NotifyNpcMove(npc);
        return;
      }
      NpcAiStateInfo info = npc.GetAiStateInfo();
      AiData_Npc_BossHulun data = GetAiData(npc);
      if (null != data) {
        CharacterInfo target = AiLogicUtility.GetLivingCharacterInfoHelper(npc, info.Target);
        if (null != target) {
          if (npc.GetSkillStateInfo().IsImpactControl()) {
            data.ControlTime += deltaTime;
            if (data.ControlTime > m_MaxControlTime) {
              NotifyNpcAddImpact(npc, m_SuperArmorImpactId);
              data.ControlTime = 0;
              NotifyNpcSkill(npc, 380308);
            }
          }
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
          if (npc.GetSkillStateInfo().IsSkillActivated()) {
            SkillInfo curSkill = npc.GetSkillStateInfo().GetCurSkillInfo();
            if (null != curSkill && curSkill.SkillId == data.ChaseTargetSkill) {
              NotifyNpcRun(npc);
              AiLogicUtility.PathToTargetWithoutObstacle(npc, data.FoundPath, targetPos, deltaTime, true, this);
              data.IsUsingChaseSkill = true;
            }
            return;
          } else if(data.IsUsingChaseSkill) {
            npc.GetMovementStateInfo().IsMoving = false;
            NotifyNpcMove(npc);
            data.IsUsingChaseSkill = false;
          }
          if (1 == data.CurStage) {
            if (npc.Hp < npc.GetActualProperty().HpMax * data.StageTwoLimit) {
              data.CurStage = 2;
              NotifyNpcSkill(npc, data.EnterStageTwoSkill);
              data.CurSkillCombo = null;
              data.CurSkillComboIndex = 0;
              return;
            } else {
              if (null == data.CurSkillCombo) {
                data.CurSkillCombo = data.m_StageOneSkillCombo;
                data.CurSkillComboIndex = 0;
              }
            }
          } else if(2 == data.CurStage) {
            if (npc.Hp < npc.GetActualProperty().HpMax * data.StageThreeLimit) {
              data.CurStage = 3;
              NotifyNpcSkill(npc, data.EnterStageTwoSkill);
              data.CurSkillCombo = null;
              data.CurSkillComboIndex = 0;
              return;
            } else {
              if (null == data.CurSkillCombo) {
                data.CurSkillCombo = data.m_StageTwoSkillCombo;
                data.CurSkillComboIndex = 0;
              }
            }
          } else if (3 == data.CurStage) {
            if (null == data.CurSkillCombo) {
              data.CurSkillCombo = data.m_StageThreeSkillCombo;
              data.CurSkillComboIndex = 0;
            }
          }
          if (null != data.CurSkillCombo) {
            if (data.CurSkillCombo.Length > data.CurSkillComboIndex) {
              if (TryCastSkill(npc, data.CurSkillCombo[data.CurSkillComboIndex], target)) {
                data.CurSkillComboIndex = data.CurSkillComboIndex + 1;
                return;
              }
            } else {
              data.CurSkillCombo = null;
              data.CurSkillComboIndex = 0;
            }
          }
          // 大于攻击距离 跑向目标
          info.Time += deltaTime;
          if (info.Time > m_IntervalTime) {
            NotifyNpcRun(npc);
            info.Time = 0;
            AiLogicUtility.PathToTargetWithoutObstacle(npc, data.FoundPath, targetPos, m_IntervalTime, true, this);
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

    private bool IsCanUseComboSkills(NpcInfo npc, CharacterInfo target, int[] combo)
    {
      for (int i = 0; i < combo.Length; ++i) {
        if (!AiLogicUtility.CanCastSkill(npc, combo[i], target)) {
          return false;
        }
      }
      return true;
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
    private AiData_Npc_BossHulun GetAiData(NpcInfo npc)
    {
      AiData_Npc_BossHulun data = npc.GetAiStateInfo().AiDatas.GetData<AiData_Npc_BossHulun>();
      if (null == data) {
        data = new AiData_Npc_BossHulun();
        npc.GetAiStateInfo().AiDatas.AddData(data);
      }
      return data;
    }
    private const long m_IntervalTime = 100;
    private const long m_ResponseTime = 100;       // 遇敌逗留时间
    private const long m_MaxControlTime = 4000;    // 最长受控时间
    private const int m_SuperArmorImpactId = 38030001; // 霸体impact

    private const int m_DisControlImpact = 88889;
    private const float m_AttackRange = 10.0f;

  }
}





