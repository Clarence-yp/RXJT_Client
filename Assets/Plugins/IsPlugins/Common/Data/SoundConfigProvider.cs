using System;
using System.Collections.Generic;

namespace DashFire
{
  public class SoundConfig : IData
  {
    public int Id;                  
    public string Name;
    public bool IsMusic;
    public int Priority;
    public float Volume;
    public bool IsLoop;
    public ScriptRuntime.Vector3 Position;
    public string Description;    

    // 扩展数据项
    public int ParamNum = 0;
    public List<string> ExtraParams = new List<string>();
    
    public SoundConfig()
    {
      ExtraParams.Clear();
    }
    
    /**
     * @brief 提取数据
     *
     * @param node
     *
     * @return 
     */
    public bool CollectDataFromDBC(DBC_Row node)
    {
      Id = DBCUtil.ExtractNumeric<int>(node, "Id", -1, true);
      Name = DBCUtil.ExtractString(node, "Name", "", true);
      IsMusic = DBCUtil.ExtractBool(node, "IsMusic", false, true);
      Priority = DBCUtil.ExtractNumeric<int>(node, "Priority", 1, false);
      Volume = DBCUtil.ExtractNumeric<float>(node, "Volume", 1.0f, false);
      IsLoop = DBCUtil.ExtractBool(node, "IsLoop", false, false);
      Position = Converter.ConvertVector3D(DBCUtil.ExtractString(node, "Position", "0,0,0", false));

      ParamNum = DBCUtil.ExtractNumeric<int>(node, "ParamNum", 0, false);
      ExtraParams.Clear();
      if (ParamNum > 0) {
        for (int i = 0; i < ParamNum; ++i) {
          string key = "Param" + i.ToString();
          ExtraParams.Insert(i, DBCUtil.ExtractString(node, key, "", false)); 
        }
      }
      
      return true;
    }

    /**
     * @brief 获取数据ID
     *
     * @return 
     */
    public int GetId()
    {
      return Id;
    }
  }

  public class SoundConfigProvider
  {
    public DataDictionaryMgr<SoundConfig> SoundConfigMgr
    {
      get { return m_SoundConfigMgr; }
    }
    public SoundConfig GetDataById(int id)
    {
      return m_SoundConfigMgr.GetDataById(id);
    }
    public SoundConfig GetRandomDataByIdList(List<int> idList)
    {
      int index = Helper.Random.Next(idList.Count);
      return m_SoundConfigMgr.GetDataById(idList[index]);
    }
    public void Load(string file,string root)
    {
      m_SoundConfigMgr.CollectDataFromDBC(file, root);
    }

    private DataDictionaryMgr<SoundConfig> m_SoundConfigMgr = new DataDictionaryMgr<SoundConfig>();

    public static SoundConfigProvider Instance
    {
      get { return s_Instance; }
    }
    private static SoundConfigProvider s_Instance = new SoundConfigProvider();
  }

}
