using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptRuntime;
using DashFireSpatial;

namespace DashFire
{
  public enum AiTargetType : int
  {
    USER = 0,
    NPC,
    ALL,
    TOWER,
    SOLDIER,
  }
  public sealed class AiLogicUtility
  {
    public const int c_MaxViewRange = 30;
    public const int c_MaxViewRangeSqr = c_MaxViewRange * c_MaxViewRange;
    public static CharacterInfo GetNearstTargetHelper(CharacterInfo srcObj, CharacterRelation relation)
    {
      return GetNearstTargetHelper(srcObj, relation, AiTargetType.ALL);
    }
    public static CharacterInfo GetNearstTargetHelper(CharacterInfo srcObj, CharacterRelation relation, AiTargetType type)
    {
      CharacterInfo nearstTarget = null;
      DashFireSpatial.ISpatialSystem spatialSys = srcObj.SpatialSystem;
      if (null != spatialSys) {
        ScriptRuntime.Vector3 srcPos = srcObj.GetMovementStateInfo().GetPosition3D();
        float minPowDist = 999999;
        spatialSys.VisitObjectInCircle(srcPos, srcObj.ViewRange, (float distSqr, ISpaceObject obj) => {
          StepCalcNearstTarget(srcObj, relation, type, distSqr, obj, ref minPowDist, ref nearstTarget);
        });
      }
      return nearstTarget;
    }

    public static CharacterInfo GetInterestestTargetHelper(CharacterInfo srcObj, CharacterRelation relation, AiTargetType type)
    {
      CharacterInfo interestestTarget = null;
      DashFireSpatial.ISpatialSystem spatialSys = srcObj.SpatialSystem;
      if (null != spatialSys) {
        ScriptRuntime.Vector3 srcPos = srcObj.GetMovementStateInfo().GetPosition3D();
        float minPowDist = 999999;
        spatialSys.VisitObjectInCircle(srcPos, srcObj.ViewRange, (float distSqr, ISpaceObject obj) => {
          GetInterestestTarget(srcObj, relation, type, distSqr, obj, ref minPowDist, ref interestestTarget);
        });
      }
      return interestestTarget;
    }

    public static CharacterInfo GetRandomTargetHelper(CharacterInfo srcObj, CharacterRelation relation)
    {
      return GetNearstTargetHelper(srcObj, relation, AiTargetType.ALL);
    }
    public static CharacterInfo GetRandomTargetHelper(CharacterInfo srcObj, CharacterRelation relation, AiTargetType type)
    {
      CharacterInfo target = null;
      DashFireSpatial.ISpatialSystem spatialSys = srcObj.SpatialSystem;
      if (null != spatialSys) {
        List<DashFireSpatial.ISpaceObject> objs = spatialSys.GetObjectInCircle(srcObj.GetMovementStateInfo().GetPosition3D(), srcObj.ViewRange, (distSqr, obj) => IsWantedTargetForRandomTarget(srcObj, relation, type, obj));
        int index = Helper.Random.Next(objs.Count);
        if (index >= 0 && index < objs.Count) {
          target = objs[index].RealObject as CharacterInfo;
        }
      }
      return target;
    }
    public static CharacterInfo GetLivingCharacterInfoHelper(CharacterInfo srcObj, int id)
    {
      CharacterInfo target = srcObj.NpcManager.GetNpcInfo(id);
      if (null == target) {
        target = srcObj.UserManager.GetUserInfo(id);
      }
      if (null != target) {
        if (target.IsDead())
          target = null;
      }
      return target;
    }
    public static CharacterInfo GetSeeingLivingCharacterInfoHelper(CharacterInfo srcObj, int id)
    {
      CharacterInfo target = srcObj.NpcManager.GetNpcInfo(id);
      if (null == target) {
        target = srcObj.UserManager.GetUserInfo(id);
      }
      if (null != target) {
        if (target.IsHaveStateFlag(CharacterState_Type.CST_Hidden))
          target = null;
        else if (target.IsDead())
          target = null;
        else if (!CanSee(srcObj, target))
          target = null;
      }
      return target;
    }

    private static void GetInterestestTarget(CharacterInfo srcObj, CharacterRelation relation, AiTargetType type, float powDist, ISpaceObject obj, ref float minPowDist, ref CharacterInfo interestestTarget)
    {
      if (type == AiTargetType.USER && obj.GetObjType() != SpatialObjType.kUser) return;
      if (type == AiTargetType.TOWER && obj.GetObjType() != SpatialObjType.kNPC) return;
      if (type == AiTargetType.SOLDIER && obj.GetObjType() != SpatialObjType.kNPC) return;
      CharacterInfo target = GetSeeingLivingCharacterInfoHelper(srcObj, (int)obj.GetID());
      if (null != target && !target.IsDead()) {
        if (target.IsControlMecha) {
          return;
        }
        NpcInfo npcTarget = target.CastNpcInfo();
        if (null != npcTarget && npcTarget.NpcType == (int)NpcTypeEnum.Skill) {
          return;
        }
        if (type == AiTargetType.SOLDIER && npcTarget.IsPvpTower)
          return;
        if (type == AiTargetType.TOWER && !npcTarget.IsPvpTower)
          return;

        if (relation == CharacterInfo.GetRelation(srcObj, target)) {
          if (powDist < minPowDist) {
            if (powDist > c_MaxViewRangeSqr || CanSee(srcObj, target)) {
              interestestTarget = target;
              minPowDist = powDist;
            }
          }
        }
      }
    }

    private static void StepCalcNearstTarget(CharacterInfo srcObj, CharacterRelation relation, AiTargetType type, float powDist, ISpaceObject obj, ref float minPowDist, ref CharacterInfo nearstTarget)
    {
      if (type == AiTargetType.USER && obj.GetObjType() != SpatialObjType.kUser) return;
      if (type == AiTargetType.NPC && obj.GetObjType() != SpatialObjType.kNPC) return;
      CharacterInfo target = GetSeeingLivingCharacterInfoHelper(srcObj, (int)obj.GetID());
      if (null != target && !target.IsDead()) {
        if (target.IsControlMecha) {
          return;
        }
        NpcInfo npcTarget = target.CastNpcInfo();
        if (null != npcTarget) {
          if (npcTarget.NpcType == (int)NpcTypeEnum.Skill || npcTarget.NpcType == (int)NpcTypeEnum.AutoPickItem) {
            return;
          }
        }
        if (relation == CharacterInfo.GetRelation(srcObj, target)) {
          if (powDist < minPowDist) {
            if (powDist > c_MaxViewRangeSqr || CanSee(srcObj, target)) {
              nearstTarget = target;
              minPowDist = powDist;
            }
          }
        }
      }
    }
    private static bool IsWantedTargetForRandomTarget(CharacterInfo srcObj, CharacterRelation relation, AiTargetType type, ISpaceObject obj)
    {
      if (type == AiTargetType.USER && obj.GetObjType() != SpatialObjType.kUser) return false;
      if (type == AiTargetType.NPC && obj.GetObjType() != SpatialObjType.kNPC) return false;
      CharacterInfo target = GetSeeingLivingCharacterInfoHelper(srcObj, (int)obj.GetID());
      if (null != target && !target.IsDead()) {
        if (target.IsControlMecha) {
          return false;
        }
        NpcInfo npcTarget = target.CastNpcInfo();
        if (null != npcTarget && npcTarget.NpcType == (int)NpcTypeEnum.Skill) {
          return false;
        }
        if (relation == CharacterInfo.GetRelation(srcObj, target)) {
          if(CanSee(srcObj, target)) {
            return true;
          }
        }
      }
      return false;
    }
    private static bool CanSee(CharacterInfo src, CharacterInfo target)
    {
      int srcCampId = src.GetCampId();
      int targetCampId = target.GetCampId();
      if (srcCampId == targetCampId)
        return true;
      else if (srcCampId == (int)CampIdEnum.Hostile || targetCampId == (int)CampIdEnum.Hostile) {
        return CharacterInfo.CanSee(src, target);
      } else {
        bool isBlue = (targetCampId == (int)CampIdEnum.Blue);
        if (isBlue && target.CurRedCanSeeMe || !isBlue && target.CurBlueCanSeeMe)
          return true;
        else
          return false;
      }
    }

    public static bool GetWalkablePosition(CharacterInfo target, CharacterInfo src, ref Vector3 pos)
    {
      Vector3 srcPos = src.GetMovementStateInfo().GetPosition3D();
      Vector3 targetPos = target.GetMovementStateInfo().GetPosition3D();
      ICellMapView view = target.SpatialSystem.GetCellMapView(src.AvoidanceRadius);
      return GetWalkablePosition(view, targetPos, srcPos, ref pos);
    }
    public static bool GetWalkablePosition(ICellMapView view, Vector3 targetPos, Vector3 srcPos, ref Vector3 pos)
    {
      bool ret=false;
      const int c_MaxCheckCells = 3;
      int row = 0;
      int col = 0;
      view.GetCell(targetPos, out row, out col);
      float radian = Geometry.GetYAngle(new Vector2(targetPos.X, targetPos.Z), new Vector2(srcPos.X, srcPos.Z));
      if (radian >= Math.PI / 4 && radian < Math.PI * 3 / 4) {//右边
        for (int ci = 1; ci <= c_MaxCheckCells; ++ci) {
          for (int ri = 0; ri <= c_MaxCheckCells; ++ri) {
            int row_ = row + ri;
            int col_ = col + ci;
            if (view.IsCellValid(row_, col_)) {
              if (view.CanPass(row_,col_)) {
                pos = view.GetCellCenter(row_, col_);
                ret = true;
                goto exit;
              }
            }
            if (ri > 0) {
              row_ = row - ri;
              if (view.IsCellValid(row_, col_)) {
                if (view.CanPass(row_, col_)) {
                  pos = view.GetCellCenter(row_, col_);
                  ret = true;
                  goto exit;
                }
              }
            }
          }
        }
      } else if (radian >= Math.PI * 3 / 4 && radian < Math.PI * 5 / 4) {//上边
        for (int ri = 1; ri <= c_MaxCheckCells; ++ri) {
          for (int ci = 0; ci <= c_MaxCheckCells; ++ci) {
            int row_ = row - ri;
            int col_ = col + ci;
            if (view.IsCellValid(row_, col_)) {
              if (view.CanPass(row_, col_)) {
                pos = view.GetCellCenter(row_, col_);
                ret = true;
                goto exit;
              }
            }
            if (ci > 0) {
              col_ = col - ci;
              if (view.IsCellValid(row_, col_)) {
                if (view.CanPass(row_, col_)) {
                  pos = view.GetCellCenter(row_, col_);
                  ret = true;
                  goto exit;
                }
              }
            }
          }
        }
      } else if (radian >= Math.PI * 5 / 4 && radian < Math.PI * 7 / 4) {//左边
        for (int ci = 1; ci <= c_MaxCheckCells; ++ci) {
          for (int ri = 0; ri <= c_MaxCheckCells; ++ri) {
            int row_ = row + ri;
            int col_ = col - ci;
            if (view.IsCellValid(row_, col_)) {
              if (view.CanPass(row_, col_)) {
                pos = view.GetCellCenter(row_, col_);
                ret = true;
                goto exit;
              }
            }
            if (ri > 0) {
              row_ = row - ri;
              if (view.IsCellValid(row_, col_)) {
                if (view.CanPass(row_, col_)) {
                  pos = view.GetCellCenter(row_, col_);
                  ret = true;
                  goto exit;
                }
              }
            }
          }
        }
      } else {//下边
        for (int ri = 1; ri <= c_MaxCheckCells; ++ri) {
          for (int ci = 0; ci <= c_MaxCheckCells; ++ci) {
            int row_ = row + ri;
            int col_ = col + ci;
            if (view.IsCellValid(row_, col_)) {
              if (view.CanPass(row_, col_)) {
                pos = view.GetCellCenter(row_, col_);
                ret = true;
                goto exit;
              }
            }
            if (ci > 0) {
              col_ = col - ci;
              if (view.IsCellValid(row_, col_)) {
                if (view.CanPass(row_, col_)) {
                  pos = view.GetCellCenter(row_, col_);
                  ret = true;
                  goto exit;
                }
              }
            }
          }
        }
      }
    exit:
      return ret;
    }
    public static bool IsWalkable(ICellMapView view, Vector3 srcPos, Vector3 dir)
    {
      int row = 0;
      int col = 0;
      view.GetCell(srcPos, out row, out col);
      float r = view.RadiusLength * 2;
      Vector3 targetPos = view.GetCellCenter(row, col);
      dir.Normalize();
      targetPos.X += r * dir.X;
      targetPos.Z += r * dir.Z;
      int oRow = 0;
      int oCol = 0;
      view.GetCell(targetPos, out oRow, out oCol);
      bool ret = true;
      if (oRow != row || oCol != col) {
        ret = view.CanPass(oRow, oCol);
      }
      return ret;
    }

    public static void DoMoveCommandState(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime, AbstractNpcStateLogic logic)
    {
      //执行状态处理
      AiData_ForMoveCommand data = GetAiDataForMoveCommand(npc);
      if (null == data) return;

      if (!data.IsFinish) {
        if (WayPointArrived(npc, data)) {
          Vector3 targetPos = new Vector3();
          MoveToNext(npc, data, ref targetPos);
          if (!data.IsFinish) {
            logic.NotifyNpcMove(npc);
          }
        }
      }

      //判断是否状态结束并执行相应处理
      if (data.IsFinish) {
        logic.NpcSendStoryMessage(npc, "npcarrived:"+npc.GetUnitId(), npc.GetId());
        logic.NpcSendStoryMessage(npc, "objarrived", npc.GetId());
        npc.GetMovementStateInfo().IsMoving = false;
        logic.ChangeToState(npc, (int)AiStateId.Idle);
      }
    }
    private static AiData_ForMoveCommand GetAiDataForMoveCommand(NpcInfo npc)
    {
      AiData_ForMoveCommand data = npc.GetAiStateInfo().AiDatas.GetData<AiData_ForMoveCommand>();
      return data;
    }
    public static void DoMoveCommandState(UserInfo user, AiCommandDispatcher aiCmdDispatcher, long deltaTime, AbstractUserStateLogic logic)
    {
      //执行状态处理
      AiData_ForMoveCommand data = GetAiDataForMoveCommand(user);
      if (null == data) return;

      if (!data.IsFinish) {
        if (WayPointArrived(user, data)) {
          Vector3 targetPos = new Vector3();
          MoveToNext(user, data, ref targetPos);
          if (!data.IsFinish) {
            logic.NotifyUserMove(user);
          }
        }
      }

      //判断是否状态结束并执行相应处理
      if (data.IsFinish) {
        if (GlobalVariables.Instance.IsClient) {
          logic.UserSendStoryMessage(user, "playerselfarrived", user.GetId());
        }
        logic.UserSendStoryMessage(user, "objarrived", user.GetId());
        user.GetMovementStateInfo().IsMoving = false;
        logic.ChangeToState(user, (int)AiStateId.Idle);
      }
    }
    private static AiData_ForMoveCommand GetAiDataForMoveCommand(UserInfo user)
    {
      AiData_ForMoveCommand data = user.GetAiStateInfo().AiDatas.GetData<AiData_ForMoveCommand>();
      return data;
    }
    private static void MoveToNext(CharacterInfo charObj, AiData_ForMoveCommand data, ref Vector3 targetPos)
    {
      if (++data.Index >= data.WayPoints.Count)
      {
        data.IsFinish = true;
        return;
      }

      var move_info = charObj.GetMovementStateInfo();
      Vector3 from = move_info.GetPosition3D();
      targetPos = data.WayPoints[data.Index];
      float move_dir = MoveDirection(from, targetPos);

      float now = TimeUtility.GetServerMilliseconds();
      float distance = Geometry.Distance(from, targetPos);
      float speed = charObj.GetActualProperty().MoveSpeed;
      data.EstimateFinishTime = now + 1000 * (distance / speed);
      //LogSystem.Info("ai_move [{0}]: now({1}), from({2}), to({3}), distance({4}), speed({5}), move_time({6}), estimate({7})",
      //       npc.GetId(), now, from.ToString(), to.ToString(), distance, speed, 1000 * (distance / speed), data.EstimateFinishTime);

      move_info.IsMoving = true;
      move_info.SetMoveDir(move_dir);
      move_info.SetFaceDir(move_dir);
    }
    private static bool WayPointArrived(CharacterInfo charObj, AiData_ForMoveCommand data)
    {
      if (TimeUtility.GetServerMilliseconds() >= data.EstimateFinishTime)
      { 
        var move_info = charObj.GetMovementStateInfo();
        Vector3 to = data.WayPoints[data.Index];
        Vector3 now = move_info.GetPosition3D();
        float distance = Geometry.Distance(now, to);
        //LogSystem.Info("ai_move [{0}]: closest distance({1}) ", npc.GetId(), distance);
        return true;
      }
      return false;
    }
    private static float MoveDirection(Vector3 from, Vector3 to)
    {
      return (float)Math.Atan2(to.X - from.X, to.Z - from.Z);    
    }

    public static void InitPatrolData(NpcInfo npc, AbstractNpcStateLogic logic) {
      AiData_ForPatrol data = new AiData_ForPatrol();
      data.IsLoopPatrol = true;
      List<ScriptRuntime.Vector3> path = new List<ScriptRuntime.Vector3>();
      NpcAiStateInfo info = npc.GetAiStateInfo();
      path = Converter.ConvertVector3DList(info.AiParam[1]);
      path.Add(npc.GetAiStateInfo().HomePos);
      data.PatrolPath.SetPathPoints(npc.GetAiStateInfo().HomePos, path);
      npc.GetAiStateInfo().AiDatas.AddData<AiData_ForPatrol>(data);
    }
    public static int GetNpcSkillId(NpcInfo npc, float dis) {
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
    public static AiActionInfo GetFitAiAction(NpcInfo npc, CharacterInfo target)
    {
      NpcAiStateInfo info = npc.GetAiStateInfo();
      if (null != info) {
        float dis = ScriptRuntime.Vector3.Distance(npc.GetMovementStateInfo().GetPosition3D(), target.GetMovementStateInfo().GetPosition3D());
        float self_hp = npc.Hp;
        float target_hp = target.Hp;
        List<AiActionInfo> fit_Actions = new List<AiActionInfo>();
        foreach (AiActionInfo action_info in  info.ActionInfos) {
          if (action_info.IsSatisfy(dis, self_hp, target_hp)) {
            fit_Actions.Add(action_info);
          }
        }
        if (fit_Actions.Count > 0) {
          return fit_Actions[0];
        }
      }
      return null;
    }

    public static void ExecAction(NpcInfo npc, CharacterInfo target, long intervalTime, AbstractNpcStateLogic logic)
    {
      NpcAiStateInfo info = npc.GetAiStateInfo();
      ScriptRuntime.Vector3 srcPos = npc.GetMovementStateInfo().GetPosition3D();
      ScriptRuntime.Vector3 targetPos = target.GetMovementStateInfo().GetPosition3D();
      AiPathData pathData = new AiPathData();
      float powDist = Geometry.DistanceSquare(srcPos, targetPos);
      if (null != info) {
        if (null != info.CurActionInfo && !info.CurActionInfo.IsFinish()) {
          switch (info.CurActionInfo.Config.AiActionType) {
            case (int)AbstractNpcStateLogic.AiAction.STAND:
              logic.TrySeeTarget(npc, target);
              break;
            case (int)AbstractNpcStateLogic.AiAction.WALK:
              logic.NotifyNpcWalk(npc);
              PathToTargetWithoutObstacle(npc, pathData, targetPos, intervalTime, true, logic);
              break;
            case (int)AbstractNpcStateLogic.AiAction.SKILL:
              int skillId = AiLogicUtility.GetNpcSkillId(npc, (float)Math.Sqrt(powDist));
              if (-1 == skillId) {
                logic.NotifyNpcRun(npc);
                PathToTargetWithoutObstacle(npc, pathData, targetPos, intervalTime, true, logic);
              } else {
                npc.GetMovementStateInfo().IsMoving = false;
                logic.NotifyNpcMove(npc);
                if (logic.TryCastSkill(npc, skillId, target)) {
                  info.CurActionInfo = null;
                }
              }
              break;
            case (int)AbstractNpcStateLogic.AiAction.TAUNT:
              logic.TrySeeTarget(npc, target);
              npc.IsTaunt = true;
              break;
          }
        } else if (null != info.CurActionInfo) {
          npc.GetMovementStateInfo().IsMoving = false;
          logic.NotifyNpcMove(npc);
          npc.IsTaunt = false;
          info.CurActionInfo = null;
        }
      }
    }
    // 通用Idle处理，寻路、休闲、遇敌。
    public static void CommonIdleHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime, AbstractNpcStateLogic logic)
    {
      if (npc.GetSkillStateInfo().IsSkillActivated()) return;
      if (npc.IsDead()) return;
      if (npc.GetSkillStateInfo().IsImpactControl()) {
        // 被动进入战斗
      }
      NpcAiStateInfo info = npc.GetAiStateInfo();
      info.Time += deltaTime;
      if (info.Time > 100) {
        info.Time = 0;
        if (null != logic.GetAiPatrolData(npc)) {
          npc.GetMovementStateInfo().IsMoving = false;
          logic.ChangeToState(npc, (int)AiStateId.Patrol);
        } else {
          CharacterInfo target = AiLogicUtility.GetNearstTargetHelper(npc, CharacterRelation.RELATION_ENEMY, AiTargetType.ALL);
          if (null != target) {
            info.Target = target.GetId();
            logic.NotifyNpcTargetChange(npc);
            npc.GetMovementStateInfo().IsMoving = false;
            logic.NotifyNpcMove(npc);
            info.Time = 0;
            logic.ChangeToState(npc, (int)AiStateId.Pursuit);
          }
        }
      }
    }
    public static void CommonIdleHandler(UserInfo user, AiCommandDispatcher aiCmdDispatcher, long deltaTime, AbstractUserStateLogic logic)
    {
      if (user.GetSkillStateInfo().IsSkillActivated()) return;
      if (user.IsDead()) return;
      if (user.GetSkillStateInfo().IsImpactControl()) {
        // 被动进入战斗
      }
      UserAiStateInfo info = user.GetAiStateInfo();
      info.Time += deltaTime;
      if (info.Time > 100) {
        info.Time = 0;
        CharacterInfo target = AiLogicUtility.GetNearstTargetHelper(user, CharacterRelation.RELATION_ENEMY, AiTargetType.USER);
        if (null != target) {
          info.Target = target.GetId();
          user.GetMovementStateInfo().IsMoving = false;
          logic.NotifyUserMove(user);
          info.Time = 0;
          logic.ChangeToState(user, (int)AiStateId.Pursuit);
        }
      }
    }

    // 通用寻路处理
    public static void CommonPatrolHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime, AbstractNpcStateLogic logic)
    {
      if (npc.IsDead() || npc.GetSkillStateInfo().IsImpactControl() || npc.GetSkillStateInfo().IsSkillActivated()) {
        return;
      }
      NpcAiStateInfo info = npc.GetAiStateInfo();
      info.Time += deltaTime;
      if (info.Time > 100) {
        long intervalTime = info.Time;
        info.Time = 0;
      CharacterInfo target = AiLogicUtility.GetNearstTargetHelper(npc, CharacterRelation.RELATION_ENEMY, AiTargetType.USER);
        if (null != target) {
          info.Target = target.GetId();
          logic.NotifyNpcTargetChange(npc);
          npc.GetMovementStateInfo().IsMoving = false;
          logic.NotifyNpcMove(npc);
          info.Time = 0;
          AiData_ForPatrol data = logic.GetAiPatrolData(npc);
          if(null!=data)
            data.FoundPath.Clear();
          logic.NpcSendStoryMessage(npc, string.Format("npcpatrolexit:{0}", npc.GetUnitId()), npc.GetId());
          logic.ChangeToState(npc, (int)AiStateId.Pursuit);
        } else {
          AiData_ForPatrol data = logic.GetAiPatrolData(npc);
          if (null != data) {
            ScriptRuntime.Vector3 srcPos = npc.GetMovementStateInfo().GetPosition3D();
            if (data.PatrolPath.HavePathPoint && !data.PatrolPath.IsReached(srcPos)) {
              logic.NotifyNpcWalk(npc);
              PathToTargetWithoutObstacle(npc, data.FoundPath, data.PatrolPath.CurPathPoint, intervalTime, true, logic);
            } else {
              data.PatrolPath.UseNextPathPoint();
              data.FoundPath.Clear();
              if (!data.PatrolPath.HavePathPoint) {
                if (data.IsLoopPatrol) {
                  data.PatrolPath.Restart();
                } else {
                  info.Time = 0;
                  logic.ChangeToState(npc, (int)AiStateId.Idle);
                }
              }
            }
            info.HomePos = npc.GetMovementStateInfo().GetPosition3D();
          } else {
            info.Time = 0;
            logic.ChangeToState(npc, (int)AiStateId.Idle);
          }
        }
      }
    }
    public static void DoPatrolCommandState(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime, AbstractNpcStateLogic logic)
    {
      if (npc.IsDead()) {
        logic.NpcSendStoryMessage(npc, string.Format("npcpatrolexit:{0}", npc.GetUnitId()), npc.GetId());
        logic.ChangeToState(npc, (int)AiStateId.Idle);
        return;
      }
      NpcAiStateInfo info = npc.GetAiStateInfo();
      info.Time += deltaTime;
      if (info.Time > 100) {
        long intervalTime = info.Time;
        info.Time = 0;
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
          logic.NotifyNpcTargetChange(npc);
          npc.GetMovementStateInfo().IsMoving = false;
          logic.NotifyNpcMove(npc);
          info.Time = 0;
          AiData_ForPatrolCommand data = GetAiDataForPatrolCommand(npc);
          if(null!=data)
            data.FoundPath.Clear();
          logic.NpcSendStoryMessage(npc, string.Format("npcpatrolexit:{0}", npc.GetUnitId()), npc.GetId());
          logic.ChangeToState(npc, (int)AiStateId.Pursuit);
        } else {
          AiData_ForPatrolCommand data = GetAiDataForPatrolCommand(npc);
          if (null != data) {
            ScriptRuntime.Vector3 srcPos = npc.GetMovementStateInfo().GetPosition3D();
            if (data.PatrolPath.HavePathPoint && !data.PatrolPath.IsReached(srcPos)) {
              PathToTargetWithoutObstacle(npc, data.FoundPath, data.PatrolPath.CurPathPoint, intervalTime, true, logic);
            } else {
              data.PatrolPath.UseNextPathPoint();
              data.FoundPath.Clear();
              if (!data.PatrolPath.HavePathPoint) {
                if (data.IsLoopPatrol) {
                  logic.NpcSendStoryMessage(npc, string.Format("npcpatrolrestart:{0}", npc.GetUnitId()), npc.GetId());
                  data.PatrolPath.Restart();
                } else {
                  logic.NpcSendStoryMessage(npc, string.Format("npcpatrolfinish:{0}", npc.GetUnitId()), npc.GetId());
                  info.Time = 0;
                  npc.GetMovementStateInfo().IsMoving = false;
                  logic.ChangeToState(npc, (int)AiStateId.Idle);
                }
              }
            }
            info.HomePos = npc.GetMovementStateInfo().GetPosition3D();
          } else {
            info.Time = 0;
            logic.ChangeToState(npc, (int)AiStateId.Idle);
          }
        }
      }
    }
    public static void PathToTargetWithoutObstacle(UserInfo user, AiPathData data, Vector3 pathTargetPos, long deltaTime, bool faceIsMoveFir, AbstractUserStateLogic logic) {
      UserAiStateInfo info = user.GetAiStateInfo();
      Vector3 srcPos = user.GetMovementStateInfo().GetPosition3D();
      if (null != data) {
        data.Clear();
        data.UpdateTime += deltaTime;
        Vector3 targetPos = pathTargetPos;
        bool canGo = true;
        if (!user.SpatialSystem.GetCellMapView(user.AvoidanceRadius).CanPass(targetPos)) {
          if (!AiLogicUtility.GetWalkablePosition(user.SpatialSystem.GetCellMapView(user.AvoidanceRadius), targetPos, srcPos, ref targetPos))
            if (!AiLogicUtility.GetForwardTargetPos(user, targetPos, 2.0f, ref targetPos)) {
              canGo = false;
            }
        }
        if (canGo) {
          List<Vector3> posList = null;
          bool canPass = user.SpatialSystem.CanPass(user.SpaceObject, targetPos);
          if (canPass) {
            posList = new List<Vector3>();
            posList.Add(srcPos);
            posList.Add(targetPos);
          } else {
            long stTime = TimeUtility.GetElapsedTimeUs();
            posList = user.SpatialSystem.FindPath(srcPos, targetPos, user.AvoidanceRadius);
            long endTime = TimeUtility.GetElapsedTimeUs();
            long calcTime = endTime - stTime;
            if (calcTime > 10000) {
              LogSystem.Warn("*** pve FindPath consume {0} us,npc:{1} from:{2} to:{3} radius:{4} pos:{5}", calcTime, user.GetId(), srcPos.ToString(), targetPos.ToString(), user.AvoidanceRadius, user.GetMovementStateInfo().GetPosition3D().ToString());
            }
          }
          if (posList.Count >= 2) {
            data.SetPathPoints(posList[0], posList, 1);
          } else {
            user.GetMovementStateInfo().IsMoving = false;
            logic.NotifyUserMove(user);
            data.IsUsingAvoidanceVelocity = false;
          }
          bool havePathPoint = data.HavePathPoint;
          if (havePathPoint) {//沿路点列表移动的逻辑
            targetPos = data.CurPathPoint;
            if (!data.IsReached(srcPos)) {//向指定路点移动（避让移动过程）
              float angle = Geometry.GetYAngle(new Vector2(srcPos.X, srcPos.Z), new Vector2(targetPos.X, targetPos.Z));
              Vector3 prefVelocity = (float)user.GetActualProperty().MoveSpeed * new Vector3((float)Math.Sin(angle), 0, (float)Math.Cos(angle));
              Vector3 v = new Vector3(targetPos.X - srcPos.X, 0, targetPos.Z - srcPos.Z);
              v.Normalize();
              Vector3 velocity = user.SpaceObject.GetVelocity();
              float speedSquare = (float)user.GetActualProperty().MoveSpeed * (float)user.GetActualProperty().MoveSpeed;
              long stTime = TimeUtility.GetElapsedTimeUs();
              Vector3 newVelocity = user.SpatialSystem.ComputeVelocity(user.SpaceObject, v, (float)deltaTime / 1000, (float)user.GetActualProperty().MoveSpeed, (float)user.GetRadius(), data.IsUsingAvoidanceVelocity);
              long endTime = TimeUtility.GetElapsedTimeUs();
              long calcTime = endTime - stTime;
              if (calcTime > 10000) {
                LogSystem.Warn("*** pve ComputeVelocity consume {0} us,npc:{1} velocity:{2} newVelocity:{3} deltaTime:{4} speed:{5} pos:{6}", calcTime, user.GetId(), velocity.ToString(), newVelocity.ToString(), deltaTime, user.GetActualProperty().MoveSpeed, user.GetMovementStateInfo().GetPosition3D().ToString());
              }
              if (data.UpdateTime > 100) {
                data.UpdateTime = 0;
                float newAngle = Geometry.GetYAngle(new Vector2(0, 0), new Vector2(newVelocity.X, newVelocity.Z));
                user.GetMovementStateInfo().SetMoveDir(newAngle);
                user.GetMovementStateInfo().SetFaceDir(newAngle);
                user.GetMovementStateInfo().SetWantFaceDir(newAngle);
                if (faceIsMoveFir)
                  //logic.NotifyUserFace(user, newAngle);
                  logic.NotifyUserFace(user);
                newVelocity.Normalize();
                user.GetMovementStateInfo().TargetPosition = srcPos + newVelocity * Geometry.Distance(srcPos, targetPos);
                user.GetMovementStateInfo().IsMoving = true;
                logic.NotifyUserMove(user);
              } else {
                data.UpdateTime += deltaTime;
              }
            } else {//改变路点或结束沿路点移动
              data.UseNextPathPoint();
              if (data.HavePathPoint) {
                targetPos = data.CurPathPoint;
                user.GetMovementStateInfo().TargetPosition = targetPos;
                float angle = Geometry.GetYAngle(new Vector2(srcPos.X, srcPos.Z), new Vector2(targetPos.X, targetPos.Z));
                user.GetMovementStateInfo().SetFaceDir(angle);
                user.GetMovementStateInfo().SetMoveDir(angle);
                user.GetMovementStateInfo().SetWantFaceDir(angle);
                if (faceIsMoveFir)
                  logic.NotifyUserFace(user);
                user.GetMovementStateInfo().IsMoving = true;
                logic.NotifyUserMove(user);
                data.IsUsingAvoidanceVelocity = false;
              } else {
                user.GetMovementStateInfo().IsMoving = false;
                data.Clear();
              }
            }
          }
        } else {
            user.GetMovementStateInfo().IsMoving = false;
            logic.NotifyUserMove(user);
        }
      }
    }
    public static void PathToTargetWithoutObstacle(NpcInfo npc, AiPathData data, Vector3 pathTargetPos, long deltaTime, bool faceIsMoveFir, AbstractNpcStateLogic logic) {
      NpcAiStateInfo info = npc.GetAiStateInfo();
      Vector3 srcPos = npc.GetMovementStateInfo().GetPosition3D();
      if (null != data) {
        data.Clear();
        data.UpdateTime += deltaTime;
        Vector3 targetPos = pathTargetPos;
        bool canGo = true;
        if (!npc.SpatialSystem.GetCellMapView(npc.AvoidanceRadius).CanPass(targetPos)) {
          if (!AiLogicUtility.GetWalkablePosition(npc.SpatialSystem.GetCellMapView(npc.AvoidanceRadius), targetPos, srcPos, ref targetPos))
            if (!AiLogicUtility.GetForwardTargetPos(npc, targetPos, 2.0f, ref targetPos)) {
              canGo = false;
            }
        }
        if (canGo) {
          List<Vector3> posList = null;
          bool canPass = npc.SpatialSystem.CanPass(npc.SpaceObject, targetPos);
          if (canPass) {
            posList = new List<Vector3>();
            posList.Add(srcPos);
            posList.Add(targetPos);
          } else {
            long stTime = TimeUtility.GetElapsedTimeUs();
            posList = npc.SpatialSystem.FindPath(srcPos, targetPos, npc.AvoidanceRadius);
            long endTime = TimeUtility.GetElapsedTimeUs();
            long calcTime = endTime - stTime;
            if (calcTime > 10000) {
              LogSystem.Warn("*** pve FindPath consume {0} us,npc:{1} from:{2} to:{3} radius:{4} pos:{5}", calcTime, npc.GetId(), srcPos.ToString(), targetPos.ToString(), npc.AvoidanceRadius, npc.GetMovementStateInfo().GetPosition3D().ToString());
            }
          }
          if (posList.Count >= 2) {
            data.SetPathPoints(posList[0], posList, 1);
          } else {
            npc.GetMovementStateInfo().IsMoving = false;
            logic.NotifyNpcMove(npc);
            data.IsUsingAvoidanceVelocity = false;
          }
          bool havePathPoint = data.HavePathPoint;
          if (havePathPoint) {//沿路点列表移动的逻辑
            targetPos = data.CurPathPoint;
            if (!data.IsReached(srcPos)) {//向指定路点移动（避让移动过程）
              float angle = Geometry.GetYAngle(new Vector2(srcPos.X, srcPos.Z), new Vector2(targetPos.X, targetPos.Z));
              Vector3 prefVelocity = (float)npc.GetActualProperty().MoveSpeed * new Vector3((float)Math.Sin(angle), 0, (float)Math.Cos(angle));
              Vector3 v = new Vector3(targetPos.X - srcPos.X, 0, targetPos.Z - srcPos.Z);
              v.Normalize();
              Vector3 velocity = npc.SpaceObject.GetVelocity();
              float speedSquare = (float)npc.GetActualProperty().MoveSpeed * (float)npc.GetActualProperty().MoveSpeed;
              long stTime = TimeUtility.GetElapsedTimeUs();
              Vector3 newVelocity = npc.SpatialSystem.ComputeVelocity(npc.SpaceObject, v, (float)deltaTime / 1000, (float)npc.GetActualProperty().MoveSpeed, (float)npc.GetRadius(), data.IsUsingAvoidanceVelocity);
              long endTime = TimeUtility.GetElapsedTimeUs();
              long calcTime = endTime - stTime;
              if (calcTime > 10000) {
                LogSystem.Warn("*** pve ComputeVelocity consume {0} us,npc:{1} velocity:{2} newVelocity:{3} deltaTime:{4} speed:{5} pos:{6}", calcTime, npc.GetId(), velocity.ToString(), newVelocity.ToString(), deltaTime, npc.GetActualProperty().MoveSpeed, npc.GetMovementStateInfo().GetPosition3D().ToString());
              }
              if (data.UpdateTime > 100) {
                data.UpdateTime = 0;
                float newAngle = Geometry.GetYAngle(new Vector2(0, 0), new Vector2(newVelocity.X, newVelocity.Z));
                npc.GetMovementStateInfo().SetMoveDir(newAngle);
                if (faceIsMoveFir)
                  logic.NotifyNpcFace(npc, newAngle);
                newVelocity.Normalize();
                npc.GetMovementStateInfo().TargetPosition = srcPos + newVelocity * Geometry.Distance(srcPos, targetPos);
                npc.GetMovementStateInfo().IsMoving = true;
                logic.NotifyNpcMove(npc);
              } else {
                data.UpdateTime += deltaTime;
              }
            } else {//改变路点或结束沿路点移动
              data.UseNextPathPoint();
              if (data.HavePathPoint) {
                targetPos = data.CurPathPoint;
                npc.GetMovementStateInfo().TargetPosition = targetPos;
                float angle = Geometry.GetYAngle(new Vector2(srcPos.X, srcPos.Z), new Vector2(targetPos.X, targetPos.Z));
                npc.GetMovementStateInfo().SetMoveDir(angle);
                if (faceIsMoveFir)
                  logic.NotifyNpcFace(npc, angle);
                npc.GetMovementStateInfo().IsMoving = true;
                logic.NotifyNpcMove(npc);
                data.IsUsingAvoidanceVelocity = false;
              } else {
                npc.GetMovementStateInfo().IsMoving = false;
                data.Clear();
              }
            }
          }
        } else {
            npc.GetMovementStateInfo().IsMoving = false;
            logic.NotifyNpcMove(npc);
        }
      }
    }
    public static void PathToTarget(NpcInfo npc, AiPathData data, Vector3 pathTargetPos, long deltaTime, bool faceIsMoveFir, AbstractNpcStateLogic logic)
    {
      NpcAiStateInfo info = npc.GetAiStateInfo();
      if (null != data) {
        data.UpdateTime += deltaTime;
        ScriptRuntime.Vector3 srcPos = npc.GetMovementStateInfo().GetPosition3D();
        float dir = npc.GetMovementStateInfo().GetMoveDir();
        bool findObstacle = false;
        bool havePathPoint = data.HavePathPoint;
        if (havePathPoint) {//沿路点列表移动的逻辑
          Vector3 targetPos = data.CurPathPoint;
          if (!data.IsReached(srcPos)) {//向指定路点移动（避让移动过程）
            float angle = Geometry.GetYAngle(new Vector2(srcPos.X, srcPos.Z), new Vector2(targetPos.X, targetPos.Z));
            Vector3 prefVelocity = (float)npc.GetActualProperty().MoveSpeed*new Vector3((float)Math.Sin(angle),0,(float)Math.Cos(angle));
            Vector3 v = new Vector3(targetPos.X-srcPos.X,0,targetPos.Z-srcPos.Z);
            v.Normalize();
            Vector3 velocity = npc.SpaceObject.GetVelocity();
            float speedSquare = (float)npc.GetActualProperty().MoveSpeed * (float)npc.GetActualProperty().MoveSpeed;
            long stTime = TimeUtility.GetElapsedTimeUs();
            Vector3 newVelocity = npc.SpatialSystem.ComputeVelocity(npc.SpaceObject, v, (float)deltaTime / 1000, (float)npc.GetActualProperty().MoveSpeed, (float)npc.GetRadius(), data.IsUsingAvoidanceVelocity);
            findObstacle = !AiLogicUtility.IsWalkable(npc.SpatialSystem.GetCellMapView(npc.AvoidanceRadius), srcPos, newVelocity);
            long endTime = TimeUtility.GetElapsedTimeUs();
            long calcTime = endTime - stTime;
            if (calcTime > 10000) {
              LogSystem.Warn("*** pve ComputeVelocity consume {0} us,npc:{1} velocity:{2} newVelocity:{3} deltaTime:{4} speed:{5} pos:{6}", calcTime, npc.GetId(), velocity.ToString(), newVelocity.ToString(), deltaTime, npc.GetActualProperty().MoveSpeed, npc.GetMovementStateInfo().GetPosition3D().ToString());
            }
            if (Geometry.DistanceSquare(newVelocity, new Vector3()) <= speedSquare*0.25f) {//避让计算的移动速度变小（说明没有更好的保持原速的选择，停止）
              npc.GetMovementStateInfo().IsMoving=false;
              logic.NotifyNpcMove(npc);
              data.IsUsingAvoidanceVelocity = false;
            } else if(findObstacle){//当前移动方向遇到阻挡，停止移动，触发寻路
              npc.GetMovementStateInfo().IsMoving = false;
              logic.NotifyNpcMove(npc);
              data.IsUsingAvoidanceVelocity = false;
            } else if (data.UpdateTime > 1000) {//避让速度改变每秒一次（表现上更像人类一些）
              data.UpdateTime = 0;

              float newAngle = Geometry.GetYAngle(new Vector2(0, 0), new Vector2(newVelocity.X, newVelocity.Z));
              npc.GetMovementStateInfo().SetMoveDir(newAngle);
              if (faceIsMoveFir)
                npc.GetMovementStateInfo().SetFaceDir(newAngle);
              newVelocity.Normalize();
              npc.GetMovementStateInfo().TargetPosition = srcPos + newVelocity * Geometry.Distance(srcPos, targetPos);
              npc.GetMovementStateInfo().IsMoving = true;
              logic.NotifyNpcMove(npc);
              data.IsUsingAvoidanceVelocity = true;
            } else if (Geometry.DistanceSquare(velocity, newVelocity) > 9.0f) {//没有到速度改变周期，但避让方向需要大幅调整
              if (Geometry.Dot(newVelocity, prefVelocity) > 0) {//如果是调整为与目标方向一致，则进行调整
                float newAngle = Geometry.GetYAngle(new Vector2(0, 0), new Vector2(newVelocity.X, newVelocity.Z));
                npc.GetMovementStateInfo().SetMoveDir(newAngle);
                if (faceIsMoveFir)
                  npc.GetMovementStateInfo().SetFaceDir(newAngle);
                newVelocity.Normalize();
                npc.GetMovementStateInfo().TargetPosition = srcPos + newVelocity * Geometry.Distance(srcPos, targetPos);
                npc.GetMovementStateInfo().IsMoving = true;
                logic.NotifyNpcMove(npc);
                data.IsUsingAvoidanceVelocity = true;
              } else {//如果调整为远离目标方向，则停止
                npc.GetMovementStateInfo().IsMoving = false;
                logic.NotifyNpcMove(npc);
                data.IsUsingAvoidanceVelocity = false;
              }
            } else if (!npc.GetMovementStateInfo().IsMoving && velocity.LengthSquared() > speedSquare * 0.25f) {//正常移动过程，继续移动
              velocity.Normalize();
              npc.GetMovementStateInfo().TargetPosition = srcPos + velocity * Geometry.Distance(srcPos, targetPos);
              npc.GetMovementStateInfo().IsMoving = true;
              logic.NotifyNpcMove(npc);
              data.IsUsingAvoidanceVelocity = false;
            } 
          } else {//改变路点或结束沿路点移动
            data.UseNextPathPoint();
            if (data.HavePathPoint) {
              targetPos = data.CurPathPoint;
              npc.GetMovementStateInfo().TargetPosition = targetPos;
              float angle = Geometry.GetYAngle(new Vector2(srcPos.X, srcPos.Z), new Vector2(targetPos.X, targetPos.Z));
              npc.GetMovementStateInfo().SetMoveDir(angle);
              if (faceIsMoveFir)
                npc.GetMovementStateInfo().SetFaceDir(angle);
              npc.GetMovementStateInfo().IsMoving = true;
              logic.NotifyNpcMove(npc);
              data.IsUsingAvoidanceVelocity = false;
            } else {
              data.Clear();
            }
          }
        }
        if (!havePathPoint || findObstacle) {//获得路点过程（寻路）
          data.Clear();
          Vector3 targetPos = pathTargetPos;
          bool canGo = true;
          if (!npc.SpatialSystem.GetCellMapView(npc.AvoidanceRadius).CanPass(targetPos)) {
            if (!AiLogicUtility.GetWalkablePosition(npc.SpatialSystem.GetCellMapView(npc.AvoidanceRadius), targetPos, srcPos, ref targetPos))
              canGo = false;
          }
          if (canGo) {
            List<Vector3> posList = null;
            bool canPass = npc.SpatialSystem.CanPass(npc.SpaceObject, targetPos);
            if (canPass) {
              posList = new List<Vector3>();
              posList.Add(srcPos);
              posList.Add(targetPos);
            } else {
              long stTime = TimeUtility.GetElapsedTimeUs();
              posList = npc.SpatialSystem.FindPath(srcPos, targetPos, npc.AvoidanceRadius);
              long endTime = TimeUtility.GetElapsedTimeUs();
              long calcTime = endTime - stTime;
              if (calcTime > 10000) {
                LogSystem.Warn("*** pve FindPath consume {0} us,npc:{1} from:{2} to:{3} radius:{4} pos:{5}", calcTime, npc.GetId(), srcPos.ToString(), targetPos.ToString(), npc.AvoidanceRadius, npc.GetMovementStateInfo().GetPosition3D().ToString());
              }
            }
            if (posList.Count >= 2) {
              data.SetPathPoints(posList[0], posList, 1);
              targetPos = data.CurPathPoint;
              npc.GetMovementStateInfo().TargetPosition = targetPos;
              float angle = Geometry.GetYAngle(new Vector2(srcPos.X, srcPos.Z), new Vector2(targetPos.X, targetPos.Z));
              npc.GetMovementStateInfo().SetMoveDir(angle);
              if (faceIsMoveFir)
                npc.GetMovementStateInfo().SetFaceDir(angle);
              npc.GetMovementStateInfo().IsMoving = true;
              logic.NotifyNpcMove(npc);
              data.IsUsingAvoidanceVelocity = false;
            } else {
              npc.GetMovementStateInfo().IsMoving = false;
              logic.NotifyNpcMove(npc);
              data.IsUsingAvoidanceVelocity = false;
            }
          } else {
            npc.GetMovementStateInfo().IsMoving = false;
            logic.NotifyNpcMove(npc);
            data.IsUsingAvoidanceVelocity = false;
          }
        }
      }
    }
    private static AiData_ForPatrolCommand GetAiDataForPatrolCommand(NpcInfo npc)
    {
      AiData_ForPatrolCommand data = npc.GetAiStateInfo().AiDatas.GetData<AiData_ForPatrolCommand>();
      return data;
    }

    // 追击命令
    public static void DoPursuitCommandState(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime, AbstractNpcStateLogic logic)
    {
      NpcAiStateInfo info = npc.GetAiStateInfo();
      AiData_ForPursuitCommand data = GetAiDataForPursuitCommand(npc);
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
            logic.NpcSendStoryMessage(npc, "npcpursuitfinish:" + npc.GetUnitId(), npc.GetId());
            logic.NpcSendStoryMessage(npc, "objpursuitfinish", npc.GetId());
            npc.GetMovementStateInfo().IsMoving = false;
            info.Time = 0;
            logic.NotifyNpcMove(npc);
            logic.ChangeToState(npc, (int)AiStateId.Idle);
          } else {
            info.Time += deltaTime;
            if (info.Time > 100) {
              info.Time = 0;
              PathToTargetWithoutObstacle(npc, data.FoundPath, targetPos, 100, true, logic);
            }
          }
        } else {
          logic.NpcSendStoryMessage(npc, "npcpursuitexit:" + npc.GetUnitId(), npc.GetId());
          logic.NpcSendStoryMessage(npc, "objpursuitexit", npc.GetId());
          npc.GetMovementStateInfo().IsMoving = false;
          info.Time = 0;
          logic.NotifyNpcMove(npc);
          logic.ChangeToState(npc, (int)AiStateId.Idle);
        }
      }
    }
    public static void DoPursuitCommandState(UserInfo user, AiCommandDispatcher aiCmdDispatcher, long deltaTime, AbstractUserStateLogic logic)
    {
      UserAiStateInfo info = user.GetAiStateInfo();
      AiData_ForPursuitCommand data = GetAiDataForPursuitCommand(user);
      if (null != data) {
        CharacterInfo target = AiLogicUtility.GetLivingCharacterInfoHelper(user, info.Target);
        if (null != target) {
          float dist = (float)user.GetActualProperty().AttackRange;
          float distGoHome = (float)user.GohomeRange;
          Vector3 targetPos = target.GetMovementStateInfo().GetPosition3D();
          ScriptRuntime.Vector3 srcPos = user.GetMovementStateInfo().GetPosition3D();
          float powDist = Geometry.DistanceSquare(srcPos, targetPos);
          float powDistToHome = Geometry.DistanceSquare(srcPos, info.HomePos);
          if (powDist < dist * dist) {
            if (GlobalVariables.Instance.IsClient) {
              logic.UserSendStoryMessage(user, "playerselfpursuitfinish", user.GetId());
            }
            logic.UserSendStoryMessage(user, "objpursuitfinish", user.GetId());
            user.GetMovementStateInfo().IsMoving = false;
            info.Time = 0;
            logic.NotifyUserMove(user);
            logic.ChangeToState(user, (int)AiStateId.Idle);
          } else {
            info.Time += deltaTime;
            if (info.Time > 100) {
              info.Time = 0;
              AiLogicUtility.PathToTargetWithoutObstacle(user, data.FoundPath, targetPos, 100, true, logic);
            }
          }
        } else {
          if (GlobalVariables.Instance.IsClient) {
            logic.UserSendStoryMessage(user, "playerselfpursuitexit", user.GetId());
          }
          logic.UserSendStoryMessage(user, "objpursuitexit", user.GetId());
          user.GetMovementStateInfo().IsMoving = false;
          info.Time = 0;
          logic.NotifyUserMove(user);
          logic.ChangeToState(user, (int)AiStateId.Idle);
        }
      }
    }
    private static AiData_ForPursuitCommand GetAiDataForPursuitCommand(NpcInfo npc)
    {
      AiData_ForPursuitCommand data = npc.GetAiStateInfo().AiDatas.GetData<AiData_ForPursuitCommand>();
      return data;
    }
    private static AiData_ForPursuitCommand GetAiDataForPursuitCommand(UserInfo user)
    {
      AiData_ForPursuitCommand data = user.GetAiStateInfo().AiDatas.GetData<AiData_ForPursuitCommand>();
      return data;
    }

    public static float GetWalkSpeed(NpcInfo info) {
      Data_ActionConfig ac = info.GetCurActionConfig();
      if (null != ac) {
        return ac.m_SlowStdSpeed;
      } else {
        LogSystem.Error("AiLogic_Demo_Melee can't find CurActionConfig");
      }
      return 0.0f;
    }

    public static float GetRunSpeed(NpcInfo info) {
      Data_ActionConfig ac = info.GetCurActionConfig();
      if (null != ac) {
        return ac.m_FastStdSpeed;
      } else {
        LogSystem.Error("AiLogic_Demo_Melee can't find CurActionConfig");
      }
      return 0.0f;
    }

    public static bool GetEscapeTargetPos(NpcInfo npc, CharacterInfo target, float dis, ref Vector3 escapePos) {
      Vector3 targetPos = Vector3.Zero;
      // try back
      Vector3 sourcePos = npc.GetMovementStateInfo().GetPosition3D();
      float angle = Geometry.GetYAngle(target.GetMovementStateInfo().GetPosition2D(), npc.GetMovementStateInfo().GetPosition2D());
      targetPos.X = sourcePos.X + dis * (float)Math.Sin(angle);
      targetPos.Y = sourcePos.Y;
      targetPos.Z = sourcePos.Z + dis * (float)Math.Cos(angle);
      bool isFind = GetWalkablePosition(npc.SpatialSystem.GetCellMapView(npc.AvoidanceRadius), targetPos, sourcePos, ref escapePos);
      // try left
      if (!isFind) {
        angle += 90.0f;
        targetPos.X = sourcePos.X + dis * (float)Math.Sin(angle);
        targetPos.Y = sourcePos.Y;
        targetPos.Z = sourcePos.Z + dis * (float)Math.Cos(angle);
        isFind = GetWalkablePosition(npc.SpatialSystem.GetCellMapView(npc.AvoidanceRadius), targetPos, sourcePos, ref escapePos);
      }
      // try right
      if (!isFind) {
        angle -= 90.0f;
        targetPos.X = sourcePos.X + dis * (float)Math.Sin(angle);
        targetPos.Y = sourcePos.Y;
        targetPos.Z = sourcePos.Z + dis * (float)Math.Cos(angle);
        isFind = GetWalkablePosition(npc.SpatialSystem.GetCellMapView(npc.AvoidanceRadius), targetPos, sourcePos, ref escapePos);
      }
      if (!isFind) {
        angle += 180.0f;
        targetPos.X = sourcePos.X + dis * (float)Math.Sin(angle);
        targetPos.Y = sourcePos.Y;
        targetPos.Z = sourcePos.Z + dis * (float)Math.Cos(angle);
        isFind = GetWalkablePosition(npc.SpatialSystem.GetCellMapView(npc.AvoidanceRadius), targetPos, sourcePos, ref escapePos);
      }
      return isFind;
    }
    public static bool GetForwardTargetPos(CharacterInfo character, Vector3 endPos, float dis, ref Vector3 pos) {
      Vector3 targetPos = Vector3.Zero;
      // try back
      Vector3 sourcePos = character.GetMovementStateInfo().GetPosition3D();
      float angle = Geometry.GetYAngle(character.GetMovementStateInfo().GetPosition2D(), new Vector2(endPos.X, endPos.Z));
      targetPos.X = sourcePos.X + dis * (float)Math.Sin(angle);
      targetPos.Y = sourcePos.Y;
      targetPos.Z = sourcePos.Z + dis * (float)Math.Cos(angle);
      bool isFind = GetWalkablePosition(character.SpatialSystem.GetCellMapView(character.AvoidanceRadius), targetPos, sourcePos, ref pos);
      return isFind;
    }

    public static AiActionInfo CreateAiAction(int id)
    {
      AiActionInfo result = null;
      AiActionConfig config = AiActionConfigProvider.Instance.GetDataById(id);
      if (null != config) {
        result = new AiActionInfo(config);
      } else {
        LogSystem.Warn("CreateAiAction:: can't find AiActionConfig {0}", id);
      }
      return result;
    }

    public static bool IsSkillInCD(CharacterInfo character, int skillId)
    {
      if (null != character.SkillController) {
        SkillInfo skillInfo = character.GetSkillStateInfo().GetSkillInfoById(skillId);
        if (null != skillInfo) {
          long curTime = TimeUtility.GetServerMilliseconds();
          if (!skillInfo.IsInCd(curTime / 1000.0f)) {
            return false;
          }
        }
      }
      return true;
    }

    public static bool IsSkillInRange(CharacterInfo character, int skillId, float dis)
    {
      SkillInfo skillInfo = character.GetSkillStateInfo().GetSkillInfoById(skillId);
      if (null != skillInfo) {
        if (skillInfo.ConfigData.SkillRangeMin < dis && skillInfo.ConfigData.SkillRangeMax > dis) {
          return true;
        }
      }
      return false;
    }

    public static bool CanCastSkill(CharacterInfo sender, int skillId, CharacterInfo target)
    {
      if (null != sender && null != target) {
        float dis = Geometry.Distance(sender.GetMovementStateInfo().GetPosition3D(), target.GetMovementStateInfo().GetPosition3D());
        if (IsSkillInRange(sender, skillId, dis) && !IsSkillInCD(sender, skillId)) {
          return true;
        }
      }
      return false;
    }
    public static bool IsTooCloseToCastSkill(CharacterInfo character, CharacterInfo target)
    {
      if (null != character && null != target) {
        float dis = Geometry.Distance(character.GetMovementStateInfo().GetPosition3D(), target.GetMovementStateInfo().GetPosition3D());
        foreach (SkillInfo si in character.GetSkillStateInfo().GetAllSkill()) {
          if (si.ConfigData.SkillRangeMin != -1 && si.ConfigData.SkillRangeMin < dis) {
            return false;
          }
        }
      }
      return true;
    }
  }
  class AiLogic_MovableNpc_Client : AbstractNpcStateLogic
  {
    protected override void OnInitStateHandlers()
    {
      SetStateHandler((int)AiStateId.Idle, this.IdleHandler);
      SetStateHandler((int)AiStateId.Move, this.MoveHandler);
      SetStateHandler((int)AiStateId.Wait, this.WaitHandler);
      SetStateHandler((int)AiStateId.MoveCommand, this.MoveCommandHandler);
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
      NpcAiStateInfo info = npc.GetAiStateInfo();
      npc.GetMovementStateInfo().IsMoving = false;
      ChangeToState(npc, (int)AiStateId.Wait);
    }
    private void MoveHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      if (npc.IsDead()) {
        npc.GetMovementStateInfo().IsMoving = false;
        ChangeToState(npc, (int)AiStateId.Wait);
        return;
      }
      NpcAiStateInfo info = npc.GetAiStateInfo();
      info.Time += deltaTime;
      if (info.Time > 10) {
        info.Time = 0;
        Vector3 srcPos = npc.GetMovementStateInfo().GetPosition3D();
        Vector3 startPos = info.HomePos;
        Vector3 targetPos = npc.GetMovementStateInfo().TargetPosition;
        if (!IsReached(srcPos, targetPos)) {
          float angle = Geometry.GetYAngle(new Vector2(srcPos.X, srcPos.Z), new Vector2(targetPos.X, targetPos.Z));
          npc.GetMovementStateInfo().SetMoveDir(angle);
          npc.GetMovementStateInfo().IsMoving = true;
        }
      }
    }
    private void WaitHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
    }
    private void MoveCommandHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      AiLogicUtility.DoMoveCommandState(npc, aiCmdDispatcher, deltaTime, this);
    }
    private bool IsReached(Vector3 src, Vector3 target)
    {
      bool ret = false;
      float powDist = Geometry.DistanceSquare(src,target);
      if (powDist <= 0.25f) {
        ret = true;
      }
      return ret;
    }
  }
  class AiLogic_ImmovableNpc_Client : AbstractNpcStateLogic
  {
    protected override void OnInitStateHandlers()
    {
      SetStateHandler((int)AiStateId.Idle, this.IdleHandler);
      SetStateHandler((int)AiStateId.Move, this.MoveHandler);
      SetStateHandler((int)AiStateId.Wait, this.WaitHandler);
      SetStateHandler((int)AiStateId.MoveCommand, this.MoveCommandHandler);
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
      NpcAiStateInfo info = npc.GetAiStateInfo();
      npc.GetMovementStateInfo().IsMoving = false;
      ChangeToState(npc, (int)AiStateId.Wait);
    }
    private void MoveHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      NpcAiStateInfo info = npc.GetAiStateInfo();
      npc.GetMovementStateInfo().IsMoving = false;
      ChangeToState(npc, (int)AiStateId.Wait);
    }
    private void WaitHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      NpcAiStateInfo info = npc.GetAiStateInfo();
      npc.GetMovementStateInfo().IsMoving = false;
    }
    private void MoveCommandHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      NpcAiStateInfo info = npc.GetAiStateInfo();
      npc.GetMovementStateInfo().IsMoving = false;
      ChangeToState(npc, (int)AiStateId.Wait);
    }
  }
  class AiLogic_FixedNpc_Client : AbstractNpcStateLogic
  {
    protected override void OnInitStateHandlers()
    {
      SetStateHandler((int)AiStateId.Idle, this.IdleHandler);
      SetStateHandler((int)AiStateId.Move, this.MoveHandler);
      SetStateHandler((int)AiStateId.Wait, this.WaitHandler);
      SetStateHandler((int)AiStateId.MoveCommand, this.MoveCommandHandler);
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
      NpcAiStateInfo info = npc.GetAiStateInfo();
      npc.GetMovementStateInfo().IsMoving = false;
      ChangeToState(npc, (int)AiStateId.Wait);
    }
    private void MoveHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      NpcAiStateInfo info = npc.GetAiStateInfo();
      npc.GetMovementStateInfo().IsMoving = false;
      ChangeToState(npc, (int)AiStateId.Wait);
    }
    private void WaitHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      npc.GetMovementStateInfo().IsMoving = false;
    }
    private void MoveCommandHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      NpcAiStateInfo info = npc.GetAiStateInfo();
      npc.GetMovementStateInfo().IsMoving = false;
      ChangeToState(npc, (int)AiStateId.Wait);
    }
  }
  class AiLogic_User_Client : AbstractUserStateLogic
  {
    protected override void OnInitStateHandlers()
    {
      SetStateHandler((int)AiStateId.Idle, this.IdleHandler);
      SetStateHandler((int)AiStateId.Move, this.MoveHandler);
      SetStateHandler((int)AiStateId.Wait, this.WaitHandler);
    }

    protected override void OnStateLogicInit(UserInfo user, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      UserAiStateInfo info = user.GetAiStateInfo();
      info.Time = 0;
      info.Target = 0;
    }

    private void IdleHandler(UserInfo user, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      UserAiStateInfo info = user.GetAiStateInfo();
      user.GetMovementStateInfo().IsMoving = false;
      ChangeToState(user, (int)AiStateId.Wait);
    }
    private void MoveHandler(UserInfo user, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
      if (user.IsDead()) {
        user.GetMovementStateInfo().IsMoving = false;
        ChangeToState(user, (int)AiStateId.Wait);
        return;
      }
      UserAiStateInfo info = user.GetAiStateInfo();
      info.Time += deltaTime;
      if (info.Time > 10) {
        info.Time = 0;
        Vector3 srcPos = user.GetMovementStateInfo().GetPosition3D();
        Vector3 targetPos = user.GetMovementStateInfo().TargetPosition;
        if (!IsReached(srcPos, targetPos)) {
          float angle = Geometry.GetYAngle(new Vector2(srcPos.X, srcPos.Z), new Vector2(targetPos.X, targetPos.Z));
          user.GetMovementStateInfo().SetMoveDir(angle);
          user.GetMovementStateInfo().IsMoving = true;
        }
      }
    }

    private void WaitHandler(UserInfo user, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
    {
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
  }
}
