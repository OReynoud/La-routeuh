using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Player;
using UnityEngine;

public class CarChild : MonoBehaviour
{

    private GameObject _myPlayer;
    private Transform _myParent;

    // Start is called before the first frame update
    void Start()
    {
        _myPlayer = GameObject.Find("Player");
        _myParent = transform.parent;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        if(_myParent.GetComponent<RollbackCar>().flyToHub)
        {
        }
    }
     private void OnTriggerExit(Collider col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
        }
    }
}
