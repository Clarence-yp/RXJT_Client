using UnityEngine;
using System.Collections;
using DashFire;

public class UITreasurePlayerInfo : MonoBehaviour {


  public UIProgressBar hpBar = null;
  public UIProgressBar mpBar = null;
  public UISprite spPortrait = null;
  public UILabel lblLevel = null;
  public UILabel lblHp = null;
  public UILabel lblMoney = null;
  public UILabel lblDiamond = null;
  public UILabel lblStamina = null;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
    RoleInfo role_info = LobbyClient.Instance.CurrentRole;
    if (role_info != null) {
      if (lblDiamond != null) lblDiamond.text = role_info.Gold.ToString();
      if (lblMoney != null) lblMoney.text = role_info.Money.ToString();
      if (lblStamina != null) lblStamina.text = role_info.CurStamina + "/" + 120;
      ExpeditionPlayerInfo ep_info = role_info.GetExpeditionInfo();
      UserInfo user_info = role_info.GetPlayerSelfInfo();
      if (ep_info != null && user_info != null) {
        if (hpBar != null) {
          int hpMax = user_info.GetActualProperty().HpMax;
          if (hpMax != 0) {
            if (0 == ep_info.Schedule && ep_info.Hp > hpMax) {
              ep_info.Hp = hpMax;
            }
            hpBar.value = ep_info.Hp / (float)hpMax;
          }
          string str = string.Format("{0}/{1}", ep_info.Hp, hpMax);
          if (lblHp != null) lblHp.text = str;
        }
        if (mpBar != null) {
          int mpMax = user_info.GetActualProperty().EnergyMax;
          if (mpMax != 0) {
            if (0 == ep_info.Schedule || ep_info.Mp > mpMax) {
              ep_info.Mp = mpMax;
            }
            mpBar.value = ep_info.Mp / (float)mpMax;
          }
        }
        if (lblLevel != null) {
          lblLevel.text = role_info.Level.ToString();
        }
        if (spPortrait != null) {
          Data_PlayerConfig playerData = PlayerConfigProvider.Instance.GetPlayerConfigById(role_info.HeroId);
          if (playerData != null) {
            spPortrait.spriteName = playerData.m_Portrait;
          }
        }
      }
    }
	}
  void OnEnable()
  {
    
  }
  public void ResetPlayerInfo()
  {
    OnEnable();
  }
  //购买金币
  public void OnBuyMoney()
  {
    UIManager.Instance.ShowWindowByName("GoldBuy");
  }
  //购买体力
  public void OnBuyStamina()
  {
    UIManager.Instance.ShowWindowByName("TiliBuy");
  }
}
