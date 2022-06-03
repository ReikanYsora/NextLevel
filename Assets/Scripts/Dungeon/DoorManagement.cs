using UnityEngine;

public class DoorManagement : MonoBehaviour
{
    #region CONSTANTS
    private const string MAIN_CAMERA_TAG = "MainCamera";
    private const string PLAYER_TAG = "Player";
    private const string ANIMATOR_TRIGGER_OPEN = "Open";
    private const string ANIMATOR_TRIGGER_CLOSE = "Close";
    #endregion

    #region ATTRIBUTES
    [SerializeField] public ExitPoint exitType;
    private bool doorState;
    private DungeonPart dungeonPart;
    private Animator animator;
    private GameObject mainCamera;
    public BoxCollider2D triggerDetection;
    [SerializeField] public GameObject SFX_DoorOpen;
    [SerializeField] public GameObject SFX_DoorClose;
    #endregion

    #region EVENTS
    public delegate void DetectPlayerEvent(DungeonPart gameObject, ExitPoint exitPoint, StartPoint startPoint);
    public event DetectPlayerEvent OnDetectPlayerEvent;
    #endregion
    
    #region UNITY METHODS
    private void Awake()
    {
        mainCamera =  GameObject.FindGameObjectWithTag(MAIN_CAMERA_TAG);
        animator = GetComponent<Animator>();
        dungeonPart = GetComponentInParent<DungeonPart>();
        dungeonPart.OnDoorOpeningOrder += Open;
        dungeonPart.OnDoorClosingOrder += Close;
        doorState = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GetComponentInParent<DungeonPart>().isInitialized)
        {
            if (collision.gameObject.CompareTag(PLAYER_TAG))
            {
                OnDetectPlayerEvent?.Invoke(dungeonPart, exitType, GetStartPointFromExitPoint(exitType));
            }
        }
    }
    #endregion

    #region METHODS
    public void InitializeDoor(bool isOpen)
    {
        doorState = true;

        if (!isOpen)
        {
            Close();  
        }
    }

    private void Close()
    {
        if (doorState)
        {
            doorState = false;
            triggerDetection.enabled = doorState;
            animator.SetTrigger(ANIMATOR_TRIGGER_CLOSE);
            Instantiate(SFX_DoorClose, transform.position, Quaternion.identity);
            mainCamera.GetComponent<CameraEffect>().Shake(2f, 1f);
        }
    }

    private void Open()
    {
        if (!doorState)
        {
            doorState = true;
            triggerDetection.enabled = doorState;
            animator.SetTrigger(ANIMATOR_TRIGGER_OPEN);
            Instantiate(SFX_DoorOpen, transform.position, Quaternion.identity);
            mainCamera.GetComponent<CameraEffect>().Shake(2f, 1f);
        }
    }

    private static StartPoint GetStartPointFromExitPoint(ExitPoint exit)
    {
        switch (exit)
        {
            default:
            case ExitPoint.Top:
                return StartPoint.Bottom;
            case ExitPoint.Bottom:
                return StartPoint.Top;
            case ExitPoint.Left:
                return StartPoint.Right;
            case ExitPoint.Right:
                return StartPoint.Left;
        }
    }
    #endregion
}
