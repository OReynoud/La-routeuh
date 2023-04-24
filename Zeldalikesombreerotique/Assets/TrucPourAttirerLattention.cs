using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using UnityEngine;
using Utilities;

public class TrucPourAttirerLattention : MonoBehaviour
{
    public float jumpDuration = 0.5f;
    public float interval;
    private bool isJumping = false;
    public DynamicObject trucABouger;
    private void OnTriggerEnter(Collider other)
    {
        if (!isJumping && other.CompareTag("Player") && !PlayerController.instance.isGrabbing && trucABouger.mobilityType != DynamicObject.MobilityType.None)
        {
            isJumping = true;
            trucABouger.transform.DOJump(trucABouger.transform.position, 0.2f, 1, jumpDuration);
            StartCoroutine(FollowUpJump());
        }
        
    }

    IEnumerator FollowUpJump()
    {
        yield return new WaitForSeconds(jumpDuration + interval);
        trucABouger.transform.DOJump(trucABouger.transform.position, 0.2f, 2, jumpDuration * 1.5f).AppendCallback((() =>
        {
            isJumping = false;
        }));
    }
}
