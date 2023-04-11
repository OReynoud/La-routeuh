using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Player;
using UnityEngine;

public class CarChild : MonoBehaviour
{

    private GameObject MyPlayer;
    private Transform MyParent;

    // Start is called before the first frame update
    void Start()
    {
        MyPlayer = GameObject.Find("Player");
        MyParent = transform.parent;
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            if(MyParent.GetComponent<RollbackCar>().flyToHub)
          {
            col.GetComponent<PlayerController>().willTriggerCinematic = true;
          }
        }
    }
     private void OnTriggerExit(Collider col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            col.GetComponent<PlayerController>().willTriggerCinematic = false;
        }
    }
}
