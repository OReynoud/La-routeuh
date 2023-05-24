using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Utilities.LD;

namespace Utilities.Cinematic
{
    public class StreetLightTriggerBehavior : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private GameObject spotLight;
        [SerializeField] private GameObject coneMesh;
        [SerializeField] private AudioClip switchOnSound;
        [SerializeField] private AudioClip switchOffSound;
        [SerializeField] private List<GameObject> objectsToAppear;
        [SerializeField] private bool canSwitchOff;
        [ShowIf("canSwitchOff")] [SerializeField] private float timeBeforeSwitchOff;
        private Coroutine _switchOffCoroutine;
        private WaitForSeconds _switchOffWaitForSeconds;
        
        private void Awake()
        {
            _switchOffWaitForSeconds = new WaitForSeconds(timeBeforeSwitchOff);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (_switchOffCoroutine != null)
                {
                    StopCoroutine(_switchOffCoroutine);
                }
                
                spotLight.SetActive(true);
                coneMesh.SetActive(true);
                audioSource.PlayOneShot(switchOnSound);
                
                foreach (var objectToAppear in objectsToAppear)
                {
                    objectToAppear.transform.GetChild(0).gameObject.SetActive(true);
                    if (objectToAppear.CompareTag("Draw")) objectToAppear.GetComponent<Draw>().Enable();
                }

                if (!canSwitchOff)
                {
                    gameObject.SetActive(false);
                }
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && canSwitchOff)
            {
                _switchOffCoroutine = StartCoroutine(SwitchOff());
            }
        }
        
        private IEnumerator SwitchOff()
        {
            yield return _switchOffWaitForSeconds;
            
            spotLight.SetActive(false);
            coneMesh.SetActive(false);
            audioSource.PlayOneShot(switchOffSound);
                
            foreach (var objectToAppear in objectsToAppear)
            {
                if (objectToAppear.CompareTag("Draw"))
                {
                    objectToAppear.GetComponent<Draw>().Disable();
                }
                else
                {
                    objectToAppear.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
            
            _switchOffCoroutine = null;
        }
    }
}
