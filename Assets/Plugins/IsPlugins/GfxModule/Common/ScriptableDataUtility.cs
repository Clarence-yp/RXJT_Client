using System;
using System.Collections.Generic;
using UnityEngine;

namespace GfxModule
{
  public static class ScriptableDataUtility
  {
    public static Vector2 CalcVector2(ScriptableData.CallData callData)
    {
      if (null == callData || callData.GetId() != "vector2")
        return Vector2.zero;
      int num = callData.GetParamNum();
      if (2 == num) {
        float x = float.Parse(callData.GetParamId(0));
        float y = float.Parse(callData.GetParamId(1));
        return new Vector2(x, y);
      } else {
        return Vector2.zero;
      }
    }
    public static Vector3 CalcVector3(ScriptableData.CallData callData)
    {
      if (null == callData || callData.GetId() != "vector3")
        return Vector3.zero;
      int num = callData.GetParamNum();
      if (3 == num) {
        float x = float.Parse(callData.GetParamId(0));
        float y = float.Parse(callData.GetParamId(1));
        float z = float.Parse(callData.GetParamId(2));
        return new Vector3(x, y, z);
      } else {
        return Vector3.zero;
      }
    }
    public static Vector4 CalcVector4(ScriptableData.CallData callData)
    {
      if (null == callData || callData.GetId() != "vector4")
        return Vector4.zero;
      int num = callData.GetParamNum();
      if (4 == num) {
        float x = float.Parse(callData.GetParamId(0));
        float y = float.Parse(callData.GetParamId(1));
        float z = float.Parse(callData.GetParamId(2));
        float w = float.Parse(callData.GetParamId(3));
        return new Vector4(x, y, z, w);
      } else {
        return Vector4.zero;
      }
    }
    public static Quaternion CalcQuaternion(ScriptableData.CallData callData)
    {
      if (null == callData || callData.GetId() != "quaternion")
        return Quaternion.identity;
      int num = callData.GetParamNum();
      if (4 == num) {
        float x = float.Parse(callData.GetParamId(0));
        float y = float.Parse(callData.GetParamId(1));
        float z = float.Parse(callData.GetParamId(2));
        float w = float.Parse(callData.GetParamId(3));
        return new Quaternion(x, y, z, w);
      } else {
        return Quaternion.identity;
      }
    }
    public static Quaternion CalcEularRotation(ScriptableData.CallData callData)
    {
      if (null == callData || callData.GetId() != "eular")
        return Quaternion.identity;
      int num = callData.GetParamNum();
      if (3 == num) {
        float x = float.Parse(callData.GetParamId(0));
        float y = float.Parse(callData.GetParamId(1));
        float z = float.Parse(callData.GetParamId(2));
        return Quaternion.Euler(x, y, z);
      } else {
        return Quaternion.identity;
      }
    }
  }
}
