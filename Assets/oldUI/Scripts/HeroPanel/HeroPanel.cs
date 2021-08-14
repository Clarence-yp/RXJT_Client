using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DashFire;
using System.Text;
public enum PanelType
{
  MySelf,
  Enemy
}
public class HeroPanel : MonoBehaviour
{

  // Use this for initialization
  public UISprite spHeroPortrait = null;
  public UIProgressBar hpProgressBar = null;
  public UIProgressBar mpProgressBar = null;
  public UILabel lblLevel = null;
  public UILabel lblHp = null;
  public UILabel lblMp = null;
  public PanelType panelType = PanelType.MySelf;
  public bool m_IsInitialized = false;
  void Start()
  {
  }
  // Update is called once per frame
  void Update()
  {
    if (panelType == PanelType.MySelf) {
      UpdateSelfPanel();
    } else {
      UpdateEnemyPanel();
    }
  }
  //自己的血条
  public void UpdateSelfPanel()
  {
    if(m_GfxUserInfo!=null){
      SharedGameObjectInfo share_info = LogicSystem.GetSharedGameObjectInfo(m_GfxUserInfo.m_ActorId);
      if (null != share_info) {
        UpdateHealthBar((int)share_info.Blood, (int)share_info.MaxBlood);
        UpdateMp((int)share_info.Energy, (int)share_info.MaxEnergy);
        SetHeroLevel(m_GfxUserInfo.m_Level);
        if (!m_IsInitialized) {
          Data_PlayerConfig playerData = PlayerConfigProvider.Instance.GetPlayerConfigById(m_GfxUserInfo.m_HeroId);
          if (playerData != null) {
            m_IsInitialized = true;
            SetHeroPortrait(playerData.m_Portrait);
          }
        }
      }
    }
  }
  //敌人的血条
  public void UpdateEnemyPanel()
  {
    if(m_GfxUserInfo==null)return;
    SharedGameObjectInfo enemy_info = LogicSystem.GetSharedGameObjectInfo(m_GfxUserInfo.m_ActorId);
    if (enemy_info != null) {
      UpdateHealthBar((int)enemy_info.Blood, (int)enemy_info.MaxBlood);
      UpdateMp((int)enemy_info.Energy, (int)enemy_info.MaxEnergy);
      SetHeroLevel(m_GfxUserInfo.m_Level);
      if (!m_IsInitialized) {
        Data_PlayerConfig playerData = PlayerConfigProvider.Instance.GetPlayerConfigById(m_GfxUserInfo.m_HeroId);
        if (playerData != null) {
          m_IsInitialized = true;
          SetHeroPortrait(playerData.m_Portrait);
        }
      }
    }
  }
  //设置玩家头像
  void SetHeroPortrait(string portrait)
  {
    if (null != spHeroPortrait) {
      spHeroPortrait.spriteName = portrait;
      UIButton btn = spHeroPortrait.GetComponent<UIButton>();
      if (btn != null)
        btn.normalSprite = portrait;
    }
  }
  //设置玩家等级
  void SetHeroLevel(int level)
  {
    if (lblLevel != null)
      lblLevel.text = level.ToString();
  }
  //设置玩家昵称
  void SetHeroNickName(string nickName)
  {
    Transform trans = this.transform.Find("NickName");
    if (trans == null)
      return;
    GameObject go = trans.gameObject;
    if (null != go) {
      UILabel label = go.GetComponent<UILabel>();
      if (null != label)
        label.text = nickName;
    }
  }
  //更新血条
  void UpdateHealthBar(int curValue, int maxValue)
  {
    if (maxValue <= 0 || curValue < 0)
      return;
    float value = curValue / (float)maxValue;
    if (null != hpProgressBar) {
      hpProgressBar.value = value;
    }
    if (null != lblHp) {
      StringBuilder sb = new StringBuilder();
      sb.Append((int)(value * 100) + "%");
      lblHp.text = sb.ToString();
    }
  }
  //更新魔法值
  void UpdateMp(int curValue, int maxValue)
  {
    if (maxValue <= 0 || curValue < 0)
      return;
    float value = curValue / (float)maxValue;
    if (null != mpProgressBar) {
      mpProgressBar.value = value;
    }
    if (null != lblMp) {
      StringBuilder sb = new StringBuilder();
      sb.Append((int)(value * 100) + "%");
      lblMp.text = sb.ToString();
    }
  }
  void CastAnimation(GameObject father)
  {
    if (null == father)
      return;
    GameObject goBack = null;
    UIProgressBar progressBar = null;
    Transform trans = father.transform.Find("Sprite(red)");
    if (trans != null)
      goBack = trans.gameObject;
    progressBar = father.GetComponent<UIProgressBar>();

    UISprite spBack = null;
    if (null != goBack) {
      spBack = goBack.GetComponent<UISprite>();
    }

    if (null != spBack && null != progressBar) {
      if (spBack.fillAmount <= progressBar.value) {
        spBack.fillAmount = progressBar.value;
      } else {
        spBack.fillAmount -= RealTime.deltaTime * 0.5f;
      }
    }
  }
  public void SetUserInfo(GfxUserInfo userinfo)
  {
    m_GfxUserInfo = userinfo;
  }
  public void OnPortraitClick()
  {
    //LogicSystem.EventChannelForGfx.Publish("ge_cast_skill", "game", "SkillEx");
  }
  //记录怒气条的6个怒气点
  private Vector3 m_OriginalPos;
  private GfxUserInfo  m_GfxUserInfo = null;

}
