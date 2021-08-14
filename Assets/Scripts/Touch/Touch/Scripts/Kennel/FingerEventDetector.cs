using UnityEngine;
using System.Collections.Generic;

public class FingerEvent {
  FingerEventDetector detector;
  TouchManager.Finger finger;
  string name = string.Empty;

  public string Name
  {
    get
    {
      return name;
    }
    set
    {
      name = value;
    }
  }

  public FingerEventDetector Detector
  {
    get
    {
      return detector;
    }
    set
    {
      detector = value;
    }
  }

  public TouchManager.Finger Finger
  {
    get
    {
      return finger;
    }
    set
    {
      finger = value;
    }
  }

  public virtual Vector2 Position
  {
    get
    {
      return finger.Position;
    }
    set
    {
      throw new System.NotSupportedException("Setting position is not supported on " + this.GetType());
    }
  }

  public Vector3 GetTouchToWorldPoint()
  {
    if(null == Camera.main)
      return Vector3.zero;
    Vector3 cur_touch_worldpos = Vector3.zero;
    Vector3 cur_touch_pos = new Vector3(Position.x, Position.y, 0);
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

  public List<GameObject> GetRayObjectsByLayerName(string name)
  {
    if (null == Camera.main)
      return null;
    List<GameObject> go = new List<GameObject>(); 
    Vector3 cur_touch_pos = new Vector3(Position.x, Position.y, 0);
    Ray ray = Camera.main.ScreenPointToRay(cur_touch_pos);
    int layermask = 1 << LayerMask.NameToLayer(name);
    RaycastHit[] rch = Physics.RaycastAll(ray, 200f, layermask);
    foreach (RaycastHit node in rch) {
      if (null != node.collider.gameObject) {
        go.Add(node.collider.gameObject);
      }
    }
    return go.Count > 0 ? go : null;
  }
}

public abstract class FingerEventDetector<T> : FingerEventDetector where T : FingerEvent, new() 
{
  List<T> fingerEventsList;
  public delegate void FingerEventHandler(T eventData);

  protected virtual T CreateFingerEvent()
  {
    return new T();
  }
  public override System.Type GetEventType()
  {
    return typeof(T);
  }
  protected override void Start()
  {
    base.Start();
    TouchManager.OnInputProviderChanged += TouchManager_OnInputProviderChanged;
    Init();
  }
  protected virtual void OnDestroy()
  {
    TouchManager.OnInputProviderChanged -= TouchManager_OnInputProviderChanged;
  }
  void TouchManager_OnInputProviderChanged()
  {
    Init();
  }
  protected virtual void Init()
  {
    Init(TouchManager.Instance.MaxFingers);
  }
  protected virtual void Init(int fingersCount)
  {
    fingerEventsList = new List<T>(fingersCount);
    for (int i = 0; i < fingersCount; ++i) {
      T e = CreateFingerEvent();
      e.Detector = this;
      e.Finger = TouchManager.GetFinger(i);
      fingerEventsList.Add(e);
    }
  }
  protected T GetEvent(TouchManager.Finger finger)
  {
    return GetEvent(finger.Index);
  }
  protected virtual T GetEvent(int fingerIndex)
  {
    return (null != fingerEventsList && fingerEventsList.Count > fingerIndex) ? fingerEventsList[fingerIndex] : null;
  }
}

public abstract class FingerEventDetector : MonoBehaviour 
{
  // -1 任何手指
  int FingerIndexFilter = -1;
  TouchManager.Finger activeFinger;

  protected abstract void ProcessFinger(TouchManager.Finger finger);
  public abstract System.Type GetEventType();

  protected virtual void Awake()
  {
  }

  protected virtual void Start()
  {
  }

  protected virtual void Update()
  {
    ProcessFingers();
  }

  protected virtual void ProcessFingers()
  {
    if (FingerIndexFilter >= 0 && FingerIndexFilter < TouchManager.Instance.MaxFingers) {
      ProcessFinger(TouchManager.GetFinger(FingerIndexFilter));
    } else {
      for (int i = 0; i < TouchManager.Instance.MaxFingers; ++i) {
        ProcessFinger(TouchManager.GetFinger(i));
      }
    }
  }

  private string CorrectionEventName(FingerEvent eventData)
  {
    ///
    if ("OnFingerUp" == eventData.Name) {
      TouchManager.Instance.GestureRecognizerSwitch(true);
    }
    ///
    bool canModify = false;
    if ("MouseInput" == TouchManager.Instance.InputProvider.name) {
      bool isRightButtonPress = Input.GetMouseButton(1);
      bool isRightButtonDown = Input.GetMouseButtonDown(1);
      bool isRightButtonUp = Input.GetMouseButtonUp(1);
      if (isRightButtonPress || isRightButtonDown || isRightButtonUp) {
        canModify = true;
      }
    } else {
      if (2 == Input.touchCount) {
        if ("OnFingerUp" == eventData.Name) {
          if (1 == Input.touchCount) {
            canModify = true;
          }
        } else {
          canModify = true;
        }
      }
    }
    if (canModify) {
      if ("OnFingerDown" == eventData.Name) {
        eventData.Name = "OnTwoFingerDown";
      } else if ("OnFingerUp" == eventData.Name) {
        eventData.Name = "OnTwoFingerUp";
      } else if ("OnFingerMove" == eventData.Name) {
        eventData.Name = "OnTwoFingerMove";
      }
    }
    return eventData.Name;
  }

  /// FireEvent
  protected void TrySendMessage(FingerEvent eventData)
  {
    eventData.Name = CorrectionEventName(eventData);
    TouchManager.FireEvent(eventData);
  }
}
