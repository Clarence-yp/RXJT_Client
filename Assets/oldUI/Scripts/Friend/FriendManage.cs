using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DashFire;
using DashFire.Network;

public class FriendManage : MonoBehaviour
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
  void Start()
  {
    if (eventlist != null) { eventlist.Clear(); }
    object eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<Dictionary<ulong, FriendInfo>>("ge_client_friends", "friend", ClientFriends);
    if (eo != null) { eventlist.Add(eo); }
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<ulong, string, AddFriendResult>("ge_add_friend", "friend", AddFriend);
    if (eo != null) { eventlist.Add(eo); }
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<DelFriendResult>("ge_del_friend", "friend", DelFriend);
    if (eo != null) { eventlist.Add(eo); }
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_sync_friend_list", "friend", SyncFriendList);
    if (eo != null) { eventlist.Add(eo); }
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<List<FriendInfo>>("ge_query_friend_result", "friend", QueryFriendResult);
    if (eo != null) { eventlist.Add(eo); }

    DashFire.GfxSystem.EventChannelForLogic.Publish("ge_request_client_friends", "ui");
  }
  void Update()
  {
  }
  private void ClientFriends(Dictionary<ulong, FriendInfo> dic)
  {
    try {
      int ct = null == dic ? 0 : dic.Count;
      Debug.Log("friend num : " + ct);
      if (ct > 0) {
        foreach (FriendInfo info in dic.Values) {
          Debug.Log("friend node -> guid : " + info.Guid + " , nick : " + info.Nickname + " , level : " + info.Level);
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private void AddFriend(ulong guid, string nick, AddFriendResult result)
  {
    try {
      if (AddFriendResult.ADD_SUCCESS == result) {
        Debug.Log("addfriend success! -> guid : " + guid + " , nick : " + nick);
      } else if (AddFriendResult.ADD_NOTICE == result) {
        RoleInfo role_info = LobbyClient.Instance.CurrentRole;
        if (null != role_info && null != role_info.Friends) {
          if (!role_info.Friends.ContainsKey(guid)) {
            /// Whether to add each other.
            /// if yes
            DashFire.LogicSystem.EventChannelForGfx.Publish("ge_confirm_friend", "lobby", guid);
            Debug.Log("confirmfriend nick : " + nick);
          }
        }
      } else {
        Debug.Log("addfriend error! -> " + result.ToString());
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private void DelFriend(DelFriendResult result)
  {
    try {
      if (DelFriendResult.DEL_SUCCESS == result) {
      } else {
      }
      Debug.Log("delfriend! -> " + result.ToString());
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private void SyncFriendList()
  {
    try {
      RoleInfo role_info = LobbyClient.Instance.CurrentRole;
      if(null != role_info && null != role_info.Friends && role_info.Friends.Count > 0) {
        int ct = role_info.Friends.Count;
        Debug.Log("friend num : " + ct);
        foreach (FriendInfo info in role_info.Friends.Values) {
          Debug.Log("friend node -> guid : " + info.Guid + " , nick : " + info.Nickname + " , level : " + info.Level);
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private void QueryFriendResult(List<FriendInfo> list)
  {
    try {
      if (null != list && list.Count > 0) {
        int ct = list.Count;
        Debug.Log("record num : " + ct);
        foreach (FriendInfo info in list) {
          Debug.Log("record -> guid : " + info.Guid + " , nick : " + info.Nickname + " , level : " + info.Level);
        }
      } else {
        Debug.Log("queryfriendresult! - > no record");
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
}
