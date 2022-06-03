using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "new EventSystemParameter", menuName = "Scriptable Objects/EventSystemParameter")]
public class EventSystemParameter : ScriptableObject
{
    public string Name;
    public float Value;
}
