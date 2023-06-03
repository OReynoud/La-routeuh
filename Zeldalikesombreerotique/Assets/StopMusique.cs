using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StopMusique : MonoBehaviour
{

    public AudioSource musique;
    public AudioSource fond;
    
    public AudioSource murmure1;
    public AudioSource murmure2;
    public AudioSource murmure3;
    public AudioSource murmure4;


    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            musique.DOFade(0, 0.8f);
            fond.DOFade(0, 0.8f);
            murmure1.DOFade(0, 0.8f);
            murmure2.DOFade(0, 0.8f);
            murmure3.DOFade(0, 0.8f);
            murmure4.DOFade(0, 0.8f);

        }
    }
}

