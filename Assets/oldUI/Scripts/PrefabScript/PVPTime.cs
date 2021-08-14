using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PVPTime : MonoBehaviour
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
      }
      eventlist.Clear();
      NGUITools.DestroyImmediate(gameObject);
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  // Use this for initialization
  void Start()
  {
    if (eventlist != null) { eventlist.Clear(); }
    object eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_ui_unsubscribe", "ui", UnSubscribe);
    if (eo != null) eventlist.Add(eo);
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<long>("ge_send_scenestart_time", "ui", PvpSceneStartTime);
    if (eo != null) eventlist.Add(eo);
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe<int, int, int, int, int, int, int, int, int, int, string>("ge_pvp_result", "ui", PvpResult);
    if (eo != null) eventlist.Add(eo);

    Transform tf = transform.Find("Label");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null) {
        timelabel = ul;
      }
    }
    DashFire.GfxSystem.EventChannelForLogic.Publish("ge_get_scenestart_time", "lobby");
  }

  // Update is called once per frame
  void Update()
  {
    if (timelabel != null) {
      long residuetime = (long)countDownTime - (DashFire.TimeUtility.GetServerMilliseconds() - StartTime) / 1000;

      string str1 = (residuetime / 60).ToString();
      if (str1.Length == 1) {
        str1 = "0" + str1;
      }
      string str2 = (residuetime % 60).ToString();
      if (str2.Length == 1) {
        str2 = "0" + str2;
      }
      timelabel.text = str1 + ":" + str2;
      if (residuetime <= 0) {
        enabled = false;
      }
    }
  }

  void PvpSceneStartTime(long starttime)
  {
    try {
      StartTime = starttime;
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  public void SetCountDownTime(int time)
  {
    countDownTime = time;
  }
  void PvpResult(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, string k)
  {
    try {
      PVPTime pt = gameObject.GetComponent<PVPTime>();
      if (pt != null) {
        pt.enabled = false;
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  private long StartTime = 0;
  private int countDownTime = 0;
  private UILabel timelabel = null;
}
