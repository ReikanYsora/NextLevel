using UnityEngine;

public class ChronoTimeManager : MonoBehaviour
{
    #region ATTRIBUTES
    private PlayerStatsManager playerStatsManager;
    private GameStateManager gameStateManager;
    private bool newGameInitialized;

    [Header("Time values")]
    [Space]
    public static float MaxTime;
    public static float ActualTime;
    public static float RealTime;
    #endregion

    #region EVENTS
    public delegate void TimeSpend(TimeDirection timeDirection, float value);
    public static event TimeSpend OnTimeSpend;
    public delegate void TimeExhausted();
    public static event TimeExhausted OnTimeExhausted;
    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        playerStatsManager = GetComponent<PlayerStatsManager>();
        gameStateManager = GetComponent<GameStateManager>();
        newGameInitialized = false;
    }

    private void OnEnable()
    {
        gameStateManager.OnGameStarted += CBGameStateManagerOnGameStarted;
    }

    private void OnDisable()
    {
        gameStateManager.OnGameStarted += CBGameStateManagerOnGameStarted;
    }

    private void Update()
    {
        if ((gameStateManager.gameMode == GameMode.Game) && (gameStateManager.gameState == GameState.Play) && newGameInitialized)
        {
            ActualTime -= Time.deltaTime;
            RealTime -= Time.deltaTime;

            if (ActualTime <= 0.0f)
            {
                OnTimeExhausted?.Invoke();
            }
        }
    }
    #endregion

    #region METHODS
    public static void ChangeTime(TimeDirection timeDirection, float time)
    {
        switch (timeDirection)
        {
            case TimeDirection.More:
                if (ActualTime + time <= MaxTime)
                {
                    ActualTime += time;
                }
                else
                {
                    ActualTime = MaxTime;
                }
                break;
            case TimeDirection.Less:
                if (ActualTime - time >= 0)
                {
                    ActualTime -= time;
                }
                else
                {
                    ActualTime = 0;
                }
                break;
            default:
                break;
        }

        OnTimeSpend?.Invoke(timeDirection, time);
    }
    #endregion

    #region CALLBACKS
    private void CBGameStateManagerOnGameStarted()
    {
        MaxTime = playerStatsManager.GetPlayerStat(GlobalConstants.PLAYER_STAT_CHRONO_TIME_MAX).StatValue;
        ActualTime = MaxTime;
        RealTime = MaxTime;
        newGameInitialized = true;
    }
    #endregion
}

public enum TimeDirection
{
    More, Less
}
