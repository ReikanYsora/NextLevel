using UnityEngine;

public class BombManager : MonoBehaviour
{
    #region CONSTANTS
    private const string ANIMATOR_TRIGGER_ACTIVATE = "Activate";
    #endregion

    #region ATTRIBUTES
    public Rigidbody2D rb;
    private Animator animator;
    public GameObject audioExplosionPrefab;
    public GameObject explosionPrefab;
    public GameObject owner;
    private float damageValue;
    private float damageRange;
    public LayerMask damageableMask;
    private CircleCollider2D cCollider;
    [Range(0.0f, 1.0F)] public float minScaleExplosionFXRange;
    [Range(1.0f, 2.0F)] public float maxScaleExplosionFXRange;
    private bool isExploding;
    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        animator = GetComponent<Animator>();
        cCollider = GetComponent<CircleCollider2D>();
        isExploding = false;
    }

    private void FixedUpdate()
    { 
        rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 2 * Time.deltaTime);
    }
    #endregion

    #region METHODS
    public void Explode()
    {
        if (!isExploding)
        {
            isExploding = true;
            Destroy(cCollider);
            GameObject tempExplosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
            float scaleRatio = UnityEngine.Random.Range(minScaleExplosionFXRange, maxScaleExplosionFXRange);
            tempExplosion.transform.localScale = new Vector3(scaleRatio, scaleRatio, tempExplosion.transform.localScale.z);

            Instantiate(audioExplosionPrefab, transform.position, transform.rotation);
            FindObjectOfType<CameraEffect>().Shake(4f, 1.5f);
            
            Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, damageRange, damageableMask);

            foreach (Collider2D tempCollider in hit)
            {
                if (tempCollider)
                {
                    GameObject tempGameObject = tempCollider.gameObject;
                    float distance = Mathf.Abs(Vector3.Distance(transform.position, tempGameObject.transform.position));

                    if (distance > damageRange)
                    {
                        distance = damageRange;
                    }

                    float tempDamage = (1 - (distance / damageRange)) * damageValue;

                    if ((tempGameObject.GetComponent<Interact>() != null) && (tempGameObject != gameObject))
                    {
                        tempGameObject.GetComponent<Interact>().Action(gameObject, tempDamage);
                    }
                }
            }
            
            Destroy(gameObject);
        }
    }

    public void Activate(GameObject _owner, float _damageValue, float _damageRange)
    {
        owner = _owner;
        damageValue = _damageValue;
        damageRange = _damageRange;
        animator.SetTrigger(ANIMATOR_TRIGGER_ACTIVATE);
    }

    public void TakeDamage(GameObject origin, float value)
    {
        Explode();
    }
    #endregion
}