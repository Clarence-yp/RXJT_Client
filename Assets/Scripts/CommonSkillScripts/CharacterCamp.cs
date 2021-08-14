using DashFire;
using UnityEngine;
using System.Collections;


public class CharacterCamp : MonoBehaviour
{
    public int m_CampId;
    [HideInInspector]
    public bool m_IsEndure = false; // 是否霸体。

    public void SetEndure(bool isEndure)
    {
        m_IsEndure = isEndure;
    }
}