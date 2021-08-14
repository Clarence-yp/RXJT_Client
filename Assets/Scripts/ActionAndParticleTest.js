#pragma strict

@script RequireComponent (Animation)

public var needTestAnimation : AnimationClip;
public var needTestParticle: GameObject;
public var BindBone: Transform;
public var needAttach: boolean = true;

private var _animation: Animation;
private var instance : GameObject;

function Start () {
   _animation = GetComponent(Animation);
}

function Update () {
    if(Input.GetButtonDown ("Fire1"))
   {

      _animation.Play(needTestAnimation.name);
      instance = Instantiate(needTestParticle,BindBone.position,BindBone.rotation);
      if(needAttach == true)
      {
         instance.transform.parent = BindBone;
      }
      
   }
   if(Input.GetButtonDown ("Fire2"))
   {
       _animation.Stop();
       Destroy(instance);
   }
}