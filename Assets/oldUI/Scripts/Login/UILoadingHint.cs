using UnityEngine;
using DashFire;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class UILoadingHint : MonoBehaviour {

  public UILabel lblHint = null;
  private int m_Amount = 1;
  private const int Number = 6;
  public float Delta = 0.3f;
	// Use this for initialization
	void Start () {
  }
	
	// Update is called once per frame
	void Update () {
    Delta -= RealTime.deltaTime;
    if (Delta <= 0) {
      m_Amount = m_Amount % 7;
      if (m_Amount == 0) m_Amount = 1;
      UpdateState(m_Amount++);
      Delta = 0.3f;
    }
	}
  void UpdateState(int amount)
  {
    StringBuilder sb = new StringBuilder(DashFire.StrDictionaryProvider.Instance.GetDictString(144));
    sb.Append('.',amount);
    if (lblHint != null)
      lblHint.text = sb.ToString();
  }

}
