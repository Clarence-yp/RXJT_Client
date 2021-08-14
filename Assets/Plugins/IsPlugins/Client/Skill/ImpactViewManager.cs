using System;
using System.Collections;
using System.Collections.Generic;

namespace DashFire
{
    public sealed class ImpactViewManager
    {
        public void Init()
        {
            ImpactSystem.EventSendImpact += this.OnSendImpact;
            ImpactSystem.EventStopImpact += this.OnStopImpact;
            ImpactSystem.EventGfxStopImpact += this.OnGfxStopImpact;
            AbstractImpactLogic.EventImpactLogicDamage += this.OnImpactDamage;
            AbstractImpactLogic.EventImpactLogicSkill += this.OnImpactSkill;
            AbstractImpactLogic.EventImpactLogicEffect += this.OnImpactEffect;
            AbstractImpactLogic.EventImpactLogicScreenTip += this.OnImpactScreenTip;

            ImpactLogic_DamageImmunityShield.EventStartDamageImmunityShiled += this.OnStartDamageImmunityShiled;
            ImpactLogic_DamageImmunityShield.EventUpdateDamageImmunityShiled += this.OnUpdateDamageImmunityShiled;
            ImpactLogic_DamageImmunityShield.EventStopDamageImmunityShiled += this.OnStopDamageImmunityShiled;

            ImpactLogic_SuperArmorShield.EventStartSuperArmorShield += this.OnStartSuperArmorShiled;
            ImpactLogic_SuperArmorShield.EventUpdateSuperArmorShield += this.OnUpdateSuperArmorShiled;
            ImpactLogic_SuperArmorShield.EventStopSuperArmorShield += this.OnStopSuperArmorShiled;
        }

        #region ImpactSystem && AbstractImpactLogic
        private void OnSendImpact(CharacterInfo sender, int targetId, int impactId, ScriptRuntime.Vector3 srcPos, float srcDir)
        {
            CharacterView senderView = EntityManager.Instance.GetCharacterViewById(sender.GetId());
            CharacterView targetView = EntityManager.Instance.GetCharacterViewById(targetId);
            if (null != senderView && null != targetView)
            {
                CharacterInfo target = WorldSystem.Instance.GetCharacterById(targetId);
                if (null != target)
                {
                    // 施法者能造成硬直且受击方没有霸体
                    ImpactInfo impactInfo = target.GetSkillStateInfo().GetImpactInfoById(impactId);
                    if (null == impactInfo) return;
                    int forceLogicId = -1;
                    if (sender.CauseStiff && !target.SuperArmor && !target.UltraArmor)
                    {
                        // 正常造成硬直
                    }
                    else
                    {
                        forceLogicId = 0;
                        impactInfo.m_IsGfxControl = false;
                    }
                    // 场景破坏物体单独处理
                    if (target.IsNpc)
                    {
                        NpcInfo npcInfo = target.CastNpcInfo();
                        if (npcInfo.NpcType == (int)NpcTypeEnum.SceneObject)
                        {
                            forceLogicId = 1;
                            impactInfo.m_IsGfxControl = true;
                        }
                    }
                    // 打断技能
                    if ((null != impactInfo && 0 != impactInfo.ConfigData.ImpactGfxLogicId && forceLogicId < 0) || forceLogicId > 0)
                    {
                        if (null != target.SkillController)
                        {
                            target.SkillController.ForceInterruptCurSkill();
                        }
                        else
                        {
                            LogSystem.Warn("{0} does't have a skillcontroller", target.GetName());
                        }
                    }
                    GfxSystem.QueueGfxAction(GfxModule.Impact.GfxImpactSystem.Instance.SendImpactToCharacter, senderView.Actor, targetView.Actor, impactId, srcPos.X, srcPos.Y, srcPos.Z, srcDir, forceLogicId);
                }
            }
        }

        private void OnStopImpact(CharacterInfo sender, int targetId, int impactId)
        {
            CharacterView target = EntityManager.Instance.GetCharacterViewById(targetId);
            if (null != target)
            {
                GfxSystem.QueueGfxAction(GfxModule.Impact.GfxImpactSystem.Instance.StopGfxImpact, target.Actor, impactId);
            }
        }

        private void OnGfxStopImpact(CharacterInfo sender, int targetId, int impactId)
        {
            CharacterInfo target = WorldSystem.Instance.GetCharacterById(targetId);
            if (null != target)
            {
                if (WorldSystem.Instance.IsPvpScene() || WorldSystem.Instance.IsMultiPveScene())
                {
                    if (target.GetId() == WorldSystem.Instance.PlayerSelfId)
                    {
                        Network.NetworkSystem.Instance.SyncStopGfxImpact(sender,
                                                                         targetId,
                                                                         impactId);
                    }
                    if (target.OwnerId == WorldSystem.Instance.PlayerSelfId)
                    {
                        Network.NetworkSystem.Instance.SyncStopGfxImpact(sender,
                                                                          targetId,
                                                                          impactId);
                    }
                }
            }
        }

        private void OnImpactDamage(CharacterInfo entity, int attackerId, int damage, bool isKiller, bool isCritical, bool isOrdinary)
        {
            if (WorldSystem.Instance.IsPureClientScene() || WorldSystem.Instance.IsPveScene())
            {
                if (null != entity)
                {
                    entity.SetAttackerInfo(attackerId, 0, isKiller, isOrdinary, isCritical, damage, 0);
                }
            }
        }

        private void OnImpactSkill(CharacterInfo entity, int skillId)
        {
            if (null != entity)
            {
                if (entity.SkillController != null)
                {
                    SkillInfo skillInfo = entity.GetSkillStateInfo().GetSkillInfoById(skillId);
                    if (null != skillInfo)
                    {
                        long curTime = TimeUtility.GetServerMilliseconds();
                        if (!skillInfo.IsInCd(curTime / 1000.0f))
                        {
                            entity.SkillController.ForceStartSkill(skillId);
                            skillInfo.BeginCD();
                        }
                    }
                }
            }
        }

        private void OnImpactEffect(CharacterInfo target, string effectPath, string bonePath, float recycleTime)
        {
            if (null != target)
            {
                CharacterView view = EntityManager.Instance.GetCharacterViewById(target.GetId());
                if (null != view)
                {
                    GfxSystem.CreateAndAttachGameObject(effectPath, view.Actor, bonePath, recycleTime);
                }
            }
        }

        private void OnImpactScreenTip(CharacterInfo target, string tip)
        {
            if (null != target)
            {
                ScriptRuntime.Vector3 pos = target.GetMovementStateInfo().GetPosition3D();
                pos.Y += 2.0f;
                GfxSystem.PublishGfxEvent("ge_screen_tip", "ui", pos.X, pos.Y, pos.Z, true, tip);
            }
        }
        #endregion

        #region ImpactLogic_DamageImmunityShield
        private void OnStartDamageImmunityShiled(CharacterInfo obj, int impactId)
        {
            if (null != obj)
            {
                CharacterView view = EntityManager.Instance.GetCharacterViewById(obj.GetId());
                if (null != view)
                {
                    ImpactInfo impactInfo = obj.GetSkillStateInfo().GetImpactInfoById(impactId);
                    if (null != impactInfo)
                    {
                        ImpactLogic_DamageImmunityShield.DamageImmunityShieldInfo info = impactInfo.LogicDatas.GetData<ImpactLogic_DamageImmunityShield.DamageImmunityShieldInfo>();
                        if (null != info)
                        {
                            info.EffectActor = GameObjectIdManager.Instance.GenNextId();
                            GfxSystem.CreateGameObjectForAttach(info.EffectActor, info.EffectPath);
                            GfxSystem.AttachGameObject(info.EffectActor, view.Actor);
                            // 黄色 1
                            GfxSystem.PublishGfxEvent("ge_start_monster_sheild", "ui", view.Actor, 1);
                        }
                    }
                }
            }
        }
        private void OnUpdateDamageImmunityShiled(CharacterInfo obj, int impactId)
        {
            if (null != obj)
            {
                CharacterView view = EntityManager.Instance.GetCharacterViewById(obj.GetId());
                if (null != view)
                {
                    ImpactInfo impactInfo = obj.GetSkillStateInfo().GetImpactInfoById(impactId);
                    if (null != impactInfo)
                    {
                        ImpactLogic_DamageImmunityShield.DamageImmunityShieldInfo info = impactInfo.LogicDatas.GetData<ImpactLogic_DamageImmunityShield.DamageImmunityShieldInfo>();
                        if (null != info)
                        {
                            GfxSystem.PublishGfxEvent("ge_update_monster_sheild", "ui", view.Actor, info.CountRemain * 1.0f / info.CountMax);
                        }
                    }
                }
            }
        }
        private void OnStopDamageImmunityShiled(CharacterInfo obj, int impactId)
        {
            if (null != obj)
            {
                CharacterView view = EntityManager.Instance.GetCharacterViewById(obj.GetId());
                if (null != view)
                {
                    ImpactInfo impactInfo = obj.GetSkillStateInfo().GetImpactInfoById(impactId);
                    if (null != impactInfo)
                    {
                        ImpactLogic_DamageImmunityShield.DamageImmunityShieldInfo info = impactInfo.LogicDatas.GetData<ImpactLogic_DamageImmunityShield.DamageImmunityShieldInfo>();
                        if (null != info)
                        {
                            GfxSystem.DestroyGameObject(info.EffectActor);
                            GfxSystem.PublishGfxEvent("ge_end_monster_sheild", "ui", view.Actor);
                        }
                    }
                }
            }
        }
        #endregion

        #region ImpactLogic_SuperArmorShield
        private void OnStartSuperArmorShiled(CharacterInfo obj, int impactId)
        {
            if (null != obj)
            {
                CharacterView view = EntityManager.Instance.GetCharacterViewById(obj.GetId());
                if (null != view)
                {
                    ImpactInfo impactInfo = obj.GetSkillStateInfo().GetImpactInfoById(impactId);
                    if (null != impactInfo)
                    {
                        ImpactLogic_SuperArmorShield.SuperArmorShieldInfo info = impactInfo.LogicDatas.GetData<ImpactLogic_SuperArmorShield.SuperArmorShieldInfo>();
                        if (null != info)
                        {
                            info.EffectActor = GameObjectIdManager.Instance.GenNextId();
                            GfxSystem.CreateGameObjectForAttach(info.EffectActor, info.EffectPath);
                            GfxSystem.AttachGameObject(info.EffectActor, view.Actor);
                            GfxSystem.PublishGfxEvent("ge_start_monster_sheild", "ui", view.Actor, 0);
                        }
                    }
                }
            }
        }
        private void OnUpdateSuperArmorShiled(CharacterInfo obj, int impactId)
        {
            if (null != obj)
            {
                CharacterView view = EntityManager.Instance.GetCharacterViewById(obj.GetId());
                if (null != view)
                {
                    ImpactInfo impactInfo = obj.GetSkillStateInfo().GetImpactInfoById(impactId);
                    if (null != impactInfo)
                    {
                        ImpactLogic_SuperArmorShield.SuperArmorShieldInfo info = impactInfo.LogicDatas.GetData<ImpactLogic_SuperArmorShield.SuperArmorShieldInfo>();
                        if (null != info)
                        {
                            GfxSystem.PublishGfxEvent("ge_update_monster_sheild", "ui", view.Actor, info.DamageRemain * 1.0f / info.DamageMax);
                        }
                    }
                }
            }
        }
        private void OnStopSuperArmorShiled(CharacterInfo obj, int impactId)
        {
            if (null != obj)
            {
                CharacterView view = EntityManager.Instance.GetCharacterViewById(obj.GetId());
                if (null != view)
                {
                    ImpactInfo impactInfo = obj.GetSkillStateInfo().GetImpactInfoById(impactId);
                    if (null != impactInfo)
                    {
                        ImpactLogic_SuperArmorShield.SuperArmorShieldInfo info = impactInfo.LogicDatas.GetData<ImpactLogic_SuperArmorShield.SuperArmorShieldInfo>();
                        if (null != info)
                        {
                            GfxSystem.DestroyGameObject(info.EffectActor);
                            GfxSystem.PublishGfxEvent("ge_end_monster_sheild", "ui", view.Actor);
                        }
                    }
                }
            }
        }
        #endregion
        public static ImpactViewManager Instance
        {
            get
            {
                return s_Instance;
            }
        }
        private static ImpactViewManager s_Instance = new ImpactViewManager();
    }
}

