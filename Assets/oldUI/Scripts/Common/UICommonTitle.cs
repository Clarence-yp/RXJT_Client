using UnityEngine;
using System.Collections;
using DashFire;

public class UICommonTitle : MonoBehaviour {
  public UISprite spPortrait = null;
  public UILabel lblLevel = null;
  public UILabel lblFighting = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
  void Update()
  {
    OnEnable();
  }
  void OnEnable()
  {
    RoleInfo info = LobbyClient.Instance.CurrentRole;
    if (null != info) {
      if (lblLevel != null) lblLevel.text = info.Level.ToString();
      if (lblFighting != null) lblFighting.text = ((int)info.FightingScore).ToString();
        Data_PlayerConfig playerData = PlayerConfigProvider.Instance.GetPlayerConfigById(info.HeroId);
        if (playerData != null) {
          if (spPortrait != null) spPortrait.spriteName = playerData.m_Portrait;
        }
    }
  }
}
