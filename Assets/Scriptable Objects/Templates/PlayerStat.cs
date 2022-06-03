using UnityEngine;

[CreateAssetMenu(fileName = "new PlayerStat", menuName = "Scriptable Objects/PlayerStats")]
public class PlayerStat : ScriptableObject
{
    public string StatName;
    public float StatValue;
}
