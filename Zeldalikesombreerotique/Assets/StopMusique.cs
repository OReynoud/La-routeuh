using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StopMusique : MonoBehaviour
{

    public AudioSource musique;
    public AudioSource fond;

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            musique.DOFade(0, 0.8f);
            fond.DOFade(0, 0.8f);

        }
    }
}

