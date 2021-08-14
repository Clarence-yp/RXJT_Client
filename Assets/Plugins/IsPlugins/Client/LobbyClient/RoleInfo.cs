using System;
using System.Collections.Generic;

namespace DashFire
{
    public class RoleInfo
    {
        public UserInfo GetPlayerSelfInfo()
        {
            return WorldSystem.Instance.GetPlayerSelf();
        }
        public void FightingScoreChangeCB(float score)
        {
            if (!Geometry.IsSameFloat(FightingScore, score))
            {
                FightingScore = score;
                Network.LobbyNetworkSystem.Instance.UpdateFightingScore(score);
            }
        }
        public ItemDataInfo GetItemData(int item_id, int random_property)
        {
            if (null != m_Items && m_Items.Count > 0)
            {
                bool ret = false;
                ItemDataInfo exist_info = null;
                foreach (ItemDataInfo info in m_Items)
                {
                    if (info.ItemId == item_id && random_property == info.RandomProperty)
                    {
                        exist_info = info;
                        ret = true;
                        break;
                    }
                }
                if (ret)
                {
                    ItemDataInfo item_info = new ItemDataInfo();
                    item_info.ItemConfig = ItemConfigProvider.Instance.GetDataById(item_id);
                    item_info.Level = exist_info.Level;
                    item_info.ItemNum = exist_info.ItemNum;
                    item_info.RandomProperty = exist_info.RandomProperty;
                    if (null != item_info.ItemConfig)
                    {
                        return item_info;
                    }
                }
            }
            return null;
        }
        public void DelItemData(int item_id, int random_property)
        {
            if (null != m_Items && m_Items.Count > 0)
            {
                for (int i = m_Items.Count - 1; i >= 0; i--)
                {
                    if (m_Items[i].ItemId == item_id
                      && random_property == m_Items[i].RandomProperty)
                    {
                        m_Items.RemoveAt(i);
                        break;
                    }
                }
            }
        }
        public void ReduceItemData(int item_id, int random_property)
        {
            if (null != m_Items && m_Items.Count > 0)
            {
                for (int i = m_Items.Count - 1; i >= 0; i--)
                {
                    if (m_Items[i].ItemId == item_id
                      && m_Items[i].RandomProperty == random_property)
                    {
                        if (m_Items[i].ItemNum > 1)
                        {
                            m_Items[i].ItemNum -= 1;
                        }
                        else
                        {
                            m_Items.RemoveAt(i);
                        }
                        break;
                    }
                }
            }
        }
        public void ReduceItemData(int item_id, int random_property, int num)
        {
            if (null != m_Items && m_Items.Count > 0)
            {
                for (int i = m_Items.Count - 1; i >= 0; i--)
                {
                    if (m_Items[i].ItemId == item_id
                      && m_Items[i].RandomProperty == random_property)
                    {
                        int residue_num = m_Items[i].ItemNum - num;
                        if (residue_num >= 0)
                        {
                            if (residue_num > 1)
                            {
                                m_Items[i].ItemNum -= num;
                            }
                            else
                            {
                                m_Items.RemoveAt(i);
                            }
                        }
                        break;
                    }
                }
            }
        }
        public void AddItemData(ItemDataInfo item)
        {
            if (null != item && null != m_Items)
            {
                bool isHave = false;
                for (int i = 0; i < m_Items.Count; i++)
                {
                    if (m_Items[i].ItemId == item.ItemId && m_Items[i].RandomProperty == item.RandomProperty
                      && null != item.ItemConfig && item.ItemConfig.m_MaxStack > 1)
                    {
                        m_Items[i].ItemNum += 1;
                        isHave = true;
                        break;
                    }
                }
                if (!isHave)
                {
                    m_Items.Add(item);
                }
            }
        }
        public void AddItemData(ItemDataInfo item, int num)
        {
            if (null != item && null != m_Items)
            {
                bool isHave = false;
                for (int i = 0; i < m_Items.Count; i++)
                {
                    if (m_Items[i].ItemId == item.ItemId && m_Items[i].RandomProperty == item.RandomProperty
                      && null != item.ItemConfig && item.ItemConfig.m_MaxStack > 1)
                    {
                        m_Items[i].ItemNum += num;
                        isHave = true;
                        break;
                    }
                }
                if (!isHave)
                {
                    m_Items.Add(item);
                }
            }
        }
        public void SetEquip(int pos, ItemDataInfo info)
        {
            if (null != m_Equips && m_Equips.Length > 0)
            {
                for (int i = 0; i < m_Equips.Length; i++)
                {
                    if (i == pos)
                    {
                        m_Equips[i] = info;
                        break;
                    }
                }
            }
        }
        public void DeleteEquip(int equip_id)
        {
            if (null != m_Equips && m_Equips.Length > 0)
            {
                for (int i = 0; i < m_Equips.Length; i++)
                {
                    if (null != m_Equips[i] && m_Equips[i].ItemId == equip_id)
                    {
                        m_Equips[i] = null;
                        break;
                    }
                }
            }
        }
        public void ClearEquip()
        {
            if (null != m_Equips && m_Equips.Length > 0)
            {
                for (int i = 0; i < m_Equips.Length; i++)
                {
                    m_Equips[i] = null;
                }
            }
        }
        public ItemDataInfo GetLegacyData(int index)
        {
            ItemDataInfo info = null;
            for (int i = 0; i < m_Legacys.Length; i++)
            {
                if (i == index)
                {
                    info = m_Legacys[i];
                    break;
                }
            }
            return info;
        }
        public void SetLegacy(int pos, ItemDataInfo info)
        {
            if (null != m_Legacys && m_Legacys.Length > 0)
            {
                for (int i = 0; i < m_Legacys.Length; i++)
                {
                    if (i == pos)
                    {
                        m_Legacys[i] = info;
                        break;
                    }
                }
            }
        }

        public void SetSceneInfo(int sceneId, int grade)
        {
            if (m_SceneInfo.ContainsKey(sceneId))
            {
                if (m_SceneInfo[sceneId] < grade)
                {
                    m_SceneInfo[sceneId] = grade;
                }
            }
            else
            {
                m_SceneInfo.Add(sceneId, grade);
            }
        }
        public Dictionary<int, int> SceneInfo {
            get { return m_SceneInfo; }
        }
        public List<int> NewbieGuides {
            get { return m_NewbieGuides; }
            set { m_NewbieGuides = value; }
        }
        public int GetSceneInfo(int sceneId)
        {
            if (m_SceneInfo.ContainsKey(sceneId))
            {
                return m_SceneInfo[sceneId];
            }
            return 0;
        }
        public void AddCompletedSceneCount(int sceneId, int count = 1)
        {
            if (m_ScenesCompletedCountData.ContainsKey(sceneId))
            {
                m_ScenesCompletedCountData[sceneId]++;
            }
            else
            {
                m_ScenesCompletedCountData.Add(sceneId, 1);
            }
        }
        public int GetCompletedSceneCount(int sceneId)
        {
            if (m_ScenesCompletedCountData.ContainsKey(sceneId))
            {
                return m_ScenesCompletedCountData[sceneId];
            }
            else
            {
                return 0;
            }
        }
        public void ResetNewEquipCache()
        {
            if (null != m_NewEquipCache)
            {
                m_NewEquipCache.Clear();
            }
        }
        public void AddToNewEquipCache(int item_id, int property_id)
        {
            if (null != m_NewEquipCache && item_id > 0)
            {
                NewEquipInfo info = new NewEquipInfo();
                info.ItemId = item_id;
                info.ItemRandomProperty = property_id;
                m_NewEquipCache.Add(info);
            }
        }
        public void RemoveMailByGuid(ulong mail_guid)
        {
            MailInfo mi = null;
            foreach (MailInfo info in m_MailInfos)
            {
                if (info.m_MailGuid == mail_guid)
                {
                    mi = info;
                    break;
                }
            }
            if (null != mi)
            {
                m_MailInfos.Remove(mi);
            }
        }
        public Dictionary<int, int> ScenesCompletedCountData {
            get { return m_ScenesCompletedCountData; }
        }
        public ulong Guid {
            get { return m_Guid; }
            set { m_Guid = value; }
        }
        public string Nickname {
            get { return m_Nickname; }
            set { m_Nickname = value; }
        }
        public int NewBieGuideScene {
            get { return m_NewBieGuideScene; }
            set { m_NewBieGuideScene = value; }
        }
        public int HeroId {
            get { return m_HeroId; }
            set { m_HeroId = value; }
        }
        public int Level {
            get { return m_Level; }
            set { m_Level = value; }
        }
        public int Money {
            get { return m_Money; }
            set { m_Money = value; }
        }
        public int Gold {
            get { return m_Gold; }
            set { m_Gold = value; }
        }
        public List<ItemDataInfo> Items {
            get { return m_Items; }
        }
        public ItemDataInfo[] Equips {
            get { return m_Equips; }
        }
        public List<SkillInfo> SkillInfos {
            get { return m_SkillInfos; }
        }
        public GowInfo Gow {
            get { return m_GowInfo; }
        }
        public List<MailInfo> MailInfos {
            get { return m_MailInfos; }
        }
        public int CurPresetIndex {
            get { return m_CurPresetIndex; }
            set { m_CurPresetIndex = value; }
        }
        public int Exp {
            get { return m_Exp; }
            set { m_Exp = value; }
        }
        public int Vip {
            get { return m_Vip; }
            set { m_Vip = value; }
        }
        public int CitySceneId {
            get { return m_CitySceneId; }
            set { m_CitySceneId = value; }
        }
        // 体力相关
        public int StaminaMax = 120;
        public int CurStamina {
            get { return m_CurStamina; }
            set {
                m_CurStamina = value;
            }
        }
        public int BuyStaminaCount {
            get { return m_BuyStaminaCount; }
            set { m_BuyStaminaCount = value; }
        }
        // 兑换金币相关
        public int BuyMoneyCount {
            get { return m_BuyMoneyCount; }
            set { m_BuyMoneyCount = value; }
        }
        // 出售物品钻石收益
        public int SellItemGoldIncome {
            get { return m_SellItemGoldIncome; }
            set { m_SellItemGoldIncome = value; }
        }
        // 任务
        public MissionStateInfo GetMissionStateInfo()
        {
            return m_MissionStateInfo;
        }
        // 远征
        public ExpeditionPlayerInfo GetExpeditionInfo()
        {
            return m_Expeditioninfo;
        }
        public ItemDataInfo[] Legacys {
            get { return m_Legacys; }
        }
        public Dictionary<ulong, FriendInfo> Friends {
            get { return m_Friends; }
        }
        public List<NewEquipInfo> NewEquipCache {
            get { return m_NewEquipCache; }
        }
        // 角色GUID
        private ulong m_Guid = 0;
        // 角色昵称               
        private string m_Nickname;
        // 新手指导场景
        private int m_NewBieGuideScene;
        // 角色职业               
        private int m_HeroId = 0;
        // 角色等级      
        private int m_Level = 0;
        // 金钱数
        private int m_Money = 0;
        // 钻石数
        private int m_Gold = 0;
        // 体力
        private int m_CurStamina = 0;
        // 经验
        private int m_Exp = 0;
        // vip等级
        private int m_Vip = 0;
        // 所在的主城场景ID
        private int m_CitySceneId = 0;
        // 购买体力计数
        private int m_BuyStaminaCount = 0;
        // 兑换金币计数
        private int m_BuyMoneyCount = 0;
        // 出售物品收益
        private int m_SellItemGoldIncome = 0;
        // 战斗分数
        public float FightingScore { get; set; }
        // 角色物品信息
        private List<ItemDataInfo> m_Items = new List<ItemDataInfo>();
        public const int c_MaxItemNum = 128;
        // 角色装备信息
        private ItemDataInfo[] m_Equips = new ItemDataInfo[EquipmentStateInfo.c_EquipmentCapacity];
        // 新获得装备信息
        private List<NewEquipInfo> m_NewEquipCache = new List<NewEquipInfo>();
        // 角色技能预设信息
        private List<SkillInfo> m_SkillInfos = new List<SkillInfo>();
        // 角色任务信息
        private MissionStateInfo m_MissionStateInfo = new MissionStateInfo();
        // 神器信息
        private ItemDataInfo[] m_Legacys = new ItemDataInfo[LegacyStateInfo.c_LegacyCapacity];
        // 通关信息
        private Dictionary<int, int> m_SceneInfo = new Dictionary<int, int>();
        // 通关次数
        private Dictionary<int, int> m_ScenesCompletedCountData = new Dictionary<int, int>();
        // 教学信息
        private List<int> m_NewbieGuides = new List<int>();
        // 远征信息
        private ExpeditionPlayerInfo m_Expeditioninfo = new ExpeditionPlayerInfo();
        // 预设索引
        private int m_CurPresetIndex = 0;
        // 战神赛信息
        private GowInfo m_GowInfo = new GowInfo();
        // 邮件信息
        private List<MailInfo> m_MailInfos = new List<MailInfo>();
        // 好友信息
        private Dictionary<ulong, FriendInfo> m_Friends = new Dictionary<ulong, FriendInfo>();
    }
}
