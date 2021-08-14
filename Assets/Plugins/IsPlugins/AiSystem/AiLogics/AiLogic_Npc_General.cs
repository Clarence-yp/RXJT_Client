using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptRuntime;

namespace DashFire {
  class AiLogic_Npc_General : AbstractNpcStateLogic {

    protected override void OnInitStateHandlers()
    {
      SetStateHandler((int)AiStateId.Idle, this.IdleHandler);
      SetStateHandler((int)AiStateId.Pursuit, this.PursuitHandler);
      SetStateHandler((int)AiStateId.Combat, this.CombatHandler);
      SetStateHandler((int)AiStateId.MoveCommand, this.MoveCommandHandler);
      SetStateHandler((int)AiStateId.PursuitCommand, this.PursuitCommandHandler);
      SetStateHandler((int)AiStateId.PatrolCommand, this.PatrolCommandHandler);
    }

    protected override void OnStateLogicInit(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      NpcAiStateInfo info = npc.GetAiStateInfo();
      info.Time = 0;
      npc.GetMovementStateInfo().IsMoving = false;
      info.HomePos = npc.GetMovementStateInfo().GetPosition3D();
      info.Target = 0;
    }

    private void IdleHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      if (!CanAiControl(npc)) {
        npc.GetMovementStateInfo().IsMoving = false;
        return;
      }
      NpcAiStateInfo info = npc.GetAiStateInfo();
      info.Time += deltaTime;
      if (info.Time > 100) {
        info.Time = 0;
        if (GetAiData(npc).HasPatrolData) {
          npc.GetMovementStateInfo().IsMoving = false;
          ChangeToState(npc, (int)AiStateId.PatrolCommand);
        } else {
          CharacterInfo target = null;
          if (info.IsExternalTarget) {
            target = AiLogicUtility.GetSeeingLivingCharacterInfoHelper(npc, info.Target);
            if (null == target) {
              target = AiLogicUtility.GetNearstTargetHelper(npc, CharacterRelation.RELATION_ENEMY);
              if (null != target)
                info.Target = target.GetId();
            }
          } else {
            target = AiLogicUtility.GetNearstTargetHelper(npc, CharacterRelation.RELATION_ENEMY);
            if (null != target)
              info.Target = target.GetId();
          }
          if (null != target) {
            NotifyNpcTargetChange(npc);
            npc.GetMovementStateInfo().IsMoving = false;
            NotifyNpcMove(npc);
            info.Time = 0;
            ChangeToState(npc, (int)AiStateId.Pursuit);
          }
        }
      }
    }

    private void PursuitHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      if (!CanAiControl(npc)) {
        npc.GetMovementStateInfo().IsMoving = false;
        return;
      }
      NpcAiStateInfo info = npc.GetAiStateInfo();
      AiData_Demo_Melee data = GetAiData(npc);
      if (null != data) {
        CharacterInfo target = AiLogicUtility.GetLivingCharacterInfoHelper(npc, info.Target);
        if (null != target) {
          float dist = (float)npc.GetActualProperty().AttackRange;
          float distGoHome = (float)npc.GohomeRange;
          Vector3 targetPos = target.GetMovementStateInfo().GetPosition3D();
          ScriptRuntime.Vector3 srcPos = npc.GetMovementStateInfo().GetPosition3D();
          float powDist = Geometry.DistanceSquare(srcPos, targetPos);
          float powDistToHome = Geometry.DistanceSquare(srcPos, info.HomePos);
          if (powDist < dist * dist) {
            npc.GetMovementStateInfo().IsMoving = false;
            info.Time = 0;
            data.Time = 0;
            ChangeToState(npc, (int)AiStateId.Combat);
            NotifyNpcMove(npc);
          } else{
            info.Time += deltaTime;
            if (info.Time > m_IntervalTime) {
              info.Time = 0;
              AiLogicUtility.PathToTargetWithoutObstacle(npc, data.FoundPath, targetPos, m_IntervalTime, true, this);
            }
          }
        }
      }
    }

    private void CombatHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      if (!CanAiControl(npc)) {
        npc.GetMovementStateInfo().IsMoving = false;
        return;
      }
      NpcAiStateInfo info = npc.GetAiStateInfo();
      info.Time += deltaTime;
      if (info.Time > m_IntervalTime) {
        AiData_Demo_Melee data = GetAiData(npc);
        if (null != data) {
          data.Time += info.Time;
          info.Time = 0;
          CharacterInfo target = AiLogicUtility.GetLivingCharacterInfoHelper(npc, info.Target);
          //CharacterInfo target = AiLogicUtility.GetInterestestTargetHelper(npc, CharacterRelation.RELATION_ENEMY, AiTargetType.USER);
          if (null != target) {
            float dist = (float)npc.GetActualProperty().AttackRange;
            float distGoHome = (float)npc.GohomeRange;
            ScriptRuntime.Vector3 targetPos = target.GetMovementStateInfo().GetPosition3D();
            ScriptRuntime.Vector3 srcPos = npc.GetMovementStateInfo().GetPosition3D();
            float powDist = Geometry.DistanceSquare(srcPos, targetPos);
            float powDistToHome = Geometry.DistanceSquare(srcPos, info.HomePos);
            if (powDist < dist * dist) {
              float rps = npc.GetActualProperty().Rps;
              long curTime = TimeUtility.GetServerMilliseconds();
              float dir = Geometry.GetYAngle(new Vector2(srcPos.X, srcPos.Z), new Vector2(targetPos.X, targetPos.Z));
              NotifyNpcFace(npc, dir);
              //if (rps > 0.001f && curTime - data.LastUseSkillTime > 1000 / rps) {
              data.Time = 0;
              data.LastUseSkillTime = curTime;
              List<SkillInfo> skillInfos = npc.GetSkillStateInfo().GetAllSkill();
              if (skillInfos.Count > 0) {
                int index = Helper.Random.Next(skillInfos.Count);
                if (null != skillInfos[index]) {
                  NotifyNpcSkill(npc, skillInfos[index].SkillId);
                }
              }
              //}
            } else{
              npc.GetMovementStateInfo().IsMoving = false;
              NotifyNpcMove(npc);
              info.Time = 0;
              data.FoundPath.Clear();
              ChangeToState(npc, (int)AiStateId.Pursuit);
            }
          }
        } else {
          info.Time = 0;
        }
      }
    }
    private void GoHomeHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      if (!CanAiControl(npc)) {
        npc.GetMovementStateInfo().IsMoving = false;
        return;
      }
      ChangeToState(npc, (int)AiStateId.Idle);
      NpcAiStateInfo info = npc.GetAiStateInfo();
      info.Time += deltaTime;
      if (info.Time > m_IntervalTime) {
        info.Time = 0;
        AiData_Demo_Melee data = GetAiData(npc);
        if (null != data) {
          Vector3 targetPos = info.HomePos;
          ScriptRuntime.Vector3 srcPos = npc.GetMovementStateInfo().GetPosition3D();
            float powDistToHome = Geometry.DistanceSquare(srcPos, info.HomePos);
          if (powDistToHome <= 1) {
            npc.GetMovementStateInfo().IsMoving = false;
            info.Time = 0;
            ChangeToState(npc, (int)AiStateId.Idle);
          } else {
            AiLogicUtility.PathToTargetWithoutObstacle(npc, data.FoundPath, targetPos, 100, true, this);
          }
        }
      }
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
    private AiData_Demo_Melee GetAiData(NpcInfo npc)
    {
      AiData_Demo_Melee data = npc.GetAiStateInfo().AiDatas.GetData<AiData_Demo_Melee>();
      if (null == data) {
        data = new AiData_Demo_Melee();
        npc.GetAiStateInfo().AiDatas.AddData(data);
      }
      return data;
    }
    private const long m_IntervalTime = 100;
    private const long m_WaltMaxTime = 2000;
    private const long m_ResponseTime = 2000;
  }
}


