using UnityEngine;
using System.Collections;

public class ChainBeat : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {
    time = 0.0f;
  }

  // Update is called once per frame
  void Update()
  {
    time += RealTime.deltaTime;
    int multiple = (int)System.Math.Round(time / 0.03f);
    switch (multiple) {
      case 0: Zoom(1.4f); break;
      case 1: Zoom(1.8f); break;
      case 2: Zoom(1.4f); break;
      case 3: Zoom(1.2f); break;
      case 4: Zoom(1.0f); break;
      default: break;
    }
    if (time > 0.12f) {
      Zoom(1.0f);
      ChainBeat cb = gameObject.GetComponent<ChainBeat>();
      if (cb != null) {
        cb.enabled = false;
      }
    }
  }
  void Zoom(float wantscale)
  {
    //transform.localScale = new Vector3(wantscale, wantscale, wantscale);
    int i = transform.childCount;
    for (int j = 0; j < i; ++j) {
      Transform tf = transform.GetChild(j);
      if (tf != null) {
        tf.localScale = new Vector3(wantscale, wantscale, wantscale);
      }
    }
  }
  public void SetInitTime()
  {
    time = 0.0f;
  }
  private float time = 0.0f;
}
