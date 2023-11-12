using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonfeuille : MonoBehaviour
{

    public AudioSource feuille;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            feuille.Play(0);
        }
    }
}
