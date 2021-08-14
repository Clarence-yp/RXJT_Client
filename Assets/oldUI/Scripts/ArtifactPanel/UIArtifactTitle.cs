using UnityEngine;
using DashFire;
using System.Collections;

public class UIArtifactTitle : MonoBehaviour {
  public UILabel lblDiamond = null;
  public UILabel lblMoneyCoin = null;
  public UILabel lblPlayerName = null;
  public UILabel lblPlayerLevel = null;
  public UISprite spPortrait = null;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
  void OnEnable()
  {
    RoleInfo info = LobbyClient.Instance.CurrentRole;
    if (null != info) {
      if (lblDiamond != null) lblDiamond.text = info.Gold.ToString();
      if (lblMoneyCoin != null) lblMoneyCoin.text = info.Money.ToString();
      if (lblPlayerName != null) lblPlayerName.text = info.Nickname;
      if (lblPlayerLevel != null) lblPlayerLevel.text = info.Level.ToString();
      UserInfo user_info = info.GetPlayerSelfInfo();
      if (user_info != null) {
        Data_PlayerConfig playerData = PlayerConfigProvider.Instance.GetPlayerConfigById(info.HeroId);
        if (playerData != null) {
          if (spPortrait != null) spPortrait.spriteName = playerData.m_Portrait;
        }
      }
    }
  }
  public void OnHideButtonClick()
  {
    UIManager.Instance.HideWindowByName("ArtifactPanel");
  }
  public void OnBuyCoinClick()
  {
    UIManager.Instance.ShowWindowByName("GoldBuy");
  }
  public void UpdateTitleInfo()
  {
    OnEnable();
  }
}
