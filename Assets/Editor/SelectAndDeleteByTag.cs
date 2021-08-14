using UnityEngine;

using System.Collections;

using UnityEditor;

public class ChangeTagOrLayer : EditorWindow
{
  private static string tagStr1 = string.Empty;

  /// ��������ʾ����

  /// </summary>
  /// 
  public GameObject[] objects;

  [@MenuItem("Custom/SelectAndDeleteByTag")]

  private static void Init()
  {

    ChangeTagOrLayer window = (ChangeTagOrLayer)GetWindow(typeof(ChangeTagOrLayer), true, "SelectAndDeleteByTag");

    window.Show();

  }
  /// <summary>

  /// ��ʾ�������������

  /// </summary>

  private void OnGUI()
  {

    tagStr1 = EditorGUILayout.TagField("Tag to Select", tagStr1);

    if (GUILayout.Button("Select Tag"))
    {
      objects = GameObject.FindGameObjectsWithTag(tagStr1);
        Selection.objects = objects;
    }

    //if (GUILayout.Button("Active Tag"))
    //{
    //  objects = GameObject.FindGameObjectsWithTag(tagStr1);
    //  foreach (GameObject go in objects)

    //    go.SetActive(true);
    //}

    if (GUILayout.Button("Delete Tag"))
    {
      objects = GameObject.FindGameObjectsWithTag(tagStr1);
      foreach (GameObject go in objects)

        GameObject.DestroyImmediate(go);
    }
  }
}