using System;
using System.Collections.Generic;

namespace DashFire
{
  public class LobbyClient
  {
    public static LobbyClient Instance
    {
      get { return s_Instance; }
    }
    private static LobbyClient s_Instance = new LobbyClient();

    public AccountInfo AccountInfo
    {
      get { return m_AccountInfo; }
    }
    public DeviceInfo Device
    {
      get { return m_Deviceinfo; }
    }
    public RoleInfo CurrentRole
    {
      get { return m_CurrentRole; }
      set { m_CurrentRole = value; }
    }

    private AccountInfo m_AccountInfo = new AccountInfo();    //玩家账号信息（服务器验证通过的）
    private DeviceInfo m_Deviceinfo = new DeviceInfo();       //客户端硬件设备信息
    private RoleInfo m_CurrentRole = null;                    //当前游戏的玩家角色
  }
}
