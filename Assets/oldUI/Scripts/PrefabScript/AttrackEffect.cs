using UnityEngine;
using System.Collections;

public class AttrackEffect : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {
    oldfontscale = this.GetComponent<UILabel>().transform.localScale;
    if (signforinitpos) {
      signforinitpos = false;
      oldpos = this.transform.localPosition;
    }
  }

  // Update is called once per frame
  void Update()
  {
    if (signforinitpos) {
      signforinitpos = false;
      oldpos = this.transform.localPosition;
    }
    time += RealTime.deltaTime;

    if (time <= 0.16f) {
      float scale = time*3;
      this.GetComponent<UILabel>().transform.localScale = new Vector3(oldfontscale.x+scale,oldfontscale.y+scale,oldfontscale.z);
      this.transform.localPosition = new Vector3(oldpos.x, oldpos.y + ((time - 0.16f) * 200), oldpos.z);
    }
    if (time > 0.16 && time <= 0.66) {
      this.transform.localPosition = new Vector3(oldpos.x, oldpos.y - ((time - 0.66f) * 5), oldpos.z);
    }
    if (time > 0.66f) {
      this.GetComponent<UILabel>().alpha = 1.0f - (time - 0.66f) / 0.2f;
      this.transform.localPosition = new Vector3(oldpos.x, oldpos.y + ((time - 0.66f) * 200), oldpos.z);
    }
    if (time > 0.86f) {
      this.GetComponent<UILabel>().alpha = 1.0f;
      this.GetComponent<UILabel>().transform.localScale = oldfontscale;
      DashFire.ResourceSystem.RecycleObject(gameObject);
      time = 0.0f;
      signforinitpos = true;
    }
  }

  private float time = 0.0f;
  private Vector3 oldpos;
  private bool signforinitpos = true;
  private Vector3 oldfontscale;
}
