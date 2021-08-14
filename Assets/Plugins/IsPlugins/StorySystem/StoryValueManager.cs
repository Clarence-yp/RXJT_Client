using System;
using System.Collections.Generic;
using ScriptRuntime;

namespace StorySystem
{
  public interface IStoryValueFactory
  {
    IStoryValue<object> Build(ScriptableData.ISyntaxComponent param);
  }
  public sealed class StoryValueFactoryHelper<C> : IStoryValueFactory where C : IStoryValue<object>, new()
  {
    public IStoryValue<object> Build(ScriptableData.ISyntaxComponent param)
    {
      C c = new C();
      c.InitFromDsl(param);
      return c;
    }
  }
  /// <summary>
  /// 这个类不加锁，约束条件：所有值注册必须在程序启动时完成。
  /// </summary>
  public class StoryValueManager
  {
    public void RegisterValueHandler(string name, IStoryValueFactory handler)
    {
      if (!m_ValueHandlers.ContainsKey(name)) {
        m_ValueHandlers.Add(name, handler);
      } else {
        //error
      }
    }
    public IStoryValue<object> CalcValue(ScriptableData.ISyntaxComponent param)
    {
      if (param.IsValid() && param.GetId().Length == 0) {
        //处理括弧
        ScriptableData.CallData callData = param as ScriptableData.CallData;
        if (null != callData && callData.GetParamNum() > 0) {
          int ct = callData.GetParamNum();
          return CalcValue(callData.GetParam(ct - 1));
        } else {
          //不支持的语法
          return null;
        }
      } else {
        IStoryValue<object> ret = null;
        string id = param.GetId();
        if (m_ValueHandlers.ContainsKey(id)) {
          ret = m_ValueHandlers[id].Build(param);
        }
        return ret;
      }
    }

    private StoryValueManager() { }

    private Dictionary<string, IStoryValueFactory> m_ValueHandlers = new Dictionary<string, IStoryValueFactory>();

    public static StoryValueManager Instance
    {
      get
      {
        return s_Instance;
      }
    }
    private static StoryValueManager s_Instance = new StoryValueManager();
  }
}
