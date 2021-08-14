using UnityEngine;
using DashFire;
using System.Collections;

public class UIArtifactIntroduce : MonoBehaviour
{

  public UILabel lblAddHp = null;
  public UILabel lblAddDamage = null;
  public UILabel lblAddArmor = null;
  public UILabel lblAddMp = null;

  public UILabel lblName = null;
  public UILabel lblDesc = null;

  public UIButton btnLevelUp = null;
  public Color color = new Color();
  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }
  public void SetIntroduce(int itemId, bool isUnLock)
  {
      ItemConfig itemCfg = ItemConfigProvider.Instance.GetDataById(itemId);
      if (itemCfg != null) {
        RoleInfo role_info = LobbyClient.Instance.CurrentRole;
        if (role_info != null) {
          int itemLevel = 0;
          for (int index = 0; index < role_info.Legacys.Length; ++index) {
            if (role_info.Legacys[index] != null && role_info.Legacys[index].ItemId == itemId)
              itemLevel = role_info.Legacys[index].Level;
          }
          UserInfo userInfo = role_info.GetPlayerSelfInfo();
          if (null != userInfo && isUnLock) {
            //解锁
            if (lblAddDamage != null) lblAddDamage.text = ((int)itemCfg.m_AttrData.GetAddAd(0, userInfo.GetLevel(), itemLevel)).ToString();//伤害
            if (lblAddHp != null) lblAddHp.text = ((int)itemCfg.m_AttrData.GetAddHpMax(0, userInfo.GetLevel(), itemLevel)).ToString();//血量
            if (lblAddArmor != null) lblAddArmor.text = ((int)itemCfg.m_AttrData.GetAddADp(0, userInfo.GetLevel(), itemLevel)).ToString();//护甲
            if (lblAddMp != null) lblAddMp.text = ((int)itemCfg.m_AttrData.GetAddMDp(0, userInfo.GetLevel(), itemLevel)).ToString();//魔抗
            if (btnLevelUp != null) {
              btnLevelUp.isEnabled = true;
              UISprite sp = btnLevelUp.GetComponent<UISprite>();
              if (sp != null) sp.color = Color.white;
            }
          } else {
            //没有解锁
            if (lblAddDamage != null) lblAddDamage.text = "0";//伤害
            if (lblAddHp != null) lblAddHp.text = "0";//血量
            if (lblAddArmor != null) lblAddArmor.text = "0";//护甲
            if (lblAddMp != null) lblAddMp.text = "0";//魔抗
            if (btnLevelUp != null) {
              btnLevelUp.isEnabled = false;
              UISprite sp = btnLevelUp.GetComponent<UISprite>();
              if (sp != null) sp.color = color;
            }
          }
          if (lblName != null) lblName.text = itemCfg.m_ItemName;
          if (lblDesc != null) lblDesc.text = itemCfg.m_Description;
        }
      }
    }
}
