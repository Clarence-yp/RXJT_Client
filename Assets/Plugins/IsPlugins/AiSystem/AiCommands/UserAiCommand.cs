using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DashFire
{
  public class UserFaceCommand : AbstractUserAiCommand<UserFaceCommand>
  {
    public override bool Execute(long deltaTime)
    {
      if (0 == m_Count) {
        Logic.NotifyUserFace(User);
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
  public class UserSkillCommand : AbstractUserAiCommand<UserSkillCommand>
  {
    public override bool Execute(long deltaTime)
    {
      Logic.NotifyUserSkill(User, m_SkillId);
      return true;
    }
    public void Init(int skillId)
    {
      m_SkillId = skillId;
    }
    
    private int m_SkillId = 0;
  }
}
