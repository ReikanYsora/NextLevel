using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Users;

public class InputManager : MonoBehaviour
{
	#region ATTRIBUTES
	private GameStateManager gameStateManager;
	private InputMaster inputMaster;
	private GameObject player;
	public InputType ActualInput { set; get; }
	#endregion

	#region EVENTS
	public delegate void InputChanged(InputType inputType);
	public event InputChanged OnInputChanged;
	public delegate void DeviceDisconnected();
	public event DeviceDisconnected OnDeviceDisconnected;
	public delegate void DeviceReconnected();
	public event DeviceReconnected OnDeviceReconnected;
	public delegate void PauseResumeAsked();
	public event PauseResumeAsked OnPauseResumeAsked;
	public delegate void PauseQuitConfirmed();
	public event PauseQuitConfirmed OnQuitGameAsked;
	public delegate void ValidatePressed();
	public event ValidatePressed OnValidatePressed;
	#endregion

	#region UNITY METHODS
	private void Awake()
    {
		gameStateManager = GetComponent<GameStateManager>();
		PlayerInput input = FindObjectOfType<PlayerInput>();
		inputMaster = new InputMaster();
	}

	private void OnEnable()
	{
		inputMaster.InGame.Enable();
		inputMaster.InGame.MenuIN.performed += CBOnMenuInPerformed;
		InputSystem.onDeviceChange += CBOnInputSystemDeviceChanged;
		inputMaster.InGame.MenuOUT.performed += CBOnMenuOutPerformed;
		inputMaster.InGame.Bomb.performed += CBOnBombPerformed;
		inputMaster.InGame.Portal.performed += CBOnPortalPerformed;
		inputMaster.InGame.Validate.performed += CBOnValidatePerformed;
		InputUser.onChange += CBOnInputChanged;
		OnInputChanged?.Invoke(InputType.KeyboardMouse);
	}

	private void OnDisable()
	{
		inputMaster.InGame.Disable();
		inputMaster.InGame.MenuIN.performed -= CBOnMenuInPerformed;
		InputSystem.onDeviceChange -= CBOnInputSystemDeviceChanged;
		inputMaster.InGame.MenuOUT.performed -= CBOnMenuOutPerformed;
		inputMaster.InGame.Bomb.performed -= CBOnBombPerformed;
		inputMaster.InGame.Portal.performed -= CBOnPortalPerformed;
		inputMaster.InGame.Validate.performed -= CBOnValidatePerformed;
		InputSystem.onDeviceChange -= CBOnInputSystemDeviceChanged;
		InputUser.onChange -= CBOnInputChanged;
	}

	private void FixedUpdate()
	{
		if (!player)
		{
			player = GameObject.FindGameObjectWithTag(GlobalConstants.TAG_PLAYER);
		}
		else
		{
			player.GetComponent<PlayerManagement>().InputDirection = inputMaster.InGame.Movement.ReadValue<Vector2>();
			player.GetComponent<PlayerManagement>().Bow = inputMaster.InGame.Bow.ReadValue<float>() > 0 ? true : false;
			player.GetComponent<PlayerManagement>().Bomb = false;
			player.GetComponent<PlayerManagement>().Portal = false;
			player.GetComponent<PlayerManagement>().Action = false;
		}
	}
	#endregion

	#region CALLBACKS
	private void CBOnMenuInPerformed(InputAction.CallbackContext context)
	{
		OnPauseResumeAsked?.Invoke();
	}

	private void CBOnMenuOutPerformed(InputAction.CallbackContext obj)
	{
		if (gameStateManager.gameState == GameState.Pause)
		{
			gameStateManager.SwitchToConfirmBackMenu();
		}
		else if (gameStateManager.gameState == GameState.ConfirmQuit)
		{
			OnQuitGameAsked?.Invoke();
		}
	}
	private void CBOnPortalPerformed(InputAction.CallbackContext context)
	{
		player.GetComponent<PlayerManagement>().Portal = true;
	}

	private void CBOnValidatePerformed(InputAction.CallbackContext context)
	{
		player.GetComponent<PlayerManagement>().Action = true;
		OnValidatePressed?.Invoke();
	}

	private void CBOnBombPerformed(InputAction.CallbackContext context)
	{
		player.GetComponent<PlayerManagement>().Bomb = true;
	}

	private void CBOnInputChanged(InputUser user, InputUserChange change, InputDevice device)
	{
		if (change == InputUserChange.ControlSchemeChanged)
		{
			if (user.controlScheme.Value.name == "GamePad")
			{
				ActualInput = InputType.Gamepad;
			}
			else
			{
				ActualInput = InputType.KeyboardMouse;
			}

			OnInputChanged?.Invoke(ActualInput);
		}
	}

	private void CBOnInputSystemDeviceChanged(InputDevice device, InputDeviceChange inputType)
	{
		if (inputType == InputDeviceChange.Disconnected)
		{
			OnDeviceDisconnected?.Invoke();
		}

		if (inputType == InputDeviceChange.Reconnected)
		{
			OnDeviceReconnected?.Invoke();
		}
	}
	#endregion
}

public enum InputType
{
	KeyboardMouse, Gamepad
}
