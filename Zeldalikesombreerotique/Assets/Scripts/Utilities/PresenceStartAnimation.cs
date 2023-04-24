using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using Player;
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
        private bool _isOn;

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
            if (other.gameObject.CompareTag("Player") && !_isOn)
            {
                _isOn = true;
                StartCoroutine(PresenceStartAnimationCoroutine());
            }
        }
        
        private IEnumerator PresenceStartAnimationCoroutine()
        {
            yield return new WaitForSeconds(timeBeforeStart);
            PlayerController.instance.controls.Disable();
            PlayerController.instance.canMove = false;
            PlayerController.instance.rb.velocity = Vector3.zero;
            PlayerController.instance.rig.SetBool("isTripping",true);
            // _audioSource.PlayOneShot(soundToRotateSpot);
            spotToRotate.transform
                .DOLocalRotate(spotToRotate.transform.localEulerAngles + new Vector3(0, angleToRotateSpot, 0),
                    timeToRotateSpot).SetEase(easeToRotateSpot)
                .OnComplete(() =>
                {
                    StartCoroutine(FootprintsCoroutine());
                    PlayerController.instance.rig.SetBool("isTripping",false);
                });
        }
        
        private IEnumerator FootprintsCoroutine()
        {
            yield return new WaitForSeconds(timeBetweenSpotAndFootprints);
            
            foreach (var footprint in footprintsToAppear)
            {
                footprint.SetActive(true);
                // _audioSource.PlayOneShot(soundToAppearFootprints, footprint.transform.GetComponentInChildren<SpriteRenderer>().color.a);
                
                yield return new WaitForSeconds(timeBetweenFootprints);
            }
            PlayerController.instance.controls.Enable();
            PlayerController.instance.canMove = true;
            PlayerController.instance.rb.velocity = Vector3.zero;
            gameObject.SetActive(false);
        }

        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once SuggestBaseTypeForParameter
        private bool IsNotNull(AudioClip ac) { return ac != null; }
    }
}
