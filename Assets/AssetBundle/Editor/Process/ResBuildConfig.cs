using System;
using System.Collections.Generic;
using System.Text;

using DashFire;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// !!!!!!!!
/// ResBuildConfig中的所有配置运行时都将从Assets/AssetBundle/Config.txt中读入，
/// 如需更改，运行插件AssetBundle/Windows/Config，并执行Apply按钮
/// </summary>
public class ResChapterInfo
{
  public int ResChapterIndex = 0;
  public string ResChapterIdContent = string.Empty;
  public HashSet<int> ResChapterIdList = new HashSet<int>();
}
public class ResBuildConfig
{
  public static string ResHomePath = "Assets/AssetBundle";
  public static string[] ConfigSplit = new string[] { ";", "|", };
  public static int VersionSlot = 3;
  public static string[] VersinNumSplitInterval = new string[] { "." };
  public static string ConfigFile = "Assets/AssetBundle/Config.txt";
  public static string LogTimeFormat = "yyyyMMddHHmmss";

  // GameConfig
  public static string ClientVersion = "1.0.1";
  public static string ServerVersion = "1.0.1";
  public static string Platform = "win32";
  public static string AppName = "DF";
  public static string Channel = "CYOU";
  public static string ResServerURL = "http://10.12.24.225:8081";

  // BuildConfig
  public static string ResBuildConfigOutputPath = "assets/AssetBundle/build";
  public static string ResBuildDirs = "Assets/Scenes;Assets/Resources";
  public static string ResBuildPattern = "*.unity;*.prefab;*.fbx;*.png;*.tga;*.mat;*.jpg;*.wav;*.mp3;*.shader";
  public static string ResBuildAssetPattern = ".unity;.prefab;.fbx;.png;.tga;.mat;.jpg;.wav;.mp3;.shader";
  public static string ResBuildPlayerLevel = "start.unity;loadingbar.unity";
  public static string ResBuildPlayerPath = "assets/../build/player";
  public static int ResBuildInChapter = 1;
  public static string ResBuildConfigFilePath = "ResBuildConfig.txt";
  public static string ResBuildConfigFormat = "{0}	{1}	{2}	{3}";
  public static string ResBuildConfigHeader = "Id	Resources	TargetName	Depends";
  //public static int ResCurChapter = 1;
  public static int ResChapterCount = 0;
  public static List<ResChapterInfo> ResChapterRes = new List<ResChapterInfo> { };

  // ResGraph
  public static string ResGraphNodeFormat = "{0}	{1}	{2}";
  public static string ResGraphNodeHeader = "ID	Depends	ChildDepends";
  public static string ResGraphNodeInfoFormat = "ID:{0} Depends:{1} ChildDepends:{2}";
  public static int ResGraphRootId = 0;

  // BuildOption
  public static BuildTarget BuildOptionTarget = BuildTarget.StandaloneWindows;
  public static BuildAssetBundleOptions BuildOptionRes =
                    BuildAssetBundleOptions.CollectDependencies |
                    BuildAssetBundleOptions.DeterministicAssetBundle |
                    BuildAssetBundleOptions.CompleteAssets;
  public static BuildOptions BuildOptionPlayer = BuildOptions.None;
  public static string BuildOptionExtend = ".ab";

  // ResVersion
  public static string ResVersionFilePath = "ResVersion.txt";
  public static string ResVersionZipPath = "ResVersion.ab";
  public static string ResVersionFormat = "{0}	{1}	{2}	{3}	{4}	{5}	{6}";
  public static string ResVersionHeader = "Id	AssetBundleName	AssetName	Depends	MD5	Chapter	Size";

  public static string ResVersionClientFilePath = "ClientResVersion.txt";
  public static string ResVersionClientFormat = "{0}	{1}	{2}";
  public static string ResVersionClientHeader = "Name	MD5	IsBuildIn";

  public static bool   ResVersionIncrementalEnable = true;
  public static string ResVersionIncrementalFilePath = "IncrementalResVersion.txt";
  public static string ResVersionIncrementalFormat = "{0}	{1}	{2}";
  public static string ResVersionIncrementalHeader = "Name	MD5	BuildTime";

  // ResSheet
  public static string ResSheetFilePath = "list.txt";
  public static string ResSheetZipPath = "ResSheet.ab";
  public static string ResSheetPattern = ".txt;.map;.dsl";
  public static string ResSheetCachePath = "DataFile/";

  // AssetDB
  public static string AssetDBFilePath = "AssetDB.txt";
  public static string AssetDBZipPath = "AssetDB.ab";
  public static string AssetDBFormat = "{0}	{1}	{2}	{3}";
  public static string AssetDBHeader = "Id	Name	Type	ResId";
  public static string AssetDBPattern = "assets/resources/*;assets/scenes/*unity$";

  // ResCache
  public static string ResCacheFilePath = "ResCache.txt";
  public static string ResCacheZipPath = "ResCache.ab";
  public static string ResCacheFormat = "{0}	{1}	{2}	{3}	{4}	{5}	{6}";
  public static string ResCacheHeader = "Id	Chapter	CacheType	ResId	Assets	AssetNames	Links";
  public static string ResCacheScenesPattern = ".unity";
  public static string ResCacheResourcesPattern = ".prefab;.png;.tga;.jpg;.shader;.wav;.mp3";
  public static string ResCacheResExclude = "assets/resources/loading/loading2.prefab;";

  // Version
  public static string VersionClientFile = "ClientVersion.txt";
  public static string VersionServerFile = "ServerVersion.txt";

  // ResLog
  public static string ResLogFilePath = "assets/../build/log/build_{0}_{1}_{2}.log";

  // ResCommit
  public static string ResCommitSearchPattern = "*.ab;*.txt";
  public static string ResCommitBuildInPath = "Assets/StreamingAssets";
  public static string ResCommitCachePath = "ResFile/";

  // Player
  public static string PlayerBundleIdentifier = "com.cyou.df";

  // Extend
  public static string ResFinderShaderFilePath = "ResFinderShader.txt";
  public static string ResFinderTypeFilePath = "ResFinderType.txt";
  public static string ResFinderTypeTargetDir = "assets/";
  public static string ResFinderTypeExcludePattern = ".meta;.svn";
  public static string ResFinderTypeIncludePattern = "*";
  public static bool SetBuildTargetPlatform(BuildTarget target)
  {
    BuildOptionTarget = target;
    return true;
  }
  public static bool SetBuildTargetDir(string dir)
  {
    ResBuildDirs = dir;
    return true;
  }
  public static bool Load()
  {
    bool ret = false;
    try {
      ConfigFile iniFile = new ConfigFile(ResBuildHelper.GetFilePathAbs(ConfigFile));

      //GameConfig
      ClientVersion = iniFile.GetSetting("GameConfig", "ClientVersion");
      ServerVersion = iniFile.GetSetting("GameConfig", "ServerVersion");
      AppName = iniFile.GetSetting("GameConfig", "AppName");
      Channel = iniFile.GetSetting("GameConfig", "Channel");
      ResServerURL = iniFile.GetSetting("GameConfig", "ResServerURL");

      //BuildConfig
      ResBuildConfigOutputPath = iniFile.GetSetting("BuildConfig", "ResBuildConfigOutputPath");
      ResBuildDirs = iniFile.GetSetting("BuildConfig", "ResBuildDirs");
      ResBuildPattern = iniFile.GetSetting("BuildConfig", "ResBuildPattern");
      ResBuildAssetPattern = iniFile.GetSetting("BuildConfig", "ResBuildAssetPattern");
      ResBuildPlayerLevel = iniFile.GetSetting("BuildConfig", "ResBuildPlayerLevel");
      ResBuildPlayerPath = iniFile.GetSetting("BuildConfig", "ResBuildPlayerPath");
      ResBuildInChapter = Convert.ToInt32(iniFile.GetSetting("BuildConfig", "ResBuildInChapter"));
      ResBuildConfigFilePath = iniFile.GetSetting("BuildConfig", "ResBuildConfigFilePath");
      ResBuildConfigFormat = iniFile.GetSetting("BuildConfig", "ResBuildConfigFormat");
      ResBuildConfigHeader = iniFile.GetSetting("BuildConfig", "ResBuildConfigHeader");
      //ResCurChapter = Convert.ToInt32(iniFile.GetSetting("BuildConfig", "ResCurChapter"));

      string strChapterCount = iniFile.GetSetting("BuildConfig", "ResChapterCount").Trim();
      if (!int.TryParse(strChapterCount, out ResChapterCount)) {
        ResChapterCount = 0;
      }
      ResChapterRes.Clear();
      for (int index = 1; index <= ResChapterCount; index++) {
        string chapterRes = iniFile.GetSetting("BuildConfig", "ResChapterRes" + index);
        ResChapterInfo info = new ResChapterInfo();
        info.ResChapterIndex = index;
        info.ResChapterIdContent = chapterRes;
        ResChapterRes.Add(info);

        string[] tResChapterPattern = chapterRes.Split(ConfigSplit, StringSplitOptions.RemoveEmptyEntries);
        if (tResChapterPattern != null) {
          foreach (string resId in tResChapterPattern) {
            info.ResChapterIdList.Add(Convert.ToInt32(resId));
          }
        }
      }

      // ResGraph
      ResGraphNodeFormat = iniFile.GetSetting("ResGraph", "ResGraphNodeFormat");
      ResGraphNodeHeader = iniFile.GetSetting("ResGraph", "ResGraphNodeHeader");
      ResGraphNodeInfoFormat = iniFile.GetSetting("ResGraph", "ResGraphNodeInfoFormat");
      //ResGraphRootId = iniFile.GetSetting("ResGraph", "ResGraphRootId");

      // BuildOption
      BuildOptionRes = ConvertStrToBuildOptionRes(iniFile.GetSetting("BuildOption", "BuildOptionRes"));
      BuildOptionPlayer = ConvertStrToBuildOptionPlayer(iniFile.GetSetting("BuildOption", "BuildOptionPlayer"));
      BuildOptionTarget = ConvertStrToBuildTarget(iniFile.GetSetting("BuildOption", "BuildOptionTarget"));
      BuildOptionExtend = iniFile.GetSetting("BuildOption", "BuildOptionExtend");

      // ResVersion
      ResVersionFilePath = iniFile.GetSetting("ResVersion", "ResVersionFilePath");
      ResVersionZipPath = iniFile.GetSetting("ResVersion", "ResVersionZipPath");
      ResVersionFormat = iniFile.GetSetting("ResVersion", "ResVersionFormat");
      ResVersionHeader = iniFile.GetSetting("ResVersion", "ResVersionHeader");

      ResVersionClientFilePath = iniFile.GetSetting("ResVersion", "ResVersionClientFilePath");
      ResVersionClientFormat = iniFile.GetSetting("ResVersion", "ResVersionClientFormat");
      ResVersionClientHeader = iniFile.GetSetting("ResVersion", "ResVersionClientHeader");

      ResVersionIncrementalEnable = Convert.ToBoolean(iniFile.GetSetting("ResVersion", "ResVersionIncrementalEnable"));
      ResVersionIncrementalFilePath = iniFile.GetSetting("ResVersion", "ResVersionIncrementalFilePath");
      ResVersionIncrementalFormat = iniFile.GetSetting("ResVersion", "ResVersionIncrementalFormat");
      ResVersionIncrementalHeader = iniFile.GetSetting("ResVersion", "ResVersionIncrementalHeader");

      // AssetDB
      AssetDBFilePath = iniFile.GetSetting("AssetDB", "AssetDBFilePath");
      AssetDBZipPath = iniFile.GetSetting("AssetDB", "AssetDBZipPath");
      AssetDBFormat = iniFile.GetSetting("AssetDB", "AssetDBFormat");
      AssetDBHeader = iniFile.GetSetting("AssetDB", "AssetDBHeader");
      AssetDBPattern = iniFile.GetSetting("AssetDB", "AssetDBPattern");

      // ResCache
      ResCacheFilePath = iniFile.GetSetting("ResCache", "ResCacheFilePath");
      ResCacheZipPath = iniFile.GetSetting("ResCache", "ResCacheZipPath");
      ResCacheHeader = iniFile.GetSetting("ResCache", "ResCacheHeader");
      ResCacheFormat = iniFile.GetSetting("ResCache", "ResCacheFormat");
      ResCacheScenesPattern = iniFile.GetSetting("ResCache", "ResCacheScenesPattern");
      ResCacheResourcesPattern = iniFile.GetSetting("ResCache", "ResCacheResourcesPattern");
      ResCacheResExclude = iniFile.GetSetting("ResCache", "ResCacheResExclude");
      
      // Version
      VersionClientFile = iniFile.GetSetting("Version", "VersionClientFile");
      VersionServerFile = iniFile.GetSetting("Version", "VersionServerFile");

      // ResLog
      ResLogFilePath = iniFile.GetSetting("ResLog", "ResLogFilePath");

      // ResCommit
      ResCommitSearchPattern = iniFile.GetSetting("ResCommit", "ResCommitSearchPattern");
      ResCommitBuildInPath = iniFile.GetSetting("ResCommit", "ResCommitBuildInPath");
      ResCommitCachePath = iniFile.GetSetting("ResCommit", "ResCommitCachePath");

      // Player
      PlayerBundleIdentifier = iniFile.GetSetting("Player", "PlayerBundleIdentifier");

      // Extend
      ResFinderShaderFilePath = iniFile.GetSetting("Extend", "ResFinderShaderFilePath");
      ResFinderTypeFilePath = iniFile.GetSetting("Extend", "ResFinderTypeFilePath");
      ResFinderTypeTargetDir = iniFile.GetSetting("Extend", "ResFinderTypeTargetDir");
      ResFinderTypeExcludePattern = iniFile.GetSetting("Extend", "ResFinderTypeExcludePattern");
      ResFinderTypeIncludePattern = iniFile.GetSetting("Extend", "ResFinderTypeIncludePattern");

      ret = true;
    } catch (System.Exception ex) {
      ResBuildLog.Warn("ResBuildConfig.Load failed, ex:" + ex);
      ret = false;
      return ret;
    }
    ResBuildLog.Info("ResBuildConfig.Load Success");
    return ret;
  }
  public static bool Save()
  {
    if (!ResBuildConfig.Check()) {
      ResBuildLog.Warn("ResBuildConfig.Check Failed");
      return false;
    }

    try {
      ConfigFile iniFile = new ConfigFile(ResBuildHelper.GetFilePathAbs(ConfigFile));

      //GameConfig
      iniFile.AddSetting("GameConfig", "ClientVersion", ClientVersion);
      iniFile.AddSetting("GameConfig", "ServerVersion", ServerVersion);
      iniFile.AddSetting("GameConfig", "AppName", AppName);
      iniFile.AddSetting("GameConfig", "Channel", Channel);
      iniFile.AddSetting("GameConfig", "ResServerURL", ResServerURL);

      //BuildConfig
      iniFile.AddSetting("BuildConfig", "ResBuildConfigOutputPath", ResBuildConfigOutputPath);
      iniFile.AddSetting("BuildConfig", "ResBuildDirs", ResBuildDirs);
      iniFile.AddSetting("BuildConfig", "ResBuildPattern", ResBuildPattern);
      iniFile.AddSetting("BuildConfig", "ResBuildAssetPattern", ResBuildAssetPattern);
      iniFile.AddSetting("BuildConfig", "ResBuildPlayerLevel", ResBuildPlayerLevel);
      iniFile.AddSetting("BuildConfig", "ResBuildPlayerPath", ResBuildPlayerPath);
      iniFile.AddSetting("BuildConfig", "ResBuildInChapter", ResBuildInChapter.ToString());
      iniFile.AddSetting("BuildConfig", "ResBuildConfigFilePath", ResBuildConfigFilePath);
      iniFile.AddSetting("BuildConfig", "ResBuildConfigFormat", ResBuildConfigFormat);
      iniFile.AddSetting("BuildConfig", "ResBuildConfigHeader", ResBuildConfigHeader);
      //iniFile.AddSetting("BuildConfig", "ResCurChapter", ResCurChapter.ToString());

      iniFile.AddSetting("BuildConfig", "ResChapterCount", ResChapterCount.ToString());
      for (int index = 0; index < ResChapterCount; index++) {
        ResChapterInfo info = ResChapterRes[index];
        iniFile.AddSetting("BuildConfig", "ResChapterRes" + (info.ResChapterIndex), info.ResChapterIdContent);
      }

      // ResGraph
      iniFile.AddSetting("ResGraph", "ResGraphNodeFormat", ResGraphNodeFormat);
      iniFile.AddSetting("ResGraph", "ResGraphNodeHeader", ResGraphNodeHeader);
      iniFile.AddSetting("ResGraph", "ResGraphNodeInfoFormat", ResGraphNodeInfoFormat);
      //iniFile.AddSetting("ResGraph", "ResGraphRootId", ResGraphRootId);

      // BuildOption
      iniFile.AddSetting("BuildOption", "BuildOptionRes", ConvertBuildOptionResToStr(BuildOptionRes));
      iniFile.AddSetting("BuildOption", "BuildOptionPlayer", ConvertBuildOptionPlayerToStr(BuildOptionPlayer));
      iniFile.AddSetting("BuildOption", "BuildOptionTarget", ConvertBuildTargetToStr(BuildOptionTarget));
      iniFile.AddSetting("BuildOption", "BuildOptionExtend", BuildOptionExtend);

      // ResVersion
      iniFile.AddSetting("ResVersion", "ResVersionFilePath", ResVersionFilePath);
      iniFile.AddSetting("ResVersion", "ResVersionZipPath", ResVersionZipPath);
      iniFile.AddSetting("ResVersion", "ResVersionFormat", ResVersionFormat);
      iniFile.AddSetting("ResVersion", "ResVersionHeader", ResVersionHeader);

      iniFile.AddSetting("ResVersion", "ResVersionClientFilePath", ResVersionClientFilePath);
      iniFile.AddSetting("ResVersion", "ResVersionClientFormat", ResVersionClientFormat);
      iniFile.AddSetting("ResVersion", "ResVersionClientHeader", ResVersionClientHeader);

      iniFile.AddSetting("ResVersion", "ResVersionIncrementalEnable", ResVersionIncrementalEnable.ToString());
      iniFile.AddSetting("ResVersion", "ResVersionIncrementalFilePath", ResVersionIncrementalFilePath);
      iniFile.AddSetting("ResVersion", "ResVersionIncrementalFormat", ResVersionIncrementalFormat);
      iniFile.AddSetting("ResVersion", "ResVersionIncrementalHeader", ResVersionIncrementalHeader);

      // AssetDB
      iniFile.AddSetting("AssetDB", "AssetDBFilePath", AssetDBFilePath);
      iniFile.AddSetting("AssetDB", "AssetDBZipPath", AssetDBZipPath);
      iniFile.AddSetting("AssetDB", "AssetDBFormat", AssetDBFormat);
      iniFile.AddSetting("AssetDB", "AssetDBHeader", AssetDBHeader);
      iniFile.AddSetting("AssetDB", "AssetDBPattern", AssetDBPattern);

      // ResCache
      iniFile.AddSetting("ResCache", "ResCacheFilePath", ResCacheFilePath);
      iniFile.AddSetting("ResCache", "ResCacheZipPath", ResCacheZipPath);
      iniFile.AddSetting("ResCache", "ResCacheHeader", ResCacheHeader);
      iniFile.AddSetting("ResCache", "ResCacheFormat", ResCacheFormat);
      iniFile.AddSetting("ResCache", "ResCacheScenesPattern", ResCacheScenesPattern);
      iniFile.AddSetting("ResCache", "ResCacheResourcesPattern", ResCacheResourcesPattern);
      iniFile.AddSetting("ResCache", "ResCacheResExclude", ResCacheResExclude);

      // Version
      iniFile.AddSetting("Version", "VersionClientFile", VersionClientFile);
      iniFile.AddSetting("Version", "VersionServerFile", VersionServerFile);

      // ResLog
      iniFile.AddSetting("ResLog", "ResLogFilePath", ResLogFilePath);

      // ResCommit
      iniFile.AddSetting("ResCommit", "ResCommitSearchPattern", ResCommitSearchPattern);
      iniFile.AddSetting("ResCommit", "ResCommitBuildInPath", ResCommitBuildInPath);
      iniFile.AddSetting("ResCommit", "ResCommitCachePath", ResCommitCachePath);

      // Player
      iniFile.AddSetting("Player", "PlayerBundleIdentifier", PlayerBundleIdentifier);

      // Extend
      iniFile.AddSetting("Extend", "ResFinderShaderFilePath", ResFinderShaderFilePath);
      iniFile.AddSetting("Extend", "ResFinderTypeFilePath", ResFinderTypeFilePath);
      iniFile.AddSetting("Extend", "ResFinderTypeTargetDir", ResFinderTypeTargetDir);
      iniFile.AddSetting("Extend", "ResFinderTypeExcludePattern", ResFinderTypeExcludePattern);
      iniFile.AddSetting("Extend", "ResFinderTypeIncludePattern", ResFinderTypeIncludePattern);

      iniFile.SaveSettings();
    } catch (System.Exception ex) {
      ResBuildLog.Warn("ResBuildConfig.Save failed, ex:" + ex);
      return false;
    }
    ResBuildLog.Info("ResBuildConfig.Save Success");
    return true;
  }
  public static bool Check()
  {
    //ClientVersion
    string[] splitRet = ClientVersion.Split(VersinNumSplitInterval, StringSplitOptions.RemoveEmptyEntries);
    if (splitRet == null || splitRet.Length != VersionSlot) {
      ResBuildLog.Warn("CheckConfig: ClientVersion Format Error");
      return false;
    }
    //ServerVersion
    splitRet = ServerVersion.Split(VersinNumSplitInterval, StringSplitOptions.RemoveEmptyEntries);
    if (splitRet == null || splitRet.Length != VersionSlot) {
      ResBuildLog.Warn("CheckConfig: ServerVersion Format Error");
      return false;
    }
    if (ClientVersion.CompareTo(ServerVersion) > 0) {
      ResBuildLog.Warn("CheckConfig: ClientVersion Greater than ServerVersion");
      return false;
    }
    //Platform
    string platform = ResBuildHelper.GetPlatformName(BuildOptionTarget);
    if (string.IsNullOrEmpty(platform)
      || platform.Equals("invalid")) {
      ResBuildLog.Warn("CheckConfig: Platform bindting to BuildOptionTarget,Current Only Suuport win32/mac/android/ios");
      return false;
    }
    string channel = ResBuildConfig.Channel;
    if (string.IsNullOrEmpty(channel)) {
      ResBuildLog.Warn("CheckConfig: Channel miss, default:dev");
      return false;
    }

    //ResServerURL
    if (string.IsNullOrEmpty(ResServerURL)) {
      ResBuildLog.Warn("CheckConfig: ResServerURL Invalid");
      return false;
    }

    //ResBuildDirs
    string[] tResBuildDirs = ResBuildConfig.ResBuildDirs.Split(ResBuildConfig.ConfigSplit,
      StringSplitOptions.RemoveEmptyEntries);
    if (tResBuildDirs == null
      || tResBuildDirs.Length == 0) {
      ResBuildLog.Warn("CheckConfig: ResBuildDirs Invalid!");
      return false;
    } else {
      foreach (String rbd in tResBuildDirs) {
        if (!ResBuildHelper.IsPathValid(rbd)) {
          ResBuildLog.Warn("CheckConfig: ResBuildDirs invalid or not exist,res:" + rbd);
          return false;
        }
      }
    }

    //ResBuildPattern
    string[] tResBuildPattern = ResBuildConfig.ResBuildPattern.Split(ResBuildConfig.ConfigSplit,
      StringSplitOptions.RemoveEmptyEntries);
    if (tResBuildPattern == null
      || tResBuildPattern.Length == 0) {
      ResBuildLog.Warn("CheckConfig: ResBuildPattern Invalid!");
      return false;
    } else {
      foreach (String rbd in tResBuildPattern) {
        if (!rbd.ToLower().StartsWith("*.")) {
          ResBuildLog.Warn("CheckConfig: ResBuildPattern MUST startwith '*.',eg.'*.unity;*.prefab'");
          return false;
        }
      }
    }

    //ResBuildAssetPattern
    string[] tResBuildAssetPattern = ResBuildConfig.ResBuildAssetPattern.Split(ResBuildConfig.ConfigSplit,
      StringSplitOptions.RemoveEmptyEntries);
    if (tResBuildAssetPattern == null
      || tResBuildAssetPattern.Length == 0) {
      ResBuildLog.Warn("CheckConfig: ResBuildAssetPattern Invalid!");
      return false;
    } else {
      foreach (String rbd in tResBuildAssetPattern) {
        if (!rbd.ToLower().StartsWith(".")) {
          ResBuildLog.Warn("CheckConfig: ResBuildAssetPattern MUST startwith '.',eg.'.unity;.prefab'");
          return false;
        }
      }
    }

    //ResBuildPlayerLevel
    string[] tResBuildPlayerLevel = ResBuildConfig.ResBuildPlayerLevel.Split(ResBuildConfig.ConfigSplit,
      StringSplitOptions.RemoveEmptyEntries);
    if (tResBuildPlayerLevel == null
      || tResBuildPlayerLevel.Length == 0) {
      ResBuildLog.Warn("CheckConfig: ResBuildPlayerLevel Invalid!");
      return false;
    } else {
      foreach (String rbd in tResBuildPlayerLevel) {
        if (!rbd.ToLower().EndsWith(".unity")) {
          ResBuildLog.Warn("CheckConfig: ResBuildPlayerLevel MUST be level,eg,'start.unity;loadingbar.unity'");
          return false;
        }
      }
    }

    ResBuildLog.Info("ResBuildConfig.Check Success");
    return true;
  }
  private static bool LoadSceneConfigData(ref Dictionary<int, Data_SceneConfig> ConfigDict)
  {
    string sceneConfigFilePath = Path.Combine(Application.streamingAssetsPath, FilePathDefine_Client.C_SceneConfig);
    bool result = true;
    MemoryStream ms = null;
    StreamReader sr = null;
    try {
      DBC document = new DBC();
      byte[] buffer = File.ReadAllBytes(sceneConfigFilePath);
      ms = new MemoryStream(buffer);
      ms.Seek(0, SeekOrigin.Begin);
      System.Text.Encoding encoding = System.Text.Encoding.UTF8;
      sr = new StreamReader(ms, encoding);
      document.LoadFromStream(sr);
      for (int index = 0; index < document.RowNum; index++) {
        DBC_Row node = document.GetRowByIndex(index);
        if (node != null) {
          Data_SceneConfig data = new Data_SceneConfig();
          bool ret = data.CollectDataFromDBC(node);
          string info = string.Format("LoadSceneConfigData collectData Row:{0} failed!", index);
          LogSystem.Assert(ret, info);
          if (ret) {
            ConfigDict.Add(data.GetId(), data);
          } else {
            result = false;
          }
        }
      }
    } catch (System.Exception ex) {
      string info = string.Format("LoadSceneConfigData exception ex:", ex);
      LogSystem.Assert(false, info);
    } finally {
      if (ms != null) {
        ms.Close();
      }
      if (sr != null) {
        sr.Close();
      }
    }
    return result;
  }
  public static BuildAssetBundleOptions ConvertStrToBuildOptionRes(string content)
  {
    BuildAssetBundleOptions ret = 0;
    string[] tBuildOptions = content.Split(ConfigSplit, StringSplitOptions.RemoveEmptyEntries);
    foreach (string tOption in tBuildOptions) {
      try {
        ret |= (BuildAssetBundleOptions)Enum.Parse(typeof(BuildAssetBundleOptions), tOption);
      } catch (Exception ex) {
        ResBuildLog.Warn("ConvertBuildAssetBundleOptions error:" + content + " ex:" + ex);
      }
    }
    return ret;
  }
  public static string ConvertBuildOptionResToStr(BuildAssetBundleOptions content)
  {
    string ret = string.Empty;
    Array list = Enum.GetValues(typeof(BuildAssetBundleOptions));
    foreach (int val in list) {
      if (((int)content & (int)val) != 0) {
        ret += Enum.GetName(typeof(BuildAssetBundleOptions), val) + ";";
      }
    }
    return ret;
  }
  public static BuildOptions ConvertStrToBuildOptionPlayer(string content)
  {
    BuildOptions ret = 0;
    string[] tBuildOptions = content.Split(ConfigSplit, StringSplitOptions.RemoveEmptyEntries);
    foreach (string tOption in tBuildOptions) {
      try {
        ret |= (BuildOptions)Enum.Parse(typeof(BuildOptions), tOption);
      } catch (Exception ex) {
        ResBuildLog.Warn("ConvertStrToBuildOptionPlayer error:" + content + " ex:" + ex);
      }
    }
    return ret;
  }
  public static string ConvertBuildOptionPlayerToStr(BuildOptions content)
  {
    string ret = string.Empty;
    Array list = Enum.GetValues(typeof(BuildOptions));
    foreach (int val in list) {
      if (((int)content & (int)val) != 0) {
        ret += Enum.GetName(typeof(BuildOptions), val) + ";";
      }
    }
    if (content == BuildOptions.None) {
      ret = "None";
    }
    return ret;
  }
  public static BuildTarget ConvertStrToBuildTarget(string content)
  {
    BuildTarget ret = BuildTarget.StandaloneWindows;
    try {
      ret = (BuildTarget)Enum.Parse(typeof(BuildTarget), content);
    } catch (Exception ex) {
      ResBuildLog.Warn("ConvertStrToBuildOption error:" + content + " ex:" + ex);
    }
    return ret;
  }
  public static string ConvertBuildTargetToStr(BuildTarget content)
  {
    return Enum.GetName(typeof(BuildTarget), content);
  }
  public static string DumpInfo()
  {
    StringBuilder sb = new StringBuilder();
    sb.AppendLine("ResBuildConfig Info:");
    //GameConfig
    sb.AppendLine("\tGameConfig");
    sb.AppendLine("\t\tClientVersion=" + ClientVersion);
    sb.AppendLine("\t\tServerVersion=" + ServerVersion);
    sb.AppendLine("\t\tAppName=" + AppName);
    sb.AppendLine("\t\tChannel=" + Channel);
    sb.AppendLine("\t\tResServerURL=" + ResServerURL);

    //BuildConfig
    sb.AppendLine("\tBuildConfig");
    sb.AppendLine("\t\tResBuildConfigOutputPath=" + ResBuildConfigOutputPath);
    sb.AppendLine("\t\tResBuildDirs=" + ResBuildDirs);
    sb.AppendLine("\t\tResBuildPattern=" + ResBuildPattern);
    sb.AppendLine("\t\tResBuildAssetPattern=" + ResBuildAssetPattern);
    sb.AppendLine("\t\tResBuildPlayerLevel=" + ResBuildPlayerLevel);
    sb.AppendLine("\t\tResBuildPlayerPath=" + ResBuildPlayerPath);
    sb.AppendLine("\t\tResBuildInChapter=" + ResBuildInChapter.ToString());
    sb.AppendLine("\t\tResBuildConfigFilePath=" + ResBuildConfigFilePath);
    sb.AppendLine("\t\tResBuildConfigFormat=" + ResBuildConfigFormat);
    sb.AppendLine("\t\tResBuildConfigHeader=" + ResBuildConfigHeader);
    //sb.AppendLine("\t\tResCurChapter=" + ResCurChapter.ToString());

    // BuildOption
    sb.AppendLine("\tBuildOption");
    sb.AppendLine("\t\tBuildOptionRes=" + ConvertBuildOptionResToStr(BuildOptionRes));
    sb.AppendLine("\t\tBuildOptionPlayer=" + ConvertBuildOptionPlayerToStr(BuildOptionPlayer));
    sb.AppendLine("\t\tBuildOptionTarget=" + ConvertBuildTargetToStr(BuildOptionTarget));
    sb.AppendLine("\t\tBuildOptionExtend=" + BuildOptionExtend);

    // ResVersion
    sb.AppendLine("\tResVersion");
    sb.AppendLine("\t\tResVersionFilePath=" + ResVersionFilePath);
    sb.AppendLine("\t\tResVersionZipPath=" + ResVersionZipPath);
    sb.AppendLine("\t\tResVersionFormat=" + ResVersionFormat);
    sb.AppendLine("\t\tResVersionHeader=" + ResVersionHeader);

    sb.AppendLine("\t\tResVersionClientFilePath=" + ResVersionClientFilePath);
    sb.AppendLine("\t\tResVersionClientFormat=" + ResVersionClientFormat);
    sb.AppendLine("\t\tResVersionClientHeader=" + ResVersionClientHeader);

    sb.AppendLine("\t\tResVersionIncrementalEnable=" + ResVersionIncrementalEnable.ToString());
    sb.AppendLine("\t\tResVersionIncrementalFilePath=" + ResVersionIncrementalFilePath);
    sb.AppendLine("\t\tResVersionIncrementalFormat=" + ResVersionIncrementalFormat);
    sb.AppendLine("\t\tResVersionIncrementalHeader=" + ResVersionIncrementalHeader);

    // AssetDB
    sb.AppendLine("\tAssetDB");
    sb.AppendLine("\t\tAssetDBFilePath=" + AssetDBFilePath);
    sb.AppendLine("\t\tAssetDBZipPath=" + AssetDBZipPath);
    sb.AppendLine("\t\tAssetDBFormat=" + AssetDBFormat);
    sb.AppendLine("\t\tAssetDBHeader=" + AssetDBHeader);
    sb.AppendLine("\t\tAssetDBPattern=" + AssetDBPattern);

    // ResCache
    sb.AppendLine("\tResCache");
    sb.AppendLine("\t\tResCacheFilePath=" + ResCacheFilePath);
    sb.AppendLine("\t\tResCacheZipPath=" + ResCacheZipPath);
    sb.AppendLine("\t\tResCacheFormat=" + ResCacheFormat);
    sb.AppendLine("\t\tResCacheHeader=" + ResCacheHeader);

    // Version
    sb.AppendLine("\tVersion");
    sb.AppendLine("\t\tVersionClientFile=" + VersionClientFile);
    sb.AppendLine("\t\tVersionServerFile=" + VersionServerFile);

    // ResLog
    sb.AppendLine("\tResLog");
    sb.AppendLine("\t\tResLogFilePath=" + ResLogFilePath);

    // ResCommit
    sb.AppendLine("\tResCommit");
    sb.AppendLine("\t\tResCommitSearchPattern=" + ResCommitSearchPattern);
    sb.AppendLine("\t\tResCommitBuildInPath=" + ResCommitBuildInPath);
    sb.AppendLine("\t\tResCommitCachePath=" + ResCommitCachePath);

    // Player
    sb.AppendLine("\tPlayer");
    sb.AppendLine("\t\tPlayerBundleIdentifier=" + PlayerBundleIdentifier);

    // Extend
    sb.AppendLine("\tExtend");
    sb.AppendLine("\t\tResFinderShaderFilePath=" + ResFinderShaderFilePath);
    sb.AppendLine("\t\tResFinderTypeFilePath=" + ResFinderTypeFilePath);
    sb.AppendLine("\t\tResFinderTypeTargetDir=" + ResFinderTypeTargetDir);
    sb.AppendLine("\t\tResFinderTypeExcludePattern=" + ResFinderTypeExcludePattern);
    sb.AppendLine("\t\tResFinderTypeIncludePattern=" + ResFinderTypeIncludePattern);

    return sb.ToString();
  }
}
