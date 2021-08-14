using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DashFire;
using System.Text;

public class UIChangeHero : MonoBehaviour
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
        eventlist.Clear();
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  // Use this for initialization
  void Start()
  {
    //DontDestroyOnLoad(UIRootGO);
    //DontDestroyOnLoad(this.gameObject);
    if (eventlist != null) { eventlist.Clear(); }
    object eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<bool>("ge_create_hero_scene", "ui", SetSceneAndLoadHero);
    if (eo != null) { eventlist.Add(eo); }
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<List<string>>("ge_nickname_result", "lobby", OnReceiveNicknames);
    if (eo != null) { eventlist.Add(eo); }
    eo = LogicSystem.EventChannelForGfx.Subscribe<string, int, string, ulong, int, string>("ge_role_enter_log", "log", OnRoleEnterLog);
    if (eo != null) { eventlist.Add(eo); }      
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_login_finish", "lobby", LoginInFinish);
    if (eo != null) { eventlist.Add(eo); }
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<bool>("ge_createhero_result", "lobby", CreateHeroResult);
    if (eo != null) { eventlist.Add(eo); }

    ChangeHeroIntroduce(0);

    SetSceneVisible(false);
  }
  void SetSceneVisible(bool vis)
  {
    NGUITools.SetActive(UIRootGO.gameObject, vis);
    NGUITools.SetActive(gameObject, vis);
  }
  void SetSceneAndLoadHero(bool vis)
  {
    try {
      SetSceneVisible(vis);
      OkToLoadHero(LobbyClient.Instance.AccountInfo.Players);
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  // Update is called once per frame
  void Update()
  {

  }

  void OkToLoadHero(List<DashFire.RoleInfo> playlist)
  {
    if (playguidlist != null && heroidlist != null) {
      heroidlist.Clear();
      playguidlist.Clear();
    }
    if (playlist != null && playlist.Count > 0) {
      //SortList(playlist);//对英雄按等级由高到低显示
      int i = playlist.Count;
      i = (i <= 3 ? i : 3);
      for (int j = 0; j < i; ++j) {
        DashFire.RoleInfo pi = playlist[j];
        if (pi != null) {
          SetHeroInfo(j, pi);
          if (playguidlist != null) {
            playguidlist.Add(pi.Guid);
          }
          if (heroidlist != null) {
            heroidlist.Add(pi.HeroId);
          }
        }
      }
      if (i < 3) {
        Transform tf = UIRootGO.transform.Find("SelectHeroPanel");
        if (tf != null) {
          Transform tfc = tf.Find("SelectHero" + i);
          if (tfc != null) {
            NGUITools.SetActive(tfc.gameObject, true);
          }
        }
      }

      DashFire.RoleInfo playerzero = playlist[0];
      if (playerzero != null) {
        ChangeHeroIntroduce(playerzero.HeroId);
        SetHeroVisible(1, false);
        num = playerzero.HeroId;
        SetHeroVisible(num, true);
      }

      WitchButtonPress(0);
    } else {
      ChangeHeroIntroduce(0);
      num = 0;
      SetHeroVisible(num, true);
      SelectHero0();
    }
  }
  private void SortList(List<DashFire.RoleInfo> playlist)
  {
    if (playlist != null) {
      playlist.Sort(SortByLevel);
    }
  }
  private int SortByLevel(DashFire.RoleInfo a, DashFire.RoleInfo b)
  {
    return (0 - a.Level + b.Level);
  }
  private void SetHeroInfo(int num, DashFire.RoleInfo pi)
  {
    if (num < 0 || num > 3) return;
    Transform tf = UIRootGO.transform.Find("SelectHeroPanel/SelectHero" + num);
    if (tf != null) {
      UISprite us = tf.gameObject.GetComponent<UISprite>();
      if (us != null) {
        us.color = Color.white;
      }
      NGUITools.SetActive(tf.gameObject, true);
      tf = tf.transform.Find("Back");
      if (tf != null) {
        NGUITools.SetActive(tf.gameObject, true);
        tf = tf.Find("Label");
        if (tf != null) {
          UILabel ul = tf.gameObject.GetComponent<UILabel>();
          if (ul != null) {
            ul.text = /*(heroinfodic.ContainsKey(pi.HeroId) ? heroinfodic[pi.HeroId].HeroName : " ") + "\n" +*/ "[i]Lv." + pi.Level + "\n" + pi.Nickname + "[-]";
          }
        }
      }
    }
    tf = UIRootGO.transform.Find("SelectHeroPanel/SelectHero" + num + "/Back/Head");
    if (tf != null) {
      UISprite us = tf.gameObject.GetComponent<UISprite>();
      if (us != null) {
        us.spriteName = pi.HeroId == 1 ? "kuang-zhan-shi-tou-xiang2" : pi.HeroId == 2 ? "ci-ke-tou-xiang2" : "";
      }
    }
    tf = UIRootGO.transform.Find("SelectHeroPanel/SelectHero" + num + "/Sprite");
    if (tf != null) {
      NGUITools.SetActive(tf.gameObject, false);
    }
  }

  public void EnterGame()
  {
    if (signforcreate) {
      //LogicSystem.PublishLogicEvent("ge_create_role", "lobby", num % HeroCount, GetHeroNickName());
      if (UIRootGO != null) {
        Transform tf = UIRootGO.transform.Find("YesOrNot/Sprite/ChatInput/Back/Label");
        if (tf != null && tf.transform != null) {
          UILabel ul = tf.gameObject.GetComponent<UILabel>();
          if (ul != null) {
            ul.text = "";
          }
        }
        tf = UIRootGO.transform.Find("YesOrNot/Sprite/ChatInput/Back");
        if (tf != null) {
          UIInput ui = tf.gameObject.GetComponent<UIInput>();
          if (ui != null) {
            ui.value = "";
          }
        }
      }
      ChooseYesOrNoVisible(DashFire.StrDictionaryProvider.Instance.GetDictString(135), true);
      LogicSystem.PublishLogicEvent("ge_create_nickname", "lobby");
    } else {
      LogicSystem.PublishLogicEvent("ge_role_enter", "lobby", signforbuttonpress);
    }
  }
  private void ActionCreateHeroFailure()
  {
    //
    YesOrNot -= ActionCreateHeroFailure;
  }
  public void ReturnLogin()
  {
    if (LobbyClient.Instance.AccountInfo.Players == null || LobbyClient.Instance.AccountInfo.Players.Count == 0) {
      return;
    }
    if (signforcreate) {
      if (heroidlist != null && heroidlist.Count > lastselectbut) {
        ButtonCreateHeroColourScale(0);
        SetHeroVisible(num % HeroCount, false);
        num = heroidlist[lastselectbut];
        SetHeroVisible(num % HeroCount, true);
        ChangeHeroIntroduce(num % HeroCount);
      }
      WitchButtonPress(lastselectbut);
      ChangeActionShowAbout(false);
    } else {
      //       SetSceneVisible(false);
      //       //返回登陆界面
      //       UIManager.Instance.ShowWindowByName("LoginPrefab");
    }
  }

  private void ChangeHeroIntroduce(int heroid)
  {
    DashFire.Data_PlayerConfig dpc = DashFire.PlayerConfigProvider.Instance.GetPlayerConfigById(heroid);
    if (heroid >= 0 && UIRootGO != null && dpc != null) {
      Transform tf = UIRootGO.transform.Find("IntroducePanelCopy/Container");
      if (tf != null) {
        tf = tf.Find("Sprite/Name");
        if (tf != null) {
          UILabel ul = tf.gameObject.GetComponent<UILabel>();
          if (ul != null) {
            ul.text = "[i]" + dpc.m_Name + "[-]";
          }
        }
      }
      tf = UIRootGO.transform.Find("IntroducePanelCopy/Container");
      if (tf != null) {
        tf = tf.Find("Bula");
        if (tf != null) {
          UILabel ul = tf.gameObject.GetComponent<UILabel>();
          if (ul != null) {
            ul.text = dpc.m_HeroIntroduce2;
          }
        }
      }
      tf = UIRootGO.transform.Find("IntroducePanelCopy/Container");
      if (tf != null) {
        tf = tf.Find("Introduce");
        if (tf != null) {
          UILabel ul = tf.gameObject.GetComponent<UILabel>();
          if (ul != null) {
            ul.text = dpc.m_HeroIntroduce1;
          }
        }
      }
      tf = UIRootGO.transform.Find("IntroducePanelCopy/Container");
      if (tf != null) {
        tf = tf.Find("Sprite/Back/Head");
        if (tf != null) {
          UISprite us = tf.gameObject.GetComponent<UISprite>();
          if (us != null) {
            us.spriteName = heroid == 1 ? "kuang-zhan-shi-tou-xiang2" : heroid == 2 ? "ci-ke-tou-xiang2" : "";
          }
        }
      }
    }
  }
  private void SetHeroVisible(int id, bool vis)
  {
    Transform tf = UIRootGO.transform.Find("Hero" + id);
    if (tf != null && tf.gameObject != null) {
      if (NGUITools.GetActive(tf.gameObject) != vis) {
        NGUITools.SetActive(tf.gameObject, vis);
      }
    }
  }
  public void CreateHeroButton()
  {
    if (true) {

    }
  }

  public void ButtonCreateHero0()
  {
    ButtonCreateHeroColourScale(1);
  }

  public void ButtonCreateHero1()
  {
    ButtonCreateHeroColourScale(2);
  }

  public void ButtonCreateHero2()
  {
    //ButtonCreateHeroColourScale(2);
  }

  public void ButtonCreateHero3()
  {
    //ButtonCreateHeroColourScale(3);
  }

  private void ButtonCreateHeroColourScale(int newnum)
  {
    if (UIRootGO != null) {
      Transform tf = UIRootGO.transform.Find("ButtonCreateHero/" + (num - 1) + "/Back");
      if (tf != null) {
        NGUITools.SetActive(tf.gameObject, false);
      }
      tf = UIRootGO.transform.Find("ButtonCreateHero/" + (newnum - 1) + "/Back");
      if (tf != null) {
        NGUITools.SetActive(tf.gameObject, true);
      }
    }
    SetHeroVisible(num, false);
    num = newnum;
    SetHeroVisible(num, true);
    ChangeHeroIntroduce(num);
  }

  public void LeftButtonClick()
  {
    SetHeroVisible(num-- % HeroCount, false);
    if (num < 0) {
      num = HeroCount - 1;
    }
    SetHeroVisible(num % HeroCount, true);
    ChangeHeroIntroduce(num % HeroCount);
  }

  public void RightButtonClick()
  {
    SetHeroVisible(num++ % HeroCount, false);
    SetHeroVisible(num % HeroCount, true);
    ChangeHeroIntroduce(num % HeroCount);
  }

  public void ButtonForNickName()
  {
    if (m_NicknameCount < m_NicknameList.Count) {
      SetHeroNickName(m_NicknameList[m_NicknameCount]);
      m_NicknameCount++;
    } else {
      LogicSystem.PublishLogicEvent("ge_create_nickname", "lobby");
    }
  }
  private void SetHeroNickName(string nickname)
  {
    try {
      if (UIRootGO != null && nickname != null) {
        Transform tf = UIRootGO.transform.Find("YesOrNot/Sprite/ChatInput/Back/Label");
        if (tf != null && tf.transform != null) {
          UILabel ul = tf.gameObject.GetComponent<UILabel>();
          if (ul != null) {
            ul.text = nickname;
          }
        }
        tf = UIRootGO.transform.Find("YesOrNot/Sprite/ChatInput/Back");
        if (tf != null) {
          UIInput ui = tf.gameObject.GetComponent<UIInput>();
          if (ui != null) {
            ui.value = nickname;
          }
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  public void OnReceiveNicknames(List<string> nicknameList)
  {
    try {
      m_NicknameList.Clear();
      m_NicknameList = nicknameList;
      m_NicknameCount = 0;
      if (m_NicknameList.Count >= 1) {
        SetHeroNickName(m_NicknameList[m_NicknameCount]);
        m_NicknameCount++;
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  public void Change()
  {
    string str = GetHeroNickName().Trim();
    ChooseYesOrNoVisible(DashFire.StrDictionaryProvider.Instance.GetDictString(134), true);
    if(CalculateStringByte(str)>14) {
      ChooseYesOrNoVisible(DashFire.StrDictionaryProvider.Instance.GetDictString(362), true);
    }
  }
  public void ButtonDown()
  {
    if (UIRootGO != null) {
      Transform tf = UIRootGO.transform.Find("YesOrNot/Sprite");
      if (tf != null) {
        UIAnchor ua = tf.gameObject.GetComponent<UIAnchor>();
        if (ua != null) {
          ua.relativeOffset = new Vector2(0f, 0f);
          ua.enabled = true;
        }
      }
    }
  }
  public void Submit()
  {
    if (UIRootGO != null) {
      Transform tf = UIRootGO.transform.Find("YesOrNot/Sprite");
      if (tf != null) {
        UIAnchor ua = tf.gameObject.GetComponent<UIAnchor>();
        if (ua != null) {
          ua.relativeOffset = new Vector2(0f, -0.25f);
          ua.enabled = true;
        }
      }
    }
  }
  private string GetHeroNickName()
  {
    if (UIRootGO != null) {
      Transform tf = UIRootGO.transform.Find("YesOrNot/Sprite/ChatInput/Back/Label");
      if (tf != null && tf.transform != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          return ul.text;
        }
      }
    }
    return null;
  }

  public void SelectHero0()
  {
    if (playguidlist.Count < 1) {
      //创建英雄
      CreateHero();
    } else {
      if (heroidlist != null && heroidlist.Count > 0) {
        SetHeroVisible(num % HeroCount, false);
        num = heroidlist[0];
        SetHeroVisible(num % HeroCount, true);
        ChangeHeroIntroduce(num % HeroCount);
      }
    }
    WitchButtonPress(0);
  }

  public void SelectHero1()
  {
    if (playguidlist.Count < 2) {
      //创建英雄
      CreateHero();
    } else {
      if (heroidlist != null && heroidlist.Count > 1) {
        SetHeroVisible(num % HeroCount, false);
        num = heroidlist[1];
        SetHeroVisible(num % HeroCount, true);
        ChangeHeroIntroduce(num % HeroCount);
      }
    }
    WitchButtonPress(1);
  }

  public void SelectHero2()
  {
    if (playguidlist.Count < 3) {
      //创建英雄
      CreateHero();
    } else {
      if (heroidlist != null && heroidlist.Count > 2) {
        SetHeroVisible(num % HeroCount, false);
        num = heroidlist[2];
        SetHeroVisible(num % HeroCount, true);
        ChangeHeroIntroduce(num % HeroCount);
      }
    }
    WitchButtonPress(2);
  }

  public void SelectHero3()
  {
    if (playguidlist.Count < 4) {
      //创建英雄
      CreateHero();
    } else {
      if (heroidlist != null && heroidlist.Count > 3) {
        SetHeroVisible(num % HeroCount, false);
        num = heroidlist[3];
        SetHeroVisible(num % HeroCount, true);
        ChangeHeroIntroduce(num % HeroCount);
      }
    }
    WitchButtonPress(3);
  }

  public void SelectHero4()
  {
    if (playguidlist.Count < 5) {
      //创建英雄
      CreateHero();
    } else {
      if (heroidlist != null && heroidlist.Count > 4) {
        SetHeroVisible(num % HeroCount, false);
        num = heroidlist[4];
        SetHeroVisible(num % HeroCount, true);
        ChangeHeroIntroduce(num % HeroCount);
      }
    }
    WitchButtonPress(4);
  }
  private void WitchButtonPress(int sign)
  {
    if (sign < 0) return;
    if (UIRootGO != null) {
      Transform tf = UIRootGO.transform.Find("SelectHeroPanel/SelectHero" + signforbuttonpress);
      if (tf != null) {
        UISprite us = tf.gameObject.GetComponent<UISprite>();
        if (us != null) {
          us.width = 400;
        }
        tf = tf.Find("Back/Frame");
        if (tf != null) {
          NGUITools.SetActive(tf.gameObject, false);
        }
      }
      tf = UIRootGO.transform.Find("SelectHeroPanel/SelectHero" + sign);
      if (tf != null) {
        UISprite us = tf.gameObject.GetComponent<UISprite>();
        if (us != null) {
          us.width = 512;
        }
        tf = tf.Find("Back/Frame");
        if (tf != null) {
          NGUITools.SetActive(tf.gameObject, true);
        }
      }
    }
    signforbuttonpress = sign;
  }
  private void CreateHero()
  {
    try {
      SetHeroVisible(num % HeroCount, false);
      ButtonCreateHero0();
      lastselectbut = signforbuttonpress;
      //num = 0;
      //SetHeroVisible(num + 1, true);
      //ChangeHeroIntroduce(0);
      ButtonCreateHero0();//设为第一个英雄
      ChangeActionShowAbout(true);
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private void ChangeActionShowAbout(bool actionsign)
  {
    if (UIRootGO == null && UIRootGO.transform != null) return;

    Transform tf = UIRootGO.transform.Find("ButtonPanel");
    //     if (tf != null) {
    //       NGUITools.SetActive(tf.gameObject, actionsign);
    //     }
    tf = UIRootGO.transform.Find("SelectHeroPanel");
    if (tf != null) {
      NGUITools.SetActive(tf.gameObject, !actionsign);
    }
    tf = UIRootGO.transform.Find("ChatInput");
    if (tf != null) {
      NGUITools.SetActive(tf.gameObject, actionsign);
    }
    tf = UIRootGO.transform.Find("ButtonCreateHero");
    if (tf != null) {
      NGUITools.SetActive(tf.gameObject, actionsign);
    }
    tf = UIRootGO.transform.Find("Title/Sprite");
    if (tf != null) {
      UISprite us = tf.gameObject.GetComponent<UISprite>();
      if (us != null) {
        if (actionsign) {
          us.spriteName = "chuang-jian-jue-se";
        } else {
          us.spriteName = "xuan-ze-jue-se";
        }
      }
    }
    tf = UIRootGO.transform.Find("IntroducePanelCopy");
    if (tf != null) {
      NGUITools.SetActive(tf.gameObject, actionsign);
    }
    tf = UIRootGO.transform.Find("ScenePanel/ReturnLogin");
    if (tf != null) {
      NGUITools.SetActive(tf.gameObject, actionsign);
    }

    tf = UIRootGO.transform.Find("ScenePanel");
    Transform tfc = tf;
    if (tf != null) {
      tf = tf.Find("EnterGame");
      if (tf != null) {
        tf = tf.Find("Label");
        if (tf != null && tf.gameObject != null) {
          UILabel ul = tf.gameObject.GetComponent<UILabel>();
          if (ul != null) {
            if (actionsign) {
              ul.text = DashFire.StrDictionaryProvider.Instance.GetDictString(133);
            } else {
              ul.text = DashFire.StrDictionaryProvider.Instance.GetDictString(132);
            }
          }
        }
      }
      tf = tfc.Find("ReturnLogin");
      if (tf != null) {
        tf = tf.Find("Label");
        if (tf != null && tf.gameObject != null) {
          UILabel ul = tf.gameObject.GetComponent<UILabel>();
          if (actionsign) {
            ul.text = DashFire.StrDictionaryProvider.Instance.GetDictString(131);
          } else {
            ul.text = DashFire.StrDictionaryProvider.Instance.GetDictString(130);
          }
        }
      }
    }

    signforcreate = actionsign;
  }

  public void Yes()
  {
    string nickname = GetHeroNickName().Trim();
    if (CalculateStringByte(nickname) > 14) {
      ChooseYesOrNoVisible(DashFire.StrDictionaryProvider.Instance.GetDictString(362), true);
    } else {
      if (nickname.Equals(String.Empty)) {
        ChooseYesOrNoVisible(DashFire.StrDictionaryProvider.Instance.GetDictString(129), true);
        return;
      }
      bool ret = WordFilter.Instance.Check(nickname);
      if (ret == true) {
        ChooseYesOrNoVisible(DashFire.StrDictionaryProvider.Instance.GetDictString(128), true);
        return;
      }
      LogicSystem.PublishLogicEvent("ge_create_role", "lobby", num % HeroCount, nickname);
    }
  }
  public void No()
  {
    Submit();
    ChooseYesOrNoVisible(null, false);
  }
  private void ChooseYesOrNoVisible(string str, bool vis)
  {
    if (UIRootGO != null) {
      Transform tf = UIRootGO.transform.Find("YesOrNot");
      if (vis && tf != null) {
        if (tf.gameObject != null) {
          NGUITools.SetActive(tf.gameObject, true);
        }
        tf = tf.Find("Sprite");
        if (tf != null) {
          tf = tf.Find("Label");
          if (tf != null && tf.gameObject != null) {
            UILabel ul = tf.gameObject.GetComponent<UILabel>();
            if (ul != null && str != null) {
              ul.text = str;
            }
          }
        }
      } else {
        if (tf != null && tf.gameObject != null) {
          NGUITools.SetActive(tf.gameObject, false);
        }
      }
    }
  }
  private void OnRoleEnterLog(string account, int logicServerId, string nickname, ulong userGuid, int userLevel, string accountId)
  {
    try {      
#if UNITY_IPHONE
      CYMGWrapperIOS.MBIOnLogin(account, logicServerId.ToString(), nickname, userGuid.ToString(), userLevel, accountId);
#endif     
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private void LoginInFinish()
  {
    try {
      UnSubscribe();
      DestroyImmediate(gameObject);
      NGUITools.DestroyImmediate(UIRootGO);
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private void CreateHeroResult(bool result)
  {
    try {
      if (!result) {
        ChooseYesOrNoVisible(DashFire.StrDictionaryProvider.Instance.GetDictString(127), true);
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private int CalculateStringByte(string str)
  {
    if (str.Equals(string.Empty))
      return 0;
    int strlen = 0;
    byte[] bytes = Encoding.UTF8.GetBytes(str);
    bytes = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, bytes);
    int count = bytes.Length;
    for (int i = 0; i < count;++i ) {
      if (bytes[i] !=0) {
        ++strlen;
      }
    }
    return strlen;
  }
  private System.Action YesOrNot = null;
  private int num = 4;
  private int signforbuttonpress = 0;//现在被按下的按钮
  private bool signforcreate = false;//选择英雄或创建英雄
  private int lastselectbut = 0;//创建英雄前选择的按钮
  private List<ulong> playguidlist = new List<ulong>();//存储英雄的GUID
  private List<int> heroidlist = new List<int>();//存储英雄的id

  public UIRoot UIRootGO = null;

  private static int HeroCount = 4;

  private int m_NicknameCount = 0;
  private List<string> m_NicknameList = new List<string>();
}
public class HeroInfo
{
  public string HeroName = null;
  public string HeroIntroduction = null;
  public string HeroIntroduction2 = null;
}
