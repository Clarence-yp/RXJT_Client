using UnityEngine;

public class ScreenBloom: MonoBehaviour
{
  //Color
  public Color m_Color;

  public Shader shader;
  private Material m_Material;
  private bool m_IsActive = false;
  private float m_Offset = 0.0f;
  private long m_TotalTime = 0;
  //Properties
  protected Material material
  {
    get
    {
      if (m_Material == null) {
        m_Material = new Material(shader);
        m_Material.hideFlags = HideFlags.HideAndDontSave;
      }
      return m_Material;
    }
  }
  //Methods
  private void Update()
  {
    if (m_IsActive) {
      float offset = m_Offset * Time.deltaTime * 1000;
      m_Color += new Color(offset, offset, offset, 0);
      m_TotalTime = (long)(m_TotalTime - Time.deltaTime * 1000);
      if (m_TotalTime < 0) {
        m_IsActive = false;
      }
    }
  }
  protected void OnDisable()
  {
    if (m_Material) {
      DestroyImmediate(m_Material);
    }
  }


  void OnEnable()
  {
    shader = Shader.Find("Hidden/DFM/BloomSimple");
  }
  // Called by camera to apply image effect
  void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    material.SetColor("_Color", m_Color);
    Graphics.Blit(source, destination, material);
  }

  // message
  void DimScreen(long time)
  {
    m_IsActive = true;
    m_TotalTime = time;
    m_Offset = 1.0f / time;
    m_Offset *= -1;
    m_Color = new Color(1, 1, 1, 1);
  }
  void LightScreen(long time)
  {
    m_IsActive = true;
    m_TotalTime = time;
    m_Offset = 1.0f / time;
    m_Color = new Color(0, 0, 0, 1);
  }
}

