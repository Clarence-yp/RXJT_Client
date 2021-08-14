using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptRuntime;

namespace DashFire {
  class AiLogic_Npc_BossDevilWarrior : AbstractNpcStateLogic {

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
      if (npc.IsDead() ||
          npc.GetSkillStateInfo().IsSkillActivated()){
          npc.GetMovementStateInfo().IsMoving = false;
          NotifyNpcMove(npc);
        return;
      }
      NpcAiStateInfo info = npc.GetAiStateInfo();
      AiData_Npc_BossDevilWarrior data = GetAiData(npc);
      if (null != data) {
        CharacterInfo target = AiLogicUtility.GetLivingCharacterInfoHelper(npc, info.Target);
        if (null != target) {
          if (npc.GetSkillStateInfo().IsImpactControl()) {
            data.ControlTime += deltaTime;
            if (data.ControlTime >= m_MaxControlTime && AiLogicUtility.CanCastSkill(npc, data.DecontrolSkill, target)) {
              NotifyNpcAddImpact(npc, m_DisControlImpact);
              data.ControlTime = 0;
              TryCastSkill(npc, data.DecontrolSkill, target, false);
              data.CurSkillCombo = data.DecontrolCombo;
              data.CurSkillComboIndex = 0;
            }
            return;
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
          if (null != data.CurSkillCombo) {
            if (data.CurSkillCombo.Length > data.CurSkillComboIndex) {
              TryCastSkill(npc, data.CurSkillCombo[data.CurSkillComboIndex], target);
              data.CurSkillComboIndex = data.CurSkillComboIndex + 1;
            } else {
              data.CurSkillCombo = null;
              data.CurSkillComboIndex = 0;
            }
          }
          // 大于攻击距离 跑向目标
          if (powDist > m_AttackRange * m_AttackRange) {
            if ((int)GfxCharacterState_Type.HitFly == target.GfxStateFlag) {
              if (TryCastSkill(npc, data.FlyRangeSkill, target)) {
                return;
              }
            } else {
              if (TryCastSkill(npc, data.RangeSkill, target)) {
                return;
              }
            }
            info.Time += deltaTime;
            if (info.Time > m_IntervalTime) {
              NotifyNpcRun(npc);
              info.Time = 0;
              AiLogicUtility.PathToTargetWithoutObstacle(npc, data.FoundPath, targetPos, m_IntervalTime, true, this);
            }
          } else {
            if ((int)GfxCharacterState_Type.HitFly == target.GfxStateFlag) {
              if (TryCastSkill(npc, data.FlyMeeleSkill, target)) {
                return;
              }
            } else {
              if (TryCastSkill(npc, data.MeeleSkill, target)) {
                return;
              }
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
    private AiData_Npc_BossDevilWarrior GetAiData(NpcInfo npc)
    {
      AiData_Npc_BossDevilWarrior data = npc.GetAiStateInfo().AiDatas.GetData<AiData_Npc_BossDevilWarrior>();
      if (null == data) {
        data = new AiData_Npc_BossDevilWarrior();
        npc.GetAiStateInfo().AiDatas.AddData(data);
      }
      return data;
    }
    private const long m_IntervalTime = 100;
    private const long m_ResponseTime = 100;       // 遇敌逗留时间
    private const long m_MaxControlTime = 10000;

    private const int m_DisControlImpact = 88889;
    private const float m_AttackRange = 10.0f;

  }
}






