using UnityEngine;
using DashFire;
using System.Collections;
using System.Collections.Generic;

public class UIArtifactSlot : MonoBehaviour {

  public UILabel lblUnlockHint = null;
  public UISprite spLock = null;
  public UISprite spLockBg = null;
  public UISprite spImage = null;
  private int m_Index = -1;
  public int Index
  {
    get
    {
      return m_Index;
    }
    set
    {
      m_Index = value;
    }
  }
  private int m_ArtifactId = -1;
  public int ArtifactId
  {
    get
    {
      return m_ArtifactId;
    }
    set
    {
      m_ArtifactId = value;
    }
  }
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
  public void Unlock(bool isUnlock)
  {
    if (spLock != null)
      NGUITools.SetActive(spLock.gameObject, !isUnlock);
    if (spLockBg != null)
      NGUITools.SetActive(spLockBg.gameObject, !isUnlock);
    if (lblUnlockHint != null)
      NGUITools.SetActive(lblUnlockHint.gameObject, !isUnlock);
  }
  public void InitSlot(int index, ItemDataInfo itemInfo)
  {
    Index = index;
    ArtifactId = itemInfo.ItemId;
    ItemConfig itemCfg = ItemConfigProvider.Instance.GetDataById(ArtifactId);
    if (itemCfg != null && spImage!=null) {
      spImage.spriteName = itemCfg.m_ItemTrueName;
    }
    Unlock(itemInfo.IsUnlock);
    MyDictionary<int, object> missionCfgDic = MissionConfigProvider.Instance.GetData();
    if (missionCfgDic != null) {
      foreach (object obj in missionCfgDic.Values) {
        MissionConfig cfg = obj as MissionConfig;
        if (cfg.UnlockLegacyId == ArtifactId && lblUnlockHint!=null) {
          string chn_des = DashFire.StrDictionaryProvider.Instance.GetDictString(409);
          lblUnlockHint.text = chn_des + cfg.Name;
          break;
        }
      }
    }
  }
}
