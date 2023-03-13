using System.Collections.Generic;
using System.Linq;
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
        [Tooltip("Color (type) of the light")] [SerializeField] private LightColorType lightColorType;
        private UnityEngine.Light _lightComponent;
        private readonly Dictionary<GameObject, bool> _hiddenObjects = new();
        private readonly Dictionary<GameObject, bool> _revealedObjects = new();
        [SerializeField] private Transform respawnPoint;

        private void Awake()
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
        }

        private void FixedUpdate()
        {
            // Optimization to avoid calling transform multiple times
            var transform1 = transform;
            var position = transform1.position;
                
            // Raycast global values
            var origin = new Vector3(position.x, position.y, position.z); // Origin point
            
            // Raycast loop
            for (var i = 0; i < rayAmount + 1; i++)
            {
                // Raycast values
                var currentAngle = transform1.rotation.eulerAngles.y + _halfAngle - _angleInterval * i; // Current angle
                var rot = Quaternion.AngleAxis(currentAngle,Vector3.up); // Quaternion for direction calculation
                var dir = rot * Vector3.forward; // Direction of the raycast
                
                // Raycast
                if (Physics.Raycast(origin,dir, out var raycastHit, distance)) // If the raycast hits something
                {
                    Debug.DrawRay(origin,dir*raycastHit.distance, Color.red);
                    
                    var hitObject = raycastHit.collider.gameObject;
                    
                    if (hitObject.CompareTag("Player")) // If the raycast hits the player
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
                            if (_revealedObjects.ContainsKey(hitObject))
                            {
                                _revealedObjects[hitObject] = true;
                            }
                            else
                            {
                                dynamicObject.meshObjectForVisibility.SetActive(true); // Reveal the object
                                _revealedObjects.Add(hitObject, true);
                            }
                        }
                        else if (lightColorType.canHideObjects && dynamicObject.visibilityType == DynamicObject.VisibilityType.CanBeHidden) // If the light can hide objects and the object can be hidden
                        {
                            if (_hiddenObjects.ContainsKey(hitObject))
                            {
                                _hiddenObjects[hitObject] = true;
                            }
                            else
                            {
                                dynamicObject.meshObjectForVisibility.SetActive(false); // Hide the object
                                _hiddenObjects.Add(hitObject, true);
                            }
                        }
                    }
                }
                else // If the raycast doesn't hit anything
                {
                    Debug.DrawRay(origin,dir*distance, Color.green);
                }
            }

            foreach (var revealedObject in _revealedObjects.Keys.ToList())
            {
                if (!_revealedObjects[revealedObject])
                {
                    revealedObject.GetComponent<DynamicObject>().meshObjectForVisibility.SetActive(false);
                    _revealedObjects.Remove(revealedObject);
                }
                else
                {
                    _revealedObjects[revealedObject] = false;
                }
            }
            
            foreach (var hiddenObject in _hiddenObjects.Keys.ToList())
            {
                if (!_hiddenObjects[hiddenObject])
                {
                    hiddenObject.GetComponent<DynamicObject>().meshObjectForVisibility.SetActive(true);
                    _hiddenObjects.Remove(hiddenObject);
                }
                else
                {
                    _hiddenObjects[hiddenObject] = false;
                }
            }
        }
    }
}
