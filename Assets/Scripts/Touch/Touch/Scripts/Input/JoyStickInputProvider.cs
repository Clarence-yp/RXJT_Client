using UnityEngine;
using System.Collections;
using DashFire;

public class JoyStickInputProvider : MonoBehaviour {
  internal void Start()
  {
    if (m_JoyStick != null) {
      m_JoyStickObj = GameObject.Instantiate(m_JoyStick) as GameObject;
      if (m_JoyStickObj != null) {
        m_JoyStickObj.transform.position = Vector3.zero;
        EasyJoystick joyStickScript = m_JoyStickObj.GetComponentInChildren<EasyJoystick>();
        if (joyStickScript != null) {
          joyStickScript.JoystickPositionOffset = new Vector2(
            m_JoyStickPosPercent.x * Screen.width,
            m_JoyStickPosPercent.y * Screen.height);
        }
        JoyStickEnable = true;
        JoyStickEnable = false;
      }
    }
  }

  internal void OnDestroy()
  {
    if (null != m_JoyStickObj) {
      GameObject.Destroy(m_JoyStickObj);
    }
  }

  private static void ShowJoyStick()
  {
    if (m_JoyStickObj != null) {
      m_JoyStickObj.SetActive(true);
      EasyJoystick ej = m_JoyStickObj.GetComponentInChildren<EasyJoystick>();
      if (null != ej) {
        ej.JoystickTouch = Vector2.zero;
      }
    }
  }

  private static void HideJoyStick()
  {
    if (m_JoyStickObj != null) {
      EasyJoystick ej = m_JoyStickObj.GetComponentInChildren<EasyJoystick>();
      if (null != ej) {
        ej.CreateEvent(EasyJoystick.MessageName.On_JoystickMoveEnd);
      }
      m_JoyStickObj.SetActive(false);
    }
  }

  public static bool JoyStickEnable
  {
    get {
      return m_JoyStickEnable;
    }
    set {
      m_JoyStickEnable = value;
      if (m_JoyStickEnable) {
        ShowJoyStick();
      }
      else {
        HideJoyStick();
      }
    }
  }
  public static void SetActive(bool active)
  {
    if (null != m_JoyStickObj) {
      EasyJoystick ej = m_JoyStickObj.GetComponentInChildren<EasyJoystick>();
      if (null != ej) {
        ej.isActivated = active;
      }
    }
  }
  private static bool m_JoyStickEnable = true;
  private static GameObject m_JoyStickObj = null;
  public GameObject m_JoyStick = null;
  public Vector2 m_JoyStickPosPercent = new Vector2(0.5f, 0.5f);
}