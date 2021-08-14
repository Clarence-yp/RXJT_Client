using UnityEngine;
using System.Collections;

public class ItemProperty : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {
    CalculateUIPosition(transform.Find("SpriteBackRight"));
    CalculateUIPosition(transform.Find("SpriteBackLeft"));
    Transform tf = gameObject.transform.Find("SpriteBackLeft");
    if (tf != null) {
      leftLocalPos = tf.localPosition;
    }
    tf = transform.Find("SpriteBackRight");
    if (tf != null) {
      rightLocalPos = tf.localPosition;
    }
    transform.localPosition = new Vector3(0f, -29f, 0f);
    UIManager.Instance.HideWindowByName("ItemProperty");
  }

  // Update is called once per frame
  void Update()
  {

  }

  public void ShowItemProperty(int itemId, int itemLevel)
  {
    SetItemProperty(itemId, 0, itemLevel, 0);
    ShowActionButton(false);
  }
  //用来显示与隐藏升级、装备按钮
  private void ShowActionButton(bool active)
  {
    Transform ts = transform.Find("SpriteInlay");
    if (ts != null) NGUITools.SetActive(ts.gameObject, active);
    ts = transform.Find("SpriteUpdate");
    if (ts != null) NGUITools.SetActive(ts.gameObject, active);
    ts = transform.Find("SpriteBackRight/Sprite");
    if (ts != null) NGUITools.SetActive(ts.gameObject, active);
  }

  public void SetItemProperty(int itemid, int pos, int itemlevel, int propertyid)
  {
    ShowActionButton(true);
    isCompareUI = false;
    ID = itemid;
    property = propertyid;
    position = pos;
    level = itemlevel;
    Transform tfc = gameObject.transform.Find("SpriteBackLeft");
    if (tfc != null) {
      NGUITools.SetActive(tfc.gameObject, false);
    }
    tfc = transform.Find("SpriteBackRight");
    if (tfc != null) {
      tfc.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
      tfc = tfc.Find("line/Label");
      if (tfc != null) {
        UILabel ul = tfc.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = DashFire.StrDictionaryProvider.Instance.GetDictString(152);
          ul.color = new Color(1.0f, 0.52549f, 0.18039f);
        }
      }
    }
    tfc = transform.Find("SpriteInlay");
    if (tfc != null) {
      //       UILabel ul = tfc.gameObject.GetComponent<UILabel>();
      //       if (ul != null) {
      //         ul.text = "镶嵌";
      //       }
      NGUITools.SetActive(tfc.gameObject, false);
    }
    tfc = transform.Find("SpriteUpdate/Label");
    if (tfc != null) {
      UILabel ul = tfc.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        ul.text = DashFire.StrDictionaryProvider.Instance.GetDictString(121);
      }
    }
    tfc = transform.Find("SpriteUpdate/money");
    if (tfc != null) {
      UILabel ul = tfc.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        DashFire.ItemLevelupConfig iluc = DashFire.ItemLevelupConfigProvider.Instance.GetDataById(level);
        if (iluc != null) {
          ul.text = (iluc.m_PartsList.Count > position ? iluc.m_PartsList[position] : 0).ToString();
        } else {
          ul.text = "0";
        }
      }
    }
    tfc = transform.Find("SpriteUpdate");
    if (tfc != null) {
      UIButton ub = tfc.GetComponent<UIButton>();
      //UISprite us = tfc.gameObject.GetComponent<UISprite>();
      if (/*us != null &&*/ ub != null) {
        ub.normalSprite = "db_ts";
        DashFire.RoleInfo ri = DashFire.LobbyClient.Instance.CurrentRole;
        if (itemlevel >= ri.Level) {
          ub.isEnabled = false;
        } else {
          ub.isEnabled = true;
        }
      }
    }
    tfc = transform.Find("SpriteUpdate/xiaohao");
    if (tfc != null) {
      UILabel ul = tfc.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        ul.text = DashFire.StrDictionaryProvider.Instance.GetDictString(155);
      }
    }
    tfc = transform.Find("SpriteUpdate/Sprite");
    if (tfc != null) {
      NGUITools.SetActive(tfc.gameObject, true);
    }
    DashFire.ItemConfig ic = DashFire.ItemConfigProvider.Instance.GetDataById(itemid);
    tfc = transform.Find("SpriteUpdate");
    if (tfc != null && ic != null) {
      if (!ic.m_CanUpgrade) {
        NGUITools.SetActive(tfc.gameObject, false);
      } else {
        NGUITools.SetActive(tfc.gameObject, true);
      }
    }
    SetItemHeadProperty(itemid, itemlevel, propertyid, transform.Find("SpriteBackRight"));
    CompareProperty(0, 0, 0, 0, 0, 0);
    CalculateUIPosition(transform.Find("SpriteBackRight"));
  }

  public void InLay()
  {
    if (isCompareUI) {
      CloseItemProterty();
      DashFire.GfxSystem.EventChannelForLogic.Publish("ge_mount_equipment", "lobby", ID, property, position);
    }
  }

  public void ItemUpdate()
  {
    if (isCompareUI) {
      int[] sell = new int[] { ID };
      int[] propertarray = new int[] { property };
      DashFire.GfxSystem.EventChannelForLogic.Publish("ge_discard_item", "lobby", sell, propertarray);
      CloseItemProterty();
//       GameObject go = UIManager.Instance.GetWindowGoByName("TaskAward");
//       if (go != null) {
//         TaskAward ta = go.GetComponent<TaskAward>();
//         DashFire.ItemConfig ic = DashFire.ItemConfigProvider.Instance.GetDataById(ID);
//         if (ta != null && ic != null) {
//           ta.SetSellGain("X " + ic.m_SellingPrice, ic.m_SellGainGoldProb == float.Epsilon ? null : ("X " + ic.m_SellGainGoldProb * 100 + "%"));
//         }
//         UIManager.Instance.ShowWindowByName("TaskAward");
//       }
    } else {
      DashFire.ItemLevelupConfig iluc = DashFire.ItemLevelupConfigProvider.Instance.GetDataById(level);
      DashFire.RoleInfo ri = DashFire.LobbyClient.Instance.CurrentRole;
      if (iluc != null && ri != null) {
        int costmoney = iluc.m_PartsList.Count > position ? iluc.m_PartsList[position] : 0;
        if (costmoney > ri.Money) {
          DashFire.MyAction<int> fun = Buttonwhich;
          DashFire.LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", DashFire.StrDictionaryProvider.Instance.GetDictString(122),
          DashFire.StrDictionaryProvider.Instance.GetDictString(140), null, null, null/*fun*/, false);
        } else {
          DashFire.LogicSystem.PublishLogicEvent("ge_upgrade_item", "lobby", position, ID, false);
        }
      }
    }
  }
  void Buttonwhich(int buttonid)
  {
    if (buttonid == 2) {
      DashFire.ItemLevelupConfig iluc = DashFire.ItemLevelupConfigProvider.Instance.GetDataById(level);
      DashFire.RoleInfo ri = DashFire.LobbyClient.Instance.CurrentRole;
      if (iluc != null && ri != null) {
        int costmoney = iluc.m_PartsList.Count > position ? iluc.m_PartsList[position] : 0;
        int needmoney = costmoney - (int)(ri.Money);
        if (needmoney > 0) {
          float needgold = needmoney * (iluc.m_Rate == float.Epsilon ? 1 : iluc.m_Rate);
          if (needgold > ri.Gold) {
            DashFire.MyAction<int> fun = Buttonwhichone;
            DashFire.LogicSystem.EventChannelForGfx.Publish("ge_show_dialog", "ui", DashFire.StrDictionaryProvider.Instance.GetDictString(123), "YES", null, null, fun, false);
          } else {
            DashFire.LogicSystem.PublishLogicEvent("ge_upgrade_item", "lobby", position, ID, true);
          }
        }
      }
    }
  }
  void Buttonwhichone(int id)
  {
  }

  public void CloseItemProterty()
  {
    Transform tf = transform.Find("SpriteUpdate");
    if (tf != null) {
      UIButton ub = tf.GetComponent<UIButton>();
      if (ub != null) {
        ub.isEnabled = true;
      }
    }
    UIManager.Instance.HideWindowByName("ItemProperty");
    //UIManager.Instance.ShowWindowByName("GamePokey");
  }

  public void SetGamePokey(GameObject go)
  {
    gamepokey = go;
  }

  private void CalculateUIPosition(Transform whichtf)
  {
    if (whichtf == null) return;
    Transform tfd = whichtf.Find("Container/DragThing");
    if (tfd != null) {
      Vector3 v3 = tfd.localPosition;
      tfd.localPosition = new Vector3(v3.x, 0.0f, v3.z);
    }

    Transform tf = whichtf.Find("Container/DragThing");
    Transform tfc = tf;
    Vector3 pos = new Vector3();
    if (tf != null) {
      tf = tfc.Find("Property");
      if (tf != null) {
        pos = tf.localPosition;
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          pos = new Vector3(pos.x, pos.y - ul.localSize.y + 20, 0.0f);
        }
      }
      tf = tfc.Find("StarRock");
      if (tf != null) {
        tf.localPosition = pos;
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          pos = new Vector3(pos.x, pos.y - ul.localSize.y - 0, 0.0f);
        }
      }
      tf = tfc.Find("Bula");
      if (tf != null) {
        tf.localPosition = pos;
        //计算是否可拖动
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
        }
      }
    }
  }

  private void SetLabelProperty(DashFire.ItemConfig itemconfig, int itemlevel, int propertyid, Transform whichtf)
  {
    if (itemconfig == null || whichtf == null) return;
    DashFire.AppendAttributeConfig aac = DashFire.AppendAttributeConfigProvider.Instance.GetDataById(propertyid);
    int level = 1;
    string str = "[ffffff]";
    float data = 0.0f;
    data = itemconfig.m_AttrData.GetAddHpMax(1.0f, level, itemlevel);
    if (Mathf.Abs(data - 0) > float.Epsilon) {
      str += (DashFire.StrDictionaryProvider.Instance.GetDictString(101) + UIManager.GetItemProtetyStr(data, itemconfig.m_AttrData.m_HpMaxType) + "\n");
    }
    data = itemconfig.m_AttrData.GetAddAd(1.0f, level, itemlevel);
    if (Mathf.Abs(data - 0) > float.Epsilon) {
      str += (DashFire.StrDictionaryProvider.Instance.GetDictString(102) + UIManager.GetItemProtetyStr(data, itemconfig.m_AttrData.m_AdType) + "\n");
    }
    data = itemconfig.m_AttrData.GetAddADp(1.0f, level, itemlevel);
    if (Mathf.Abs(data - 0) > float.Epsilon) {
      str += (DashFire.StrDictionaryProvider.Instance.GetDictString(103) + UIManager.GetItemProtetyStr(data, itemconfig.m_AttrData.m_ADpType) + "\n");
    }
    data = itemconfig.m_AttrData.GetAddMDp(1.0f, level, itemlevel);
    if (Mathf.Abs(data - 0) > float.Epsilon) {
      str += (DashFire.StrDictionaryProvider.Instance.GetDictString(104) + UIManager.GetItemProtetyStr(data, itemconfig.m_AttrData.m_MDpType) + "\n");
    }
    switch (itemconfig.m_DamageType) {
      case DashFire.ElementDamageType.DC_Fire:
        str += DashFire.StrDictionaryProvider.Instance.GetDictString(118);
        break;
      case DashFire.ElementDamageType.DC_Ice:
        str += DashFire.StrDictionaryProvider.Instance.GetDictString(119);
        break;
      case DashFire.ElementDamageType.DC_Poison:
        str += DashFire.StrDictionaryProvider.Instance.GetDictString(120);
        break;
      case DashFire.ElementDamageType.DC_None:
        break;
      default: break;
    }
    str += "[-]";
    Transform tf = whichtf.Find("Container/DragThing/Property");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        ul.text = str;
      }
    }
    str = "";
    if (aac != null) {
      data = aac.GetAddCri(1.0f, level);
      if (Mathf.Abs(data - 0) > float.Epsilon) {
        str += (DashFire.StrDictionaryProvider.Instance.GetDictString(105) + "+" + Mathf.FloorToInt(data * 100) + "%\n");
      }
      data = aac.GetAddPow(1.0f, level);
      if (Mathf.Abs(data - 0) > float.Epsilon) {
        str += (DashFire.StrDictionaryProvider.Instance.GetDictString(106) + "+" + Mathf.FloorToInt(data * 100) + "%\n");
      }
      data = aac.GetAddBackHitPow(1.0f, level);
      if (Mathf.Abs(data - 0) > float.Epsilon) {
        str += (DashFire.StrDictionaryProvider.Instance.GetDictString(107) + "+" + Mathf.FloorToInt(data * 100) + "%\n");
      }
      data = aac.GetAddCrackPow(1.0f, level);
      if (Mathf.Abs(data - 0) > float.Epsilon) {
        str += (DashFire.StrDictionaryProvider.Instance.GetDictString(108) + "+" + Mathf.FloorToInt(data * 100) + "%\n");
      }
    }
    data = itemconfig.m_AttrData.GetAddFireDam(1.0f, level);
    if (Mathf.Abs(data - 0) > float.Epsilon) {
      str += (DashFire.StrDictionaryProvider.Instance.GetDictString(109) + UIManager.GetItemProtetyStr(data, itemconfig.m_AttrData.m_FireDamType) + "\n");
    }
    data = itemconfig.m_AttrData.GetAddIceDam(1.0f, level);
    if (Mathf.Abs(data - 0) > float.Epsilon) {
      str += (DashFire.StrDictionaryProvider.Instance.GetDictString(110) + UIManager.GetItemProtetyStr(data, itemconfig.m_AttrData.m_IceDamType) + "\n");
    }
    data = itemconfig.m_AttrData.GetAddPoisonDam(1.0f, level);
    if (Mathf.Abs(data - 0) > float.Epsilon) {
      str += (DashFire.StrDictionaryProvider.Instance.GetDictString(111) + UIManager.GetItemProtetyStr(data, itemconfig.m_AttrData.m_PoisonDamType) + "\n");
    }
    data = itemconfig.m_AttrData.GetAddFireErd(1.0f, level);
    if (Mathf.Abs(data - 0) > float.Epsilon) {
      str += (DashFire.StrDictionaryProvider.Instance.GetDictString(112) + UIManager.GetItemProtetyStr(data, itemconfig.m_AttrData.m_FireErdType) + "\n");
    }
    data = itemconfig.m_AttrData.GetAddIceErd(1.0f, level);
    if (Mathf.Abs(data - 0) > float.Epsilon) {
      str += (DashFire.StrDictionaryProvider.Instance.GetDictString(113) + UIManager.GetItemProtetyStr(data, itemconfig.m_AttrData.m_IceErdType) + "\n");
    }
    data = itemconfig.m_AttrData.GetAddPoisonErd(1.0f, level);
    if (Mathf.Abs(data - 0) > float.Epsilon) {
      str += (DashFire.StrDictionaryProvider.Instance.GetDictString(114) + UIManager.GetItemProtetyStr(data, itemconfig.m_AttrData.m_PoisonErdType) + "\n");
    }
    if (str != "") {
      str = "\n[00ff00]" + str + "[-]";
    }
    tf = whichtf.Find("Container/DragThing/StarRock");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        ul.text = str;
      }
    }
    tf = whichtf.Find("Container/DragThing/Bula");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        ul.text = itemconfig.m_Description;
      }
    }
  }
  public void Compare(int leftitem, int leftlevel, int leftpropertyid, int rightitem, int rightlevel, int rightpropertyid, int pos)
  {
    Transform tf = null;
    isCompareUI = true;
    ID = rightitem;
    property = rightpropertyid;
    position = pos;
    if (leftitem == 0) {
      tf = gameObject.transform.Find("SpriteBackLeft");
      if (tf != null) {
        NGUITools.SetActive(tf.gameObject, false);
      }
      tf = transform.Find("SpriteBackRight");
      if (tf != null) {
        tf.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        tf = tf.Find("line/Label");
        if (tf != null) {
          UILabel ul = tf.gameObject.GetComponent<UILabel>();
          if (ul != null) {
            ul.text = DashFire.StrDictionaryProvider.Instance.GetDictString(153);
            ul.color = Color.white;
          }
        }
      }
    } else {
      tf = gameObject.transform.Find("SpriteBackLeft");
      if (tf != null) {
        tf.localPosition = leftLocalPos;
        NGUITools.SetActive(tf.gameObject, true);
      }
      tf = transform.Find("SpriteBackRight");
      if (tf != null) {
        tf.localPosition = rightLocalPos;
        tf = tf.Find("line/Label");
        if (tf != null) {
          UILabel ul = tf.gameObject.GetComponent<UILabel>();
          if (ul != null) {
            ul.text = DashFire.StrDictionaryProvider.Instance.GetDictString(152);
            ul.color = Color.white;
          }
        }
      }
    }
    tf = transform.Find("SpriteInlay/Label");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        ul.text = DashFire.StrDictionaryProvider.Instance.GetDictString(116);
      }
      NGUITools.SetActive(tf.parent.gameObject, true);
    }
    tf = transform.Find("SpriteUpdate/Label");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        ul.text = DashFire.StrDictionaryProvider.Instance.GetDictString(117);
      }
    }
    tf = transform.Find("SpriteUpdate/xiaohao");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        ul.text = "";//DashFire.StrDictionaryProvider.Instance.GetDictString(154);
      }
    }
    tf = transform.Find("SpriteUpdate/Sprite");
    if (tf != null) {
      NGUITools.SetActive(tf.gameObject, false);
    }

    DashFire.ItemConfig itemconfig = DashFire.LogicSystem.GetItemDataById(rightitem);
    if (itemconfig != null) {
      tf = transform.Find("SpriteUpdate/money");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = itemconfig.m_SellingPrice.ToString();
        }
      }
      tf = transform.Find("SpriteBackRight/Texture");
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
      tf = transform.Find("SpriteBackRight/TextureFrame");
      if (tf != null) {
        UISprite us = tf.gameObject.GetComponent<UISprite>();
        if (us != null) {
          us.spriteName = "EquipFrame" + itemconfig.m_PropertyRank;
        }
      }
      tf = transform.Find("SpriteBackRight/LabelLv");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = "Lv." + rightlevel;
        }
      }
      tf = transform.Find("SpriteBackRight/LabelName");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = itemconfig.m_ItemName;
          Color col = new Color();
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
      tf = transform.Find("SpriteUpdate");
      if (tf != null) {
        NGUITools.SetActive(tf.gameObject, true);
        UIButton ub = tf.gameObject.GetComponent<UIButton>();
        if (ub != null) {
          ub.normalSprite = "db_cs";
        }
      }
      SetItemHeadProperty(leftitem, leftlevel, leftpropertyid, transform.Find("SpriteBackLeft"));
      SetItemHeadProperty(rightitem, rightlevel, rightpropertyid, transform.Find("SpriteBackRight"));
    }
    CompareProperty(leftitem, leftlevel, leftpropertyid, rightitem, rightlevel, rightpropertyid);
    CalculateUIPosition(transform.Find("SpriteBackLeft"));
    CalculateUIPosition(transform.Find("SpriteBackRight"));
  }
  private void SetItemHeadProperty(int itemid, int itemlevel, int propertyid, Transform whichtf)
  {
    DashFire.ItemConfig itemconfig = DashFire.LogicSystem.GetItemDataById(itemid);
    if (itemconfig != null) {
      Transform tf = whichtf.Find("Texture");
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
      tf = whichtf.Find("TextureFrame");
      if (tf != null) {
        UISprite us = tf.gameObject.GetComponent<UISprite>();
        if (us != null) {
          us.spriteName = "EquipFrame" + itemconfig.m_PropertyRank;
        }
      }
      tf = whichtf.Find("LabelLv");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = "Lv." + itemlevel;
        }
      }
      tf = whichtf.Find("LabelName");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = itemconfig.m_ItemName;
          Color col = new Color();
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
      SetLabelProperty(itemconfig, itemlevel, propertyid, whichtf);
    }
  }
  private void CompareProperty(int leftitem, int leftitemlevel, int leftpropertyid, int rightitem, int rightitemlevel, int rightpropertyid)
  {
    if (isCompareUI) {
      int level = 1;
      float dataL = 0.0f;
      float dataR = 0.0f;
      string str = "";
      DashFire.ItemConfig itemconfigL = DashFire.LogicSystem.GetItemDataById(leftitem);
      DashFire.ItemConfig itemconfigR = DashFire.LogicSystem.GetItemDataById(rightitem);
      if (itemconfigL != null && itemconfigR != null) {
        DashFire.AppendAttributeConfig aacL = DashFire.AppendAttributeConfigProvider.Instance.GetDataById(leftpropertyid);
        DashFire.AppendAttributeConfig aacR = DashFire.AppendAttributeConfigProvider.Instance.GetDataById(rightpropertyid);
        dataL = UIManager.GetItemPropertyData(itemconfigL.m_AttrData.GetAddHpMax(1.0f, level, leftitemlevel), itemconfigR.m_AttrData.m_HpMaxType);
        dataR = UIManager.GetItemPropertyData(itemconfigR.m_AttrData.GetAddHpMax(1.0f, level, leftitemlevel), itemconfigR.m_AttrData.m_HpMaxType);
        if (Mathf.Abs(dataR - dataL) > float.Epsilon) {
          str += (((dataR - dataL) > 0.0f ? "[00ffea]" : "[ff0000]") + DashFire.StrDictionaryProvider.Instance.GetDictString(101) + UIManager.GetItemProtetyStr((dataR - dataL), itemconfigR.m_AttrData.m_HpMaxType) + "[-]\n");
        }
        dataL = UIManager.GetItemPropertyData(itemconfigL.m_AttrData.GetAddAd(1.0f, level, leftitemlevel), itemconfigR.m_AttrData.m_AdType);
        dataR = UIManager.GetItemPropertyData(itemconfigR.m_AttrData.GetAddAd(1.0f, level, leftitemlevel), itemconfigR.m_AttrData.m_AdType);
        if (Mathf.Abs(dataR - dataL) > float.Epsilon) {
          str += (((dataR - dataL) > 0.0f ? "[00ffea]" : "[ff0000]") + DashFire.StrDictionaryProvider.Instance.GetDictString(102) + UIManager.GetItemProtetyStr((dataR - dataL), itemconfigR.m_AttrData.m_AdType) + "[-]\n");
        }
        dataL = UIManager.GetItemPropertyData(itemconfigL.m_AttrData.GetAddADp(1.0f, level, leftitemlevel), itemconfigR.m_AttrData.m_ADpType);
        dataR = UIManager.GetItemPropertyData(itemconfigR.m_AttrData.GetAddADp(1.0f, level, leftitemlevel), itemconfigR.m_AttrData.m_ADpType);
        if (Mathf.Abs(dataR - dataL) > float.Epsilon) {
          str += (((dataR - dataL) > 0.0f ? "[00ffea]" : "[ff0000]") + DashFire.StrDictionaryProvider.Instance.GetDictString(103) + UIManager.GetItemProtetyStr((dataR - dataL), itemconfigR.m_AttrData.m_ADpType) + "[-]\n");
        }
        dataL = UIManager.GetItemPropertyData(itemconfigL.m_AttrData.GetAddMDp(1.0f, level, leftitemlevel), itemconfigR.m_AttrData.m_MDpType);
        dataR = UIManager.GetItemPropertyData(itemconfigR.m_AttrData.GetAddMDp(1.0f, level, leftitemlevel), itemconfigR.m_AttrData.m_MDpType);
        if (Mathf.Abs(dataR - dataL) > float.Epsilon) {
          str += (((dataR - dataL) > 0.0f ? "[00ffea]" : "[ff0000]") + DashFire.StrDictionaryProvider.Instance.GetDictString(104) + UIManager.GetItemProtetyStr((dataR - dataL), itemconfigR.m_AttrData.m_MDpType) + "[-]\n");
        }
        dataL = (aacL == null ? 0.0f : Mathf.FloorToInt(aacL.GetAddCri(1.0f, level) * 100) / 100.0f);
        dataR = (aacR == null ? 0.0f : Mathf.FloorToInt(aacR.GetAddCri(1.0f, level) * 100) / 100.0f);
        if (Mathf.Abs(dataR - dataL) > float.Epsilon) {
          str += (((dataR - dataL) > 0.0f ? "[00ffea]" : "[ff0000]") + DashFire.StrDictionaryProvider.Instance.GetDictString(105) + ((dataR - dataL) > 0.0f ? "+" : "") + Mathf.FloorToInt((dataR - dataL) * 100) + "%[-]\n");
        }
        dataL = (aacL == null ? 0.0f : Mathf.FloorToInt(aacL.GetAddPow(1.0f, level) * 100) / 100.0f);
        dataR = (aacR == null ? 0.0f : Mathf.FloorToInt(aacR.GetAddPow(1.0f, level) * 100) / 100.0f);
        if (Mathf.Abs(dataR - dataL) > float.Epsilon) {
          str += (((dataR - dataL) > 0.0f ? "[00ffea]" : "[ff0000]") + DashFire.StrDictionaryProvider.Instance.GetDictString(106) + ((dataR - dataL) > 0.0f ? "+" : "") + Mathf.FloorToInt((dataR - dataL) * 100) + "%[-]\n");
        }
        dataL = (aacL == null ? 0.0f : Mathf.FloorToInt(aacL.GetAddBackHitPow(1.0f, level) * 100) / 100.0f);
        dataR = (aacR == null ? 0.0f : Mathf.FloorToInt(aacR.GetAddBackHitPow(1.0f, level) * 100) / 100.0f);
        if (Mathf.Abs(dataR - dataL) > float.Epsilon) {
          str += (((dataR - dataL) > 0.0f ? "[00ffea]" : "[ff0000]") + DashFire.StrDictionaryProvider.Instance.GetDictString(107) + ((dataR - dataL) > 0.0f ? "+" : "") + Mathf.FloorToInt((dataR - dataL) * 100) + "%[-]\n");
        }
        dataL = (aacL == null ? 0.0f : Mathf.FloorToInt(aacL.GetAddCrackPow(1.0f, level) * 100) / 100.0f);
        dataR = (aacR == null ? 0.0f : Mathf.FloorToInt(aacR.GetAddCrackPow(1.0f, level) * 100) / 100.0f);
        if (Mathf.Abs(dataR - dataL) > float.Epsilon) {
          str += (((dataR - dataL) > 0.0f ? "[00ffea]" : "[ff0000]") + DashFire.StrDictionaryProvider.Instance.GetDictString(108) + ((dataR - dataL) > 0.0f ? "+" : "") + Mathf.FloorToInt((dataR - dataL) * 100) + "%[-]\n");
        }
        dataL = UIManager.GetItemPropertyData(itemconfigL.m_AttrData.GetAddFireDam(1.0f, level), itemconfigR.m_AttrData.m_FireDamType);
        dataR = UIManager.GetItemPropertyData(itemconfigR.m_AttrData.GetAddFireDam(1.0f, level), itemconfigR.m_AttrData.m_FireDamType);
        if (Mathf.Abs(dataR - dataL) > float.Epsilon) {
          str += (((dataR - dataL) > 0.0f ? "[00ffea]" : "[ff0000]") + DashFire.StrDictionaryProvider.Instance.GetDictString(109) + UIManager.GetItemProtetyStr((dataR - dataL), itemconfigR.m_AttrData.m_FireDamType) + "[-]\n");
        }
        dataL = UIManager.GetItemPropertyData(itemconfigL.m_AttrData.GetAddIceDam(1.0f, level), itemconfigR.m_AttrData.m_IceDamType);
        dataR = UIManager.GetItemPropertyData(itemconfigR.m_AttrData.GetAddIceDam(1.0f, level), itemconfigR.m_AttrData.m_IceDamType);
        if (Mathf.Abs(dataR - dataL) > float.Epsilon) {
          str += (((dataR - dataL) > 0.0f ? "[00ffea]" : "[ff0000]") + DashFire.StrDictionaryProvider.Instance.GetDictString(110) + UIManager.GetItemProtetyStr((dataR - dataL), itemconfigR.m_AttrData.m_IceDamType) + "[-]\n");
        }
        dataL = UIManager.GetItemPropertyData(itemconfigL.m_AttrData.GetAddPoisonDam(1.0f, level), itemconfigR.m_AttrData.m_PoisonDamType);
        dataR = UIManager.GetItemPropertyData(itemconfigR.m_AttrData.GetAddPoisonDam(1.0f, level), itemconfigR.m_AttrData.m_PoisonDamType);
        if (Mathf.Abs(dataR - dataL) > float.Epsilon) {
          str += (((dataR - dataL) > 0.0f ? "[00ffea]" : "[ff0000]") + DashFire.StrDictionaryProvider.Instance.GetDictString(111) + UIManager.GetItemProtetyStr((dataR - dataL), itemconfigR.m_AttrData.m_PoisonDamType) + "[-]\n");
        }
        dataL = UIManager.GetItemPropertyData(itemconfigL.m_AttrData.GetAddFireErd(1.0f, level), itemconfigR.m_AttrData.m_FireErdType);
        dataR = UIManager.GetItemPropertyData(itemconfigR.m_AttrData.GetAddFireErd(1.0f, level), itemconfigR.m_AttrData.m_FireErdType);
        if (Mathf.Abs(dataR - dataL) > float.Epsilon) {
          str += (((dataR - dataL) > 0.0f ? "[00ffea]" : "[ff0000]") + DashFire.StrDictionaryProvider.Instance.GetDictString(112) + UIManager.GetItemProtetyStr((dataR - dataL), itemconfigR.m_AttrData.m_FireErdType) + "[-]\n");
        }
        dataL = UIManager.GetItemPropertyData(itemconfigL.m_AttrData.GetAddIceErd(1.0f, level), itemconfigR.m_AttrData.m_IceErdType);
        dataR = UIManager.GetItemPropertyData(itemconfigR.m_AttrData.GetAddIceErd(1.0f, level), itemconfigR.m_AttrData.m_IceErdType);
        if (Mathf.Abs(dataR - dataL) > float.Epsilon) {
          str += (((dataR - dataL) > 0.0f ? "[00ffea]" : "[ff0000]") + DashFire.StrDictionaryProvider.Instance.GetDictString(113) + UIManager.GetItemProtetyStr((dataR - dataL), itemconfigR.m_AttrData.m_IceErdType) + "[-]\n");
        }
        dataL = UIManager.GetItemPropertyData(itemconfigL.m_AttrData.GetAddPoisonErd(1.0f, level), itemconfigR.m_AttrData.m_PoisonErdType);
        dataR = UIManager.GetItemPropertyData(itemconfigL.m_AttrData.GetAddPoisonErd(1.0f, level), itemconfigR.m_AttrData.m_PoisonErdType);
        if (Mathf.Abs(dataR - dataL) > float.Epsilon) {
          str += (((dataR - dataL) > 0.0f ? "[00ffea]" : "[ff0000]") + DashFire.StrDictionaryProvider.Instance.GetDictString(114) + UIManager.GetItemProtetyStr((dataR - dataL), itemconfigR.m_AttrData.m_PoisonErdType) + "[-]\n");
        }
      }
      if (str.Length != 0) {
        str = DashFire.StrDictionaryProvider.Instance.GetDictString(100) + "\n" + str;
      }
      str += itemconfigR.m_Description;
      Transform tfr = transform.Find("SpriteBackRight/Container/DragThing/Bula");
      if (tfr != null) {
        UILabel ul = tfr.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = str;
        }
      }
    }
    //     else {
    //       Transform tfr = transform.Find("SpriteBackRight/Container/DragThing/Bula");
    //       if (tfr != null) {
    //         UILabel ul = tfr.gameObject.GetComponent<UILabel>();
    //         if (ul != null) {
    //           ul.text = DashFire.StrDictionaryProvider.Instance.GetDictString(115);
    //         }
    //       }
    //     }
  }
  private bool isCompareUI = false;
  private int ID = 0;
  private int property = 0;
  private int position = 0;
  private int level = 0;
  private GameObject gamepokey = null;
  private Vector3 leftLocalPos = new Vector3();
  private Vector3 rightLocalPos = new Vector3();
}
