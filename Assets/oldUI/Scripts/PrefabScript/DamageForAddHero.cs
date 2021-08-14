using UnityEngine;
using System.Collections;

public class DamageForAddHero : MonoBehaviour
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

    if (livetime < 0.1f) {
      float scale = livetime * 3;
      this.GetComponent<UILabel>().transform.localScale = new Vector3(oldfontscale.x + scale, oldfontscale.y + scale, oldfontscale.z);
      this.transform.localPosition = new Vector3(oldpos.x, oldpos.y + ((livetime) * 200), oldpos.z);
    }
    if (livetime > 1.0f) {
      this.GetComponent<UILabel>().alpha = 1.0f - (livetime - 1.0f) / 0.25f;
    }
    if (livetime > 1.25f) {
      this.GetComponent<UILabel>().transform.localScale = oldfontscale;
      this.GetComponent<UILabel>().alpha = 1.0f;
      DashFire.ResourceSystem.RecycleObject(gameObject);
      livetime = 0.0f;
      signforinitpos = true;
    }
  }

  private float livetime = 0.0f;
  private Vector3 oldpos;
  private bool signforinitpos = true;
  private Vector3 oldfontscale;
}
