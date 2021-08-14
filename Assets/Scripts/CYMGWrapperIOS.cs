using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public static class CYMGWrapperIOS
{
  [DllImport("__Internal")]
  private static extern void _InitCYMG(int isDebug);
  [DllImport("__Internal")]
  private static extern void _StartLogin(int isAuto);
  [DllImport("__Internal")]
  private static extern void _OnLoginBillingSuccess(string accountId);
  [DllImport("__Internal")]
  private static extern void _StartLogout();
  [DllImport("__Internal")]
  private static extern void _ShowCallCenter(); 
  //[DllImport("__Internal")]
  //private static extern void _RequestGoodsList();
  //[DllImport("__Internal")]
  //private static extern void _StartPayment(string roleId, string groupId);
  [DllImport("__Internal")]
  private static extern void _MBIOnLogin(string account, string server, string roleName, string roleId, int level, string userId);
  [DllImport("__Internal")]
  private static extern void _GetUniqueIdentifier();

  //初始化畅游平台OpenSDK
  public static void InitCYMG(bool isDebug)
  {
    if (Application.platform  == RuntimePlatform.IPhonePlayer) {
      int flag = 0;
      if (isDebug == true) {
        flag = 1;
      }
      _InitCYMG(flag);
    }
  }
  //账号登录
  public static void StartLogin(bool isAuto)
  {
    if (Application.platform == RuntimePlatform.IPhonePlayer) {
      int flag = 0;
      if (isAuto == true) {
        flag = 1;
      }
      _StartLogin(flag);
    }
  }
  //账号登录成功，服务器验证通过
  public static void OnLoginBillingSuccess(string accountId)
  {
    if (Application.platform == RuntimePlatform.IPhonePlayer) {
      _OnLoginBillingSuccess(accountId);
    }
  }
  //账号注销
  public static void StartLogout()
  {
    if (Application.platform == RuntimePlatform.IPhonePlayer) {
      _StartLogout();
    }
  }
  //显示客服中心
  public static void ShowCallCenter()
  {
    if (Application.platform == RuntimePlatform.IPhonePlayer) {
      _ShowCallCenter();
    }
  }
  //设备标示符
  public static void GetUniqueIdentifier()
  {
    if (Application.platform == RuntimePlatform.IPhonePlayer) {
      _GetUniqueIdentifier();
    }
  }
  /*
  //请求商品列表
  public static void RequestGoodsList()
  {
    if (Application.platform == RuntimePlatform.IPhonePlayer) {
      _RequestGoodsList();
    }
  }
//充值支付
public static void StartPayment(string roleId, string groupId)
{
if (Application.platform == RuntimePlatform.IPhonePlayer) {
  _StartPayment(roleId, groupId);
}
}
*/
  //MBI 玩家角色进入游戏日志
  public static void MBIOnLogin(string account, string server, string roleName, string roleId, int level, string userId)
  {
    if (Application.platform == RuntimePlatform.IPhonePlayer) {
      _MBIOnLogin(account, server, roleName, roleId, level, userId);
    }
  }
}
