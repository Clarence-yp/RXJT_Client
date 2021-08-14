using System;
using System.Collections.Generic;
using ScriptRuntime;

namespace DashFire
{
    class AiLogic_Npc_SmallMouse : AbstractNpcStateLogic
    {

        public class BlackBoard_SmallMouse
        {
            public BlackBoard_SmallMouse()
            {
                m_AliveNpcs = new List<int>();
                m_ReadyNpcs = new Dictionary<int, List<int>>();
                m_TargetId = 0;
            }
            public bool AddAliveNpc(int id)
            {
                if (!m_AliveNpcs.Contains(id))
                {
                    m_AliveNpcs.Add(id);
                    return true;
                }
                return false;
            }
            public bool AddReadyNpc(int targetId, int npcId)
            {
                if (0 == m_TargetId)
                {
                    if (!m_ReadyNpcs.ContainsKey(targetId))
                    {
                        m_ReadyNpcs.Add(targetId, new List<int>());
                    }
                    if (!m_ReadyNpcs[targetId].Contains(npcId))
                    {
                        m_ReadyNpcs[targetId].Add(npcId);
                        if (m_ReadyNpcs[targetId].Count > m_AliveNpcs.Count * m_Threshold)
                        {
                            m_TargetId = targetId;
                        }
                        return true;
                    }
                }
                return false;
            }
            public int TargetId
            {
                get { return m_TargetId; }
                set { m_TargetId = value; }
            }
            private List<int> m_AliveNpcs;
            private Dictionary<int, List<int>> m_ReadyNpcs;
            private int m_TargetId;

            public const float m_Threshold = 0.5f;
        }
        protected override void OnInitStateHandlers()
        {
            SetStateHandler((int)AiStateId.Idle, this.IdleHandler);
            SetStateHandler((int)AiStateId.Pursuit, this.PursuitHandler);
            SetStateHandler((int)AiStateId.MoveCommand, this.MoveCommandHandler);
            SetStateHandler((int)AiStateId.PursuitCommand, this.PursuitCommandHandler);
            SetStateHandler((int)AiStateId.PatrolCommand, this.PatrolCommandHandler);
        }

        protected override void OnStateLogicInit(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
        {
            NpcAiStateInfo info = npc.GetAiStateInfo();
            info.Time = 0;
            npc.GetMovementStateInfo().IsMoving = false;
            info.Target = 0;
            BlackBoard_SmallMouse blackBoradInfo = GetBlackBorad(npc);
            blackBoradInfo.AddAliveNpc(npc.GetId());
        }

        private void IdleHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
        {
            if (!CanAiControl(npc))
            {
                npc.GetMovementStateInfo().IsMoving = false;
                return;
            }
            NpcAiStateInfo info = npc.GetAiStateInfo();
            info.Time += deltaTime;
            if (info.Time > 100)
            {
                info.Time = 0;
                if (GetAiData(npc).HasPatrolData)
                {
                    npc.GetMovementStateInfo().IsMoving = false;
                    ChangeToState(npc, (int)AiStateId.PatrolCommand);
                }
                else
                {
                    CharacterInfo target = null;
                    if (info.IsExternalTarget)
                    {
                        target = AiLogicUtility.GetSeeingLivingCharacterInfoHelper(npc, info.Target);
                        if (null == target)
                        {
                            target = AiLogicUtility.GetNearstTargetHelper(npc, CharacterRelation.RELATION_ENEMY);
                            if (null != target)
                                info.Target = target.GetId();
                        }
                    }
                    else
                    {
                        target = AiLogicUtility.GetNearstTargetHelper(npc, CharacterRelation.RELATION_ENEMY);
                        if (null != target)
                            info.Target = target.GetId();
                    }
                    // 写黑板
                    if (0 != info.HateTarget)
                    {
                        GetBlackBorad(npc).TargetId = info.HateTarget;
                    }
                    // 读黑板
                    int blackBoardTargetId = GetBlackBorad(npc).TargetId;
                    if (0 != blackBoardTargetId)
                    {
                        target = AiLogicUtility.GetLivingCharacterInfoHelper(npc, blackBoardTargetId);
                        info.Target = blackBoardTargetId;
                    }
                    if (null != target)
                    {
                        NotifyNpcTargetChange(npc);
                        npc.GetMovementStateInfo().IsMoving = false;
                        NotifyNpcMove(npc);
                        GetBlackBorad(npc).AddReadyNpc(info.Target, npc.GetId());
                        info.Time = 0;
                        ChangeToState(npc, (int)AiStateId.Pursuit);
                    }
                }
            }
        }

        private void PursuitHandler(NpcInfo npc, AiCommandDispatcher aiCmdDispatcher, long deltaTime)
        {
            if (npc.IsDead())
            {
                npc.GetMovementStateInfo().IsMoving = false;
                NotifyNpcMove(npc);
                return;
            }
            if (npc.GetSkillStateInfo().IsSkillActivated()) return;
            NpcAiStateInfo info = npc.GetAiStateInfo();
            AiData_Npc_Bluelf data = GetAiData(npc);
            if (null != data)
            {
                // 写黑板
                if (0 != info.HateTarget)
                {
                    GetBlackBorad(npc).TargetId = info.HateTarget;
                }
                if (npc.GetSkillStateInfo().IsImpactControl()) return;
                // 读黑板
                int blackBoardTarget = GetBlackBorad(npc).TargetId;
                if (0 != blackBoardTarget)
                {
                    info.Target = blackBoardTarget;
                    NotifyNpcTargetChange(npc);
                }
                CharacterInfo target = AiLogicUtility.GetLivingCharacterInfoHelper(npc, info.Target);
                if (null != target)
                {
                    float dist = (float)npc.GetActualProperty().AttackRange;
                    float distGoHome = (float)npc.GohomeRange;
                    Vector3 targetPos = target.GetMovementStateInfo().GetPosition3D();
                    ScriptRuntime.Vector3 srcPos = npc.GetMovementStateInfo().GetPosition3D();
                    float powDist = Geometry.DistanceSquare(srcPos, targetPos);
                    float powDistToHome = Geometry.DistanceSquare(srcPos, info.HomePos);
                    // 遇敌是播放特效 逗留两秒。
                    if (data.WaitTime <= m_ResponseTime)
                    {
                        if (!data.HasMeetEnemy)
                        {
                            NotifyNpcMeetEnemy(npc, Animation_Type.AT_Attack);
                            data.HasMeetEnemy = true;
                        }
                        TrySeeTarget(npc, target);
                        data.WaitTime += deltaTime;
                        return;
                    }

                    // 追逐目标 
                    if (powDist > m_AttackRange * m_AttackRange)
                    {
                        npc.GetActualProperty().SetMoveSpeed(Operate_Type.OT_Absolute, AiLogicUtility.GetRunSpeed(npc));
                        npc.GetMovementStateInfo().MovementMode = MovementMode.HighSpeed;
                        info.Time += deltaTime;
                        if (info.Time > m_IntervalTime)
                        {
                            info.Time = 0;
                            AiLogicUtility.PathToTargetWithoutObstacle(npc, data.FoundPath, targetPos, m_IntervalTime, true, this);
                        }
                    }
                    else
                    {
                        //小于攻击距离, 可以攻击
                        npc.GetMovementStateInfo().IsMoving = false;
                        if (0 != blackBoardTarget)
                        {
                            int skillId = GetCanCastSkillId(npc, (float)Math.Sqrt(powDist));
                            TryCastSkill(npc, skillId, target);
                        }
                        else
                        {
                            // 不敢攻击
                            TrySeeTarget(npc, target);
                        }
                    }
                }
            }
        }
        private int GetCanCastSkillId(NpcInfo npc, float dis)
        {
            List<SkillInfo> skills = npc.GetSkillStateInfo().GetAllSkill();
            List<int> canCastSkills = new List<int>();
            foreach (SkillInfo skill in skills)
            {
                if (skill.ConfigData.SkillRangeMin < dis && skill.ConfigData.SkillRangeMax > dis)
                {
                    canCastSkills.Add(skill.SkillId);
                }
            }
            if (canCastSkills.Count > 0)
            {
                return canCastSkills[Helper.Random.Next(canCastSkills.Count)];
            }
            else
            {
                return -1;
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
        private AiData_Npc_Bluelf GetAiData(NpcInfo npc)
        {
            AiData_Npc_Bluelf data = npc.GetAiStateInfo().AiDatas.GetData<AiData_Npc_Bluelf>();
            if (null == data)
            {
                data = new AiData_Npc_Bluelf();
                npc.GetAiStateInfo().AiDatas.AddData(data);
            }
            return data;
        }

        private BlackBoard_SmallMouse GetBlackBorad(NpcInfo npc)
        {
            BlackBoard_SmallMouse info = npc.SceneContext.BlackBoard.BlackBoardDatas.GetData<BlackBoard_SmallMouse>();
            if (null == info)
            {
                info = new BlackBoard_SmallMouse();
                npc.SceneContext.BlackBoard.BlackBoardDatas.AddData<BlackBoard_SmallMouse>(info);
            }
            return info;
        }
        private const long m_IntervalTime = 100;
        private const long m_ResponseTime = 100;       // 遇敌逗留时间
        private const long m_MeetWalkMaxTime = 0;
        private const long m_ChaseWalkMaxTime = 0;
        private const long m_TauntTime = 3000;

        private const float m_AttackRange = 2.0f;
    }
}
