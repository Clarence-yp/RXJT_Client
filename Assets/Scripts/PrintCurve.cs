using UnityEngine;
using System.Collections;

public class PrintCurve: MonoBehaviour {
  public AnimationCurve m_Curve;
	// Use this for initialization
	void Start () {
    if (m_Curve == null) {
      return;
    }
    string result = "";
    foreach (Keyframe key in m_Curve.keys) {
      result = result + "keyframe(" + key.time + ", " + key.value + ", " + key.inTangent + ", " + key.outTangent + ");\n";
    }
    Debug.Log(result);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
