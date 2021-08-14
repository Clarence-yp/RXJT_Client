using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class UVInfo {
  public GameObject Target;
  public Mesh Mesh;
  public Renderer Renderer;
  public Vector2 UVOff;
  public Vector3 Vertex;
  public int TriangleIndex = -1;
}
public class TriangleInfo {
  public GameObject Target;
  public Mesh Mesh;
  public Renderer Renderer;
  public List<Vector2> UVOffs = new List<Vector2>();
  public List<Vector2> TexOffs = new List<Vector2>();
  public List<Vector3> Vertexs = new List<Vector3>();
  public int TriangleIndex = -1;
  public override string ToString() {
    return string.Format("Target:{0} Mesh:{1} Renderer:{2} UVOffs:{3} TexOffs:{3} Vertexs:{3}",
      Target.name, Mesh.name, Renderer.name,
      UVOffs.ToArray().ToString(), TexOffs.ToArray().ToString(), Vertexs.ToArray().ToString());
  }
}
public class LocalProcessInfo {
  public bool IsAddMeshCollider = false;
  public int LayerMaskOld = -1;
}
public class LightmapHelper {
  public static TriangleInfo FindSelectedTriangle(List<TriangleInfo> triangles, Vector2 pos, Rect textureRect) {
    TriangleInfo retTriInfo = null;
    Vector2 leftTop = new Vector2(textureRect.xMin, textureRect.yMin);
    Vector2 selPos = pos - leftTop;
    foreach (TriangleInfo triInfo in triangles) {
      Vector2 A = triInfo.TexOffs[0];
      Vector2 B = triInfo.TexOffs[1];
      Vector2 C = triInfo.TexOffs[2];
      Vector2 P = selPos;
      bool isIn = GUIHelp.IsPointInTriangle(P, A, B, C);
      if (isIn) {
        retTriInfo = triInfo;
        break;
      }
    }
    return retTriInfo;
  }
  public static TriangleInfo FindSelectedTriangle(List<TriangleInfo> triangles, Vector3 pos) {
    TriangleInfo retTriInfo = null;
    foreach (TriangleInfo triInfo in triangles) {
      Vector3 A = triInfo.Vertexs[0];
      Vector3 B = triInfo.Vertexs[1];
      Vector3 C = triInfo.Vertexs[2];
      Vector3 P = pos;
      bool isIn = GUIHelp.IsPointInTriangle(P, A, B, C);
      if (isIn) {
        retTriInfo = triInfo;
        break;
      }
    }
    return retTriInfo;
  }
  public static TriangleInfo FindSelectedTriangleByIndex(List<TriangleInfo> triangles, int triIndex) {
    TriangleInfo retTriInfo = null;
    foreach (TriangleInfo triInfo in triangles) {
      if (triInfo.TriangleIndex == triIndex) {
        retTriInfo = triInfo;
        break;
      }
    }
    return retTriInfo;
  }
  public static List<List<UVInfo>> CaculateUV(GameObject target) {
    List<List<UVInfo>> retGroup = new List<List<UVInfo>>();

    List<Mesh> meshList = new List<Mesh>();
    List<Renderer> renderList = new List<Renderer>();
    // Test for SMF first
    SkinnedMeshRenderer[] smrs = target.GetComponents<SkinnedMeshRenderer>();
    if (smrs.Length > 0) {
      foreach (SkinnedMeshRenderer smr in smrs) {
        meshList.Add(smr.sharedMesh);
        renderList.Add(smr.GetComponent<Renderer>());
      }
    } else {
      MeshRenderer[] mrs = target.GetComponents<MeshRenderer>();
      MeshFilter[] mfs = target.GetComponents<MeshFilter>();
      renderList.AddRange(mrs);
      foreach (MeshFilter mf in mfs) {
        meshList.Add(mf.sharedMesh);
      }
    }
    Mesh[] meshes = meshList.ToArray();
    Renderer[] renderers = renderList.ToArray();
    for (int mIndex = 0; mIndex < meshes.Length; mIndex++) {
      List<UVInfo> retUV = new List<UVInfo>();

      Mesh mesh = meshes[mIndex];
      Renderer renderer = renderers[mIndex];

      if (renderer.lightmapIndex < 0
        || LightmapSettings.lightmaps == null
        || renderer.lightmapIndex >= LightmapSettings.lightmaps.Length) {
        continue;
      }

      //LightmapData lmData = LightmapSettings.lightmaps[renderer.lightmapIndex];
      Vector4 offset = renderer.lightmapScaleOffset;

      Vector2 Tilling = new Vector2(offset.x, offset.y);
      Vector2 Offset = new Vector2(offset.z, offset.w);
      //Texture2D lm = lmData.lightmapFar;

      Vector2[] uvs = mesh.uv2;
      int[] triangles = mesh.triangles;
      if (triangles == null || triangles.Length % 3 != 0
        || uvs.Length != mesh.vertices.Length) {
        UnityEngine.Debug.Log(string.Format("CaculateUV Error"));
        continue;
      }

      for (int tIndex = 0; tIndex < triangles.Length; tIndex++) {
        int vIndex1 = triangles[tIndex];
        Vector2 uv = uvs[vIndex1];
        float uvx = ((Tilling.x * uv.x) + Offset.x);
        float uvy = (Tilling.y * uv.y) + Offset.y;
        Vector2 uvOff = new Vector2(uvx, uvy);
        UVInfo info = new UVInfo();
        info.UVOff = uvOff;
        info.Vertex = mesh.vertices[vIndex1];
        info.Target = target;
        info.Mesh = mesh;
        info.Renderer = renderer;
        info.TriangleIndex = Mathf.RoundToInt(tIndex / 3);
        retUV.Add(info);
      }
      retGroup.Add(retUV);
    }
    return retGroup;
  }
  public static List<TriangleInfo> CaculateTex(List<List<UVInfo>> group, Rect textureRect) {
    List<TriangleInfo> triList = new List<TriangleInfo>();
    if (group != null && group.Count > 0) {
      foreach (List<UVInfo> uvs in group) {
        for (int index = 0; index < uvs.Count; index += 3) {
          if ((index + 2) < uvs.Count) {
            UVInfo uvInfo1 = uvs[index];
            UVInfo uvInfo2 = uvs[index + 1];
            UVInfo uvInfo3 = uvs[index + 2];

            Vector2 curUV1 = uvInfo1.UVOff;
            Vector2 curUV2 = uvInfo2.UVOff;
            Vector2 curUV3 = uvInfo3.UVOff;
            Vector2 pointA = new Vector2(curUV1.x * textureRect.width, (1 - curUV1.y) * textureRect.height);
            Vector2 pointB = new Vector2(curUV2.x * textureRect.width, (1 - curUV2.y) * textureRect.height);
            Vector2 pointC = new Vector2(curUV3.x * textureRect.width, (1 - curUV3.y) * textureRect.height);

            TriangleInfo triInfo = new TriangleInfo();
            triInfo.Target = uvInfo1.Target;
            triInfo.Mesh = uvInfo1.Mesh;
            triInfo.Renderer = uvInfo1.Renderer;
            triInfo.TriangleIndex = uvInfo1.TriangleIndex;

            triInfo.UVOffs.Clear();
            triInfo.UVOffs.Add(curUV1);
            triInfo.UVOffs.Add(curUV2);
            triInfo.UVOffs.Add(curUV3);

            triInfo.TexOffs.Clear();
            triInfo.TexOffs.Add(pointA);
            triInfo.TexOffs.Add(pointB);
            triInfo.TexOffs.Add(pointC);

            triInfo.Vertexs.Clear();
            triInfo.Vertexs.Add(uvInfo1.Vertex);
            triInfo.Vertexs.Add(uvInfo2.Vertex);
            triInfo.Vertexs.Add(uvInfo3.Vertex);

            triList.Add(triInfo);
          }
        }
      }
    }
    return triList;
  }
  public static LocalProcessInfo PreProcessObject(GameObject target, int targetLayerMask) {
    LocalProcessInfo procInfo = new LocalProcessInfo();
    if (target == null) {
      return procInfo;
    }
    if (target.GetComponent<MeshCollider>() == null) {
      procInfo.IsAddMeshCollider = true;
      target.AddComponent<MeshCollider>();
    }
    procInfo.LayerMaskOld = target.layer;
    target.layer = targetLayerMask;

    return procInfo;
  }
  public static void PostProcessObject(GameObject target, LocalProcessInfo info) {
    if (target == null || info == null) return;
    if (info.IsAddMeshCollider) {
      if (target.GetComponent<MeshCollider>() != null) {
        GameObject.DestroyImmediate(target.GetComponent<MeshCollider>());
      }
    }
    if (info.LayerMaskOld >= 0 && info.LayerMaskOld <= 31) {
      target.layer = info.LayerMaskOld;
    }
  }
}