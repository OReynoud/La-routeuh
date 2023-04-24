using System.Collections;
using UnityEngine;

namespace Utilities
{
    public class BrokenSpot : MonoBehaviour
    {
        [Tooltip("Light game object")] [SerializeField] private GameObject lightObject;
        [Tooltip("Time before blink begins")] [SerializeField] private float timeBeforeBlink;
        [Tooltip("Time between two blinks begins")] [SerializeField] private float timeBetweenBlinks;
        [Tooltip("Number of blinks")] [SerializeField] private int numberOfBlinks;

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
            for (var i = 0; i < numberOfBlinks; i++)
            {
                lightObject.SetActive(false);
                yield return new WaitForSeconds(timeBetweenBlinks * Random.value);
                lightObject.SetActive(true);
                yield return new WaitForSeconds(timeBetweenBlinks * Random.value);
            }
            lightObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
