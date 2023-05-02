using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public class BrokenSpot : MonoBehaviour
    {
        [Tooltip("Audio source")] [SerializeField] private AudioSource audioSource;
        [Tooltip("Light game object")] [SerializeField] private GameObject lightObject;
        [Tooltip("Time before blink begins")] [SerializeField] private float timeBeforeBlink;
        [Tooltip("Time between each blink")] [SerializeField] private List<float> timesBetweenBlinks;
        [Tooltip("Duration of a blink when it is lighted")] [SerializeField] private float lightedBlinkDuration;
        [Tooltip("Sound for the blink")] [SerializeField] private AudioClip blinkSound;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                StartCoroutine(BlinkCoroutine());
            }
        }

        private IEnumerator BlinkCoroutine()
        {
            yield return new WaitForSeconds(timeBeforeBlink);
            foreach (var time in timesBetweenBlinks)
            {
                lightObject.SetActive(false);
                audioSource.PlayOneShot(blinkSound);
                yield return new WaitForSeconds(time);
                lightObject.SetActive(true);
                yield return new WaitForSeconds(lightedBlinkDuration);
            }
            lightObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
