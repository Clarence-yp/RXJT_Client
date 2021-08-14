using System;
using System.Collections.Generic;
using DashFire;
using UnityEngine;

namespace GfxModule.Impact {
  class GfxImpactLogic_Stiffness : AbstarctGfxImpactLogic{

    private enum StiffnessAction {
      HURT_FRONT = 0,
      HURT_RIGHT = 1,
      HURT_LEFT = 2,
    }
    public override void StartImpact(ImpactLogicInfo logicInfo) {
      GeneralStartImpact(logicInfo);
      logicInfo.ActionType = GetStiffnessAction(logicInfo);
      SharedGameObjectInfo shareInfo = LogicSystem.GetSharedGameObjectInfo(logicInfo.Target);
      if (null != shareInfo) {
        shareInfo.GfxStateFlag = shareInfo.GfxStateFlag | (int)GfxCharacterState_Type.Stiffness;
      }
      PlayAnimation(logicInfo.Target, (Animation_Type)logicInfo.ActionType);
    }

    private int GetStiffnessAction(ImpactLogicInfo logicInfo) {
      int type = 0;
      int actionCount = logicInfo.ConfigData.ActionList.Count;
      foreach (int i in logicInfo.ConfigData.ActionList) {
      }
      if (null != logicInfo && actionCount > 0) {
        type = logicInfo.ConfigData.ActionList[UnityEngine.Random.Range(0, actionCount)];
      } else {
      }
      switch (type) {
        case (int)StiffnessAction.HURT_FRONT:
          return (int)Animation_Type.AT_Hurt0;
        case (int)StiffnessAction.HURT_RIGHT:
          return (int)Animation_Type.AT_Hurt1;
        case (int)StiffnessAction.HURT_LEFT:
          return (int)Animation_Type.AT_Hurt2;
        default:
          return (int)Animation_Type.AT_Hurt0;
      }
    }
    public override void Tick(ImpactLogicInfo logicInfo) {
      float animSpeed = GetAnimationSpeedByTime(logicInfo, Time.time - logicInfo.StartTime);
      UpdateMovement(logicInfo, Time.deltaTime);
      SetAnimationSpeed(logicInfo.Target, (Animation_Type)logicInfo.ActionType, animSpeed);
      UpdateEffect(logicInfo);
      logicInfo.ElapsedTime += Time.deltaTime * animSpeed;
      logicInfo.ElapsedTimeForEffect += Time.deltaTime * GetLockFrameRate(logicInfo, Time.time - logicInfo.StartTime);
      if (logicInfo.ElapsedTime > GetAnimationLenthByType(logicInfo.Target, (Animation_Type)logicInfo.ActionType)) {
        StopImpact(logicInfo);
      }
    }

    protected override void UpdateMovement(ImpactLogicInfo info, float deltaTime) {
      if(null != info.ConfigData && null != info.Target){
        float speedRate = GetLockFrameRate(info, Time.time - info.StartTime);
        Vector3 motion = info.MoveDir * info.MovementInfo.GetSpeedByTime(info.ElapsedTimeForEffect) * deltaTime * speedRate;
        info.NormalPos += motion;
        motion = GfxImpactSystem.Instance.GetAdjustPoint(info.NormalPos - info.OrignalPos, info) + info.OrignalPos - info.Target.transform.position;
        Vector3 pos = info.Target.transform.position + motion;
        pos = new Vector3(pos.x, GetTerrainHeight(pos), pos.z);
        MoveTo(info.Target, pos);
      LogicSystem.NotifyGfxUpdatePosition(info.Target, info.Target.transform.position.x, info.Target.transform.position.y, info.Target.transform.position.z, 0, info.Target.transform.rotation.eulerAngles.y * Mathf.PI / 180f, 0);
      }
    }

    public override void UpdateEffect(ImpactLogicInfo logicInfo) {
      if (null == logicInfo.Target) return;
      SharedGameObjectInfo shareInfo = LogicSystem.GetSharedGameObjectInfo(logicInfo.Target);
      if (null != shareInfo && !shareInfo.AcceptStiffEffect) return;
      for (int i = 0; i < logicInfo.EffectList.Count; ++i) {
        EffectInfo effectInfo = logicInfo.EffectList[i];
        if (null != effectInfo) {
          if (effectInfo.StartTime < 0 && logicInfo.ElapsedTimeForEffect >= effectInfo.DelayTime / 1000) {
            effectInfo.IsActive = true;
            effectInfo.StartTime = Time.time;
            GameObject obj = ResourceSystem.NewObject(effectInfo.Path, effectInfo.PlayTime / 1000) as GameObject;
            if (null != obj) {
              if (String.IsNullOrEmpty(effectInfo.MountPoint)) {
                obj.transform.position = logicInfo.Target.transform.position + effectInfo.RelativePoint;
                Quaternion q = Quaternion.Euler(effectInfo.RelativeRotation.x, effectInfo.RelativeRotation.y, effectInfo.RelativeRotation.z);
                if (effectInfo.RotateWithTarget && null != logicInfo.Sender) {
                  obj.transform.rotation = Quaternion.LookRotation(logicInfo.Target.transform.position - logicInfo.Sender.transform.position, Vector3.up);
                  obj.transform.rotation = Quaternion.Euler(obj.transform.rotation.eulerAngles + effectInfo.RelativeRotation);
                } else {
                  obj.transform.rotation = q;
                }
              } else {
                Transform parent = LogicSystem.FindChildRecursive(logicInfo.Target.transform, effectInfo.MountPoint);
                if (null != parent) {
                  obj.transform.parent = parent;
                  obj.transform.localPosition = Vector3.zero;
                  Quaternion q = Quaternion.Euler(ImpactUtility.RadianToDegree(effectInfo.RelativeRotation.x), ImpactUtility.RadianToDegree(effectInfo.RelativeRotation.y), ImpactUtility.RadianToDegree(effectInfo.RelativeRotation.z));
                  obj.transform.localRotation = q;
                }
              }
            }
          }
        }
      }
    }

    public override void StopImpact(ImpactLogicInfo logicInfo) {
      if (IsLogicDead(logicInfo.Target)) {
        //SetGfxDead(logicInfo.Target, true);
      }
      SharedGameObjectInfo shareInfo = LogicSystem.GetSharedGameObjectInfo(logicInfo.Target);
      if (null != shareInfo) {
        shareInfo.GfxStateFlag = shareInfo.GfxStateFlag & ~((int)GfxCharacterState_Type.Stiffness);
      }
      logicInfo.IsActive = false;
      GeneralStopImpact(logicInfo);
    }

    public override void OnInterrupted(ImpactLogicInfo logicInfo) {
      StopAnimation(logicInfo.Target, (Animation_Type)logicInfo.ActionType);
      SharedGameObjectInfo shareInfo = LogicSystem.GetSharedGameObjectInfo(logicInfo.Target);
      if (null != shareInfo) {
        shareInfo.GfxStateFlag = shareInfo.GfxStateFlag & ~((int)GfxCharacterState_Type.Stiffness);
      }
      logicInfo.IsActive = false;
      GeneralStopImpact(logicInfo);
    }

    public override bool OnOtherImpact(int logicId, ImpactLogicInfo logicInfo, bool isSameImpact) {
      switch (logicId) {
        case (int)GfxImpactLogicManager.GfxImpactLogicId.GfxImpactLogic_Default:
          return false;
        case (int)GfxImpactLogicManager.GfxImpactLogicId.GfxImpactLogic_HitFly:
          StopImpact(logicInfo);
          return true;
        case (int)GfxImpactLogicManager.GfxImpactLogicId.GfxImpactLogic_KnockDown:
          StopImpact(logicInfo);
          return true;
        case (int)GfxImpactLogicManager.GfxImpactLogicId.GfxImpactLogic_Stiffness:
          if (!isSameImpact) {
            StopImpact(logicInfo);
          } else {
          }
          return true;
        case (int)GfxImpactLogicManager.GfxImpactLogicId.GfxImpactLogic_Grab:
          StopImpact(logicInfo);
          return true;
        default:
          return false;
      }
    }
  }
}
