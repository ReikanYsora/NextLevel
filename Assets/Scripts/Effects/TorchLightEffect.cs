using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TorchLightEffect : MonoBehaviour
{
    #region ATTRIBUTES
    public float refreshTime;
    private float tempRefreshTime;
    [Range(0.5f , 1.5f)]
    public float minRange;
    [Range(0.5f, 1.5f)]
    public float maxRange;
    private UnityEngine.Rendering.Universal.Light2D torchLight;
    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        torchLight = GetComponent<UnityEngine.Rendering.Universal.Light2D>();
    }

    private void Update()
    {
        if (tempRefreshTime <= refreshTime / 100.0f)
        {
            tempRefreshTime += Time.deltaTime;
        }
        else
        {
            tempRefreshTime = 0.0f;
            float rng = Random.Range(minRange, maxRange);
            torchLight.intensity = rng;
            torchLight.shadowIntensity = rng / maxRange;
            torchLight.shadowVolumeIntensity = rng / maxRange;
        }
    }
    #endregion
}
