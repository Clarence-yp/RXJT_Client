//
//  Cylib.h
//  Cylib
//
//  Created by zhangshuhao@cyou-inc.com on 13-11-6.
//  Copyright (c) 2013年 zhangshuhao@cyou-inc.com. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface Cylib : NSObject

/**
 *	@method	onStart     初始化统计实例     请在application:didFinishLaunchingWithOptions:方法里调用
 *	@param 	appId       appId           类型:NSString     应用的唯一标识  请传billingkey
 *  @param 	channelId   渠道名(可选)      类型:NSString     如“app store”
 */
+(void)onStart:(NSString *)appId withChannelID:(NSString *)channelId;

/**
 *  @brief 获取设备唯一标识
 *  @return 设备唯一标识 类型:NSString
 */
+(NSString *)CYDeviceID;

/**
 *	@method	onEvent       自定义事件
 *	@param 	event         事件ID        类型:NSString
 */
+(void)onEventActionID:(NSString *)eventID;


//获取信息接口


/**
 *	@method	setAccountId  设置帐号
 *	@param 	accountId     帐号id    类型:NSString
 */
+(void)setAccountId:(NSString *)accountId;


/**
 *	@method	setServer  设置分区
 *	@param 	server     分区    类型:NSString
 */
+(void)setServer:(NSString *)server;


/**
 *	@method	setRoleId  设置角色Id
 *	@param 	RoleId     角色Id    类型:NSString
 */
+(void)setRoleId:(NSString *)roleId;


/**
 *	@method	setRoleName  设置角色名称
 *	@param 	RoleName     角色名称    类型:NSString
 */
+(void)setRoleName:(NSString *)roleName;


/**
 *	@method	setLevel    设置帐号等级
 *	@param 	level       升级之后的等级     类型:int
 */
+(void)setLevel:(int)level;

/**
 *	@method	setUserID   设置用户ID
 *	@param 	userID      登录验证成功后的用户ID     类型:NSString
 */
+(void)setUserID:(NSString *)userID;


//功能接口

/**
 *  @brief 用户点击登陆提交按钮时上传日志
 */
+(void)loginClick;

/**
 *  @brief 用户登陆成功并进入游戏时上传日志
 */
+(void)onLogin;

/**
 *  @brief 点击注册按钮时上传日志
 */
+(void)registerClick;

/**
 *  @brief 注册成功(绑定成功)时上传日志
 */
+(void)onRegister;

/**
 *  @brief 点击充值按钮时上传日志
 */
+(void)chargeClick;



#pragma mark - deprecated method

/**
 *	@method	onUpgrade    帐号升级等级       该方法已废弃
 *	@param 	level        升级之后的等级     类型:int
 */
+(void)onUpgrade:(int)level __attribute__((deprecated));


/**
 *	@method	onCharge      充值方法         该方法已废弃
 *	@param 	amount        充值金额         类型:double            若为空请传0；
 *	@param 	productId     购买商品Id       类型:NSString
 *	@param 	productName   购买商品名称      类型:NSString
 *	@param 	payId         支付方式Id       类型:NSString
 *	@param 	payName       支付方式名称      类型:NSString
 */
+(void)onCharge:(double)amount withProductId:(NSString *)productId withProductName:(NSString *)productName withPayId:(NSString *)payId withPayName:(NSString *)payName __attribute__((deprecated));


/**
 *	@method	onItemBuy        购买道具方法         该方法已废弃
 *	@param 	commodityId      道具Id             类型:NSString
 *	@param 	commodityName    道具名称            类型:NSString
 *	@param 	commodityCount   购买道具数量         类型:int          若为空请传0；
 *	@param 	goldId           消耗货币Id          类型:NSString
 *	@param 	goldName         消耗货币名称         类型:NSString
 *	@param 	goldCount        消耗货币数量         类型:int          若为空请传0；
 */
+(void)onItemBuy:(NSString *)commodityId withCommodityName:(NSString *)commodityName withCommodityCount:(int)commodityCount withGoldId:(NSString *)goldId withGoldName:(NSString *)goldName withGoldCount:(int)goldCount __attribute__((deprecated));



/**
 *	@method	onItemUse        道具消耗方法         该方法已废弃
 *	@param 	commodityId      道具Id             类型:NSString
 *	@param 	commodityName    道具名称            类型:NSString
 *	@param 	commodityCount   消耗道具数量         类型:int          若为空请传0；
 */
+(void)onItemUse:(NSString *)commodityId withCommodityName:(NSString *)commodityName withCommodityCount:(int)commodityCount __attribute__((deprecated));



@end
