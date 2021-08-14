using UnityEngine;
using System.Collections;

public class MonsterPrePower : MonoBehaviour {

	// Use this for initialization
	void Start () {
    progress = this.GetComponent<UIProgressBar>();
    progress.value = 0;
	}
	
	// Update is called once per frame
	void Update () {
    UpdatePos();
	  if(Duration >0)
    {
      if (progress != null)
        progress.value += RealTime.deltaTime / Duration;

      if (progress.value >= 1) {
        {
          NGUITools.SetActive(this.gameObject, false);
          Destroy(this.gameObject);
        }
      }
    }

	}

  private void UpdatePos()
  {
    Vector3 pos = Position;
    if (Camera.main != null)
      pos = Camera.main.WorldToScreenPoint(pos);
    pos.z = 0;
    Vector3 nguiPos = Vector3.zero;
    if (UICamera.mainCamera != null) {
      nguiPos = UICamera.mainCamera.ScreenToWorldPoint(pos);
    }
    if (this.transform != null) {
      this.transform.position = nguiPos;
    }
  }


  public float Duration
  {
    get
    {
      return m_Duration;
    }
    set
    {
      m_Duration = value;
    }
  }

  public int PowerId
  {
    get { return m_PowerId; }
    set
    {
      m_PowerId = value;
    }
  }
  public Vector3 Position
  {
    get { return m_Pos; }
    set
    {
      m_Pos = value;
    }
  }

  private UIProgressBar progress = null;
  private float m_Duration = 1f;
  private int m_PowerId = -1;
  private Vector3 m_Pos = new Vector3();

}
