using System;
using UnityEngine;

[Serializable]
public class PortalSlot
{
    public GameObject portal;
    public GameObject activeDungeonPart;
    public PortalColor color;
}

public enum PortalColor
{
    Blue, Orange
}
