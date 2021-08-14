using System;
using System.Collections.Generic;

namespace DashFire
{
  public class MailItem
  {
    public int m_ItemId;
    public int m_ItemNum;
  }
  public class MailInfo
  {
    public bool m_AlreadyRead;
    public ulong m_MailGuid;
    public string m_Title;
    public string m_Sender;
    public ModuleMailTypeEnum m_Module = ModuleMailTypeEnum.None;
    public DateTime m_SendTime;
    public string m_Text;
    public List<MailItem> m_Items;
    public int m_Money;
    public int m_Gold;
    public int m_Stamina;
  }
}