using UnityEngine;

public class DestroyEvent : MonoBehaviour
{
    #region METHODS
    public void Destroy()
    {
        Destroy(gameObject);
    }
    #endregion
}
