using UnityEngine;

public class Actionnable : MonoBehaviour
{
    #region ATTRIBUTES
    [Header("Actionnable item settings")]
    [Space]
    public bool IsActionnable;
    public string tagRequired;
    public bool oneShot;
    public GameObject UIActionButtonIndicatorAnchor;
    private Camera mainCamera;
    #endregion

    #region PROPERTIES
    protected bool Actionned { set; get; }
    #endregion

    #region UNITY METHODS
    private void Update()
    {
        if (!Actionned)
        {
            UIActionButtonIndicatorAnchor.SetActive(IsActionnable);
        }
    }

    private void LateUpdate()
    {
        if (!mainCamera)
        {
            mainCamera = GameObject.FindGameObjectWithTag(GlobalConstants.TAG_MAIN_CAMERA).GetComponent<Camera>();
        }
        else
        {
            UIActionButtonIndicatorAnchor.transform.LookAt(UIActionButtonIndicatorAnchor.transform.position + mainCamera.transform.forward);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(tagRequired))
        {
            IsActionnable = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsActionnable)
        {
            IsActionnable = false;
        }
    }
    #endregion

    #region METHODS
    public virtual void Action()
    {
        UIActionButtonIndicatorAnchor.SetActive(false);
    }
    #endregion

}
