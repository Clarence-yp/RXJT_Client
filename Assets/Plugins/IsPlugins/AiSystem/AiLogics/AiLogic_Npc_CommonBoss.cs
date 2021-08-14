using System;
using System.Collections.Generic;
using ScriptRuntime;

namespace DashFire
{
  public class AiLogic_Npc_CommonBoss : AbstractNpcStateLogic
  {
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
      if (npc.IsDead()) {
        npc.GetMovementStateInfo().IsMoving = false;
        NotifyNpcMove(npc);
        return;
      }
      NpcAiStateInfo info = npc.GetAiStateInfo();
      AiData_Npc_CommonBoss data = GetAiData(npc);
      if (null != data) {
        CharacterInfo target = AiLogicUtility.GetLivingCharacterInfoHelper(npc, info.Target);
        if (null != target) {
          if (npc.GetSkillStateInfo().IsSkillActivated()) {
            npc.GetMovementStateInfo().IsMoving = false;
            NotifyNpcMove(npc);
            data.LastUseSkillTime = TimeUtility.GetServerMilliseconds();
            return;
          }
          if (npc.GetSkillStateInfo().IsImpactControl()) {
            data.ControlTime += deltaTime;
            if (data.ControlTime >= m_MaxControlTime && AiLogicUtility.CanCastSkill(npc, m_DisControlSkillId, target)) {
              NotifyNpcAddImpact(npc, m_DisControlImpactId);
              data.ControlTime = 0;
              TryCastSkill(npc, m_DisControlSkillId, target, false);
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
          int skillId = GetCanCastSkillId(npc, (float)Math.Sqrt(powDist));
          long curTime = TimeUtility.GetServerMilliseconds();
          if (-1 != skillId && curTime - data.LastUseSkillTime > m_SkillInterval && AiLogicUtility.CanCastSkill(npc, skillId, target)) {
            if (TryCastSkill(npc, skillId, target, true)) {
              NotifyNpcAddImpact(npc, m_SuperArmorImpactId);
            }
          } else {
            info.Time += deltaTime;
            if (info.Time > m_IntervalTime) {
              info.Time = 0;
              AiLogicUtility.PathToTargetWithoutObstacle(npc, data.FoundPath, targetPos, m_IntervalTime, true, this);
            }
          }
        }
      }
    }

    private int GetCanCastSkillId(NpcInfo npc, float dis) {
      List<SkillInfo> skills = npc.GetSkillStateInfo().GetAllSkill();
      List<int> canCastSkills = new List<int>();
      foreach (SkillInfo skill in skills) {
        if (skill.ConfigData.SkillRangeMin < dis && skill.ConfigData.SkillRangeMax > dis && skill.SkillId != m_DisControlSkillId) {
          canCastSkills.Add(skill.SkillId);
        }
      }
      if (canCastSkills.Count > 0) {
        return canCastSkills[Helper.Random.Next(canCastSkills.Count)];
      } else {
        return -1;
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

    private AiData_Npc_CommonBoss GetAiData(NpcInfo npc)
    {
      AiData_Npc_CommonBoss data = npc.GetAiStateInfo().AiDatas.GetData<AiData_Npc_CommonBoss>();
      if (null == data) {
        data = new AiData_Npc_CommonBoss();
        npc.GetAiStateInfo().AiDatas.AddData(data);
      }
      return data;
    }

    private const long m_ResponseTime = 100;
    private const long m_IntervalTime = 100;
    private const long m_SkillInterval = 500;
    private const int m_SuperArmorImpactId = 30010004;
    private const int m_SuperArmorTime = 500;
    private const long m_MaxControlTime = 10000;
    private const int m_DisControlSkillId = 380202;
    private const int m_DisControlImpactId = 88889;
  }
}
