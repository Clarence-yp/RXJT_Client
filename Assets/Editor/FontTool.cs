using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
public class FontReplese
{
    //批量替换场景中的字体  
    [MenuItem("Custom/Label/ChangeSceneLabel")]
    public static void ChangeSceneFont()
    {
        List<UILabel> labelList = NGUIEditorTools.FindAll<UILabel>();
        ChangeLabelFont(labelList);
    }


    public static void ChangeLabelFont(List<UILabel> labelList)
    {

        //Font mFont = NGUIEditorTools.LoadAsset<Font>("Assets/font/DroidSansFallback.TTF");
        Font mFont = NGUIEditorTools.LoadAsset<Font>("Assets/Font/simhei.ttf");
        //注意这个地方是要替换成的字体的路径  
        if (mFont == null)
        {
            Debug.LogError(" Font not found ! ");
            return;
        }
        foreach (var label in labelList)
        {
            //if (label != null && label.trueTypeFont != null && label.trueTypeFont.name == "Arial")//这个地方的name可以改为原来的字体的名称  
            if (label != null )//这个地方的name可以改为原来的字体的名称  
            {
                label.trueTypeFont = mFont;
             
            }
        }

        Debug.LogError(labelList.Count);
    }

    //改变选中的预设上边的字体  
    [MenuItem("Custom/Label/ChangeSelectionLabelFont")]
    public static void ChangeSelectObjFont()
    {
        GetTypeList();
        //ChangeLabelFont ( );  
    }

    public static List<UILabel> GetTypeList ()
    {
        Object[] objList = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
        //Debug.LogError (Selection.activeObject.name);  
        Debug.LogError(objList.Length);
        List<UILabel> tmpList = new List<UILabel>();
        foreach (var tmp in objList)
        {
            if ((tmp as GameObject ) != null )  
{
                string assetstr = AssetDatabase.GetAssetPath(tmp);
                GameObject obj = PrefabUtility.InstantiatePrefab(tmp) as GameObject;

                List<UILabel> tmpLabel = GetTypeIncludeChildren(obj);
                if (tmpLabel.Count > 0)
                {
                    ChangeLabelFont(tmpLabel); try
                    {
                        PrefabUtility.ReplacePrefab(obj, tmp);
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogError(ex.ToString() + tmp.name);
                    }
                    //Debug.LogError ( AssetDatabase.RenameAsset ( assetstr, name ) );  
                }

                GameObject.DestroyImmediate(obj);
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return tmpList;
    }

    public static List<UILabel> GetTypeIncludeChildren (Object obj )
    {
        GameObject tmp = obj as GameObject;
        if (tmp == null)
        {
            //return new List<UILabel> ();  
        }
        List<UILabel> tmpList = new List<UILabel>();
        UILabel tmpLabel = tmp.GetComponent<UILabel>();
        if (tmpLabel != null)
        {
            tmpList.Add(tmpLabel);
        }
        foreach (Transform child in tmp.transform)
        {
            tmpList.AddRange(GetTypeIncludeChildren(child.gameObject));
        }
        return tmpList;
    }

}