using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Utilities
{
    public class PresenceStartAnimation : MonoBehaviour
    {
        [SerializeField] private float timeBeforeStart;
        [SerializeField] private GameObject spotToRotate;
        [SerializeField] private float angleToRotateSpot;
        [SerializeField] private float timeToRotateSpot;
        [SerializeField] private Ease easeToRotateSpot;
        [SerializeField] private float timeBetweenSpotAndFootprints;
        [SerializeField] private List<GameObject> footprintsToAppear;
        [SerializeField] private float timeBetweenFootprints;

        private void Awake()
        {
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
                yield return new WaitForSeconds(timeBetweenFootprints);
            }
        }
    }
}
