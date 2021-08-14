using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DashFire
{
  public sealed class AiCommandDispatcher
  {
    public void NpcFace(NpcInfo npc, AbstractNpcStateLogic logic)
    {
      NpcFaceCommand cmd = m_NpcFaceCommandPool.Alloc();
      if (null != cmd) {
        cmd.SetContext(npc, logic);
        cmd.Init();
        npc.GetAiStateInfo().CommandQueue.Enqueue(cmd);
      }
    }
    public void NpcSkill(NpcInfo npc, AbstractNpcStateLogic logic, int skillId)
    {
      NpcSkillCommand cmd = m_NpcSkillCommandPool.Alloc();
      if (null != cmd) {
        cmd.SetContext(npc, logic);
        cmd.Init(skillId);
        npc.GetAiStateInfo().CommandQueue.Enqueue(cmd);
      }
    }
    public void UserFace(UserInfo user, AbstractUserStateLogic logic)
    {
      UserFaceCommand cmd = m_UserFaceCommandPool.Alloc();
      if (null != cmd) {
        cmd.SetContext(user, logic);
        cmd.Init();
        user.GetAiStateInfo().CommandQueue.Enqueue(cmd);
      }
    }
    public void UserSkill(UserInfo user, AbstractUserStateLogic logic, int skillId)
    {
      UserSkillCommand cmd = m_UserSkillCommandPool.Alloc();
      if (null != cmd) {
        cmd.SetContext(user, logic);
        cmd.Init(skillId);
        user.GetAiStateInfo().CommandQueue.Enqueue(cmd);
      }
    }
    public AiCommandDispatcher()
    {
      if (GlobalVariables.Instance.IsClient) {
        m_NpcFaceCommandPool.Init(16);
        m_NpcSkillCommandPool.Init(16);
        m_UserFaceCommandPool.Init(16);
        m_UserSkillCommandPool.Init(16);
      } else {
        m_NpcFaceCommandPool.Init(256);
        m_NpcSkillCommandPool.Init(256);
        m_UserFaceCommandPool.Init(256);
        m_UserSkillCommandPool.Init(256);
      }
    }

    private ObjectPool<NpcFaceCommand> m_NpcFaceCommandPool = new ObjectPool<NpcFaceCommand>();
    private ObjectPool<NpcSkillCommand> m_NpcSkillCommandPool = new ObjectPool<NpcSkillCommand>();
    private ObjectPool<UserFaceCommand> m_UserFaceCommandPool = new ObjectPool<UserFaceCommand>();
    private ObjectPool<UserSkillCommand> m_UserSkillCommandPool = new ObjectPool<UserSkillCommand>();
  }
}
