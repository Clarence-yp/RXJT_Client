using UnityEngine;
using System.Collections;

public class MasterMonsterHealthBar : MonoBehaviour {

  // Use this for initialization
  private GameObject goHealthBar = null;
  void Start()
  {
    positionVec3 = this.transform.localPosition;
    Transform trans = this.transform.Find("HealthBar");
    if(trans != null)
      goHealthBar = trans.gameObject;
    if (null == goHealthBar) {
      Debug.LogError("Can Not Find HealthBar");
    }
    SetActive(false);
    //EndShakeHealthBar();
  }

  // Update is called once per frame
  void Update()
  {
    CastAnimation(goHealthBar);
    if (countdown >= 0) {
      countdown -= RealTime.deltaTime;
      if (countdown <= 0)
        SetActive(false);
    }
  }

  public void SetActive(bool isActive)
  {
    NGUITools.SetActive(this.gameObject, isActive);
  }
  void SetHealthValueText(int current, int max)
  {
    Transform trans = this.transform.Find("HealthBar/healthValue");
    if(null == trans)
      return;
    GameObject go = trans.gameObject;
    if (null != go) {
      UILabel label = go.GetComponent<UILabel>();
      if (null != label)
        label.text = current.ToString() + "/" + max.ToString();
    }
  }

  private void SetName(string name)
  {
    Transform trans = transform.Find("name");
    if(trans ==null )
      return;
    GameObject go = trans.gameObject;
    UILabel label = null;
    if (go != null)
      label = go.GetComponent<UILabel>();
    if (null != label) {
      label.text = name.ToString();
    }
  }
  void ShakeHealthBar()
  {
    TweenPosition tweenPosition = this.gameObject.GetComponent<TweenPosition>();
    if (tweenPosition != null) { 
      //Debug.Log("Destory tweenPos");
      Destroy(tweenPosition);
    }
    TweenPosition tween = this.gameObject.AddComponent<TweenPosition>();
    if (null != tween) {
      tween.from = new Vector3(positionVec3.x + 25, positionVec3.y + 25, positionVec3.z);
      tween.to = positionVec3;
      tween.duration = 0.3f;
    }
  }

  void UpdateHealthBar(int curValue, int maxValue)
  {
    ShakeHealthBar();
    SetActive(true);
    SetHealthValueText(curValue, maxValue);
    countdown = waitTime;
    int valueOfLine = maxValue / m_Index;
    if (valueOfLine <= 0)
      return;
    int index = curValue / valueOfLine;
    if (curValue % valueOfLine == 0)
      index--;
    float value = (curValue - index * valueOfLine) / (float)valueOfLine;
    if (curValue <= 0)
      value = 0;
    UIProgressBar progressBar = null;
    progressBar = goHealthBar.GetComponent<UIProgressBar>();
    if (null != progressBar) {
      progressBar.value = value;
    }
    GameObject go = null;
    if (null != goHealthBar && index >= 0) {
      Transform trans = goHealthBar.transform.Find("fore");
      UISprite spFore = null;
      if(null != trans)
        spFore = trans.gameObject.GetComponent<UISprite>();
      trans = goHealthBar.transform.Find("back");
      UISprite spBack = null;
      if (null != trans)
        spBack = trans.gameObject.GetComponent<UISprite>();
      index = index >= m_Index ? 0 : index;
      if (null != spFore) {
        spFore.spriteName = "blood" + (m_Index - index).ToString();
        if (index <= 0) {
          spBack.spriteName = "back";
        } else {
          spBack.spriteName = "blood" + (m_Index -index +1).ToString();
        }
      }
    }
    Transform trs =  transform.Find("itemNum");
    if(trs!=null)
      go = trs.gameObject;
    UILabel label = null;
    if (go != null)
      label = go.GetComponent<UILabel>();
    if (null != label) {
      if (curValue <= 0)
        label.text = "Dead";
      else {
        label.text = " * " + index.ToString(); 
      }
    }
  }

  void CastAnimation(GameObject father)
  {
    Transform trans = null;
    GameObject goBack = null;
    UIProgressBar progressBar = null;
    if (null != father) {
      trans = father.transform.Find("forDel");
      if(trans!= null)
        goBack = trans.gameObject;
      progressBar = father.GetComponent<UIProgressBar>();
    }

    UISprite spBack = null;
    if (null != goBack) {
      spBack = goBack.GetComponent<UISprite>();

    }

    if (null != spBack && null != progressBar) {
      if (spBack.fillAmount <= progressBar.value) {
        spBack.fillAmount = progressBar.value;
      } else {
        spBack.fillAmount -= RealTime.deltaTime * 0.5f;
      }
    }

  }

  public void TestButton()
  {
    if (goHealthBar != null) {
      //ShakeHealthBar();
      testValue -= testDelta;
      //Debug.Log(testValue);
      UpdateHealthBar(testValue, 500);
    }
  }
  private float countdown = -1;
  public float waitTime = 1f;
  public int testDelta = 20;
  public int testValue = 500;
  private Vector3 positionVec3 = new Vector3();
  //记录当前Boss所剩血条数(应该用参数赋值，这里做测试用)
  private int m_Index = 5;
  //记录血条的颜色(Config)
  public Color[] color = new Color[] {};
}
