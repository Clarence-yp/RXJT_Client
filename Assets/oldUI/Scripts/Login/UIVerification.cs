using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DashFire;

public class UIVerification : MonoBehaviour
{
  public enum ActivateAccountState : int
  {
    Activated = 0,  //已激活
    Unactivated,    //未激活    
    Banned,         //账号封停
  }
  public enum ActivateAccountResult : int
  {
    Success = 0,    //激活成功
    InvalidCode,    //失效的激活码（该激活码已经被使用）
    MistakenCode,   //错误的激活码（该激活码不存在）
    Error,          //激活失败(系统问题)
  }
  // Use this for initialization
  public UILabel lblHint = null;
  public UIInput uiInput = null;
  private List<object> m_EventList = new List<object>();
  private ActivateAccountState m_ActivateState = ActivateAccountState.Unactivated;
  private int m_AcivateFailureCount = 0;    
  private const int c_BanCount = 30;  
  private const float c_TimeoutInterval = 30.0f;    //激活未响应超时时间30s
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
    object obj = null;
    obj = LogicSystem.EventChannelForGfx.Subscribe<int>("ge_activate_result", "lobby", OnActivateResult);
    if (obj != null) m_EventList.Add(obj);
    obj = LogicSystem.EventChannelForGfx.Subscribe("ge_ui_unsubscribe", "ui", UnSubscribe);
    if (obj != null) m_EventList.Add(obj);

    m_AcivateFailureCount = PlayerPrefs.GetInt("AcivateFailureCount");
  }
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }
  public void OnReturn()
  {
    UIManager.Instance.HideWindowByName("Verification");
    UIManager.Instance.ShowWindowByName("LoginPrefab");
  }
  public void OnSubmit()
  {
    string hintStr = "";
    if (m_AcivateFailureCount > c_BanCount) {
      //封停
      m_ActivateState = ActivateAccountState.Banned;      
      hintStr = "激活码连续错误30次以上，账号封停";       
    }
    if (m_ActivateState == ActivateAccountState.Unactivated){
      if (uiInput != null) {
        string code = uiInput.value.Trim();
        if (code.Equals(String.Empty)) {
            hintStr = "激活码不能为空";            
        } else {
          LogicSystem.PublishLogicEvent("ge_activate_account", "lobby", code);
          Invoke("OnActivateTimeout", c_TimeoutInterval);
        }            
      }
    }
    if (lblHint != null) {
      lblHint.text = hintStr;
      NGUITools.SetActive(lblHint.gameObject, true);
    }
  }
  public void OnActivateResult(int ret)
  {
    try {
      CancelInvoke("OnActivateTimeout");
      string hintStr = "";
      if (ret == (int)ActivateAccountResult.Success) {
        //获取角色列表成功，跳转到角色选择页面
        m_AcivateFailureCount = 0;
        UIManager.Instance.HideWindowByName("Verification");
        DashFire.LogicSystem.EventChannelForGfx.Publish("ge_create_hero_scene", "ui", true);
      } else if (ret == (int)ActivateAccountResult.InvalidCode) {
        m_AcivateFailureCount++;
        hintStr = "激活码已失效，请重新输入。";      
      } else if (ret == (int)ActivateAccountResult.MistakenCode) {
        m_AcivateFailureCount++;
        hintStr = "激活码有误，请重新输入。";          
      } else {
        hintStr = "激活失败，请重新尝试。";        
      }
      if (lblHint != null) {
        lblHint.text = hintStr;
        NGUITools.SetActive(lblHint.gameObject, true);
      }
      PlayerPrefs.SetInt("AcivateFailureCount",m_AcivateFailureCount);      
    }
    catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  private void OnActivateTimeout()
  {
    this.OnActivateResult((int)ActivateAccountResult.Error);
  }
}
