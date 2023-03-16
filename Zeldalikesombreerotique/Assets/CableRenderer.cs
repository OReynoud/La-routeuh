using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class CableRenderer : MonoBehaviour
{
    public GameObject recursionPrefab;
    public float raycastLength;
    public List<Collider> recordedColliders = new List<Collider>();
    public List<RecursiveRaycastHit> recordedPoints = new List<RecursiveRaycastHit>();


    public Collider connectedObject1;
    public Collider connectedObject2;
    public Vector3 nearestMainTarget;


    // Start is called before the first frame update
    void Start()
    {
        nearestMainTarget = connectedObject2.transform.position;
        //MainRaycast();
    }

    // Update is called once per frame


    private void FixedUpdate()
    {
        var dir = nearestMainTarget - connectedObject1.transform.position;
        var dirNormed = dir.normalized;
        if (Physics.Raycast(connectedObject1.transform.position+ Vector3.up * 0.1f,dirNormed,out var raycastHit,
                Vector3.Distance(connectedObject1.transform.position+ Vector3.up * 0.1f,nearestMainTarget)))
        {
            if (raycastHit.collider == connectedObject2)
            {
                Debug.DrawRay(connectedObject1.transform.position, dirNormed * raycastHit.distance, Color.magenta);
                    //MainRaycast();
                    return;
            }

            if (raycastHit.point != nearestMainTarget)
            {
                SecondaryRaycast(nearestMainTarget);   
            }
            nearestMainTarget = raycastHit.point;
            Debug.DrawRay(connectedObject1.transform.position+ Vector3.up * 0.1f, dirNormed * raycastHit.distance, Color.yellow);
            //MainRaycast();
            return;
        }
        Debug.DrawRay(connectedObject1.transform.position, dirNormed * raycastHit.distance, Color.white);


    }
    void SecondaryRaycast(Vector3 origin)
    {
        //Debug.Log("called" );
        var dir = origin - connectedObject1.transform.position;
        var dirNormed = dir.normalized;
        Debug.DrawRay(origin,Vector3.up * 2,Color.black);
        if (Physics.Raycast(origin,dirNormed, out var raycastHit, Vector3.Distance(connectedObject1.transform.position+ Vector3.up * 0.1f,origin)))
        {
            if (raycastHit.collider == connectedObject1)
            {
                nearestMainTarget = origin;
                return;
            }

            StartCoroutine(MaintainRaycast(origin));
            //SecondaryRaycast(origin);
        }
    }

    IEnumerator MaintainRaycast(Vector3 origin)
    {
        yield return new WaitForFixedUpdate();
        SecondaryRaycast(origin);
    }

    #region MaSolution

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

    #endregion
    
}
