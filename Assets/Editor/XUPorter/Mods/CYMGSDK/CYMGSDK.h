//
//  CYMGSDK.h
//  CyouMobileGameSDK
//
//  Created by Jack Cheng on 14-2-18.
//  Copyright (c) 2014年 cyou-inc. All rights reserved.
//

#import <Foundation/Foundation.h>

@protocol CYMGUserAccountDelegate;
@protocol CYMGGameInfoDataSource;
@protocol CYMGPaymentDelegate;
@protocol CYMGReqPaymentGoodsListDelegate;
typedef enum {
    LoginType_Changyou = 0,
    LoginType_QQ,
    LoginType_SinaWeibo,
    LoginType_91,
    LoginType_PP,
    LoginType_TB,
    LoginType_KY,
}LoginType;

/*
 * @brief 该类为下单支付传入商品信息（包括 游戏角色ID、角色所属分区、角色名称）
 */
@interface CYMGPayGoodInfo : NSObject
@property (nonatomic, strong)NSString *goods_describe;      //商品描述
@property (nonatomic, strong)NSString *goods_icon;          //商品图标
@property (nonatomic, strong)NSString *goods_id;            //商品id
@property (nonatomic, strong)NSString *goods_name;          //商品名称
@property (nonatomic, strong)NSString *goods_price;         //商品价格
@property (nonatomic, strong)NSString *goods_number;        //商品数量
@property (nonatomic, strong)NSString *goods_register_id;   //商品注册id
@property (nonatomic, strong)NSNumber *type;                // Int	商品类型:0普通、1首冲、2活动、3随心购
@property (nonatomic, strong)NSString *group_id;            // 服务器唯一标识,分区ID
@property (nonatomic, strong)NSString *role_id;             // 游戏角色ID
@property (nonatomic, strong)NSString *role_name;           // 游戏角色名称
@property (nonatomic, strong)NSString *order_id;            //订单号
@end

@interface CYMGSDK : NSObject

/*
 *	@brief	设置是否开启调试模式
 *  @note   发布时请务必改为NO
 *	@param 	nFlag  YES为开启，NO为关闭
 *  @return    无返回
 */
+ (void)setDebugMode:(BOOL)nFlag;

/**
 *  显示悬浮工具条
 */
+ (void)showToolBar;

/**
 *  隐藏悬浮工具条
 */
+ (void)hideToolBar;

/**
 *	@brief	进入用户中心
 */
+ (void)enterUserCenter;

/*
 *  客服页面
 */
+ (void)showCallCenter;

/*
 *  充值记录页面
 */
+ (void)showRechargeRecord;

/*
 *  SDK版本号
 */
+ (NSString *)SDKVersion;

/*
 *  登录平台，进入登录或者注册界面入口，默认自动登录
 */
+(void)startLogin;

/*
 *  登录平台，进入登录或者注册界面入口
 *  @param autoLogin 是否自动登录
 */
+(void)startLogin:(BOOL)autoLogin;

/*
 *  登录平台，进入登录或者注册界面入口
 *  @param  autoLogin   是否自动登录
 *  @param  delegate    登录回调代理
 */
+(void)startLogin:(BOOL)autoLogin withCallBackDelegate:(id<CYMGUserAccountDelegate>) delegate;

/*
 *  注销
 */
+ (void)startLogout;

/*
 *  注销
 *  @param  delegate    注销回调代理
 */
+ (void)startLogoutWithDelegate:(id<CYMGUserAccountDelegate>) delegate;

/*
 *  设置SDK用户登录回调代理
 *  @param  delegate    登录相关操作回调对象的指针
 */
+ (void)setUserAccountCallBackDelegate:(id<CYMGUserAccountDelegate>) delegate;

/*
 *  开始下单支付流程
 *  @param  delegate    支付回调代理
 */
+ (void)startPaymentWithDelegate:(id<CYMGPaymentDelegate>) delegate andGoodInfo:(CYMGPayGoodInfo *)gooInfo;

/*
 *  开始支付流程，进入充值界面入口
 *  @param  roleID      当角色和元宝由畅游BILLING来管理时,此参数不能为空,否则玩家无法充值
 *  @param  groupID     请将此值设置为分区标识,无分区不需要设置
 */
+ (void)startPaymentWithRoleID:(NSString *)roleID andGroupID:(NSString *)groupID;

/*
 *  开始支付流程，进入充值界面入口
 *  @param  delegate    支付回调代理
 *  @param  roleID      当角色和元宝由畅游BILLING来管理时,此参数不能为空,否则玩家无法充值
 *  @param  groupID     请将此值设置为分区标识,无分区不需要设置
 */
+ (void)startPaymentWithDelegate:(id<CYMGPaymentDelegate>) delegate andRoleID:(NSString *)roleID andGroupID:(NSString *)groupID;

/*
 *  设置SDK支付回调代理
 */
+ (void)setPaymentCallBackDelegate:(id<CYMGPaymentDelegate>) delegate;



/*
 *  开始请求商品列表数据，
 *  @param  delegate    商品列表数据回调代理
 *  @param  roleID      当角色和元宝由畅游BILLING来管理时,此参数不能为空,否则玩家无法充值
 *  @param  groupID     请将此值设置为分区标识,无分区不需要设置
 */
+ (void)reqPaymentGoodsListWithDelegate:(id<CYMGReqPaymentGoodsListDelegate>) delegate ;

/*
 *  设置SDK请求商品列表回调代理
 */
+ (void)reqPaymentGoodsListDelegate:(id<CYMGReqPaymentGoodsListDelegate>)delegate;
/*
 *  目前用于第三方登录方式回调URL处理，支持QQ和Sina微博登录的回调处理。如不接入QQ和Sina微博登录功能，可不调用。pp和同步推渠道必须实现该方法
 *  需要在AppDelegate中的两个函数中调用。
 *  eg:
         -(BOOL)application:(UIApplication *)application openURL:(NSURL *)url sourceApplication:(NSString *)sourceApplication annotation:(id)annotation
         {
         return [CYMGSDK handleURL:url];
         
         }
         
         -(BOOL)application:(UIApplication *)application handleOpenURL:(NSURL *)url{
         
         return [CYMGSDK handleURL:url];
         }
 */
+ (BOOL)handleURL:(NSURL *)url;

/*
 *  设置SDK获取游戏信息的数据源
 *  @param  datasource  数据源对象的指针
 */
+ (void)setGameInfoDataSource:(id<CYMGGameInfoDataSource>) datasource;

@end

@protocol CYMGUserAccountDelegate <NSObject>

@optional
- (void)userLoginSuccessWithVerifyData:(NSDictionary *)verifyData;

@optional
- (void)userLogout;

@end

@protocol CYMGGameInfoDataSource <NSObject>


/*
 *  游戏角色ID。此ID为billing登录成功返回的userID
 */
- (NSString *)userID;

/*
 *  billing生成的AppKey
 */
- (NSString *)appKey;

/*
 *  billing生成的AppSecrect
 */
- (NSString *)appSecrect;

/*
 *  对应渠道号
 *  eg:
        苹果官方渠道channelID = @"2001"
 */
- (NSString *)channelID;

/*
 *  订单的扩展信息，可记录游戏区服，角色信息之类，会在充值成功后发给游戏Server
 */
- (NSString *)orderPushInfo;

/*
 *  第三方渠道平台生成的AppID
 */
@optional
- (NSString *)thirdPartyChannel_AppID;

/*
 *  第三方渠道生成的AppKey
 */
@optional
- (NSString *)thirdPartyChannel_AppKey;

/*
 *  新浪微博AppKey
 */
@optional
- (NSString *)appKeyForSinaWeibo;

/*
 *  新浪微博重定向地址
 */
@optional
- (NSString *)redirectURIForSinaWeibo;

/*
 *  QQ平台的AppID
 */
@optional
- (NSString *)appIDForQQ;

/*
 *  第三方渠道平台生成的登录验证ID，目前只有快用登录需要此ID，传入在快用平台申请的APPKey
 */
@optional
- (NSString *)thirdPartyChannel_loginID;

@end

@protocol CYMGPaymentDelegate <NSObject>

@optional
- (void)paymentSuccessedOrderInfo:(NSDictionary *)orderInfo;

@optional
- (void)paymentFailedOrderInfo:(NSDictionary *)orderInfo;

@end

@protocol CYMGReqPaymentGoodsListDelegate <NSObject>

@optional
- (void)reqPaymentGoodsListSuccessedGoodsInfo:(NSArray *)goodsInfo;

@optional
- (void)reqPaymentGoodsListFailedGoodsInfo:(NSString *)errorInfo;

@end