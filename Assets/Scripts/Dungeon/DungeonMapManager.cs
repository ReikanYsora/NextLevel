using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DungeonMapManager : MonoBehaviour
{
    #region CONSTANTS
    private const string CAMERA_ANCHOR_TAG = "CameraAnchor";
    #endregion

    #region ATTRIBUTES
    [Header("Camera movement and effects attributes")]
    [Space]
    public float cameraTransitionSpeed;
    private Transform cameraAnchor;

    [Header("Dungeon procedural management")]
    [Space]
    [SerializeField] public List<GameObject> dungeonPrefabs;
    [SerializeField] private List<DungeonPartID> dungeonParts;
    private GameObject activeDungeonPart;
    public GameStateManager gameStateManager;
    private List<DoorManagement> loadingZones;
    public int dungeonPartTotal;
    public int dungeonPartInOneLine;
    public int boundX = 24;
    public int boundY = 15;
    #endregion

    #region EVENTS
    public delegate void DungeonGenerationCompleted(DungeonPart startPart);
    public event DungeonGenerationCompleted OnDungeonGenerationCompleted;
    #endregion

    #region PROPERTIES
    public GameObject ActiveDungeonPart
    {
        set
        {
            if (activeDungeonPart != null)
            {
                activeDungeonPart.GetComponent<DungeonPart>().isFocused = false;
            }
            
            activeDungeonPart = value;
            activeDungeonPart.GetComponent<DungeonPart>().isFocused = true;
            StartCoroutine(MoveCameraToNextRoom());
        }
        get
        {
            return activeDungeonPart;
        }
    }
    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        cameraAnchor = GameObject.FindGameObjectWithTag(CAMERA_ANCHOR_TAG).GetComponent<Transform>();
    }
    #endregion

    #region METHODS
    private DungeonPart AddDungeonPart(GameObject go, int x, int y)
    {
        Guid tempGUID = Guid.NewGuid();
        GameObject instance = Instantiate(go, new Vector3(x * boundX, y * boundY, 0), Quaternion.identity);
        instance.GetComponent<DungeonPart>().ID = tempGUID;
        dungeonParts.Add(new DungeonPartID
        {
            ID = tempGUID,
            X = x,
            Y = y,
            Part = instance
        });

        return instance.GetComponent<DungeonPart>();
    }

    public void GenerateDungeon()
    {
        DungeonPart dungeonStartPart;
        int maxLines = Mathf.CeilToInt(dungeonPartTotal / dungeonPartInOneLine);
        dungeonParts = new List<DungeonPartID>();

        if (dungeonPartTotal < 2)
        {
            Debug.Log("The generation need a minimum of two parts. Stop generation.");
            return;
        }

        if (dungeonPrefabs != null)
        {
            bool canContinue = false;
            int indexPart = 0;
            int levelPart = 0;

            //Find start parts
            List<GameObject> startParts = dungeonPrefabs.Where(x => x.GetComponent<DungeonPart>() != null && x.GetComponent<DungeonPart>().isStartPart).ToList();

            if (startParts.Count == 0)
            {
                Debug.Log("Can't find start part. Stop generation.");
                return;
            }

            int indexStart = UnityEngine.Random.Range(0, startParts.Count - 1);
            GameObject lastPart = startParts[indexStart];
            dungeonStartPart = AddDungeonPart(lastPart, indexPart, levelPart);

            indexPart++;
            canContinue = true;

            //Loop for create each parts
            while (canContinue)
            {
                if (indexPart > dungeonPartInOneLine - 1)
                {
                    indexPart = 0;
                    levelPart++;

                    if (levelPart > maxLines)
                    {
                        canContinue = false;
                        break;
                    }
                }

                GameObject[] results = dungeonPrefabs.Where(x => !x.GetComponent<DungeonPart>().isStartPart).ToArray();

                //If is first case, we dont' want prefab with left door
                if (indexPart == 0)
                {
                    results = results.Where(x => x.GetComponent<DungeonPart>().leftEntrance == null).ToArray();
                }
                else
                {
                    //We need to check if left case have a right door
                    var lastLeftCase = dungeonParts.Where(x => x.X == indexPart - 1 && x.Y == levelPart).Select(x => x.Part).FirstOrDefault();

                    if (lastLeftCase == null)
                    {
                        Debug.Log("Incorrect result. Stop generation."); ;
                        return;
                    }

                    if (lastLeftCase.GetComponent<DungeonPart>().rightEntrance != null)
                    {
                        results = results.Where(x => x.GetComponent<DungeonPart>().leftEntrance != null).ToArray();
                    }
                    else
                    {
                        results = results.Where(x => x.GetComponent<DungeonPart>().leftEntrance == null).ToArray();
                    }
                }

                //If is line 1, we don't want prefab with bottom door
                if (levelPart == 0)
                {
                    results = results.Where(x => x.GetComponent<DungeonPart>().bottomEntrance == null).ToArray();
                }
                else
                {
                    //We need to check if bottom case have a top door
                    var lastBottomCase = dungeonParts.Where(x => x.X == indexPart && x.Y == levelPart - 1).Select(x => x.Part).FirstOrDefault();

                    if (lastBottomCase == null)
                    {
                        Debug.Log("Incorrect generation. Stop generation."); ;
                        return;
                    }

                    if (lastBottomCase.GetComponent<DungeonPart>().topEntrance != null)
                    {
                        results = results.Where(x => x.GetComponent<DungeonPart>().bottomEntrance != null).ToArray();
                    }
                    else
                    {
                        results = results.Where(x => x.GetComponent<DungeonPart>().bottomEntrance == null).ToArray();
                    }
                }

                //If is last part of a line, we don't want prefab with right door
                if (indexPart == dungeonPartInOneLine - 1)
                {
                    results = results.Where(x => x.GetComponent<DungeonPart>().rightEntrance == null).ToArray();
                }

                var tempResult = results.Where(x => x.GetComponent<DungeonPart>().rightEntrance != null).ToArray();

                GameObject tempPart;

                if (tempResult.Length > 0)
                {
                    tempPart = tempResult[UnityEngine.Random.Range(0, tempResult.Length - 1)];
                }
                else
                {
                    if (results.Length > 0)
                    {
                        tempPart = results[UnityEngine.Random.Range(0, results.Length - 1)];
                    }
                    else
                    {
                        tempPart = null;
                    }
                }

                if (tempPart != null)
                {
                    lastPart = tempPart;
                    AddDungeonPart(lastPart, indexPart, levelPart);
                    indexPart++;
                }
                else
                {
                    canContinue = false;

                    loadingZones = FindObjectsOfType<DoorManagement>().ToList();
                    loadingZones.ForEach(x => x.OnDetectPlayerEvent += CBLoadingZonesOnDetectPlayerEvent);

                    OnDungeonGenerationCompleted?.Invoke(dungeonStartPart);
                }
            }
        }
    }

    IEnumerator MoveCameraToNextRoom()
    {
        Time.timeScale = 0;

        var t = 0.0f;
        while (t < cameraTransitionSpeed)
        {
            t += Time.unscaledDeltaTime;
            cameraAnchor.position = Vector3.Lerp(cameraAnchor.position, new Vector3(activeDungeonPart.transform.position.x, activeDungeonPart.transform.position.y, cameraAnchor.position.z), t);
            yield return null;
        }

        Time.timeScale = 1;
    }
    #endregion

    #region CALLBACKS
    private void CBLoadingZonesOnDetectPlayerEvent(DungeonPart dp, ExitPoint exit, StartPoint start)
    {
        ActiveDungeonPart = dp.gameObject;
        var part = dungeonParts.Where(x => x.ID == dp.ID).FirstOrDefault();
        GameObject nextPart = null;

        if (part != null)
        {
            switch (exit)
            {
                case ExitPoint.Top:
                    nextPart = dungeonParts.Where(x => x.X == part.X && x.Y == (part.Y + 1)).Select(x => x.Part).FirstOrDefault();
                    break;
                case ExitPoint.Bottom:
                    nextPart = dungeonParts.Where(x => x.X == part.X && x.Y == (part.Y - 1)).Select(x => x.Part).FirstOrDefault();
                    break;
                case ExitPoint.Left:
                    nextPart = dungeonParts.Where(x => x.X == (part.X - 1) && x.Y == part.Y).Select(x => x.Part).FirstOrDefault();
                    break;
                case ExitPoint.Right:
                    nextPart = dungeonParts.Where(x => x.X == (part.X + 1) && x.Y == part.Y).Select(x => x.Part).FirstOrDefault();
                    break;
                default:
                    break;
            }
        }

        switch (start)
        {
            case StartPoint.Top:
                gameStateManager.ChangePlayerPosition(nextPart.GetComponent<DungeonPart>().topEntrance.position);
                break;
            case StartPoint.Bottom:
                gameStateManager.ChangePlayerPosition(nextPart.GetComponent<DungeonPart>().bottomEntrance.position);
                break;
            case StartPoint.Left:
                gameStateManager.ChangePlayerPosition(nextPart.GetComponent<DungeonPart>().leftEntrance.position);
                break;
            case StartPoint.Right:
                gameStateManager.ChangePlayerPosition(nextPart.GetComponent<DungeonPart>().rightEntrance.position);
                break;
        }

        ActiveDungeonPart = nextPart;
    }
    #endregion
}

[Serializable]
public class DungeonPartID
{
    public int X;
    public int Y;
    public Guid ID;
    public GameObject Part;
}
