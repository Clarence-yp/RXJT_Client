using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DashFire
{
  public class EquipmentInfo {
    public const int c_MaxEquipmentNum = 8;
    public ItemDataInfo[] Armor
    {
      get
      {
        return m_BodyArmor;
      }
    }
    public void Reset()
    {
      for (int ix = 0; ix < c_MaxEquipmentNum; ++ix) {
        m_BodyArmor[ix] = null;
      }
    }
    private ItemDataInfo[] m_BodyArmor = new ItemDataInfo[c_MaxEquipmentNum];
  }

  public class EquipmentStateInfo
  {
    public const int c_EquipmentCapacity = 8;
    public bool EquipmentChanged
    {
      get { return m_EquipmentChanged; }
      set { m_EquipmentChanged = value; }
    }
    public EquipmentInfo EquipmentInfo
    {
      get { return m_EquipmentInfo; }
    }
    public void SetEquipmentData(int index, ItemDataInfo info)
    {
      if (index >= 0 && index < c_EquipmentCapacity && info != null) {
        m_EquipmentInfo.Armor[index] = info;
        m_EquipmentChanged = true;
        if ((int)EquipmentType.E_Weapon == index && null != info.ItemConfig) {
          WeaponDamageType = info.ItemConfig.m_DamageType;
        }
      }
    }
    public ItemDataInfo GetEquipmentData(int index)
    {
      ItemDataInfo info = null;
      if (index >= 0 && index < c_EquipmentCapacity) {
        info = m_EquipmentInfo.Armor[index];
      }
      return info;
    }
    public void ResetEquipmentData(int index)
    {
      if (index >= 0 && index < c_EquipmentCapacity) {
        m_EquipmentInfo.Armor[index] = null;
        m_EquipmentChanged = true;
        if ((int)EquipmentType.E_Weapon == index) {
          WeaponDamageType = ElementDamageType.DC_None;
        }
      }
    }
    public void Reset()
    {
      m_EquipmentInfo.Reset();
      WeaponDamageType = ElementDamageType.DC_None;
    }
    public ElementDamageType WeaponDamageType
    {
      get { return m_DamageType; }
      set { m_DamageType = value; }
    }
    private ElementDamageType m_DamageType = ElementDamageType.DC_None;
    private bool m_EquipmentChanged = false;
    private EquipmentInfo m_EquipmentInfo = new EquipmentInfo();
  }
}
