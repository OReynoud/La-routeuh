using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class UIObject : MonoBehaviour
{
    public Transform objectToFollow;
    public float yOffset;
    public float xOffset;

    public bool syncRotation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        FollowObject();
        if (syncRotation)
        {
            RotateObject();
        }
    }

    void FollowObject()
    {
        //transform.position = new Vector3(objectToFollow.localPosition.x + xOffset, 1,objectToFollow.localPosition.z + yOffset);
        if (objectToFollow.transform.localRotation.eulerAngles.y > 180)
        {
            transform.position = new Vector3(objectToFollow.position.x - xOffset, 0.01f,objectToFollow.position.z - yOffset) - objectToFollow.right;
        }
        else
        {
            transform.position = new Vector3(objectToFollow.position.x + xOffset, 0.01f,objectToFollow.position.z + yOffset) + objectToFollow.right;
        }
        
    }

    void RotateObject()
    {
        transform.rotation = Quaternion.Euler(new Vector3(90,objectToFollow.transform.localRotation.eulerAngles.y,0));
    }
}
