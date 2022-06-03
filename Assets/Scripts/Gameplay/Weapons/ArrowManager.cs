using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    #region CONSTANTS
    private const string ANIMATOR_TRIGGER_START = "Start";
    private const string ANIMATOR_TRIGGER_STOP = "Stop";
    #endregion

    #region ATTRIBUTES
    private float damage;
    private float speed;
    public GameObject owner;
    private Collider2D arrowCollider;
    public Rigidbody2D arrowRigidbody;
    public Renderer arrowRenderer;
    public Animator animator;
    public GameObject SFX_ArrowImpact;
    public GameObject SFX_ArrowThrow;
    #endregion

    #region UNITY METHODS
    private void Awake() 
    {
        arrowCollider = GetComponent<Collider2D>();
        arrowCollider.enabled = false;
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if ((other.gameObject.GetComponent<Interact>() != null) && (other.gameObject != gameObject))
        {
            other.gameObject.GetComponent<Interact>().Action(gameObject, damage);
        }

        Instantiate(SFX_ArrowImpact, transform.position, Quaternion.identity);
        animator.SetTrigger(ANIMATOR_TRIGGER_STOP);
        arrowRigidbody.velocity = Vector2.zero;
        Destroy(GetComponent<Collider2D>());
        Destroy(gameObject, 0.2F);
    }
    #endregion

    #region METHODS
    public void Initialize(GameObject _owner, Vector2 direction, float _damage, float _speed)
    {
        damage = _damage;
        owner = _owner;
        speed = _speed;

        arrowRigidbody.velocity = direction * speed;
        animator.SetTrigger(ANIMATOR_TRIGGER_START);
        Instantiate(SFX_ArrowThrow , transform.position, Quaternion.identity);    
        Invoke("EnableCollider", 0.05f);
    }

    private void EnableCollider()
    {
        arrowCollider.enabled = true;
    }
    #endregion
}
