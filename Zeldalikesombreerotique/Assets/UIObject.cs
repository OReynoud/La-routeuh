using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
[RequireComponent(typeof(SpriteRenderer))]
public class UIObject : MonoBehaviour
{
    public Transform objectToFollow;
    public float yOffset;
    public float xOffset;
    public SpriteRenderer sr;
    public bool syncRotation;

    public bool switchWithOrientation;
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
        if (!switchWithOrientation)
        { 
            transform.position = new Vector3(objectToFollow.localPosition.x + xOffset, 0.01f,objectToFollow.localPosition.z + yOffset);
            return;
        }

        if (objectToFollow.transform.localRotation.eulerAngles.y > 180)
        {
            transform.position =
                new Vector3(objectToFollow.position.x - xOffset, 0.01f, objectToFollow.position.z - yOffset) -
                objectToFollow.right; 
            sr.flipX = true;
        }
        else
        {
            transform.position =
                new Vector3(objectToFollow.position.x + xOffset, 0.01f, objectToFollow.position.z + yOffset) +
                objectToFollow.right; 
            sr.flipX = false;
        }
    }

    void RotateObject()
    {
        transform.rotation = Quaternion.Euler(new Vector3(90,objectToFollow.transform.localRotation.eulerAngles.y,0));
    }
}
