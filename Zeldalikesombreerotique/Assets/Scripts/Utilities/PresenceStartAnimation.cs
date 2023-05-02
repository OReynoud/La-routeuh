using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using Unity.VisualScripting;
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
        [SerializeField] private AudioClip soundToRotateSpot;
        [SerializeField] private float timeBetweenSpotAndFootprints;
        [SerializeField] private List<GameObject> footprintsToAppear;
        [SerializeField] private float timeBetweenFootprints;
        [SerializeField] private AudioClip soundToAppearFootprints;
        [SerializeField] private float randomValuePitchFootprints;
        private bool _isOn;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            
            /*foreach (var footprint in footprintsToAppear)
            {
                footprint.SetActive(false);
            }*/
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && !_isOn)
            {
                _isOn = true;
                StartCoroutine(PresenceStartAnimationCoroutine());
            }
        }
        
        private IEnumerator PresenceStartAnimationCoroutine()
        {
            yield return new WaitForSeconds(timeBeforeStart);

            StartCoroutine(WaitToFallCoroutine());
            
            _audioSource.PlayOneShot(soundToRotateSpot);
            
            spotToRotate.transform
                .DOLocalRotate(spotToRotate.transform.localEulerAngles + new Vector3(0, angleToRotateSpot, 0),
                    timeToRotateSpot).SetEase(easeToRotateSpot)
                .OnComplete(() =>
                {
                    StartCoroutine(FootprintsCoroutine());
                });
        }
        
        private IEnumerator FootprintsCoroutine()
        {
            yield return new WaitForNextFrameUnit();
            
            PlayerController.instance.rig.SetBool("isTripping",false);
            
            yield return new WaitForSeconds(timeBetweenSpotAndFootprints);

            var i = 0;
            
            foreach (var footprint in footprintsToAppear)
            {
                footprint.SetActive(true);
                
                if (i % 2 == 0)
                {
                    _audioSource.PlayOneShot(soundToAppearFootprints);
                    _audioSource.pitch = 1f + Random.Range(-randomValuePitchFootprints, randomValuePitchFootprints);
                }

                i++;
                
                yield return new WaitForSeconds(timeBetweenFootprints);
            }
            PlayerController.instance.controls.Enable();
            PlayerController.instance.canMove = true;
            PlayerController.instance.rb.velocity = Vector3.zero;
            gameObject.SetActive(false);
        }

        private IEnumerator WaitToFallCoroutine()
        {
            yield return new WaitForSeconds(timeToRotateSpot * 0.5f);
            
            PlayerController.instance.controls.Disable();
            PlayerController.instance.canMove = false;
            PlayerController.instance.rb.velocity = Vector3.zero;
            PlayerController.instance.rig.SetBool("isTripping",true);
        }
    }
}
