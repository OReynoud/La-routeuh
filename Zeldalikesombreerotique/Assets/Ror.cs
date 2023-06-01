using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ror : MonoBehaviour
{
    public Transform refern;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.RotateAround(refern.position,transform.up, 1);
    }
}
