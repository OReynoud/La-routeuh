using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundSettingsUpdater : MonoBehaviour
{
    public static SoundSettingsUpdater instance;
    public AudioMixerGroup musicGroup;

    public AudioMixerGroup sfxGroup;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(gameObject);
        }
        instance = this;
        
    }

    void Start()
    {
        UpdateSfx();
        UpdateMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSfx()
    {
        var newDBValue = -80 + PlayerPrefs.GetFloat("Sound",0.8f) * 100 ;
        sfxGroup.audioMixer.SetFloat("SFX", newDBValue);
    }

    public void UpdateMusic()
    {
        var newDBValue = - 80 + PlayerPrefs.GetFloat("Music",0.8f) * 100 ;
        sfxGroup.audioMixer.SetFloat("Music", newDBValue);
    }
}
