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
    public AudioSource clock;
    public float timeBeforeReturning;
    public float timeForFade;
    public GameObject self;
    


    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            if (musique != null)
            {
                musique.DOFade(0, 0.8f);

            }
            if (fond != null)
            {
                fond.DOFade(0, 0.8f);

            }
            if (murmure1 != null)
            {
                murmure1.DOFade(0, 0.8f);

            }
            if (murmure2 != null)
            {
                murmure2.DOFade(0, 0.8f);

                
            }
            if (murmure3 != null)
            {
                murmure3.DOFade(0, 0.8f);

            }
            if (murmure4 != null)
            {
                murmure4.DOFade(0, 0.8f);

            }
            if (clock != null)
            {
                StartCoroutine(Clock());

            }
        }
        
    }
    
    IEnumerator Clock()
    {
        clock.DOPitch(0.5f, timeForFade);
        yield return new WaitForSeconds(timeForFade + timeBeforeReturning - 0.4f);
        clock.DOPitch(1, 1.2f);
        self.SetActive(false);
    }
}

