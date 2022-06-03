using UnityEngine;

public class AudioDestroyAuto : MonoBehaviour
{
    #region UNITY METHODS
    private void Start()
    {
        var audio = GetComponent<AudioSource>();

        if ((audio) && (audio.clip))
        {            
            Destroy(gameObject, audio.clip.length * 2);
        }
    }
    #endregion
}
