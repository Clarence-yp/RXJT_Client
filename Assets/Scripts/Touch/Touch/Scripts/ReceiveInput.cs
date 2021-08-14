using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DashFire;

public struct SkillIconInfo
{
    public Vector3 targetPos;
    public SkillCategory skillType;
    public bool isVetical;
    public float time;

}

public class ReceiveInput : MonoBehaviour
{
    protected struct CandidateSkillInfo
    {
        public SkillCategory skillType;
        public Vector3 targetPos;
    }

    internal void Start()
    {
        DashFire.LogicSystem.EventChannelForGfx.Subscribe<int>("Ai_InputSkillPursuitCmd", "Input", this.SkillPursuit);
        DashFire.LogicSystem.EventChannelForGfx.Subscribe<int, float, float, float>("Ai_InputAttackCmd", "Input", this.AttackHandle);
        DashFire.LogicSystem.EventChannelForGfx.Subscribe<int>("Ai_InputSkillCmd", "Input", this.SkillHandle);
        DashFire.LogicSystem.EventChannelForGfx.Subscribe<GestureEvent, float, float, float, bool, bool>("Op_InputEffect", "Input", this.EffectHandle);
        DashFire.LogicSystem.EventChannelForGfx.Subscribe<bool>("SetTouchEnable", "Input", SetTouchEnable);
        DashFire.LogicSystem.EventChannelForGfx.Subscribe<bool>("SetJoystickEnable", "Input", SetJoystickEnable);
        ///
        TouchManager.OnGestureHintEvent += OnGestureHintEvent;
        TouchManager.OnGestureEvent += OnGestureEvent;
        ///
        EasyJoystick.On_JoystickMoveStart += On_JoystickMoveStart;
        EasyJoystick.On_JoystickMove += On_JoystickMove;
        EasyJoystick.On_JoystickMoveEnd += On_JoystickMoveEnd;
    }

    private void SetTouchEnable(bool value)
    {
        try
        {
            TouchManager.TouchEnable = value;
        }
        catch (Exception ex)
        {
            DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
        }
    }

    private void SetJoystickEnable(bool value)
    {
        try
        {
            JoyStickInputProvider.JoyStickEnable = value;
        }
        catch (Exception ex)
        {
            DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
        }
    }

    private void SkillPursuit(int id)
    {
        try
        {
            if (null != skill_ctrl)
            {
                skill_ctrl.AddBreakSkillTask();
            }
        }
        catch (Exception ex)
        {
            DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
        }
    }

    internal void Update()
    {
        if (!isRegister)
        {
            if (null != skill_ctrl)
            {
                skill_ctrl.RegisterSkillQECanInputHandler(SkillQEInputHandler);
                skill_ctrl.RegisterSkillStartHandler(SkillStartHandler);
                isRegister = true;
            }
        }
        ///
        skill_active_remaintime -= Time.deltaTime;
        if (skill_active_remaintime <= 0)
        {
            can_conjure_q_skill = false;
            can_conjure_e_skill = false;
        }
    }

    internal void OnDestroy()
    {
        TouchManager.OnGestureHintEvent -= OnGestureHintEvent;
        TouchManager.OnGestureEvent -= OnGestureEvent;
        ///
        EasyJoystick.On_JoystickMoveStart -= On_JoystickMoveStart;
        EasyJoystick.On_JoystickMove -= On_JoystickMove;
        EasyJoystick.On_JoystickMoveEnd -= On_JoystickMoveEnd;
        //GfxModule.Skill.Trigers.TriggerUtil.IsWantMoving = false;
    }

    private void OnGestureHintEvent(Gesture gesture)
    {
        if (null == gesture)
        {
            return;
        }
        if (HintType.Hint == gesture.HintFlag)
        {
        }
        else if (HintType.RFailure == gesture.HintFlag)
        {
        }
        else if (HintType.RSucceed == gesture.HintFlag)
        {
            ///
            bool isInSkillCD = false;
            if (0 == gesture.SectionNum && skill_ctrl != null && skill_ctrl.IsCategorySkillInCD(gesture.SkillTags))
            {
                isInSkillCD = true;
            }
            ///
            if (gesture.SkillTags == SkillCategory.kSkillE || gesture.SkillTags == SkillCategory.kSkillQ)
            {
                if ((can_conjure_q_skill && gesture.SkillTags == SkillCategory.kSkillQ)
                  || (can_conjure_e_skill && gesture.SkillTags == SkillCategory.kSkillE))
                {
                    DashFire.LogicSystem.EventChannelForGfx.Publish("ge_ui_angle", "ui", gesture.Position, gesture.SkillTags, isInSkillCD);
                    can_conjure_q_skill = false;
                    can_conjure_e_skill = false;
                }
            }
            else
            {
                DashFire.LogicSystem.EventChannelForGfx.Publish("ge_ui_angle", "ui", gesture.StartPosition, gesture.SkillTags, isInSkillCD);
                if (0 == gesture.SectionNum)
                {
                }
                ///
                List<SkillNode> nodeInfo = null;
                if (skill_ctrl != null)
                {
                    skill_ctrl.QuerySkillQE(gesture.SkillTags, gesture.SectionNum);
                }
                if (null != nodeInfo && nodeInfo.Count > 0)
                {
                    skill_active_remaintime = 1.0f;
                    foreach (SkillNode node in nodeInfo)
                    {
                        if (SkillCategory.kSkillQ == node.Category)
                        {
                            can_conjure_q_skill = true;
                        }
                        else
                        {
                            can_conjure_e_skill = true;
                        }
                    }
                }
            }
        }
    }

    private void OnGestureEvent(Gesture gesture)
    {
        /*
        if (null == gesture) {
          return;
        }
        Vector3 target_pos = gesture.GetStartTouchToWorldPoint();
        if (null != gesture.Recognizer && Vector3.zero != target_pos) {
          if ("OnDoubleTap" == gesture.Recognizer.EventMessageName
            && gesture.SelectedID < 0) {
            GfxModule.Skill.GfxSkillSystem.Instance.StopAttack(DashFire.LogicSystem.PlayerSelf);
            GfxModule.Skill.GfxSkillSystem.Instance.PushSkill(DashFire.LogicSystem.PlayerSelf, SkillCategory.kRoll, target_pos);
            return;
          }
        }
        if (SkillCategory.kNone != gesture.SkillTags) {
          switch (gesture.SkillTags) {
            case SkillCategory.kSkillA:
            case SkillCategory.kSkillB:
            case SkillCategory.kSkillC:
            case SkillCategory.kSkillD:
            case SkillCategory.kSkillQ:
            case SkillCategory.kSkillE:
              GfxModule.Skill.GfxSkillSystem.Instance.StopAttack(DashFire.LogicSystem.PlayerSelf);
              if (gesture.SectionNum > 0) {
                if (waite_skill_buffer.Count > 0) {
                  CandidateSkillInfo candidate_skill_info = new CandidateSkillInfo();
                  candidate_skill_info.skillType = gesture.SkillTags;
                  candidate_skill_info.targetPos = Vector3.zero;
                  waite_skill_buffer.Add(candidate_skill_info);
                } else {
                  GfxModule.Skill.GfxSkillSystem.Instance.PushSkill(DashFire.LogicSystem.PlayerSelf, gesture.SkillTags, Vector3.zero);
                }
              } else {
                waite_skill_buffer.Clear();
                CandidateSkillInfo candidate_skill_info = new CandidateSkillInfo();
                candidate_skill_info.skillType = gesture.SkillTags;
                candidate_skill_info.targetPos = target_pos;
                waite_skill_buffer.Add(candidate_skill_info);
                GestureArgs e = TouchManager.ToGestureArgs(gesture);
                LogicSystem.FireGestureEvent(e);
              }
              break;
          }
        }*/
    }

    private void AttackHandle(int id, float x, float y, float z)
    {
        try
        {
            GameObject obj = DashFire.LogicSystem.PlayerSelf;
            if (null != obj)
            {
                GfxModule.Skill.GfxSkillSystem.Instance.StartAttack(DashFire.LogicSystem.PlayerSelf, new Vector3(x, y, z));
            }
        }
        catch (Exception ex)
        {
            DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
        }
    }

    private void SkillHandle(int id)
    {
        try
        {
            foreach (CandidateSkillInfo node in waite_skill_buffer)
            {
                push_skill_buffer.Add(node);
            }
            waite_skill_buffer.Clear();
            foreach (CandidateSkillInfo node in push_skill_buffer)
            {
                GfxModule.Skill.GfxSkillSystem.Instance.PushSkill(DashFire.LogicSystem.PlayerSelf, node.skillType, node.targetPos);
            }
            push_skill_buffer.Clear();
        }
        catch (Exception ex)
        {
            DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
        }
    }

    private void SkillQEInputHandler(float remaintime, List<SkillNode> skills)
    {
        if (null != skills && skills.Count > 0)
        {
            skill_active_remaintime = remaintime;
            foreach (SkillNode node in skills)
            {
                if (SkillCategory.kSkillQ == node.Category)
                {
                    can_conjure_q_skill = true;
                }
                else
                {
                    can_conjure_e_skill = true;
                }
            }
        }
    }

    private void SkillStartHandler()
    {
        GestureArgs e = new GestureArgs();
        e.name = GestureEvent.OnSkillStart.ToString();
        LogicSystem.FireGestureEvent(e);
    }

    void On_JoystickMoveStart(MovingJoystick move)
    {
        //TouchManager.GestureEnable = false;
    }
    void On_JoystickMoveEnd(MovingJoystick move)
    {
        TriggerMove(move, true);
        //TouchManager.GestureEnable = true;
    }
    void On_JoystickMove(MovingJoystick move)
    {
        if (TouchManager.Touches.Count > 0)
        {
            TriggerMove(move, false);
        }
    }
    private void TriggerMove(MovingJoystick move, bool isLift)
    {
        if (isLift)
        {
            GestureArgs e = new GestureArgs();
            e.name = "OnSingleTap";
            e.airWelGamePosX = 0f;
            e.airWelGamePosY = 0f;
            e.airWelGamePosZ = 0f;
            e.selectedObjID = -1;
            e.towards = -1f;
            e.inputType = InputType.Joystick;
            //LogicSystem.FireGestureEvent(e);
            LogicSystem.SetJoystickInfo(e);
            return;
        }

        GameObject playerSelf = LogicSystem.PlayerSelf;
        if (playerSelf != null && move.joystickAxis != Vector2.zero)
        {
            Vector2 joyStickDir = move.joystickAxis * 10.0f;
            Vector3 targetRot = new Vector3(joyStickDir.x, 0, joyStickDir.y);
            Vector3 targetPos = playerSelf.transform.position + targetRot;

            GestureArgs e = new GestureArgs();
            e.name = "OnSingleTap";
            e.selectedObjID = -1;
            float towards = Mathf.Atan2(targetPos.x - playerSelf.transform.position.x, targetPos.z - playerSelf.transform.position.z);
            e.towards = towards;
            e.airWelGamePosX = targetPos.x;
            e.airWelGamePosY = targetPos.y;
            e.airWelGamePosZ = targetPos.z;
            e.inputType = InputType.Joystick;
            //LogicSystem.FireGestureEvent(e);
            LogicSystem.SetJoystickInfo(e);
        }
    }

    /// effect handle
    private void EffectHandle(GestureEvent ge, float posX, float posY, float posZ, bool isSelected, bool isLogicCmd)
    {
        try
        {
            Vector3 effect_pos = new Vector3(posX, posY, posZ);
            if (GestureEvent.OnSingleTap == ge)
            {
                if (isSelected)
                {
                    if (!isLogicCmd)
                    {
                        PlayEffect(Go_Lock, effect_pos, Go_Lock_Time);
                    }
                }
                else
                {
                    PlayEffect(Go_Landmark, effect_pos, Go_Landmark_Time);
                    if (null != skill_ctrl)
                    {
                        skill_ctrl.AddBreakSkillTask();
                        skill_ctrl.HideSkillTip(SkillCategory.kNone);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
        }
    }

    private void PlayEffect(GameObject effectPrefab, Vector3 position, float playTime)
    {
        GameObject obj = DashFire.ResourceSystem.NewObject(effectPrefab, playTime) as GameObject;
        if (obj != null)
        {
            obj.transform.position = position;
            obj.transform.rotation = Quaternion.identity;
        }
    }

    protected float skill_active_remaintime = -1;
    protected bool can_conjure_q_skill = false;
    protected bool can_conjure_e_skill = false;
    protected bool isRegister = false;
    protected List<CandidateSkillInfo> waite_skill_buffer = new List<CandidateSkillInfo>();
    protected List<CandidateSkillInfo> push_skill_buffer = new List<CandidateSkillInfo>();
    protected DashFire.SkillController skill_ctrl = null;
    public GameObject Go_Landmark = null;
    public float Go_Landmark_Time = 0.2f;
    public GameObject Go_Lock = null;
    public float Go_Lock_Time = 0.2f;
    ///
    public delegate void EventHandler(float towards);
    public static EventHandler OnJoystickMove;
}