using UnityEngine;
using System.Collections;

public class StageClear : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {
    m_Particle =  DashFire.ResourceSystem.GetSharedResource("Hero_FX/1_Swordman/ui_FX_01") as GameObject;
   
    Transform trans =this.transform.Find("stage");
    if (trans == null)
      return;
    GameObject go = trans.gameObject;
    if (null != go) {
      Vector3 pos = go.transform.position * 0.3f;
      PlayParticle("stage", pos);
      pos = go.transform.position * 1.3f;
      PlayParticle("stage", pos);
    }
    trans = this.transform.Find("clear");
    if (trans == null)
      return;
    go = trans.gameObject;
    if (null != go) {
      Vector3 pos = go.transform.position * 0.3f;
      PlayParticle("clear", pos);
      pos = go.transform.position * 1.3f;
      PlayParticle("clear", pos);
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (m_IsStageFinished && m_IsClearFinished) {
      timeDelta -= RealTime.deltaTime;
      if (timeDelta <= 0) {

        StartTween();
        m_IsStageFinished = false;
        m_IsClearFinished = false;
        timeDelta = 0.5f;
      }
    }
  }

  private void StartTween()
  {
    Transform trans = this.transform.Find("stage");
    if(trans == null)
      return;
    GameObject go = trans.gameObject;
    if (null == go)
      return;
    StartTweenPos(go);
    TweenAlpha.Begin(go, 0.2f, 0f);

    trans =this.transform.Find("clear");
    if (null == trans)
      return;
    go = trans.gameObject;
    if (null == go)
      return;
    StartTweenPos(go);
    TweenAlpha.Begin(go, 0.2f, 0f);
  }
  private void StartTweenPos(GameObject go)
  {
    if (null == go)
      return;
    TweenPosition tweenPos = go.GetComponent<TweenPosition>();
    if (tweenPos != null)
      Destroy(tweenPos);
    tweenPos = go.AddComponent<TweenPosition>();
    tweenPos.from = go.transform.localPosition;
    tweenPos.to = new Vector3(0, 0, 0);
    tweenPos.duration = 0.2f;
  }
  public void OnStageAnimFinished()
  {
    Transform trans = this.transform.Find("stage");
    if (trans == null)
      return;
    GameObject go = trans.gameObject;
    if (null == go)
      return;
    Vector3  pos = go.transform.position;
    PlayParticle("stage", pos);
    UISprite sp = go.GetComponent<UISprite>();
    if (null == sp)
      return;
    sp.spriteName = m_ClearType;
    m_IsStageFinished = true;
  }
  public void OnClearAnimFinished()
  {
    Transform trans  = this.transform.Find("clear");
    if (null == trans)
      return;
    GameObject go = trans.gameObject;
    if (null == go)
      return;
    Vector3 pos = go.transform.position;
    PlayParticle("clear", pos);
    UISprite sp = go.GetComponent<UISprite>();
    if (null == sp)
      return;
    sp.spriteName = "clear";
    m_IsClearFinished = true;
  }
  private void PlayParticle(string father, Vector3 nguiPos)
  {
    Transform trans = this.transform.Find(father);
    if(null == trans)
      return;
    GameObject fatherGo = trans.gameObject;
    if (null == fatherGo)
      return;
   
    if (null != m_Particle) {
      Vector3 curPos = UICamera.mainCamera.WorldToScreenPoint(nguiPos);
      curPos = Camera.main.ScreenToWorldPoint(new Vector3(curPos.x, curPos.y, 5));
      GameObject effect = GameObject.Instantiate(m_Particle, curPos, Quaternion.identity) as GameObject;
      if (null != effect)
        effect.transform.position = curPos;
      Destroy(effect, duration);

    }

  }
  public void SetClearType(string type)
  {
    m_ClearType = type.ToLower();
  }
  private string m_ClearType = "stage";
  private GameObject m_Particle = null;
  private bool m_IsStageFinished = false;
  private bool m_IsClearFinished = false;
  public float duration = 1.0f;
  public float timeDelta = 0.5f;
}
