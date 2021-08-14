using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using DashFire;
using UnityEngine;
using UnityEditor;

public enum ResBuildLogType
{
  LT_Debug,
  LT_Info,
  LT_Warn,
  LT_Error,
  LT_Assert,
}
public delegate void LogSystemOutputDelegation(ResBuildLogType type, string msg);
public class ResBuildLog
{
  private static bool s_EnableConsoleLog = true;
  private static bool s_EnableFileLog = true;

  private static string s_LogFilePath = string.Empty;

  static ResBuildLog()
  {
    ResetLog();
  }
  public static void ResetLog()
  {
    s_EnableConsoleLog = true;
    s_EnableFileLog = false;

    s_LogFilePath = string.Empty;
  }
  public static bool SetFileLog(bool enable, string logfile)
  {
    bool ret = false;
    if (enable) {
      try {
        string curLogFile = ResBuildHelper.GetFilePathAbs(logfile);
        string dir = Path.GetDirectoryName(curLogFile);
        if (!Directory.Exists(dir)) {
          Directory.CreateDirectory(dir);
        }
        if (!ResBuildHelper.CheckFilePath(curLogFile)) {
          UnityEngine.Debug.Log("ResBuildLog file create failed logFile:" + curLogFile);
          return false;
        }
        s_LogFilePath = curLogFile;
        s_EnableFileLog = true;
        File.WriteAllText(s_LogFilePath, FomatLogFileHeaderInfo());
        ret = true;
      } catch (System.Exception ex) {
        UnityEngine.Debug.Log("ResBuildLog.RedirectLog failed ex:" + ex);
        return false;
      }
      return ret;
    } else {
      s_EnableFileLog = false;
      s_LogFilePath = string.Empty;
      ret = true;
    }
    return ret;
  }
  public static bool SetConsoleLog(bool enable)
  {
    s_EnableConsoleLog = enable;
    return true;
  }
  public static void Debug(string format, params object[] args)
  {
    string str = string.Format("[Debug]:" + format, args);
    Output(ResBuildLogType.LT_Debug, str);
  }
  public static void Info(string format, params object[] args)
  {
    string str = string.Format("[Info]:" + format, args);
    Output(ResBuildLogType.LT_Info, str);
  }
  public static void Warn(string format, params object[] args)
  {
    string str = string.Format("[Warn]:" + format, args);
    Output(ResBuildLogType.LT_Warn, str);
  }
  public static void Error(string format, params object[] args)
  {
    string str = string.Format("[Error]:" + format, args);
    Output(ResBuildLogType.LT_Error, str);
  }
  public static void Assert(bool check, string format, params object[] args)
  {
    if (!check) {
      string str = string.Format("[Assert]:" + format, args);
      Output(ResBuildLogType.LT_Assert, str);
    }
  }
  private static void Output(ResBuildLogType type, string msg)
  {
    if (s_EnableConsoleLog) {
      ConsoleLog(type, msg);
    }
    if (s_EnableFileLog) {
      FileLog(type, msg);
    }
  }
  private static void ConsoleLog(ResBuildLogType type, string msg)
  {
    UnityEngine.Debug.Log(msg);
  }
  private static void FileLog(ResBuildLogType type, string msg)
  {
    File.AppendAllText(s_LogFilePath, msg + "\n");
  }
  private static string FomatLogFileHeaderInfo()
  {
    return string.Format("Build Log Start Time:{0}\n{1}\n", DateTime.Now.ToString(), ResBuildConfig.DumpInfo());
  }
  private static string FomatLogFileTailInfo()
  {
    return string.Format("Build Log End Time:{0}\n", DateTime.Now.ToString());
  }
}
