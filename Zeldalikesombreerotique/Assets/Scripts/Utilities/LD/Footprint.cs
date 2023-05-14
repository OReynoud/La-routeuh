using UnityEngine;

namespace Utilities.LD
{
    public class Footprint : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip footstepSound;
    
        internal void PlayFootstepSound(float pitch)
        {
            audioSource.pitch = pitch;
            audioSource.PlayOneShot(footstepSound);
        }
    }
}
