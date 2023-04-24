using UnityEngine;

namespace Utilities
{
    public class SoundTrigger : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private bool isPlayingOnce;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                audioSource.PlayOneShot(audioClip);
                if (isPlayingOnce)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
