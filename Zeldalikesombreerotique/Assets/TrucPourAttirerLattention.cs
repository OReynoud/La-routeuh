using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TrucPourAttirerLattention : MonoBehaviour
{
    public float jumpDuration = 0.5f;
    public float interval;
    private bool isJumping = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!isJumping && other.CompareTag("Player"))
        {
            isJumping = true;
            transform.DOJump(transform.position, 0.2f, 1, jumpDuration);
            StartCoroutine(FollowUpJump());
        }
        
    }

    IEnumerator FollowUpJump()
    {
        yield return new WaitForSeconds(jumpDuration + interval);
        transform.DOJump(transform.position, 0.2f, 2, jumpDuration * 1.5f).AppendCallback((() =>
        {
            isJumping = false;
        }));
    }
}
