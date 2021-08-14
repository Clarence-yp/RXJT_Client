using System;
using System.Collections.Generic;
using UnityEngine;
using DashFire;

namespace StoryDlg
{
  class StoryDlgManager
  {
    public void Init()
    {
      DashFire.RoleInfo ri = DashFire.LobbyClient.Instance.CurrentRole;
      if (ri != null) {
        if (ri.HeroId == 1) {
          herostr = "kuangzhan";
        } else {
          herostr = "cike";
        }
        heroname = ri.Nickname;
      }
      m_StoryInfos.Clear();
      DBC dlgCfg = new DBC();
      if (dlgCfg.Load(HomePath.GetAbsolutePath(FilePathDefine_Client.C_DialogConfig))) {
        for (int index = 0; index < dlgCfg.RowNum; index++) {
          DBC_Row node = dlgCfg.GetRowByIndex(index);
          if (null != node) {
            StoryDlgInfo info = new StoryDlgInfo();
            info.ID = DBCUtil.ExtractNumeric<int>(node, "Id", 0, true);
            info.StoryName = DBCUtil.ExtractString(node, "StoryName", "", true);
            info.DlgType = (StoryDlgPanel.StoryDlgType)DBCUtil.ExtractNumeric<int>(node, "DlgType", 0, true);
            info.IntervalTime = DBCUtil.ExtractNumeric<float>(node, "IntervalTime", 1.0f, true);
            info.StoryItems = BuildStoryItems(info.StoryName);
            if (!m_StoryInfos.ContainsKey(info.ID))
              m_StoryInfos.Add(info.ID, info);
          }
        }
      }
    }

    public bool ExistStory(int id)
    {
      return m_StoryInfos.ContainsKey(id);
    }
    public StoryDlgInfo GetStoryInfoByID(int id)
    {
      if (m_StoryInfos.ContainsKey(id)) {
        return m_StoryInfos[id];
      }
      return null;
    }
    public void AddStoryEndListener(StoryEndDelegate handler)
    {
      if (!m_StoryEndListeners.Contains(handler)) {
        m_StoryEndListeners.Add(handler);
      }
    }
    public void AddStoryStartListener(StoryStartDelegate handler)
    {
      if (!m_StoryStartListeners.Contains(handler)) {
        m_StoryStartListeners.Add(handler);
      }
    }

    public void ClearListener()
    {
      m_StoryStartListeners.Clear();
      m_StoryEndListeners.Clear();
    }
    public void FireStoryEndMsg(int id)
    {
      foreach (StoryEndDelegate listener in m_StoryEndListeners) {
        listener(id);
      }
    }
    public void FireStoryStartMsg()
    {
      foreach (StoryStartDelegate listener in m_StoryStartListeners) {
        listener();
      }
    }
    private List<StoryDlgItem> BuildStoryItems(string storyName)
    {
      DBC storyItems = new DBC();
      string str = String.Format("{0}{1}.txt", FilePathDefine_Client.C_DialogPath, storyName.Trim());
      if (storyItems.Load(HomePath.GetAbsolutePath(str))) {
        List<StoryDlgItem> itemList = new List<StoryDlgItem>();
        for (int index = 0; index < storyItems.RowNum; index++) {
          DBC_Row node = storyItems.GetRowByIndex(index);
          if (null != node) {
            StoryDlgItem item = new StoryDlgItem();
            item.Number = DBCUtil.ExtractNumeric<int>(node, "Number", 0, true);
            item.IntervalTime = DBCUtil.ExtractNumeric<float>(node, "IntervalTime", 0f, false);
            item.SpeakerName = DBCUtil.ExtractString(node, "SpeakerName", "", true);
            if (item.SpeakerName.Contains("player")) {
              item.SpeakerName = item.SpeakerName.Replace("player", heroname);
            }
            item.ImageLeftAtlas = DBCUtil.ExtractString(node, "ImageLeftAtlas", "", false);
            item.ImageLeft = DBCUtil.ExtractString(node, "ImageLeft", "", true);
            if (item.ImageLeft.Contains("player")) {
              item.ImageLeft = item.ImageLeft.Replace("player", herostr);
            }
            item.ImageLeftBig = string.Format("{0}_big", item.ImageLeft);
            item.ImageLeftSmall = string.Format("{0}_small", item.ImageLeft);
            item.ImageRightAtlas = DBCUtil.ExtractString(node, "ImageRightAtlas", "", false);
            item.ImageRight = DBCUtil.ExtractString(node, "ImageRight", "", true);
            if (item.ImageRight.Contains("player")) {
              item.ImageRight = item.ImageRight.Replace("player", herostr);
            }
            item.ImageRightBig = string.Format("{0}_big", item.ImageRight);
            item.ImageRightSmall = string.Format("{0}_small", item.ImageRight);
            item.Words = DBCUtil.ExtractString(node, "Words", "", true);
            if (item.Words.Contains("player")) {
              item.Words = item.Words.Replace("player", heroname);
            }
            itemList.Add(item);
          }
        }
        return itemList;
      }
      return null;
    }
    private string herostr = "";
    private string heroname = "";
    private Dictionary<int, StoryDlgInfo> m_StoryInfos = new Dictionary<int, StoryDlgInfo>();
    public delegate void StoryEndDelegate(int storyId);
    private List<StoryEndDelegate> m_StoryEndListeners = new List<StoryEndDelegate>();
    public delegate void StoryStartDelegate();
    private List<StoryStartDelegate> m_StoryStartListeners = new List<StoryStartDelegate>();
    static private StoryDlgManager m_Instance = new StoryDlgManager();
    static public StoryDlgManager Instance
    {
      get { return m_Instance; }
    }
  }
}
