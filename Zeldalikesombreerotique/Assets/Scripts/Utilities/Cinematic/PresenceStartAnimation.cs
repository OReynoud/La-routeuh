using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using Utilities.LD;

namespace Utilities.Cinematic
{
    public class PresenceStartAnimation : MonoBehaviour
    {
        // Cached components
        private AudioSource _audioSource;
        
        // Spot
        [Header("Spot")]
        [SerializeField] private bool hasSpotToRotate;
        [ShowIf("hasSpotToRotate")] [SerializeField] private float timeBeforeSpot;
        [ShowIf("hasSpotToRotate")] [SerializeField] private GameObject spotToRotate;
        [ShowIf("hasSpotToRotate")] [SerializeField] private float angleToRotateSpot;
        [ShowIf("hasSpotToRotate")] [SerializeField] private float timeToRotateSpot;
        [ShowIf("hasSpotToRotate")] [SerializeField] private Ease easeToRotateSpot;
        [ShowIf("hasSpotToRotate")] [SerializeField] private AudioClip  soundToRotateSpot;
        
        // Footprints
        [Header("Footprints")]
        [SerializeField] private List<GameObject> footprintsToAppear; 
        [SerializeField] private float timeToBeginFootprints;
        [SerializeField] private float timeBetweenFootprints;
        [SerializeField] private float randomValuePitchFootprints;
        
        // Little sister
        [Header("Little Sister")]
        [SerializeField] private Parkour littleSisterScript;
        [SerializeField] private bool isStandingAtStart;

        // Fall
        [Header("Fall")]
        [SerializeField] private bool hasToFall;
        [ShowIf("hasToFall")] [SerializeField] private float timeBeforeFall;
        [ShowIf("hasToFall")] [SerializeField] private Ease easeToSlowDownBeforeFall;
        [ShowIf("hasToFall")] [SerializeField] private float timeToFall;

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
                if (hasSpotToRotate) StartCoroutine(SpotCoroutine());
                if (hasToFall) StartCoroutine(WaitToFallCoroutine());
            }
        }
        
        private IEnumerator SpotCoroutine()
        {
            yield return new WaitForSeconds(timeBeforeSpot);
            
            _audioSource.PlayOneShot(soundToRotateSpot);
            
            spotToRotate.transform
                .DOLocalRotate(spotToRotate.transform.localEulerAngles + new Vector3(0, angleToRotateSpot, 0),
                    timeToRotateSpot).SetEase(easeToRotateSpot);
        }
        
        private IEnumerator FootprintsCoroutine()
        {
            yield return new WaitForSeconds(timeToBeginFootprints);
            littleSisterScript.TriggerGirl(isStandingAtStart);
            var i = 0;
            
            foreach (var footprint in footprintsToAppear)
            {
                footprint.SetActive(true);
                
                if (i % 3 == 0)
                {
                    footprint.GetComponent<Footprint>().PlayFootstepSound(1f + Random.Range(-randomValuePitchFootprints, randomValuePitchFootprints));
                }

                i++;
                
                yield return new WaitForSeconds(timeBetweenFootprints);
            }
            
            gameObject.SetActive(false);
        }

        private IEnumerator WaitToFallCoroutine()
        {
            yield return new WaitForEndOfFrame();
            PlayerController.instance.introCinematic = true;
            PlayerController.instance.rig[0].SetBool("isWalking",true);
            PlayerController.instance.rig[1].SetBool("isWalking",true);
            var maxSpeedTemp = PlayerController.instance.maxSpeed;
            var minSpeedTemp = PlayerController.instance.minSpeed;
            /*DOTween.To(()=> PlayerController.instance.maxSpeed, x=> PlayerController.instance.maxSpeed = x, 0f, timeToBeginFootprints).SetEase(easeToSlowDownBeforeFall);
            DOTween.To(()=> PlayerController.instance.minSpeed, x=> PlayerController.instance.minSpeed = x, 0f, timeToBeginFootprints).SetEase(easeToSlowDownBeforeFall);*/
            DOTween.To(()=> PlayerController.instance.rb.velocity, x=> PlayerController.instance.rb.velocity = x, Vector3.zero, timeToBeginFootprints).SetEase(easeToSlowDownBeforeFall);
            DOTween.To(()=> PlayerController.instance.rig[0].GetFloat("Speed"), x=> PlayerController.instance.rig[0].GetFloat("Speed"), 0, timeToBeginFootprints).SetEase(easeToSlowDownBeforeFall);
            DOTween.To(()=> PlayerController.instance.rig[1].GetFloat("Speed"), x=> PlayerController.instance.rig[1].GetFloat("Speed"), 0, timeToBeginFootprints).SetEase(easeToSlowDownBeforeFall);
            CinematicBands.instance.OpenBands();
            PlayerController.instance.controls.Disable();
            PlayerController.instance.canMove = false;
            yield return new WaitForSeconds(timeToBeginFootprints);
            PlayerController.instance.rig[0].SetBool("isWalking",false);
            PlayerController.instance.rig[1].SetBool("isWalking",false);
            yield return new WaitForSeconds(timeBeforeFall - timeToBeginFootprints);
            PlayerController.instance.rb.velocity = Vector3.zero;
            StartCoroutine(PlayerController.instance.OmgJeSuisSurpris(littleSisterScript.objectToMove.transform));
            //PlayerController.instance.rig[0].SetBool(IsTripping,true);
            
           // yield return new WaitForNextFrameUnit();
            
            
            yield return new WaitForSeconds(timeToFall);
            PlayerController.instance.introCinematic = false;
            PlayerController.instance.rig[0].SetBool(IsTripping,false);
            CinematicBands.instance.CloseBands();
            PlayerController.instance.controls.Enable();
            PlayerController.instance.canMove = true;
            PlayerController.instance.rb.velocity = Vector3.zero;
            PlayerController.instance.maxSpeed = maxSpeedTemp;
            PlayerController.instance.minSpeed = minSpeedTemp;
        }
    }
}
