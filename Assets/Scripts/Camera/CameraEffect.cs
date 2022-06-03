using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraEffect : MonoBehaviour
{
    #region CONSTANTS
    private const string MAIN_CAMERA_TAG = "MainCamera";
    #endregion

    #region ATTRIBUTES
    private Camera mainCamera;
    private float chromaticAberrationInitialValue;
    private float shakeAmount;
    private float caRatio;
    private bool isShaking;
    private bool isCAChanging;
    public VolumeProfile volumeProfile;
    private ChromaticAberration chromaticAberration;
    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        mainCamera = GameObject.FindGameObjectWithTag(MAIN_CAMERA_TAG).GetComponent<Camera>();
        isShaking = false;
        isCAChanging = false;
    }   
    
    private void Start()
    { 
        if (volumeProfile.TryGet(out chromaticAberration))
        {
            chromaticAberrationInitialValue = chromaticAberration.intensity.value;
        }
    }
    #endregion

    #region METHODS
    public void Shake(float amount, float length)
    {
        if (!isShaking)
        {
            isShaking = true;
            shakeAmount = amount / 100.0f;
            InvokeRepeating("BeginShake", 0, 0.01f);
            Invoke("StopShake", length / 10.0f);
        }
    }
    public void ChangeChromaticAberration(float ratio)
    {
        caRatio = ratio;

        if (!isCAChanging)
        {
            StartCoroutine("ChangeChromaticAberation");
        }
    }

    private void BeginShake()
    {
        if (shakeAmount > 0)
        {
            Vector3 camPosition = mainCamera.transform.position;
            float shakeAmountX = Random.value * shakeAmount * 2 - shakeAmount;
            float shakeAmountY = Random.value * shakeAmount * 2 - shakeAmount;
            camPosition.x += shakeAmountX;
            camPosition.y += shakeAmountY;
            mainCamera.transform.position = camPosition;    
        }
    }

    private void StopShake()
    {
        CancelInvoke("BeginShake");
        mainCamera.transform.localPosition = Vector3.zero;
        isShaking = false;
    }
    
    IEnumerator ChangeChromaticAberation()
    {
        isCAChanging = true;

        bool next = true;
        bool halfPassed = false;
        float value = 0.0f;

        while (next)
        {
            if (!halfPassed)
            {
                value += caRatio * Time.fixedDeltaTime;
            }
            else
            {
                value -= caRatio * Time.fixedDeltaTime;
            }

            if (value >= 0.6f)
            {
                halfPassed = true;
            } 

            if ((value <= 0.0f) && (halfPassed))
            {
                next = false;
            }

            chromaticAberration.intensity.Override(value);
            yield return null;
        }
         
        isCAChanging = false;
        chromaticAberration.intensity.Override(chromaticAberrationInitialValue);
    }
    #endregion
}
