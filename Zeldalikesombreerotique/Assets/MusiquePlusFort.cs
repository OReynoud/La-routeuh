using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MusiquePlusFort : MonoBehaviour
{

    public AudioSource musiquePiano;



    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            musiquePiano.DOFade(5, 1f);


        }
    }
}

