using UnityEngine;
public class Teleportable : MonoBehaviour
{
    #region ATTRIBUTES
    [Header("Teleportation settings")]
    [Space]
    public RotationOnTeleportation rotationOnTeleportalation;
    public bool canBeTeleported;
    public float teleportCooldown;
    private float tempTeleportCooldown;
    #endregion

    #region UNITY METHODS
    private void Start() 
    {
        tempTeleportCooldown = 0.0f;
        canBeTeleported = true;
    }

    private void Update() 
    {
        if (!canBeTeleported)
        {
            tempTeleportCooldown += Time.deltaTime;

            if (tempTeleportCooldown >= teleportCooldown)
            {
                canBeTeleported = true;
                tempTeleportCooldown = 0.0f;
            }
        }
    }
    #endregion
}

public enum RotationOnTeleportation
{
    None,
    Flip
}
