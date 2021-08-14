using UnityEngine;
using DashFire;

#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;

public class CustomGestureTemplate : ScriptableObject 
{
  [SerializeField]
  public int tolerance = 10;
  [SerializeField]
  public bool isCaclTowards = false;
  [SerializeField]
  public bool isRotate = true;
  [SerializeField]
  public float RotateAngle = 360f;
  [SerializeField]
  public float RotateStep = 18f;
  [SerializeField]
  List<int> strokeIds; // maps point index -> stroke id
  [SerializeField]
  List<Vector2> positions;
  [SerializeField]
  int strokeCount = 0;
  [SerializeField]
  Vector2 size = Vector2.zero; // normalized size

  ///  Normalized size
  public Vector2 Size
  {
    get
    {
      return size;
    }
  }

  /// Normalized width
  public float Width
  {
    get
    {
      return size.x;
    }
  }

  /// Normalized height
  public float Height
  {
    get
    {
      return size.y;
    }
  }

  void Awake()
  {
    if (positions == null) {
      positions = new List<Vector2>();
    }
    if (strokeIds == null) {
      strokeIds = new List<int>();
    }
  }

  public void BeginPoints()
  {
    positions.Clear();
    strokeIds.Clear();
    strokeCount = 0;
    size = Vector2.zero;
  }

  public void AddPoint(int stroke, Vector2 p)
  {
    strokeIds.Add(stroke);
    positions.Add(p);
  }

  public void AddPoint(int stroke, float x, float y)
  {
    AddPoint(stroke, new Vector2(x, y));
  }

  public void EndPoints()
  {
    Normalize();

    List<int> uniqueStrokesFound = new List<int>();

    for (int i = 0; i < strokeIds.Count; ++i) {
      int id = strokeIds[i];

      if (!uniqueStrokesFound.Contains(id))
        uniqueStrokesFound.Add(id);
    }

    strokeCount = uniqueStrokesFound.Count;

    MakeDirty();
  }

  public Vector2 GetPosition(int pointIndex)
  {
    return positions[pointIndex];
  }

  public int GetStrokeId(int pointIndex)
  {
    return strokeIds[pointIndex];
  }

  public int PointCount
  {
    get
    {
      return positions.Count;
    }
  }

  public int StrokeCount
  {
    get
    {
      return strokeCount;
    }
  }

  public void Normalize()
  {
    Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
    Vector2 max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

    for (int i = 0; i < positions.Count; ++i) {
      Vector2 p = positions[i];
      min.x = Mathf.Min(min.x, p.x);
      min.y = Mathf.Min(min.y, p.y);
      max.x = Mathf.Max(max.x, p.x);
      max.y = Mathf.Max(max.y, p.y);
    }

    float width = max.x - min.x;
    float height = max.y - min.y;

    float biggestSide = Mathf.Max(width, height);
    float invSize = 1.0f / biggestSide;

    size.x = width * invSize;
    size.y = height * invSize;

    Vector2 offset = -0.5f * size;

    for (int i = 0; i < positions.Count; ++i)
      positions[i] = ((positions[i] - min) * invSize) + offset;
  }

  void MakeDirty()
  {
#if UNITY_EDITOR
    EditorUtility.SetDirty(this);
#endif
  }
}
