using UnityEngine;
using UnityEditor;
using System.IO;

public class AnimationSplitWindow : EditorWindow
{
    static string txtFilePath;
    static bool isInited = false;

    static bool bEnableAutoSlpit;
    static int index = 2;
    string[] option = { "None", "Legacy", "Generic", "Human" };

    [MenuItem("Custom/AutoSplitAnimation")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        AnimationSplitWindow window = 
            (AnimationSplitWindow)EditorWindow.GetWindow(typeof(AnimationSplitWindow));

        InitData();

        isInited = true;
    }

    static void ReadTxtFile(FileInfo fileInfo)
    {
        StreamReader sr;
        sr = fileInfo.OpenText();

        string line = sr.ReadLine();
        if (line != null)
        {
            txtFilePath = line;
            AnimationSplitClip.LoadSplitFile(txtFilePath);
        }
        line = sr.ReadLine();
        if (line != null)
        {
            int temp = int.Parse(line);
            bEnableAutoSlpit = temp == 0 ? false : true;
        }
        line = sr.ReadLine();
        if (line != null)
        {
            int temp = int.Parse(line);
            index = temp;
        }
        sr.Close();
        sr.Dispose();
    }

    static void WriteTxtFile(FileInfo fileInfo)
    {
        StreamWriter sw;
        sw = fileInfo.CreateText();
        sw.WriteLine(txtFilePath);
        int temp = bEnableAutoSlpit ? 1 : 0;
        sw.WriteLine(temp);
        sw.WriteLine(index);
        sw.Close();
        sw.Dispose();
    }

    static void InitData()
    {
        FileInfo fileInfo = new FileInfo(Application.dataPath + "//" + "AnimationSplitOptionFile.txt");
        if (!fileInfo.Exists)
        {
            WriteTxtFile(fileInfo);
        }
        else
        {
            ReadTxtFile(fileInfo);
        }
        //bEnableAutoSlpit = AnimationSplitClip.bEnable;
    }

    //void OnInspectorUpdate()
    //{
    //    if (!isInited)
    //    {
    //        InitData();
    //        isInited = true;
    //    }
    //}

    void OnGUI()
    {
        if (!isInited)
        {
            InitData();
            isInited = true;
        }

        bool needSave = false;

        GUILayout.Label("导入非骨骼模型时需要关闭自动分割", EditorStyles.boldLabel);
        bool bTemp;
        bTemp = EditorGUILayout.Toggle("启用自动分割", bEnableAutoSlpit);
        AnimationSplitClip.bEnable = bEnableAutoSlpit;
        if (bTemp != bEnableAutoSlpit)
        {
            bEnableAutoSlpit = bTemp;
            needSave = true;
        }

        int temp;
        temp = EditorGUILayout.Popup("动画类型", index, option);

        if (temp != index)
        {
            index = temp;
            needSave = true;
        }

        switch (index)
        {
            case 0:
                AnimationSplitClip.animType = ModelImporterAnimationType.None;
                break;
            case 1:
                AnimationSplitClip.animType = ModelImporterAnimationType.Legacy;
                break;
            case 2:
                AnimationSplitClip.animType = ModelImporterAnimationType.Generic;
                break;
            case 3:
                AnimationSplitClip.animType = ModelImporterAnimationType.Human;
                break;
        }

        if (GUILayout.Button("打开分割配置文件", GUILayout.Width(100)))
        {
            AnimationSplitClip.LoadSplitFile();
            txtFilePath = AnimationSplitClip.fullPath;
            needSave = true;
        }

        if (needSave)
            SaveChange();
        
        EditorGUILayout.TextField(txtFilePath);
    }

    void SaveChange()
    {
        FileInfo fileInfo = new FileInfo(Application.dataPath + "//" + "AnimationSplitOptionFile.txt");
        if (fileInfo.Exists)
        {
            File.Delete(Application.dataPath + "//" + "AnimationSplitOptionFile.txt");
            WriteTxtFile(fileInfo);
        }
    }
}
