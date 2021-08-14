using System;
using System.Collections.Generic;
using DashFire;

namespace DashFire
{
  public class Timeout<T>
  {
    public class TimeoutInfo<T>
    {
      public TimeoutInfo(T cb, long startTime, Action onTimeout)
      {
        m_Callback = cb;
        m_StartTime = startTime;
        m_OnTimeout = onTimeout;
      }
      
      public T Callback
      {
        get { return m_Callback; }
        set { m_Callback = value; }
      }
      public long StartTime
      {
        get { return m_StartTime; }
        set { m_StartTime = value; }
      }
      public Action OnTimeout
      {
        get { return m_OnTimeout; }
        set { m_OnTimeout = value; }
      }
      private T m_Callback = default(T);
      private long m_StartTime = 0;
      private Action m_OnTimeout = null;
    }

    public Timeout()
    {
      DefaultTimeoutMS = 30000;
    }

    public uint DefaultTimeoutMS { get; set; }

    public void Set(string timeoutKey, T cb, Action onTimeout)
    {
      long startTime = TimeUtility.GetServerMilliseconds();
      TimeoutInfo<T> info = new TimeoutInfo<T>(cb, startTime, onTimeout);
      lock (m_Lock) {
        if (m_TimeoutInfoDict.ContainsKey(timeoutKey)) {
          m_TimeoutInfoDict[timeoutKey] = info;
        } else {
          m_TimeoutInfoDict.Add(timeoutKey, info);
        }
      }
    }

    public T Get(string timeoutKey)
    {
      TimeoutInfo<T> info = null;
      lock (m_Lock) {
        m_TimeoutInfoDict.TryGetValue(timeoutKey, out info);
      }
      if (info != null) {
        return info.Callback;
      } else {
        return default(T);
      }
    }
    public bool Delete(string timeoutKey)
    {
      bool ret = false;
      lock (m_Lock) {
        ret = m_TimeoutInfoDict.Remove(timeoutKey);
      }
      return ret;
    }

    public void Tick()
    {
      lock (m_Lock) {
        long curTime = TimeUtility.GetServerMilliseconds();
        List<string> deleteKeys = new List<string>();
        foreach (var kv in m_TimeoutInfoDict) {
          if (kv.Value != null && curTime - kv.Value.StartTime > DefaultTimeoutMS) {
            if (kv.Value.OnTimeout != null) {
              kv.Value.OnTimeout();
            }
            deleteKeys.Add(kv.Key);
          }
        }
        foreach (string key in deleteKeys) {
          m_TimeoutInfoDict.Remove(key);
        }
      }
    }
    private object m_Lock = new object();
    private Dictionary<string, TimeoutInfo<T>> m_TimeoutInfoDict = new Dictionary<string, TimeoutInfo<T>>();
  }
}