using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class PetiteFilleChase : MonoBehaviour
{
    public Parkour chaseScript;

    public bool willSlowDownPlayer;
    private float savedMaxSpeed;
    private float savedMinSpeed;
    public float slowDownFactor;
    [Range(0, 1)] public float percentMaxSlow = 0.4f;

    public float slowDownRadius;
    
    public LayerMask playerMask;
    // Start is called before the first frame update

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, slowDownRadius);
    }

    void Start()
    {
        savedMaxSpeed = PlayerController.instance.maxSpeed;
        savedMinSpeed = PlayerController.instance.minSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!willSlowDownPlayer) return;
        var oui = Physics.OverlapSphere(transform.position, slowDownRadius,playerMask);
        foreach (var playerCollider in oui)
        {
            if (!playerCollider.gameObject.CompareTag("Player")) return;
            var currentSlowDown =
                Vector3.Distance(playerCollider.ClosestPoint(transform.position), transform.position) / (slowDownFactor + slowDownRadius);
            if (currentSlowDown < percentMaxSlow)
            {
                currentSlowDown = percentMaxSlow;
            }
            PlayerController.instance.maxSpeed = savedMaxSpeed * currentSlowDown;
            PlayerController.instance.minSpeed = savedMinSpeed * currentSlowDown;
            //METTRE LE SON ICI
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            chaseScript.TriggerGirl(true);
            if (willSlowDownPlayer)
            {
                StartCoroutine(ResetPlayerSpeed());
            }
        }
    }

    private IEnumerator ResetPlayerSpeed()
    {
        PlayerController.instance.maxSpeed = 0;
        PlayerController.instance.minSpeed = 0;
        yield return new WaitForSeconds(0.3f);
        willSlowDownPlayer = false;
        PlayerController.instance.maxSpeed = savedMaxSpeed;
        PlayerController.instance.minSpeed = savedMinSpeed;
    }
}
