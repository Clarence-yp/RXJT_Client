using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class NickName : MonoBehaviour
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
  }

  // Update is called once per frame
  void Update()
  {
    if (nicklabel != null && playergo != null && nicklabel.enabled != playergo.activeInHierarchy) {
      nicklabel.enabled = playergo.activeInHierarchy;
    }
  }

  void LateUpdate()
  {
    if (playergo != null && Camera.main != null) {
      Vector3 pos = playergo.transform.position;
      pos = Camera.main.WorldToScreenPoint(new Vector3(pos.x, pos.y + height, pos.z));
      pos.z = 0;
      pos = UICamera.mainCamera.ScreenToWorldPoint(pos);
      gameObject.transform.position = pos;
    }
  }

  public void SetPlayerGameObjectAndNickName(GameObject go, string nickname, Color col)
  {
    playergo = go;

    UILabel ul = gameObject.GetComponent<UILabel>();
    if (ul != null && nickname != null) {
      ul.text = nickname;
      ul.color = col;
      nicklabel = ul;
    }

    Update();
    LateUpdate();
  }
  private GameObject playergo = null;
  private float height = 2.5f;
  private UILabel nicklabel = null;
}
