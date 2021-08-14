using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MarsVictory : MonoBehaviour
{
  private List<object> eventlist = new List<object>();
  public void UnSubscribe()
  {
    try {
      if (eventlist != null) {
        foreach (object eo in eventlist) {
          if (eo != null) {
            DashFire.LogicSystem.EventChannelForGfx.Unsubscribe(eo);
          }
        }
      }
      eventlist.Clear();
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  // Use this for initialization
  void Start()
  {
    if (eventlist != null) { eventlist.Clear(); }
    object eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_ui_unsubscribe", "ui", UnSubscribe);
    if (eo != null) eventlist.Add(eo);
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<int, int, int, int, int, int, int, int, int, int, string>("ge_pvp_result", "ui", PvpResult);
    if (eo != null) eventlist.Add(eo);

    UIManager.Instance.HideWindowByName("MarsVictory");
  }

  // Update is called once per frame
  void Update()
  {

  }
  public void ReturnMainCity()
  {
    UIManager.Instance.HideWindowByName("MarsVictory");
    DashFire.GfxSystem.EventChannelForLogic.Publish("ge_return_maincity", "lobby");
  }

  void PvpResult(int result, int enemyheroid, int oldelo, int elo, int enemyoldelo, int enemyelo, int damage, int enemydamage, int maxhitcount, int enemyhitcount, string enemynickname)
  {
    try {
      UIManager.Instance.MarsIntegral = elo;
      if (JoyStickInputProvider.JoyStickEnable) {
        JoyStickInputProvider.JoyStickEnable = false;
      }
      UIManager.Instance.ShowWindowByName("MarsVictory");

      Transform tf = transform.Find("sp_winlose");
      if (tf != null) {
        UISprite us = tf.gameObject.GetComponent<UISprite>();
        if (us != null) {
          if (result == 0) {
            us.spriteName = "win";
          } else {
            us.spriteName = "lose";
          }
        }
      }
      DashFire.RoleInfo ri = DashFire.LobbyClient.Instance.CurrentRole;
      if (ri != null) {
        tf = transform.Find("sp_headMeBottom/lb_nameMe");
        if (tf != null) {
          UILabel ul = tf.gameObject.GetComponent<UILabel>();
          if (ul != null) {
            ul.text = ri.Nickname;
          }
        }
        tf = transform.Find("sp_headMeBottom/sp_headMe");
        if (tf != null) {
          UISprite us = tf.gameObject.GetComponent<UISprite>();
          if (us != null) {
            us.spriteName = ri.HeroId == 1 ? "jianshi" : ri.HeroId == 2 ? "cike" : "";
          }
        }
      }
      tf = transform.Find("sp_headYouBottom/sp_headYou");
      if (tf != null) {
        UISprite us = tf.gameObject.GetComponent<UISprite>();
        if (us != null) {
          us.spriteName = enemyheroid == 1 ? "jianshi" : enemyheroid == 2 ? "cike" : "";
        }
      }
      tf = transform.Find("sp_headYouBottom/lb_nameMe");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = enemynickname;
        }
      }
      tf = transform.Find("go_mainRect/go_jifen/lb_jifenMe");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = oldelo.ToString();
        }
      }
      tf = transform.Find("go_mainRect/go_jifen/lb_jifenMeJia");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = (elo - oldelo) >= 0 ? "+" + (elo - oldelo) : (elo - oldelo).ToString();
        }
      }
      tf = transform.Find("go_mainRect/go_jifen/lb_jifenYou");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = enemyoldelo.ToString();
        }
      }
      tf = transform.Find("go_mainRect/go_jifen/lb_jifenYouJia");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = (enemyelo - enemyoldelo) >= 0 ? "+" + (enemyelo - enemyoldelo) : (enemyelo - enemyoldelo).ToString();
        }
      }
      tf = transform.Find("go_mainRect/go_hurt/lb_hurtMe");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = damage.ToString();
        }
      }
      tf = transform.Find("go_mainRect/go_hurt/lb_hurtYou");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = enemydamage.ToString();
        }
      }
      tf = transform.Find("go_mainRect/go_lianji/lb_lianjiMe");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = maxhitcount.ToString();
        }
      }
      tf = transform.Find("go_mainRect/go_lianji/lb_lianjiYou");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = enemyhitcount.ToString();
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
}
