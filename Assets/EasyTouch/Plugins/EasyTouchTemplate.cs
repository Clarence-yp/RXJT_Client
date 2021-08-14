// EasyTouch v3.0 (September 2012)
// EasyTouch library is copyright (c) of Hedgehog Team
// Please send feedback or bug reports to the.hedgehog.team@gmail.com
using UnityEngine;
using System.Collections;

public class C_EasyTouchTemplate : MonoBehaviour {

	void OnEnable(){
	
		EasyTouch.On_Cancel += On_Cancel;
		EasyTouch.On_TouchStart += On_TouchStart;
		EasyTouch.On_TouchDown += On_TouchDown;
		EasyTouch.On_TouchUp += On_TouchUp;
		EasyTouch.On_SimpleTap += On_SimpleTap;
		EasyTouch.On_DoubleTap += On_DoubleTap;
		EasyTouch.On_LongTapStart +=On_LongTapStart;
		EasyTouch.On_LongTap += On_LongTap;
		EasyTouch.On_LongTapEnd += On_LongTapEnd;
		EasyTouch.On_DragStart += On_DragStart;
		EasyTouch.On_Drag += On_Drag;
		EasyTouch.On_DragEnd += On_DragEnd;
		EasyTouch.On_SwipeStart += On_SwipeStart;
		EasyTouch.On_Swipe += On_Swipe;
		EasyTouch.On_SwipeEnd += On_SwipeEnd;
		EasyTouch.On_TouchStart2Fingers += On_TouchStart2Fingers;
		EasyTouch.On_TouchDown2Fingers += On_TouchDown2Fingers;
		EasyTouch.On_TouchUp2Fingers += On_TouchUp2Fingers;
		EasyTouch.On_SimpleTap2Fingers += On_SimpleTap2Fingers;
		EasyTouch.On_DoubleTap2Fingers += On_DoubleTap2Fingers;
		EasyTouch.On_LongTapStart2Fingers += On_LongTapStart2Fingers;
		EasyTouch.On_LongTap2Fingers += On_LongTap2Fingers;
		EasyTouch.On_LongTapEnd2Fingers += On_LongTapEnd2Fingers;
		EasyTouch.On_Twist += On_Twist;
		EasyTouch.On_TwistEnd += On_TwistEnd;
		EasyTouch.On_PinchIn += On_PinchIn;
		EasyTouch.On_PinchOut += On_PinchOut;
		EasyTouch.On_PinchEnd += On_PinchEnd;
		EasyTouch.On_DragStart2Fingers += On_DragStart2Fingers;
		EasyTouch.On_Drag2Fingers += On_Drag2Fingers;
		EasyTouch.On_DragEnd2Fingers += On_DragEnd2Fingers;
		EasyTouch.On_SwipeStart2Fingers += On_SwipeStart2Fingers;
		EasyTouch.On_Swipe2Fingers += On_Swipe2Fingers;
		EasyTouch.On_SwipeEnd2Fingers += On_SwipeEnd2Fingers;
	}
	
	void OnDisable(){
		EasyTouch.On_Cancel -= On_Cancel;
		EasyTouch.On_TouchStart -= On_TouchStart;
		EasyTouch.On_TouchDown -= On_TouchDown;
		EasyTouch.On_TouchUp -= On_TouchUp;
		EasyTouch.On_SimpleTap -= On_SimpleTap;
		EasyTouch.On_DoubleTap -= On_DoubleTap;
		EasyTouch.On_LongTapStart -=On_LongTapStart;
		EasyTouch.On_LongTap -= On_LongTap;
		EasyTouch.On_LongTapEnd -= On_LongTapEnd;
		EasyTouch.On_DragStart -= On_DragStart;
		EasyTouch.On_Drag -= On_Drag;
		EasyTouch.On_DragEnd -= On_DragEnd;
		EasyTouch.On_SwipeStart -= On_SwipeStart;
		EasyTouch.On_Swipe -= On_Swipe;
		EasyTouch.On_SwipeEnd -= On_SwipeEnd;
		EasyTouch.On_TouchStart2Fingers -= On_TouchStart2Fingers;
		EasyTouch.On_TouchDown2Fingers -= On_TouchDown2Fingers;
		EasyTouch.On_TouchUp2Fingers -= On_TouchUp2Fingers;
		EasyTouch.On_SimpleTap2Fingers -= On_SimpleTap2Fingers;
		EasyTouch.On_DoubleTap2Fingers -= On_DoubleTap2Fingers;
		EasyTouch.On_LongTapStart2Fingers -= On_LongTapStart2Fingers;
		EasyTouch.On_LongTap2Fingers -= On_LongTap2Fingers;
		EasyTouch.On_LongTapEnd2Fingers -= On_LongTapEnd2Fingers;
		EasyTouch.On_Twist -= On_Twist;
		EasyTouch.On_TwistEnd -= On_TwistEnd;
		EasyTouch.On_PinchIn -= On_PinchIn;
		EasyTouch.On_PinchOut -= On_PinchOut;
		EasyTouch.On_PinchEnd -= On_PinchEnd;
		EasyTouch.On_DragStart2Fingers -= On_DragStart2Fingers;
		EasyTouch.On_Drag2Fingers -= On_Drag2Fingers;
		EasyTouch.On_DragEnd2Fingers -= On_DragEnd2Fingers;
		EasyTouch.On_SwipeStart2Fingers -= On_SwipeStart2Fingers;
		EasyTouch.On_Swipe2Fingers -= On_Swipe2Fingers;
		EasyTouch.On_SwipeEnd2Fingers -= On_SwipeEnd2Fingers;		
	}
	
	void OnDestroy(){
		EasyTouch.On_Cancel -= On_Cancel;
		EasyTouch.On_TouchStart -= On_TouchStart;
		EasyTouch.On_TouchDown -= On_TouchDown;
		EasyTouch.On_TouchUp -= On_TouchUp;
		EasyTouch.On_SimpleTap -= On_SimpleTap;
		EasyTouch.On_DoubleTap -= On_DoubleTap;
		EasyTouch.On_LongTapStart -=On_LongTapStart;
		EasyTouch.On_LongTap -= On_LongTap;
		EasyTouch.On_LongTapEnd -= On_LongTapEnd;
		EasyTouch.On_DragStart -= On_DragStart;
		EasyTouch.On_Drag -= On_Drag;
		EasyTouch.On_DragEnd -= On_DragEnd;
		EasyTouch.On_SwipeStart -= On_SwipeStart;
		EasyTouch.On_Swipe -= On_Swipe;
		EasyTouch.On_SwipeEnd -= On_SwipeEnd;
		EasyTouch.On_TouchStart2Fingers -= On_TouchStart2Fingers;
		EasyTouch.On_TouchDown2Fingers -= On_TouchDown2Fingers;
		EasyTouch.On_TouchUp2Fingers -= On_TouchUp2Fingers;
		EasyTouch.On_SimpleTap2Fingers -= On_SimpleTap2Fingers;
		EasyTouch.On_DoubleTap2Fingers -= On_DoubleTap2Fingers;
		EasyTouch.On_LongTapStart2Fingers -= On_LongTapStart2Fingers;
		EasyTouch.On_LongTap2Fingers -= On_LongTap2Fingers;
		EasyTouch.On_LongTapEnd2Fingers -= On_LongTapEnd2Fingers;
		EasyTouch.On_Twist -= On_Twist;
		EasyTouch.On_TwistEnd -= On_TwistEnd;
		EasyTouch.On_PinchIn -= On_PinchIn;
		EasyTouch.On_PinchOut -= On_PinchOut;
		EasyTouch.On_PinchEnd -= On_PinchEnd;
		EasyTouch.On_DragStart2Fingers -= On_DragStart2Fingers;
		EasyTouch.On_Drag2Fingers -= On_Drag2Fingers;
		EasyTouch.On_DragEnd2Fingers -= On_DragEnd2Fingers;
		EasyTouch.On_SwipeStart2Fingers -= On_SwipeStart2Fingers;
		EasyTouch.On_Swipe2Fingers -= On_Swipe2Fingers;
		EasyTouch.On_SwipeEnd2Fingers -= On_SwipeEnd2Fingers;		
	}

  private void On_Cancel(EasyTouchGesture gesture)
  {
  }

  private void On_TouchStart(EasyTouchGesture gesture)
  {
  }

  private void On_TouchDown(EasyTouchGesture gesture)
  {
  }

  private void On_TouchUp(EasyTouchGesture gesture)
  {
  }

  private void On_SimpleTap(EasyTouchGesture gesture)
  {
  }

  private void On_DoubleTap(EasyTouchGesture gesture)
  {
  }

  private void On_LongTapStart(EasyTouchGesture gesture)
  {
  }

  private void On_LongTap(EasyTouchGesture gesture)
  {
  }

  private void On_LongTapEnd(EasyTouchGesture gesture)
  {
  }

  private void On_DragStart(EasyTouchGesture gesture)
  {
  }

  private void On_Drag(EasyTouchGesture gesture)
  {
  }

  private void On_DragEnd(EasyTouchGesture gesture)
  {
  }

  private void On_SwipeStart(EasyTouchGesture gesture)
  {
  }

  private void On_Swipe(EasyTouchGesture gesture)
  {
  }

  private void On_SwipeEnd(EasyTouchGesture gesture)
  {
  }

  private void On_TouchStart2Fingers(EasyTouchGesture gesture)
  {
  }

  private void On_TouchDown2Fingers(EasyTouchGesture gesture)
  {
  }

  private void On_TouchUp2Fingers(EasyTouchGesture gesture)
  {
  }

  private void On_SimpleTap2Fingers(EasyTouchGesture gesture)
  {
  }

  private void On_DoubleTap2Fingers(EasyTouchGesture gesture)
  {
  }

  private void On_LongTapStart2Fingers(EasyTouchGesture gesture)
  {
  }

  private void On_LongTap2Fingers(EasyTouchGesture gesture)
  {
  }

  private void On_LongTapEnd2Fingers(EasyTouchGesture gesture)
  {
  }

  private void On_Twist(EasyTouchGesture gesture)
  {
  }

  private void On_TwistEnd(EasyTouchGesture gesture)
  {
  }

  private void On_PinchIn(EasyTouchGesture gesture)
  {
  }

  private void On_PinchOut(EasyTouchGesture gesture)
  {
  }

  private void On_PinchEnd(EasyTouchGesture gesture)
  {
  }

  private void On_DragStart2Fingers(EasyTouchGesture gesture)
  {
  }

  private void On_Drag2Fingers(EasyTouchGesture gesture)
  {
  }

  private void On_DragEnd2Fingers(EasyTouchGesture gesture)
  {
  }

  private void On_SwipeStart2Fingers(EasyTouchGesture gesture)
  {
  }

  private void On_Swipe2Fingers(EasyTouchGesture gesture)
  {
  }

  private void On_SwipeEnd2Fingers(EasyTouchGesture gesture)
  {
  }
}
