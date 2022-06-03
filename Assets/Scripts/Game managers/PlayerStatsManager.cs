using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerStatsManager : MonoBehaviour
{
    #region ATTRIBUTES
    [SerializeField] public List<PlayerStat> PlayerStats;
    #endregion

    #region UNITY METHODS
    private void Update() 
    {
        
    }
    #endregion

    #region METHODS
    public PlayerStat GetPlayerStat(string statName)
    {
        return PlayerStats.Where(x => x.name == statName).FirstOrDefault();
    }
    #endregion
}
 