using System.Collections;
using DG.Tweening;
using Managers;
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
        
        [SerializeField] private float slowDownValue;
        [SerializeField] private float slowDownTime;
        [SerializeField] private Ease slowDownEase;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && !PlayerController.instance.isProtected)
            {
                CameraManager.Instance.BoutToBeKilled();
                _slowDownTween = DOTween.To(()=> PlayerController.instance.maxSpeed, x=> PlayerController.instance.maxSpeed = x, slowDownValue, slowDownTime).SetEase(slowDownEase);
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
            PlayerController.instance.isDead = true;
            
            CameraManager.Instance.NoMoreBoutToBeKilled();
            PlayerController.instance.transform.position = RespawnPoint.position;
            PlayerController.instance.maxSpeed = _tempPlayerMaxSpeed;
            PlayerController.instance.rig[0].Play("idle");
            PlayerController.instance.rig[1].Play("idle");
            PlayerController.instance.rig[0].SetBool("isWalking",false);
            PlayerController.instance.rig[0].SetBool("isPulling",false);
            PlayerController.instance.rig[0].SetBool("isPushing",false);
            PlayerController.instance.rig[0].SetBool("IsGrabbing",false);
            PlayerController.instance.pushingPullingRotate = false;
            PlayerController.instance.isGrabbing = false;
            PlayerController.instance.canMove = true;
            PlayerController.instance.joint.autoConfigureConnectedAnchor = true;
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
            PlayerController.instance.isDead = false;
        }
    }
}
