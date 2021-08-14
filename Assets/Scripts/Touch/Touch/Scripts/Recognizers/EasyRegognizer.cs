using UnityEngine;
using System.Collections.Generic;
using DashFire;

public class EasySectionArg {
  public enum SkillMode : int {
    Normal = 0,
    Qte = 1,
  }
  /// QTE
  public enum UIMode : int {
    Horizontal = 0,
    Vertical = 1,
  }

  public float HintDistance = 20f;
  public float ToleranceDistance = 40f;
  public float FireDistance = 50f;
  public float QteActiveDistance = 80f;
  public float QteFireDistance = 10f;

  public float TimeOut = 2000f;
  public Vector2 StartPos = Vector2.zero;
  public float StartTime = 0;
  public Vector2 EndPos = Vector2.zero;
  public SkillMode skillmode = SkillMode.Normal;

  public float PathLength = 0;
  ///
  public EasySectionArg()
  {
    HintDistance = 20f;
    ToleranceDistance = 40f;
    FireDistance = 50f;
    QteActiveDistance = 80f;
    QteFireDistance = 10f;
    TimeOut = 2000f;
    StartPos = Vector2.zero;
    StartTime = 0;
    EndPos = Vector2.zero;
    skillmode = SkillMode.Normal;
    PathLength = 0;
  }
  public EasySectionArg(float hintDistance, float toleranceDistance, float fireDistance, float qteActiveDistance, float qteFireDistance, float timeOut)
  {
    HintDistance = hintDistance;
    ToleranceDistance = toleranceDistance;
    FireDistance = fireDistance;
    QteActiveDistance = qteActiveDistance;
    QteFireDistance = qteFireDistance;
    TimeOut = timeOut;
    StartPos = Vector2.zero;
    StartTime = 0;
    EndPos = Vector2.zero;
    skillmode = SkillMode.Normal;
    PathLength = 0;
  }
  public void ReSet()
  {
    StartPos = Vector2.zero;
    StartTime = 0;
    EndPos = Vector2.zero;
    skillmode = SkillMode.Normal;
    PathLength = 0;
  }
}

public class EasyGesture : Gesture 
{
}

public class EasyRegognizer : GestureRecognizerBase<EasyGesture> 
{
  private const int SectionNumber = 3;
  public List<EasySectionArg> SkillSection = new List<EasySectionArg>(SectionNumber);
  private static Section CurActiveSection = Section.Invalid;
  private SkillCategory ActiveSkill = SkillCategory.kNone;
  private float ActiveTowards = float.NegativeInfinity;
  private Vector2 LastInputPos = Vector2.zero;
  private Vector2 WasInputPos = Vector2.zero;
  private bool QteEnable = true;

  public float HintDistance = 20f;
  public float ToleranceDistance = 40f;
  public float FireDistance = 50f;
  public float QteActiveDistance = 80f;
  public float QteFireDistance = 10f;
  public float TimeOut = 2000f;
  public float QteSimilarityDistance = 1f;

  public EasyRegognizer()
  {
  }

  protected override void OnInit()
  {
    SkillSection.Clear();
    for (int i = 0; i < SectionNumber; i++) {
      EasySectionArg arg = new EasySectionArg(HintDistance, ToleranceDistance, FireDistance, QteActiveDistance, QteFireDistance, TimeOut);
      SkillSection.Add(arg);
    }
  }

  protected override void Refresh(EasyGesture gesture)
  {
    gesture.IsInvalid = false;
  }

  protected override void Reset(EasyGesture gesture, bool isPressed)
  {
    gesture.ClusterId = 0;
    gesture.IsActive = false;
    gesture.SelectedID = -1;
    gesture.SkillTags = SkillCategory.kNone;
    gesture.HintFlag = HintType.None;
    gesture.SectionNum = -1;
    LastInputPos = Vector2.zero;
    WasInputPos = Vector2.zero;
    QteEnable = true;

    if (isPressed) {
      if ((int)CurActiveSection >= SectionNumber - 1) {
        gesture.IsInvalid = true;
      }
    } else {
      if (CurActiveSection != Section.Invalid) {
        ReleaseFingers(gesture);
        gesture.Fingers.Clear();
        CurActiveSection = Section.Invalid;
        ActiveSkill = SkillCategory.kNone;
        ActiveTowards = float.NegativeInfinity;
        gesture.IsHint = false;
        for (int i = 0; i < SectionNumber; i++) {
          SkillSection[i].ReSet();
        }
      }
    }
    gesture.State = GestureRecognitionState.Ready;
  }

  protected override void Awake()
  {
    base.Awake();
  }

  private bool DetectionQTE(Section section, Vector2 sp, Vector2 cp, EasySectionArg.UIMode mode)
  {
    if (!QteEnable) {
      return false;
    }
    if (EasySectionArg.UIMode.Horizontal == mode) {
      if (Mathf.Abs(cp.y - sp.y) > SkillSection[(int)section].QteActiveDistance) {
        SkillSection[(int)section].skillmode = EasySectionArg.SkillMode.Qte;
        return true;
      }
    } else {
      if (Mathf.Abs(cp.x - sp.x) > SkillSection[(int)section].QteActiveDistance) {
        SkillSection[(int)section].skillmode = EasySectionArg.SkillMode.Qte;
        return true;
      }
    }
    return false;
  }

  private Vector2 CalcSectionStartPos(EasyGesture gesture, Vector2 cp, Vector2 lp, Section section, Vector2 startpos)
  {
    ///Qte
    SkillController skill_ctrl = null;
    if (null != skill_ctrl) {
      List<SkillNode> nodeInfo = skill_ctrl.QuerySkillQE(ActiveSkill, (int)section);
      if (null != nodeInfo && nodeInfo.Count > 0) {
        QteEnable = true;
      } else {
        QteEnable = false;
      }
    }
    ///
    Vector2 start_pos = Vector2.zero;
    if (Section.Second == section) {
      switch (ActiveSkill) {
        case SkillCategory.kSkillA:
          if (cp.x < lp.x) {
            start_pos = cp;
          }
          if (DetectionQTE(section, startpos, cp, EasySectionArg.UIMode.Horizontal)) {
            start_pos = cp;
          }
          break;
        case SkillCategory.kSkillB:
          if (cp.y > lp.y) {
            start_pos = cp;
          }
          if (DetectionQTE(section, startpos, cp, EasySectionArg.UIMode.Vertical)) {
            start_pos = cp;
          }
          break;
        case SkillCategory.kSkillC:
          if (cp.x > lp.x) {
            start_pos = cp;
          }
          if (DetectionQTE(section, startpos, cp, EasySectionArg.UIMode.Horizontal)) {
            start_pos = cp;
          }
          break;
        case SkillCategory.kSkillD:
          if (cp.y < lp.y) {
            start_pos = cp;
          }
          if (DetectionQTE(section, startpos, cp, EasySectionArg.UIMode.Vertical)) {
            start_pos = cp;
          }
          break;
      }
    } else if (Section.Third == section) {
      switch (ActiveSkill) {
        case SkillCategory.kSkillA:
          if (cp.x > lp.x) {
            start_pos = cp;
          }
          if (DetectionQTE(section, startpos, cp, EasySectionArg.UIMode.Horizontal)) {
            start_pos = cp;
          }
          break;
        case SkillCategory.kSkillB:
          if (cp.y < lp.y) {
            start_pos = cp;
          }
          if (DetectionQTE(section, startpos, cp, EasySectionArg.UIMode.Vertical)) {
            start_pos = cp;
          }
          break;
        case SkillCategory.kSkillC:
          if (cp.x < lp.x) {
            start_pos = cp;
          }
          if (DetectionQTE(section, startpos, cp, EasySectionArg.UIMode.Horizontal)) {
            start_pos = cp;
          }
          break;
        case SkillCategory.kSkillD:
          if (cp.y > lp.y) {
            start_pos = cp;
          }
          if (DetectionQTE(section, startpos, cp, EasySectionArg.UIMode.Vertical)) {
            start_pos = cp;
          }
          break;
      }
    }
    return start_pos;
  }

  protected override bool CanBegin(EasyGesture gesture, TouchManager.IFingerList touches)
  {
    if (touches.Count != RequiredFingerCount) {
      return false;
    }
    if ("MouseInput" == TouchManager.Instance.InputProvider.name) {
      if (!Input.GetMouseButton(0)) {
        return false;
      }
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

    Section medi_section = (CurActiveSection == Section.Invalid ? Section.First : CurActiveSection + 1);
    float hint_distance = SkillSection[(int)medi_section].HintDistance;
    float tolerance_distance = SkillSection[(int)medi_section].ToleranceDistance;
    if (tolerance_distance > 0) {
      float cur_distance = 0;
      if (medi_section == Section.First) {
        gesture.IsInvalid = false;
        cur_distance = touches.GetAverageDistanceFromStart();
      } else if (medi_section == Section.Second || medi_section == Section.Third) {
        if (Vector2.zero == SkillSection[(int)medi_section].StartPos) {
          if (Vector2.zero == LastInputPos) {
            LastInputPos = touches.GetAveragePosition();
            return false;
          }
          Vector2 cp = touches.GetAveragePosition();
          Vector2 lp = LastInputPos;
          SkillSection[(int)medi_section].StartPos = CalcSectionStartPos(gesture, cp, lp, medi_section, SkillSection[(int)(medi_section - 1)].EndPos);
          LastInputPos = cp;
          ///
          if (Vector2.zero != WasInputPos) {
            SkillSection[(int)CurActiveSection].PathLength += Vector2.Distance(WasInputPos, cp);
          } else {
            SkillSection[(int)CurActiveSection].PathLength += Vector2.Distance(SkillSection[(int)(medi_section-1)].EndPos, cp);
          }
          WasInputPos = cp;
          return false;
        } else {
          cur_distance = Vector2.Distance(touches.GetAveragePosition(), SkillSection[(int)medi_section].StartPos);
        }
      }

      if (cur_distance < hint_distance) {
        return false;
      } else {
        if (!gesture.IsHint) {
          gesture.HintFlag = HintType.Hint;
          gesture.StartPosition = touches.GetAverageStartPosition();
          RaiseHintEvent(gesture);
          gesture.IsHint = true;
        }
      }
      if (cur_distance < tolerance_distance && EasySectionArg.SkillMode.Normal == SkillSection[(int)medi_section].skillmode) {
        return false;
      } else {
        if (!gesture.IsActive) {
          gesture.IsActive = true;
        }
      }
    }
    return true;
  }

  protected override void OnBegin(EasyGesture gesture, TouchManager.IFingerList touches)
  {
    gesture.StartPosition = touches.GetAverageStartPosition();
    gesture.Position = touches.GetAveragePosition();
    CurActiveSection++;
    //SkillSection[(int)CurActiveSection].ReSet();
    if (Section.First == CurActiveSection) {
      SkillSection[(int)CurActiveSection].StartTime = gesture.StartTime;
      SkillSection[(int)CurActiveSection].StartPos = gesture.StartPosition;
    } else {
      SkillSection[(int)CurActiveSection].StartTime = Time.time;
      SkillSection[(int)CurActiveSection].StartPos = touches.GetAveragePosition();
    }
  }

  bool RecognizeCustom(EasyGesture gesture)
  {
    float fire_distance = SkillSection[(int)CurActiveSection].FireDistance;
    if (QteEnable && EasySectionArg.SkillMode.Qte == SkillSection[(int)CurActiveSection].skillmode) {
      fire_distance = SkillSection[(int)CurActiveSection].QteFireDistance;
    }
    float cur_distance = Vector2.Distance(SkillSection[(int)CurActiveSection].StartPos, gesture.Position);
    if (cur_distance > fire_distance) {
      if ((int)CurActiveSection > SectionNumber - 1) {
        gesture.Recognizer.ResetMode = GestureResetMode.EndOfTouchSequence;
      } else {
        gesture.Recognizer.ResetMode = GestureResetMode.NextFrame;
      }
      SkillSection[(int)CurActiveSection].EndPos = gesture.Position;
    } else {
      gesture.Recognizer.ResetMode = GestureResetMode.EndOfTouchSequence;
    }
    return cur_distance > fire_distance;
  }

  private bool IsTimeOut(EasyGesture gesture)
  {
    if (Time.time - SkillSection[(int)CurActiveSection].StartTime > SkillSection[(int)CurActiveSection].TimeOut) {
      return true;
    }
    return false;
  }

  private bool IsExceedDistance(EasyGesture gesture)
  {
    float distance = Vector2.Distance(gesture.Position, SkillSection[(int)CurActiveSection].StartPos);
    float fireDistance = SkillSection[(int)CurActiveSection].FireDistance;
    if (QteEnable && EasySectionArg.SkillMode.Qte == SkillSection[(int)CurActiveSection].skillmode) {
      fireDistance = SkillSection[(int)CurActiveSection].QteFireDistance;
    }
    if (distance > fireDistance) {
      return true;
    }
    return false;
  }

  protected override GestureRecognitionState OnRecognize(EasyGesture gesture, TouchManager.IFingerList touches)
  {
    if (touches.Count != RequiredFingerCount) {
      if (touches.Count < RequiredFingerCount) {
        if (((int)CurActiveSection >= 0) && ((int)CurActiveSection <= SectionNumber - 1)) {
          /*
          if (RecognizeCustom(gesture)) {
            return GestureRecognitionState.Recognized;
          }
          gesture.HintFlag = HintType.RFailure;
          RaiseHintEvent(gesture);
          return GestureRecognitionState.Failed;*/
        }
        return GestureRecognitionState.Failed;
      }
      return GestureRecognitionState.Failed;
    }
    /// ³¬Ê±
    /*
    if (IsTimeOut(gesture)) {
      gesture.HintFlag = HintType.RFailure;
      return GestureRecognitionState.Failed;
    }*/
    /// ³¬³ö¾àÀë
    if (IsExceedDistance(gesture)) {
      if (RecognizeCustom(gesture)) {
        SkillCategory  skill_tags = CaclSkillTag(gesture);
        if (SkillCategory.kSkillQ == skill_tags || SkillCategory.kSkillE == skill_tags) {
          gesture.IsInvalid = true;
        }
        if (0 == SkillSection[(int)CurActiveSection].PathLength) {
          SkillSection[(int)CurActiveSection].PathLength = Vector2.Distance(SkillSection[(int)CurActiveSection].StartPos, touches.GetAveragePosition());
        } else {
          SkillSection[(int)CurActiveSection].PathLength += Vector2.Distance(WasInputPos, gesture.Position);
        }
        return GestureRecognitionState.Recognized;
      }

      gesture.IsInvalid = true;
      return GestureRecognitionState.Failed;
    }

    if (CurActiveSection > 0) {
      CalcSectionStartPos(gesture, touches.GetAveragePosition(), LastInputPos, CurActiveSection, SkillSection[(int)(CurActiveSection - 1)].EndPos);
    }

    gesture.Position = touches.GetAveragePosition();
    if (Vector2.zero == WasInputPos) {
      SkillSection[(int)CurActiveSection].PathLength += Vector2.Distance(SkillSection[(int)CurActiveSection].StartPos, gesture.Position);
    } else {
      SkillSection[(int)CurActiveSection].PathLength += Vector2.Distance(WasInputPos, gesture.Position);
    }
    LastInputPos = gesture.Position;
    WasInputPos = gesture.Position;
    return GestureRecognitionState.InProgress;
  }

  public override string GetDefaultEventMessageName()
  {
    return string.IsNullOrEmpty(EventMessageName) ? "OnEasyGesture" : EventMessageName;
  }

  protected override int CaclSection()
  {
    return (int)CurActiveSection;
  }

  protected override float CaclTowards(EasyGesture gesture)
  {
    if (null != gesture) {
      Vector3 start_pos = GetTouchToWorldPoint(SkillSection[(int)CurActiveSection].StartPos);
      Vector3 end_pos = GetTouchToWorldPoint(gesture.Position);
      return Geometry.GetYAngle(new ScriptRuntime.Vector2(start_pos.x, start_pos.z), new ScriptRuntime.Vector2(end_pos.x, end_pos.z));
    }
    return float.NegativeInfinity;
  }

  protected override SkillCategory CaclSkillTag(EasyGesture gesture)
  {
    float towards = Geometry.GetYAngle(new ScriptRuntime.Vector2(SkillSection[(int)CurActiveSection].StartPos.x,
       SkillSection[(int)CurActiveSection].StartPos.y), new ScriptRuntime.Vector2(gesture.Position.x, gesture.Position.y));
    float angle = (towards / Mathf.PI * 180f);
    float region = 90;
    float tolerance = 45;

    SkillCategory skill_flag = SkillCategory.kNone;
    if (isInExtent(region * 1, angle, tolerance)) {
      skill_flag = SkillCategory.kSkillA;
    } else if (isInExtent(region * 2, angle, tolerance)) {
      skill_flag = SkillCategory.kSkillB;
    } else if (isInExtent(region * 3, angle, tolerance)) {
      skill_flag = SkillCategory.kSkillC;
    } else {
      skill_flag = SkillCategory.kSkillD;
    }

    /// record
    if (Section.First == CurActiveSection) {
      ActiveSkill = skill_flag;
      ActiveTowards = CaclTowards(gesture);
    } else {
      skill_flag = ReCaclSkillCategory(skill_flag);
    }

    if (QteEnable && (SkillCategory.kSkillQ == skill_flag || SkillCategory.kSkillE == skill_flag)) {
      float line_dis = Vector2.Distance(gesture.Position, gesture.StartPosition);
      float reality_dis = SkillSection[(int)(CurActiveSection - 1)].PathLength 
        + SkillSection[(int)(CurActiveSection)].PathLength
        + QteActiveDistance;
      if (reality_dis < line_dis + QteSimilarityDistance) {
        skill_flag = SkillCategory.kNone;
      }
    }

    return skill_flag;
  }

  private SkillCategory ReCaclSkillCategory(SkillCategory category)
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

    ///QTE
    if (QteEnable) {
      switch (ActiveSkill) {
        case SkillCategory.kSkillA:
        case SkillCategory.kSkillC:
          if (SkillCategory.kSkillD == category) {
            return SkillCategory.kSkillQ;
          }
          else if (SkillCategory.kSkillB == category) {
            return SkillCategory.kSkillE;
          }
          break;
        case SkillCategory.kSkillB:
        case SkillCategory.kSkillD:
          if (SkillCategory.kSkillC == category) {
            return SkillCategory.kSkillQ;
          }
          else if (SkillCategory.kSkillA == category) {
            return SkillCategory.kSkillE;
          }
          break;
        default:
          break;
      }
    }

    return SkillCategory.kNone;
  }

  private bool isInExtent(float region, float angle, float tolerance)
  {
    float minAngle = region - tolerance;
    float maxAngle = region + tolerance;
    if (angle > minAngle && angle < maxAngle) {
      return true;
    }
    return false;
  }

  private Vector3 GetTouchToWorldPoint(Vector2 arg)
  {
    if (null == Camera.main)
      return Vector3.zero;
    Vector3 cur_touch_worldpos = Vector3.zero;
    Vector3 cur_touch_pos = new Vector3(arg.x, arg.y, 0);
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
}