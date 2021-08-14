using UnityEngine;
using DashFire;
using System;
using System.Collections;

public class UITreasureSlot : MonoBehaviour {

  public UILabel lblFighting = null;
  public UILabel lblLevel = null;
  public UILabel lblName = null;
  public UISprite spPortrait = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
  public void SetLevelInfo(int monsterId,int attrId)
  {
    RoleInfo role_info = LobbyClient.Instance.CurrentRole;
    if (role_info != null) {
    Data_NpcConfig npcCfg =  NpcConfigProvider.Instance.GetNpcConfigById(monsterId);
    if (npcCfg != null) {
      if (lblLevel != null) lblLevel.text = "Lv."+role_info.Level.ToString();
      if (lblName != null) lblName.text = npcCfg.m_Name;
      if (spPortrait != null) spPortrait.spriteName = npcCfg.m_Portrait;
    }
    }
    ExpeditionMonsterAttrConfig attrCfg= ExpeditionMonsterAttrConfigProvider.Instance.GetExpeditionMonsterAttrConfigById(attrId);
    if(attrCfg!=null&&lblFighting!=null){
      if (lblFighting != null) lblFighting.text =((int) attrCfg.m_AttrData.GetAddAd(0, role_info.Level)).ToString();
    }
  }
  public void SetLevelInfo(ExpeditionImageInfo enemy_info)
  {
    if (enemy_info != null) {
      //string CHN = StrDictionaryProvider.Instance.GetDictString(361);
      if (lblFighting != null) lblFighting.text =((int)enemy_info.FightingScore).ToString();
      if (lblLevel != null) lblLevel.text ="Lv."+enemy_info.Level.ToString();
      if (lblName != null) lblName.text = enemy_info.Nickname;
      Data_PlayerConfig playerCfg = PlayerConfigProvider.Instance.GetPlayerConfigById(enemy_info.HeroId);
      if (playerCfg != null && spPortrait!=null)
        spPortrait.spriteName = playerCfg.m_Portrait;
    }
  }

}
