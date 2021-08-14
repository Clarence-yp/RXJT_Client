using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class AssetBundleExec : Editor 
{
  [MenuItem("Assets/AssetBundle/Generate Windows32 AssetBunldes")]
  public static void GenerateWindows32AssetBunldes()
  {
    CreateAssetBunldes(BuildTarget.StandaloneWindows);
  }

  [MenuItem("Assets/AssetBundle/Generate Windows64 AssetBunldes")]
  public static void GenerateWindows64AssetBunldes()
  {
    CreateAssetBunldes(BuildTarget.StandaloneWindows64);
  }

  [MenuItem("Assets/AssetBundle/Generate iPhone AssetBunldes")]
  public static void GenerateIPhoneAssetBunldes()
  {
    CreateAssetBunldes(BuildTarget.iOS);
  }

  [MenuItem("Assets/AssetBundle/Generate Android AssetBunldes")]
  public static void GenerateAndroidAssetBunldes()
  {
    CreateAssetBunldes(BuildTarget.Android);
  }

  private static void CreateAssetBunldes(BuildTarget bt)
  {
    CheckPlatform(bt);
    Caching.ClearCache();
    string Path = EditorUtility.SaveFilePanel("Save Resource", "", "Package", "AssetBundle");
    Object[] SelectedAsset = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
    Debug.Log("selected asset count: "+SelectedAsset.Length);
    if (BuildPipeline.BuildAssetBundle(null, SelectedAsset, Path, BuildAssetBundleOptions.CollectDependencies|BuildAssetBundleOptions.UncompressedAssetBundle, bt)) {
      AssetDatabase.Refresh();
    }
  }

  private static bool CheckPlatform(UnityEditor.BuildTarget target)
  {
    if (EditorUserBuildSettings.activeBuildTarget != target) {
      EditorUtility.DisplayDialog("The target platform do not agree with the current platform. Please first to convert the platform.", "cur platform：" + EditorUserBuildSettings.activeBuildTarget + "\ntarget platform：" + target, "OK");
      return false;
    }
    return true;
  }
}
