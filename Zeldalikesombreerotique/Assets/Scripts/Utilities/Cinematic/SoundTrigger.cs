using UnityEngine;

namespace Utilities
{
    public class SoundTrigger : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private bool isPlayingOnce;
        [SerializeField] private bool isLooping;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                if (!isLooping)
                {
                    audioSource.PlayOneShot(audioClip);
                }
                else
                {
                    audioSource.clip = audioClip;
                    audioSource.Play();
                }
                if (isPlayingOnce)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
