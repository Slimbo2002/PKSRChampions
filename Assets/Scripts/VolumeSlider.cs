using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public enum sliderType
    {
        Master,
        Music
    }
    public AudioMixer audioMixer;
    public sliderType volumeSlider;

    public Slider thisSlider;
    void Start()
    {
        switch (volumeSlider) 
        { 
            case(sliderType.Master):
                audioMixer.GetFloat("Volume", out float master);
                thisSlider.value = master;
                break;
            case(sliderType.Music):
                audioMixer.GetFloat("MusicVol", out float music);
                thisSlider.value = music;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
