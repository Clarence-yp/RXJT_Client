using UnityEngine;
using DashFire;
using System;
using System.Collections;
using System.Collections.Generic;
public class UIArtifactPanel : MonoBehaviour
{

  public GameObject goSlot = null;
  public UIButton leftButton = null;
  public UIButton rightButton = null;
  public UIGrid uiGrid = null;
  public UILabel lblName = null;
  public UILabel lblCostItemInfo = null;
  public UILabel lblCostItemCount = null;//当前剩余物品数
  public UILabel lblCostItemName = null;//消耗物品名
  public UILabel lblLegacyLevel = null;
  public UILabel lblLegacyAppendAttr = null;//附加属性
  public UITexture texCostItem = null;//消耗物品图
  public UIProgressBar uiProgressBar = null;
  public UISprite[] spIndexHintArr = new UISprite[c_ArtifactNum];
  public UIArtifactIntroduce artifactIntroduce = null;
  public UIArtifactTitle artifactTitle = null;
  private int m_CurrentArtifactId = -1;
  private int m_CostItemId = -1;
  private const int c_ArtifactNum = 4;
  private const string AshHint = "skilllevel";
  private const string LightHint = "skilllevel2";
  private float m_OffsetX = 350;
  // Use this for initialization
  private List<object> m_EventList = new List<object>();
  //
  public void UnSubscribe()
  {
    try {
      foreach (object obj in m_EventList) {
        if (null != obj) LogicSystem.EventChannelForGfx.Unsubscribe(obj);
      }
      m_EventList.Clear();
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  void Start()
  {
    object obj = null;
    obj = LogicSystem.EventChannelForGfx.Subscribe("ge_ui_unsubscribe", "ui", UnSubscribe);
    if (obj != null) m_EventList.Add(obj);
    obj = LogicSystem.EventChannelForGfx.Subscribe<int, int, DashFire.Network.GeneralOperationResult>("ge_upgrade_legacy", "legacy", HandleLeveUpResult);
    if (obj != null) m_EventList.Add(obj);
    UICenterOnChild centerOnChild = GetComponentInChildren<UICenterOnChild>();
    if (centerOnChild != null) {
      centerOnChild.onFinished += OnScrollViewFinished;
    }
    InitArtifact();
    if (uiGrid != null) m_OffsetX = uiGrid.cellWidth;
  }

  // Update is called once per frame
  void Update()
  {

  }
  void OnDestroy()
  {
    UICenterOnChild centerOnChild = GetComponentInChildren<UICenterOnChild>();
    if (centerOnChild != null) {
      centerOnChild.onFinished -= OnScrollViewFinished;
    }
  }
  void OnEnable()
  {
    UIArtifactSlot[] slotArr = uiGrid.transform.GetComponentsInChildren<UIArtifactSlot>();
    foreach (UIArtifactSlot slot in slotArr) {
      if (slot != null) {
        ItemDataInfo data_info = GetArtifactInfoById(slot.ArtifactId);
        if (data_info != null) slot.Unlock(data_info.IsUnlock);
      }
    }
  }
  //Init 
  public void InitArtifact()
  {
    RoleInfo role_info = LobbyClient.Instance.CurrentRole;
    if (role_info != null) {
      foreach (ItemDataInfo item_info in role_info.Legacys) {
        if (item_info != null && goSlot != null && uiGrid != null) {
          GameObject go = NGUITools.AddChild(uiGrid.gameObject, goSlot);
          //UISprite sp = go.GetComponent<UISprite>();
          int artifactIndex = GetArfifactIndex(item_info.ItemId);
          UIArtifactSlot artifactSlot = go.GetComponent<UIArtifactSlot>();
          if (artifactSlot != null) {
            artifactSlot.InitSlot(artifactIndex, item_info);
            //设置初始Id
            if (m_CurrentArtifactId == -1) {
              SetArtifactId(item_info.ItemId);
              UpdateArtifactInfo(m_CurrentArtifactId);
              if (artifactIntroduce != null) artifactIntroduce.SetIntroduce(m_CurrentArtifactId, item_info.IsUnlock);
            }
          }
        }
      }
    }
    if (uiGrid != null) uiGrid.Reposition();
  }
  public void OnScrollViewFinished()
  {
    UICenterOnChild centerOnChild = this.GetComponentInChildren<UICenterOnChild>();
    if (centerOnChild != null) {
      GameObject goArtifactSlot = centerOnChild.centeredObject;
      if (goArtifactSlot != null) {
        UIArtifactSlot artifactSlot = goArtifactSlot.GetComponent<UIArtifactSlot>();
        if (artifactSlot != null) {
          SetArtifactId(artifactSlot.ArtifactId);
          UpdateArtifactInfo(m_CurrentArtifactId);
          ItemDataInfo data_info = GetArtifactInfoById(m_CurrentArtifactId);
          if (artifactIntroduce != null && data_info != null) artifactIntroduce.SetIntroduce(m_CurrentArtifactId, data_info.IsUnlock);
        }
      }
    }
  }
  //
  public void SetArtifactId(int id)
  {
    m_CurrentArtifactId = id;
  }
  //更新UI显示的所有信息
  public void UpdateArtifactInfo(int currentArtifactId)
  {
    ItemDataInfo data_info = GetArtifactInfoById(m_CurrentArtifactId);
    if (data_info != null) {
      LegacyLevelupConfig legacyLvUpCfg = LegacyLevelupConfigProvider.Instance.GetDataById(data_info.Level);
      if (legacyLvUpCfg != null) {
        int legacyIndex = GetArfifactIndex(m_CurrentArtifactId);
        if (legacyIndex >= 0 && legacyIndex < legacyLvUpCfg.m_CostItemList.Count) {
          int costitemId = legacyLvUpCfg.m_CostItemList[legacyIndex];
          int currentNum = 0;
          ItemDataInfo costItemDataInfo = GetItemDataInfoById(costitemId);
          SetCostItem(costitemId);
          if (costItemDataInfo != null) {
            currentNum = costItemDataInfo.ItemNum;
          }
          int max = legacyLvUpCfg.m_CostNum;
          if (lblCostItemCount != null) lblCostItemCount.text = "X "+currentNum.ToString();
          if (uiProgressBar != null && max != 0) {
            uiProgressBar.value = (float)currentNum / max;
          }
          //设置等级信息及所需物品的数量
          SetIndexHint(legacyIndex);
          if (lblCostItemInfo != null) lblCostItemInfo.text = max.ToString();
          if (lblLegacyLevel != null) lblLegacyLevel.text = "Lv." + data_info.Level;
          ItemConfig itemCfg = ItemConfigProvider.Instance.GetDataById(m_CurrentArtifactId);
          if (itemCfg != null) {
            if (lblName != null) lblName.text = itemCfg.m_ItemName;
            //获取附加属性
            List<int> appendAttrList = itemCfg.m_AttachedProperty;
            if (appendAttrList == null) return;
            //默认神器的附加属性只有一个
            int appendId = -1;
            if (appendAttrList.Count > 0) appendId = appendAttrList[0];
            AppendAttributeConfig cfg = AppendAttributeConfigProvider.Instance.GetDataById(appendId);
            if (lblLegacyAppendAttr != null) {
              lblLegacyAppendAttr.text = GetAppendAttr(cfg, legacyIndex).ToString();
            }
          }
        }
      }
    }
  }
  //设置消耗物品
  public void SetCostItem(int itemId)
  {
    m_CostItemId = itemId;
    ItemConfig itemCfg = ItemConfigProvider.Instance.GetDataById(itemId);
    if (itemCfg != null) {
      if (lblCostItemName != null) lblCostItemName.text = itemCfg.m_ItemName;
      Texture tex = ResourceSystem.GetSharedResource(itemCfg.m_ItemTrueName) as Texture;
      if (texCostItem != null) {
        if (tex != null) {
          texCostItem.mainTexture = tex;
        } else {
          DashFire.ResLoadAsyncHandler.LoadAsyncItem(itemCfg.m_ItemTrueName, texCostItem);
        }
      }
    }
  }
  /// <summary>
  /// 参数index 代表第几个神器、已确定其属性，目前只能写死了！！
  /// <param name="index"> </param>

  public string GetAppendAttr(AppendAttributeConfig cfg, int index)
  {
    if (cfg == null) return "";
    float ret =
             cfg.GetAddHpMax(1.0f, 1) + cfg.GetAddEpMax(1.0f, 1) +
             cfg.GetAddAd(1.0f, 1) + cfg.GetAddADp(1.0f, 1) +
             cfg.GetAddMDp(1.0f, 1) + cfg.GetAddCri(1.0f, 1) +
             cfg.GetAddPow(1.0f, 1) + cfg.GetAddBackHitPow(1.0f, 1) +
             cfg.GetAddCrackPow(1.0f, 1) + cfg.GetAddFireDam(1.0f, 1) +
             cfg.GetAddIceDam(1.0f, 1) + cfg.GetAddPoisonDam(1.0f, 1) +
             cfg.GetAddFireDam(1.0f, 1) + cfg.GetAddIceErd(1.0f, 1) +
             cfg.GetAddPoisonErd(1.0f, 1) + cfg.GetAddEpRecover1(1.0f, 1)
             + cfg.GetAddHpRecover1(1f, 1) + cfg.GetAddAd2(1.0f, 1)
             + cfg.GetAddHpMax2(1f, 1);
    string str = "";
    switch (index) {
      case 0:
        str = StrDictionaryProvider.Instance.GetDictString(405);
        str = str + ret; break;
      case 1:
        str = StrDictionaryProvider.Instance.GetDictString(406);
        str = str + ret; break;
      case 2:
        str = StrDictionaryProvider.Instance.GetDictString(407);
        str = str + (int)(ret * 100) + "%"; break;
      case 3:
        str = StrDictionaryProvider.Instance.GetDictString(408);
        str = str + (int)(ret * 100) + "%"; break;
    }
    return str;
  }

  //设置Index提示
  public void SetIndexHint(int hintIndex)
  {
    for (int index = 0; index < spIndexHintArr.Length; ++index) {
      if (spIndexHintArr[index] != null) {
        if (index != hintIndex) {
          spIndexHintArr[index].spriteName = AshHint;
        } else {
          spIndexHintArr[index].spriteName = LightHint;
        }
      }
    }
  }
  //升级
  public void OnLevelUpButtonClick()
  {
    ItemDataInfo data_info = GetArtifactInfoById(m_CurrentArtifactId);
    if (data_info != null) {
      LegacyLevelupConfig legacyLvUpCfg = LegacyLevelupConfigProvider.Instance.GetDataById(data_info.Level);
      if (legacyLvUpCfg != null) {
        int legacyIndex = GetArfifactIndex(m_CurrentArtifactId);
        if (legacyIndex >= 0 && legacyIndex < legacyLvUpCfg.m_CostItemList.Count) {
          int costitemId = legacyLvUpCfg.m_CostItemList[legacyIndex];
          int currentNum = 0;
          ItemDataInfo costItemDataInfo = GetItemDataInfoById(costitemId);
          if (costItemDataInfo != null) {
            currentNum = costItemDataInfo.ItemNum;
          }
          int max = legacyLvUpCfg.m_CostNum;
          if (currentNum >= max) {
            LogicSystem.PublishLogicEvent("ge_upgrade_legacy", "lobby", legacyIndex, m_CurrentArtifactId, true);
          } else {
            //物品不够、钻来凑
            int DiamondNum = Mathf.CeilToInt(legacyLvUpCfg.m_Rate * (max - currentNum));
            RoleInfo role_info = LobbyClient.Instance.CurrentRole;
            string CHN_CONFIRM = StrDictionaryProvider.Instance.GetDictString(4);//确定
            string CHN_CANCEL = StrDictionaryProvider.Instance.GetDictString(9);//取消
            string CHN_LEVELUP = StrDictionaryProvider.Instance.GetDictString(404);//升级
            if (role_info != null && role_info.Gold >= DiamondNum) {
              DashFire.MyAction<int> Func = HandleDialog;
              string CHN_DESC = StrDictionaryProvider.Instance.GetDictString(401);
              CHN_DESC = string.Format(CHN_DESC, DiamondNum);
              LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", CHN_DESC, null, CHN_LEVELUP, CHN_CANCEL, Func, false);
            } else {
              string CHN_DESC = StrDictionaryProvider.Instance.GetDictString(402);
              CHN_DESC = string.Format(CHN_DESC, DiamondNum);
              LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", CHN_DESC, CHN_CONFIRM, null, null, null, false);
            }
          }
        }
      }
    }
  }

  //回调
  void HandleDialog(int action)
  {
    int legacyIndex = GetArfifactIndex(m_CurrentArtifactId);
    if (action == 1) {
      LogicSystem.PublishLogicEvent("ge_upgrade_legacy", "lobby", legacyIndex, m_CurrentArtifactId, true);
    }
  }

  //处理升级结果
  public void HandleLeveUpResult(int index, int artifactId, DashFire.Network.GeneralOperationResult result)
  {
    try {
      if (result == DashFire.Network.GeneralOperationResult.LC_Succeed) {
        UpdateArtifactInfo(artifactId);
        if (artifactIntroduce != null)
          artifactIntroduce.SetIntroduce(m_CurrentArtifactId, true);
        if (artifactTitle != null)
          artifactTitle.UpdateTitleInfo();
      } else {
        string CHN_DESC = StrDictionaryProvider.Instance.GetDictString(403);
        string CHN_CONFIRM = StrDictionaryProvider.Instance.GetDictString(4);
        LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", CHN_DESC, CHN_CONFIRM, null, null, null, false);
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  public void OnLeftButtonClick()
  {
    int index = GetArfifactIndex(m_CurrentArtifactId);
    if (index > 0 && index < spIndexHintArr.Length) {
      RoleInfo role_info = LobbyClient.Instance.CurrentRole;
      if (role_info != null) {
        ItemDataInfo data_info = role_info.GetLegacyData(--index);
        if (data_info != null) {
          TransGrid(m_OffsetX);
          SetArtifactId(data_info.ItemId);
          UpdateArtifactInfo(data_info.ItemId);
          if (artifactIntroduce != null) artifactIntroduce.SetIntroduce(m_CurrentArtifactId, data_info.IsUnlock);
        }
      }
    }
  }
  public void OnRightButtonClick()
  {
    int index = GetArfifactIndex(m_CurrentArtifactId);
    if (index >= 0 && index < spIndexHintArr.Length - 1) {
      RoleInfo role_info = LobbyClient.Instance.CurrentRole;
      if (role_info != null) {
        ItemDataInfo data_info = role_info.GetLegacyData(index + 1);
        if (data_info != null) {
          TransGrid(-m_OffsetX);
          SetArtifactId(data_info.ItemId);
          UpdateArtifactInfo(data_info.ItemId);
          if (artifactIntroduce != null) artifactIntroduce.SetIntroduce(m_CurrentArtifactId, data_info.IsUnlock);
        }
      }
    }
  }
  private void TransGrid(float offsetX)
  {
    if (uiGrid != null) {
      if (leftButton != null && rightButton != null) {
        leftButton.isEnabled = false;
        rightButton.isEnabled = false;
      }
      Vector3 pos = uiGrid.transform.localPosition;
      Vector3 newPos = new Vector3(pos.x + offsetX, pos.y, pos.z);
      TweenPosition tween = TweenPosition.Begin(uiGrid.gameObject, 0.2f, newPos);
      if (tween != null)
        EventDelegate.Add(tween.onFinished, OnTweenFinished);
    }
  }
  //
  private void OnTweenFinished()
  {
    if (leftButton != null) leftButton.isEnabled = true;
    if (rightButton != null) rightButton.isEnabled = true;
  }
  //获取神器信息
  public ItemDataInfo GetArtifactInfoById(int itemId)
  {
    RoleInfo role_info = LobbyClient.Instance.CurrentRole;
    if (role_info != null) {
      foreach (ItemDataInfo item_info in role_info.Legacys) {
        if (item_info != null && item_info.ItemId == itemId) {
          return item_info;
        }
      }
    }
    return null;
  }
  //获取物品信息
  public ItemDataInfo GetItemDataInfoById(int itemId)
  {
    RoleInfo role_info = LobbyClient.Instance.CurrentRole;
    if (role_info != null) {
      foreach (ItemDataInfo item_info in role_info.Items) {
        if (item_info != null && item_info.ItemId == itemId) {
          return item_info;
        }
      }
    }
    return null;
  }
  //获取神器Index
  public int GetArfifactIndex(int artifactId)
  {
    RoleInfo role_info = LobbyClient.Instance.CurrentRole;
    if (role_info != null) {
      for (int index = 0; index < role_info.Legacys.Length; ++index) {
        if (role_info.Legacys[index] != null && artifactId == role_info.Legacys[index].ItemId)
          return index;
      }
    }
    return -1;
  }
  //点击物品
  public void OnItemBtnClick()
  {
    GameObject ipgo = UIManager.Instance.GetWindowGoByName("ItemProperty");
    if (ipgo != null && !NGUITools.GetActive(ipgo)) {
      ItemProperty ip = ipgo.GetComponent<ItemProperty>();
      ip.ShowItemProperty(m_CostItemId, 1);
      UIManager.Instance.ShowWindowByName("ItemProperty");
    }
  }
}
