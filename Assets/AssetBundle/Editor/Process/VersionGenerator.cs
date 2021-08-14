using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using DashFire;
using UnityEngine;
using UnityEditor;
using System.Security.Cryptography;

public class VersionGenerator
{
  public static bool GenVersionFile()
  {
    string clientfilePath = ResBuildHelper.GetFilePathAbs(ResBuildHelper.FormatVersionClientFilePath());
    string serverfilePath = ResBuildHelper.GetFilePathAbs(ResBuildHelper.FormatVersionServerFilePath());
    try {
      string dir = Path.GetDirectoryName(clientfilePath);
      if (!Directory.Exists(dir)) {
        Directory.CreateDirectory(dir);
      }
      if (File.Exists(clientfilePath)) {
        File.Delete(clientfilePath);
      }
      if (!File.Exists(clientfilePath)) {
        FileStream fs = File.Create(clientfilePath);
        fs.Close();
      }
      ConfigFile m_IniFile = new ConfigFile(clientfilePath);
      m_IniFile.AddSetting("AppSetting", "Version", ResBuildConfig.ClientVersion);
      m_IniFile.AddSetting("AppSetting", "Platform", ResBuildHelper.GetPlatformName(ResBuildConfig.BuildOptionTarget));
      m_IniFile.AddSetting("AppSetting", "AppName", ResBuildConfig.AppName);
      m_IniFile.AddSetting("AppSetting", "Channel", ResBuildConfig.Channel);
      m_IniFile.AddSetting("AppSetting", "ResServerURL", ResBuildConfig.ResServerURL);

      m_IniFile.AddSetting("Runtime", "CurChapter", ResBuildConfig.ResBuildInChapter.ToString());
      m_IniFile.AddSetting("Runtime", "IsResVersionConfigCached", false.ToString());
      m_IniFile.AddSetting("Runtime", "IsAssetDBConfigCached", false.ToString());
      m_IniFile.AddSetting("Runtime", "IsResCacheConfigCached", false.ToString());
      m_IniFile.AddSetting("Runtime", "IsResSheetConfigCached", false.ToString());
      m_IniFile.SaveSettings();

      dir = Path.GetDirectoryName(serverfilePath);
      if (!Directory.Exists(dir)) {
        Directory.CreateDirectory(dir);
      }
      if (File.Exists(serverfilePath)) {
        File.Delete(serverfilePath);
      }
      if (!File.Exists(serverfilePath)) {
        FileStream fs = File.Create(serverfilePath);
        fs.Close();
      }

      m_IniFile = new ConfigFile(serverfilePath);
      m_IniFile.AddSetting("AppSetting", "Version", ResBuildConfig.ServerVersion);
      m_IniFile.AddSetting("AppSetting", "Platform", ResBuildHelper.GetPlatformName(ResBuildConfig.BuildOptionTarget));
      m_IniFile.AddSetting("AppSetting", "AppName", ResBuildConfig.AppName);
      m_IniFile.AddSetting("AppSetting", "Channel", ResBuildConfig.Channel);
      m_IniFile.AddSetting("AppSetting", "ResServerURL", ResBuildConfig.ResServerURL);

      m_IniFile.AddSetting("Runtime", "CurChapter", ResBuildConfig.ResChapterRes.Count.ToString());
      m_IniFile.AddSetting("Runtime", "IsResVersionConfigCached", false.ToString());
      m_IniFile.AddSetting("Runtime", "IsAssetDBConfigCached", false.ToString());
      m_IniFile.AddSetting("Runtime", "IsResCacheConfigCached", false.ToString());
      m_IniFile.AddSetting("Runtime", "IsResSheetConfigCached", false.ToString());
      m_IniFile.SaveSettings();
    } catch (System.Exception ex) {
      ResBuildLog.Warn("VersionGenerator.GenVersionFile file failed!" + ex);
      return false;
    }
    AssetDatabase.Refresh();
    ResBuildLog.Info("VersionGenerator.GenVersionFile Success");
    return true;
  }
}
