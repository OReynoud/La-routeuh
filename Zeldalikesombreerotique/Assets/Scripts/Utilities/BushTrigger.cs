using DG.Tweening;
using UnityEngine;

namespace Utilities
{
    public class BushTrigger : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip audioClip;
        [SerializeField] private GameObject bush;
        [SerializeField] private float shakeDuration;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                audioSource.PlayOneShot(audioClip);
                bush.transform.DOShakePosition(shakeDuration);
                gameObject.SetActive(false);
            }
        }
    }
}
