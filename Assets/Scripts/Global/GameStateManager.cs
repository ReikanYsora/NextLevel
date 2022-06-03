using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
	#region ATTRIBUTES
	[Header("Game mode managements")]
	[Space]
	public GameMode gameMode;
	public GameState gameState;
	public InputManager inputManager;

	[Header("Dungeon management")]
	[Space]
	public DungeonMapManager dungeonMapManager;

	[Header("Player management")]
	[Space]
	public GameObject prefabPlayer;
	public Transform playerInstantiationPosition;
	private GameObject player;

	public bool StartScene;
	#endregion

	#region EVENTS
	public delegate void GameStarted();
	public event GameStarted OnGameStarted;
	#endregion

	#region UNITY METHODS
	private void Awake()
	{
		gameMode = GameMode.None;
		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		if (StartScene)
		{
			dungeonMapManager.OnDungeonGenerationCompleted += CBDungeonMapManagerOnDungeonGenerationCompleted;
			dungeonMapManager.GenerateDungeon();
		}
		else
		{
			gameMode = GameMode.Jail;
			GetComponent<AmbianceManager>().ChangeAmbiance(gameMode);
			player = Instantiate(prefabPlayer, playerInstantiationPosition.position, Quaternion.identity);
			PlayerManagement playerManagement = player.GetComponent<PlayerManagement>();
			playerManagement.BowEnabled = false;
			playerManagement.BombEnabled = false;
			playerManagement.PortalEnabled = false;
		}
	}

	private void OnEnable()
	{
		ChronoTimeManager.OnTimeExhausted += CBChronoTimeManagerOnTimeExhausted;
		inputManager.OnPauseResumeAsked += CBInputManagerOnPauseResumeAsked;
		inputManager.OnQuitGameAsked += CBInputOnQuitGameAsked;
	}

	private void OnDisable()
	{
		ChronoTimeManager.OnTimeExhausted -= CBChronoTimeManagerOnTimeExhausted;
		inputManager.OnPauseResumeAsked -= CBInputManagerOnPauseResumeAsked;
		inputManager.OnQuitGameAsked -= CBInputOnQuitGameAsked;
	}
	#endregion

	#region METHODS
	public void StartGame()
	{
		OnGameStarted?.Invoke();
	}

	public void SwitchToConfirmBackMenu()
	{
		if ((gameState == GameState.Pause) && (gameMode == GameMode.Game))
		{
			gameState = GameState.ConfirmQuit;
		}
	}

	public void ChangePlayerPosition(Vector3 playerPosition)
	{
		player.transform.position = playerPosition;
	}
	#endregion

	#region CALLBACKS
	private void CBDungeonMapManagerOnDungeonGenerationCompleted(DungeonPart startPart)
	{
		gameMode = GameMode.Game;
		GetComponent<AmbianceManager>().ChangeAmbiance(gameMode);
		player = Instantiate(prefabPlayer, startPart.startPosition.position, Quaternion.identity);
		PlayerManagement playerManagement = player.GetComponent<PlayerManagement>();
		playerManagement.BowEnabled = true;
		playerManagement.BombEnabled = true;
		playerManagement.PortalEnabled = true;
		gameState = GameState.Play;

		StartGame();
	}

	private void CBInputManagerOnPauseResumeAsked()
	{
		if (gameMode == GameMode.Game)
		{
			switch (gameState)
			{
				case GameState.Play:
					Time.timeScale = 0f;
					gameState = GameState.Pause;
					break;
				case GameState.ConfirmQuit:
				case GameState.Pause:
					Time.timeScale = 1f;
					gameState = GameState.Play;
					break;
			}
		}		
	}

	private void CBChronoTimeManagerOnTimeExhausted()
	{
		gameState = GameState.TimeOver;
		Time.timeScale = 0f;
	}

	public void CBInputOnQuitGameAsked()
	{
		Application.Quit();
	}
	#endregion
}

public enum GameState
{
	Play, Pause, ConfirmQuit, TimeOver
}

public enum GameMode
{
	None, Jail, Game
}