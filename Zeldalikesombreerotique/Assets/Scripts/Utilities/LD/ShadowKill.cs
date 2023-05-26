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
                _tempPlayerMaxSpeed = PlayerController.instance.maxSpeed;
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
                    PlayerController.instance.maxSpeed = _tempPlayerMaxSpeed;
                    CameraManager.Instance.NoMoreBoutToBeKilled(true);
                }
            }
        }

        private IEnumerator KillPlayer()
        {
            yield return new WaitForSeconds(timeBeforeKill);
            
            CameraManager.Instance.NoMoreBoutToBeKilled();
            PlayerController.instance.transform.position = RespawnPoint.position;
            PlayerController.instance.maxSpeed = _tempPlayerMaxSpeed;
            
            _killPlayerCoroutine = null;
        }
    }
}
