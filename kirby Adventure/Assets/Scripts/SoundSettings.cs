using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class SoundSettings : MonoBehaviour
{
    [SerializeField] Slider soundSlider;
    [SerializeField] AudioMixer soundSource;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
    private void Start()
    {
        SetVolume(PlayerPrefs.GetFloat("SaveMasterVolume", 100));
    }

    public void SetVolume(float volume)
    {
        if (volume < 1)
        {
            volume = .001f;
        }

        RefreshSlider(volume);
        PlayerPrefs.SetFloat("SaveMasterVolume", volume);
        soundSource.SetFloat("MasterVolume", Mathf.Log10(volume / 100) * 20f);
        
    }
    public void SetVolumeFromSlider()
    {
        SetVolume(soundSlider.value);
    }

    public void RefreshSlider(float volume)
    {
        soundSlider.value = volume;
    }
}
