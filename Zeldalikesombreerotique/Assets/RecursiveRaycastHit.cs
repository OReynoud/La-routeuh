using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class RecursiveRaycastHit : MonoBehaviour
{
    [SerializeField] private bool hasBrokenRaycastOnce;

    [SerializeField] private CableRenderer _cableRenderer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void CastStuff()
    {
        RaycastHit raycastHit;
        var dir = transform.position - PlayerController.instance.transform.position;
        var dirNormed = dir.normalized;
        if (Physics.Raycast(transform.position,dirNormed,out raycastHit, dir.magnitude))
        {
            if (raycastHit.collider.gameObject == PlayerController.instance.gameObject)
            {
                //method to check order of the point to break the LineRenderer position or not
            }
            if (!hasBrokenRaycastOnce)
            {
                return;
            }
            
            
        }
        if (!hasBrokenRaycastOnce)
        {
            _cableRenderer.InstantiateRaycastObject(raycastHit.point);
        }
        hasBrokenRaycastOnce = true;
    }
}
