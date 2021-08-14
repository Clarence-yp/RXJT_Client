namespace DashFire
{
  public class StrDictionary : IData
  {
    public int Id = 0;
    public string m_String = "";

    public bool CollectDataFromDBC(DBC_Row node)
    {
      Id = DBCUtil.ExtractNumeric<int>(node, "Id", 0, true);
      m_String = DBCUtil.ExtractString(node, "String", "", true);
      return true;
    }
    public int GetId()
    {
      return Id;
    }
  }
  public class StrDictionaryProvider
  {
    public string GetDictString(int id)
    {
      string ret;
      StrDictionary dict = GetDataById(id);
      if (null != dict && null != dict.m_String) {
        ret = dict.m_String;
      } else {
        ret = "";
      }
      return ret;
    }
    public string Format(int id, params object[] args)
    {
      string ret;
      StrDictionary dict = GetDataById(id);
      if (null != dict && null != dict.m_String) {
        ret = string.Format(dict.m_String, args);
      } else {
        ret = "";
      }
      return ret;
    }

    public DataDictionaryMgr<StrDictionary> StrDictionaryMgr
    {
      get { return m_StrDictionaryMgr; }
    }
    public StrDictionary GetDataById(int id)
    {
      return m_StrDictionaryMgr.GetDataById(id);
    }
    public int GetDataCount()
    {
      return m_StrDictionaryMgr.GetDataCount();
    }
    public void Load(string file, string root)
    {
      m_StrDictionaryMgr.CollectDataFromDBC(file, root);
    }

    private DataDictionaryMgr<StrDictionary> m_StrDictionaryMgr = new DataDictionaryMgr<StrDictionary>();

    public static StrDictionaryProvider Instance
    {
      get { return s_Instance; }
    }
    private static StrDictionaryProvider s_Instance = new StrDictionaryProvider();
  }
}
public static class Dict
{
  public static string Parse(string txtFromServer)
  {
    string[] txts = txtFromServer.Split('$');
    int ct = txts.Length;
    for (int i = 1; i < ct; i += 2) {
      if (txts[i].Length == 0) {
        txts[i] = "$";
      } else {
        int dictId = int.Parse(txts[i]);
        txts[i] = Get(dictId);
      }
    }
    return string.Join("", txts);
  }
  public static string Get(int id)
  {
    return DashFire.StrDictionaryProvider.Instance.GetDictString(id);
  }
  public static string Format(int id, params object[] args)
  {
    return DashFire.StrDictionaryProvider.Instance.Format(id, args);
  }
}
