using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region CONSTANTS
    private const string CHRONO_TRIGGER_SPEND_TIME = "spend_time";
    #endregion

    #region ATTRIBUTES
    [Header("Game management")]
    [Space]
    private GameStateManager gameStateManager;

    [Header("UI parts")]
    [Space]
    public GameObject UIMainAnchor;
    public GameObject chronoBar;
    public GameObject actionBarsAnchor;
    public GameObject bowActionBar;
    public GameObject bombActionBar;
    public GameObject portalActionBar;

    [Header("Pause menu management")]
    [Space]
    public GameObject mainMenuUI;
    public GameObject mainMenuText;
    public GameObject confirmText;

    [Header("HUD management")]
    [Space]
    public Animator chronoAnimator;
    public Slider chronoRealTimeSlider;
    public Slider chronoLostTimeSlider;
    public TextMeshProUGUI chronoRealTimeText;
    public TextMeshProUGUI chronoLostTimeText;
    public GameObject bombCross;
    public GameObject portalCross;

    public TextMeshProUGUI arrowInfoText;
    public TextMeshProUGUI bombInfoText;
    public TextMeshProUGUI portalInfoText;
    private GameObject player;
    public Transform chronoTooltipAnchor;
    public GameObject prefabChonoTooltip;
    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        gameStateManager = GetComponent<GameStateManager>();
        UIMainAnchor.SetActive(true);
    }

    private void OnEnable()
    {
        ChronoTimeManager.OnTimeSpend += CBChronoTimeManagerOnTimeSpend;
        gameStateManager.OnGameStarted += CBGameStateManagerOnGameStarted;
    }

    private void OnDisable()
    {
        ChronoTimeManager.OnTimeSpend -= CBChronoTimeManagerOnTimeSpend;
        gameStateManager.OnGameStarted -= CBGameStateManagerOnGameStarted;
    }

    private void Update()
    {
        if (gameStateManager.gameMode == GameMode.Game)
        {
            ManageGameUI();
        }
        else
        {
            ManageJailUI();
        }
    }

    #endregion

    #region METHODS
    private void ManageJailUI()
    {
        if (chronoBar.activeSelf)
        {
            chronoBar.SetActive(false);
        }

        ManagePlayerUI();
    }

    private void ManageGameUI()
    {
        if (!chronoBar.activeSelf)
        {
            chronoBar.SetActive(true);
        }

        ManageMenu();
        ManagePlayerUI();
        ManageChronoBar();
    }

    private void ManageMenu()
    {
        switch (gameStateManager.gameState)
        {
            case GameState.Play:
                mainMenuUI.SetActive(false);
                break;
            case GameState.Pause:
                if (!mainMenuUI.activeSelf)
                {
                    mainMenuUI.SetActive(true);
                    mainMenuText.SetActive(true);
                    confirmText.SetActive(false);
                }
                break;
            case GameState.ConfirmQuit:
                if ((mainMenuUI.activeSelf) && (mainMenuText.activeSelf))
                {
                    mainMenuText.SetActive(false);
                    confirmText.SetActive(true);
                }
                break;
        }
    }

    private void ManageChronoBar()
    {
        chronoLostTimeSlider.value = ChronoTimeManager.ActualTime / ChronoTimeManager.MaxTime;
        TimeSpan t1 = TimeSpan.FromSeconds(ChronoTimeManager.ActualTime);
        chronoLostTimeText.text = string.Format("{0:D2}:{1:D2}",t1.Minutes, t1.Seconds);

        chronoRealTimeSlider.value = ChronoTimeManager.RealTime / ChronoTimeManager.MaxTime;
        TimeSpan t2 = TimeSpan.FromSeconds(ChronoTimeManager.RealTime);
        chronoRealTimeText.text = string.Format("{0:D2}:{1:D2}", t2.Minutes, t2.Seconds);
    }

    private void ManagePlayerUI()
    {
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag(GlobalConstants.TAG_PLAYER);
        }
        else
        {
            if (!actionBarsAnchor.activeSelf)
            {
                actionBarsAnchor.SetActive(true);
            }

            bowActionBar.SetActive(player.GetComponent<PlayerManagement>().BowEnabled);
            bombActionBar.SetActive(player.GetComponent<PlayerManagement>().BombEnabled);
            portalActionBar.SetActive(player.GetComponent<PlayerManagement>().PortalEnabled);

            if (player.GetComponent<PlayerManagement>().BowEnabled)
            {
                arrowInfoText.text = string.Format("-{0:0.00}", FindObjectOfType<PlayerStatsManager>().GetPlayerStat(GlobalConstants.PLAYER_STAT_ARROW_COST).StatValue.ToString());
            }

            if (player.GetComponent<PlayerManagement>().BombEnabled)
            {
                bombCross.SetActive(player.GetComponent<PlayerManagement>().IsMoving);
                bombInfoText.text = string.Format("-{0:0.00}", FindObjectOfType<PlayerStatsManager>().GetPlayerStat(GlobalConstants.PLAYER_STAT_BOMB_COST).StatValue.ToString());
            }

            if (player.GetComponent<PlayerManagement>().PortalEnabled)
            {
                portalCross.SetActive(player.GetComponent<PlayerManagement>().PortalInRange);
                portalInfoText.text = string.Format("-{0:0.00}", FindObjectOfType<PlayerStatsManager>().GetPlayerStat(GlobalConstants.PLAYER_STAT_PORTAL_COST).StatValue.ToString());
            }
        }
    }
    #endregion

    #region CALLBACK
    private void CBGameStateManagerOnGameStarted()
    {

    }

    private void CBChronoTimeManagerOnTimeSpend(TimeDirection timeDirection, float value)
    {
        GameObject tempTooltip = Instantiate(prefabChonoTooltip, chronoTooltipAnchor.position, Quaternion.identity);
        tempTooltip.transform.SetParent(chronoTooltipAnchor);
        tempTooltip.transform.localPosition = new Vector3(15, 0, 0);
        tempTooltip.transform.localScale = new Vector3(1, 1, 1);
        tempTooltip.GetComponent<ChronoInfoTooltipManager>().Initialized(timeDirection, Mathf.CeilToInt(value));

        if (chronoAnimator)
        {
            chronoAnimator.SetTrigger(CHRONO_TRIGGER_SPEND_TIME);
        } 
    }
    #endregion
}
