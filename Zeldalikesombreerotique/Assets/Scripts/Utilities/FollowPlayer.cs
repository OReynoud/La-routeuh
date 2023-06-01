using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Animator rig;

    public float minDistanceToPlayer;
    public float triggerDistance;
    public float baseSpeed;
    public float catchupMultiplier;
    public float timeToResetPos;
    private float resetTimer;
    public bool isTriggered;
    public Transform spawnPoint;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var dir = PlayerController.instance.transform.position - transform.position;
        var dirNormed = dir.normalized;
        var angle = Mathf.Atan2(dirNormed.x, dirNormed.z) * Mathf.Rad2Deg;
        var distanceWithPlayer = Vector3.Distance(transform.position, PlayerController.instance.transform.position);
        transform.rotation = Quaternion.AngleAxis(angle,Vector3.up);
        if (isTriggered &&  distanceWithPlayer < triggerDistance)
        {
            Follow(distanceWithPlayer);
        }

        if (!isTriggered)
        {
            if (resetTimer <= timeToResetPos)
            {
                resetTimer += Time.fixedDeltaTime;
            }
            else
            {
                transform.position = spawnPoint.position;
            }

        }
    }

    public void Follow(float distance)
    {
        resetTimer = 0;
        transform.position =
            Vector3.MoveTowards(transform.position, PlayerController.instance.transform.position, baseSpeed  * (distance/triggerDistance));
    }
}
