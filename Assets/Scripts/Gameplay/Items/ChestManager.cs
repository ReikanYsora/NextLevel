using UnityEngine;

public class ChestManager : Actionnable
{
    #region CONSTANTS
    private const string ANIMATOR_IS_OPEN = "IsOpen";
    #endregion

    #region ATTRIBUTES
    [Header("Animator")]
    [Space]
    public Animator animator;
    public GameObject SFX_Open;
    #endregion

    #region METHODS
    public override void Action()
    {
        if (!Actionned)
        {
            if (oneShot)
            {
                Actionned = true;
            }

            animator.SetBool(ANIMATOR_IS_OPEN, true);
            Instantiate(SFX_Open, transform.position, Quaternion.identity);

            base.Action();
        }
    }
    #endregion
}

