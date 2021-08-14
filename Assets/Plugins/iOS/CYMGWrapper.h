//
//  CYMGWrapper.h
//
//  Created by Sirius on 14-7-3.
//  Copyright (c) 2014ๅนด cyou-inc. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "CYMGSDK.h"

@interface CYMGWrapper : NSObject

// **********************************
// C wrapper API
// **********************************
//初始化CYMG SDK
void _InitCYMG(int isDebug);
//账号登录
void _StartLogin(int isAuto);
//账号服务器验证成功
void _OnLoginBillingSuccess(const char* accountId);
//账号注销
void _StartLogout();
//显示客服中心
void _ShowCallCenter();
//申请商品列表
//void _RequestGoodsList();
//充值
//void _StartPayment(const char* roleId, const char* groupId);

//MBI游戏数据统计
//玩家进入游戏日志
void _MBIOnLogin(const char* account, const char* server, const char* roleName, const char* roleID, int level, const char* userId);

//其它
//获取设备唯一标识符
void _GetUniqueIdentifier();

// **********************************
// Objective-C wrapper API
// **********************************
// CYMGWrapper Instance
+ (CYMGWrapper*) Instance;
// Initialize CYMG
- (void) CYMGInit:(BOOL)isDebug;
// Start login
- (void) CYMGStartLogin:(BOOL)isAuto;
// On login billing server success
- (void) CYMGOnLoginBillingSuccess:(NSString*)accountId;
// Start logout
- (void) CYMGStartLogout;
// Show call center
- (void) CYMGShowCallCenter;
// Request good list
//- (void) CYMGRequestGoodsList;
// Start payment
//- (void) CYMGStartPayment:(NSString *)roldId second:(NSString *)groupId;
// MBI role enter log
- (void) CYMGMBIOnLogin:(NSString *)account second:(NSString *)server third:(NSString *)roleName fourth:(NSString *)roleId fifth:(int)level sixth:(NSString *)userId;
// Get Device UniqueIdentifier
- (void) GetUniqueIdentifier;

@end
