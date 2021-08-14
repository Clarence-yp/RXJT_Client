using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;

public class AnimationSplitClip : MonoBehaviour {

    public static ArrayList txtList;
    public static string fullPath;
    public static bool bEnable = true;
    public static ModelImporterAnimationType animType = ModelImporterAnimationType.Generic;
    //static string path;
    //static string fileName;

    public struct animationClipStruct
    {
        public string animationClipName;
        public int startFrame;
        public int endFrame;
        public bool isLoop;
        public WrapMode wrapMode;
    };

    enum ANIMATIONSTRUCT
    {
        eClipName = 0,
        eStartFrame = 1,
        eEndFrame = 2,
        //eIsLoop = 3,
        eWrapMode = 3
    };

    public static void LoadSplitFile()
    {
        fullPath = EditorUtility.OpenFilePanel("OpenSplitFile", Application.dataPath, "txt");
        txtList = LoadTxtFile();
    }

    public static void LoadSplitFile(string path)
    {
        fullPath = path;
        txtList = LoadTxtFile();
    }

    static ArrayList LoadTxtFile()
    {
        ArrayList arrlist = new ArrayList();
        animationClipStruct animClip = new animationClipStruct();       

        StreamReader sr = null;
        try
        {
            sr = File.OpenText(fullPath);

        int index = 0;
        string line;
        while ((line = sr.ReadLine()) != null)
        {
            index++;
            string[] array = line.Split(new string[]{"	"}, StringSplitOptions.RemoveEmptyEntries);
            string temp;
            if (array.Length > 0)
            {
                animClip.animationClipName = array[(int)ANIMATIONSTRUCT.eClipName];
                temp = array[(int)ANIMATIONSTRUCT.eStartFrame];
                animClip.startFrame = int.Parse(temp);
                temp = array[(int)ANIMATIONSTRUCT.eEndFrame];
                animClip.endFrame = int.Parse(temp);
                //temp = array[(int)ANIMATIONSTRUCT.eIsLoop];
                //animClip.isLoop = bool.Parse(temp);
                temp = array[(int)ANIMATIONSTRUCT.eWrapMode];
					
                bool bSucceed = false;
                if (temp == "Default" || temp == "default")
                {
                    animClip.wrapMode = WrapMode.Default;
                    bSucceed = true;
                }
                if (temp == "Once" || temp == "once")
                {
                    animClip.wrapMode = WrapMode.Once;
                    bSucceed = true;
                }
                if (temp == "Clamp" || temp == "clamp")
                {
                    animClip.wrapMode = WrapMode.Clamp;
                    bSucceed = true;
                }
                if (temp == "Loop" || temp == "loop")
                {
                    animClip.wrapMode = WrapMode.Loop;
                    bSucceed = true;
                }
                if (temp == "PingPong" || temp == "pingpong" || temp == "Pingpong")
                {
                    animClip.wrapMode = WrapMode.PingPong;
                    bSucceed = true;
                }
                if (temp == "ClampForever" || temp == "clampforever" || temp == "Clampforever")
                {
                    animClip.wrapMode = WrapMode.ClampForever;
                    bSucceed = true;
                }
                if (!bSucceed)
                {
                    Debug.Log("Error in TXT File in Line" + index);
                }
            }
            arrlist.Add(animClip);
        }
			        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            return null;
        }
		finally
		{
		sr.Close();
        sr.Dispose();
		sr = null;
		}
        
        return arrlist;
    }
}
