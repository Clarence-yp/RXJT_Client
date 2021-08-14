using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using DashFire;
using UnityEngine;
using UnityEditor;
using System.Security.Cryptography;
using GfxModule.SkillResourceAnalysis;
using SkillSystem;

public class ResCacheGenerator
{
  class ResCacheInfo
  {
    public int Id;
    public int Chapter;
    public ResCacheType CacheType;
    public int ResId;
    public HashSet<string> AssetNameList = new HashSet<string>();
    public HashSet<int> AssetList = new HashSet<int>();
    public HashSet<int> AssetLinkList = new HashSet<int>();
    public ResCacheInfo Clone()
    {
      ResCacheInfo cloneObj = new ResCacheInfo();
      cloneObj.Id = Id;
      cloneObj.Chapter = Chapter;
      cloneObj.CacheType = CacheType;
      cloneObj.ResId = ResId;
      cloneObj.AssetNameList.UnionWith(AssetNameList);
      cloneObj.AssetList.UnionWith(AssetList);
      cloneObj.AssetLinkList.UnionWith(AssetLinkList);
      return cloneObj;
    }
  }

  private static int s_IdGen = 0;
  private static Dictionary<int, ResCacheInfo> s_ResCacheDict = new Dictionary<int, ResCacheInfo>();
  private static SkillResourceAnalysis s_SkillResourcesAnlysis = null;
  private static string[] s_CacheResExclude = null;

  private static ResCacheInfo uiCacheInfo = null;
  private static ResCacheInfo monsterEffectInfo = null;

  public static bool BuildResCacheFile()
  {
    string filePath = ResBuildHelper.FormatResCacheFilePath();
    s_IdGen = 0;
    s_ResCacheDict.Clear();
    s_CacheResExclude = ResBuildConfig.ResCacheResExclude.Split(ResBuildConfig.ConfigSplit, StringSplitOptions.RemoveEmptyEntries);

    if (!CollectResCache()) {
      ResBuildLog.Warn("ResBuildGenerator.CollectResCache failed");
      return false;
    }
    if (!OutputResCacheFile(filePath)) {
      ResBuildLog.Warn("ResBuildGenerator.OutputResBuildFile failed");
      return false;
    }
    if (!ExtractResChapterData()) {
      ResBuildLog.Warn("ResBuildGenerator.ExtractResChapterData failed");
      return false;
    }
    if (!DumpResChapterInfo()) {
      ResBuildLog.Warn("ResCacheGenerator.DumpResChapterInfo failed");
      return false;
    }
    if (!BuildResCache()) {
      ResBuildLog.Warn("ResBuildGenerator.BuildResCache failed");
      return false;
    }
    ResBuildLog.Info("ResCacheGenerator.BuildResCacheFile Success");
    return true;
  }
  private static void SearchSkillLinkList(int skillId, ref HashSet<int> skillLinkList)
  {
    SkillLogicData skillLogicData = (SkillLogicData)(SkillConfigProvider.Instance.ExtractData(SkillConfigType.SCT_SKILL, skillId));
    if (skillLogicData != null && !skillLinkList.Contains(skillId)) {
      skillLinkList.Add(skillId);
      if (skillLogicData.NextSkillId > 0) {
        SearchSkillLinkList(skillLogicData.NextSkillId, ref skillLinkList);
      }
      if (skillLogicData.QSkillId > 0) {
        SearchSkillLinkList(skillLogicData.QSkillId, ref skillLinkList);
      }
      if (skillLogicData.ESkillId > 0) {
        SearchSkillLinkList(skillLogicData.ESkillId, ref skillLinkList);
      }
    }
  }
  private static void SearchSkillLiftList(int skillId, ref HashSet<int> skillLiftList)
  {
    SkillLogicData skillLogicData = (SkillLogicData)(SkillConfigProvider.Instance.ExtractData(SkillConfigType.SCT_SKILL, skillId));
    if (skillLogicData != null && !skillLiftList.Contains(skillId)) {
      skillLiftList.Add(skillId);
      if (skillLogicData.LiftSkillId > 0) {
        SearchSkillLiftList(skillLogicData.LiftSkillId, ref skillLiftList);
      }
    }
  }
  private static SkillInstance SearchSkillLogicResInfo(int skillId)
  {
    SkillInstance sIns = null;

    if (s_SkillResourcesAnlysis == null) {
      s_SkillResourcesAnlysis = new SkillResourceAnalysis();
      s_SkillResourcesAnlysis.Init();
    }
    SkillLogicData skillLogicData = (SkillLogicData)(SkillConfigProvider.Instance.ExtractData(SkillConfigType.SCT_SKILL, skillId));
    if (skillLogicData == null || string.IsNullOrEmpty(skillLogicData.SkillDataFile)) {
      ResBuildLog.Warn("ResCacheGeneragor.SearchSkillLogicResInfo analyze skill invalid.skillId:" + skillId);
      return sIns;
    }

    sIns = s_SkillResourcesAnlysis.Analyze(skillId);
    if (sIns == null) {
      ResBuildLog.Warn("ResCacheGeneragor.SearchSkillLogicResInfo analyze skill failed.skillId:" + skillId);
    }
    return sIns;
  }
  private static bool OutputResCacheFile(string filePath)
  {
    string fileContent = ResBuildConfig.ResCacheHeader + "\n";
    foreach (ResCacheInfo info in s_ResCacheDict.Values) {
      //Id	Chapter	CacheType	ResName	Assets	Links
      string abInfo = string.Format(ResBuildConfig.ResCacheFormat + "\n",
        info.Id,
        info.Chapter,
        Enum.GetName(typeof(ResCacheType), info.CacheType),
        info.ResId,
        FormatAssetList(info.AssetList),
        FormatAssetList(info.AssetNameList),
        FormatAssetList(info.AssetLinkList));
      fileContent += abInfo;
    }
    try {
      if (!ResBuildHelper.CheckFilePath(filePath)) {
        ResBuildLog.Warn("ResBuildGenerator.OutputResCacheFile file not exist.filepath:" + filePath);
        return false;
      }
      File.WriteAllText(filePath, fileContent);
    } catch (System.Exception ex) {
      ResBuildLog.Warn("ResBuildGenerator.OutputResCacheFile file failed!" + ex);
      return false;
    }
    AssetDatabase.Refresh();
    ResBuildLog.Info("ResBuildGenerator.OutputResCacheFile Success!");
    return true;
  }
  private static bool BuildResCache()
  {
    if (string.IsNullOrEmpty(ResBuildConfig.ResCacheFilePath)
      || string.IsNullOrEmpty(ResBuildConfig.ResCacheZipPath)) {
      ResBuildLog.Warn("ResCacheGenerator.BuildResCacheFile ResCacheFile config invalid.");
      return false;
    }

    try {
      string outputPath = ResBuildHelper.GetFilePathAbs(ResBuildHelper.GetPlatformPath(ResBuildConfig.BuildOptionTarget));
      if (!System.IO.Directory.Exists(outputPath)) {
        System.IO.Directory.CreateDirectory(outputPath);
      }
      if (!System.IO.Directory.Exists(outputPath)) {
        ResBuildLog.Warn("ResCacheGenerator.BuildResCacheFile directory create failed Path:" + outputPath);
        return false;
      }
    } catch (System.Exception ex) {
      ResBuildLog.Warn("ResCacheGenerator.BuildResCacheFile directory check failed! ex:" + ex);
      return false;
    }

    UnityEngine.TextAsset resVersionObj = AssetDatabase.LoadAssetAtPath(
      ResBuildHelper.FormatResCacheFilePath(), typeof(UnityEngine.TextAsset)) as TextAsset;
    UnityEngine.Object[] assets = { resVersionObj };
    string[] assetNames = { ResBuildConfig.ResCacheFilePath };
    if (resVersionObj != null) {
      BuildPipeline.BuildAssetBundleExplicitAssetNames(assets, assetNames,
        ResBuildHelper.FormatResCacheZipPath(),
        ResBuildConfig.BuildOptionRes,
        ResBuildConfig.BuildOptionTarget);
    } else {
      ResBuildLog.Warn("ResCacheGenerator.BuildResCacheFile failed:");
      return false;
    }
    AssetDatabase.Refresh();
    ResBuildLog.Info("ResCacheGenerator.BuildResCache Success");
    return true;
  }
  private static bool ExtractResChapterData()
  {
    string resCacheFilePathAbs = ResBuildHelper.GetFilePathAbs(ResBuildHelper.FormatResCacheFilePath());
    byte[] buffer = File.ReadAllBytes(resCacheFilePathAbs);
    ResCacheProvider.Instance.Clear();
    ResCacheProvider.Instance.CollectDataFromDBC(buffer);

    ResBuildConfig.ResChapterRes.Clear();
    List<ResChapterInfo> tChapterResList = ResBuildConfig.ResChapterRes;
    int maxChapterNum = 0;
    foreach (ResCacheData cacheData in ResCacheProvider.Instance.GetArray()) {
      ResChapterInfo chapterInfo = null;
      int curChapter = cacheData.m_Chapter;
      if (curChapter < 1) {
        curChapter = 1;
      }
      if (curChapter > maxChapterNum) {
        maxChapterNum = curChapter;
      }
      while (tChapterResList.Count < maxChapterNum) {
        chapterInfo = new ResChapterInfo();
        tChapterResList.Add(chapterInfo);
        chapterInfo.ResChapterIndex = tChapterResList.Count;
      }
      chapterInfo = tChapterResList[curChapter - 1];
      chapterInfo.ResChapterIdList.UnionWith(cacheData.m_Assets);
    }

    HashSet<int> alreadyAssetSet = new HashSet<int>();
    for (int index = 0; index < tChapterResList.Count; index++) {
      ResChapterInfo info = tChapterResList[index];
      info.ResChapterIdList.ExceptWith(alreadyAssetSet);
      alreadyAssetSet.UnionWith(info.ResChapterIdList);
    }

    ResBuildConfig.ResChapterCount = ResBuildConfig.ResChapterRes.Count;
    foreach (ResChapterInfo info in tChapterResList) {
      info.ResChapterIdContent = FormatAssetList(info.ResChapterIdList);
    }

    ResBuildConfig.Save();
    return true;
  }
  private static bool DumpResChapterInfo()
  {
    StringBuilder sb = new StringBuilder();
    sb.AppendLine("ResChapterRes: count:" + ResBuildConfig.ResChapterRes.Count);
    foreach (ResChapterInfo info in ResBuildConfig.ResChapterRes) {
      sb.AppendLine(string.Format("ResChapterRes{0}:{1}", (info.ResChapterIndex), info.ResChapterIdContent));
    }
    ResBuildLog.Info(sb.ToString());
    return true;
  }

  private static bool CollectUICache()
  {
    // ui_global
    uiCacheInfo = new ResCacheInfo();
    uiCacheInfo.Id = ++s_IdGen;
    s_ResCacheDict.Add(uiCacheInfo.Id, uiCacheInfo);
    uiCacheInfo.Chapter = 1;
    uiCacheInfo.CacheType = ResCacheType.ui;
    uiCacheInfo.ResId = -1;
    return true;
  }
  private static bool CollectLevelAndMonsterCache()
  {
    string outResName = string.Empty;
    int outResId = -1;
    HashSet<int> allSkillSet = new HashSet<int>();
    HashSet<int> allImpactSet = new HashSet<int>();
    HashSet<string> resources = new HashSet<string>();

    // level
    DataDictionaryMgr<Data_SceneConfig> tSceneConfigMgr = SceneConfigProvider.Instance.SceneConfigMgr;
    foreach (Data_SceneConfig sceneConfig in tSceneConfigMgr.GetData().Values) {
      // level
      ResCacheInfo sceneInfo = new ResCacheInfo();
      sceneInfo.Id = ++s_IdGen;
      s_ResCacheDict.Add(sceneInfo.Id, sceneInfo);
      sceneInfo.Chapter = sceneConfig.m_Chapter;
      sceneInfo.CacheType = ResCacheType.level;
      sceneInfo.ResId = sceneConfig.GetId();
      string outSceneName = string.Empty;
      int outSceneId = -1;
      if (FormatSceneFile(sceneConfig.m_ClientSceneFile, out outSceneName, out outSceneId)) {
        sceneInfo.AssetList.Add(outSceneId);
        sceneInfo.AssetNameList.Add(outSceneName);
      }
      //if (uiCityCacheInfo != null
      //  && FormatResFile(sceneConfig.m_AtlasPath, out outResName, out outResId)) {
      //  uiCityCacheInfo.AssetList.Add(outResId);
      //  uiCityCacheInfo.AssetNameList.Add(outResName);
      //}

      MapDataProvider dataProvider = SceneConfigProvider.Instance.GetMapDataBySceneResId(sceneConfig.GetId());
      if (dataProvider != null) {
        DataDictionaryMgr<Data_Unit> tUnitMgr = dataProvider.m_UnitMgr;
        if (tUnitMgr != null) {
          Dictionary<int, ResCacheInfo> tMonsterResCacheDict = new Dictionary<int, ResCacheInfo>();
          foreach (Data_Unit sceneUnit in tUnitMgr.GetData().Values) {
            Data_NpcConfig ncpConfig = NpcConfigProvider.Instance.GetNpcConfigById(sceneUnit.m_LinkId);
            if (ncpConfig != null) {
              // monster_model
              ResCacheInfo monsterModelInfo = null;
              if (tMonsterResCacheDict.ContainsKey(sceneUnit.m_LinkId)) {
                monsterModelInfo = tMonsterResCacheDict[sceneUnit.m_LinkId].Clone();
                monsterModelInfo.Id = ++s_IdGen;
                s_ResCacheDict.Add(monsterModelInfo.Id, monsterModelInfo);
                monsterModelInfo.Chapter = sceneConfig.m_Chapter;
                monsterModelInfo.CacheType = ResCacheType.monster;
                monsterModelInfo.ResId = sceneUnit.GetId();
                sceneInfo.AssetLinkList.Add(monsterModelInfo.Id);
                continue;
              } else {
                monsterModelInfo = new ResCacheInfo();
                monsterModelInfo.Id = ++s_IdGen;
                s_ResCacheDict.Add(monsterModelInfo.Id, monsterModelInfo);
                monsterModelInfo.Chapter = sceneConfig.m_Chapter;
                monsterModelInfo.CacheType = ResCacheType.monster;
                monsterModelInfo.ResId = sceneUnit.GetId();
                sceneInfo.AssetLinkList.Add(monsterModelInfo.Id);
                tMonsterResCacheDict.Add(sceneUnit.m_LinkId, monsterModelInfo);
              }

              if (FormatResFile(ncpConfig.m_Model, out outResName, out outResId)) {
                monsterModelInfo.AssetList.Add(outResId);
                monsterModelInfo.AssetNameList.Add(outResName);
              }
              // monster_skill
              if (ncpConfig.m_SkillList != null && ncpConfig.m_SkillList.Count > 0) {
                foreach (int skillId in ncpConfig.m_SkillList) {
                  allSkillSet.Clear();
                  allImpactSet.Clear();
                  resources.Clear();

                  CollectSkillResources(skillId, ref resources, ref allSkillSet, ref allImpactSet);
                  foreach (string innerRes in resources) {
                    if (FormatResFile(innerRes, out outResName, out outResId)) {
                      monsterModelInfo.AssetList.Add(outResId);
                      monsterModelInfo.AssetNameList.Add(outResName);
                    }
                  }
                }
              }
              // monster_effect
              if (monsterEffectInfo != null) {
                monsterModelInfo.AssetLinkList.Add(monsterEffectInfo.Id);
              }
            }
          }
        }
      }

      // scenedropout
      Data_SceneDropOut dropOutInfo = SceneConfigProvider.Instance.GetSceneDropOutById(sceneConfig.m_DropId);
      if (dropOutInfo != null) {
        ResCacheInfo levelObjInfo = new ResCacheInfo();
        levelObjInfo.Id = ++s_IdGen;
        s_ResCacheDict.Add(levelObjInfo.Id, levelObjInfo);
        levelObjInfo.Chapter = sceneConfig.m_Chapter;
        levelObjInfo.CacheType = ResCacheType.level_obj;
        levelObjInfo.ResId = sceneConfig.m_DropId;
        sceneInfo.AssetLinkList.Add(levelObjInfo.Id);

        if (FormatResFile(dropOutInfo.m_GoldModel, out outResName, out outResId)) {
          levelObjInfo.AssetList.Add(outResId);
          levelObjInfo.AssetNameList.Add(outResName);
        }
        if (FormatResFile(dropOutInfo.m_GoldParticle, out outResName, out outResId)) {
          levelObjInfo.AssetList.Add(outResId);
          levelObjInfo.AssetNameList.Add(outResName);
        }
        if (FormatResFile(dropOutInfo.m_HpModel, out outResName, out outResId)) {
          levelObjInfo.AssetList.Add(outResId);
          levelObjInfo.AssetNameList.Add(outResName);
        }
        if (FormatResFile(dropOutInfo.m_HpParticle, out outResName, out outResId)) {
          levelObjInfo.AssetList.Add(outResId);
          levelObjInfo.AssetNameList.Add(outResName);
        }
        if (FormatResFile(dropOutInfo.m_MpModel, out outResName, out outResId)) {
          levelObjInfo.AssetList.Add(outResId);
          levelObjInfo.AssetNameList.Add(outResName);
        }
        if (FormatResFile(dropOutInfo.m_MpParticle, out outResName, out outResId)) {
          levelObjInfo.AssetList.Add(outResId);
          levelObjInfo.AssetNameList.Add(outResName);
        }
      }
    }

    return true;
  }
  private static bool CollectMonsterEffect()
  {
    string outResName = string.Empty;
    int outResId = -1;
    monsterEffectInfo = new ResCacheInfo();
    monsterEffectInfo.Id = ++s_IdGen;
    s_ResCacheDict.Add(monsterEffectInfo.Id, monsterEffectInfo);
    monsterEffectInfo.Chapter = 1;
    monsterEffectInfo.CacheType = ResCacheType.monster_effect;
    monsterEffectInfo.ResId = -1;

    DataDictionaryMgr<EffectLogicData> effectLogicDataMgr = SkillConfigProvider.Instance.effectLogicDataMgr;
    if (effectLogicDataMgr != null && effectLogicDataMgr.GetDataCount() > 0) {
      foreach (EffectLogicData data in effectLogicDataMgr.GetData().Values) {
        if (data != null) {
          if (FormatResFile(data.EffectPath, out outResName, out outResId)) {
            monsterEffectInfo.AssetList.Add(outResId);
            monsterEffectInfo.AssetNameList.Add(outResName);
          }
        }
      }
    }
    return true;
  }
  private static bool CollectSkillResources(int curSkillId, ref HashSet<string> resources, ref HashSet<int> allSkillSet, ref HashSet<int> allImpactSet)
  {
    if (!allSkillSet.Contains(curSkillId)) {
      allSkillSet.Add(curSkillId);

      HashSet<int> skillLinkList = new HashSet<int>();
      SearchSkillLinkList(curSkillId, ref skillLinkList);
      foreach (int linkSkillId in skillLinkList) {
        SkillLogicData skillLogicData = (SkillLogicData)(SkillConfigProvider.Instance.ExtractData(SkillConfigType.SCT_SKILL, linkSkillId));
        if (skillLogicData != null) {
          SkillInstance sIns = SearchSkillLogicResInfo(skillLogicData.GetId());
          if (sIns != null) {
            resources.UnionWith(sIns.Resources);
            foreach (int impactId in sIns.EnableImpactsToOther) {
              CollectImpactResources(impactId, ref resources, ref allSkillSet, ref allImpactSet);
            }
            foreach (int impactId in sIns.EnableImpactsToMyself) {
              CollectImpactResources(impactId, ref resources, ref allSkillSet, ref allImpactSet);
            }
          }
        }
      }
    }

    return true;
  }
  private static bool CollectImpactResources(int impactId, ref HashSet<string> resources, ref HashSet<int> allSkillSet, ref HashSet<int> allImpactSet)
  {
    if (!allImpactSet.Contains(impactId)) {
      allImpactSet.Add(impactId);

      ImpactLogicData impactLogicData = (ImpactLogicData)(SkillConfigProvider.Instance.ExtractData(SkillConfigType.SCT_IMPACT, impactId));
      if (impactLogicData != null && impactLogicData.EffectList.Count > 0) {
        foreach (List<int> effectListId in impactLogicData.EffectList) {
          foreach (int effectId in effectListId) {
            EffectLogicData effectLogicData = (EffectLogicData)(SkillConfigProvider.Instance.ExtractData(SkillConfigType.SCT_EFFECT, effectId));
            if (effectLogicData != null) {
              resources.Add(effectLogicData.EffectPath);
            }
          }
          switch ((ImpactLogicManager.ImpactLogicId)impactLogicData.ImpactLogicId) {
            case ImpactLogicManager.ImpactLogicId.ImpactLogic_General: {

              } break;
            case ImpactLogicManager.ImpactLogicId.ImpactLogic_SuperArmor: {

              } break;
            case ImpactLogicManager.ImpactLogicId.ImpactLogic_Invincible: {

              } break;
            case ImpactLogicManager.ImpactLogicId.ImpactLogic_ChangeSkill: {
                try {
                  if (impactLogicData.ExtraParams.Count >= 2) {
                    int head_skillid = int.Parse(impactLogicData.ExtraParams[1]);
                    CollectSkillResources(head_skillid, ref resources, ref allSkillSet, ref allImpactSet);
                  }
                } catch (System.Exception ex) {
                  ResBuildLog.Warn("ResCacheGenerator.CollectImpactResources failed: impactId:" + impactId + " ex:" + ex);
                }
              } break;
            case ImpactLogicManager.ImpactLogicId.ImpactLogic_StopImpact: {

              } break;
            case ImpactLogicManager.ImpactLogicId.ImpactLogic_RefixDamage: {

              } break;
            case ImpactLogicManager.ImpactLogicId.ImpactLogic_BlockAndBeat: {
                try {
                  if (impactLogicData.ParamNum >= 3) {
                    int beatSkillId = int.Parse(impactLogicData.ExtraParams[0]);
                    int blockImpactId = int.Parse(impactLogicData.ExtraParams[1]);
                    string EffectAndBone = impactLogicData.ExtraParams[2];
                    string[] EffectBonePair = EffectAndBone.Split('|');
                    if (EffectBonePair.Length >= 2) {
                      resources.Add(EffectBonePair[0]);
                    }
                    CollectSkillResources(beatSkillId, ref resources, ref allSkillSet, ref allImpactSet);
                  }
                } catch (System.Exception ex) {
                  ResBuildLog.Warn("ResCacheGenerator.CollectImpactResources failed: impactId:" + impactId + " ex:" + ex);
                }
              } break;
          }
        }
      }
    }
    return true;
  }
  private static bool CollectItemCache()
  {
    string outResName = string.Empty;
    int outResId = -1;
    // item
    if (uiCacheInfo != null) {
      DataDictionaryMgr<ItemConfig> tItemConfigMgr = ItemConfigProvider.Instance.ItemConfigMgr;
      foreach (ItemConfig config in tItemConfigMgr.GetData().Values) {
        if (FormatResFile(config.m_ItemTrueName, out outResName, out outResId)) {
          uiCacheInfo.AssetList.Add(outResId);
          uiCacheInfo.AssetNameList.Add(outResName);
        }
      }
    }
    return true;
  }
  private static bool CollectResCache()
  {
    AssetDatabase.Refresh();
    LoadResCacheConfig();

    CollectUICache();
    CollectMonsterEffect();
    CollectLevelAndMonsterCache();
    CollectItemCache();

    UnloadResCacheConfig();
    return true;
  }
  private static string FormatAssetList<T>(IEnumerable<T> assetlist)
  {
    StringBuilder sb = new StringBuilder();
    foreach (T asset in assetlist) {
      sb.Append(asset.ToString() + ";");
    }
    return sb.ToString();
  }
  #region Collect Cache Resources
  private static void LoadResCacheConfig()
  {
    HomePath.CurHomePath = Application.streamingAssetsPath;
    GlobalVariables.Instance.IsDebug = false;
    FileReaderProxy.RegisterReadFileHandler((string filePath) => {
      byte[] buffer = null;
      try {
        buffer = File.ReadAllBytes(filePath);
      } catch (Exception e) {
        ResBuildLog.Warn(string.Format("Exception:{0}\n{1}", e.Message, e.StackTrace));
        return null;
      }
      return buffer;
    });
    UnloadResCacheConfig();

    SceneConfigProvider.Instance.Load(FilePathDefine_Client.C_SceneConfig, "ScenesConfigs");
    SceneConfigProvider.Instance.LoadDropOutConfig(FilePathDefine_Client.C_SceneDropOut, "SceneDropOut");
    SceneConfigProvider.Instance.LoadAllSceneConfig(FilePathDefine_Client.C_RootPath);
    NpcConfigProvider.Instance.LoadNpcConfig(FilePathDefine_Client.C_NpcConfig, "NpcConfig");
    PlayerConfigProvider.Instance.LoadPlayerConfig(FilePathDefine_Client.C_PlayerConfig, "PlayerConfig");

    SkillConfigProvider.Instance.CollectData(SkillConfigType.SCT_SOUND, FilePathDefine_Client.C_SoundConfig, "SoundConfig");
    SkillConfigProvider.Instance.CollectData(SkillConfigType.SCT_SKILL, FilePathDefine_Client.C_SkillSystemConfig, "SkillConfig");
    SkillConfigProvider.Instance.CollectData(SkillConfigType.SCT_IMPACT, FilePathDefine_Client.C_ImpactSystemConfig, "ImpactConfig");
    SkillConfigProvider.Instance.CollectData(SkillConfigType.SCT_EFFECT, FilePathDefine_Client.C_EffectConfig, "EffectConfig");

    ItemConfigProvider.Instance.Load(FilePathDefine_Client.C_ItemConfig, "ItemConfig");
    UiConfigProvider.Instance.Load(FilePathDefine_Client.C_UiConfig, "UiConfig");

    ResBuildGenerator.LoadResConfig();

    s_SkillResourcesAnlysis = new SkillResourceAnalysis();
    s_SkillResourcesAnlysis.Init();
  }
  private static void UnloadResCacheConfig()
  {
    SceneConfigProvider.Instance.Clear();
    NpcConfigProvider.Instance.Clear();
    PlayerConfigProvider.Instance.Clear();
    SkillConfigProvider.Instance.Clear();

    ItemConfigProvider.Instance.Clear();
    UiConfigProvider.Instance.Clear();

    s_SkillResourcesAnlysis = null;
  }
  private static bool FormatSceneFile(string sceneName, out string outResName, out int outResId)
  {
    outResName = string.Empty;
    outResId = -1;
    if (string.IsNullOrEmpty(sceneName.Trim())) {
      return false;
    }
    sceneName = ResBuildHelper.ConvertPathSlash(sceneName);
    sceneName = sceneName.ToLower() + ".unity";
    List<ResBuildData> resList = ResBuildProvider.Instance.SearchResByNamePostfix(sceneName);
    if (resList.Count == 1) {
      outResName = resList[0].m_Resources;
      outResId = resList[0].m_Id;
      return true;
    } else if (resList.Count > 1) {
      ResBuildLog.Warn("ResCacheGenerator.FormatResFile res file duplicate: sceneName:" + sceneName);
      outResName = resList[0].m_Resources;
      outResId = resList[0].m_Id;
      return true;
    } else {
      ResBuildLog.Warn("ResCacheGenerator.FormatResFile res file miss: sceneName:" + sceneName);
      return false;
    }
  }
  private static bool FormatResFile(string resFile, out string outResName, out int outResId)
  {
    bool ret = false;
    outResName = string.Empty;
    outResId = -1;
    if (string.IsNullOrEmpty(resFile.Trim())) {
      return ret;
    }


    resFile = ResBuildHelper.ConvertPathSlash(resFile);
    string resFileInAsset = "assets/resources/" + resFile.ToLower() + ".";
    List<ResBuildData> resList = ResBuildProvider.Instance.SearchResByNamePrefix(resFileInAsset);
    if (resList.Count == 1) {
      outResName = resList[0].m_Resources;
      outResId = resList[0].m_Id;
      ret = true;
    } else if (resList.Count > 1) {
      ResBuildLog.Warn("ResCacheGenerator.FormatResFile res file duplicate: resFiles:" + resFile);
      outResName = resList[0].m_Resources;
      outResId = resList[0].m_Id;
      ret = true;
    } else {
      ResBuildLog.Warn("ResCacheGenerator.FormatResFile res file miss: resFile:" + resFile);
      ret = false;
    }

    if (ret && s_CacheResExclude != null && s_CacheResExclude.Length > 0
      && ResBuildHelper.CheckFilePatternStartsWith(outResName, s_CacheResExclude)) {
      ResBuildLog.Info("ResCacheGenerator.FormatResFile res exclude.resFile:" + resFile + "outResName:" + outResName);
      ret = false;
    }
    return ret;
  }
  #endregion
}
