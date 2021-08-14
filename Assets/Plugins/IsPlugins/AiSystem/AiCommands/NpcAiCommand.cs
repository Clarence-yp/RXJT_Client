using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DashFire
{
  public class NpcFaceCommand : AbstractNpcAiCommand<NpcFaceCommand>
  {
    public override bool Execute(long deltaTime)
    {
      if (0 == m_Count) {
        Logic.NotifyNpcFace(Npc, Npc.GetMovementStateInfo().GetFaceDir());
        ++m_Count;
        return false;
      } else {
        return true;
      }
    }
    public void Init()
    {
      m_Count = 0;
    }

    private long m_Count = 0;
  }
  public class NpcSkillCommand : AbstractNpcAiCommand<NpcSkillCommand>
  {
    public override bool Execute(long deltaTime)
    {
      Logic.NotifyNpcSkill(Npc, m_SkillId);
      return true;
    }
    public void Init(int skillId)
    {
      m_SkillId = skillId;
    }

    private int m_SkillId = 0;
  }
}
