using System;
using System.Collections;
using DG.Tweening;
using Managers;
using Player;
using UnityEngine;

namespace Utilities.Cinematic
{
    public class ThreateningCollision : MonoBehaviour
    {
        public AudioSource heartbeat;
        public AudioSource breathing;
        
        [SerializeField] private float timeBeforeKill;
        [SerializeField] private Transform respawnPoint;
        private Coroutine _killPlayerCoroutine;
        private float _tempPlayerMaxSpeed;
        private Tween _slowDownTween;
        
        [SerializeField] private float slowDownValue;
        [SerializeField] private float slowDownTime;
        [SerializeField] private Ease slowDownEase;

        private void Awake()
        {
            heartbeat = GameObject.FindGameObjectWithTag("Heartbeat").GetComponent<AudioSource>();
            breathing = GameObject.FindGameObjectWithTag("Breathing").GetComponent<AudioSource>();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player") && !PlayerController.instance.isProtected)
            {


                heartbeat.DOFade(1, 0.5f);
                breathing.DOFade(1, 0.5f);
                
                
                CameraManager.Instance.BoutToBeKilled();
                _tempPlayerMaxSpeed = PlayerController.instance.maxSpeed;
                _slowDownTween = DOTween.To(()=> PlayerController.instance.maxSpeed, x=> PlayerController.instance.maxSpeed = x, slowDownValue, slowDownTime).SetEase(slowDownEase);
                _killPlayerCoroutine = StartCoroutine(KillPlayer());
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag("Player") && !PlayerController.instance.isProtected)
            {
                if (_killPlayerCoroutine != null)
                {
                    heartbeat.DOFade(0, 0.5f);
                    breathing.DOFade(0, 0.5f);

                    StopCoroutine(_killPlayerCoroutine);
                    _killPlayerCoroutine = null;
                    
                    _slowDownTween.Kill();
                    PlayerController.instance.maxSpeed = _tempPlayerMaxSpeed;
                    CameraManager.Instance.NoMoreBoutToBeKilled(true);
                }
            }
        }

        private IEnumerator KillPlayer()
        {
            
            yield return new WaitForSeconds(timeBeforeKill);
            PlayerController.instance.isDead = true;
            _slowDownTween.Kill();
            CameraManager.Instance.NoMoreBoutToBeKilled();
            heartbeat.DOFade(0, 0.5f);
            breathing.DOFade(0, 0.5f);
            PlayerController.instance.transform.position = respawnPoint.position;
            PlayerController.instance.maxSpeed = PlayerController.instance.savedMaxSpeed;
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
            PlayerController.instance.SetJoint(false);
            if (PlayerController.instance.objectToGrab)
            {
                PlayerController.instance.objectToGrab.position = PlayerController.instance.objectType.spawnPos;
                PlayerController.instance.objectToGrab = null;
            }
            PlayerController.instance.objectType = null;

            _killPlayerCoroutine = null;
            yield return new WaitForFixedUpdate();
            yield return new WaitForEndOfFrame();
            PlayerController.instance.maxSpeed = PlayerController.instance.savedMaxSpeed;
            PlayerController.instance.isDead = false;
        }
    }
}
