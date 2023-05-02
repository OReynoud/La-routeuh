using UnityEngine;

namespace Utilities
{
    public class StreetLightTriggerBehavior : MonoBehaviour
    {
        [SerializeField] private GameObject spotLight;
        [SerializeField] private GameObject coneMesh;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                spotLight.SetActive(true);
                coneMesh.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }
}
