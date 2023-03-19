using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using Player;
using UnityEngine;

namespace Utilities
{
    public class Light : MonoBehaviour
    {
        [Tooltip("Raycast number (more raycasts = more precision = less FPS)")] [SerializeField] private int rayAmount;
        [Tooltip("Distance")] [SerializeField] private float distance;
        [Range(1f, 177f)] [Tooltip("Angle")] [SerializeField] private float angle;
        [Tooltip("Angle (margin) not detected by raycasts")] [SerializeField] private float angleTolerance;
        private float _physicAngle;
        private float _halfAngle;
        private float _angleInterval;
        [Tooltip("Color (type) of the light")] [SerializeField] internal LightColorType lightColorType;
        private UnityEngine.Light _lightComponent;
        private readonly Dictionary<GameObject, bool> hiddenObjects = new();
        private readonly Dictionary<GameObject, bool> revealedObjects = new();
        [Tooltip("Point where the player will respawn if they are killed by the light")] [SerializeField] internal Transform respawnPoint;
        private Vector3 direction;
        private readonly Dictionary<GameObject, bool> activeMirrors = new();
        [Tooltip("Does the light need a battery to be switched on?")] [SerializeField] private bool doesNeedABattery;
        [Tooltip("Battery area detection object")] [ShowIf("doesNeedABattery")] [SerializeField] private GameObject batteryDetectionObject;
        [Tooltip("Radius of the detection area for the battery")] [ShowIf("doesNeedABattery")] [SerializeField] private float batteryDetectionRadius;
        [Tooltip("Mesh of the light")] [SerializeField] private GameObject meshObject;
        private readonly Dictionary<GameObject, bool> lightedObjects = new();

        private void OnEnable()
        {
            // Light component initialization
            _lightComponent = GetComponent<UnityEngine.Light>();
            _lightComponent.color = lightColorType.color;
            _lightComponent.range = distance;
            _lightComponent.spotAngle = angle;
            _lightComponent.innerSpotAngle = angle;

            // Global values
            _physicAngle = angle - angleTolerance; // Angle without the tolerance
            _halfAngle = _physicAngle * 0.5f; // Half angle calculation
            _angleInterval = _physicAngle / rayAmount; // Angle difference between each raycast
            
            // Battery detection
            if (doesNeedABattery)
            {
                batteryDetectionObject.SetActive(true);
                batteryDetectionObject.GetComponent<CapsuleCollider>().radius = batteryDetectionRadius;
                var material = meshObject.GetComponent<MeshRenderer>().material;
                var materialColor = material.color;
                material.color = new Color(materialColor.r, materialColor.g, materialColor.b, 0.4f);
                doesNeedABattery = false;
                gameObject.SetActive(false);
            }
        }

        private void FixedUpdate()
        {
            ThrowRaycasts();
        }

        private void ThrowRaycasts()
        {

            // Optimization to avoid calling transform multiple times
            var transform1 = transform;
            var position = transform1.position;

            transform1.rotation = direction == Vector3.zero ? transform1.rotation : Quaternion.LookRotation(direction);
            
            // Raycast global values
            var origin = new Vector3(position.x, position.y, position.z); // Origin point
            var mirrorsToReflectRays = new Dictionary<GameObject, List<Vector3>>();

            // Raycast loop
            for (var i = 0; i < rayAmount + 1; i++)
            {
                // Raycast values
                var currentAngle = transform1.rotation.eulerAngles.y + _halfAngle - _angleInterval * i; // Current angle
                var rot = Quaternion.AngleAxis(currentAngle, Vector3.up); // Quaternion for direction calculation
                var dir = rot * Vector3.forward; // Direction of the raycast
                dir.Normalize();

                // Raycast
                if (Physics.Raycast(origin, dir, out var raycastHit, distance)) // If the raycast hits something
                {
                    Debug.DrawRay(origin, dir * raycastHit.distance, Color.red);

                    var hitObject = raycastHit.collider.gameObject;

                    if (hitObject.CompareTag("Mirror")) // If the raycast hits a mirror
                    {
                        if (!mirrorsToReflectRays.ContainsKey(hitObject))
                        {
                            mirrorsToReflectRays.Add(hitObject, new List<Vector3>());
                        }
                        mirrorsToReflectRays[hitObject].Add(Vector3.Reflect(dir, raycastHit.normal));
                    }
                    
                    else if (hitObject.CompareTag("Player")) // If the raycast hits the player
                    {
                        if (lightColorType.canKillPlayer) // If the light can kill the player
                        {
                            PlayerController.instance.transform.position = respawnPoint.position; // Respawn the player
                            PlayerController.instance.joint.connectedBody = null;
                            PlayerController.instance.joint.gameObject.SetActive(false);
                            PlayerController.instance.isGrabbing = false;
                        }
                    }
                    
                    else if (hitObject.CompareTag("Shadows")) // If the raycast hits a shadow
                    {
                        if (lightColorType.canKillShadows) // If the light can kill shadows
                        {
                            hitObject.SetActive(false); // Kill the shadow
                        }
                    }
                    
                    else if (hitObject.CompareTag("Objects")) // If the raycast hits an object
                    {
                        var dynamicObject = hitObject.GetComponent<DynamicObject>();

                        if (lightColorType.canRevealObjects && dynamicObject.visibilityType == DynamicObject.VisibilityType.CanBeRevealed) // If the light can reveal objects and the object can be revealed
                        {
                            if (revealedObjects.ContainsKey(hitObject))
                            {
                                revealedObjects[hitObject] = true;
                            }
                            else
                            {
                                dynamicObject.meshObjectForVisibility.SetActive(true); // Reveal the object
                                revealedObjects.Add(hitObject, true);
                            }
                        }
                        else if (lightColorType.canHideObjects && dynamicObject.visibilityType == DynamicObject.VisibilityType.CanBeHidden) // If the light can hide objects and the object can be hidden
                        {
                            if (hiddenObjects.ContainsKey(hitObject))
                            {
                                hiddenObjects[hitObject] = true;
                            }
                            else
                            {
                                dynamicObject.meshObjectForVisibility.SetActive(false); // Hide the object
                                hiddenObjects.Add(hitObject, true);
                            }
                        }
                    }
                    
                    else if (hitObject.CompareTag("ElementToLight")) // If the raycast hits an element to light
                    {
                        if (lightedObjects.ContainsKey(hitObject))
                        {
                            lightedObjects[hitObject] = true;
                        }
                        else
                        {
                            var material = hitObject.GetComponentInChildren<MeshRenderer>().material;
                            var materialColor = material.color;
                            material.color = new Color(materialColor.r, materialColor.g, materialColor.b, 1f); // Light the element
                            hitObject.GetComponent<ElementToLight>().isLighted = true;
                            lightedObjects.Add(hitObject, true);
                        }
                    }
                }
                
                else // If the raycast doesn't hit anything
                {
                    Debug.DrawRay(origin, dir * distance, Color.green);
                }
            }

            foreach (var mirrorToReflectRays in mirrorsToReflectRays.Keys.ToList())
            {
                // Average of the reflected rays
                var average = Vector3.zero;
                foreach (var rayToReflect in mirrorsToReflectRays[mirrorToReflectRays])
                {
                    average += rayToReflect;
                }
                average /= mirrorsToReflectRays[mirrorToReflectRays].Count;
                
                // Mirror values affectation
                var mirrorLightGameObject = mirrorToReflectRays.transform.GetChild(0).gameObject;
                var mirrorLightComponent = mirrorLightGameObject.GetComponent<Light>();
                
                if (!activeMirrors.ContainsKey(mirrorToReflectRays))
                {
                    mirrorLightComponent.lightColorType = lightColorType;
                    mirrorLightComponent.respawnPoint = respawnPoint;
                    mirrorLightGameObject.transform.rotation = Quaternion.LookRotation(average);
                    mirrorLightGameObject.SetActive(true);
                    activeMirrors.Add(mirrorToReflectRays, true);
                }
                else
                {
                    activeMirrors[mirrorToReflectRays] = true;
                }
                
                mirrorLightComponent.direction = average;
            }
            
            foreach (var activeMirror in activeMirrors.Keys.ToList())
            {
                if (!activeMirrors[activeMirror])
                {
                    activeMirror.transform.GetChild(0).gameObject.SetActive(false);
                    activeMirrors.Remove(activeMirror);
                }
                else
                {
                    activeMirrors[activeMirror] = false;
                }
            }

            foreach (var revealedObject in revealedObjects.Keys.ToList())
            {
                if (!revealedObjects[revealedObject])
                {
                    revealedObject.GetComponent<DynamicObject>().meshObjectForVisibility.SetActive(false);
                    revealedObjects.Remove(revealedObject);
                }
                else
                {
                    revealedObjects[revealedObject] = false;
                }
            }

            foreach (var hiddenObject in hiddenObjects.Keys.ToList())
            {
                if (!hiddenObjects[hiddenObject])
                {
                    hiddenObject.GetComponent<DynamicObject>().meshObjectForVisibility.SetActive(true);
                    hiddenObjects.Remove(hiddenObject);
                }
                else
                {
                    hiddenObjects[hiddenObject] = false;
                }
            }

            foreach (var lightedObject in lightedObjects.Keys.ToList())
            {
                if (!lightedObjects[lightedObject])
                {
                    var material = lightedObject.GetComponentInChildren<MeshRenderer>().material;
                    var materialColor = material.color;
                    material.color = new Color(materialColor.r, materialColor.g, materialColor.b, 0.1f);
                    lightedObject.GetComponent<ElementToLight>().isLighted = false;
                    lightedObjects.Remove(lightedObject);
                }
                else
                {
                    lightedObjects[lightedObject] = false;
                }
            }
        }
    }
}
