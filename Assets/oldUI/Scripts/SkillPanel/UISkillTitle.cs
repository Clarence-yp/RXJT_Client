using UnityEngine;
using System.Collections;
using DashFire;

public class UISkillTitle : MonoBehaviour
{

  public UILabel lblLevel = null;
  public UILabel lblMoneyCoin = null;
  public UILabel lblDiamond = null;
  public UILabel lblName = null;
  public UISprite spPortrait = null;
  // Use this for initialization
  void Start()
  {
    RoleInfo info = LobbyClient.Instance.CurrentRole;
    if (null != info) {
      UserInfo user_info = info.GetPlayerSelfInfo();
      if (user_info != null) {
        if (lblLevel != null) lblLevel.text = info.Level.ToString();
        if (lblName != null) lblName.text = info.Nickname;
        Data_PlayerConfig playerData = PlayerConfigProvider.Instance.GetPlayerConfigById(info.HeroId);
        if (playerData != null) {
          if(spPortrait!=null)  spPortrait.spriteName = playerData.m_Portrait;
        }
      }
    }
  }

  // Update is called once per frame
  void Update()
  {

  }

  public void SetLevel(int level)
  {
    if (null != lblLevel) {
      lblLevel.text =level.ToString();
    }
  }
  public void SetMoneyCoin(int amount)
  {
    if (null != lblMoneyCoin) {
      lblMoneyCoin.text = amount.ToString();
    }
  }
  public void SetDiamond(int amount)
  {
    if (null != lblDiamond) {
      lblDiamond.text = amount.ToString();
    }
  }
  public void OnHideButtonClick()
  {
    DashFire.LogicSystem.PublishLogicEvent("ge_set_preset", "lobby", UISkillSetting.presetIndex);
    UIManager.Instance.HideWindowByName("SkillPanel");
  }
  public void OnBuyCoinClick()
  {
    UIManager.Instance.ShowWindowByName("GoldBuy");
  }

}
