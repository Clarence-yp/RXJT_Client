using System;
using UnityEngine;
using System.Collections.Generic;
using DashFire;

public enum UIType : int
{
  DontLoad = -1,
  NoneActive = 0,
  Active = 1
}
public enum SecenType : int
{
  LoginScene = 0,
  MainCityScene = 1,
  PveScene = 2,
  PvpScene = 3,
}

public class UIManager
{
  public delegate void VoidDelegate();
  public VoidDelegate OnAllUiLoadedDelegate;
  public void Init(GameObject rootWindow)
  {
    DashFire.LogicSystem.EventChannelForGfx.Subscribe<string>("show_ui", "ui", ShowWindowByName);
    DashFire.LogicSystem.EventChannelForGfx.Subscribe<string>("hide_ui", "ui", HideWindowByName);
    m_RootWindow = rootWindow;
    uiConfigDataDic = UiConfigProvider.Instance.GetData();
  }

  public void Clear()
  {
    m_IsLoadedWindow.Clear();
    m_VisibleWindow.Clear();
    m_UnVisibleWindow.Clear();
    m_ExclusionWindow.Clear();
  }
  //获取已经加载的窗口GameObject
  public GameObject GetWindowGoByName(string windowName)
  {
    if (windowName == null) return null;
    if (m_IsLoadedWindow.ContainsKey(windowName.Trim()))
      return m_IsLoadedWindow[windowName];
    return null;
  }
  //获取UI的路径
  public string GetPathByName(string windowName)
  {
    UiConfig uiCfg = GetUiConfigByName(windowName);
    if (uiCfg != null) {
      return uiCfg.m_WindowPath;
    }
    return null;
  }
  public UiConfig GetUiConfigByName(string name)
  {
    foreach (UiConfig uiCfg in uiConfigDataDic.Values) {
      if (uiCfg != null && uiCfg.m_WindowName == name) {
        return uiCfg;
      }
    }
    return null;
  }
  public GameObject LoadWindowByName(string windowName, Camera cam)
  {
    GameObject window = null;
    UiConfig uiCfg = GetUiConfigByName(windowName);
    if (null != uiCfg) {
      window = DashFire.ResourceSystem.GetSharedResource(uiCfg.m_WindowPath) as GameObject;
      if (null != window) {
        window = NGUITools.AddChild(m_RootWindow, window);
        Vector3 screenPos = CalculateUiPos(uiCfg.m_OffsetLeft, uiCfg.m_OffsetRight, uiCfg.m_OffsetTop, uiCfg.m_OffsetBottom);
        if (null != window && cam != null)
          window.transform.position = cam.ScreenToWorldPoint(screenPos);
        string name = uiCfg.m_WindowName;
        while (m_IsLoadedWindow.ContainsKey(name)) {
          name += "ex";
        }
        m_IsLoadedWindow.Add(name, window);
        m_VisibleWindow.Add(name, window);
        return window;
      } else {
        Debug.Log("!!!load " + uiCfg.m_WindowPath + " failed");
      }
    } else {
      Debug.Log("!!!load " + windowName + " failed");
    }
    return null;
  }

  public void LoadAllWindows(int sceneType, Camera cam)
  {
    if (null == m_RootWindow)
      return;
    foreach (UiConfig info in uiConfigDataDic.Values) {
      if (info.m_ShowType != (int)(UIType.DontLoad) && sceneType == info.m_OwnToSceneId) {
        //Debug.Log(info.m_WindowName);
        GameObject go = DashFire.ResourceSystem.GetSharedResource(info.m_WindowPath) as GameObject;
        if (go == null) {
          Debug.Log("!!!Load ui " + info.m_WindowPath + " failed.");
          continue;
        }
        GameObject child = NGUITools.AddChild(m_RootWindow, go);
        if (info.m_ShowType == (int)(UIType.Active)) {
          NGUITools.SetActive(child, true);
          if (!m_VisibleWindow.ContainsKey(info.m_WindowName)) {
            m_VisibleWindow.Add(info.m_WindowName, child);
          }
        } else {
          NGUITools.SetActive(child, false);
          if (!m_UnVisibleWindow.ContainsKey(info.m_WindowName)) {
            m_UnVisibleWindow.Add(info.m_WindowName, child);
          }
        }
        Vector3 screenPos = CalculateUiPos(info.m_OffsetLeft, info.m_OffsetRight, info.m_OffsetTop, info.m_OffsetBottom);
        if (!m_IsLoadedWindow.ContainsKey(info.m_WindowName)) {
          m_IsLoadedWindow.Add(info.m_WindowName, child);
        }
        if (null != child && cam != null)
          child.transform.position = cam.ScreenToWorldPoint(screenPos);
      }
    }
    IsUIVisible = true;
  }
  public void UnLoadAllWindow()
  {
    //每一个订阅事件的窗口UI都需要一个UnSubscribe函数用于消除事件
    LogicSystem.EventChannelForGfx.Publish("ge_ui_unsubscribe", "ui");
    foreach (GameObject window in m_IsLoadedWindow.Values) {
      if (null != window)
        //NGUIDebug.Log(window.name);
        NGUITools.DestroyImmediate(window);
    }
    Clear();
  }
  //卸载窗口
  public void UnLoadWindowByName(string name)
  {
    GameObject go = GetWindowGoByName(name);
    if (go != null) {
      NGUITools.Destroy(go);
      m_IsLoadedWindow.Remove(name);
    }
  }
  public void ShowWindowByName(string windowName)
  {
    try {
      if (windowName == null) return;
      if (m_VisibleWindow.ContainsKey(windowName))
        return;
      if (m_UnVisibleWindow.ContainsKey(windowName)) {
        GameObject window = m_UnVisibleWindow[windowName];
        if (null != window) {
          NGUITools.SetActive(window, true);
          m_VisibleWindow.Add(windowName, window);
          m_UnVisibleWindow.Remove(windowName);
        }
      }
      UiConfig uiCfg = GetUiConfigByName(windowName);
      if (uiCfg != null && uiCfg.m_IsExclusion == true) {
        CloseExclusionWindow(windowName);
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private void ShowWindow(string windowName)
  {
    if (windowName == null) return;
    if (m_VisibleWindow.ContainsKey(windowName))
      return;
    if (m_UnVisibleWindow.ContainsKey(windowName)) {
      GameObject window = m_UnVisibleWindow[windowName];
      if (null != window) {
        NGUITools.SetActive(window, true);
        m_VisibleWindow.Add(windowName, window);
        m_UnVisibleWindow.Remove(windowName);
      }
    }
  }
  public void HideWindowByName(string windowName)
  {
    try {
      if (windowName == null) return;
      if (m_UnVisibleWindow.ContainsKey(windowName))
        return;

      if (m_VisibleWindow.ContainsKey(windowName)) {
        GameObject window = m_VisibleWindow[windowName];
        if (null != window) {
          NGUITools.SetActive(window, false);
          m_UnVisibleWindow.Add(windowName, window);
          m_VisibleWindow.Remove(windowName);
        }
      }
      UiConfig uiCfg = GetUiConfigByName(windowName);
      if (uiCfg != null && uiCfg.m_IsExclusion == true) {
        OpenExclusionWindow(windowName);
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private void HideWindow(string windowName)
  {
    if (windowName == null) return;
    if (m_UnVisibleWindow.ContainsKey(windowName))
      return;

    if (m_VisibleWindow.ContainsKey(windowName)) {
      GameObject window = m_VisibleWindow[windowName];
      if (null != window) {
        NGUITools.SetActive(window, false);
        m_UnVisibleWindow.Add(windowName, window);
        m_VisibleWindow.Remove(windowName);
      }
    }
  }
  public void ToggleWindowVisible(string windowName)
  {
    if (IsWindowVisible(windowName)) {
      HideWindowByName(windowName);
    } else {
      ShowWindowByName(windowName);
    }
  }
  public bool IsWindowVisible(string windowName)
  {
    if (m_VisibleWindow.ContainsKey(windowName))
      return true;
    else {
      return false;
    }
  }
  //关闭除windowName之外的所有窗口
  public void CloseExclusionWindow(string windowName)
  {
    foreach (string name in m_VisibleWindow.Keys) {
      if (name != windowName) {
        m_ExclusionWindow.Add(name);
      }
    }
    foreach (string name in m_ExclusionWindow) {
      HideWindow(name);
    }
    if (DFMUiRoot.InputMode == InputType.Joystick) {
      JoyStickInputProvider.JoyStickEnable = false;
    }
  }
  //打开之前关闭的窗口
  public void OpenExclusionWindow(string windowName)
  {
    //
    foreach (string name in m_ExclusionWindow) {
      ShowWindow(name);
    }
    m_ExclusionWindow.Clear();
    if (DFMUiRoot.InputMode == InputType.Joystick) {
      JoyStickInputProvider.JoyStickEnable = true;
    }
  }
  public void SetAllUiVisible(bool isVisible)
  {
    if (isVisible) {
      TouchManager.TouchEnable = true;
      OpenExclusionWindow("");
    } else {
      TouchManager.TouchEnable = false;
      CloseExclusionWindow("");
    }
    IsUIVisible = isVisible;
    NicknameAndMoney(isVisible);
  }
  void NicknameAndMoney(bool vis)
  {
    if (m_RootWindow != null) {
      Transform tf = m_RootWindow.transform.Find("DynamicWidget");
      if (tf != null) {
        NGUITools.SetActive(tf.gameObject, vis);
      }
      tf = m_RootWindow.transform.Find("PveFightInfo(Clone)");
      if (tf != null) {
        NGUITools.SetActive(tf.gameObject, vis);
      }
      tf = m_RootWindow.transform.Find("ScreenScrollTip");
      if (tf != null) {
        NGUITools.SetActive(tf.gameObject, vis);
      }
    }
  }
  public Vector3 CalculateUiPos(float offsetL, float offsetR, float offsetT, float offsetB)
  {
    float screen_width = 0;
    float screen_height = 0;
    if (UICamera.mainCamera != null) {
      screen_width = UICamera.mainCamera.pixelRect.width;
      screen_height = UICamera.mainCamera.pixelRect.height;
    } else {
      screen_width = Screen.width;
      screen_height = Screen.height;
    }

    Vector3 screenPos = new Vector3();
    if (offsetL == -1 && offsetR == -1) {
      screenPos.x = screen_width / 2;
    } else {
      if (offsetL != -1)
        screenPos.x = offsetL;
      else {
        screenPos.x = screen_width - offsetR;
      }
    }
    if (offsetT == -1 && offsetB == -1) {
      screenPos.y = screen_height / 2;
    } else {
      if (offsetT != -1) {
        screenPos.y = screen_height - offsetT;
      } else {
        screenPos.y = offsetB;
      }
    }
    screenPos.z = 0;
    return screenPos;
  }
  static private UIManager m_Instance = new UIManager();
  static public UIManager Instance
  {
    get { return m_Instance; }
  }
  static public string GetItemProtetyStr(float data, int type)
  {
    string str = "";
    switch (type) {
      case (int)DashFire.ItemAttrDataConfig.ValueType.AbsoluteValue:
        str = (data > 0.0f ? "+" : "") + Mathf.FloorToInt(data);
        break;
      case (int)DashFire.ItemAttrDataConfig.ValueType.PercentValue:
        str = (data > 0.0f ? "+" : "") + Mathf.FloorToInt(data * 100) + "%";
        break;
      case (int)DashFire.ItemAttrDataConfig.ValueType.LevelRateValue:
        str = (data > 0.0f ? "+" : "") + Mathf.FloorToInt(data);
        break;
      default:
        str = "No This Item Type!";
        break;
    }
    return str;
  }
  static public float GetItemPropertyData(float data, int type)
  {
    float dataf = data;
    switch (type) {
      case (int)DashFire.ItemAttrDataConfig.ValueType.AbsoluteValue:
        dataf = (float)(Mathf.FloorToInt(data));
        break;
      case (int)DashFire.ItemAttrDataConfig.ValueType.PercentValue:
        dataf = (float)(Mathf.FloorToInt(data * 100) / 100.0f);
        break;
      case (int)DashFire.ItemAttrDataConfig.ValueType.LevelRateValue:
        break;
      default:
        break;
    }
    return dataf;
  }
  static public int UIRootMinimumHeight = 640;
  static public int UIRootMaximunHeight = 768;
  static public UnityEngine.Color SkillDrectorColor = new UnityEngine.Color(255, 255, 255);
  static public List<GameObject> CheckItemForDelete = new List<GameObject>();
  static public float dragtime = 0.0f;
  public bool IsUIVisible = true;
  public int MarsIntegral = 0;
  private GameObject m_RootWindow = null;
  private List<string> m_ExclusionWindow = new List<string>();
  private Dictionary<string, GameObject> m_IsLoadedWindow = new Dictionary<string, GameObject>();
  private Dictionary<string, GameObject> m_VisibleWindow = new Dictionary<string, GameObject>();
  private Dictionary<string, GameObject> m_UnVisibleWindow = new Dictionary<string, GameObject>();
  public Dictionary<string, WindowInfo> m_WindowsInfoDic = new Dictionary<string, WindowInfo>();
  MyDictionary<int, object> uiConfigDataDic = new MyDictionary<int, object>();
}