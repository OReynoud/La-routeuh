using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MusiquePiano : MonoBehaviour
{


    public GameObject musiqueFin;

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            musiqueFin.SetActive(true);
        }
    }
}
