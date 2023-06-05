using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class Cheats : MonoBehaviour
{
    public static Cheats instance;
    public Transform[] checkpoints;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
        }

        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.F5))
        {
            Tp();
        }
        if (Input.GetKey(KeyCode.F6))
        {
            
        }
        if (Input.GetKey(KeyCode.F7))
        {
            Debug.Log(PlayerController.instance.gameObject.layer);
        }
    }

    void Tp()
    {
        var nearestCheckPoint = checkpoints[0];
        foreach (var point in checkpoints)
        {
            if (!point.gameObject.activeInHierarchy) continue;
            if (Vector3.Distance(PlayerController.instance.transform.position,point.position) <=Vector3.Distance(PlayerController.instance.transform.position,nearestCheckPoint.position))
            {
                nearestCheckPoint = point;
            }
        }
        Debug.Log("Tp to: ",nearestCheckPoint);
        PlayerController.instance.maxSpeed = PlayerController.instance.savedMaxSpeed;
        PlayerController.instance.rig[0].Play("idle");
        PlayerController.instance.rig[1].Play("idle");
        PlayerController.instance.rig[0].SetBool("isWalking",false);
        PlayerController.instance.rig[0].SetBool("IsPushing",false);
        PlayerController.instance.rig[0].SetBool("IsGrabbing",false);
        PlayerController.instance.pushingPullingRotate = false;
        PlayerController.instance.isGrabbing = false;
        PlayerController.instance.canMove = true;
        PlayerController.instance.joint.autoConfigureConnectedAnchor = true;
        PlayerController.instance.SetJoint(false);
        if (PlayerController.instance.objectToGrab)
        {
            PlayerController.instance.objectToGrab.constraints = RigidbodyConstraints.FreezeRotation;
            PlayerController.instance.objectToGrab.position = PlayerController.instance.objectType.spawnPos;
            PlayerController.instance.objectToGrab = null;
        }

        PlayerController.instance.objectType = null;
        PlayerController.instance.transform.position = nearestCheckPoint.position;
    }
}
