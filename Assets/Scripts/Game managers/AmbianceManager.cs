using UnityEngine;
using UnityEngine.Audio;


public class AmbianceManager : MonoBehaviour
{
    #region ATTRIBUTES
    private GameStateManager gameStateManager;

    [Header("Audio Mixer & source settings")]
    [Space]
    public AudioMixer AudioMixer;
    public AudioSource musicSource;

    [Header("Light settings")]
    [Space]
    public UnityEngine.Rendering.Universal.Light2D globaLight;

    [Header("Jail")]
    [ColorUsageAttribute(true, true)]
    public Color jailLightColor;
    public float jailLightIntensity;
    public AudioClip jailTheme;

    [Header("Dungeon")]
    [ColorUsageAttribute(true, true)]
    public Color dungeonLightColor;
    public float dungeonLightIntensity;
    public AudioClip dungeonTheme;
    #endregion

    #region METHODS
    public void SetMasterVolume(float sfxVolume)
    {
        AudioMixer.SetFloat(GlobalConstants.AUDIO_MASTER_VOLUME, sfxVolume);
    }
    public void SetSFXVolume(float sfxVolume)
    {
        AudioMixer.SetFloat(GlobalConstants.AUDIO_SFX_VOLUME, sfxVolume);
    }

    public void SetMusicVolume(float musicLevel)
    {
        AudioMixer.SetFloat(GlobalConstants.AUDIO_MUSIC_VOLUME, musicLevel);
    }

    private void Play(AudioClip audioClip)
    {
        musicSource.clip = audioClip;
        musicSource.Play();
    }

    private void Stop()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }

    public void ChangeAmbiance(GameMode gameMode)
    {
        switch (gameMode)
        {
            case GameMode.None:
                Stop();
                break;
            case GameMode.Jail:
                globaLight.color = jailLightColor;
                globaLight.intensity = jailLightIntensity;
                Play(jailTheme);
                break;
            case GameMode.Game:
                globaLight.color = dungeonLightColor;
                globaLight.intensity = dungeonLightIntensity;
                Play(dungeonTheme);
                break;
        }
    }
    #endregion
}
