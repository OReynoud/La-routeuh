using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundSettingsUpdater : MonoBehaviour
{
    public AudioMixerGroup[] musicGroup;

    public AudioMixerGroup[] sfxGroup;

    public float[] musicGroupBase;
    public float[] sfxGroupBase;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < musicGroup.Length; i++)
        {
            musicGroup[i].audioMixer.GetFloat("Volume", out var baseVolume);
            Debug.Log(baseVolume);
            musicGroupBase[i] = baseVolume;
            musicGroup[i].audioMixer.SetFloat("Volume", -10);
        }
        for (int i = 0; i < sfxGroup.Length; i++)
        {
            sfxGroup[i].audioMixer.GetFloat("Volume", out var baseVolume);
            Debug.Log(baseVolume);
            sfxGroupBase[i] = baseVolume;
            sfxGroup[i].audioMixer.SetFloat("Volume", -10);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
