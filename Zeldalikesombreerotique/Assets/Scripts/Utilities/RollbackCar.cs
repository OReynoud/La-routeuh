using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Player;
using UnityEngine;

public class RollbackCar : MonoBehaviour
{
    public Rigidbody rb;

    public float storedForce;

    public float maxStoredForce;
    public float forceIncrements;
    public float MaxTimeOfAppliedForce;
    public float timeApplied;
    public bool flyToHub;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (flyToHub)
        {
            var nearbyObjects= Physics.OverlapSphere(transform.position, 2);
            foreach (var item in nearbyObjects)
            {
                if (item == PlayerController.instance.playerColl)
                {
                    PlayerController.instance.willTriggerCinematic = true;
                }
            }
        }
        
        if (PlayerController.instance.isGrabbing && PlayerController.instance.objectToGrab == rb && rb.velocity.magnitude > .1f)
        {
            var differential = rb.velocity.normalized - transform.forward;
            var difValue  = Mathf.Abs(differential.x) + Mathf.Abs(differential.z);
            if (difValue > 1)
            {
                storedForce += (difValue - 1) * forceIncrements;
            }
            else
            {
                storedForce -= (difValue +0.5f)* forceIncrements;
            }

            if (storedForce < 0)
            {
                storedForce = 0;
            }

            if (storedForce > maxStoredForce)
            {
                storedForce = maxStoredForce;
            }

            if (!PlayerController.instance.controls.Player.Interact.IsPressed())
            {
                timeApplied = MaxTimeOfAppliedForce * (storedForce / maxStoredForce);
            }
            return;
        }

        if (timeApplied > 0)
        {
            timeApplied -= Time.deltaTime;
            rb.AddForce(transform.forward * storedForce);
            storedForce -= forceIncrements;
        }
        else if (timeApplied<0 && !PlayerController.instance.isGrabbing)
        {
            storedForce = 0;
        }
    }

    private void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Wall"))
        {
            Destroy(collisionInfo.gameObject);
            Destroy(gameObject);
            
        }
    }
}
