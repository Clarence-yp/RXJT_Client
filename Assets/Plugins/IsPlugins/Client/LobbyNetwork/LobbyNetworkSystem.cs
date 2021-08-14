using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DashFire;
using LitJson;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace DashFire.Network
{
    public sealed partial class LobbyNetworkSystem
    {
        public void Init(IActionQueue asyncQueue)
        {
            //WebSocket的事件不是在当前线程触发的，我们需要自己进行线程调整
            m_AsyncActionQueue = asyncQueue;

            JsonMessageDispatcher.Init();
            LobbyMessageInit();
            FriendMessageInit();

            m_IsWaitStart = true;
            m_IsLogged = false;
            m_LastReceiveHeartbeatTime = 0;
        }
        public void ConnectIfNotOpen()
        {
            if (!IsConnected)
            {
                m_LastReceiveHeartbeatTime = 0;
                m_LastConnectTime = TimeUtility.GetLocalMilliseconds();
                LogSystem.Info("ConnectIfNotOpen at time:{0} ServerAddress:{1}", m_LastConnectTime, m_Url);

                WorldSystem.Instance.IsWaitMatch = false;

                m_WebSocket = new WebSocket4Net.WebSocket(m_Url);
                m_WebSocket.EnableAutoSendPing = true;
                m_WebSocket.AutoSendPingInterval = 10;
                m_WebSocket.Opened += OnWsOpened;
                m_WebSocket.MessageReceived += OnWsMessageReceived;
                m_WebSocket.DataReceived += OnWsDataReceived;
                m_WebSocket.Error += OnWsError;
                m_WebSocket.Closed += OnWsClosed;
                m_WebSocket.Open();
            }
        }
        public void Tick()
        {
            if (!m_IsWaitStart)
            {
                long curTime = TimeUtility.GetLocalMilliseconds();

                if (!IsConnected)
                {
                    if (m_LastConnectTime + 10000 < curTime && (WorldSystem.Instance.IsPureClientScene() || WorldSystem.Instance.IsPvpScene() || WorldSystem.Instance.IsMultiPveScene()))
                    {
                        ConnectIfNotOpen();
                        GfxSystem.PublishGfxEvent("ge_ui_connect_hint", "ui", false, true);
                    }
                }
                else
                {
                    if (m_IsLogged && m_LastConnectTime + 5000 < curTime)
                    {
                        if (m_LastHeartbeatTime + 5000 < curTime)
                        {
                            m_LastHeartbeatTime = curTime;
                            if (m_LastReceiveHeartbeatTime == 0)
                            {
                                m_LastReceiveHeartbeatTime = curTime;
                            }
                            SendHeartbeat();
                        }
                        if (m_LastReceiveHeartbeatTime > 0 && m_LastReceiveHeartbeatTime + 30000 < curTime)
                        {
                            //断开连接
                            if (IsConnected)
                            {
                                m_WebSocket.Close();
                                m_LastReceiveHeartbeatTime = 0;
                            }
                            //触发重连
                            m_LastConnectTime = curTime - 10000;
                        }
                    }
                }
            }
        }
        public void Release()
        {
            if (IsConnected)
            {
                m_WebSocket.Close();
            }
        }
        public void QuitRoom()
        {
            if (m_Guid != 0)
            {
                JsonData msg = new JsonData();
                msg.SetJsonType(JsonType.Object);
                msg.Set("m_Guid", m_Guid);
                SendMessage(JsonMessageID.QuitRoom, msg);
            }
        }
        public void QuitClient()
        {
            if (m_Guid != 0)
            {
                JsonData msg = new JsonData();
                msg.SetJsonType(JsonType.Object);
                msg.Set("m_Guid", m_Guid);
                SendMessage(JsonMessageID.Logout, msg);
            }
            if (LobbyClient.Instance.AccountInfo.Account != string.Empty)
            {
                JsonData msg = new JsonData();
                msg.SetJsonType(JsonType.Object);
                msg.Set("m_Account", m_Account);
                SendMessage(JsonMessageID.AccountLogout, msg);
            }
            m_IsWaitStart = true;
            m_IsLogged = false;
            if (IsConnected)
            {
                m_WebSocket.Close();
            }
        }
        public bool IsConnected
        {
            get
            {
                bool ret = false;
                if (null != m_WebSocket)
                    ret = (m_WebSocket.State == WebSocket4Net.WebSocketState.Open && m_WebSocket.IsSocketConnected);
                return ret;
            }
        }
        public ulong Guid
        {
            get { return m_Guid; }
        }

        internal bool SendMessage(string msgStr)
        {
            bool ret = false;
            try
            {
                if (IsConnected)
                {
                    m_WebSocket.Send(msgStr);
                    LogSystem.Info("SendToLobby {0}", msgStr);
                    ret = true;
                }
            }
            catch (Exception ex)
            {
                LogSystem.Error("SendMessage throw exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
            return ret;
        }

        private void SendHeartbeat()
        {
            JsonData msg = new JsonData();
            msg.SetJsonType(JsonType.Object);
            msg.Set("m_Guid", m_Guid);
            SendMessage(JsonMessageID.UserHeartbeat, msg);
        }

        private void HandleUserHeartbeat(JsonMessage lobbyMsg)
        {
            m_LastReceiveHeartbeatTime = TimeUtility.GetLocalMilliseconds();
        }

        private void RegisterMsgHandler(JsonMessageID id, JsonMessageHandlerDelegate handler)
        {
            JsonMessageDispatcher.RegisterMessageHandler((int)id, null, handler);
        }
        private void SendMessage(JsonMessageID id, JsonData msg)
        {
            try
            {
                JsonMessageDispatcher.SendMessage((int)id, msg);
            }
            catch (Exception ex)
            {
                LogSystem.Error("LobbyNetworkSystem.SendMessage throw Exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }
        private void RegisterMsgHandler(JsonMessageID id, Type t, JsonMessageHandlerDelegate handler)
        {
            JsonMessageDispatcher.RegisterMessageHandler((int)id, t, handler);
        }
        private void SendMessage(JsonMessage msg)
        {
            try
            {
                JsonMessageDispatcher.SendMessage(msg);
            }
            catch (Exception ex)
            {
                LogSystem.Error("LobbyNetworkSystem.SendMessage throw Exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
        }

        private void OnOpened()
        {
            //首先校验版本
            JsonData versionMsg = new JsonData();
            versionMsg.Set("m_Version", (uint)CodeVersion.Value);
            SendMessage(JsonMessageID.VersionVerify, versionMsg);
        }
        private void OnError(Exception ex)
        {
            if (null != ex)
            {
                LogSystem.Error("LobbyNetworkSystem.OnError Exception:{0}\n{1}", ex.Message, ex.StackTrace);
            }
            else
            {
                LogSystem.Error("LobbyNetworkSystem.OnError (Unknown)");
            }
        }
        private void OnClosed()
        {
            LogSystem.Error("LobbyNetworkSystem.OnClosed");
        }
        private void OnMessageReceived(string msg)
        {
            if (null != msg)
            {
                JsonMessageDispatcher.HandleNodeMessage(msg);
            }
        }
        private void OnDataReceived(byte[] data)
        {
            LogSystem.Info("Receive Data Message:\n{0}", Helper.BinToHex(data));
        }

        private void OnWsOpened(object sender, EventArgs e)
        {
            if (null != m_AsyncActionQueue)
            {
                m_AsyncActionQueue.QueueActionWithDelegation((MyAction)this.OnOpened);
            }
        }
        private void OnWsError(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            if (null != m_AsyncActionQueue)
            {
                m_AsyncActionQueue.QueueActionWithDelegation((MyAction<Exception>)this.OnError, e.Exception);
            }
        }
        private void OnWsClosed(object sender, EventArgs e)
        {
            if (null != m_AsyncActionQueue)
            {
                m_AsyncActionQueue.QueueActionWithDelegation((MyAction)this.OnClosed);
            }
        }
        private void OnWsMessageReceived(object sender, WebSocket4Net.MessageReceivedEventArgs e)
        {
            if (null != m_AsyncActionQueue)
            {
                m_AsyncActionQueue.QueueActionWithDelegation((MyAction<string>)this.OnMessageReceived, e.Message);
            }
        }
        private void OnWsDataReceived(object sender, WebSocket4Net.DataReceivedEventArgs e)
        {
            if (null != m_AsyncActionQueue)
            {
                m_AsyncActionQueue.QueueActionWithDelegation((MyAction<byte[]>)this.OnDataReceived, e.Data);
            }
        }

        private string GetIp()
        {
            string ip = "127.0.0.1";
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.Supports(NetworkInterfaceComponent.IPv4))
                {
                    UnicastIPAddressInformationCollection uniCast = adapter.GetIPProperties().UnicastAddresses;
                    if (uniCast.Count > 0)
                    {
                        foreach (UnicastIPAddressInformation uni in uniCast)
                        {
                            //得到IPv4的地址。 AddressFamily.InterNetwork指的是IPv4
                            if (uni.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                ip = uni.Address.ToString();
                            }
                        }
                    }
                }
            }
            return ip;
        }

        private bool m_IsWaitStart = true;
        private bool m_IsLogged = false;
        private long m_LastConnectTime = 0;
        private long m_LastHeartbeatTime = 0;
        private long m_LastReceiveHeartbeatTime = 0;

        private LoginMode m_LoginMode = LoginMode.DirectLogin;
        private string m_Url;
        private int m_LogicServerId = 1;
        private string m_Account;
        private ulong m_Guid;
        private string m_UniqueIdentifier;

        private int m_OpCode;
        private int m_ChannelId;
        private string m_Data;

        private WebSocket4Net.WebSocket m_WebSocket;
        private IActionQueue m_AsyncActionQueue;

        public static LobbyNetworkSystem Instance
        {
            get
            {
                return s_Instance;
            }
        }
        private static LobbyNetworkSystem s_Instance = new LobbyNetworkSystem();
    }
}
