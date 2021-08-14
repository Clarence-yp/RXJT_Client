using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

public class LightmapGraphic {
  public static void PaintTextureTriangles(List<TriangleInfo> triangles, Rect textureRect, Color color) {
    Vector2 leftTop = new Vector2(textureRect.xMin, textureRect.yMin);
    foreach (TriangleInfo triInfo in triangles) {
      Vector2 pointA = triInfo.TexOffs[0] + leftTop;
      Vector2 pointB = triInfo.TexOffs[1] + leftTop;
      Vector2 pointC = triInfo.TexOffs[2] + leftTop;
      GUIGraphic.DrawLine(pointA, pointB, color);
      GUIGraphic.DrawLine(pointA, pointC, color);
      GUIGraphic.DrawLine(pointB, pointC, color);
    }
  }
  public static void PaintTextureTriangle(TriangleInfo triInfo, Rect textureRect, Color color) {
    if (triInfo == null || triInfo.Vertexs == null || triInfo.Vertexs.Count != 3) {
      return;
    }
    Vector2 leftTop = new Vector2(textureRect.xMin, textureRect.yMin);
    Vector2 OffMin = new Vector2(0.5f, 0.5f);
    for (int index = 0; index < 1; index++) {
      Vector2 Off = OffMin * index;
      Vector2 pointA = triInfo.TexOffs[0] + leftTop + Off;
      Vector2 pointB = triInfo.TexOffs[1] + leftTop + Off;
      Vector2 pointC = triInfo.TexOffs[2] + leftTop + Off;
      GUIGraphic.DrawLine(pointA, pointB, color);
      GUIGraphic.DrawLine(pointA, pointC, color);
      GUIGraphic.DrawLine(pointB, pointC, color);
    }
  }
  public static void PaintSceneTriangle(TriangleInfo triInfo) {
    if (triInfo == null || triInfo.Vertexs == null || triInfo.Vertexs.Count != 3) {
      return;
    }
    List<Vector3> vertexs = triInfo.Vertexs;
    Transform transform = triInfo.Renderer.transform;
    Vector3 pointA = transform.TransformPoint(vertexs[0]);
    Vector3 pointB = transform.TransformPoint(vertexs[1]);
    Vector3 pointC = transform.TransformPoint(vertexs[2]);
    Color oldColor = Handles.color;
    Handles.color = Color.red;
    Handles.DrawLine(pointA, pointB);
    Handles.DrawLine(pointA, pointC);
    Handles.DrawLine(pointB, pointC);
    Handles.color = oldColor;
  }

}