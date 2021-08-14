using System;
using System.Collections.Generic;
using DashFire;
using UnityEngine;

namespace GfxModule.Impact {
  class GfxImpactLogic_Grab : AbstarctGfxImpactLogic{

    private enum StiffnessAction {
      HURT_FRONT = 0,
      HURT_RIGHT = 1,
      HURT_LEFT = 2,
    }
    public override void StartImpact(ImpactLogicInfo logicInfo) {
      LogicSystem.NotifyGfxAnimationStart(logicInfo.Target, false);
      SharedGameObjectInfo shareInfo = LogicSystem.GetSharedGameObjectInfo(logicInfo.Target);
      PlayAnimation(logicInfo.Target, Animation_Type.AT_Grab);
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
      UpdateEffect(logicInfo);
      string animName = GetAnimationNameByType(logicInfo.Target, Animation_Type.AT_Grab);
      if (null != logicInfo.Target) {
        if (!logicInfo.Target.GetComponent<Animation>().IsPlaying(animName)) {
          PlayAnimation(logicInfo.Target, Animation_Type.AT_Grab);
        }
      }
    }

    public override void StopImpact(ImpactLogicInfo logicInfo) {
      logicInfo.IsActive = false;
      StopAnimation(logicInfo.Target, Animation_Type.AT_Grab);
      if (null != logicInfo.Target) {
        Vector3 rotation = logicInfo.Target.transform.rotation.eulerAngles;
        logicInfo.Target.transform.rotation = Quaternion.Euler(0, rotation.y, 0);
      }
      LogicSystem.NotifyGfxAnimationFinish(logicInfo.Target, false);
      LogicSystem.NotifyGfxStopImpact(logicInfo.Sender, logicInfo.ImpactId, logicInfo.Target);
    }

    public override void OnInterrupted(ImpactLogicInfo logicInfo) {
      StopAnimation(logicInfo.Target, Animation_Type.AT_Grab);
      StopImpact(logicInfo);
    }

    public override bool OnOtherImpact(int logicId, ImpactLogicInfo logicInfo, bool isSameImpact) {
      switch (logicId) {
        case (int)GfxImpactLogicManager.GfxImpactLogicId.GfxImpactLogic_Default:
          return false;
        case (int)GfxImpactLogicManager.GfxImpactLogicId.GfxImpactLogic_HitFly:
          return false;
        case (int)GfxImpactLogicManager.GfxImpactLogicId.GfxImpactLogic_KnockDown:
          return false;
        case (int)GfxImpactLogicManager.GfxImpactLogicId.GfxImpactLogic_Stiffness:
          return false;
        case (int)GfxImpactLogicManager.GfxImpactLogicId.GfxImpactLogic_Grab:
          return false;
        default:
          return false;
      }
    }
  }
}

