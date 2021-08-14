using UnityEngine;
using System.Collections;

public class BloodAnimation : MonoBehaviour
{

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public void AnimaionFinish()
  {
    DashFire.ResourceSystem.RecycleObject(gameObject);
    EventDelegate.Remove(anim.onFinished, AnimaionFinish);
  }

  public void PlayAnimation()
  {
    Transform tf = transform.Find("Label");
    if (tf != null) {
      Animation a = tf.gameObject.GetComponent<Animation>();
      if (a != null) {
        anim = ActiveAnimation.Play(a, a.clip.name, AnimationOrTween.Direction.Forward,
          AnimationOrTween.EnableCondition.EnableThenPlay, AnimationOrTween.DisableCondition.DisableAfterForward);
        EventDelegate.Add(anim.onFinished, AnimaionFinish, true);
      }
    }
  }
  private ActiveAnimation anim = null;
}
