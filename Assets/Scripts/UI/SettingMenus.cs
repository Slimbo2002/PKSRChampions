using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;
using TMPro;
using System;

public class SettingMenus : MonoBehaviour
{
    public AudioMixer audioMixer;
    public TMP_Dropdown qualityDropdown, resolutionDropdown;

    public Toggle vSyncToggle, fullScreenToggle;

    UniversalRenderPipelineAsset customPipeline;

    float defaultMasterVol = -20f;
    float defaultMusicVol = -20f;

    int defaultQualityLevel = 3;
    int defaultResolutionIndex = 3;

    private void Start()
    {
        LoadAudioSettings();
        LoadQualitySettings();
    }
    #region AudioSettings
    public void SetMasterVolume(float vol)
    {
        audioMixer.SetFloat("Volume", vol);

        PlayerPrefs.SetFloat("MasterVol", vol);
    }
    public void SetMusicVolume(float vol)
    {
        audioMixer.SetFloat("MusicVol", vol);
        PlayerPrefs.SetFloat("MusicVol", vol);
    }

    void LoadAudioSettings()
    {
        // Load audio settings
        audioMixer.SetFloat("MasterVol", PlayerPrefs.GetFloat("MasterVol", defaultMasterVol));
        audioMixer.SetFloat("MusicVol", PlayerPrefs.GetFloat("MusicVol", defaultMusicVol));
    }
    #endregion
    void SaveSettings()
    {
        PlayerPrefs.SetInt("QualityLevel", qualityDropdown.value);
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        PlayerPrefs.SetInt("FullScreen", Screen.fullScreen ? 1 : 0); // 1 for true, 0 for false
        PlayerPrefs.SetInt("VSync", vSyncToggle.isOn ? 1 : 0); // 1 for true, 0 for false
        PlayerPrefs.Save(); // Saves the PlayerPrefs to disk
    }

    public void ApplyQualityChanges()
    {
        QualitySettings.SetQualityLevel(qualityDropdown.value);

        if(vSyncToggle.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }

        Screen.fullScreen=fullScreenToggle.isOn;


        switch (resolutionDropdown.value) 
        {
            case (0):
                Screen.SetResolution(1280, 720, fullScreenToggle.isOn);
                break;
            case (1):
                Screen.SetResolution(1366, 768, fullScreenToggle.isOn);
                break;
            case (2):
                Screen.SetResolution(1600, 900, fullScreenToggle.isOn);
                break;
            case (3):
                Screen.SetResolution(1920, 1080, fullScreenToggle.isOn);
                break;
            case (4):
                Screen.SetResolution(2560, 1080, fullScreenToggle.isOn);
                break;
            case (5):
                Screen.SetResolution(2560, 1440, fullScreenToggle.isOn);
                break;
            case (6):
                Screen.SetResolution(3440, 1440, fullScreenToggle.isOn);
                break;
            case (7):
                Screen.SetResolution(3840, 2160, fullScreenToggle.isOn);
                break;

        }

        SaveSettings();

    }
    void LoadQualitySettings()
    {
        // Load quality settings
        qualityDropdown.value = PlayerPrefs.GetInt("QualityLevel", defaultQualityLevel);
        resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionIndex", defaultResolutionIndex);
        Screen.fullScreen = PlayerPrefs.GetInt("FullScreen", 1) == 1; // Default is fullscreen (1)
        vSyncToggle.isOn = PlayerPrefs.GetInt("VSync", 0) == 1; // Default is VSync off (0)

        ApplyQualityChanges();
    }
}
