using UnityEngine;
using System.Collections;

public class YesOrNot : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void Yes()
  {
    if (doSomething != null) {
      doSomething(true);
    }
    doSomething = null;
    NGUITools.DestroyImmediate(gameObject);
  }
  public void Not()
  {
    if (doSomething != null) {
      doSomething(false);
    }
    doSomething = null;
    NGUITools.DestroyImmediate(gameObject);
  }
  public void SetMessageAndDO(string message, System.Action<bool> dofunction)
  {
    Transform tf = gameObject.transform.Find("Sprite/Label");
    if (tf != null) {
      UILabel ul = tf.gameObject.GetComponent<UILabel>();
      if (ul != null && message != null) {
        ul.text = message;
      }
    }
    doSomething = dofunction;
  }
  private System.Action<bool> doSomething = null;
}
