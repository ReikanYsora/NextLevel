using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class ScreenFitCamera : MonoBehaviour
{
    #region ATTRIBUTS
    [SerializeField] private float sceneWidth = 10;
    private Camera gameCamera;
    #endregion

    #region UNITY METHODS
    private void Start()
    {
        gameCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        float unitsPerPixel = sceneWidth / Screen.width;
        float desiredHalfHeight = 0.5f * unitsPerPixel * Screen.height;
        gameCamera.orthographicSize = desiredHalfHeight;
    }
    #endregion
}