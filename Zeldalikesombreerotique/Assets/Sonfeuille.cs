using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonfeuille : MonoBehaviour
{

    public AudioSource feuille;
    public List<AudioClip> audioSources;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            int value = Random.Range(0, audioSources.Count);
            feuille.clip = audioSources[value];
            feuille.Play();
          //  audioSources[value].Play();
            //Un random sur la liste
         //   feuille.Play(0);
        }
    }
}
