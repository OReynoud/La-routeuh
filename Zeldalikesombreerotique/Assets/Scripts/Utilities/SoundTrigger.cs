using UnityEngine;

namespace Utilities
{
    public class SoundTrigger : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                audioSource.enabled = false;
                audioSource.enabled = true;
                gameObject.SetActive(false);
            }
        }
    }
}
