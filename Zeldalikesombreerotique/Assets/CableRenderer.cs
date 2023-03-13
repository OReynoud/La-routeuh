using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class CableRenderer : MonoBehaviour
{
    public int maxVertices;
    public GameObject recursionPrefab;
    public float raycastLength;
    public List<Collider> recordedColliders = new List<Collider>();
    public List<RecursiveRaycastHit> recordedPoints = new List<RecursiveRaycastHit>();


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*var currentAngle = transform1.rotation.eulerAngles.y + _halfAngle - _angleInterval * i; // Current angle
        var rot = Quaternion.AngleAxis(currentAngle,Vector3.up); // Quaternion for direction calculation
        var dir = rot * Vector3.forward; // Direction of the raycast*/
        GetRayCasts(-90);
        GetRayCasts(90);
        UseRecordedPoints();
    }

    void GetRayCasts(float side)
    {
                
        var angle = PlayerController.instance.transform.eulerAngles.y + side;
        var rot = Quaternion.AngleAxis(angle, Vector3.up);
        var dir = rot * Vector3.forward;
        RaycastHit raycastHit;
        if (Physics.Raycast(PlayerController.instance.transform.position,dir,out raycastHit,raycastLength))
        {
            if (raycastHit.collider == recordedColliders.Find(collider1 => raycastHit.collider))
            {
                return;
            }
            recordedColliders.Add(raycastHit.collider);
            InstantiateRaycastObject(raycastHit.point);
            Debug.DrawRay(PlayerController.instance.transform.position,dir * raycastHit.distance);
        }
        else
        {
            Debug.DrawRay(PlayerController.instance.transform.position,dir * raycastLength);
        }
    }

    void UseRecordedPoints()
    {
        foreach (var point in recordedPoints)
        {
            point.CastStuff();
        }
    }

    public void InstantiateRaycastObject(Vector3 pos)
    {
        
        var currentPoint = Instantiate(recursionPrefab, pos, Quaternion.identity, transform).GetComponent<RecursiveRaycastHit>();
        recordedPoints.Add(currentPoint);
    }
}
