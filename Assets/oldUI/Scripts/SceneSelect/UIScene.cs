using UnityEngine;
using System;
using System.Collections;
using DashFire;

public class UIScene : MonoBehaviour {

  public UILabel lblName = null;
  public UISprite spSceneImage = null;
  public UISprite spLock = null;
  public GameObject goMasterIcon = null;
  public Vector3 masterModePos = new Vector3(0,-68.9f,0);
  private Vector3 commonModePos = new Vector3(0,-37f,0);
  private GameObject[] starArr = new GameObject[3];

  private const string c_MasterSpirite = "hong-qi-zi";
  private const string c_CommonSpirite = "hong-qi-zi_1";
  private int m_SceneId = -1;
  private int m_SceneGrade = -1;
  private SubSceneType m_SubType = SubSceneType.Common;

  public float TweenDuration = 0.67f;
  public Vector3 TweenFromScale = new Vector3(1.0f, 1.0f, 1.0f);
  public Vector3 TweenToScale = new Vector3(1.1f, 1.1f, 1.1f);
  public UITweener.Style TweenStyle = UITweener.Style.PingPong;


	// Use this for initialization
   void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
  public void InitStarArr()
  {
    for (int index = 1; index <= 3; ++index) {
      string name = "star" + index;
      Transform star = this.transform.Find(name);
      if (star != null && index <= starArr.Length) {
        starArr[index - 1] = star.gameObject;
      }
    }
  }
  public void InitScene(int sceneId, bool isLock,SubSceneType subType,int grade)
	{
    if (sceneId == -1) {
      NGUITools.SetActive(this.gameObject, false);
    } else {
      if(!NGUITools.GetActive(this.gameObject))
        NGUITools.SetActive(this.gameObject, true);
    }
		InitStarArr();
    m_SceneId = sceneId;
    m_SceneGrade = grade;
    m_SubType = subType;
    LockScene(isLock);
    Data_SceneConfig sceneCfg = SceneConfigProvider.Instance.GetSceneConfigById(sceneId);
    if (null != sceneCfg) {
      SetSceneName((sceneCfg.m_Order +1).ToString());
    }
    ShowSceneMode(subType, grade);
  }
  public void SetSceneName(string name)
  { 
    if (null != lblName)
      lblName.text =name;
  }
  //点击跳转到场景介绍界面
  void OnClick()
  {
	  //UIManager.Instance.HideWindowByName("SceneSelect");
    UIManager.Instance.ShowWindowByName("SceneIntroduce");
    LogicSystem.EventChannelForGfx.Publish("ge_init_sceneintroduce", "ui", m_SceneId,m_SceneGrade,m_SubType);
    
  }
  public int GetSceneId()
  {
    return m_SceneId;
  }
  public void HightLight()
  {
    
    TweenScale tween = TweenScale.Begin(this.gameObject, TweenDuration, TweenToScale);
    if (tween != null) {
      tween.style = TweenStyle;
      tween.from = TweenFromScale;
    }
  }
  public void DelTweenOnScene()
  {
    TweenScale tween = this.GetComponent<TweenScale>();
    if (tween != null) Destroy(tween);
  }
  public void LockScene(bool islock)
  {
    if (spLock != null) {
      NGUITools.SetActive(spLock.gameObject, islock);
      UIButton uiButton = this.GetComponent<UIButton>();
      if (uiButton != null) {
        //临时注释掉
        uiButton.isEnabled = !islock;
      }
    }
  }
  //显示与隐藏精英模式
  public void ShowSceneMode(SubSceneType subType,int grade)
  {
    bool visible = subType == SubSceneType.Master ? true : false;
    if (goMasterIcon != null)
      NGUITools.SetActive(goMasterIcon, visible);
    if (subType == SubSceneType.Master) {
      if (spSceneImage != null) {
        UIButton btn = this.GetComponent<UIButton>();
        if (btn != null) btn.normalSprite = c_MasterSpirite;
        spSceneImage.spriteName = c_MasterSpirite;
        spSceneImage.transform.localPosition = masterModePos;
        for (int i = 0; i < starArr.Length; ++i) {
          if (i < grade) {
            if (starArr[i] != null) NGUITools.SetActive(starArr[i], true);
          } else {
            if (starArr[i] != null) NGUITools.SetActive(starArr[i], false);
          }
        }
      }
    } else {
      if (spSceneImage != null) {
        UIButton btn = this.GetComponent<UIButton>();
        if (btn != null) btn.normalSprite = c_CommonSpirite;
        spSceneImage.spriteName = c_CommonSpirite;
        spSceneImage.transform.localPosition = commonModePos;
        for (int i = 0; i < starArr.Length; ++i) {
            if (starArr[i] != null) NGUITools.SetActive(starArr[i], false);
        }
      }
    }
  }

}
