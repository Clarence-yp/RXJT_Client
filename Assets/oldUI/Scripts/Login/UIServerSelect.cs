using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DashFire;

public class UIServerSelect : MonoBehaviour {

  public GameObject goItemContainer = null;
  public GameObject goTweenContainer = null;
  public UIServerItem recentLogin = null;
  public AnimationCurve CurveForDown = null;
  public AnimationCurve CurveForUpwards = null;
  public float DurationForDown = 0.3f;
  public float DurationForUpwards = 0.2f;
  public float TweenOffset = 500f;
  private Transform trans = null;
  private UIRoot uiRoot = null;
  private int m_SelectServerId = -1;
  private List<object> m_EventList = new List<object>();
	// Use this for initialization
  public void UnSubscribe()
  {
    try {
      foreach (object obj in m_EventList) {
        if (null != obj) LogicSystem.EventChannelForGfx.Unsubscribe(obj);
      }
      m_EventList.Clear();
    }
    catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  void Awake()
  {
    object obj = LogicSystem.EventChannelForGfx.Subscribe<int>("ge_recent_server", "ui", SetRecentServer);
    if (null != obj) m_EventList.Add(obj);
    obj = LogicSystem.EventChannelForGfx.Subscribe("ge_ui_unsubscribe", "ui", UnSubscribe);
    if (obj != null) m_EventList.Add(obj);
    trans = this.transform;
  }
  void Start () {
    Init();
    if(goTweenContainer!=null)
       goTweenContainer.transform.localPosition = new Vector3(0, 1000f, 0);
  }
	// Update is called once per frame
	void Update () {
	}
  public void Init()
  {
    if (goItemContainer == null) return;
    UIGrid grid = this.GetComponentInChildren<UIGrid>();
    if(grid ==null)return;
    MyDictionary<int, object> serverConfigDic = ServerConfigProvider.Instance.GetData();
    int serverIndex = 0;
    GameObject go = null;
    foreach (ServerConfig cfg in serverConfigDic.Values) {
      if(serverIndex%2==0)
        go = NGUITools.AddChild(grid.gameObject, goItemContainer);
      if (go != null) {
        UIServerItemContainer itemContainer = go.GetComponent<UIServerItemContainer>();
        if (itemContainer != null) {
          itemContainer.SetServerItem(serverIndex % 2, cfg.ServerId, cfg.ServerName, "");
          serverIndex++;
        }
      }
      grid.Reposition();
    }
    if (serverIndex % 2 == 1 && go!=null) {
      UIServerItemContainer con = go.GetComponent<UIServerItemContainer>();
      if (con != null) con.HideServerItem();
    }
  }
  public void SetRecentServer(int serverId)
  {
    try {
      if (recentLogin != null) {
        ServerConfig cfg = ServerConfigProvider.Instance.GetDataById(serverId);
        if (cfg != null) {
          recentLogin.SetServerInfo(cfg.ServerId, cfg.ServerName, "火爆");
        }
      }
    }
    catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  void OnEnable()
  {
    TweenOffset = Screen.height;
    if (uiRoot == null) {
      uiRoot = NGUITools.FindInParents<UIRoot>(gameObject);
    }
    if (uiRoot != null) {
      if (Screen.height > uiRoot.maximumHeight)
        TweenOffset = uiRoot.maximumHeight;
      if (Screen.height < uiRoot.minimumHeight)
        TweenOffset = uiRoot.minimumHeight;
    }
      if (goTweenContainer != null) {
        TweenPosition tweenPos = TweenPosition.Begin(goTweenContainer, DurationForDown, new Vector3(0, TweenOffset / 2, 0f));
        if (tweenPos != null) {
          //tweenPos.style = UITweener.Style.Loop;
          tweenPos.animationCurve = CurveForDown;
          tweenPos.from = new Vector3(0, TweenOffset * 1.5f, 0);
        }
      }
  }
  //向上移动至消失
  public void TweenUpwards(int serverId)
  {
    if (goTweenContainer != null) {
      m_SelectServerId = serverId;
      TweenPosition tweenPos = TweenPosition.Begin(this.goTweenContainer, DurationForUpwards, new Vector3(0, TweenOffset*1.5f, 0f));
      if (tweenPos != null) {
        tweenPos.animationCurve = CurveForUpwards;
        EventDelegate.Add(tweenPos.onFinished, OnTweenUpwardsFinished);
      }
    }
  }
  private void OnTweenUpwardsFinished()
  {
    UIManager.Instance.HideWindowByName("ServerSelect");
    GameObject goLogin = UIManager.Instance.GetWindowGoByName("LoginPrefab");
    if (goLogin != null) {
      Login uiLogin = goLogin.GetComponent<Login>();
      if (uiLogin != null) uiLogin.TweenDownServerBtn();
    }
    DashFire.GfxSystem.PublishGfxEvent("ge_set_current_server", "ui", m_SelectServerId);
    TweenPosition tween = goTweenContainer.GetComponent<TweenPosition>();
    if (null != tween) Destroy(tween);
  }
}
