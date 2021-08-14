using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DashFire;

/// Enums
public enum GestureRecognitionState 
{
  Ready,
  Started,
  InProgress,
  Failed,
  Ended,
  Recognized = Ended,
  ///
  FailAndRetry,
}

public enum GestureResetMode 
{
  Default,
  NextFrame,
  EndOfTouchSequence,
}

public enum HintType 
{
  None,
  Hint,
  RSucceed,
  RFailure,
}

public abstract class Gesture 
{
  public delegate void EventHandler(Gesture gesture);
  public EventHandler OnStateChanged;

  int clusterid = 0;
  public int ClusterId
  {
    get
    {
      return clusterid;
    }
    set
    {
      clusterid = value;
    }
  }

  GestureRecognizer recognizer;
  float startTime = 0;
  Vector2 startPosition = Vector2.zero;
  Vector2 position = Vector2.zero;
  GestureRecognitionState state = GestureRecognitionState.Ready;
  GestureRecognitionState prevState = GestureRecognitionState.Ready;
  TouchManager.FingerList fingers = new TouchManager.FingerList();
  float towards = float.NegativeInfinity;
  SkillCategory skillTag = SkillCategory.kNone;
  bool isHint = false;
  bool isActive = false;
  int selectedID = -1;
  HintType hintFlag = HintType.None;

  bool isInvalid = false;
  int sectionNum = -1;

  public static implicit operator bool(Gesture gesture)
  {
    return gesture != null;
  }

  public int SectionNum
  {
    get
    {
      return sectionNum;
    }
    set
    {
      sectionNum = value;
    }
  }

  public bool IsInvalid
  {
    get
    {
      return isInvalid;
    }
    set
    {
      isInvalid = value;
    }
  }

  public HintType HintFlag
  {
    get
    {
      return hintFlag;
    }
    set
    {
      hintFlag = value;
    }
  }

  public int SelectedID
  {
    get
    {
      return selectedID;
    }
    set
    {
      selectedID = value;
    }
  }

  public bool IsHint
  {
    get
    {
      return isHint;
    }
    set
    {
      isHint = value;
    }
  }

  public bool IsActive
  {
    get
    {
      return isActive;
    }
    set
    {
      isActive = value;
    }
  }

  public TouchManager.FingerList Fingers
  {
    get
    {
      return fingers;
    }
    set
    {
      fingers = value;
    }
  }

  public float Towards
  {
    get
    {
      return towards;
    }
    set
    {
      towards = value;
    }
  }

  public SkillCategory SkillTags
  {
    get
    {
      return skillTag;
    }
    set
    {
      skillTag = value;
    }
  }

  public GestureRecognizer Recognizer
  {
    get
    {
      return recognizer;
    }
    set
    {
      recognizer = value;
    }
  }

  public float StartTime
  {
    get
    {
      return startTime;
    }
    set
    {
      startTime = value;
    }
  }

  public Vector2 StartPosition
  {
    get
    {
      return startPosition;
    }
    set
    {
      startPosition = value;
    }
  }

  public Vector2 Position
  {
    get
    {
      return position;
    }
    set
    {
      position = value;
    }
  }

  public GestureRecognitionState State
  {
    get
    {
      return state;
    }
    set
    {
      if (state != value) {
        prevState = state;
        state = value;
        if (OnStateChanged != null) {
          OnStateChanged(this);
        }
      }
    }
  }

  public GestureRecognitionState PreviousState
  {
    get
    {
      return prevState;
    }
  }

  public float ElapsedTime
  {
    get
    {
      return Time.time - StartTime;
    }
  }

  public Vector3 GetTouchToAirWallWorldPoint()
  {
    if (null == Camera.main)
      return Vector3.zero;
    Vector3 cur_touch_worldpos = Vector3.zero;
    Vector3 cur_touch_pos = new Vector3(position.x, position.y, 0);
    Ray ray = Camera.main.ScreenPointToRay(cur_touch_pos);
    RaycastHit hitInfo;
    //int layermask = 1 << LayerMask.NameToLayer("AirWall");
    //layermask |= 1 << LayerMask.NameToLayer("SceneObjEffect");
    //layermask |= 1 << LayerMask.NameToLayer("SceneObj");
    //layermask = ~layermask;
    int layermask = 1 << LayerMask.NameToLayer("Terrains");
    if (Physics.Raycast(ray, out hitInfo, 200f, layermask)) {
      cur_touch_worldpos = hitInfo.point;
      GameObject go = DashFire.LogicSystem.PlayerSelf;
      if (null != go) {
        Vector3 srcPos = go.transform.position;
        Vector3 targetPos = cur_touch_worldpos;
        float length = Vector3.Distance(srcPos, targetPos);
        RaycastHit airWallHitInfo = new RaycastHit();
        int airWallLayermask = 1 << LayerMask.NameToLayer("AirWall");
        Vector3 direction = (targetPos - srcPos).normalized;
        if (Physics.Raycast(go.transform.position, direction, out airWallHitInfo, length, airWallLayermask)) {
          BoxCollider bc = airWallHitInfo.collider.gameObject.GetComponent<BoxCollider>();
          if (null != bc && !bc.isTrigger) {
            cur_touch_worldpos = airWallHitInfo.point;
          }
        }
      }
    }
    return cur_touch_worldpos;
  }

  public Vector3 GetTouchToWorldPoint()
  {
    if (null == Camera.main)
      return Vector3.zero;
    Vector3 cur_touch_worldpos = Vector3.zero;
    Vector3 cur_touch_pos = new Vector3(position.x, position.y, 0);
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

  public Vector3 GetStartTouchToWorldPoint()
  {
    if (null == Camera.main)
      return Vector3.zero;
    float skill_blear_radius = 0.5f;
    Vector3 start_touch_worldpos = Vector3.zero;
    Vector3 start_touch_pos = new Vector3(startPosition.x, startPosition.y, 0);
    Ray ray = Camera.main.ScreenPointToRay(start_touch_pos);
    RaycastHit hitInfo;
    //int layermask = (1 << LayerMask.NameToLayer("AirWall"));
    //layermask |= 1 << LayerMask.NameToLayer("Character");
    //layermask |= 1 << LayerMask.NameToLayer("Player");
    //layermask |= 1 << LayerMask.NameToLayer("SceneObjEffect");
    //layermask |= 1 << LayerMask.NameToLayer("SceneObj");
    //layermask = ~layermask;
    int layermask = (1 << LayerMask.NameToLayer("Terrains"))
      | (1 << LayerMask.NameToLayer("Character"));
    SkillController player_skill_ctrl = null;
    if (null != player_skill_ctrl) {
      SkillInputData skill_input_data = player_skill_ctrl.GetSkillInputData(SkillTags);
      if (null != skill_input_data) {
        skill_blear_radius = skill_input_data.targetChooseRange;
      }
    }
    if (Physics.Raycast(ray, out hitInfo, 200f, layermask)) {
      start_touch_worldpos = hitInfo.point;
      GameObject go = DashFire.LogicSystem.PlayerSelf;
      if (null != go) {
        Vector3 srcPos = go.transform.position;
        Vector3 targetPos = start_touch_worldpos;
        float length = Vector3.Distance(srcPos, targetPos);
        RaycastHit airWallHitInfo = new RaycastHit();
        int airWallLayermask = 1 << LayerMask.NameToLayer("AirWall");
        Vector3 direction = (targetPos - srcPos).normalized;
        if (Physics.Raycast(go.transform.position, direction, out airWallHitInfo, length, airWallLayermask)) {
          BoxCollider bc = airWallHitInfo.collider.gameObject.GetComponent<BoxCollider>();
          if (null != bc && !bc.isTrigger) {
            start_touch_worldpos = airWallHitInfo.point;
          }
        }
      }
      Collider[] hitObjs = Physics.OverlapSphere(start_touch_worldpos, skill_blear_radius, (1 << LayerMask.NameToLayer("Character")));
      if (hitObjs.Length > 0) {
        start_touch_worldpos = hitObjs[0].gameObject.transform.position;
      }
    }

    return start_touch_worldpos;
  }

  public int GetTouchObjID()
  {
    if (null == Camera.main)
      return -1;
    int object_id = -1;
    float skill_blear_radius = 0.5f;
    Vector3 touch_pos = new Vector3(position.x, position.y, 0);
    Ray ray = Camera.main.ScreenPointToRay(touch_pos);
    RaycastHit hitInfo;
    GameObject hitGameObj = null;
    int layermask = (1 << LayerMask.NameToLayer("Terrains"))
      | (1 << LayerMask.NameToLayer("Character"));
    SkillController player_skill_ctrl = null;
    if (null != player_skill_ctrl) {
      SkillInputData skill_input_data = player_skill_ctrl.GetSkillInputData(SkillTags);
      if (null != skill_input_data) {
        skill_blear_radius = skill_input_data.targetChooseRange;
      }
    }
    if (Physics.Raycast(ray, out hitInfo, 200f, layermask)) {
      Collider[] hitObjs = Physics.OverlapSphere(hitInfo.point, skill_blear_radius, (1 << LayerMask.NameToLayer("Character")));
      if (hitObjs.Length > 0) {
        hitGameObj = hitObjs[0].gameObject;
        if (null != hitGameObj) {
          SharedGameObjectInfo selfInfo = LogicSystem.PlayerSelfInfo;
          SharedGameObjectInfo targetInfo = LogicSystem.GetSharedGameObjectInfo(hitGameObj);
          if (null != targetInfo && null!=selfInfo) {
            // camp
            if(DashFire.CharacterInfo.GetRelation(selfInfo.CampId,targetInfo.CampId)==DashFire.CharacterRelation.RELATION_ENEMY && targetInfo.Blood>0) {
              object_id = targetInfo.m_LogicObjectId;
            }
          }
        }
      }
    }

    if (-1 != object_id && null != hitGameObj) {
      GfxSystem.PublishGfxEvent("Op_InputEffect", "Input", GestureEvent.OnSingleTap, hitGameObj.transform.position.x, hitGameObj.transform.position.y, hitGameObj.transform.position.z, true, false);
    }

    return object_id;
  }
}

public abstract class GestureRecognizerBase<T> : GestureRecognizer where T : Gesture, new() 
{
  List<T> gestures;

  public delegate void GestureEventHandler(T gesture);
  public event GestureEventHandler OnGesture;

  protected override void Start()
  {
    base.Start();
    InitGestures();
  }

  protected override void OnEnable()
  {
    base.OnEnable();
#if UNITY_EDITOR
    InitGestures();
#endif
  }

  protected virtual void OnInit()
  {
  }

  void InitGestures()
  {
    if (gestures == null) {
      gestures = new List<T>();
      for (int i = 0; i < MaxSimultaneousGestures; ++i) {
        AddGesture();
      }
    }
    OnInit();
  }

  protected T AddGesture()
  {
    T gesture = CreateGesture();
    gesture.Recognizer = this;
    gesture.OnStateChanged += OnStateChanged;
    gestures.Add(gesture);
    return gesture;
  }

  public List<T> Gestures
  {
    get
    {
      return gestures;
    }
  }

  protected virtual bool CanBegin(T gesture, TouchManager.IFingerList touches)
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
    if (UICamera.lastHit.collider != null) {
      return false;
    }
    return true;
  }

  protected abstract void OnBegin(T gesture, TouchManager.IFingerList touches);

  protected abstract GestureRecognitionState OnRecognize(T gesture, TouchManager.IFingerList touches);

  protected virtual int CaclSection()
  {
    return -1;
  }

  protected virtual float CaclTowards(T gesture)
  {
    return float.NegativeInfinity;
  }

  protected virtual SkillCategory CaclSkillTag(T gesture)
  {
    return SkillCategory.kNone;
  }

  protected virtual T CreateGesture()
  {
    return new T();
  }

  public override System.Type GetGestureType()
  {
    return typeof(T);
  }

  protected virtual void OnStateChanged(Gesture sender)
  {
    T gesture = (T)sender;
    if (gesture.State == GestureRecognitionState.Recognized) {
      GesturePreprocess(gesture);
      RaiseEvent(gesture);
    }
  }

  private void GesturePreprocess(Gesture gesture)
  {
    if(null != gesture && null != gesture.Recognizer) {
      if ("OnSingleTap" == gesture.Recognizer.EventMessageName
        || "OnDoubleTap" == gesture.Recognizer.EventMessageName) {
        int selected_id = gesture.GetTouchObjID();
        if (selected_id >= 0) {
          gesture.SelectedID = selected_id;
        }
      }
      if ("OnLongPress" == gesture.Recognizer.EventMessageName) {
        TouchManager.Instance.GestureRecognizerSwitch(false);
      }
    }
  }

  protected virtual T FindFreeGesture()
  {
    return gestures.Find(g => g.State == GestureRecognitionState.Ready);
  }

  protected virtual void Refresh(T gesture)
  {
    gesture.IsInvalid = false;
  }

  protected virtual void Reset(T gesture, bool isPressed = false)
  {
    ReleaseFingers(gesture);

    gesture.ClusterId = 0;
    gesture.Fingers.Clear();
    gesture.IsHint = false;
    gesture.IsActive = false;
    gesture.SelectedID = -1;
    gesture.SectionNum = -1;
    gesture.HintFlag = HintType.None;
    gesture.State = GestureRecognitionState.Ready;
  }

  /// Updates
  static TouchManager.FingerList tempTouchList = new TouchManager.FingerList();
  public virtual void FixedUpdate()
  {
    if (IsExclusive) {
      UpdateExclusive();
    } else {
      UpdatePerFinger();
    }
  }

  void UpdateExclusive()
  {
    T gesture = gestures[0];
    TouchManager.IFingerList touches = TouchManager.Touches;
    if (gesture.State == GestureRecognitionState.Ready) {
      if (CanBegin(gesture, touches)) {
        Begin(gesture, 0, touches);
      }
    }
    UpdateGesture(gesture, touches);
    if ("OnEasyGesture" == gesture.Recognizer.EventMessageName) {
      if (touches.Count <= 0) {
        Reset(gesture, false);
      }
    }
  }

  void UpdatePerFinger()
  {
    for (int i = 0; i < TouchManager.Instance.MaxFingers && i < MaxSimultaneousGestures; ++i) {
      TouchManager.Finger finger = TouchManager.GetFinger(i);
      T gesture = gestures[i];
      TouchManager.FingerList touches = tempTouchList;
      touches.Clear();

      if (finger.IsDown) {
        touches.Add(finger);
      }
      if (gesture.State == GestureRecognitionState.Ready) {
        if (CanBegin(gesture, touches)) {
          Begin(gesture, 0, touches);
        }
      }
      UpdateGesture(gesture, touches);
      if ("OnEasyGesture" == gesture.Recognizer.EventMessageName) {
        if (touches.Count <= 0) {
          Reset(gesture, false);
        }
      }
    }
  }

  protected void ReleaseFingers(T gesture)
  {
    for (int i = 0; i < gesture.Fingers.Count; ++i) {
      Release(gesture.Fingers[i]);
    }
  }

  void Begin(T gesture, int clusterId, TouchManager.IFingerList touches)
  {
    gesture.ClusterId = clusterId;
    gesture.StartTime = Time.time;

#if UNITY_EDITOR
    if (gesture.Fingers.Count > 0) {
      //Debug.LogWarning("Begin Gesture Error");
    }
#endif

    for (int i = 0; i < touches.Count; ++i) {
      TouchManager.Finger finger = touches[i];
      gesture.Fingers.Add(finger);
      Acquire(finger);
    }

    OnBegin(gesture, touches);
    gesture.State = GestureRecognitionState.Started;
  }

  protected virtual TouchManager.IFingerList GetTouches(T gesture)
  {
    return TouchManager.Touches;
  }

  protected virtual void UpdateGesture(T gesture, TouchManager.IFingerList touches)
  {
    if (gesture.State == GestureRecognitionState.Ready) {
      return;
    }
    if (gesture.State == GestureRecognitionState.Started) {
      gesture.State = GestureRecognitionState.InProgress;
    }

    switch (gesture.State) {
      case GestureRecognitionState.InProgress: {
          GestureRecognitionState newState = OnRecognize(gesture, touches);
          if (newState == GestureRecognitionState.FailAndRetry) {
            gesture.State = GestureRecognitionState.Failed;
            int clusterId = gesture.ClusterId;
            Reset(gesture);
            if (CanBegin(gesture, touches)) {
              Begin(gesture, clusterId, touches);
            }
          }
          else {
            gesture.State = newState;
          }
        }
        break;
      case GestureRecognitionState.Recognized:
      case GestureRecognitionState.Failed: {
          if (ResetMode == GestureResetMode.NextFrame || (ResetMode == GestureResetMode.EndOfTouchSequence && touches.Count == 0)) {
            if ("OnEasyGesture" == gesture.Recognizer.EventMessageName) {
              if (GestureRecognitionState.Failed == gesture.State) {
                gesture.HintFlag = HintType.RFailure;
                RaiseHintEvent(gesture);
              } else {
                if (SkillCategory.kNone != gesture.SkillTags) {
                  gesture.HintFlag = HintType.RSucceed;
                  RaiseHintEvent(gesture);
                }
              }
            }

            if (touches.Count == 0) {
              Reset(gesture, false);
              Refresh(gesture);
            } else {
              Reset(gesture, true);
            }
          }
        }
        break;
      default:
        Debug.LogError("UpdateGesture Failing");
        gesture.State = GestureRecognitionState.Failed;
        break;
    }
  }

  protected void RaiseHintEvent(T gesture)
  {
    TouchManager.FireHintEvent(gesture);
  }

  protected void RaiseEvent(T gesture)
  {
    if (OnGesture != null) {
      OnGesture(gesture);
    }

    gesture.SectionNum = CaclSection();
    gesture.Towards = CaclTowards(gesture);
    if ("OnEasyGesture" == gesture.Recognizer.EventMessageName) {
      gesture.SkillTags = CaclSkillTag(gesture);
    }
    TouchManager.FireEvent(gesture);
  }
}

public abstract class GestureRecognizer : MonoBehaviour 
{
  protected static readonly TouchManager.IFingerList EmptyFingerList = new TouchManager.FingerList();

  public int requiredFingerCount = 1;
  public DistanceUnit DistanceUnit = DistanceUnit.Centimeters;
  public int MaxSimultaneousGestures = 1;
  public GestureResetMode ResetMode = GestureResetMode.NextFrame;
  public string EventMessageName;
  public bool IsExclusive = true;

  public virtual int RequiredFingerCount
  {
    get
    {
      return requiredFingerCount;
    }
    set
    {
      requiredFingerCount = value;
    }
  }

  public virtual bool SupportFingerClustering
  {
    get
    {
      return true;
    }
  }

  public virtual GestureResetMode GetDefaultResetMode()
  {
    return GestureResetMode.NextFrame;
  }

  public abstract string GetDefaultEventMessageName();

  public abstract System.Type GetGestureType();

  protected virtual void Awake()
  {
    if (string.IsNullOrEmpty(EventMessageName)) {
      EventMessageName = GetDefaultEventMessageName();
    }
    if (ResetMode == GestureResetMode.Default) {
      ResetMode = GetDefaultResetMode();
    }
  }

  protected virtual void OnEnable()
  {
    if (TouchManager.Instance) {
      TouchManager.Register(this);
    }
  }

  protected virtual void OnDisable()
  {
    if (TouchManager.Instance) {
      TouchManager.Unregister(this);
    }
  }

  protected void Acquire(TouchManager.Finger finger)
  {
    if (!finger.GestureRecognizers.Contains(this)) {
      finger.GestureRecognizers.Add(this);
    }
  }

  public bool Release(TouchManager.Finger finger)
  {
    return finger.GestureRecognizers.Remove(this);
  }

  protected virtual void Start()
  {
    if (!TouchManager.Instance) {
      Debug.Log("touch instance not found in current scene");
      enabled = false;
      return;
    }
  }

  /// tool
  protected bool Young(TouchManager.IFingerList touches)
  {
    TouchManager.Finger oldestTouch = touches.GetOldest();
    if (oldestTouch == null) {
      return false;
    }
    float elapsedTimeSinceFirstTouch = Time.time - oldestTouch.StarTime;
    return elapsedTimeSinceFirstTouch < 0.25f;
  }

  public float ToPixels(float distance)
  {
    return distance.Convert(DistanceUnit, DistanceUnit.Pixels);
  }

  public float ToSqrPixels(float distance)
  {
    float pixelDist = ToPixels(distance);
    return pixelDist * pixelDist;
  }
}