using UnityEngine;

namespace Utilities
{
    public class LoopingMap : MonoBehaviour
    {
        [SerializeField] private Transform newPosition;
        [SerializeField] private GameObject mapPartToMove;
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                mapPartToMove.transform.position = newPosition.position;
            }
        }
    }
}
