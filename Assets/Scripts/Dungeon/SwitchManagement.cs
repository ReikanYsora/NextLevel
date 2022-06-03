using UnityEngine;

public class SwitchManagement : MonoBehaviour
{
    #region CONSTANTS
    private const string PLAYER_TAG = "Player";
    private const string ANIMATOR_ACTIVATE_TRIGGER = "Activate";
    #endregion

    #region ATTRIBUTES
    public SwitchType switchType;
    private DungeonPart dungeonPart;
    private Animator animator;
    public Collider2D switchCollider;
    #endregion

    #region UNITY METHODS
    private void Start() 
    {
        dungeonPart = GetComponentInParent<DungeonPart>();
        animator = GetComponent<Animator>();
    }  

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (switchType)
        {
            default:
            case SwitchType.Key :
            case SwitchType.Action :
            break;
            case SwitchType.Walk :
            if (other.gameObject.CompareTag(PLAYER_TAG))
            {
                switchCollider.enabled = false;
                animator.SetTrigger(ANIMATOR_ACTIVATE_TRIGGER);
                dungeonPart.SendGameAction(this);
            }
            break;
        }
    }
    #endregion

    #region METHODS
    public void Active(GameObject origin, float value)
    {
        if (switchType == SwitchType.Action)
        {
            switchCollider.enabled = false;
            animator.SetTrigger(ANIMATOR_ACTIVATE_TRIGGER);
            dungeonPart.SendGameAction(this);
        }
    }
    #endregion
}

public enum SwitchType
{
    Action, Walk, Key
}
