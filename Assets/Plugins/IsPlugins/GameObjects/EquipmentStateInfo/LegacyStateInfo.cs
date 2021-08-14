using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DashFire
{
  public class LegacyInfo 
  {
    public ItemDataInfo[] Legacy
    {
      get { return m_Legacy; }
    }
    public void Reset()
    {
      for (int ix = 0; ix < LegacyStateInfo.c_LegacyCapacity; ++ix) {
        m_Legacy[ix] = null;
      }
    }
    private ItemDataInfo[] m_Legacy = new ItemDataInfo[LegacyStateInfo.c_LegacyCapacity];
  }
  public class LegacyStateInfo
  {
    public const int c_LegacyCapacity = 4;
    public bool LegacyChanged
    {
      get { return m_LegacyChanged; }
      set { m_LegacyChanged = value; }
    }
    public LegacyInfo LegacyInfo
    {
      get { return m_LegacyInfo; }
    }
    public void SetLegacyData(int index, ItemDataInfo info)
    {
      if (index >= 0 && index < c_LegacyCapacity && info != null) {
        m_LegacyInfo.Legacy[index] = info;
        m_LegacyChanged = true;
      }
    }
    public ItemDataInfo GetLegacyData(int index)
    {
      ItemDataInfo info = null;
      if (index >= 0 && index < c_LegacyCapacity) {
        info = m_LegacyInfo.Legacy[index];
      }
      return info;
    }
    public void ResetLegacyData(int index)
    {
      if (index >= 0 && index < c_LegacyCapacity) {
        m_LegacyInfo.Legacy[index] = null;
        m_LegacyChanged = true;
      }
    }
    public void Reset()
    {
      m_LegacyInfo.Reset();
      m_LegacyChanged = true;
    }
    private bool m_LegacyChanged = false;
    private LegacyInfo m_LegacyInfo = new LegacyInfo();
  }
}
