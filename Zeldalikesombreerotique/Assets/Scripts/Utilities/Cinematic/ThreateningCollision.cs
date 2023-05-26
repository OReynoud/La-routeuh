using System.Collections;
using DG.Tweening;
using Managers;
using Player;
using UnityEngine;

namespace Utilities.Cinematic
{
    public class ThreateningCollision : MonoBehaviour
    {
        [SerializeField] private float timeBeforeKill;
        [SerializeField] private Transform respawnPoint;
        private Coroutine _killPlayerCoroutine;
        private float _tempPlayerMaxSpeed;
        private Tween _slowDownTween;
        
        [SerializeField] private float slowDownValue;
        [SerializeField] private float slowDownTime;
        [SerializeField] private Ease slowDownEase;

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player") && !PlayerController.instance.isProtected)
            {
                CameraManager.Instance.BoutToBeKilled();
                _tempPlayerMaxSpeed = PlayerController.instance.maxSpeed;
                DOTween.To(()=> PlayerController.instance.maxSpeed, x=> PlayerController.instance.maxSpeed = x, slowDownValue, slowDownTime).SetEase(slowDownEase);
                _killPlayerCoroutine = StartCoroutine(KillPlayer());
            }
        }

        private void OnCollisionExit(Collision other)
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
            PlayerController.instance.transform.position = respawnPoint.position;
            PlayerController.instance.maxSpeed = _tempPlayerMaxSpeed;
            
            _killPlayerCoroutine = null;
        }
    }
}
