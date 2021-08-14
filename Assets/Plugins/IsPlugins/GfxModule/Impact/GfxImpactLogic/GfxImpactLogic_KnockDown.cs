using System;
using System.Collections.Generic;
using DashFire;
using UnityEngine;

namespace GfxModule.Impact {
  class GfxImpactLogic_KnockDown : AbstarctGfxImpactLogic {

    private enum KnockDownState {
      Falling,
      OnGround,
      StandUp,
    }

    private class KnockDownParams {
      public KnockDownState ImpactState = KnockDownState.Falling;
      public float HitGroundTime = 0.0f;
    }
    public override void StartImpact(ImpactLogicInfo logicInfo) {
      GeneralStartImpact(logicInfo);
      SetGfxStateFlag(logicInfo.Target, Operate_Type.OT_AddBit, GfxCharacterState_Type.KnockDown);
      KnockDownParams param = GetKnockDownParams(logicInfo);
      param.ImpactState = KnockDownState.Falling;
      CrossFadeAnimation(logicInfo.Target, Animation_Type.AT_FlyDown);
    }

    public override void Tick(ImpactLogicInfo logicInfo) {
      UpdateMovement(logicInfo, Time.deltaTime);
      UpdateEffect(logicInfo);
      GameObject target = logicInfo.Target;
      KnockDownParams param = GetKnockDownParams(logicInfo);
      switch (param.ImpactState) {
        case KnockDownState.Falling:
          if (Time.time > logicInfo.StartTime + m_FallDownTime) {
            // 落地尘土
            param.ImpactState = KnockDownState.OnGround;
            param.HitGroundTime = Time.time;
            PlayAnimation(target, Animation_Type.AT_FlyDownGround);
          }
          break;
        case KnockDownState.OnGround:
          if (IsLogicDead(target)) {
            SetGfxDead(target, true);
            CrossFadeAnimation(target, Animation_Type.AT_OnGround);
            StopImpact(logicInfo);
          }
          if (Time.time > param.HitGroundTime + GetAnimationLenthByType(target, Animation_Type.AT_FlyDownGround) + logicInfo.ConfigData.OnGroundTime) {
            // 倒地时间
            SetGfxStateFlag(logicInfo.Target, Operate_Type.OT_RemoveBit, GfxCharacterState_Type.KnockDown);
            SetGfxStateFlag(logicInfo.Target, Operate_Type.OT_AddBit, GfxCharacterState_Type.GetUp);
            PlayAnimation(target, Animation_Type.AT_GetUp1);
            param.ImpactState = KnockDownState.StandUp;
          }
          break;
        case KnockDownState.StandUp:
          if(Time.time > param.HitGroundTime + GetAnimationLenthByType(target, Animation_Type.AT_FlyDownGround) + logicInfo.ConfigData.OnGroundTime + GetAnimationLenthByType(target, Animation_Type.AT_GetUp1)) {
               SetGfxStateFlag(logicInfo.Target, Operate_Type.OT_RemoveBit, GfxCharacterState_Type.GetUp);
               StopImpact(logicInfo);
          }
          break;
      }
    }

    protected override void UpdateMovement(ImpactLogicInfo info, float deltaTime) {
      if(null != info.ConfigData && null != info.Target){
        info.Velocity = info.MoveDir * info.MovementInfo.GetSpeedByTime(Time.time - info.StartTime);
        Vector3 motion = info.Velocity * deltaTime;
        info.NormalPos += motion;
        motion = GfxImpactSystem.Instance.GetAdjustPoint(info.NormalPos - info.OrignalPos, info) + info.OrignalPos - info.Target.transform.position;
        Vector3 pos = info.Target.transform.position + motion;
        pos = new Vector3(pos.x, GetTerrainHeight(pos), pos.z);
        MoveTo(info.Target, pos);
      LogicSystem.NotifyGfxUpdatePosition(info.Target, info.Target.transform.position.x, info.Target.transform.position.y, info.Target.transform.position.z, 0, info.Target.transform.rotation.eulerAngles.y * Mathf.PI / 180f, 0);
      }
    }
    public override void StopImpact(ImpactLogicInfo logicInfo) {
      if (IsLogicDead(logicInfo.Target)) {
        //SetGfxDead(logicInfo.Target, true);
      }
      logicInfo.IsActive = false;
      GeneralStopImpact(logicInfo);
    }

    public override void OnInterrupted(ImpactLogicInfo logicInfo) {
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
          if (!isSameImpact) {
            StopImpact(logicInfo);
          }
          return true;
        case (int)GfxImpactLogicManager.GfxImpactLogicId.GfxImpactLogic_Stiffness:
          return false;
        case (int)GfxImpactLogicManager.GfxImpactLogicId.GfxImpactLogic_Grab:
          StopImpact(logicInfo);
          return true;
        default:
          return false;
      }
    }

    private KnockDownParams GetKnockDownParams(ImpactLogicInfo logicInfo)
    {
      KnockDownParams result = logicInfo.CustomDatas.GetData<KnockDownParams>();
      if (null == result) {
        result = new KnockDownParams();
        logicInfo.CustomDatas.AddData<KnockDownParams>(result);
      }
      return result;
    }

    private const float m_FallDownTime = 0.5f;
  }
}

