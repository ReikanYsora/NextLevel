using UnityEngine;

public class Interact : MonoBehaviour
{
    #region ATTRIBUTES
    [Header("Unity event")]
    [Space]
    public InteractEvent interactEvent;
    #endregion

    #region METHODS
    public void Action(GameObject origin, float value)
    {
        interactEvent?.Invoke(origin, value);
    }
    #endregion
}
