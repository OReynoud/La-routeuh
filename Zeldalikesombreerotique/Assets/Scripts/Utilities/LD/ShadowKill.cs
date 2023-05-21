using System.Collections;
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && !PlayerController.instance.isProtected)
            {
                CameraManager.Instance.BoutToBeKilled();
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
                    
                    CameraManager.Instance.NoMoreBoutToBeKilled(true);
                }
            }
        }

        private IEnumerator KillPlayer()
        {
            yield return new WaitForSeconds(timeBeforeKill);
            
            CameraManager.Instance.NoMoreBoutToBeKilled();
            PlayerController.instance.transform.position = RespawnPoint.position;
            
            _killPlayerCoroutine = null;
        }
    }
}
