//todo:node <-> lobbySrv
var MsgDefine = {
  Zero: 0,
  Logout: 1,
  RequestMatch: 2,
  CancelMatch: 3,
  MatchResult: 4,
  StartGame: 5,
  StartGameResult: 6,
  NodeJsRegister: 7,
  NodeJsRegisterResult: 8,
  QuitRoom: 9,
  UserHeartbeat: 10,
  SyncPrepared: 11,
  SyncQuitRoom: 12,
  AddFriend: 13,
  AddFriendResult: 14,
  ConfirmFriend: 15,
  DelFriend: 16,
  DelFriendResult: 17,
  FriendList: 18,
  SyncFriendList: 19,
  SyncCancelFindTeam: 20,
  AddBlack: 21,
  AddBlackResult: 22,
  DelBlack: 23,
  RefuseFriend: 24,
  SyncTeamingState: 25,
  SinglePVE: 26,
  SyncMpveBattleResult: 27,
  SyncGowBattleResult: 28,
  DiscardItem: 29,
  DiscardItemResult: 30,
  MountEquipment: 31,
  MountEquipmentResult: 32,
  UnmountEquipment: 33,
  UnmountEquipmentResult: 34,
  AccountLogin: 35,
  AccountLoginResult: 36,
  AccountLogout: 37,
  RoleList: 38,
  RoleListResult: 39,
  CreateNickname: 40,
  CreateNicknameResult: 41,
  CreateRole: 42,
  CreateRoleResult: 43,
  RoleEnter: 44,
  RoleEnterResult: 45,
  MountSkill: 46,
  MountSkillResult: 47,
  UnmountSkill: 48,
  UnmountSkillResult: 49,
  UpgradeSkill: 50,
  UpgradeSkillResult: 51,
  UnlockSkill: 52,
  UnlockSkillResult: 53,
  SwapSkill: 54,
  SwapSkillResult: 55,
  UpgradeItem: 56,
  UpgradeItemResult: 57,
  UserLevelup: 58,
  SaveSkillPreset: 59,
  ActivateAccount: 60,
  ActivateAccountResult: 61,
  SyncStamina: 62,
  StageClear: 63,
  StageClearResult: 64,
  AddAssets: 65,
  AddAssetsResult: 66,
  AddItem: 67,
  AddItemResult: 68,
  LiftSkill: 69,
  LiftSkillResult: 70,
  BuyStamina: 71,
  BuyStaminaResult: 72,
  FinishMission: 73,
  FinishMissionResult: 74,
  BuyLife: 75,
  BuyLifeResult: 76,
  UnlockLegacy: 77,
  UnlockLegacyResult: 78,
  UpgradeLegacy: 79,
  UpgradeLegacyResult: 80,
  UpdateFightingScore: 81,
  NotifyNewMail: 82,
  GetMailList: 83,
  ReceiveMail: 84,
  SyncMailList: 85,
  ExpeditionReset: 86,
  ExpeditionResetResult: 87,
  VersionVerify: 88,
  VersionVerifyResult: 89,
  RequestExpedition: 90,
  RequestExpeditionResult: 91,
  FinishExpedition: 92,
  FinishExpeditionResult: 93,
  ExpeditionAward: 94,
  ExpeditionAwardResult: 95,
  GetGowStarList: 96,
  SyncGowStarList: 97,
  DirectLogin: 98,
  QueryExpeditionInfo: 99,
  ReadMail: 100,
  SendMail: 101,
  ExpeditionFailure: 102,
  MidasTouch: 103,
  MidasTouchResult: 104,
  ResetDailyMissions : 105,
  GMResetDailyMissions : 106,
  QueryFriendInfo : 107,
  QueryFriendInfoResult : 108,
  MissionCompleted : 109,
  PublishNotice : 110,
  SyncNoticeContent : 111,
  KickUser: 112,
  MaxNum : 113,
};

MsgDefine.Responses = {
  LOGIN_SUCCESS: 0,
  LOGIN_FAIL: 1,
  LOGIN_USER_ERROR: 2,
  LOGIN_PWD_ERROR: 3,
  ERROR: 4
};

var hash = require("./hash"),
	config = require("./config"),
	PORT = 9001,
	WebSocketServer = require('ws').Server;


//todo:socket manage
var AccountList = new hash.HashTable(); //todo:Account
var GuidList = new hash.HashTable(); //todo:guid
var Socket2Account = new hash.HashTable();
var Socket2Guid = new hash.HashTable();

function JsonParse(jsonStr) {
  try {
    var jsonObj = JSON.parse(jsonStr);
    return jsonObj;
  } catch (ex) {
    console.log("JsonParse exception:"+ex);
  }
  return null;
}


//todo:node logic
var serverLogic = {};
serverLogic.init = function () {

  io = new WebSocketServer({ port: PORT });
  console.log("Server runing at port: " + PORT + ".");

  io.on('connection', function (socket) {
    socket.on('message', function (arg) {
      try {
        var ix = arg.indexOf('|');
        if (ix >= 0) {
          var ix2 = arg.indexOf('|', ix + 1);
          var msgId = parseInt(arg.substr(0, ix));
          var msgBody;
          if (ix2 >= 0)
            msgBody = arg.substr(ix + 1, ix2 - ix - 1);
          else
            msgBody = arg.substr(ix + 1);
  
          switch (msgId) {
            case MsgDefine.VersionVerify: 
              {
                console.log("version verify:" + arg);
                var jsonObj = JsonParse(msgBody);
                if(null!=jsonObj) {
                  var version = jsonObj["m_Version"];
                  if ((config.version & 0xffffff00) == (version & 0xffffff00)) {
                    socket.send(MsgDefine.VersionVerifyResult + "|{\"m_Result\":1}");
                  } else {
                    socket.send(MsgDefine.VersionVerifyResult + "|{\"m_Result\":0}");
                  }
                }
              }
              break;
            case MsgDefine.AccountLogin: 
              {
                core_send_message_by_name("Lobby", arg);
                console.log("!!post to lobbySrv msg: " + arg);
                var jsonObj = JsonParse(msgBody);
                if(null!=jsonObj) {
                  AccountList.add(jsonObj.m_Account, socket);
                }
              }
              break;
            case MsgDefine.DirectLogin: 
              {
                core_send_message_by_name("Lobby", arg);
                console.log("!!post to lobbySrv msg: " + arg);
                var jsonObj = JsonParse(msgBody);
                if(null!=jsonObj) {
                  AccountList.add(jsonObj.m_Account, socket);
                }
              }
              break;
            case MsgDefine.RoleEnter: 
              {
                core_send_message_by_name("Lobby", arg);
                console.log("!!post to lobbySrv msg: " + arg);
              }
              break;
            case MsgDefine.AccountLogout: 
              {
                core_send_message_by_name("Lobby", arg);
                console.log("!!post to lobbySrv msg: " + arg);
                var jsonObj = JsonParse(msgBody);
                if(null!=jsonObj) {
                  AccountList.remove(jsonObj.m_Account);
                }
              }
              break;
            case MsgDefine.Logout: 
              {
                core_send_message_by_name("Lobby", arg);
                console.log("!!post to lobbySrv msg: " + arg);
                var jsonObj = JsonParse(msgBody);
                if(null!=jsonObj) {
                  GuidList.remove(jsonObj.m_Guid);
                }
              }
              break;
            case MsgDefine.UserHeartbeat: 
              {
                core_send_message_by_name("Lobby", arg);
                //console.log("!!post to lobbySrv msg: " + arg);
              }
              break;
            default:
              var jsonObj = JsonParse(msgBody);
              if(null!=jsonObj) {
                var socketKey = socket.upgradeReq.headers['sec-websocket-key'];
                if (Socket2Guid.getValue(socketKey) === null && Socket2Account.getValue(socketKey) === null) {
                  console.log("msg " + arg + " from unknown socket " + socketKey);              
                } else {
                  var guid = Socket2Guid.getValue(socketKey);
                  if (jsonObj["m_Guid"] && jsonObj["m_Guid"] != guid) {
                    console.log("msg " + arg + " from socket " + socketKey + " (guid " + guid + "), guid is different !");
                  }
                }
              } else {
                break;
              }
              core_send_message_by_name("Lobby", arg);
              //console.log("!!post to lobbySrv msg: " + arg);
              break;
          }
        }
      } catch(ex) {
        console.log("onmessage exception:"+ex);
      }
    });
    socket.on('close', function () {
      try {
        var socketKey = socket.upgradeReq.headers['sec-websocket-key'];
        if(socketKey){
  	      var account = Socket2Account.getValue(socketKey);
  	      var guid = Socket2Guid.getValue(socketKey);
  	      if(guid) {	      	
  		      var logoutMsg = {m_Guid:guid};
  		      core_send_message_by_name("Lobby", MsgDefine.Logout+"|"+JSON.stringify(logoutMsg));    
  	      	GuidList.remove(guid);
  	      }
  	      if(account) {
  		      var accountLogoutMsg={m_Account:account};
  		      core_send_message_by_name("Lobby", MsgDefine.AccountLogout+"|"+JSON.stringify(accountLogoutMsg));
  	      	AccountList.remove(account);
  	      }
  	      
  	      console.log("user disconnect, account:"+account+" guid:"+guid);	 
  	      
  	      Socket2Account.remove(socketKey);
  	      Socket2Guid.remove(socketKey);       
  	    }
      } catch(ex) {
        console.log("onclose exception:"+ex);
      }
    });
    socket.on('error', function () {
      try {
        var socketKey = socket.upgradeReq.headers['sec-websocket-key'];
        if(socketKey){
  	      var account = Socket2Account.getValue(socketKey);
  	      var guid = Socket2Guid.getValue(socketKey);
  	      if(guid) {	      	
  		      var logoutMsg = {m_Guid:guid};
  		      core_send_message_by_name("Lobby", MsgDefine.Logout+"|"+JSON.stringify(logoutMsg));    
  	      	GuidList.remove(guid);
  	      }
  	      if(account) {
  		      var accountLogoutMsg={m_Account:account};
  		      core_send_message_by_name("Lobby", MsgDefine.AccountLogout+"|"+JSON.stringify(accountLogoutMsg));
  	      	AccountList.remove(account);
  	      }
  	      
        	console.log("user network error, account:"+account+" guid:"+guid);
        	
  	      Socket2Account.remove(socketKey);
  	      Socket2Guid.remove(socketKey);
  	    }
      } catch(ex) {
        console.log("onerror exception:"+ex);
      }
    });
  });
};

global.onCoreMessage = function (handle, session, msg) {
  try {
    //console.log("@@node.js receive core message:" + handle + "," + session + "," + msg);
  
    var ix = msg.indexOf('|');
    if (ix >= 0) {
      var ix2 = msg.indexOf('|', ix + 1);
      var msg_id = parseInt(msg.substr(0, ix));
      var msg_tmp;
      if (ix2 >= 0)
        msg_tmp = msg.substr(ix + 1, ix2 - ix - 1);
      else
        msg_tmp = msg.substr(ix + 1);
      var msg_body = JsonParse(msg_tmp);
      if(null==msg_body) {
        return;
      }
  
      if (msg_id === MsgDefine.AccountLoginResult) {
        var _account = msg_body["m_Account"];
        var _socket = AccountList.getValue(_account);
        if (_socket) {
          _socket.send(msg);
  
          var socketKey = _socket.upgradeReq.headers['sec-websocket-key'];
          console.log("Socket2Account:" + socketKey + " -> " + _account);
  
          Socket2Account.add(socketKey, _account);
        }
      } else if (msg_id === MsgDefine.ActivateAccountResult) {
        var _account = msg_body["m_Account"];
        var _socket = AccountList.getValue(_account);
        if (_socket) {
          _socket.send(msg);
        }
      } else if (msg_id === MsgDefine.RoleListResult) {
        var _account = msg_body["m_Account"];
        var _socket = AccountList.getValue(_account);
        if (_socket) {
          _socket.send(msg);
        }
      } else if (msg_id === MsgDefine.CreateNicknameResult) {
        var _account = msg_body["m_Account"];
        var _socket = AccountList.getValue(_account);
        if (_socket) {
          _socket.send(msg);
        }
      } else if (msg_id === MsgDefine.CreateRoleResult) {
        var _account = msg_body["m_Account"];
        var _socket = AccountList.getValue(_account);
        if (_socket) {
          _socket.send(msg);
        }
      } else if (msg_id === MsgDefine.RoleEnterResult) {
        var _account = msg_body["m_Account"];
        var _guid = msg_body["m_Guid"];
        var _socket = AccountList.getValue(_account);
        if (_socket) {
          _socket.send(msg);
  
          var socketKey = _socket.upgradeReq.headers['sec-websocket-key'];
          console.log("Socket2Guid:" + socketKey + " -> " + _guid);
  
          GuidList.add(_guid, _socket);
          Socket2Guid.add(socketKey, _guid);
        }
      } else if (msg_id === MsgDefine.UserHeartbeat) {
        var _guid = msg_body["m_Guid"];
        var _socket = GuidList.getValue(_guid);
        if (_socket) {
          _socket.send(msg);
        }
      } else if (msg_id === MsgDefine.KickUser) {
        var _guid = msg_body["m_Guid"];
        var _socket = GuidList.getValue(_guid);
        if (_socket) {
          _socket.close();
          
          console.log("kick user, guid:"+_guid);
          
          var socketKey = _socket.upgradeReq.headers['sec-websocket-key'];
          if(socketKey){
    	      var account = Socket2Account.getValue(socketKey);
    	      var guid = Socket2Guid.getValue(socketKey);
    	      if(guid) {	      	
    		      GuidList.remove(guid);
    	      }
    	      if(account) {
    		      AccountList.remove(account);
    	      }
          	
    	      Socket2Account.remove(socketKey);
    	      Socket2Guid.remove(socketKey);
    	    }
    	  }
      } else if (msg_id === MsgDefine.NodeJsRegisterResult) {
        var _IsOk = msg_body["m_IsOk"];
        if (_IsOk === true) {
          registered = true;
          if (tick_handle !== null) {
            clearInterval(tick_handle);
            tick_handle = null;
          }
          serverLogic.init();
        }
      } else {
        var _Guid = msg_body["m_Guid"];
        var _socket = GuidList.getValue(_Guid);
        if (_socket) {
          _socket.send(msg);
        }
      }
    }
  } catch(ex) {
    console.log("oncoremessage exception:"+ex);
  }
}

global.onCoreExit = function () {
  server.close();
}

var registered = false;
var tick_handle = null;
var mainLogic = {};
mainLogic.Start = function () {
  tick_handle = setInterval(function () {
    try {
      if (registered === false) {
        var name = core_service_name();
        var cacheStr = JSON.stringify({ "m_Name": name });
        var jsonMsg = MsgDefine.NodeJsRegister + "|" + cacheStr;
        core_send_message_by_name("Lobby", jsonMsg);
        console.log("!!post to lobbySrv msg: " + jsonMsg);
      }
    } catch(ex) {
      console.log("ontick exception:"+ex);
    }
  }, 8000);
}

mainLogic.Start();
