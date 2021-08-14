using UnityEngine;
using System.Collections;

public class WindowInfo
{
  public string windowName = "";
  public string windowPath = "";
  public Vector3 windowPos = new Vector3();
  public UIType windowType = UIType.DontLoad;
  public bool isCenter = false;
  public int sceneId = 0;
}