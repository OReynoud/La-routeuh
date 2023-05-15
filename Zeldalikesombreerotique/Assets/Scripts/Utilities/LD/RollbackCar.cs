using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using NaughtyAttributes;
using Player;
using UnityEngine;

public class RollbackCar : MonoBehaviour
{
    internal Rigidbody rb;

    [ShowNonSerializedField]private float storedForce;

    [Tooltip("Force maximale que peut avoir la voiture")]public float maxStoredForce;
    [Tooltip("Vitesse à laquelle la voiture se charge ou décharge de force")]public float forceIncrements;
    [Tooltip("Temps durant lequel la force sera appliqué à la voiture, plus c'est long, plus la voiture ira loin sans nécessairement aller plus vite")]public float MaxTimeOfAppliedForce;
    [ShowNonSerializedField]private float timeApplied;
    [Tooltip("Détermine si la voiture va se tp au hub")]public bool flyToHub;
    
    
    
    // Start is called before the first frame update : en gros c'est bien
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

         /*if (!PlayerController.instance.controls.Player.Interact.IsPressed())
         {
             timeApplied = MaxTimeOfAppliedForce * (storedForce / maxStoredForce);
         }*/
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
