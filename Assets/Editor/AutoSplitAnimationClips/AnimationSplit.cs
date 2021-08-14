using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class AnimationSplit : AssetPostprocessor
{
  void OnPreprocessModel()
  {
    if (AnimationSplitClip.txtList != null && AnimationSplitClip.txtList.Count > 0 && AnimationSplitClip.bEnable) {
      ModelImporter modelImporter = assetImporter as ModelImporter;
      editorImporterUtil.clipArrayListCreater creater = new editorImporterUtil.clipArrayListCreater();

      modelImporter.animationType = AnimationSplitClip.animType;//ModelImporterAnimationType.Legacy;
      modelImporter.animationCompression = ModelImporterAnimationCompression.Off;

      int index = 0;
      while (index < AnimationSplitClip.txtList.Count) {
        AnimationSplitClip.animationClipStruct animClip =
            (AnimationSplitClip.animationClipStruct)(AnimationSplitClip.txtList[index]);
        creater.addClip(animClip.animationClipName,
            animClip.startFrame, animClip.endFrame, animClip.isLoop, animClip.wrapMode);
        index++;
      }
      modelImporter.clipAnimations = creater.getArray();
    }
  }
}

namespace editorImporterUtil
{
  public class clipArrayListCreater
  {
    private List<ModelImporterClipAnimation> clipList = new List<ModelImporterClipAnimation>();
    public void addClip(string name, int firstFrame, int lastFrame, bool loop, WrapMode wrapMode)
    {
      ModelImporterClipAnimation tempClip = new ModelImporterClipAnimation();
      tempClip.name = name;
      tempClip.firstFrame = firstFrame;
      tempClip.lastFrame = lastFrame;
      tempClip.loop = loop;
      tempClip.wrapMode = wrapMode;

      clipList.Add(tempClip);
    }

    public ModelImporterClipAnimation[] getArray()
    {
      return clipList.ToArray();
    }
  }

}
