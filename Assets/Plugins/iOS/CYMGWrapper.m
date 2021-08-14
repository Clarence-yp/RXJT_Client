//
//  CYMGWrapper.m
//
//  Created by Sirius on 14-7-3.
//  Copyright (c) 2014年 cyou-inc. All rights reserved.
//

#import <UIKit/UIKit.h>
#include <sys/socket.h>
#include <sys/sysctl.h>
#include <net/if.h>
#include <net/if_dl.h>
#import <AdSupport/ASIdentifierManager.h>

#import "CYMGWrapper.h"
#import "Cylib.h"
#import "JSONKit.h"

@interface CYMGWrapper()<CYMGGameInfoDataSource, CYMGUserAccountDelegate, CYMGPaymentDelegate,CYMGReqPaymentGoodsListDelegate>

@end

@implementation CYMGWrapper

static CYMGWrapper* cygmInstance = nil;

+ (CYMGWrapper*) Instance
{
    if (cygmInstance == nil) {
        cygmInstance = [[CYMGWrapper alloc] init];
    }
    return cygmInstance;
}
- (id)init
{
    self = [super init];
    if (self) {
    }
    return self;
}

//=============================================
// C Implementation
//=============================================
void _InitCYMG(int isDebug)
{
	[CYMGWrapper Instance];
	if (isDebug == 1) {
		[cygmInstance CYMGInit:YES];
	} else {
		[cygmInstance CYMGInit:NO];
	}
}
void _StartLogin(int isAuto)
{
	if (isAuto == 0) {
		[cygmInstance CYMGStartLogin:NO];
	} else {
		[cygmInstance CYMGStartLogin:YES];
	}    
}
void _OnLoginBillingSuccess(const char* accountId)
{
	[cygmInstance CYMGOnLoginBillingSuccess:CreateNSString(accountId)];
}
void _StartLogout()
{
    [cygmInstance CYMGStartLogout];
}
void _ShowCallCenter()
{
    [cygmInstance CYMGShowCallCenter];
}
/*
void _RequestGoodslist()
{
    [cygmInstance CYMGRequestGoodsList];
}
void _StartPayment(char* roleId, char* groupId)
{
    [cygmInstance CYMGStartPayment: [NSString stringWithUTF8String: roleID] : [NSString stringWithUTF8String: groupID]];
}
*/
//MBI游戏统计
void _MBIOnLogin(const char* account, const char* server, const char* roleName, const char* roleID, int level, const char* userId)
{
    [cygmInstance CYMGMBIOnLogin:CreateNSString(account) second:CreateNSString(server) third:CreateNSString(roleName) fourth:CreateNSString(roleID) fifth:level sixth:CreateNSString(userId)];
}
//其它
void _GetUniqueIdentifier()
{
   	[cygmInstance GetUniqueIdentifier];    
}
//Converts C Style string to NSString
NSString* CreateNSString(const char* string)
{
    if (string) {
        return [NSString stringWithUTF8String: string];
    } else {
        return [NSString stringWithUTF8String: ""];
    }
}
//===================================================================
- (NSString *) appKey
{
    return @"1407921103977";
}
- (NSString *) appSecrect
{
    return @"f550062065eb4089bd9c727e00391e8c";
}
- (NSString *) channelID
{
    return @"4001";
}
- (NSString*) thirdPartyChannel_AppID
{
	return @"112949";
}
- (NSString*) thirdPartyChannel_AppKey
{
	return @"d7e3fa8a455a3914a42414353dcdad9b629eff4da48f7e4f";
}
- (NSString *) userID
{
    return [[NSUserDefaults standardUserDefaults] objectForKey:@"userid"];
}
- (NSString *) orderPushInfo
{
    return @"order:测试orderPushInfo";
}
- (NSString *) appKeyForSinaWeibo
{
    return @"1583240852";
}
- (NSString *) redirectURIForSinaWeibo
{
    return @"http://www.sina.com";
}
- (NSString *) appIDForQQ
{
    return @"101005256";
}
- (NSString*) thirdPartyChannel_loginID
{
	return @"1cc2d8578e1d8b371da1f5190a11ad6b2";
}
//==================================================================
//封装CYMGSDK方法
- (void) CYMGInit:(BOOL)isDebug
{
    //MBI初始化
    //[Cylib onStart:@"1407921103977" withChannelID:@"1010802002"];
    //开启debug模式
    [CYMGSDK setDebugMode:isDebug];
    // 设置游戏信息数据源
    [CYMGSDK setGameInfoDataSource:cygmInstance];
}
- (void) CYMGStartLogin:(BOOL)isAuto
{
    // 登录，不带回调
    //[CYMGSDK startLogin:isAuto];
    // 登录，带回调
    [CYMGSDK startLogin:isAuto withCallBackDelegate:cygmInstance];
}
- (void) CYMGOnLoginBillingSuccess:(NSString*)accountId
{
    [[NSUserDefaults standardUserDefaults] setObject:accountId forKey:@"userid"];
	[[NSUserDefaults standardUserDefaults] synchronize];
	//[Cylib setUserID:accountId];
    //[Cylib onLogin];
}
- (void) CYMGStartLogout
{
    [CYMGSDK startLogoutWithDelegate:cygmInstance];
}
-(void) CYMGShowCallCenter
{
    [CYMGSDK showCallCenter];
}
/*
- (void) CYMGRequestGoodsList
{
    [CYMGSDK  reqPaymentGoodsListWithDelegate:cygmInstance];
}
- (void) CYMGStartPayment:(NSString *)roldId second:(NSString *)groupId
{
    [CYMGSDK startPaymentWithDelegate:cygmInstance andRoleID:roldID andGroupID:groupID];
}
*/
- (void) CYMGMBIOnLogin:(NSString *)account second:(NSString *)server third:(NSString *)roleName fourth:(NSString *)roleId fifth:(int)level sixth:(NSString *)userId
{
    [Cylib setAccountId:account];
    [Cylib setServer:server];
    [Cylib setRoleName:roleName];
    [Cylib setRoleId:roleId];
    [Cylib setLevel:level];
    [Cylib setUserID:userId];
    [Cylib onLogin];
}

- (void) GetUniqueIdentifier
{
	NSString* idStr = [cygmInstance UniqueIdentifier];
    NSLog(@"%@", idStr);
    UnitySendMessage("CYMGConnector", "OnGetUniqueIdentifier", [idStr UTF8String]);
}

- (NSString *) UniqueIdentifier
{
	NSString *uniqueIdentifier = @"Mac Address";
	int                 mib[6];
  size_t              len;
  char                *buf;
  unsigned char       *ptr;
  struct if_msghdr    *ifm;
  struct sockaddr_dl  *sdl;
  mib[0] = CTL_NET;
  mib[1] = AF_ROUTE;
  mib[2] = 0;
  mib[3] = AF_LINK;
  mib[4] = NET_RT_IFLIST;
  if ((mib[5] = if_nametoindex("en0")) == 0) {
    uniqueIdentifier = NULL;
  }
  if (sysctl(mib, 6, NULL, &len, NULL, 0) < 0) {
    uniqueIdentifier = NULL;
  }
  if ((buf = malloc(len)) == NULL) {
    uniqueIdentifier = NULL;
  }
  if (sysctl(mib, 6, buf, &len, NULL, 0) < 0) {
    free(buf);
    uniqueIdentifier = NULL;
  }
  if (uniqueIdentifier != NULL) {
  	ifm = (struct if_msghdr *)buf;
	  sdl = (struct sockaddr_dl *)(ifm + 1);
	  ptr = (unsigned char *)LLADDR(sdl);
	  uniqueIdentifier = [NSString stringWithFormat:@"%02X:%02X:%02X:%02X:%02X:%02X", *ptr, *(ptr+1), *(ptr+2), *(ptr+3), *(ptr+4), *(ptr+5)];
	  free(buf);
  }
	float iosVersion = [[[UIDevice currentDevice] systemVersion] floatValue];
	if (iosVersion < 7) {
		if (uniqueIdentifier == NULL) {
			uniqueIdentifier = [[[ASIdentifierManager sharedManager] advertisingIdentifier] UUIDString];
		}
	} else {
		uniqueIdentifier = [[[ASIdentifierManager sharedManager] advertisingIdentifier] UUIDString];
	}
	return uniqueIdentifier;
}
//=============================================================
//CYMGSDK回调方法
//账号登录平台成功回调方法
-(void) userLoginSuccessWithVerifyData:(NSDictionary *)verifyData
{
    //Send Message to Unity
    NSString* jsonStr = [verifyData JSONString];
    NSLog(@"%@", jsonStr);
    UnitySendMessage("CYMGConnector", "OnAccountLoginCYMGSuccess", [jsonStr UTF8String]);
}
//账号注销回调方法
-(void) userLogout
{
    //账号注销后续处理
}
//申请商品列表成功回调方法
-(void) reqPaymentGoodsListSuccessedGoodsInfo:(NSArray *)goodsInfo
{
    //Send Message to Unity
    NSString* jsonStr = [goodsInfo JSONString];
    NSLog(@"%@", jsonStr);
    UnitySendMessage("CYMGConnector", "OnRequestGoodsList", [jsonStr UTF8String]);
}
//申请商品列表失败回调方法
-(void) reqPaymentGoodsListFailedGoodsInfo:(NSString *)errorInfo
{
    //Send Message to Unity
    UnitySendMessage("CYMGConnector", "OnRequestGoodsList", [errorInfo UTF8String]);
}
//支付成功的回调方法
- (void) paymentSuccessedOrderInfo:(NSDictionary *)orderInfo
{
    //NSLog(@"payment Info:%@", orderInfo);
    //Send Message to Unity
    //UnitySendMessage("CYMGConnector", "OnPaymentResult", [orderInfo UTF8String]);
}
//支付失败的回调方法
- (void) paymentFailedOrderInfo:(NSDictionary *)orderInfo
{
    //NSLog(@"payment Info:%@", orderInfo);
    //Send Message to Unity
    //UnitySendMessage("CYMGConnector", "OnPaymentResult", [orderInfo UTF8String]);
}

@end




