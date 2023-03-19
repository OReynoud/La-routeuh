using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public class LoopingMap : MonoBehaviour
    {
        [SerializeField] private Transform newPosition;
        [SerializeField] private GameObject mapPartToMove;
        [SerializeField] private List<GameObject> mapPartsToDisable = new();
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                mapPartToMove.transform.position = newPosition.position;
                mapPartToMove.SetActive(true);
                foreach (var mapPart in mapPartsToDisable)
                {
                    mapPart.SetActive(false);
                }
            }
        }
    }
}
