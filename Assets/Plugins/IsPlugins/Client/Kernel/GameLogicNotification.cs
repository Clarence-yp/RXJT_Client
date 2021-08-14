using System;
using System.Collections.Generic;

namespace DashFire
{
  public class GameLogicNotification : IGameLogicNotification
  {
    public void OnGfxStartStory(int id)
    {
      if (WorldSystem.Instance.IsPveScene()) {
        ClientStorySystem.Instance.StartStory(id);
      } else {
        // OnGfxStartStory 只会在单人pve场景中被调用
      }
    }
    public void OnGfxSendStoryMessage(string msgId, object[] args)
    {
      if (WorldSystem.Instance.IsPureClientScene() || WorldSystem.Instance.IsPveScene()) {
        ClientStorySystem.Instance.SendMessage(msgId, args);
      } else {
        //通知服务器
        string msgIdPrefix = "dialogover:";
        if (msgId.StartsWith(msgIdPrefix)) {
          DashFireMessage.Msg_CR_DlgClosed msg = new DashFireMessage.Msg_CR_DlgClosed();
          msg.dialog_id = int.Parse(msgId.Substring(msgIdPrefix.Length));
          Network.NetworkSystem.Instance.SendMessage(msg);
        }
      }
    }
    public void OnGfxControlMoveStart(int objId, int id, bool isSkill)
    {
      CharacterInfo charObj = WorldSystem.Instance.GetCharacterById(objId);
      if (null != charObj) {
        charObj.GetMovementStateInfo().IsSkillMoving = true;
        if (WorldSystem.Instance.IsPvpScene() || WorldSystem.Instance.IsMultiPveScene()) {
          if (objId == WorldSystem.Instance.PlayerSelfId || charObj.OwnerId == WorldSystem.Instance.PlayerSelfId) {
            Network.NetworkSystem.Instance.SyncGfxMoveControlStart(charObj, id, isSkill);
          }
        }
      }
    }
    public void OnGfxControlMoveStop(int objId, int id, bool isSkill)
    {
      CharacterInfo charObj = WorldSystem.Instance.GetCharacterById(objId);
      if (null != charObj) {
        charObj.GetMovementStateInfo().IsSkillMoving = false;
        if (WorldSystem.Instance.IsPvpScene() || WorldSystem.Instance.IsMultiPveScene()) {
          if (objId == WorldSystem.Instance.PlayerSelfId || charObj.OwnerId == WorldSystem.Instance.PlayerSelfId) {
            Network.NetworkSystem.Instance.SyncGfxMoveControlStop(charObj, id, isSkill);
          }
        }
      }
    }
    public void OnGfxHitTarget(int id, int impactId, int targetId, int hitCount, int skillId, int duration, float x, float y, float z, float dir)
    {
      CharacterInfo sender = WorldSystem.Instance.GetCharacterById(id);
      CharacterInfo target = WorldSystem.Instance.GetCharacterById(targetId);
      UserInfo playerSelf = WorldSystem.Instance.GetPlayerSelf();
      bool hitCountChanged = false;
      if (id == WorldSystem.Instance.PlayerSelfId && null != playerSelf) {
        long curTime = TimeUtility.GetLocalMilliseconds();
        if (hitCount > 0) {
          CombatStatisticInfo combatInfo = playerSelf.GetCombatStatisticInfo();
          long lastHitTime = combatInfo.LastHitTime;
          if (0 == lastHitTime || lastHitTime + 1500 > curTime) {
            combatInfo.MultiHitCount = combatInfo.MultiHitCount + hitCount;
          }
          if (combatInfo.MaxMultiHitCount < combatInfo.MultiHitCount) {
            combatInfo.MaxMultiHitCount = combatInfo.MultiHitCount;
            hitCountChanged = true;
          }
          combatInfo.LastHitTime = curTime;
          if (combatInfo.MultiHitCount > 1) {
            GfxSystem.PublishGfxEvent("ge_hitcount", "ui", combatInfo.MultiHitCount);
          }
        }
      }
      if (targetId == WorldSystem.Instance.PlayerSelfId && null != playerSelf) {
        if (hitCount > 0) {
          CombatStatisticInfo combatInfo = playerSelf.GetCombatStatisticInfo();
          combatInfo.HitCount += hitCount;
          hitCountChanged = true;
          if (WorldSystem.Instance.IsELiteScene()) {
            RoleInfo roleInfo = LobbyClient.Instance.CurrentRole;
            SceneResource curScene = WorldSystem.Instance.GetCurScene();
            if(null != roleInfo && null != curScene && roleInfo.GetSceneInfo(WorldSystem.Instance.GetCurSceneId()) == 2){ //当前在挑战3星通关
              GfxSystem.PublishGfxEvent("ge_pve_fightinfo", "ui", 0, combatInfo.HitCount, curScene.SceneConfig.m_CompletedHitCount, 0);
            }
          }
        }
      }
      if (hitCountChanged && null != playerSelf && (WorldSystem.Instance.IsPvpScene() || WorldSystem.Instance.IsMultiPveScene())) {
        CombatStatisticInfo combatInfo = playerSelf.GetCombatStatisticInfo();
        DashFireMessage.Msg_CR_HitCountChanged msg = new DashFireMessage.Msg_CR_HitCountChanged();
        msg.max_multi_hit_count = combatInfo.MaxMultiHitCount;
        msg.hit_count = combatInfo.HitCount;
        Network.NetworkSystem.Instance.SendMessage(msg);
      }
      if (null !=sender && null != target) {
        OnGfxStartImpact(sender.GetId(), impactId, target.GetId(), skillId, duration, new ScriptRuntime.Vector3(x, y, z), dir);
      }
    }

    public void OnGfxMoveMeetObstacle(int id)
    {
      CharacterInfo charObj = WorldSystem.Instance.GetCharacterById(id);
      if (null != charObj) {
        charObj.GetMovementStateInfo().IsMoveMeetObstacle = true;
        WorldSystem.Instance.NotifyMoveMeetObstacle(false);
      }
    }

    public void OnGfxStartImpact(int sender, int impactId, int target, int skillId, int duration, ScriptRuntime.Vector3 srcPos, float dir) {
      CharacterInfo senderObj = WorldSystem.Instance.GetCharacterById(sender);
      if (WorldSystem.Instance.IsPvpScene() || WorldSystem.Instance.IsMultiPveScene()) {
        if (null != senderObj) {
          bool isSend = false;
          if (senderObj.GetId() == WorldSystem.Instance.PlayerSelfId) {
            isSend = true;
          }
          if (senderObj is NpcInfo) {
            if (senderObj.OwnerId == WorldSystem.Instance.GetPlayerSelf().GetId()) {
              isSend = true;
            }
          }
          if (isSend) {
            bool ret = ImpactSystem.Instance.SendImpactToCharacter(senderObj, impactId, target, skillId, duration, srcPos, dir);
            if (ret)
              Network.NetworkSystem.Instance.SyncSendImpact(senderObj, impactId, target, skillId, duration, srcPos, dir);
          }
        }
      } else {
        bool ret = ImpactSystem.Instance.SendImpactToCharacter(senderObj, impactId, target, skillId, duration, srcPos, dir);
      }
    }

    public void OnGfxStartSkill(int id, SkillCategory category, float x, float y, float z)
    {
      CharacterInfo charObj = WorldSystem.Instance.GetCharacterById(id);
      if (null != charObj) {
        if (charObj.SkillController != null) {
          charObj.SkillController.PushSkill(category, new ScriptRuntime.Vector3(x, y, z));
        }
        //LogSystem.Debug("OnGfxStartSkill");
      }
    }

    public void OnGfxForceStartSkill(int id, int skillId)
    {
      CharacterInfo charObj = WorldSystem.Instance.GetCharacterById(id);
      if (null != charObj) {
        if (charObj.SkillController != null) {
          charObj.SkillController.ForceStartSkill(skillId);
        }
        //LogSystem.Debug("OnGfxStartSkill");
      }
    }

    public void OnGfxStopSkill(int id, int skillId)
    {
      CharacterInfo charObj = WorldSystem.Instance.GetCharacterById(id);
      if (null != charObj) {
        if ((WorldSystem.Instance.IsPvpScene() || WorldSystem.Instance.IsMultiPveScene()) && skillId != charObj.Combat2IdleSkill) {
          if (charObj.GetId() == WorldSystem.Instance.PlayerSelfId || charObj.OwnerId == WorldSystem.Instance.PlayerSelfId) {
            LogSystem.Debug("---ongfxstopskill id={0}, skillid=", id, skillId);
            Network.NetworkSystem.Instance.SyncPlayerStopSkill(charObj, skillId);
          }
        }
        if (skillId == charObj.Combat2IdleSkill) {
          CharacterView userview = EntityManager.Instance.GetCharacterViewById(id);
          if (userview != null) {
            userview.OnCombat2IdleSkillOver();
          }
        }
        SkillInfo skillInfo = charObj.GetSkillStateInfo().GetSkillInfoById(skillId);
        if (null != skillInfo) {
          skillInfo.IsSkillActivated = false;
          LogSystem.Debug("-----OnGfxStopSkill " + skillId);
        }
      }
    }

    public void OnGfxStartAttack(int id, float x, float y, float z)
    {
      CharacterInfo charObj = WorldSystem.Instance.GetCharacterById(id);
      if (null != charObj) {
        charObj.SkillController.StartAttack(new ScriptRuntime.Vector3(x, y, z));
      }
    }

    public void OnGfxStopAttack(int id)
    {
      CharacterInfo charObj = WorldSystem.Instance.GetCharacterById(id);
      if (null != charObj) {
        charObj.SkillController.StopAttack();
      }
    }

    public void OnGfxSkillBreakSection(int objid, int skillid, int breaktype, int startime, int endtime, bool isinterrupt)
    {
      CharacterInfo charObj = WorldSystem.Instance.GetCharacterById(objid);
      if (null != charObj) {
        charObj.SkillController.AddBreakSection(skillid, breaktype, startime, endtime, isinterrupt);
      }
    }

    public void OnGfxStopImpact(int id, int impactId) {
      CharacterInfo charObj = WorldSystem.Instance.GetCharacterById(id);
      if (null != charObj) {
        ImpactSystem.Instance.OnGfxStopImpact(charObj, impactId);
      }
    }

    public void OnGfxSetCrossFadeTime(int id, string fadeTargetAnim, float crossTime)
    {
      CharacterInfo charObj = WorldSystem.Instance.GetCharacterById(id);
      if (null != charObj && charObj.GetSkillStateInfo() != null) {
        if (fadeTargetAnim == "stand") {
          charObj.GetSkillStateInfo().CrossToStandTime = crossTime;
        } else if (fadeTargetAnim == "run") {
          charObj.GetSkillStateInfo().CrossToRunTime = crossTime;
        }
      }
    }

    public void OnGfxAddLockInputTime(int id, SkillCategory category, float lockinputtime)
    {
      CharacterInfo charObj = WorldSystem.Instance.GetCharacterById(id);
      if (null != charObj && charObj.GetSkillStateInfo() != null) {
        SkillInfo curskill = charObj.GetSkillStateInfo().GetCurSkillInfo();
        if (curskill != null) {
          curskill.AddLockInputTime(category, lockinputtime);
        }
      }
    }

    public void OnGfxSummonNpc(int owner_id, int owner_skillid, int npc_type_id, string modelPrefab, int skillid, 
                               float x, float y, float z)
    {
      CharacterInfo charObj = WorldSystem.Instance.GetCharacterById(owner_id);
      
      NpcInfo npc_owner = charObj as NpcInfo;
      /*if (npc_owner != null && npc_owner.SummonOwnerId > 0) {
        charObj = WorldSystem.Instance.GetCharacterById(npc_owner.SummonOwnerId);
      }*/
      if (charObj == null) {
        return;
      }

      Data_Unit data = new Data_Unit();
      data.m_Id = -1;
      data.m_LinkId = npc_type_id;
      data.m_CampId = charObj.GetCampId();
      data.m_IsEnable = true;
      data.m_Pos = new ScriptRuntime.Vector3(x, y, z);
      data.m_RotAngle = 0;
      data.m_AiLogic = 0;
      NpcInfo npc = WorldSystem.Instance.NpcManager.AddNpc(data);
      if (!string.IsNullOrEmpty(modelPrefab)) {
        npc.SetModel(modelPrefab);
      }
      SkillInfo skillinfo = new SkillInfo(skillid);
      npc.GetSkillStateInfo().AddSkill(skillinfo);
      npc.SkillController = new SwordManSkillController(npc, GfxModule.Skill.GfxSkillSystem.Instance);
      npc.GetMovementStateInfo().SetPosition(data.m_Pos);
      npc.SummonOwnerId = charObj.GetId();
      EntityManager.Instance.CreateNpcView(npc.GetId());
      charObj.GetSkillStateInfo().AddSummonObject(npc);
      NpcView npcview = EntityManager.Instance.GetNpcViewById(npc.GetId());
      CharacterView owner_view = EntityManager.Instance.GetCharacterViewById(charObj.GetId());
      npcview.ObjectInfo.SummonOwnerActorId = owner_view.Actor;
      npcview.ObjectInfo.SummonOwnerSkillId = owner_skillid;
      owner_view.ObjectInfo.Summons.Add(npcview.Actor);

      npc.SkillController.ForceStartSkill(skillid);
    }

    public void OnGfxDestroyObj(int id)
    {
      CharacterInfo charObj = WorldSystem.Instance.GetCharacterById(id);
      if (charObj == null) {
        return;
      }
      EntityManager.Instance.DestroyNpcView(charObj.GetId());
      WorldSystem.Instance.DestroyCharacterById(charObj.GetId());
    }

    public void OnGfxSetObjLifeTime(int id, long life_remaint_time)
    {
      NpcInfo npcinfo = WorldSystem.Instance.GetCharacterById(id) as NpcInfo;
      if (npcinfo == null) {
        return;
      }
      npcinfo.LifeEndTime = TimeUtility.GetServerMilliseconds() + life_remaint_time;
    }

    public void OnGfxDestroySummonObject(int id)
    {
      CharacterInfo character = WorldSystem.Instance.GetCharacterById(id);
      if (character == null) {
        return;
      }
      List<NpcInfo> summon_pool = character.GetSkillStateInfo().GetSummonObject();
      foreach (NpcInfo so in summon_pool) {
        EntityManager.Instance.DestroyNpcView(so.GetId());
        WorldSystem.Instance.DestroyCharacterById(so.GetId());
      }
      summon_pool.Clear();
    }

    public void OnGfxSimulateMove(int id)
    {
      NpcInfo npc = WorldSystem.Instance.GetCharacterById(id) as NpcInfo;
      if (npc == null) {
        return;
      }
      if (npc.SummonOwnerId < 0) {
        return;
      }
      CharacterInfo owner = WorldSystem.Instance.GetCharacterById(npc.SummonOwnerId);
      if (owner == null) {
        return;
      }
      CharacterView owner_view = EntityManager.Instance.GetCharacterViewById(npc.SummonOwnerId);
      if (owner_view == null) {
        return;
      }
      npc.GetActualProperty().SetMoveSpeed(Operate_Type.OT_Absolute, owner.GetActualProperty().MoveSpeed);
      npc.VelocityCoefficient = owner.VelocityCoefficient;
      npc.Combat2IdleSkill = owner.Combat2IdleSkill;
      npc.Combat2IdleTime = owner.Combat2IdleTime;
      npc.Idle2CombatWeaponMoves = owner.Idle2CombatWeaponMoves;
      List<SkillInfo> skills = owner.GetSkillStateInfo().GetAllSkill();
      npc.GetSkillStateInfo().AddSkill(new SkillInfo(npc.Combat2IdleSkill));
      foreach (SkillInfo si in skills) {
        npc.GetSkillStateInfo().AddSkill(new SkillInfo(si.SkillId));
      }
      npc.SkillController.Init();
      npc.IsSimulateMove = true;
    }

    public static GameLogicNotification Instance
    {
      get { return s_Instance; }
    }
    private static GameLogicNotification s_Instance = new GameLogicNotification();
  }
}
