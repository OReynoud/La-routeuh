using Player;
using UnityEngine;

namespace Utilities
{
    public class Shadow : MonoBehaviour
    {
        [Tooltip("Point where the player will respawn if they are killed by the shadow")] [SerializeField] internal Transform respawnPoint;
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && !PlayerController.instance.isProtected)
            {
                other.gameObject.transform.position = respawnPoint.position;
            }
        }
    }
}
