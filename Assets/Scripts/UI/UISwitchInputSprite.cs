using System.Linq;
using UnityEngine;

public class UISwitchInputSprite : MonoBehaviour
{
    #region ATTRIBUTES
    public GameObject[] KeyboardSprite;
    public GameObject[] GamepadSprite;
    private InputManager inputManager;
    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        inputManager = FindObjectOfType<InputManager>();
    }

    private void OnEnable()
    {
        inputManager.OnInputChanged += CBInputManagerOnInputChanged;
    }

    private void OnDisable()
    {
        inputManager.OnInputChanged -= CBInputManagerOnInputChanged;
    }

    private void Update()
    {
        switch (inputManager.ActualInput)
        {
            default:
            case InputType.KeyboardMouse:
                KeyboardSprite.ToList().ForEach(x => x.SetActive(true));
                GamepadSprite.ToList().ForEach(x => x.SetActive(false));
                break;
            case InputType.Gamepad:
                KeyboardSprite.ToList().ForEach(x => x.SetActive(false));
                GamepadSprite.ToList().ForEach(x => x.SetActive(true));
                break;
        }
    }

    private void CBInputManagerOnInputChanged(InputType inputType)
    {
        switch (inputType)
        {
            default:
            case InputType.KeyboardMouse:
                KeyboardSprite.ToList().ForEach(x => x.SetActive(true));
                GamepadSprite.ToList().ForEach(x => x.SetActive(false));
                break;
            case InputType.Gamepad:
                KeyboardSprite.ToList().ForEach(x => x.SetActive(false));
                GamepadSprite.ToList().ForEach(x => x.SetActive(true));
                break;
        }
    }
    #endregion
}
