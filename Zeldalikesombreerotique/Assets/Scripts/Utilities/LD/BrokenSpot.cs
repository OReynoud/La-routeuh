using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;

namespace Utilities.LD
{
    public class BrokenSpot : MonoBehaviour
    {
        [Tooltip("Audio source")] [SerializeField] private AudioSource audioSource;
        [Tooltip("Light game object")] [SerializeField] private GameObject lightObject;
        [Tooltip("Time before blink begins")] [SerializeField] private float timeBeforeBlink;
        [Tooltip("Time between each blink")] [SerializeField] private List<float> timesBetweenBlinks;
        [Tooltip("Duration of a blink when it is lighted")] [SerializeField] private float lightedBlinkDuration;
        [Tooltip("Sound for the blink")] [SerializeField] private AudioClip blinkSound;
        private Coroutine _blinkCoroutine;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _blinkCoroutine ??= StartCoroutine(BlinkCoroutine());
            }
        }

        private IEnumerator BlinkCoroutine()
        {
            yield return new WaitForSeconds(timeBeforeBlink);
            foreach (var time in timesBetweenBlinks)
            {
                if (LightedElementsManager.Instance.OriginOfTheCheck == lightObject)
                {
                    LightedElementsManager.Instance.MapPartOrigin = lightObject;
                }
                else
                {
                    lightObject.SetActive(false);
                }
                audioSource.PlayOneShot(blinkSound);
                yield return new WaitForSeconds(time);
                lightObject.SetActive(true);
                yield return new WaitForSeconds(lightedBlinkDuration);
            }
            if (LightedElementsManager.Instance.OriginOfTheCheck == lightObject)
            {
                LightedElementsManager.Instance.MapPartOrigin = lightObject;
            }
            else
            {
                lightObject.SetActive(false);
            }
            gameObject.SetActive(false);
        }
    }
}
