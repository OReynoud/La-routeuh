using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashScript : MonoBehaviour
{

public Transform DiroLight;
public Transform player;
public Transform EndObject;
public float dir;

public bool isTraverse;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isTraverse)
        {
            dir = Vector3.Distance(EndObject.position, player.position);
            DiroLight.GetComponent<Light>().intensity = ( 1 * dir )/3.5f;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            isTraverse = true;
        }
    }

}
