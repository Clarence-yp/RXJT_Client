using UnityEngine;
using System.Collections;

public class DamageForCutHero : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {
    livetime = 0.0f;
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
    livetime += RealTime.deltaTime;

    if (livetime <= 0.16f) {
      float scale = livetime * 3;
      this.GetComponent<UILabel>().transform.localScale = new Vector3(oldfontscale.x + scale, oldfontscale.y + scale, oldfontscale.z);
      this.transform.localPosition = new Vector3(oldpos.x, oldpos.y - ((livetime - 0.14f) * 200), oldpos.z);
    }
    if (livetime > 0.16 && livetime <= 0.66) {
      this.transform.localPosition = new Vector3(oldpos.x, oldpos.y + ((livetime - 0.66f) * 5), oldpos.z);
    }
    if (livetime > 0.66f) {
      this.GetComponent<UILabel>().alpha = 1.0f - (livetime - 0.66f) / 0.2f;
      this.transform.localPosition = new Vector3(oldpos.x, oldpos.y - ((livetime - 0.66f) * 200));
    }
    if (livetime > 0.86f) {
      this.GetComponent<UILabel>().transform.localScale = oldfontscale;
      this.GetComponent<UILabel>().alpha = 1.0f;
      DashFire.ResourceSystem.RecycleObject(gameObject);
      livetime = 0.0f;
      signforinitpos = true;
    }
  }

  private float livetime = 0.0f;
  //private int oldfontsize = 0;
  private Vector3 oldpos;
  private bool signforinitpos = true;
  private Vector3 oldfontscale;
}
