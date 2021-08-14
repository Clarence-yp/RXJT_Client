using UnityEngine;
using DashFire;
using System;
using System.Collections;
using System.Collections.Generic;

public class GamePokeyManager : MonoBehaviour
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
        eventlist.Clear();
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  // Use this for initialization
  void Start()
  {
    //GamePokey
    if (eventlist != null) { eventlist.Clear(); }
    if (changeitemDic != null) { changeitemDic.Clear(); }
    object eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<int[], int[], int[]>("ge_add_item", "bag", AddItem);
    if (eo != null) { eventlist.Add(eo); }
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<int[], int[], int[], DashFire.Network.GeneralOperationResult>("ge_delete_item", "bag", DeleteItemInCheck);
    if (eo != null) { eventlist.Add(eo); }
    //     eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<int, int>("ge_delete_item", "equipment", DeleteInEquipment);
    //     if (eo != null) { eventlist.Add(eo); }
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_ui_unsubscribe", "ui", UnSubscribe);
    if (eo != null) eventlist.Add(eo);
    //HeroEquipment
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<int, int, int, int, DashFire.Network.GeneralOperationResult>("ge_fiton_equipment", "equipment", HeroPutOnEquipment);
    if (eo != null) { eventlist.Add(eo); }
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<int, int, int, int, DashFire.Network.GeneralOperationResult>("ge_upgrade_item", "equipment", UpgradeItem);
    if (eo != null) { eventlist.Add(eo); }
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<DashFire.CharacterProperty, DashFire.Network.GeneralOperationResult>("ge_request_role_property", "property", HeroUpdateProperty);
    if (eo != null) { eventlist.Add(eo); }
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_update_role_dynamic_property", "ui", UpdateDynamicProperty);
    if (eo != null) { eventlist.Add(eo); }
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<List<NewEquipInfo>>("ge_new_equip", "equipment", NewEquipment);
    if (eo != null) { eventlist.Add(eo); }

    //RequestGamePokey
    try {
      DashFire.GfxSystem.EventChannelForLogic.Publish("ge_request_items", "ui");
    } catch (System.Exception ex) {
      DashFire.LogSystem.Error("Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
    SetRoleStaticProperty();
    RecordSomeWidget();
    SetRoleDynamicProperty();
    InitEquipmentTransform();
    UIManager.Instance.HideWindowByName("GamePokey");

    //money
  }

  // Update is called once per frame
  void Update()
  {
    if (signForInitPos) {
      SetInitPosition();
      signForInitPos = false;
    }
    SetRoleDynamicProperty();
  }

  void SetRoleStaticProperty()
  {
    DashFire.RoleInfo player = DashFire.LobbyClient.Instance.CurrentRole;
    if (player != null) {
      Transform tf = transform.parent.Find("Head/name");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = player.Nickname;
        }
      }
      tf = transform.parent.Find("chartexture");
      if (tf != null) {
        UISprite us = tf.gameObject.GetComponent<UISprite>();
        if (us != null) {
          if (player.HeroId == 1) {
            us.spriteName = "kuang-zhan-shi";
          } else {
            us.spriteName = "ci-ke";
          }
        }
      }
      tf = transform.parent.Find("Head/headPic");
      if (tf != null) {
        UISprite us = tf.gameObject.GetComponent<UISprite>();
        if (us != null) {
          if (player.HeroId == 1) {
            us.spriteName = "jianshi";
          } else {
            us.spriteName = "cike";
          }
        }
      }
      tf = transform.parent.Find("RoleInfo/lb_name");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = player.Nickname;
        }
      }
      tf = transform.parent.Find("RoleInfo/lb_zhiye");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        DashFire.Data_PlayerConfig dpc = DashFire.PlayerConfigProvider.Instance.GetPlayerConfigById(player.HeroId);
        if (ul != null && dpc != null) {
          ul.text = dpc.m_Name;
        }
      }
    }
    fightscore = Mathf.FloorToInt(player.FightingScore);
  }

  void RecordSomeWidget()
  {
    Transform tf = transform.parent.Find("Head/level");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        labellv = ul;
      }
    }
    tf = transform.parent.Find("HeroEquip/Title/LabelSword");
    if (tf != null) {
      fightlabellocalpos = tf.position;
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        labellvfight = ul;
      }
    }
    tf = transform.parent.Find("HeroEquip/Title/LabelSome");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        labelexp = ul;
      }
    }
    tf = transform.parent.Find("HeroEquip/Title/ProgressBarBack");
    if (tf != null) {
      UIProgressBar upb = tf.gameObject.GetComponent<UIProgressBar>();
      if (upb != null) {
        progressupb = upb;
      }
    }
    tf = transform.parent.Find("Money/Label");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        money = ul;
      }
    }
    tf = transform.parent.Find("Diamond/Label");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        diamond = ul;
      }
    }
    tf = transform.parent.Find("HeroEquip/Title/LabelLove");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        labelhpmax = ul;
      }
    }
    tf = transform.parent.Find("RoleInfo/lb_level");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        roleinfolvlabel = ul;
      }
    }
  }

  void SetRoleDynamicProperty()
  {
    DashFire.RoleInfo player = DashFire.LobbyClient.Instance.CurrentRole;
    if (player != null) {
      if (labellv != null) {
        labellv.text = "" + player.Level;
      }
      if (roleinfolvlabel != null) {
        roleinfolvlabel.text = "Lv." + player.Level;
      }
      if (labellvfight != null) {
        labellvfight.text = Mathf.FloorToInt(player.FightingScore).ToString();
      }
      if (money != null) {
        money.text = player.Money.ToString();
      }
      if (diamond != null) {
        diamond.text = player.Gold.ToString();
      }
      int needexp = 0;
      DashFire.PlayerLevelupExpConfig plec = DashFire.PlayerConfigProvider.Instance.GetPlayerLevelupExpConfigById(player.HeroId);
      if (plec != null) {
        needexp = plec.m_ConsumeExp;
      }

      UpdateEx(player.Level, player.Exp);

      if (labelhpmax != null) {
        DashFire.UserInfo ui = player.GetPlayerSelfInfo();
        if (ui != null) {
          DashFire.CharacterProperty cp = ui.GetActualProperty();
          if (cp != null) {
            labelhpmax.text = cp.HpMax.ToString();
          }
        }
      }
      int score = Mathf.FloorToInt(player.FightingScore);
      if (score != fightscore) {
        if (score - fightscore > 0) {
          DashFire.LogicSystem.EventChannelForGfx.Publish("ge_screen_tip", "ui",
          fightlabellocalpos.x, fightlabellocalpos.y, fightlabellocalpos.z, false, "[00ff00]+" + (score - fightscore) + "[-]");
        } else {
          DashFire.LogicSystem.EventChannelForGfx.Publish("ge_screen_tip", "ui",
          fightlabellocalpos.x, fightlabellocalpos.y, fightlabellocalpos.z, false, "[ff0000]" + (score - fightscore) + "[-]");
        }
        fightscore = score;
      }
    }
  }

  //更新经验值
  public void UpdateEx(int level, int exp)
  {
    int curent = 0, max = 0;
    int baseExp = 0;
    if (level == 1) {
      baseExp = 0;
    } else {
      PlayerLevelupExpConfig expCfg = PlayerConfigProvider.Instance.GetPlayerLevelupExpConfigById(level - 1);
      if (expCfg != null)
        baseExp = expCfg.m_ConsumeExp;
    }
    PlayerLevelupExpConfig expCfgHith = PlayerConfigProvider.Instance.GetPlayerLevelupExpConfigById(level);
    if (expCfgHith != null) {
      max = expCfgHith.m_ConsumeExp - baseExp;
    }
    curent = exp - baseExp;
    if (progressupb != null && max != 0) {
      progressupb.value = curent / (float)max;
    }
    if (labelexp != null && max != 0) {
      labelexp.text = curent.ToString() + "/" + max;
    }
  }

  void UpgradeItem(int pos, int id, int itemlevel, int item_random_property, DashFire.Network.GeneralOperationResult result)
  {
    try {
      if (DashFire.Network.GeneralOperationResult.LC_Succeed == result) {
        HeroPutOnEquipment(id, pos, itemlevel, item_random_property, DashFire.Network.GeneralOperationResult.LC_Succeed);
        GameObject go = UIManager.Instance.GetWindowGoByName("ItemProperty");
        if (go != null) {
          ItemProperty ip = go.GetComponent<ItemProperty>();
          if (ip != null) {
            ip.SetItemProperty(id, pos, itemlevel, item_random_property);
          }
        }
      }
      //       else {
      //         DashFire.LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", result.ToString(), "understand", null, null, null, false);
      //       }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  void SetInitPosition()
  {
    Transform tf = transform.Find("Left Scroll View");
    if (tf != null) {
      tf = tf.Find("Grid");
      if (tf != null) {
        UIGridForDFM ug = tf.gameObject.GetComponent<UIGridForDFM>();
        if (ug != null) {
          ug.repositionNow = true;
        }
      }
    }
    tf = transform.Find("Control - Simple Vertical Scroll Bar");
    if (tf != null) {
      UIScrollBar usb = tf.gameObject.GetComponent<UIScrollBar>();
      if (usb != null) {
        usb.value = 0;
      }
    }
  }

  public void AddItem(int[] item, int[] item_num, int[] item_append_property)
  {
    try {
      Transform tf = transform.Find("Left Scroll View");
      if (tf != null) {
        tf = tf.Find("Grid");
        if (tf == null) {
          Debug.Log("AddItem tf == null");
        }
      }
      int count = item.Length;
      for (int i = 0; i < count; ++i) {
        int itemcell = item[i];
        DashFire.ItemConfig item_data = DashFire.ItemConfigProvider.Instance.GetDataById(itemcell);
        if (null != item_data && item_data.m_CanWear) {
          GameObject go = DashFire.ResourceSystem.GetSharedResource("UI/GamePokey/ItemCell") as GameObject;
          go = NGUITools.AddChild(tf.gameObject, go);
          if (go != null) {
            //更改itemcell上的物品信息
            SetItemInformation(go, itemcell, item_append_property[i]);
            //记录物品id
            ItemClick ic = go.GetComponent<ItemClick>();
            if (ic != null) {
              ic.ID = itemcell;
              ic.PropertyId = item_append_property[i];
            }
          }
          //添加物品后更改控件名，便于后续工作
          go.transform.name = "Item" + itemcount++;
        }
      }
      if (tf != null) {
        UIGridForDFM ug = tf.gameObject.GetComponent<UIGridForDFM>();
        if (ug != null) {
          ug.repositionNow = true;
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  public void DeleteItem(int[] item)
  {
    Transform tf = transform.Find("Left Scroll View");
    if (tf != null) {
      tf = tf.Find("Grid");
      if (tf == null) {
        Debug.Log("AddItem tf == null");
      }
    }

    if (tf != null) {
      UIGridForDFM ug = tf.gameObject.GetComponent<UIGridForDFM>();
      if (ug != null) {
        ug.repositionNow = true;
      }
    }
  }
  public void DeleteItemInCheck(int[] item, int[] item_property_id, int[] item_num, DashFire.Network.GeneralOperationResult result)
  {
    try {
      if (DashFire.Network.GeneralOperationResult.LC_Succeed == result) {
        Transform tf = transform.Find("Left Scroll View");
        if (tf != null) {
          tf = tf.Find("Grid");
          Transform tfc = null;
          for (int j = 0; j < item.Length; ++j) {
            for (int i = 0; i < tf.childCount; ++i) {
              tfc = tf.GetChild(i);
              if (tfc != null) {
                ItemClick ic = tfc.gameObject.GetComponent<ItemClick>();
                if (ic != null && ic.ID == item[j] && ic.PropertyId == item_property_id[j]) {
                  NGUITools.DestroyImmediate(tfc.gameObject);
                  break;
                }
              }
            }
          }
          if (tf != null) {
            UIGridForDFM ug = tf.gameObject.GetComponent<UIGridForDFM>();
            if (ug != null) {
              ug.repositionNow = true;
            }
          }
        }
      } else {
        DashFire.LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", result.ToString(), "YES", null, null, null, false);
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  //   public void DeleteInEquipment(int id, int property)
  //   {
  //     try {
  //       Transform tf = transform.Find("Left Scroll View");
  //       if (tf != null) {
  //         tf = tf.Find("Grid");
  //         if (tf != null) {
  //           Transform tfc = null;
  //           for (int i = 0; i < tf.childCount; ++i) {
  //             tfc = tf.GetChild(i);
  //             if (tfc != null) {
  //               ItemClick ic = tfc.gameObject.GetComponent<ItemClick>();
  //               if (ic != null && ic.ID == id && ic.PropertyId == property) {
  //                 NGUITools.DestroyImmediate(tfc.gameObject);
  //                 break;
  //               }
  //             }
  //           }
  // 
  //           UIGridForDFM ug = tf.gameObject.GetComponent<UIGridForDFM>();
  //           if (ug != null) {
  //             ug.repositionNow = true;
  //           }
  //         }
  //       }
  //     } catch (Exception ex) {
  //       DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
  //     }
  //   }
  public void ArrangedItem()
  {
    Transform tf = transform.Find("Left Scroll View");
    if (tf != null) {
      tf = tf.Find("Grid");
      if (tf != null) {
        UIGridForDFM ug = tf.gameObject.GetComponent<UIGridForDFM>();
        if (ug != null) {
          //排序，规则未制定
          ug.sortRepositionForDF = true;
        }
      }
    }
  }
  private void SetItemInformation(GameObject go, int id, int propertyid)
  {
    DashFire.ItemConfig itemconfig = DashFire.LogicSystem.GetItemDataById(id);
    if (itemconfig == null) return;

    if (go != null) {
      Transform tf = go.transform.Find("Icon");
      if (tf != null) {
        UITexture ut = tf.gameObject.GetComponent<UITexture>();
        if (ut != null) {
          Texture tt = GamePokeyManager.GetTextureByPicName(itemconfig.m_ItemTrueName);
          if (tt != null) {
            ut.mainTexture = tt;
          } else {
            DashFire.ResLoadAsyncHandler.LoadAsyncItem(itemconfig.m_ItemTrueName, ut);
          }
        }
      }
      tf = go.transform.Find("LabelLevel");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = "";//itemconfig.m_Grade.ToString();
        }
      }
      tf = go.transform.Find("LabelName");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = itemconfig.m_ItemName;
          Color col;
          switch (itemconfig.m_PropertyRank) {
            case 1:
              col = new Color(1.0f, 1.0f, 1.0f);
              break;
            case 2:
              col = new Color(0.0f, 1.0f, 0.0f);
              break;
            case 3:
              col = new Color(0.0f, 0.0f, 1.0f);
              break;
            case 4:
              col = new Color(0.824f, 0.0f, 0.91f);
              break;
            case 5:
              col = new Color(1.0f, 0.54f, 0.19f);
              break;
            default:
              col = new Color(1.0f, 1.0f, 1.0f);
              break;
          }
          ul.color = col;
        }
      }
      tf = go.transform.Find("LabelFight");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = Mathf.FloorToInt(GetItemFightScore(itemconfig, propertyid, 1)).ToString();
        }
      }
      tf = go.transform.Find("LabelOccupation");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = itemconfig.m_ItemType;
        }
      }
      tf = go.transform.Find("IconKuang");
      if (tf != null) {
        UISprite us = tf.gameObject.GetComponent<UISprite>();
        if (us != null) {
          us.spriteName = "SEquipFrame" + itemconfig.m_PropertyRank;
        }
      }
    }
  }

  private float GetItemFightScore(int itemid, int propertyid, int itemlevel)
  {
    DashFire.ItemConfig itemconfig = DashFire.LogicSystem.GetItemDataById(itemid);
    if (itemconfig != null) {
      return GetItemFightScore(itemconfig, propertyid, itemlevel);
    }
    return 0f;
  }

  private float GetItemFightScore(DashFire.ItemConfig itemconfig, int propertyid, int itemlevel)
  {
    DashFire.RoleInfo ri = DashFire.LobbyClient.Instance.CurrentRole;
    if (itemconfig != null && ri != null) {
      DashFire.AppendAttributeConfig aac = DashFire.AppendAttributeConfigProvider.Instance.GetDataById(propertyid);
      if (aac == null) {
        return (DashFire.AttributeScoreConfigProvider.Instance.CalcAttributeScore(
          itemconfig.m_AttrData.GetAddHpMax(1.0f, ri.Level, itemlevel), itemconfig.m_AttrData.GetAddEpMax(1.0f, ri.Level, itemlevel), itemconfig.m_AttrData.GetAddAd(1.0f, ri.Level, itemlevel),
          itemconfig.m_AttrData.GetAddADp(1.0f, ri.Level, itemlevel), itemconfig.m_AttrData.GetAddMDp(1.0f, ri.Level, itemlevel), itemconfig.m_AttrData.GetAddCri(1.0f, ri.Level),
          itemconfig.m_AttrData.GetAddPow(1.0f, ri.Level), itemconfig.m_AttrData.GetAddBackHitPow(1.0f, ri.Level), itemconfig.m_AttrData.GetAddCrackPow(1.0f, ri.Level),
          itemconfig.m_AttrData.GetAddFireDam(1.0f, ri.Level), itemconfig.m_AttrData.GetAddIceDam(1.0f, ri.Level), itemconfig.m_AttrData.GetAddPoisonDam(1.0f, 1),
          itemconfig.m_AttrData.GetAddFireErd(1.0f, ri.Level), itemconfig.m_AttrData.GetAddIceErd(1.0f, ri.Level), itemconfig.m_AttrData.GetAddPoisonErd(1.0f, ri.Level)
          ));
      } else {
        return (DashFire.AttributeScoreConfigProvider.Instance.CalcAttributeScore(
         itemconfig.m_AttrData.GetAddHpMax(1.0f, ri.Level, itemlevel) + aac.GetAddHpMax(1.0f, ri.Level), itemconfig.m_AttrData.GetAddEpMax(1.0f, ri.Level, itemlevel) + aac.GetAddEpMax(1.0f, ri.Level),
         itemconfig.m_AttrData.GetAddAd(1.0f, ri.Level, itemlevel) + aac.GetAddAd(1.0f, ri.Level), itemconfig.m_AttrData.GetAddADp(1.0f, ri.Level, itemlevel) + aac.GetAddADp(1.0f, itemlevel),
         itemconfig.m_AttrData.GetAddMDp(1.0f, ri.Level, itemlevel) + aac.GetAddMDp(1.0f, ri.Level), itemconfig.m_AttrData.GetAddCri(1.0f, ri.Level) + aac.GetAddCri(1.0f, ri.Level),
         itemconfig.m_AttrData.GetAddPow(1.0f, ri.Level) + aac.GetAddPow(1.0f, ri.Level), itemconfig.m_AttrData.GetAddBackHitPow(1.0f, ri.Level) + aac.GetAddBackHitPow(1.0f, ri.Level),
         itemconfig.m_AttrData.GetAddCrackPow(1.0f, ri.Level) + aac.GetAddCrackPow(1.0f, ri.Level), itemconfig.m_AttrData.GetAddFireDam(1.0f, ri.Level) + aac.GetAddFireDam(1.0f, ri.Level),
         itemconfig.m_AttrData.GetAddIceDam(1.0f, ri.Level) + aac.GetAddIceDam(1.0f, ri.Level), itemconfig.m_AttrData.GetAddPoisonDam(1.0f, ri.Level) + aac.GetAddPoisonDam(1.0f, ri.Level),
         itemconfig.m_AttrData.GetAddFireErd(1.0f, ri.Level) + aac.GetAddFireDam(1.0f, ri.Level), itemconfig.m_AttrData.GetAddIceErd(1.0f, ri.Level) + aac.GetAddIceErd(1.0f, ri.Level),
         itemconfig.m_AttrData.GetAddPoisonErd(1.0f, ri.Level) + aac.GetAddPoisonErd(1.0f, ri.Level)));
      }
    }
    return 0f;
  }

  private void UpdateDynamicProperty()
  {
    try {
      SetRoleDynamicProperty();
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  private void HeroPutOnEquipment(int id, int pos, int itemLevel, int itemRandomProperty, DashFire.Network.GeneralOperationResult result)
  {
    try {
      if (DashFire.Network.GeneralOperationResult.LC_Succeed == result) {
        if (pos >= 0 && pos < 6) {
          equiparry[pos].SetEquipmentInfo(id, itemLevel, itemRandomProperty);
        }
      } else {
        if (DashFire.Network.GeneralOperationResult.LC_Failure_Position == result) {
          DashFire.LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", DashFire.StrDictionaryProvider.Instance.GetDictString(151),
          DashFire.StrDictionaryProvider.Instance.GetDictString(140), null, null, null, false);
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  private void HeroUpdateProperty(DashFire.CharacterProperty info, DashFire.Network.GeneralOperationResult result)
  {
    try {
      if (result == DashFire.Network.GeneralOperationResult.LC_Succeed) {
        Transform tfroot = transform.parent;
        if (tfroot != null) {
          Transform tf = tfroot.Find("RoleInfo/DragThing/Right0");
          if (tf != null) {
            UILabel ul = tf.gameObject.GetComponent<UILabel>();
            if (ul != null) {
              string str = "[ffffff]";
              if (info != null) {
                str += info.AttackBase + "\n";
                str += info.Critical * 100 + "%\n";
                str += info.CriticalPow * 100 + "%\n";
                str += info.CriticalBackHitPow * 100 + "%\n";
                str += info.CriticalCrackPow * 100 + "%\n";
                str += info.FireDamage + "\n";
                str += info.IceDamage + "\n";
                str += info.PoisonDamage + "[-]";
              }
              ul.text = str;
            }
          }
          tf = tfroot.Find("RoleInfo/DragThing/Right1");
          if (tf != null) {
            UILabel ul = tf.gameObject.GetComponent<UILabel>();
            if (ul != null) {
              string str = "[ffffff]";
              if (info != null) {
                str += info.HpMax + "\n";
                str += info.ADefenceBase + "\n";
                str += info.MDefenceBase + "\n";
                str += info.FireERD + "\n";
                str += info.IceERD + "\n";
                str += info.PoisonERD + "[-]";
              }
              ul.text = str;
            }
          }
          tf = tfroot.Find("RoleInfo/DragThing/Right2");
          if (tf != null) {
            UILabel ul = tf.gameObject.GetComponent<UILabel>();
            if (ul != null) {
              string str = "[ffffff]";
              if (info != null) {
                str += "+" + info.MoveSpeed * 100 + "%\n";
                str += info.HpRecover + "/5S\n";
                str += info.EnergyRecover + "/5S\n";
                str += "+" + /*info.EnergyRecover * 10*/0 + "%\n";
              }
              ul.text = str;
            }
          }
        }
      } else {
        DashFire.LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", result.ToString(), "understand", null, null, null, false);
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private void InitEquipmentTransform()
  {
    Transform tf = transform.parent.Find("HeroEquip/Equipment");
    if (tf != null) {
      for (int i = 0; i < 6; ++i) {
        Transform tf1 = tf.Find("Slot" + i);
        if (tf1 != null) {
          Transform tf2 = tf1.Find("icon");
          if (tf2 != null) {
            UITexture ut = tf2.gameObject.GetComponent<UITexture>();
            if (ut != null && equiparry.Length > i) {
              equiparry[i].Itemtexture = ut;
            }
          }
          tf2 = tf1.Find("Sprite");
          if (tf2 != null) {
            UISprite us = tf2.gameObject.GetComponent<UISprite>();
            if (us != null && equiparry.Length > i) {
              equiparry[i].Framesprite = us;
            }
          }
        }
      }
    }
  }
  private void NewEquipment(List<NewEquipInfo> neil)
  {
    try {
      if (neil != null && neil.Count > 0 && changeitemDic != null) {
        DashFire.ItemConfig ic = null;
        foreach (NewEquipInfo nei in neil) {
          if (nei != null) {
            ic = DashFire.ItemConfigProvider.Instance.GetDataById(nei.ItemId);
            if (ic != null) {
              if (changeitemDic.ContainsKey(ic.m_WearParts)) {
                ChangeNewEquip cne = changeitemDic[ic.m_WearParts];
                if (cne != null) {
                  float score = GetItemFightScore(nei.ItemId, nei.ItemRandomProperty, cne.needlevel);
                  if (score > cne.fightscore) {
                    changeitemDic[ic.m_WearParts] = new ChangeNewEquip(nei.ItemId, nei.ItemRandomProperty, score, cne.needlevel);
                  }
                }
              } else if (GetEquipmentInfo(ic.m_WearParts) != null) {
                EquipmentInfo ei = GetEquipmentInfo(ic.m_WearParts);
                float score0 = GetItemFightScore(ei.id, ei.propertyid, ei.level);
                float score1 = GetItemFightScore(nei.ItemId, nei.ItemRandomProperty, ei.level);
                if (score0 < score1) {
                  changeitemDic.Add(ic.m_WearParts, new ChangeNewEquip(nei.ItemId, nei.ItemRandomProperty, score1, ei.level));
                }
              } else {
                float score2 = GetItemFightScore(nei.ItemId, nei.ItemRandomProperty, 1);
                changeitemDic.Add(ic.m_WearParts, new ChangeNewEquip(nei.ItemId, nei.ItemRandomProperty, score2, 1));
              }
            }
          }
        }
        foreach (ChangeNewEquip cne in changeitemDic.Values) {
          if (cne != null) {
            GameObject go = UIManager.Instance.GetWindowGoByName("DynamicEquipment");
            if (go != null) {
              DynamicEquipment de = go.GetComponent<DynamicEquipment>();
              if (de != null) {
                de.SetEquipment(new ChangeNewEquip(cne.id, cne.propertyid, 0, 0));
                break;
              }
            }
          }
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private bool signForInitPos = true;
  private long itemcount = 99999;

  private UILabel money = null;
  private UILabel labellv = null;
  private UILabel diamond = null;
  private int fightscore = 0;
  private UILabel labellvfight = null;
  private UILabel labelexp = null;
  private UILabel labelhpmax = null;
  private UIProgressBar progressupb = null;
  private Vector3 fightlabellocalpos;
  private UILabel roleinfolvlabel = null;
  static private EquipmentInfo[] equiparry = { new EquipmentInfo(), new EquipmentInfo(), new EquipmentInfo(), new EquipmentInfo(), new EquipmentInfo(), new EquipmentInfo() };
  static public Dictionary<int, ChangeNewEquip> changeitemDic = new Dictionary<int, ChangeNewEquip>();
  static public EquipmentInfo GetEquipmentInfo(int pos)
  {
    if (pos >= 0 && pos < 6) {
      return equiparry[pos];
    } else {
      return null;
    }
  }

  static public Texture GetTextureByPicName(string picturename)
  {
    return DashFire.ResourceSystem.GetSharedResource(picturename) as Texture;
  }
}
public class EquipmentInfo
{
  public void SetEquipmentInfo(int itemid, int itemlevel, int itempro)
  {
    id = itemid;
    level = itemlevel;
    propertyid = itempro;
    DashFire.ItemConfig ic = DashFire.ItemConfigProvider.Instance.GetDataById(itemid);
    if (ic != null) {
      if (Itemtexture != null) {
        Texture tt = GamePokeyManager.GetTextureByPicName(ic.m_ItemTrueName);
        if (tt != null) {
          Itemtexture.mainTexture = tt;
        } else {
          DashFire.ResLoadAsyncHandler.LoadAsyncItem(ic.m_ItemTrueName, Itemtexture);
        }
      }
      if (Framesprite != null) {
        Framesprite.spriteName = "EquipFrame" + ic.m_PropertyRank;
      }
    }
  }
  public int id = 0;
  public int level = 0;
  public int propertyid = 0;
  public UITexture Itemtexture = null;
  public UISprite Framesprite = null;
}
public class ChangeNewEquip
{
  public ChangeNewEquip(int itemid, int itempro, float score, int level)
  {
    id = itemid;
    propertyid = itempro;
    fightscore = score;
    needlevel = level;
  }
  public int id = 0;
  public int propertyid = 0;
  public float fightscore = 0f;
  public int needlevel = 0;
}
