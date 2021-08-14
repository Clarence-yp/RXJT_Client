using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DashFire
{
  public sealed class AiLogicManager
  {
    public INpcStateLogic GetNpcStateLogic(int id)
    {
      INpcStateLogic logic = null;
      if (m_NpcStateLogics.ContainsKey(id))
        logic = m_NpcStateLogics[id];
      return logic;
    }
    public IUserStateLogic GetUserStateLogic(int id)
    {
      IUserStateLogic logic = null;
      if (m_UserStateLogics.ContainsKey(id))
        logic = m_UserStateLogics[id];
      return logic;
    }
    private AiLogicManager()
    {
      //这里初始化所有的Ai状态逻辑，并记录到对应的列表(客户端的逻辑因为通常比较简单，很多会使用通用的ai逻辑)
      if (GlobalVariables.Instance.IsClient) {
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_General, new AiLogic_Npc_General());
        m_NpcStateLogics.Add((int)AiStateLogicId.DropOut_AutoPick, new AiLogic_DropOut_AutoPick());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_OneSkill, new AiLogic_Npc_OneSkill());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_Bluelf01, new AiLogic_Npc_Bluelf01());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_Bluelf02, new AiLogic_Npc_Bluelf02());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_Bluelf03, new AiLogic_Npc_Bluelf03());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_Blof01, new AiLogic_Npc_Blof01());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_Blof02, new AiLogic_Npc_Blof02());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_SmallMouse, new AiLogic_Npc_SmallMouse());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_BluelfBoss, new AiLogic_Npc_BluelfBoss());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_BossXiLie, new AiLogic_Npc_BossXiLie());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_BossDevilWarrior, new AiLogic_Npc_BossDevilWarrior());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_BossCommon, new AiLogic_Npc_CommonBoss());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_BossHulun, new AiLogic_Npc_BossHulun());

        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_CommonMelee, new AiLogic_Npc_CommonMelee());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_CommonRange, new AiLogic_Npc_CommonRange());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_CommonLittleBoss, new AiLogic_Npc_CommonLittleBoss());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_LittleBossWithSuperArmor, new AiLogic_Npc_LittleBossWithSuperArmor());
        //-------------------------------------------------------------------------------------
        AiLogic_User_Client userLogic = new AiLogic_User_Client();
        m_UserStateLogics.Add((int)AiStateLogicId.UserMirror_General, new AiLogic_UserMirror_General());
        m_UserStateLogics.Add((int)AiStateLogicId.PvpUser_General, userLogic);
        m_UserStateLogics.Add((int)AiStateLogicId.UserSelf_General, new AiLogic_UserSelf_General());
        m_UserStateLogics.Add((int)AiStateLogicId.UserSelfRange_General, new AiLogic_UserSelfRange_General());
      } else {
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_General, new AiLogic_Npc_General());
        m_NpcStateLogics.Add((int)AiStateLogicId.DropOut_AutoPick, new AiLogic_DropOut_AutoPick());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_OneSkill, new AiLogic_Npc_OneSkill());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_Bluelf01, new AiLogic_Npc_Bluelf01());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_Bluelf02, new AiLogic_Npc_Bluelf02());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_Bluelf03, new AiLogic_Npc_Bluelf03());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_SmallMouse, new AiLogic_Npc_SmallMouse());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_Blof01, new AiLogic_Npc_Blof01());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_Blof02, new AiLogic_Npc_Blof02());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_BluelfBoss, new AiLogic_Npc_BluelfBoss());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_BossXiLie, new AiLogic_Npc_BossXiLie());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_BossDevilWarrior, new AiLogic_Npc_BossDevilWarrior());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_BossCommon, new AiLogic_Npc_CommonBoss());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_BossHulun, new AiLogic_Npc_BossHulun());

        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_CommonMelee, new AiLogic_Npc_CommonMelee());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_CommonRange, new AiLogic_Npc_CommonRange());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_CommonLittleBoss, new AiLogic_Npc_CommonLittleBoss());
        m_NpcStateLogics.Add((int)AiStateLogicId.Npc_LittleBossWithSuperArmor, new AiLogic_Npc_LittleBossWithSuperArmor());
        //-------------------------------------------------------------------------------------
        m_UserStateLogics.Add((int)AiStateLogicId.UserMirror_General, new AiLogic_UserMirror_General());
        m_UserStateLogics.Add((int)AiStateLogicId.UserSelf_General, new AiLogic_UserSelf_General());
        m_UserStateLogics.Add((int)AiStateLogicId.UserSelfRange_General, new AiLogic_UserSelfRange_General());
      }
    }
    private Dictionary<int, INpcStateLogic> m_NpcStateLogics = new Dictionary<int, INpcStateLogic>();
    private Dictionary<int, IUserStateLogic> m_UserStateLogics = new Dictionary<int, IUserStateLogic>();

    public static AiLogicManager Instance
    {
      get { return s_AiLogicManager; }
    }
    private static AiLogicManager s_AiLogicManager = new AiLogicManager();
  }
}
