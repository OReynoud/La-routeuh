using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities
{
    public class LightedElementsManager : MonoBehaviour
    {
        internal static LightedElementsManager Instance;
        
        internal readonly Dictionary<GameObject, bool> HiddenObjects = new();
        internal readonly Dictionary<GameObject, bool> RevealedObjects = new();
        
        internal Coroutine CurrentCheckCoroutine;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /*private void FixedUpdate()
        {
            var numberOfLights = Physics.OverlapSphere(PlayerController.instance.transform.position, 2)
                .Select(x => x.gameObject).Where(obj => !obj.TryGetComponent<Light>(out _))
                .Select(obj => obj.GetComponent<Light>()).Count();
        }*/

        internal IEnumerator CheckDictionariesCoroutine()
        {
            yield return new WaitForEndOfFrame();
            CheckDictionaries();
            CurrentCheckCoroutine = null;
        }

        private void CheckDictionaries()
        {
            foreach (var revealedObject in RevealedObjects.Keys.ToList())
            {
                if (!RevealedObjects[revealedObject])
                {
                    revealedObject.GetComponent<DynamicObject>().meshObjectForVisibility.SetActive(false);
                    RevealedObjects.Remove(revealedObject);
                }
                else
                {
                    RevealedObjects[revealedObject] = false;
                }
            }

            foreach (var hiddenObject in HiddenObjects.Keys.ToList())
            {
                if (!HiddenObjects[hiddenObject])
                {
                    hiddenObject.GetComponent<DynamicObject>().meshObjectForVisibility.SetActive(true);
                    HiddenObjects.Remove(hiddenObject);
                }
                else
                {
                    HiddenObjects[hiddenObject] = false;
                }
            }
        }
    }
}
