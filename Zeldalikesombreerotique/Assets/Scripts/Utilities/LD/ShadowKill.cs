using Player;
using UnityEngine;

namespace Utilities
{
    public class ShadowKill : MonoBehaviour
    {
        internal Transform RespawnPoint;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && !PlayerController.instance.isProtected)
            {
                other.gameObject.transform.position = RespawnPoint.position;
            }
        }
    }
}
