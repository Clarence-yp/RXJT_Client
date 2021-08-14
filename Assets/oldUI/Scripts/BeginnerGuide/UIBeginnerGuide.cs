using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DashFire;

public class UIBeginnerGuide{
  //初始化新手引导事件
  public delegate void OnGuideGameStart();
  public OnGuideGameStart onGuideGameStart; 
  public void InitGuide(OnGuideGameStart guideEvent)
  {
    onGuideGameStart = guideEvent;
    UIManager.Instance.OnAllUiLoadedDelegate += ShowStartUI;
  }
  public void ShowStartUI()
  {
    UIManager.Instance.LoadWindowByName("GuideStartGame", UICamera.mainCamera);
    UIManager.Instance.HideWindowByName("HeroPanel");
	UIManager.Instance.HideWindowByName("SkillBar");
    JoyStickInputProvider.JoyStickEnable = false;
  }
  public void ClearHandler()
	{
		UIManager.Instance.OnAllUiLoadedDelegate -= ShowStartUI;
	}

  //移动小手到普通攻击按钮、显示一个普通攻击按钮
  public void TransHandInCommonAttact(int index)
  {
	  UIManager.Instance.ShowWindowByName("SkillBar");
    GameObject goHand = UIManager.Instance.GetWindowGoByName("GuideHand");
    if(goHand==null)
      goHand = UIManager.Instance.LoadWindowByName("GuideHand", UICamera.mainCamera);
    GameObject goSkillbar = UIManager.Instance.GetWindowGoByName("SkillBar");
    if (goHand != null && goSkillbar!=null) {
      SkillBar sb = goSkillbar.GetComponent<SkillBar>();
      if (sb != null) {
        if (sb.spAshEx != null) NGUITools.SetActive(sb.spAshEx.gameObject, false);
        if(index==1)sb.InitSkillBar(null);//关掉所有技能按钮
        if (sb.CommonSkillGo != null) {
          Vector3 pos = sb.CommonSkillGo.transform.position;
          pos = UICamera.mainCamera.WorldToScreenPoint(pos);
          UISprite sp = sb.CommonSkillGo.GetComponent<UISprite>();
          if (sp != null) {
            //640、720为UIRoot设置的俩值
            float scale = 1f;
            if (Screen.height < UIManager.UIRootMinimumHeight)
              scale = Screen.height / (float)UIManager.UIRootMinimumHeight;
            if (Screen.height > UIManager.UIRootMaximunHeight)
              scale = Screen.height / (float)UIManager.UIRootMaximunHeight;
            pos = new Vector3(pos.x-(sp.width/2 * scale), pos.y + (sp.height/2) * scale, 0);
          }
          pos = UICamera.mainCamera.ScreenToWorldPoint(pos);
          goHand.transform.position = pos;
        }
      }
    }
	UIManager.Instance.ShowWindowByName("GuideHand");
  }
  //移动小手到第一个技能按钮
  public void TransHandInFirstSkill()
  {
    GameObject goHand = UIManager.Instance.GetWindowGoByName("GuideHand");
    GameObject goSkillbar = UIManager.Instance.GetWindowGoByName("SkillBar");
    if (goHand != null && goSkillbar != null) {
      RoleInfo role_info = LobbyClient.Instance.CurrentRole;
      SkillBar sb = goSkillbar.GetComponent<SkillBar>();
      if (sb != null) {
        infos.Clear();
        foreach (SkillInfo info in role_info.SkillInfos) {
          if (info.ConfigData.Category == SkillCategory.kSkillA) {
            infos.Add(info);
            sb.InitSkillBar(infos);
            break;
          }
        }
        //sb.InitSkillBar(role_info.SkillInfos);
        //sb.UnlockSkill(SkillCategory.kSkillA, true);
      }
      Transform tsSkillA = goSkillbar.transform.Find("Skill0/skill0");
      if (tsSkillA != null) {
        Vector3 pos = tsSkillA.position;
        goHand.transform.position = pos;
      }
    }
    UIManager.Instance.ShowWindowByName("GuideHand");
  }
  //显示回到主城按钮
  public void ShowReturnButton()
  {
	  UIManager.Instance.HideWindowByName("GuideHand");
	  UIManager.Instance.LoadWindowByName("ReturnToMaincity", UICamera.mainCamera);
  }
	//设置技能按钮不可用
  public void SetSkillBarActive(bool active)
  {
    GameObject go = UIManager.Instance.GetWindowGoByName("SkillBar");
    if (null != go) {
      UIButton[] skillBtnArr = go.GetComponentsInChildren<UIButton>();
      foreach (UIButton btn in skillBtnArr) {
        btn.isEnabled = active;
      }
    }
  }
  //
  private bool m_IsShow = false;
  public void ShowGuideDlgInControl(Vector2 center, float y)
  {
    if (!m_IsShow) {
      m_IsShow = true;
      //
      UIManager.Instance.HideWindowByName("SkillBar");
      float scale = 1f;
      if (Screen.height > UIManager.UIRootMaximunHeight)
        scale = Screen.height / (float)UIManager.UIRootMaximunHeight;
      if (Screen.height < UIManager.UIRootMinimumHeight)
        scale = Screen.height / (float)UIManager.UIRootMinimumHeight;
      Vector3 pos = new Vector3(center.x - c_OffsetX * scale, center.y + y * 2 / 3, 0f);
      pos = UICamera.mainCamera.ScreenToWorldPoint(pos);
      GameObject goGuideDlg = UIManager.Instance.GetWindowGoByName("GuideDlg");
      if (goGuideDlg == null) {
        goGuideDlg = UIManager.Instance.LoadWindowByName("GuideDlg", UICamera.mainCamera);
      }
      if (goGuideDlg != null) {
        goGuideDlg.transform.position = pos;
        UIGuideDlg guideDlg = goGuideDlg.GetComponent<UIGuideDlg>();
        if (guideDlg != null) guideDlg.SetDescription(501);
      }
    }
  }
  public void ShowGuideDlgAboveCommon(int index)
  {
    Vector3 pos = new Vector3(20, 300, 0);
    GameObject goSkillBar = UIManager.Instance.GetWindowGoByName("SkillBar");
    if (goSkillBar != null) {
      SkillBar skillBar = goSkillBar.GetComponent<SkillBar>();
      if (skillBar != null && skillBar.CommonSkillGo!=null) {
        pos = skillBar.CommonSkillGo.transform.position;
        pos = UICamera.mainCamera.WorldToScreenPoint(pos);
        UISprite sp = skillBar.CommonSkillGo.GetComponent<UISprite>();
        if (sp != null) {
          //640、768为UIRoot设置的俩值
          float scale = 1f;
          if (Screen.height < UIManager.UIRootMinimumHeight)
            scale = Screen.height / (float)UIManager.UIRootMinimumHeight;
          if (Screen.height > UIManager.UIRootMaximunHeight)
            scale = Screen.height / (float)UIManager.UIRootMaximunHeight;
          pos = new Vector3(pos.x - (sp.width/2 - c_OffsetX)*scale, pos.y + sp.height * scale+ c_OffsetY, 0);
        }
        pos = UICamera.mainCamera.ScreenToWorldPoint(pos);
      }
    }
    
    GameObject goGuideDlg = UIManager.Instance.GetWindowGoByName("GuideDlgRight");
    if (goGuideDlg == null) {
      goGuideDlg = UIManager.Instance.LoadWindowByName("GuideDlgRight", UICamera.mainCamera);
    }
    if (goGuideDlg != null) {
      goGuideDlg.transform.position = pos;
      UIGuideDlg guideDlg = goGuideDlg.GetComponent<UIGuideDlg>();
      if(guideDlg!=null){
        if (index == 1) guideDlg.SetDescription(502);
        if (index == 2) guideDlg.SetDescription(505);
      }
    }
    UIManager.Instance.ShowWindowByName("GuideDlgRight");
  }
  public void ShowGuideDlgAboveSkillA(int index)
  {
    Vector3 pos = new Vector3(20, 300, 0);
    GameObject goSkillBar = UIManager.Instance.GetWindowGoByName("SkillBar");
    if (goSkillBar != null) {
      Transform tsSkillA = goSkillBar.transform.Find("Skill0/skill0");
      if (tsSkillA != null) {
        pos = tsSkillA.position;
        pos = UICamera.mainCamera.WorldToScreenPoint(pos);
        UISprite sp = tsSkillA.GetComponent<UISprite>();
        if (sp != null) {
          float scale = 1f;
          if (Screen.height < UIManager.UIRootMinimumHeight)
            scale = Screen.height / (float)UIManager.UIRootMinimumHeight;
          if (Screen.height > UIManager.UIRootMaximunHeight)
            scale = Screen.height / (float)UIManager.UIRootMaximunHeight;
          pos = new Vector3(pos.x + c_OffsetX*scale , pos.y + sp.height * scale, 0);
        }
        pos = UICamera.mainCamera.ScreenToWorldPoint(pos);
      }
    }
    GameObject goGuideDlg = UIManager.Instance.GetWindowGoByName("GuideDlgRight");
    if (goGuideDlg == null) {
      goGuideDlg = UIManager.Instance.LoadWindowByName("GuideDlgRight", UICamera.mainCamera);
    }
    if (goGuideDlg != null) {
      goGuideDlg.transform.position = pos;
      UIGuideDlg guideDlg = goGuideDlg.GetComponent<UIGuideDlg>();
      if (guideDlg != null) {
        if (index == 1) guideDlg.SetDescription(503);
        if (index == 2) guideDlg.SetDescription(504);
      }
    }
    UIManager.Instance.ShowWindowByName("GuideDlgRight");
  }
  //让普攻图标失效
  public void SetCommonSkillBtnActive(bool active)
  {
    GameObject go = UIManager.Instance.GetWindowGoByName("SkillBar");
    if (go == null) return;
    SkillBar skillBar = go.GetComponent<SkillBar>();
    if (skillBar != null && skillBar.CommonSkillGo != null) {
      UIButton btn = skillBar.CommonSkillGo.GetComponent<UIButton>();
      if (btn != null) btn.isEnabled = active;
    }
  }
  //这是 c_OffsetX = 45 是根据GuideDlg控件箭头到左边距的大小来定的
  // c_OffsetY 
  private const float c_OffsetX = 45f;
  private const float c_OffsetY = 25f;
  public List<SkillInfo> infos = new List<SkillInfo>();
  public static UIBeginnerGuide Instance
  {
    get
    {
      return m_Instance;
    }
  }
  private static UIBeginnerGuide m_Instance = new UIBeginnerGuide();
}
