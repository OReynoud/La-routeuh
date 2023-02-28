using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class Light : MonoBehaviour
{
    public Vector3 endPosition;

    public int rayAmount;
    public float originSpacing;
    public float endSpacing;
    public float distance;
    
    public float oneSideAngle;
    // Start is called before the first frame update
    void Start()
    {
        /*var rot = Quaternion.AngleAxis(rayAmount,Vector3.up);
        var dir = rot * Vector3.forward;
        Debug.Log(dir);*/
        var angleDiff = (oneSideAngle * 2) / rayAmount;
        Debug.Log(angleDiff);
    }

    // Update is called once per frame
    void Update()
    {
        /*endPosition = PlayerController.instance.playerDir;
        for (int i = 0; i < rayAmount * 0.5f; i++)
        {
            Debug.DrawRay(new Vector3(transform.position.x + originSpacing * i, 2,transform.position.z),new Vector3(endPosition.x + endSpacing * i, 2, endPosition.z)* 10, Color.red);
        }
        for (int i = 0; i < rayAmount * 0.5f; i++)
        {
            Debug.DrawRay(new Vector3(transform.position.x - originSpacing * i, 2,transform.position.z),new Vector3(endPosition.x - endSpacing * i, 2, endPosition.z)* 10, Color.blue);
        }*/

        var angleDiff = (oneSideAngle * 2) / rayAmount;
        //Debug.Log(angleDiff);
        for (int i = 0; i < rayAmount; i++)
        {
            var origin = new Vector3(transform.position.x, 2, transform.position.z);
            var currentAngle = oneSideAngle - angleDiff * i;
            var rot = Quaternion.AngleAxis(currentAngle,Vector3.up);
            var dir = rot * Vector3.forward;
            Debug.Log(dir);
            if (Physics.Raycast(origin,dir, out var raycastHit, distance))
            {
                Debug.Log("touché");
                Debug.DrawRay(origin,dir*raycastHit.distance, Color.red);
            }
            else
            {
                Debug.DrawRay(origin,dir*distance, Color.green);
            }
            
        }
        //Debug.DrawRay(transform.position, endPosition * 10, Color.blue);
    }
}
