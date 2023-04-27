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
        internal readonly Dictionary<Shadow, Dictionary<Light, bool>> AffectedShadows = new();
        
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

        internal IEnumerator CheckDictionariesCoroutine()
        {
            yield return new WaitForEndOfFrame();
            CheckDictionaries();
            CurrentCheckCoroutine = null;
        }

        private void CheckDictionaries()
        {
            CheckVisibilityDictionary(RevealedObjects, false);
            CheckVisibilityDictionary(HiddenObjects, true);

            foreach (var affectedShadow in AffectedShadows.Keys.ToList())
            {
                foreach (var affectingLight in AffectedShadows[affectedShadow].Keys.ToList())
                {
                    if (!AffectedShadows[affectedShadow][affectingLight])
                    {
                        // affectedShadow.GetComponent<Shadow>().ResetShadow(affectedShadow, affectingLight);
                        AffectedShadows[affectedShadow].Remove(affectingLight);
                    }
                    else
                    {
                        AffectedShadows[affectedShadow][affectingLight] = false;
                    }
                }

                switch (AffectedShadows[affectedShadow].Count)
                {
                    case 0:
                        AffectedShadows.Remove(affectedShadow);
                        break;
                    
                    case 1:
                        var affectingLight = AffectedShadows[affectedShadow].Keys.ToList()[0];
                        
                        // Raycast values
                        var currentAngle = affectingLight.transform.rotation.eulerAngles.y; // Current angle
                        var rot = Quaternion.AngleAxis(currentAngle, Vector3.up); // Quaternion for direction calculation
                        var dir = rot * Vector3.forward; // Direction of the raycast
                        dir.Normalize();

                        var lightPosition = affectingLight.transform.position;
                        Physics.Raycast(lightPosition, dir, out var raycastHit, affectingLight.distance); // Raycast
                        
                        affectedShadow.MoveShadow(affectingLight.PhysicAngle, raycastHit.point, lightPosition);
                        break;
                    
                    default:
                        // affectedShadow.ResetShadow();
                        break;
                }
                
                if (AffectedShadows[affectedShadow].Count == 0)
                {
                    AffectedShadows.Remove(affectedShadow);
                }
            }
        }

        private static void CheckVisibilityDictionary(Dictionary<GameObject,bool> visibilityDictionary, bool defaultVisibility)
        {
            foreach (var visibilityObject in visibilityDictionary.Keys.ToList())
            {
                if (!visibilityDictionary[visibilityObject])
                {
                    visibilityObject.GetComponent<DynamicObject>().meshObjectForVisibility.SetActive(defaultVisibility);
                    visibilityDictionary.Remove(visibilityObject);
                }
                else
                {
                    visibilityDictionary[visibilityObject] = false;
                }
            }
        }
    }
}