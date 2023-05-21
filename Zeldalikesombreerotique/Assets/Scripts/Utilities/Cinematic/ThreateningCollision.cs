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
        
        [SerializeField] private float slowDownValue;
        [SerializeField] private float slowDownTime;
        [SerializeField] private Ease slowDownEase;

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player") && !PlayerController.instance.isProtected)
            {
                CameraManager.Instance.BoutToBeKilled();
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
                    
                    CameraManager.Instance.NoMoreBoutToBeKilled(true);
                }
            }
        }

        private IEnumerator KillPlayer()
        {
            yield return new WaitForSeconds(timeBeforeKill);
            
            CameraManager.Instance.NoMoreBoutToBeKilled();
            PlayerController.instance.transform.position = respawnPoint.position;
            
            _killPlayerCoroutine = null;
        }
    }
}
