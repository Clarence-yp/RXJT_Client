using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(CustomGestureTemplate))]
public class CustomGestureTemplateInspector : Editor 
{
  public const float GestureEditorSize = 400;
  protected static GUIContent LABEL_IsRotate = new GUIContent("Is Rotate", "");
  protected static GUIContent LABEL_RotateAngle = new GUIContent("Rotate Angle", "");
  protected static GUIContent LABEL_RotateStep = new GUIContent("Rotate Step", "");
  protected static GUIContent LABEL_CaclTowards = new GUIContent("Cacl Towards", "");
  protected static GUIContent LABEL_Tolerance = new GUIContent("Tolerance Angle", "");
  
  [MenuItem("Assets/Create/Custom Gesture")]
  public static void CreateCustomGesture()
  {
    CustomGestureTemplate template = FingerGesturesEditorUtils.CreateAsset<CustomGestureTemplate>();
    FingerGesturesEditorUtils.SelectAssetInProjectView(template);
  }

  public override void OnInspectorGUI()
  {
    CustomGestureTemplate template = target as CustomGestureTemplate;

    if (GUILayout.Button("Edit", GUILayout.Height(50)))
      CustomGestureEditor.Open(template);

    template.tolerance = EditorGUILayout.IntField(LABEL_Tolerance, template.tolerance);
    template.isCaclTowards = EditorGUILayout.Toggle(LABEL_CaclTowards, template.isCaclTowards);
    template.isRotate = EditorGUILayout.Toggle(LABEL_IsRotate, template.isRotate);
    template.RotateAngle = EditorGUILayout.FloatField(LABEL_RotateAngle, template.RotateAngle);
    template.RotateStep = EditorGUILayout.FloatField(LABEL_RotateStep, template.RotateStep);

    /*
    if( GUILayout.Button( "Triangle" ) )
    {
        template.BeginPoints();
        template.AddPoint( 0, 1, 1 );
        template.AddPoint( 0, 2, 2 );
        template.AddPoint( 0, 3, 1 );
        template.AddPoint( 0, 1, 1 );
        template.EndPoints();          
    }

    if( GUILayout.Button( "Square" ) )
    {
        template.BeginPoints();
        template.AddPoint( 0, 2, 1 );
        template.AddPoint( 0, 2, 3 );
        template.AddPoint( 0, 4, 3 );
        template.AddPoint( 0, 4, 1 );
        template.AddPoint( 0, 2, 1 );
        template.EndPoints();
    }*/
  }

  static GUIContent previewTitle = new GUIContent("Gesture Preview");

  public override bool HasPreviewGUI()
  {
    return true;
  }

  public override GUIContent GetPreviewTitle()
  {
    return previewTitle;
  }

  public override void OnPreviewSettings()
  {
    base.OnPreviewSettings();

    CustomGestureTemplate template = target as CustomGestureTemplate;
    GUILayout.Label(template.PointCount + " points, " + template.StrokeCount + " stroke(s)");
  }

  public override void OnPreviewGUI(Rect r, GUIStyle background)
  {
    base.OnPreviewGUI(r, background);

    float size = 0.95f * Mathf.Min(r.width, r.height);
    Rect canvasRect = new Rect(r.center.x - 0.5f * size, r.center.y - 0.5f * size, size, size);

    CustomGestureTemplate template = target as CustomGestureTemplate;

    Vector2 center = canvasRect.center;

    float scale = 0.95f * size;

    Handles.color = Color.white;
    for (int i = 1; i < template.PointCount; ++i) {
      Vector2 p1 = template.GetPosition(i - 1);
      Vector2 p2 = template.GetPosition(i);

      p1.y = -p1.y;
      p2.y = -p2.y;

      p1 = center + scale * p1;
      p2 = center + scale * p2;

      if (canvasRect.width > 100) {
        float handleSize = canvasRect.width / 200.0f;
        Handles.CircleHandleCap(0, p1, Quaternion.identity, handleSize, EventType.MouseDown);
      }

      Handles.DrawLine(p1, p2);
    }
  }
}
