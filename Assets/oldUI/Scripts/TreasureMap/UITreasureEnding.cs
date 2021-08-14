using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using DashFire;

public class UITreasureEnding: MonoBehaviour {

  public GameObject goFailure = null;
  public GameObject goWinner = null;
  public UILabel lblCountDown = null;
  public UILabel lblHp = null;
  public UILabel lblMp = null;
  public UIProgressBar progressHp = null;
  public UIProgressBar progressMp = null;

  public float ReturnCountDownForWin = 1f;
  public float CountDownDelta = 60;//s
  private float m_CountDown = 0f;
  private bool m_IsWinner = false;
	// Use this for initialization
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
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }
  void Awake(){
    object eo = null;
    eo =LogicSystem.EventChannelForGfx.Subscribe<int,bool,int,int,int,int,int>("ge_finish_expedition", "expedition", ExpeditionFinish);
    if (eo != null) eventlist.Add(eo);
    eo = DashFire.LogicSystem.EventChannelForGfx.Subscribe("ge_ui_unsubscribe", "ui", UnSubscribe);
    if (eo != null) eventlist.Add(eo);
  }
	void Start () {
    //ExpeditionFinish(1, false, 10, 30, 100, 100);
	}
	
	// Update is called once per frame
	void Update () {
    if (!m_IsWinner && lblCountDown != null && m_CountDown >= 0) {
      int minite = ((int)m_CountDown) / 60;
      int second = ((int)m_CountDown) % 60;
      string str = string.Format("{0:D2}:{1:D2}", minite, second);
      lblCountDown.text = str;
      m_CountDown -= Time.deltaTime;
    } else {
      if (!m_IsWinner && m_CountDown < 0) {
        OnReturnMainCity();
      }
    }
	}
  public void ExpeditionFinish(int tollgateNum, bool isWinner, int hp, int mp, int hpMax, int mpMax, int rage)
  {
    try {
      if (m_CountDown > 0) return;
      UIManager.Instance.ShowWindowByName("TreasureEnding");
      NGUITools.SetActive(goFailure, !isWinner);
      NGUITools.SetActive(goWinner, isWinner);
			m_IsWinner = isWinner;
      if (isWinner) {
        if (progressHp != null &&hpMax!=0) {
          progressHp.value = (float)hp / hpMax;
        }
        StringBuilder sBuilder = new StringBuilder(hp + "/" + hpMax);
        if (lblHp != null) lblHp.text = sBuilder.ToString();
        if (progressMp != null && mpMax != 0) {
          progressMp.value = (float)mp / mpMax;
        }
        StringBuilder sBuilder1 = new StringBuilder(mp + "/" + mpMax);
        if (lblMp != null) lblMp.text = sBuilder1.ToString();
        StartCoroutine(ReturnBackMainCity(ReturnCountDownForWin));
      } else {
        m_CountDown = CountDownDelta;
      }
    } catch (Exception ex) {
      DashFire.LogicSystem.LogicLog("[Error]:Exception:{0}\n{1}", ex.Message, ex.StackTrace);
    }
  }

  public void OnReturnMainCity()
  {
    UIManager.Instance.HideWindowByName("TreasureEnding");
    DashFire.GfxSystem.EventChannelForLogic.Publish("ge_return_maincity", "lobby");
  }
  public IEnumerator ReturnBackMainCity(float delta)
  {
    yield return new WaitForSeconds(delta);

    DashFire.GfxSystem.EventChannelForLogic.Publish("ge_return_maincity", "lobby");
  }
}
