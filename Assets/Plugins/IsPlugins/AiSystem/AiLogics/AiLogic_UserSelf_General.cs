using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptRuntime;

namespace DashFire
{
  class AiLogic_UserSelf_General : AbstractUserStateLogic
  {
    protected override void OnInitStateHandlers()
    {
      SetStateHandler((int)AiStateId.Idle, this.IdleHandler);
      SetStateHandler((int)AiStateId.Pursuit, this.PursuitHandler);
      SetStateHandler((int)AiStateId.Move, this.MoveHandler);
      SetStateHandler((int)AiStateId.Combat, this.CombatHandler);
      SetStateHandler((int)AiStateId.MoveCommand, this.MoveCommandHandler);
      SetStateHandler((int)AiStateId.PursuitCommand, this.PursuitCommandHandler);
    }
    protected override void OnStateLogicInit(UserInfo user, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      UserAiStateInfo info = user.GetAiStateInfo();
      info.HomePos = user.GetMovementStateInfo().GetPosition3D();
      info.Time = 0;
      user.GetMovementStateInfo().IsMoving = false;
    }
    private void IdleHandler(UserInfo user, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
    }

    private void CombatHandler(UserInfo user, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      if (user.IsDead())
        return;
      UserAiStateInfo info = user.GetAiStateInfo();
      info.Time += deltaTime;
      if (info.Time > 100) {
        AiData_UserSelf_General data = GetAiData(user);
        if (null != data) {
          data.Time += info.Time;
          info.Time = 0;
          ///
          CharacterInfo target = AiLogicUtility.GetSeeingLivingCharacterInfoHelper(user, info.Target);
          Vector3 targetPos = Vector3.Zero;
          float dist = info.AttackRange;
          if (null != target) {
            targetPos = target.GetMovementStateInfo().GetPosition3D();
          } else {
            targetPos = info.TargetPos;
          }
          Vector3 srcPos = user.GetMovementStateInfo().GetPosition3D();
          float powDist = Geometry.DistanceSquare(srcPos, targetPos);
          if (!info.IsAttacked) {
            if (powDist < dist * dist) {
              data.Time = 0;
              info.AttackRange = 0;
              info.IsAttacked = true;
              float dir = Geometry.GetYAngle(new Vector2(srcPos.X, srcPos.Z), new Vector2(targetPos.X, targetPos.Z));
              user.GetMovementStateInfo().SetFaceDir(dir);
              user.GetMovementStateInfo().SetMoveDir(dir);
              user.GetMovementStateInfo().SetWantFaceDir(dir);
              NotifyUserSkill(user, 1);
              ///
              user.GetMovementStateInfo().StopMove();
              NotifyUserMove(user);
              info.Time = 0;
              data.FoundPath.Clear();
              ChangeToState(user, (int)AiStateId.Idle);
            } else {
              user.GetMovementStateInfo().StopMove();
              NotifyUserMove(user);
              info.Time = 0;
              info.IsAttacked = false;
              data.FoundPath.Clear();
              ChangeToState(user, (int)AiStateId.Pursuit);
            }
          }
        } else {
          info.Time = 0;
        }
      }
    }

    private void MoveHandler(UserInfo user, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      if (user.IsDead())
        return;
      UserAiStateInfo info = user.GetAiStateInfo();
      AiData_UserSelf_General data = GetAiData(user);
      Vector3 targetPos = info.TargetPos;
      ScriptRuntime.Vector3 srcPos = user.GetMovementStateInfo().GetPosition3D();

      if (null != data && !IsReached(srcPos, targetPos)) {
        if (info.IsTargetPosChanged) {
          info.IsTargetPosChanged = false;
          data.FoundPath.Clear();
        }
        PathToTarget(user, data.FoundPath, targetPos, deltaTime);
      } else {
        user.GetMovementStateInfo().StopMove();
        NotifyUserMove(user);
        info.Time = 0;
        data.Time = 0;
        data.FoundPath.Clear();
        ChangeToState(user, (int)AiStateId.Idle);
      }
    }
    private void PursuitHandler(UserInfo user, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      if (user.IsDead())
        return;
      UserAiStateInfo info = user.GetAiStateInfo();
      AiData_UserSelf_General data = GetAiData(user);
      if (null != data) {
        if (info.Target > 0) {
          CharacterInfo target = AiLogicUtility.GetLivingCharacterInfoHelper(user, info.Target);
          if (null != target) {
            float dist = info.AttackRange - 1.0f;
            Vector3 targetPos = target.GetMovementStateInfo().GetPosition3D();
            ScriptRuntime.Vector3 srcPos = user.GetMovementStateInfo().GetPosition3D();
            float powDist = Geometry.DistanceSquare(srcPos, targetPos);
            if (powDist < dist * dist) {
              user.GetMovementStateInfo().IsMoving = false;
              info.Time = 0;
              data.Time = 0;
              ChangeToState(user, (int)AiStateId.Combat);
              NotifyUserMove(user);
            } else {
              info.Time += deltaTime;
              if (info.Time > 100) {
                info.Time = 0;
                CharacterInfo target2 = GetCanAttackUserTarget(user);
                if (null == target2)
                  AiLogicUtility.GetNearstTargetHelper(user, CharacterRelation.RELATION_ENEMY);
                if (null == target2 || target == target2) {
                  PathToTarget(user, data.FoundPath, targetPos, deltaTime);
                }
                else {
                  info.Target = target2.GetId();
                  return;
                }
              }
            }
          } else {
            user.GetMovementStateInfo().StopMove();
            NotifyUserMove(user);
            info.Time = 0;
            data.Time = 0;
            data.FoundPath.Clear();
            ChangeToState(user, (int)AiStateId.Idle);
          }
        } else {
          float dist = info.AttackRange;
          Vector3 targetPos = info.TargetPos;
          ScriptRuntime.Vector3 srcPos = user.GetMovementStateInfo().GetPosition3D();
          float powDist = Geometry.DistanceSquare(srcPos, targetPos);
          if (powDist < dist * dist) {
            user.GetMovementStateInfo().IsMoving = false;
            info.Time = 0;
            data.Time = 0;
            ChangeToState(user, (int)AiStateId.Combat);
            NotifyUserMove(user);
          } else {
            info.Time += deltaTime;
            if (info.Time > 100) {
              info.Time = 0;
              PathToTarget(user, data.FoundPath, targetPos, deltaTime);
            }
          }
        }
      }
    }
    private void MoveCommandHandler(UserInfo user, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      AiLogicUtility.DoMoveCommandState(user, aiCmdDispatcher, deltaTime, this);
    }
    private void PursuitCommandHandler(UserInfo user, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      AiLogicUtility.DoPursuitCommandState(user, aiCmdDispatcher, deltaTime, this);
    }
    private bool IsReached(Vector3 src, Vector3 target)
    {
      bool ret = false;
      float powDist = Geometry.DistanceSquare(src, target);
      if (powDist <= 0.25f) {
        ret = true;
      }
      return ret;
    }
    private CharacterInfo GetCanAttackUserTarget(UserInfo user)
    {
      //float dist = user.GetActualProperty().AttackRange;
      float dist = 3f;
      LinkedListDictionary<int, UserInfo> list = user.SceneContext.UserManager.Users;
      for (LinkedListNode<UserInfo> node = list.FirstValue; null != node; node = node.Next) {
        UserInfo other = node.Value;
        if (null != other && CharacterRelation.RELATION_ENEMY == CharacterInfo.GetRelation(user, other)) {
          if (Geometry.DistanceSquare(user.GetMovementStateInfo().GetPosition3D(), other.GetMovementStateInfo().GetPosition3D()) <= dist * dist) {
            return other;
          }
        }
      }
      return null;
    }
    private void PathToTarget(UserInfo user, AiPathData data, Vector3 pathTargetPos, long deltaTime)
    {
      UserAiStateInfo info = user.GetAiStateInfo();
      if (null != data) {
        data.UpdateTime += deltaTime;
        ScriptRuntime.Vector3 srcPos = user.GetMovementStateInfo().GetPosition3D();
        float dir = user.GetMovementStateInfo().GetMoveDir();
        bool findObstacle = false;
        bool havePathPoint = data.HavePathPoint;
        if (havePathPoint) {//沿路点列表移动的逻辑
          Vector3 targetPos = data.CurPathPoint;
          if (!data.IsReached(srcPos)) {//向指定路点移动（避让移动过程）
            user.GetMovementStateInfo().TargetPosition = targetPos;
            float angle = Geometry.GetYAngle(new Vector2(srcPos.X, srcPos.Z), new Vector2(targetPos.X, targetPos.Z));
            if (!Geometry.IsSameDouble(angle, user.GetMovementStateInfo().GetMoveDir())) {
              user.GetMovementStateInfo().SetFaceDir(angle);
              user.GetMovementStateInfo().SetMoveDir(angle);
              user.GetMovementStateInfo().IsMoving = true;
              NotifyUserMove(user);
            }
          } else {//改变路点或结束沿路点移动
            data.UseNextPathPoint();
            if (data.HavePathPoint) {
              targetPos = data.CurPathPoint;
              user.GetMovementStateInfo().TargetPosition = targetPos;
              float angle = Geometry.GetYAngle(new Vector2(srcPos.X, srcPos.Z), new Vector2(targetPos.X, targetPos.Z));
              user.GetMovementStateInfo().SetFaceDir(angle);
              user.GetMovementStateInfo().SetMoveDir(angle);
              user.GetMovementStateInfo().IsMoving = true;
              NotifyUserMove(user);
            } else {
              data.Clear();
            }
          }
        }
        if (!havePathPoint || findObstacle) {//获得路点过程（寻路）
          data.Clear();
          Vector3 targetPos = pathTargetPos;
          if (Geometry.DistanceSquare(srcPos, targetPos) > 400) {
            targetPos = user.SpatialSystem.CalcNearstReachablePoint(srcPos, targetPos, 20);
          }
          bool canGo = true;
          if (!user.SpatialSystem.GetCellMapView(user.AvoidanceRadius).CanPass(targetPos)) {
            if (!AiLogicUtility.GetWalkablePosition(user.SpatialSystem.GetCellMapView(user.AvoidanceRadius), targetPos, srcPos, ref targetPos))
              canGo = false;
          }
          if (canGo) {
            List<Vector3> posList = null;
            if (user.SpatialSystem.CanPass(user.SpaceObject, targetPos)) {
              posList = new List<Vector3>();
              posList.Add(srcPos);
              posList.Add(targetPos);
            } else {
              long stTime = TimeUtility.GetElapsedTimeUs();
              posList = user.SpatialSystem.FindPath(srcPos, targetPos, user.AvoidanceRadius);
              long endTime = TimeUtility.GetElapsedTimeUs();
              long calcTime = endTime - stTime;
              if (calcTime > 1000) {
                // LogSystem.Warn("pvp FindPath consume {0} us,user:{1} from:{2} to:{3} radius:{4} pos:{5}", calcTime, user.GetId(), srcPos.ToString(), targetPos.ToString(), user.AvoidanceRadius, user.GetMovementStateInfo().GetPosition3D().ToString());
              }
            }
            if (posList.Count >= 2) {
              data.SetPathPoints(posList[0], posList, 1);
              targetPos = data.CurPathPoint;
              user.GetMovementStateInfo().TargetPosition = targetPos;
              float angle = Geometry.GetYAngle(new Vector2(srcPos.X, srcPos.Z), new Vector2(targetPos.X, targetPos.Z));
              user.GetMovementStateInfo().SetFaceDir(angle);
              user.GetMovementStateInfo().SetMoveDir(angle);
              user.GetMovementStateInfo().IsMoving = true;
              NotifyUserMove(user);
            } else {
              user.GetMovementStateInfo().IsMoving = false;
              NotifyUserMove(user);
            }
          } else {
            user.GetMovementStateInfo().IsMoving = false;
            NotifyUserMove(user);
          }
        }
      }
    }
    private AiData_UserSelf_General GetAiData(UserInfo user)
    {
      AiData_UserSelf_General data = user.GetAiStateInfo().AiDatas.GetData<AiData_UserSelf_General>();
      if (null == data) {
        data = new AiData_UserSelf_General();
        user.GetAiStateInfo().AiDatas.AddData(data);
      }
      return data;
    }
  }
}
