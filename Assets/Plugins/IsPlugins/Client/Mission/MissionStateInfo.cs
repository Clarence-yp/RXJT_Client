using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DashFire
{

    public enum MissionStateType
    {
        UNCOMPLETED = 0,
        COMPLETED = 1,
    }

    public enum MissionOperationType
    {
        ADD,
        FINISH,
        DELETE,
        UPDATA,
    }
    public class MissionStateInfo
    {

        public bool AddMission(int id, MissionStateType type)
        {
            switch (type)
            {
                case MissionStateType.UNCOMPLETED:
                    AddUnCompletedMission(id);
                    return true;
                case MissionStateType.COMPLETED:
                    AddCompletedMission(id);
                    return true;
                default:
                    return false;
            }
        }

        private bool AddUnCompletedMission(int id)
        {
            LogSystem.Debug("Add uncompleted mission {0}", id);
            if (!m_UnCompletedMissions.ContainsKey(id))
            {
                m_UnCompletedMissions.Add(id, new MissionInfo(id));
                return true;
            }
            return false;
        }

        private bool AddCompletedMission(int id)
        {
            LogSystem.Debug("Add completed mission {0}", id);
            if (!m_CompletedMissions.ContainsKey(id))
            {
                m_CompletedMissions.Add(id, new MissionInfo(id));
                return true;
            }
            return false;
        }
        public void UpdateMissionState(int id, int param0, int param1, bool isCompleted)
        {
            if (m_UnCompletedMissions.ContainsKey(id))
            {
                m_UnCompletedMissions[id].Param0 = param0;
                m_UnCompletedMissions[id].Param1 = param1;
            }
            if (isCompleted)
            {
                if (!m_CompletedMissions.ContainsKey(id))
                {
                    m_CompletedMissions.Add(id, m_UnCompletedMissions[id]);
                    m_UnCompletedMissions.Remove(id);
                }
            }
        }

        public bool UnlockMission(int id)
        {
            LogSystem.Debug("Unlock mission {0}", id);
            if (!m_UnCompletedMissions.ContainsKey(id))
            {
                m_UnCompletedMissions.Add(id, new MissionInfo(id));
                return true;
            }
            return false;
        }

        public bool CompletedMission(int id)
        {
            LogSystem.Debug("Completed mission {0}", id);
            if (m_UnCompletedMissions.ContainsKey(id) && !m_CompletedMissions.ContainsKey(id))
            {
                m_CompletedMissions.Add(id, m_UnCompletedMissions[id]);
                m_UnCompletedMissions.Remove(id);
                return true;
            }
            return false;
        }
        public void RemoveCompletedMission(int id)
        {
            if (m_CompletedMissions.ContainsKey(id))
            {
                m_CompletedMissions.Remove(id);
            }
        }
        public void ResetDailyMissions()
        {
            foreach (int missionId in MissionConfigProvider.Instance.GetDailyMissionId())
            {
                if (m_UnCompletedMissions.ContainsKey(missionId))
                {
                    m_UnCompletedMissions.Remove(missionId);
                }
                else if (m_CompletedMissions.ContainsKey(missionId))
                {
                    m_CompletedMissions.Remove(missionId);
                }
            }
            foreach (int missionId in MissionConfigProvider.Instance.GetDailyMissionId())
            {
                AddMission(missionId, MissionStateType.UNCOMPLETED);
            }
        }
        public int GetMissionsExpReward(int missionId, int userLevel)
        {
            int result = 0;
            MissionConfig mc = MissionConfigProvider.Instance.GetDataById(missionId);
            if (null != mc)
            {
                Data_SceneDropOut dropOutConfig = SceneConfigProvider.Instance.GetSceneDropOutById(mc.DropId);
                if (null != dropOutConfig)
                {
                    if (mc.MissionType == (int)MissionType.DAILY && dropOutConfig.m_Exp > 0)
                    {
                        if (userLevel < 19)
                        {
                            userLevel = 19;
                        }
                        result = (int)(0.1567 * Math.Pow(userLevel, 4) - 24.201 * Math.Pow(userLevel, 3) + 1303.2 * Math.Pow(userLevel, 2) - 26052 * userLevel + 173090);
                    }
                    else
                    {
                        result = dropOutConfig.m_Exp;
                    }
                }
            }
            return result;
        }
        public void Clear()
        {
            m_UnCompletedMissions.Clear();
            m_CompletedMissions.Clear();
        }
        public Dictionary<int, MissionInfo> UnCompletedMissions {
            get { return m_UnCompletedMissions; }
        }
        public Dictionary<int, MissionInfo> CompletedMissions {
            get { return m_CompletedMissions; }
        }

        private Dictionary<int, MissionInfo> m_UnCompletedMissions = new Dictionary<int, MissionInfo>();
        private Dictionary<int, MissionInfo> m_CompletedMissions = new Dictionary<int, MissionInfo>();
    }
}
