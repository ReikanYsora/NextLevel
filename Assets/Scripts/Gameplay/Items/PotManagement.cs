using System;
using UnityEngine;

public class PotManagement : MonoBehaviour
{
    #region CONSTANTS
    public const string ANIMAT0R_STATE_VALUE = "state";
    #endregion

    #region ATTRIBUTES
    public Rigidbody2D rb;
    public Animator animator;
    public float resistance;
    private float resistanceStart;
    public GameObject SFXPotDestructionPrefab;
    public GameObject SubEffectDestroyPrefab;
    [Range(0.0f, 1.0F)] public float minScaleEffectRange;
    [Range(1.0f, 2.0F)] public float maxScaleEffectRange;
    #endregion

    #region UNITY METHODS
    private void Start()
    {
        resistanceStart = resistance;
        animator.SetInteger(ANIMAT0R_STATE_VALUE, 0);
    }

    private void FixedUpdate()
    { 
        rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 2 * Time.deltaTime);
    }
    #endregion
    
    #region METHODS
    public void TakeDamage(GameObject origin, float value)
    {
        if (resistance > 0)
        {
            if (value < resistance)
            {
                resistance -= value;
                Vector2 difference = transform.position - origin.transform.position;
                rb.velocity = difference.normalized;

                var quart = resistanceStart / 4.0f;

                if (resistance >= quart * 3)
                {
                    animator.SetInteger(ANIMAT0R_STATE_VALUE, 0);
                }
                else if (resistance >= quart * 2)
                {
                    animator.SetInteger(ANIMAT0R_STATE_VALUE, 1);
                }
                else
                {
                    animator.SetInteger(ANIMAT0R_STATE_VALUE, 2);
                }
            }
            else 
            {
                resistance = 0;
                animator.SetInteger(ANIMAT0R_STATE_VALUE, 3);            
                GameObject smokeEffect = Instantiate(SubEffectDestroyPrefab, transform.position, Quaternion.identity);

                float scaleX = UnityEngine.Random.Range(minScaleEffectRange, maxScaleEffectRange);
                float scaleY = UnityEngine.Random.Range(minScaleEffectRange, maxScaleEffectRange);

                if (Convert.ToBoolean(UnityEngine.Random.Range( 0, 2 )))
                {
                    scaleX = scaleX * -1;
                }

                Vector3 scale = smokeEffect.transform.localScale;
                scale.x = scaleX;
                scale.y = scaleY;
                smokeEffect.transform.localScale = scale;

                Instantiate(SFXPotDestructionPrefab, transform.position, Quaternion.identity);
            }
        }
    }
    #endregion
}
