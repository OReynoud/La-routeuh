using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Managers;
using NaughtyAttributes;
using Player;
using UnityEngine;

namespace Utilities.LD
{
    public class ShadowKill : MonoBehaviour
    {
        [SerializeField] private float timeBeforeKill = 1f;
        internal Transform RespawnPoint;
        private Coroutine _killPlayerCoroutine;
        private float _tempPlayerMaxSpeed;
        private Tween _slowDownTween;

        [ShowNonSerializedField] private const float SlowDownValue = 1;
        [ShowNonSerializedField] private const float SlowDownTime = 0.1f;
        [ShowNonSerializedField] private const Ease SlowDownEase = Ease.Linear;

        internal bool IsPuzzle4Shadow;
        internal List<GameObject> TriggersToReset;
        internal List<AudioSource> AudiosToReset;
        internal Shadow Shadow;
        
        private static readonly int IsWalking = Animator.StringToHash("isWalking");
        private static readonly int IsPushing = Animator.StringToHash("IsPushing");
        private static readonly int IsGrabbing = Animator.StringToHash("IsGrabbing");

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && !PlayerController.instance.isProtected)
            {
                CameraManager.Instance.BoutToBeKilled();
                _slowDownTween = DOTween.To(()=> PlayerController.instance.maxSpeed, x=> PlayerController.instance.maxSpeed = x, SlowDownValue, SlowDownTime).SetEase(SlowDownEase);
                _killPlayerCoroutine = StartCoroutine(KillPlayer());
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && !PlayerController.instance.isProtected)
            {
                if (_killPlayerCoroutine != null)
                {
                    StopCoroutine(_killPlayerCoroutine);
                    _killPlayerCoroutine = null;
                    
                    _slowDownTween.Kill();
                    CameraManager.Instance.NoMoreBoutToBeKilled(true);
                    PlayerController.instance.maxSpeed = PlayerController.instance.savedMaxSpeed;
                }
            }
        }

        private IEnumerator KillPlayer()
        {
            yield return new WaitForSeconds(timeBeforeKill);

            if (IsPuzzle4Shadow)
            {
                foreach (var trigger in TriggersToReset)
                {
                    trigger.SetActive(true);
                    trigger.GetComponent<BoxCollider>().enabled = true;
                }
                
                foreach (var audioSource in AudiosToReset)
                {
                    audioSource.Stop();
                    audioSource.volume = 1f;
                }
                
                Shadow.ResetShadowPuzzle4();
            }
            
            CameraManager.Instance.Kill();
            
            PlayerController.instance.isDead = true;
            _slowDownTween.Kill();
            PlayerController.instance.transform.position = RespawnPoint.position;
            PlayerController.instance.maxSpeed = PlayerController.instance.savedMaxSpeed;
            PlayerController.instance.rig[0].Play("idle");
            PlayerController.instance.rig[1].Play("idle");
            PlayerController.instance.rig[0].SetBool(IsWalking,false);
            PlayerController.instance.rig[0].SetBool(IsPushing,false);
            PlayerController.instance.rig[0].SetBool(IsGrabbing,false);
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

            _killPlayerCoroutine = null;
            
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            PlayerController.instance.maxSpeed = PlayerController.instance.savedMaxSpeed;
            PlayerController.instance.isDead = false;
            
            CameraManager.Instance.NoMoreKill();
            CameraManager.Instance.NoMoreBoutToBeKilled();
        }
    }
}
