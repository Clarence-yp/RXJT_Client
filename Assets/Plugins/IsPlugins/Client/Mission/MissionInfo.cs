using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DashFire {
  public class MissionInfo {

    public enum MissionType {
      MAIN_LINE,
      CHALLENGE,
    }

    public MissionInfo(int id) {
      m_MissionId = id;
      m_Config = MissionConfigProvider.Instance.GetDataById(m_MissionId);
      if (null != m_Config) {
        m_MissionType = (MissionType)m_Config.MissionType;
      } else {
        LogSystem.Warn("MissionInfo:: can not find mission {0}", id);
      }
    }

    public int MissionId {
      get { return m_MissionId; }
    }

    public MissionConfig Config{
      get { return m_Config; }
    }

    public int Param0 {
      get { return m_Param0; }
      set { m_Param0 = value; }
    }
    public int Param1 {
      get { return m_Param1; }
      set { m_Param1 = value; }
    }

    public string Progress {
      get { return m_Progress; }
      set { m_Progress = value; }
    }

    private int m_MissionId;
    private MissionType m_MissionType;
    private MissionConfig m_Config;
    private int m_Param0;
    private int m_Param1;
    private string m_Progress;
  }
}
