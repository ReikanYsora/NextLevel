using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class PortalManagement : MonoBehaviour
{
    #region CONSTANTS
    private const string ANIMATOR_DESTROY_TRIGGER = "destroy";
    #endregion

    #region ATTRIBUTES
    [Header("Teleportation settings")]
    [Space]    
    public GameObject teleportationEffectPrefab;
    public GameObject SFX_OpenPortal;
    public GameObject SFX_TeleportElement;
    private Transform otherPortal;
    private GameObject dungeonPartLink;
    private PortalColor portalColor;
    private bool isInitialized;
    private bool isDeployed;
    private Animator animator;
    #endregion

    #region UNITY METHODS
    private void OnTriggerEnter2D(Collider2D other)
	{ 
        if ((isInitialized) && (isDeployed))
        {
            Teleportable tempTeleportable = other.gameObject.GetComponent<Teleportable>();

            if ((tempTeleportable != null) && (tempTeleportable.canBeTeleported))
            {
                FindOtherPortal();

                if (otherPortal == null)
                {
                    return;
                }

                tempTeleportable.canBeTeleported = false;
                GameObject itemToTeleport = other.gameObject;
                Rigidbody2D itemToTeleportRB = itemToTeleport.GetComponent<Rigidbody2D>();
                itemToTeleport.transform.position = otherPortal.transform.position;

                if (tempTeleportable.rotationOnTeleportalation == RotationOnTeleportation.Flip)
                {
                    itemToTeleport.transform.rotation = otherPortal.transform.rotation * transform.rotation * itemToTeleport.transform.rotation;
                }

                if (itemToTeleportRB)
                {
                    itemToTeleportRB.velocity = otherPortal.transform.TransformDirection(transform.InverseTransformDirection(itemToTeleportRB.velocity));
                }

                if (itemToTeleport.CompareTag(GlobalConstants.TAG_PLAYER))
                {
                    FindObjectOfType<CameraEffect>().ChangeChromaticAberration(5.0f);
                }

                Vector2 fxPortal1 = otherPortal.transform.position;
                fxPortal1.y += otherPortal.GetComponent<CircleCollider2D>().radius;
                Vector2 fxPortal2 = transform.position;
                fxPortal2.y += GetComponent<CircleCollider2D>().radius;

                Instantiate(teleportationEffectPrefab, fxPortal1, Quaternion.identity);
                Instantiate(teleportationEffectPrefab, fxPortal2, Quaternion.identity);

                if (SFX_TeleportElement)
                {
                    Instantiate(SFX_TeleportElement, transform.position, Quaternion.identity);
                }
            }
        }
	}
    #endregion

    #region METHODS
    private void FindOtherPortal()
    {    
        List<PortalManagement> portals = GameObject.FindObjectsOfType<PortalManagement>().ToList();

        foreach(var p in portals)
        {
            if (p.gameObject != gameObject)
            {
                otherPortal = p.transform;
                break;
            }
        }
    }
    public void Initialize(GameObject activePart, PortalColor color)
    {
        
        animator = GetComponent<Animator>();
        dungeonPartLink = activePart;
        portalColor = color;
        isInitialized = true;
        isDeployed = false;

        if (SFX_OpenPortal)
        {
            Instantiate(SFX_OpenPortal, transform.position, Quaternion.identity);
        }
    }

    public void DestroyPortal()
    {
        isInitialized = false;
        isDeployed = false;
        Destroy(GetComponent<Collider2D>());
        animator.SetTrigger(ANIMATOR_DESTROY_TRIGGER);
    }

    public void DestroyAfterAnimation()
    {
        Destroy(gameObject);
    }

    public void DeployPortal()
    {
        isDeployed = true;
    }

    public GameObject GetDungeonPartLink()
    {
        return dungeonPartLink;
    }
    #endregion
}
