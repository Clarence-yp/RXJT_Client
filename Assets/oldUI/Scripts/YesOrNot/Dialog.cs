using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Dialog : MonoBehaviour
{
  private List<object> eventlist = new List<object>();
  public void UnSubscribe()
  {
    try {
      if (eventlist != null) {
        foreach (object eo in eventlist) {
          if (eo != null) {
            DashFire.LogicSystem.EventChannelForGfx.Unsubscribe(eo);
          }
        }
        eventlist.Clear();
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  // Use this for initialization
  void Awake()
  {
    if (eventlist != null) {
      eventlist.Clear();
    }
    object eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_ui_unsubscribe", "ui", UnSubscribe);
    if (eo != null) eventlist.Add(eo);
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<string, string, string, string, DashFire.MyAction<int>, bool>("ge_show_dialog", "ui", ManageDialog);
    if (eo != null) eventlist.Add(eo);
  }
  void Start()
  {
    //UIManager.Instance.HideWindowByName("Dialog");
  }

  // Update is called once per frame
  void Update()
  {

  }
  private void ManageDialog(string message, string button0, string button1, string button2, DashFire.MyAction<int> dofunction, bool islogic)
  {
    try {
      doSomething = dofunction;
      isLogic = islogic;
      UIManager.Instance.ShowWindowByName("Dialog");

      Transform tf = transform.Find("Sprite/Button0");
      if (tf != null) {
        if (button0 == null) {
          if (NGUITools.GetActive(tf.gameObject)) {
            NGUITools.SetActive(tf.gameObject, false);
          }
        } else {
          if (!NGUITools.GetActive(tf.gameObject)) {
            NGUITools.SetActive(tf.gameObject, true);
          }
          tf = tf.Find("Label");
          if (tf != null) {
            UILabel ul = tf.gameObject.GetComponent<UILabel>();
            if (ul != null) {
              ul.text = button0;
            }
          }
        }
      }
      tf = transform.Find("Sprite/Button1");
      if (tf != null) {
        if (button1 == null) {
          if (NGUITools.GetActive(tf.gameObject)) {
            NGUITools.SetActive(tf.gameObject, false);
          }
        } else {
          if (!NGUITools.GetActive(tf.gameObject)) {
            NGUITools.SetActive(tf.gameObject, true);
          }
          tf = tf.Find("Label");
          if (tf != null) {
            UILabel ul = tf.gameObject.GetComponent<UILabel>();
            if (ul != null) {
              ul.text = button1;
            }
          }
        }
      }
      tf = transform.Find("Sprite/Button2");
      if (tf != null) {
        if (button2 == null) {
          if (NGUITools.GetActive(tf.gameObject)) {
            NGUITools.SetActive(tf.gameObject, false);
          }
        } else {
          if (!NGUITools.GetActive(tf.gameObject)) {
            NGUITools.SetActive(tf.gameObject, true);
          }
          tf = tf.Find("Label");
          if (tf != null) {
            UILabel ul = tf.gameObject.GetComponent<UILabel>();
            if (ul != null) {
              ul.text = button2;
            }
          }
        }
      }
      tf = transform.Find("Sprite/Label");
      if (tf != null) {
        UILabel ul = tf.gameObject.GetComponent<UILabel>();
        if (ul != null) {
          ul.text = message;
        }
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  public void Button0()
  {
    UIManager.Instance.HideWindowByName("Dialog");
    if (doSomething != null) {
      if (isLogic) {
        DashFire.LogicSystem.QueueLogicAction(doSomething, 0);
      } else {
        doSomething(0);
      }
    }
    doSomething = null;
  }
  public void Button1()
  {
    UIManager.Instance.HideWindowByName("Dialog");
    if (doSomething != null) {
      if (isLogic) {
        DashFire.LogicSystem.QueueLogicAction(doSomething, 1);
      } else {
        doSomething(1);
      }
    }
    doSomething = null;
  }
  public void Button2()
  {
    UIManager.Instance.HideWindowByName("Dialog");
    if (doSomething != null) {
      if (isLogic) {
        DashFire.LogicSystem.QueueLogicAction(doSomething, 2);
      } else {
        doSomething(2);
      }
    }
    doSomething = null;
  }
  private DashFire.MyAction<int> doSomething = null;
  private bool isLogic = false;
}
