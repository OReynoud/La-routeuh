using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class EndingTrigger : MonoBehaviour
{
    public Transform girl;
    public Transform girlHand;
    public Transform girlHead;
    public Transform chapo;
    public GameObject fireHead;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController.instance.laPetite = girl;
            PlayerController.instance.girlHand = girlHand;
            PlayerController.instance.girlHead = girlHead;
            PlayerController.instance.chapo = chapo;
            PlayerController.instance.fireHead = fireHead;
            StartCoroutine(PlayerController.instance.LaDerniereRoute());
        }

        GetComponent<SphereCollider>().enabled = false;
    }
}
