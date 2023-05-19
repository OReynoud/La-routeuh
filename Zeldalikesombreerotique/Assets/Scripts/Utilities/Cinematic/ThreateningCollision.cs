using System.Collections;
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

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Player") && !PlayerController.instance.isProtected)
            {
                CameraManager.Instance.BoutToBeKilled();
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
