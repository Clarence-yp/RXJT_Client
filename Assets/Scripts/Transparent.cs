using UnityEngine;
using System.Collections.Generic;
using DashFire;

public class Transparent : MonoBehaviour
{
  // Use this for initialization
  void Start()
  {
    m_LayerMask = (1 << LayerMask.NameToLayer("Default"));
    m_TransprentShader = Shader.Find("Transparent/Diffuse");
  }

  // Update is called once per frame
  void Update()
  {
    GameObject target = LogicSystem.PlayerSelf;
    if (null == target || null == m_TransprentShader) {
      return;
    }
    Vector3 targetPos = target.transform.position;
    Vector3 dir = targetPos - transform.position;
    float distance = dir.magnitude;
    
    m_CurRenderers.Clear();
    RaycastHit[] hits;
    hits = Physics.RaycastAll(transform.position, transform.forward, distance, m_LayerMask);
    int i = 0;
    while (i < hits.Length) {
      RaycastHit hit = hits[i];
      Renderer renderer = hit.collider.GetComponent<Renderer>();
      if (renderer) {
        if (!m_OriginalShaders.ContainsKey(renderer)) {
          List<Shader> shaders = new List<Shader>();
          for (int ix = 0; ix < renderer.materials.Length; ++ix) {
            Material mat = renderer.materials[ix];
            shaders.Add(mat.shader);
          }
          m_OriginalShaders.Add(renderer, shaders);
        }
        for (int ix = 0; ix < renderer.materials.Length; ++ix) {
          Material mat = renderer.materials[ix];
          mat.shader = m_TransprentShader;
          Color c = mat.color;
          c.a = 0.3f;
          mat.color = c;
        }

        m_CurRenderers.Add(renderer);
        m_LastRenderers.Remove(renderer);
      }
      ++i;
    }
    foreach (Renderer renderer in m_LastRenderers) {
      List<Shader> shaders = m_OriginalShaders[renderer];
      if (null != shaders && shaders.Count == renderer.materials.Length) {
        for (int ix = 0; ix < renderer.materials.Length; ++ix) {
          renderer.material.shader = shaders[ix];
        }
      }
    }
    HashSet<Renderer> temp = m_LastRenderers;
    m_LastRenderers = m_CurRenderers;
    m_CurRenderers = temp;
  }

  private int m_LayerMask = 0;
  private Shader m_TransprentShader = null;
  private Dictionary<Renderer, List<Shader>> m_OriginalShaders = new Dictionary<Renderer, List<Shader>>();
  private HashSet<Renderer> m_LastRenderers = new HashSet<Renderer>();
  private HashSet<Renderer> m_CurRenderers = new HashSet<Renderer>(); 
}
