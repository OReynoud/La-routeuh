using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

namespace Utilities
{
    public class PresenceStartAnimation : MonoBehaviour
    {
        private AudioSource _audioSource;
        [SerializeField] private float timeBeforeStart;
        [SerializeField] private GameObject spotToRotate;
        [SerializeField] private float angleToRotateSpot;
        [SerializeField] private float timeToRotateSpot;
        [SerializeField] private Ease easeToRotateSpot;
        [ValidateInput("IsNotNull", "Don't forget to uncomment the line in code to play the sound.")]
        [SerializeField] private AudioClip soundToRotateSpot;
        [SerializeField] private float timeBetweenSpotAndFootprints;
        [SerializeField] private List<GameObject> footprintsToAppear;
        [SerializeField] private float timeBetweenFootprints;
        [ValidateInput("IsNotNull", "Don't forget to uncomment the line in code to play the sound.")] 
        [SerializeField] private AudioClip soundToAppearFootprints;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            
            foreach (var footprint in footprintsToAppear)
            {
                footprint.SetActive(false);
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                StartCoroutine(PresenceStartAnimationCoroutine());
            }
        }
        
        private IEnumerator PresenceStartAnimationCoroutine()
        {
            yield return new WaitForSeconds(timeBeforeStart);
            // _audioSource.PlayOneShot(soundToRotateSpot);
            spotToRotate.transform
                .DOLocalRotate(spotToRotate.transform.localEulerAngles + new Vector3(0, angleToRotateSpot, 0),
                    timeToRotateSpot).SetEase(easeToRotateSpot)
                .OnComplete(() => StartCoroutine(FootprintsCoroutine()));
        }
        
        private IEnumerator FootprintsCoroutine()
        {
            yield return new WaitForSeconds(timeBetweenSpotAndFootprints);
            foreach (var footprint in footprintsToAppear)
            {
                footprint.SetActive(true);
                // _audioSource.PlayOneShot(soundToAppearFootprints);
                yield return new WaitForSeconds(timeBetweenFootprints);
            }
        }

        private bool IsNotNull(AudioClip ac) { return ac != null; }
    }
}
