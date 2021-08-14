using UnityEngine;
using System.Collections;
using DashFire;

public class ExInputEffect : MonoBehaviour
{
  internal void Start()
  {
    DashFire.LogicSystem.EventChannelForGfx.Subscribe("ex_skill_start", "skill", this.SkillStart);
    DashFire.LogicSystem.EventChannelForGfx.Subscribe("ex_skill_end", "skill", this.SkillEnd);

    TouchManager.OnFingerEvent += OnFingerEvent;
  }

  private void SkillStart()
  {
    active = true;
    if (InputType.Joystick == DFMUiRoot.InputMode) {
      JoyStickInputProvider.JoyStickEnable = false;
    } else {
      TouchManager.GestureEnable = false;
    }
  }

  private void SkillEnd()
  {
    active = false;
    if (InputType.Joystick == DFMUiRoot.InputMode) {
      JoyStickInputProvider.JoyStickEnable = true;
    } else {
      TouchManager.GestureEnable = true;
    }
  }

  internal void OnDestroy()
  {
    TouchManager.OnFingerEvent -= OnFingerEvent;
    if (obj != null) {
      GameObject.Destroy(obj);
      obj = null;
    }
    original = null;
  }

  void Update()
  {
  }

  private void OnFingerEvent(FingerEvent args)
  {
    if (!active) {
      return;
    }
    if (null == args) {
      return;
    }
    if (DashFire.TouchType.Regognizer != TouchManager.curTouchState) {
      return;
    }
    if (GestureEvent.OnFingerMove.ToString() == args.Name) {
      if (TouchManager.GestureEnable || JoyStickInputProvider.JoyStickEnable) {
        return;
      }
      if (args.Finger.IsDown && args.Finger.IsMoving) {
        if (args.Finger.DistanceFromStart > validLength) {
          Vector3 curPos = Camera.main.ScreenToWorldPoint(new Vector3(args.Position.x, args.Position.y, depth));
          if (null == obj) {
            obj = GameObject.Instantiate(original, curPos, Quaternion.identity) as GameObject;
          }
          if (null != obj) {
            obj.transform.position = curPos;
          }
        }
      }
    } else if (GestureEvent.OnFingerUp.ToString() == args.Name) {
      if (args.Finger.WasDown && null != obj) {
        GameObject.Destroy(obj, duration);
        obj = null;
      }
    }
  }

  private bool active = false;
  private GameObject obj = null;
  public Object original = null;
  [SerializeField]
  public float duration = 1.0f;
  [SerializeField]
  public float depth = 1.0f;
  [SerializeField]
  public float validLength = 10f;
}