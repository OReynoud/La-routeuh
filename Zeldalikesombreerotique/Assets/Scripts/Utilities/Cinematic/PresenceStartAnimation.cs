using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using Player;
using Unity.VisualScripting;
using UnityEngine;

namespace Utilities.Cinematic
{
    public class PresenceStartAnimation : MonoBehaviour
    {
        // Cached components
        private AudioSource _audioSource;
        
        // Spot
        [Foldout("Spot")] [SerializeField] private float timeBeforeSpot;
        [Foldout("Spot")] [SerializeField] private GameObject spotToRotate;
        [Foldout("Spot")] [SerializeField] private float angleToRotateSpot;
        [Foldout("Spot")] [SerializeField] private float timeToRotateSpot;
        [Foldout("Spot")] [SerializeField] private Ease easeToRotateSpot;
        [Foldout("Spot")] [SerializeField] private AudioClip soundToRotateSpot;
        
        // Footprints
        [Space]
        [Foldout("Footprints")] [SerializeField] private List<GameObject> footprintsToAppear;
        [Foldout("Footprints")] [SerializeField] private float timeBetweenFootprints;
        [Foldout("Footprints")] [SerializeField] private AudioClip soundToAppearFootprints;
        [Foldout("Footprints")] [SerializeField] private float randomValuePitchFootprints;
        
        // Little sister
        [Space]
        [Foldout("Little Sister")] [SerializeField] private Parkour scriptFille;
        [Foldout("Little Sister")] [SerializeField] private float timeBeforeFall;
        [Foldout("Little Sister")] [SerializeField] private float timeToFall;

        // Private variables
        private bool _isOn;
        
        // Hashed strings
        private static readonly int IsTripping = Animator.StringToHash("isTripping");

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && !_isOn)
            {
                _isOn = true;
                
                StartCoroutine(FootprintsCoroutine());
                StartCoroutine(SpotCoroutine());
                StartCoroutine(WaitToFallCoroutine());
            }
        }
        
        private IEnumerator SpotCoroutine()
        {
            yield return new WaitForSeconds(timeBeforeSpot);
            
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
            scriptFille.TriggerGirl();
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
            
            gameObject.SetActive(false);
        }

        private IEnumerator WaitToFallCoroutine()
        {
            yield return new WaitForSeconds(timeBeforeFall);
            
            PlayerController.instance.controls.Disable();
            PlayerController.instance.canMove = false;
            PlayerController.instance.rb.velocity = Vector3.zero;
            PlayerController.instance.rig.SetBool(IsTripping,true);
            
            yield return new WaitForNextFrameUnit();
            
            PlayerController.instance.rig.SetBool(IsTripping,false);
            
            yield return new WaitForSeconds(timeToFall);
            
            PlayerController.instance.controls.Enable();
            PlayerController.instance.canMove = true;
            PlayerController.instance.rb.velocity = Vector3.zero;
        }
    }
}
