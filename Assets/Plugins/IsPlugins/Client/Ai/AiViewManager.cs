using System;
using System.Collections;
using System.Collections.Generic;

namespace DashFire
{
    public sealed class AiViewManager
    {
        public void Init()
        {
            AbstractUserStateLogic.OnUserSkill += this.OnUserSkill;
            AbstractUserStateLogic.OnUserStopSkill += this.OnUserStopSkill;
            AbstractUserStateLogic.OnUserSendStoryMessage += this.OnUserSendStoryMessage;
            AbstractNpcStateLogic.OnNpcSkill += this.NpcSkill;
            AbstractNpcStateLogic.OnNpcAddImpact += this.OnNpcAddImpact;
            AbstractNpcStateLogic.OnNpcFaceClient += this.OnNpcFace;
            AbstractNpcStateLogic.OnNpcMeetEnemy += this.OnNpcMeetEnemy;
            AbstractNpcStateLogic.OnNpcSendStoryMessage += this.OnNpcSendStoryMessage;
            AiLogic_DropOut_AutoPick.OnDropoutPlayEffect += this.OnDropoutPlayEffect;
        }

        private void OnUserSkill(UserInfo user, int skillId)
        {
            user.SkillController.PushSkill(SkillCategory.kAttack, ScriptRuntime.Vector3.Zero);
        }

        private void OnUserStopSkill(UserInfo user)
        {
        }

        private void OnUserSendStoryMessage(UserInfo user, string msgId, object[] args)
        {
            if (WorldSystem.Instance.IsPveScene() || WorldSystem.Instance.IsPureClientScene())
            {
                ClientStorySystem.Instance.SendMessage(msgId, args);
            }
        }

        private void OnNpcAddImpact(NpcInfo npc, int impactId)
        {
            ImpactSystem.Instance.SendImpactToCharacter(npc, impactId, npc.GetId(), -1, -1, npc.GetMovementStateInfo().GetPosition3D(), npc.GetMovementStateInfo().GetFaceDir());
        }
        private void OnNpcFace(NpcInfo npc, float faceDirection)
        {
            npc.GetMovementStateInfo().SetWantFaceDir(faceDirection);
            ControlSystemOperation.AdjustCharacterFaceDir(npc.GetId(), faceDirection);
        }
        private void NpcSkill(NpcInfo npc, int skillId)
        {
            if (null != npc)
            {
                if (npc.SkillController != null)
                {
                    SkillInfo skillInfo = npc.GetSkillStateInfo().GetSkillInfoById(skillId);
                    if (null != skillInfo)
                    {
                        long curTime = TimeUtility.GetServerMilliseconds();
                        if (!skillInfo.IsInCd(curTime / 1000.0f))
                        {
                            npc.SkillController.ForceStartSkill(skillId);
                            skillInfo.BeginCD();
                        }
                    }
                }
            }
        }

        private void OnNpcMeetEnemy(NpcInfo npc, Animation_Type animType)
        {
            CharacterView view = EntityManager.Instance.GetCharacterViewById(npc.GetId());
            if (null != view)
            {
                GfxSystem.SendMessage(view.Actor, "OnEventMeetEnemy", null);
            }
            ImpactSystem.Instance.SendImpactToCharacter(npc, npc.GetMeetEnemyImpact(), npc.GetId(), -1, -1, npc.GetMovementStateInfo().GetPosition3D(), npc.GetMovementStateInfo().GetFaceDir());
        }

        private void OnNpcSendStoryMessage(NpcInfo npc, string msgId, object[] args)
        {
            if (WorldSystem.Instance.IsPveScene() || WorldSystem.Instance.IsPureClientScene())
            {
                ClientStorySystem.Instance.SendMessage(msgId, args);
            }
        }

        private void OnDropoutPlayEffect(int id, string effect, string path)
        {
            CharacterView view = EntityManager.Instance.GetCharacterViewById(id);
            if (null != view)
            {
                GfxSystem.CreateAndAttachGameObject(effect, view.Actor, path, 1.0f);
            }
        }

        private AiViewManager() { }

        public static AiViewManager Instance
        {
            get
            {
                return s_Instance;
            }
        }
        private static AiViewManager s_Instance = new AiViewManager();
    }
}
