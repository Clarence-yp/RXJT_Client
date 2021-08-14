using UnityEngine;
using System.Collections.Generic;
using DashFire;

public enum Section : int 
{
  Invalid = -1,
  First = 0,
  Second = 1,
  Third = 2,
}

public class SectionArg
{
  public float ActiveDistance = 30f;
  public float FireDistance = 100f;
  public float TimeOut = 5000f;
  public Vector2 StartPos = Vector2.zero;
  public float StartTime = 0;

  public SectionArg ()
  {
    ActiveDistance = 30f;
    FireDistance = 100f;
    TimeOut = 5000f;
    StartPos = Vector2.zero;
    StartTime = 0;
  }

  public void ReSet()
  {
    StartPos = Vector2.zero;
    StartTime = 0;
  }
}

public class CustomGesture : Gesture 
{
  public List<CustomRegognizer.Point> RawPoints = new List<CustomRegognizer.Point>(64);
  public List<CustomRegognizer.Point> NormalizedPoints = new List<CustomRegognizer.Point>(64);
  public CustomGestureTemplate RecognizedTemplate = null;
  public float MatchDistance = 0;
  public float MatchScore = 0;
}

public class CustomRegognizer : GestureRecognizerBase<CustomGesture> 
{
  /// 两个连续点最小距离 (像素）
  public float MinDistanceBetweenSamples = 5f;
  /// 允许最大的匹配偏差
  public float MaxMatchDistance = 3.5f;
  /// 几段
  const int SectionNumber = 3;
  /// 段
  public List<SectionArg> SkillSection = new List<SectionArg>(SectionNumber);
  /// 模板列表
  public List<CustomGestureTemplate> Templates;
  /// 当前所在段
  static public Section CurActiveSection = Section.Invalid;
  /// 当前触发的技能
  public SkillCategory ActiveSkill = SkillCategory.kNone;
  /// 对应的激活朝向
  public float ActiveTowards = float.NegativeInfinity;
  /// 最后输入位置
  public Vector2 LastInputPos = Vector2.zero;

  public CustomRegognizer()
  {
    for (int i = 0; i < SectionNumber; i++) {
      SectionArg arg = new SectionArg();
      SkillSection.Add(arg);
    }
  }

  protected override void Refresh(CustomGesture gesture)
  {
    gesture.IsInvalid = false;
    Reset(gesture, false);
  }

  protected override void Reset(CustomGesture gesture, bool isPressed)
  {
    gesture.ClusterId = 0;
    gesture.IsActive = false;
    gesture.SelectedID = -1;
    gesture.HintFlag = HintType.None;
    gesture.SectionNum = -1;
    LastInputPos = Vector2.zero;

    ///
    if (isPressed) {
      if ((int)CurActiveSection >= SectionNumber - 1) {
        gesture.IsInvalid = true;
      }
      for (int i = 0; i < SectionNumber; i++) {
        SkillSection[i].ReSet();
      }
      gesture.RawPoints.Clear();
    } else {
      if (CurActiveSection != Section.Invalid) {
        ReleaseFingers(gesture);
        gesture.Fingers.Clear();
        ///
        CurActiveSection = Section.Invalid;
        ActiveSkill = SkillCategory.kNone;
        ActiveTowards = float.NegativeInfinity;
        for (int i = 0; i < SectionNumber; i++) {
          SkillSection[i].ReSet();
        }
      }
    }
    ///
    gesture.State = GestureRecognitionState.Ready;
  }

  public struct Point {
    public Point(int strokeId, Vector2 pos)
    {
      this.StrokeId = strokeId;
      this.Position = pos;
    }
    public Point(int strokeId, float x, float y)
    {
      this.StrokeId = strokeId;
      this.Position = new Vector2(x, y);
    }
    public int StrokeId;
    public Vector2 Position;
  }

  class NormalizedTemplate 
  {
    public CustomGestureTemplate Source;
    public List<Point> Points;
  }

  class GestureNormalizer 
  {
    List<Point> normalizedPoints;
    List<Point> pointBuffer;

    public GestureNormalizer()
    {
      normalizedPoints = new List<Point>();
      pointBuffer = new List<Point>();
    }

    public List<Point> Apply(List<Point> inputPoints, int normalizedPointsCount)
    {
      normalizedPoints = Resample(inputPoints, normalizedPointsCount);
      Scale(normalizedPoints);
      TranslateToOrigin(normalizedPoints);
      return normalizedPoints;
    }

    // X points => normalizedPointsCount points
    List<Point> Resample(List<Point> points, int normalizedPointsCount)
    {
      float intervalLength = PathLength(points) / (normalizedPointsCount - 1);
      float D = 0;
      Point q = new Point();

      normalizedPoints.Clear();
      normalizedPoints.Add(points[0]);

      pointBuffer.Clear();
      pointBuffer.AddRange(points);

      for (int i = 1; i < pointBuffer.Count; ++i) {
        Point a = pointBuffer[i - 1];
        Point b = pointBuffer[i];

        if (a.StrokeId == b.StrokeId) {
          float d = Vector2.Distance(a.Position, b.Position);

          if ((D + d) > intervalLength) {
            q.Position = Vector2.Lerp(a.Position, b.Position, (intervalLength - D) / d);
            q.StrokeId = a.StrokeId;

            normalizedPoints.Add(q);
            pointBuffer.Insert(i, q);

            D = 0;
          } else {
            D += d;
          }
        }
      }

      if (normalizedPoints.Count == normalizedPointsCount - 1) {
        normalizedPoints.Add(pointBuffer[pointBuffer.Count - 1]);
      }

      return normalizedPoints;
    }

    static float PathLength(List<Point> points)
    {
      float d = 0;
      for (int i = 1; i < points.Count; ++i) {
        if (points[i].StrokeId == points[i - 1].StrokeId) {
          d += Vector2.Distance(points[i - 1].Position, points[i].Position);
        }
      }
      return d;
    }

    static void Scale(List<Point> points)
    {
      Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
      Vector2 max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

      for (int i = 0; i < points.Count; ++i) {
        Point p = points[i];
        min.x = Mathf.Min(min.x, p.Position.x);
        min.y = Mathf.Min(min.y, p.Position.y);
        max.x = Mathf.Max(max.x, p.Position.x);
        max.y = Mathf.Max(max.y, p.Position.y);
      }

      float size = Mathf.Max(max.x - min.x, max.y - min.y);
      float invSize = 1.0f / size;

      for (int i = 0; i < points.Count; ++i) {
        Point p = points[i];
        p.Position = (p.Position - min) * invSize;
        points[i] = p;
      }
    }

    static void TranslateToOrigin(List<Point> points)
    {
      Vector2 c = Centroid(points);
      for (int i = 0; i < points.Count; ++i) {
        Point p = points[i];
        p.Position -= c;
        points[i] = p;
      }
    }

    static Vector2 Centroid(List<Point> points)
    {
      Vector2 c = Vector2.zero;
      for (int i = 0; i < points.Count; ++i) {
        c += points[i].Position;
      }
      c /= points.Count;
      return c;
    }
  }

  const int NormalizedPointCount = 32;
  GestureNormalizer normalizer;
  List<NormalizedTemplate> normalizedTemplates;

  protected override void Awake()
  {
    base.Awake();
    normalizer = new GestureNormalizer();
    normalizedTemplates = new List<NormalizedTemplate>();
    foreach (CustomGestureTemplate template in Templates) {
      AddTemplate(template);
    }
  }

  NormalizedTemplate FindNormalizedTemplate(CustomGestureTemplate template)
  {
    return normalizedTemplates.Find(t => (t.Source == template));
  }

  List<Point> Normalize(List<Point> points)
  {
    return new List<Point>(normalizer.Apply(points, NormalizedPointCount));
  }

  public bool AddTemplate(CustomGestureTemplate template)
  {
    if (FindNormalizedTemplate(template) != null) {
      Debug.Log("AddTemplate Exist");
      return false;
    }

    List<Point> points = new List<Point>();
    for (int i = 0; i < template.PointCount; ++i) {
      points.Add(new Point(template.GetStrokeId(i), template.GetPosition(i)));
    }

    NormalizedTemplate nt = new NormalizedTemplate();
    nt.Source = template;
    nt.Points = Normalize(points);
    normalizedTemplates.Add(nt);

    if (template.isRotate) {
      const float PI = 3.141592f;
      const float Angle = 1.0f / 180.0f * PI;

      float ox = template.GetPosition(template.PointCount / 2).x;
      float oy = template.GetPosition(template.PointCount / 2).y;
      List<Point> newPoints = new List<Point>();
      for (int i = (int)template.RotateStep; i < template.RotateAngle; i += (int)template.RotateStep) {
        for (int j = 0; j < template.PointCount; ++j) {
          float curAngle = i * Angle;
          float px = template.GetPosition(j).x;
          float py = template.GetPosition(j).y;
          float x = ox - (px - ox) * Mathf.Cos(curAngle) - (py - oy) * Mathf.Sin(curAngle);
          float y = oy - (px - ox) * Mathf.Sin(curAngle) + (py - oy) * Mathf.Cos(curAngle);
          newPoints.Add(new Point(0, new Vector2(x, y)));
        }
        NormalizedTemplate newNT = new NormalizedTemplate();
        newNT.Source = template;
        newNT.Points = Normalize(newPoints);
        normalizedTemplates.Add(newNT);
        newPoints.Clear();
      }
    }
    return true;
  }

  protected override bool CanBegin(CustomGesture gesture, TouchManager.IFingerList touches)
  {
    if (touches.Count != RequiredFingerCount) {
      return false;
    }
    if (IsExclusive && TouchManager.Touches.Count != RequiredFingerCount) {
      return false;
    }
    if (DashFire.TouchType.Regognizer != TouchManager.curTouchState) {
      return false;
    }
    if ((int)CurActiveSection >= SectionNumber - 1) {
      return false;
    }
    if (CurActiveSection != Section.Invalid && gesture.IsInvalid) {
      return false;
    }
    Section MediSection = (CurActiveSection == Section.Invalid ? Section.First : CurActiveSection + 1);
    float ActiveDistance = SkillSection[(int)MediSection].ActiveDistance;
    if (ActiveDistance > 0) {
      float Distance = 0;
      if (MediSection == Section.First) {
        gesture.IsInvalid = false;
        Distance = touches.GetAverageDistanceFromStart();
      } else if (MediSection == Section.Second) {
        if (Vector2.zero == SkillSection[(int)MediSection].StartPos) {
          if (Vector2.zero == LastInputPos) {
            LastInputPos = touches.GetAveragePosition();
            return false;
          }
          Point cp = new Point(0, touches.GetAveragePosition());
          Point lp = new Point(0, LastInputPos);

          if (SkillCategory.kSkillA == ActiveSkill) {
            if (cp.Position.x < lp.Position.x) {
              SkillSection[(int)MediSection].StartPos = touches.GetAveragePosition();
            }
          } else if (SkillCategory.kSkillB == ActiveSkill) {
            if (cp.Position.y > lp.Position.y) {
              SkillSection[(int)MediSection].StartPos = touches.GetAveragePosition();
            }
          } else if (SkillCategory.kSkillC == ActiveSkill) {
            if (cp.Position.x > lp.Position.x) {
              SkillSection[(int)MediSection].StartPos = touches.GetAveragePosition();
            }
          } else if (SkillCategory.kSkillD == ActiveSkill) {
            if (cp.Position.y < lp.Position.y) {
              SkillSection[(int)MediSection].StartPos = touches.GetAveragePosition();
            }
          }
          return false;
        } else {
          Distance = Vector2.Distance(touches.GetAveragePosition(), SkillSection[(int)MediSection].StartPos);
        }
      } else if (MediSection == Section.Third) {
        if (Vector2.zero == SkillSection[(int)MediSection].StartPos) {
          if (Vector2.zero == LastInputPos) {
            LastInputPos = touches.GetAveragePosition();
            return false;
          }
          Point cp = new Point(0, touches.GetAveragePosition());
          Point lp = new Point(0, LastInputPos);

          if (SkillCategory.kSkillA == ActiveSkill) {
            if (cp.Position.x > lp.Position.x) {
              SkillSection[(int)MediSection].StartPos = touches.GetAveragePosition();
            }
          } else if (SkillCategory.kSkillB == ActiveSkill) {
            if (cp.Position.y < lp.Position.y) {
              SkillSection[(int)MediSection].StartPos = touches.GetAveragePosition();
            }
          } else if (SkillCategory.kSkillC == ActiveSkill) {
            if (cp.Position.x < lp.Position.x) {
              SkillSection[(int)MediSection].StartPos = touches.GetAveragePosition();
            }
          } else if (SkillCategory.kSkillD == ActiveSkill) {
            if (cp.Position.y > lp.Position.y) {
              SkillSection[(int)MediSection].StartPos = touches.GetAveragePosition();
            }
          }

          return false;
        } else {
          Distance = Vector2.Distance(touches.GetAveragePosition(), SkillSection[(int)MediSection].StartPos);
        }
      }

      if (Distance < ActiveDistance) {
        return false;
      } else {
        if (!gesture.IsActive) {
          gesture.HintFlag = HintType.Hint;
          gesture.StartPosition = touches.GetAverageStartPosition();
          RaiseHintEvent(gesture);
          gesture.IsActive = true;
        }
      }
    }
    return true;
  }
  ///
  protected override void OnBegin(CustomGesture gesture, TouchManager.IFingerList touches)
  {
    gesture.StartPosition = touches.GetAverageStartPosition();
    gesture.Position = touches.GetAveragePosition();
    gesture.RawPoints.Clear();
    gesture.RawPoints.Add(new Point(0, gesture.Position));
    ///
    CurActiveSection++;
    ///
    SkillSection[(int)CurActiveSection].ReSet();
    if (Section.First == CurActiveSection) {
      SkillSection[(int)CurActiveSection].StartTime = gesture.StartTime;
      SkillSection[(int)CurActiveSection].StartPos = gesture.StartPosition;
    } else {
      SkillSection[(int)CurActiveSection].StartTime = Time.time;
      SkillSection[(int)CurActiveSection].StartPos = touches.GetAveragePosition();
    }
  }

  bool RecognizeCustom(CustomGesture gesture)
  {
    gesture.MatchDistance = 0;
    gesture.MatchScore = 0;
    gesture.RecognizedTemplate = null;
    gesture.NormalizedPoints.Clear();

    if (gesture.RawPoints.Count < 2) {
      return false;
    }

    gesture.NormalizedPoints.AddRange(normalizer.Apply(gesture.RawPoints, NormalizedPointCount));

    float bestDist = float.PositiveInfinity;
    for (int i = 0; i < normalizedTemplates.Count; ++i) {
      NormalizedTemplate template = normalizedTemplates[i];
      float d = 0;
      d = GreedyCloudMatch(gesture.NormalizedPoints, template.Points);
      if (d < bestDist) {
        bestDist = d;
        gesture.RecognizedTemplate = template.Source;
      }
    }

    if (gesture.RecognizedTemplate != null) {
      gesture.MatchDistance = bestDist;
      gesture.MatchScore = Mathf.Max((MaxMatchDistance - bestDist) / MaxMatchDistance, 0.0f);
    }

    if (gesture.MatchScore > 0) {
      if ((int)CurActiveSection >= SectionNumber - 1 || SkillCategory.kNone == gesture.SkillTags) {
        gesture.Recognizer.ResetMode = GestureResetMode.EndOfTouchSequence;
      } else {
        gesture.Recognizer.ResetMode = GestureResetMode.NextFrame;
      }
    } else {
      gesture.Recognizer.ResetMode = GestureResetMode.EndOfTouchSequence;
    }

    return gesture.MatchScore > 0;
  }

  float GreedyCloudMatch(List<Point> points, List<Point> refPoints)
  {
    float e = 0.5f;
    int step = Mathf.FloorToInt(Mathf.Pow(points.Count, 1 - e));
    float min = float.PositiveInfinity;

    for (int i = 0; i < points.Count; i += step) {
      float d1 = CloudDistance(points, refPoints, i);
      float d2 = CloudDistance(refPoints, points, i);
      min = Mathf.Min(min, d1, d2);
    }

    return min;
  }

  static float CloudDistance(List<Point> points1, List<Point> points2, int startIndex)
  {
    int numPoints = points1.Count;
    ResetMatched(numPoints);
#if UNITY_EDITOR
    if (points1.Count != points2.Count) {
      return float.PositiveInfinity;
    }
#endif
    float sum = 0;
    int i = startIndex;
    do {
      int index = -1;
      float minDistance = float.PositiveInfinity;
      for (int j = 0; j < numPoints; ++j) {
        if (!matched[j]) {
          float distance = Vector2.Distance(points1[i].Position, points2[j].Position);
          if (distance < minDistance) {
            minDistance = distance;
            index = j;
          }
        }
      }
      matched[index] = true;
      float weight = 1 - ((i - startIndex + points1.Count) % points1.Count) / points1.Count;
      sum += weight * minDistance;
      i = (i + 1) % points1.Count;
    } while (i != startIndex);
    return sum;
  }

  private static bool[] matched = new bool[NormalizedPointCount];

  static void ResetMatched(int count)
  {
    if (matched.Length < count) {
      matched = new bool[count];
    }
    for (int i = 0; i < count; ++i) {
      matched[i] = false;
    }
  }

  private bool IsTimeOut(CustomGesture gesture)
  {
    if (Time.time - SkillSection[(int)CurActiveSection].StartTime > SkillSection[(int)CurActiveSection].TimeOut) {
      return true;
    }
    return false;
  }

  private bool IsExceedDistance(CustomGesture gesture)
  {
    float distance = Vector2.Distance(gesture.Position, SkillSection[(int)CurActiveSection].StartPos);
    if (distance > SkillSection[(int)CurActiveSection].FireDistance) {
      return true;
    }
    return false;
  }

  protected override GestureRecognitionState OnRecognize(CustomGesture gesture, TouchManager.IFingerList touches)
  {
    if (touches.Count != RequiredFingerCount) {
      if (touches.Count < RequiredFingerCount) {
        if (RecognizeCustom(gesture)) {
          if (SkillCategory.kNone != gesture.SkillTags) {
            gesture.HintFlag = HintType.RSucceed;
          }
          return GestureRecognitionState.Recognized;
        }
        gesture.HintFlag = HintType.RFailure;
        return GestureRecognitionState.Failed;
      }
      gesture.HintFlag = HintType.RFailure;
      return GestureRecognitionState.Failed;
    }
    /// 超时
    if (IsTimeOut(gesture)) {
      gesture.HintFlag = HintType.RFailure;
      return GestureRecognitionState.Failed;
    }
    /// 超出距离
    if (IsExceedDistance(gesture)) {
      if (RecognizeCustom(gesture)) {
        if (SkillCategory.kNone != gesture.SkillTags) {
          gesture.HintFlag = HintType.RSucceed;
        }
        return GestureRecognitionState.Recognized;
      }
      gesture.IsInvalid = true;
      gesture.HintFlag = HintType.RFailure;
      return GestureRecognitionState.Failed;
    }
    // 更新手势位置
    gesture.Position = touches.GetAveragePosition();
    Vector2 lastSamplePos = gesture.RawPoints[gesture.RawPoints.Count - 1].Position;

    float dist = Vector2.SqrMagnitude(gesture.Position - lastSamplePos);
    if (dist > MinDistanceBetweenSamples * MinDistanceBetweenSamples) {
      int strokeId = 0;
      gesture.RawPoints.Add(new Point(strokeId, gesture.Position));
    }

    return GestureRecognitionState.InProgress;
  }

  public override string GetDefaultEventMessageName()
  {
    return string.IsNullOrEmpty(EventMessageName) ? "OnCustomGesture" : EventMessageName;
  }

  protected override int CaclSection()
  {
    return (int)CurActiveSection;
  }

  protected override float CaclTowards(CustomGesture gesture)
  {
    if (null != gesture.RecognizedTemplate && gesture.RecognizedTemplate.isCaclTowards) {
      return CaclTowards(gesture.RawPoints);
    }

    return float.NegativeInfinity;
  }

  protected override SkillCategory CaclSkillTag(CustomGesture gesture)
  {
    float towards = gesture.Towards;
    int angle = (int)(towards / Mathf.PI * 180f);
    int region = 90;
    int tolerance = 35;
    if (null != gesture.RecognizedTemplate) {
      tolerance = gesture.RecognizedTemplate.tolerance;
    }

    SkillCategory skill_flag = SkillCategory.kNone;
    if (isInExtent(region * 1, angle, tolerance)) {
      skill_flag = SkillCategory.kSkillA;
    } else if (isInExtent(region * 2, angle, tolerance)) {
      skill_flag = SkillCategory.kSkillB;
    } else if (isInExtent(region * 3, angle, tolerance)) {
      skill_flag = SkillCategory.kSkillC;
    } else if (isInExtent(region * 4, angle, tolerance)) {
      skill_flag = SkillCategory.kSkillD;
    }

    /// record
    if (Section.First == CurActiveSection) {
      ActiveSkill = skill_flag;
      ActiveTowards = CaclTowards(gesture.RawPoints);
    } else {
      skill_flag = ReCaclSkillCategory(skill_flag, gesture);
    }

    return skill_flag;
  }

  private SkillCategory ReCaclSkillCategory(SkillCategory category, CustomGesture gesture)
  {
    if (Section.Second == CurActiveSection) {
      if (ActiveSkill != category) {
        SkillCategory temp_a = category - 2;
        SkillCategory temp_b = category + 2;
        if (ActiveSkill == temp_a || ActiveSkill == temp_b) {
          return ActiveSkill;
        }
      }
    } else if (Section.Third == CurActiveSection) {
      if (ActiveSkill == category) {
        return ActiveSkill;
      }
    }
    return SkillCategory.kNone;
  }

  private bool isInExtent(int region, int angle, int tolerance)
  {
    int minAngle = region - tolerance;
    int maxAngle = region + tolerance;
    if (360 == region) {
      angle = angle < minAngle ? (angle + 360) : angle;
    }
    if (angle > minAngle && angle < maxAngle) {
      return true;
    }
    return false;
  }

  private Vector3 GetTouchToWorldPoint(Point arg)
  {
    if (null == Camera.main)
      return Vector3.zero;
    Vector3 cur_touch_worldpos = Vector3.zero;
    Vector3 cur_touch_pos = new Vector3(arg.Position.x, arg.Position.y, 0);
    Ray ray = Camera.main.ScreenPointToRay(cur_touch_pos);
    RaycastHit hitInfo;
    //int layermask = 1 << LayerMask.NameToLayer("AirWall");
    //layermask |= 1 << LayerMask.NameToLayer("SceneObjEffect");
    //layermask |= 1 << LayerMask.NameToLayer("SceneObj");
    //layermask = ~layermask;
    int layermask = 1 << LayerMask.NameToLayer("Terrains");
    if (Physics.Raycast(ray, out hitInfo, 200f, layermask)) {
      cur_touch_worldpos = hitInfo.point;
    }
    return cur_touch_worldpos;
  }

  private float CaclTowards(List<Point> points)
  {
    if (points.Count > 1) {
      Vector3 start_pos = GetTouchToWorldPoint(points[0]);
      Vector3 end_pos = GetTouchToWorldPoint(points[points.Count - 1]);
      return Geometry.GetYAngle(new ScriptRuntime.Vector2(start_pos.x, start_pos.z), new ScriptRuntime.Vector2(end_pos.x, end_pos.z));
    }
    return float.NegativeInfinity;
  }
}