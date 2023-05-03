using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public class StreetLightTriggerBehavior : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private GameObject spotLight;
        [SerializeField] private GameObject coneMesh;
        [SerializeField] private AudioClip switchOnSound;
        [SerializeField] private List<GameObject> footprintsToAppear;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                spotLight.SetActive(true);
                coneMesh.SetActive(true);
                // audioSource.PlayOneShot(switchOnSound);
                
                foreach (var footprint in footprintsToAppear)
                {
                    footprint.SetActive(true);
                }
                
                gameObject.SetActive(false);
            }
        }
    }
}
