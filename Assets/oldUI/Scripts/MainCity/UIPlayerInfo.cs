using UnityEngine;
using System;
using System.Collections;
using System.Text;
using DashFire;

public class UIPlayerInfo : MonoBehaviour {

  public UILabel level = null;
  public UILabel NickName = null;
  public UILabel MainCity = null;
  //战斗力
  public UILabel FightingValue = null;
  //体力
  public UILabel lblStamina = null;
  //头像
  public UISprite portrait = null;
  //经验条
  public UIProgressBar progressBar = null;
  public bool m_IsInitialized = false;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
     RoleInfo role_info = LobbyClient.Instance.CurrentRole;
     if (role_info != null) {
       UpdateEx(role_info.Level, role_info.Exp);
       SetPlayerLevel(role_info.Level);
       SetFighting((int)role_info.FightingScore);
       StringBuilder sbuild = new StringBuilder();
       sbuild.Append(role_info.CurStamina + "/" + role_info.StaminaMax);
       if (lblStamina != null) lblStamina.text = sbuild.ToString();
       if (!m_IsInitialized) {
         Data_PlayerConfig playerData = PlayerConfigProvider.Instance.GetPlayerConfigById(role_info.HeroId);
         if (playerData != null) {
           m_IsInitialized = true;
           SetPortrait(playerData.m_Portrait);
           SetNickname(role_info.Nickname);
         }
       }
     }
	}
  //更新经验值
  public void UpdateEx(int level,int exp)
  {
    int curent=0, max = 0 ;
    int baseExp = 0;
    if (level == 1) {
      baseExp = 0;
    } else {
      PlayerLevelupExpConfig expCfg = PlayerConfigProvider.Instance.GetPlayerLevelupExpConfigById(level - 1);
      if (expCfg != null)
        baseExp = expCfg.m_ConsumeExp;
    }
    PlayerLevelupExpConfig expCfgHith = PlayerConfigProvider.Instance.GetPlayerLevelupExpConfigById(level);
    if (expCfgHith != null) max = expCfgHith.m_ConsumeExp - baseExp;
    curent = exp - baseExp;
    if (progressBar != null && max!=0) {
      progressBar.value = curent / (float)max;
    }
  }
  //设置战斗力
  public void SetFighting(int value)
  {
    if (FightingValue != null) {
      FightingValue.text = value.ToString();
    }
  }
  public void SetNickname(string name)
  {
    if (NickName != null) {
      NickName.text = name;
    }
  }
  public void SetCityName(string name)
  {
    if (MainCity != null) {
      MainCity.text = name;
    }
  }
  public void SetPlayerLevel(int playerLevel)
  {
    if (level != null) {
      level.text = ""+playerLevel.ToString();
    }
  }
  public void SetPortrait(string name)
  {
    if (portrait != null) {
      portrait.spriteName = name;
      UIButton btn = portrait.GetComponent<UIButton>();
      if (btn != null)
        btn.normalSprite = name;
    }
  }
  public void OnPortraitClick()
  { 
    UIManager.Instance.ShowWindowByName("GamePokey");
  }
  public void ButtonShowOption()
  {
    GameObject go = UIManager.Instance.GetWindowGoByName("Option");
    if (go != null) {
      if (NGUITools.GetActive(go)) {
        UIManager.Instance.HideWindowByName("Option");
      } else {
        UIManager.Instance.ShowWindowByName("Option");
      }
    }
  }
}
