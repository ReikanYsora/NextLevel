using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerManagement : MonoBehaviour
{
    #region CONSTANTS
    private const string ANIMATOR_MOVING = "moving";
    private const string ANIMATOR_MOVE_X = "moveX";
    private const string ANIMATOR_MOVE_Y = "moveY";
    private const string ANIMATOR_BOW_FIRING = "bowFiring";
    private const string ANIMATOR_BOW_TRIGGERED = "bowTriggered";
    #endregion

    #region ATTRIBUTES
    public GameStateManager gameStateManager;
    public Vector2 InputDirection { set; get; }
    public bool Bow { set; get; }
    public bool Portal { set; get; }
    public bool Bomb { set; get; }

    public bool Action { set; get; }
    private InputManager inputManager;
    private DungeonMapManager dungeonMapManager;
    private PlayerStatsManager playerStatsManager;
    private Rigidbody2D playerRigidbody;
    private Animator playerAnimator;
    private Vector2 actualDirection;
    private Vector2 movementChange;
    private float timePressX;
    private float timePressY;

    [Header("Bomb management")]
    [Space]
    public GameObject bombPrefab;
    public Transform bombOriginLeft;
    public Transform bombOriginRight;
    public Transform bombOriginUp;
    public Transform bombOriginDown;
    public GameObject SFX_DropBombPrefab;
    public LayerMask hitColliderLayer;
    private bool bomb;
    private float bombTempCooldown;
    private bool bombCanBeUsed;

    [Header("Bow management")]
    [Space]
    public GameObject arrowPrefab;
    public Transform fireOriginLeft;
    public Transform fireOriginRight;
    public Transform fireOriginUp;
    public Transform fireOriginDown;
    public GameObject bowLoadedEffect;
    public GameObject SFX_BowFullLoaded;
    private bool bowFiring;
    private float bowTimeTempToMaxDamage;
    private bool bowMaxEffectReach;

    [Header("Portals management")]
    [Space]
    public GameObject portalBluePrefab;
    public GameObject portalOrangePrefab;
    private float tempPortalCooldown;
    private bool canCreatePortal;
    private List<PortalSlot> portals;
    #endregion

    #region PROPERTIES
    public bool IsMoving
    {
        get
        {
            return (InputDirection != Vector2.zero) && (!bowFiring);
        }
    }

    public bool PortalInRange { set; get; }

    public bool BowEnabled { get; set; }

    public bool PortalEnabled { get; set; }

    public bool BombEnabled { get; set; }
    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        gameStateManager = FindObjectOfType<GameStateManager>();
        portals = new List<PortalSlot>();
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>(); 
        playerStatsManager = FindObjectOfType<PlayerStatsManager>();    
        dungeonMapManager = FindObjectOfType<DungeonMapManager>();
        inputManager = FindObjectOfType<InputManager>();

        actualDirection = Vector2.down;
        playerAnimator.SetFloat(ANIMATOR_MOVE_X, actualDirection.x);
        playerAnimator.SetFloat(ANIMATOR_MOVE_Y, actualDirection.y);
        playerAnimator.SetBool(ANIMATOR_MOVING, IsMoving);

        bombTempCooldown = 0.0f;
        tempPortalCooldown = 0.0f;
        bombCanBeUsed = true;
        bowFiring = false;
        bomb = false;
        bowMaxEffectReach = false;
        canCreatePortal = true;

        BowEnabled = false;
        BombEnabled = false;
        PortalEnabled = false;
    }

    private void OnEnable()
    {
        inputManager.OnValidatePressed += CBInputManagerOnValidatePressed;
    }

    private void OnDisable()
    {
        inputManager.OnValidatePressed -= CBInputManagerOnValidatePressed;
    }

    private void Update()
    {
        if (gameStateManager.gameState == GameState.Play)
        {
            ManageInputs();
            ManageCharacterDirection();
            ManageBomb();
            ManagePortals();
            UpdateAnimations();
        }
    }

    private void FixedUpdate()
    {
        if (gameStateManager.gameState == GameState.Play)
        {
            playerRigidbody.velocity = Vector2.Lerp(playerRigidbody.velocity, Vector2.zero, 2 * Time.deltaTime);
            MoveCharacter();
        }
    }
    #endregion

    #region METHODS
    #region INPUTS MANAGEMENT
    private void ManageInputs()
    {
        if ((Portal) && (PortalEnabled))
        {
            CreatePortal();
        }

        if (BowEnabled)
        {
            if (bowFiring)
            {
                bowTimeTempToMaxDamage += Time.deltaTime;

                if ((bowTimeTempToMaxDamage >= playerStatsManager.GetPlayerStat(GlobalConstants.PLAYER_STAT_ARROW_CHARGING_TIME).StatValue) && !(bowMaxEffectReach))
                {
                    BowCharging();
                    bowMaxEffectReach = true;
                }
            }

            if (Bow && (!bowFiring))
            {
                bowMaxEffectReach = false;
                bowTimeTempToMaxDamage = 0.0f;
                bowFiring = true;
            }


            if (bowFiring && !Bow)
            {
                bowFiring = false;
                float ratio = bowTimeTempToMaxDamage / playerStatsManager.GetPlayerStat(GlobalConstants.PLAYER_STAT_ARROW_CHARGING_TIME).StatValue;
                BowTriggered(ratio > 1 ? 1 : ratio);
                playerAnimator.SetTrigger(ANIMATOR_BOW_TRIGGERED);
            }
        }

        if (BombEnabled)
        {
            if (Bomb && bombCanBeUsed)
            {
                bomb = true;
                bombCanBeUsed = false;
            }
            else
            {
                bomb = false;
            }

            if (!bombCanBeUsed)
            {
                bombTempCooldown += Time.deltaTime;

                if (bombTempCooldown >= playerStatsManager.GetPlayerStat(GlobalConstants.PLAYER_STAT_BOMB_COOLDOWN).StatValue)
                {
                    bombCanBeUsed = true;
                    bombTempCooldown = 0.0f;
                }
            }
        }
    }
    #endregion

    #region MOVEMENTS & ORIENTATION
    private void ManageCharacterDirection()
    {
        float xValue = InputDirection.x;
        float yValue = InputDirection.y;

        if (xValue != 0)
        {
            if (timePressX == 0.0f)
            {
                timePressX = Time.time;
            }
        }
        else
        {
            timePressX = 0.0f;
        }

        if (yValue != 0)
        {
            if (timePressY == 0.0f)
            {
                timePressY = Time.time;
            }
        }
        else
        {
            timePressY = 0.0f;
        }

        if (timePressX > timePressY)
        {
            movementChange = Vector2.right * xValue;

            if (!bowFiring)
            {
                if (movementChange.x > 0)
                {
                    actualDirection = Vector2.right;
                }
                else if (movementChange.x < 0)
                {
                    actualDirection = Vector2.left;
                }
            }

        }
        else if (timePressX < timePressY)
        {
            movementChange = Vector2.up * yValue;

            if (!bowFiring)
            {
                if (movementChange.y > 0)
                {
                    actualDirection = Vector2.up;
                }
                else if (movementChange.y < 0)
                {
                    actualDirection = Vector2.down;
                }
            }
        }
        else
        {
            movementChange = Vector2.zero;
        }
    }

    private void MoveCharacter()
    {
        if ((InputDirection != Vector2.zero) && (!bowFiring))
        {
            playerRigidbody.MovePosition(new Vector2(transform.position.x, transform.position.y) + InputDirection.normalized * playerStatsManager.GetPlayerStat(GlobalConstants.PLAYER_STAT_PLAYER_SPEED).StatValue * Time.fixedDeltaTime);
        }
    }
    #endregion

    #region WEAPONS MANAGEMENTS
    #region BOW MANAGEMENT
    private void BowCharging()
    {
        Vector2 position;
        Instantiate(SFX_BowFullLoaded, transform.position, transform.rotation);

        if (actualDirection == Vector2.left)
        {
            position = fireOriginLeft.position;
        }
        else if (actualDirection == Vector2.up)
        {
            position = fireOriginUp.position;
        }
        else if (actualDirection == Vector2.down)
        {
            position = fireOriginDown.position;
        }
        else if (actualDirection == Vector2.right)
        {
            position = fireOriginRight.position;
        }
        else
        {
            position = transform.position;
        }

        GameObject effect = Instantiate(bowLoadedEffect, position, Quaternion.identity);
        effect.transform.SetParent(transform);
    }

    private void BowTriggered(float ratio)
    {
        Vector2 position = Vector2.zero;
        Vector3 rotation = new Vector3(0, 0, 0);

        if (actualDirection == Vector2.left)
        {
            position = fireOriginLeft.position;
            rotation = new Vector3(0, 180, 0);
        }
        else if (actualDirection == Vector2.up)
        {
            position = fireOriginUp.position;
            rotation = new Vector3(0, 0, 90);
        }
        else if (actualDirection == Vector2.down)
        {
            position = fireOriginDown.position;
            rotation = new Vector3(0, 0, -90);
        }
        else if (actualDirection == Vector2.right)
        {
            position = fireOriginRight.position;
        }

        GameObject tempArrow = Instantiate(arrowPrefab, position, Quaternion.identity);
        tempArrow.transform.Rotate(rotation);
        tempArrow.GetComponent<ArrowManager>().Initialize(gameObject, actualDirection, playerStatsManager.GetPlayerStat(GlobalConstants.PLAYER_STAT_ARROW_MAX_DAMAGE).StatValue * ratio, playerStatsManager.GetPlayerStat(GlobalConstants.PLAYER_STAT_ARROW_SPEED).StatValue);

        ChronoTimeManager.ChangeTime(TimeDirection.Less, playerStatsManager.GetPlayerStat(GlobalConstants.PLAYER_STAT_ARROW_COST).StatValue);
    }

    public void TakeDamage(GameObject origin, float value)
    {
        Vector2 difference = transform.position - origin.transform.position;

        if (difference == Vector2.zero)
        {
            difference = -actualDirection;
        }

        difference = difference.normalized * 10.0F;
        playerRigidbody.velocity = difference;

        FindObjectOfType<CameraEffect>().Shake(2f, 3f);

        ChronoTimeManager.ChangeTime(TimeDirection.Less, value);
    }

    #endregion

    #region BOMB MANAGEMENT
    private void ManageBomb()
    {
        if (bomb)
        {
            Vector2 position;
            float distance = 0.5f;

            if (!IsMoving)
            {
                if (actualDirection == Vector2.left)
                {
                    position = bombOriginLeft.position;
                }
                else if (actualDirection == Vector2.up)
                {
                    position = bombOriginUp.position;
                }
                else if (actualDirection == Vector2.down)
                {
                    position = bombOriginDown.position;
                    distance = 1.0f;
                }
                else if (actualDirection == Vector2.right)
                {
                    position = bombOriginRight.position;
                }
                else
                {
                    position = transform.position;
                }


                GameObject tempBomb;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, actualDirection, distance, hitColliderLayer);

                if (hit)
                {
                    tempBomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);
                }
                else
                {
                    tempBomb = Instantiate(bombPrefab, position, Quaternion.identity);
                }

                Instantiate(SFX_DropBombPrefab, position, Quaternion.identity);
                tempBomb.GetComponent<BombManager>().Activate(gameObject, playerStatsManager.GetPlayerStat(GlobalConstants.PLAYER_STAT_BOMB_DAMAGE).StatValue, playerStatsManager.GetPlayerStat(GlobalConstants.PLAYER_STAT_BOMB_DAMAGE_RANGE).StatValue);

                ChronoTimeManager.ChangeTime(TimeDirection.Less, playerStatsManager.GetPlayerStat(GlobalConstants.PLAYER_STAT_BOMB_COST).StatValue);
            }
        }
    }
    #endregion
    #endregion

    #region ANIMATIONS MANAGEMENT
    private void UpdateAnimations()
    {
        if ((movementChange != Vector2.zero) && (!bowFiring))
        {
            playerAnimator.SetFloat(ANIMATOR_MOVE_X, movementChange.x);
            playerAnimator.SetFloat(ANIMATOR_MOVE_Y, movementChange.y);
        }

        playerAnimator.SetBool(ANIMATOR_MOVING, IsMoving);
        playerAnimator.SetBool(ANIMATOR_BOW_FIRING, bowFiring);
    }
    #endregion

    #region PORTAL MANAGEMENT
    private void ManagePortals()
    {
        PortalInRange = CheckPortalInRange();

        if (!canCreatePortal)
        {
            tempPortalCooldown += Time.deltaTime;

            if (tempPortalCooldown >= playerStatsManager.GetPlayerStat(GlobalConstants.PLAYER_STAT_PORTAL_COOLDOWN).StatValue)
            {
                tempPortalCooldown = 0.0f;
                canCreatePortal = true;
            }
        }
    }

    private void CreatePortal()
    {
        if (canCreatePortal && !PortalInRange)
        {               
            canCreatePortal = false;

            DestroyPortalsNotInDungeonPart();

            PortalSlot portalSlot;
            PortalSlot tempSlot = portals.FirstOrDefault();

            if (portals.Count == 0)
            {
                portalSlot = Instantiate(PortalColor.Blue);
            }
            else if (portals.Count == 1)
            {
                switch (tempSlot.color)
                {
                    default:
                    case PortalColor.Blue:
                        portalSlot = Instantiate(PortalColor.Orange);
                        break;
                    case PortalColor.Orange:
                        portalSlot = Instantiate(PortalColor.Blue);
                        break;
                }
            }
            else
            {
                tempSlot.portal.GetComponent<PortalManagement>().DestroyPortal();
                portals.Remove(tempSlot);

                switch (tempSlot.color)
                {
                    default:
                    case PortalColor.Blue:
                        portalSlot = Instantiate(PortalColor.Blue);
                        break;
                    case PortalColor.Orange:
                        portalSlot = Instantiate(PortalColor.Orange);
                        break;
                }
            }

            portals.Add(portalSlot);

            ChronoTimeManager.ChangeTime(TimeDirection.Less, playerStatsManager.GetPlayerStat(GlobalConstants.PLAYER_STAT_PORTAL_COST).StatValue);
        }
    }

    private bool CheckPortalInRange()
    {
        if (portals.Count > 0)
        {
            return Vector3.Distance(transform.position, portals[portals.Count - 1].portal.gameObject.transform.position) < 1.5f;
        }

        return false;
    }

    private void DestroyPortalsNotInDungeonPart()
    {
        if (portals.Where(x => x.activeDungeonPart != dungeonMapManager.ActiveDungeonPart).Any())
        {
            portals.ForEach(x => x.portal.GetComponent<PortalManagement>().DestroyPortal());
            portals.Clear();
        }
    }

    private PortalSlot Instantiate(PortalColor color)
    {
        GameObject prefabToInstanciate;

        switch (color)
        {
            default:
            case PortalColor.Blue:
                prefabToInstanciate = portalBluePrefab;
                break;
            case PortalColor.Orange:
                prefabToInstanciate = portalOrangePrefab;
                break;
        }

        GameObject tempPortal = Instantiate(prefabToInstanciate, transform.position, Quaternion.identity);
        tempPortal.GetComponent<PortalManagement>().Initialize(dungeonMapManager.ActiveDungeonPart, color);

        PortalSlot portalSlot = new PortalSlot
        {
            color = color,
            activeDungeonPart = dungeonMapManager.ActiveDungeonPart,
            portal = tempPortal
        };

        return portalSlot;
    }
    #endregion

    #region ACTION ITEM MANAGEMENT
    private void ManageActionnableItems()
    {
        List<Actionnable> actionnables = FindObjectsOfType<Actionnable>().Where(x => x.IsActionnable).ToList();

        if (actionnables.Count > 0)
        {
            actionnables.ForEach(x => x.Action());
        }
    }
    #endregion
    #endregion

    #region CALLBACKS
    private void CBInputManagerOnValidatePressed()
    {
        ManageActionnableItems();
    }
    #endregion
}