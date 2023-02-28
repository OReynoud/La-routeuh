using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class Light : MonoBehaviour
{
    public Vector3 endPosition;

    public int rayAmount;

    [Range(-1,1)]public float posRange;

    public float minAngle;

    public float maxAngle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        endPosition = PlayerController.instance.playerDir;
        Debug.DrawRay(transform.position, endPosition * 10, Color.blue);
    }
}
