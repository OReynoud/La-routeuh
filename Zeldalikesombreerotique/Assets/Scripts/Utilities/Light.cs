using System.Collections;
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
        private readonly Dictionary<GameObject, bool> _hiddenObjects = new();
        private readonly Dictionary<GameObject, bool> _revealedObjects = new();
        [Tooltip("Point where the player will respawn if they are killed by the light")] [SerializeField] internal Transform respawnPoint;
        private Vector3 _direction;
        private readonly Dictionary<GameObject, bool> _activeMirrors = new();
        [Tooltip("Does the light need a battery to be switched on?")] [SerializeField] private bool doesNeedABattery;
        [Tooltip("Battery area detection object")] [ShowIf("doesNeedABattery")] [SerializeField] private GameObject batteryDetectionObject;
        [Tooltip("Radius of the detection area for the battery")] [ShowIf("doesNeedABattery")] [SerializeField] private float batteryDetectionRadius;
        [Tooltip("Mesh of the light")] [SerializeField] private GameObject meshObject;
        private readonly Dictionary<GameObject, bool> _lightedObjects = new();
        private readonly Dictionary<GameObject, bool> _lightedHoles = new();
        [SerializeField] private bool isBlinking;

        [ShowIf("isBlinking")] [SerializeField]
        private float blinkInterval;

        public Light light;
        public MeshRenderer lightMeshRenderer;

        private Vector3[] _rayOutPosition;
        
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
            
            // Battery detection
            if (doesNeedABattery)
            {
                batteryDetectionObject.SetActive(true);
                if (batteryDetectionObject.TryGetComponent(out CapsuleCollider batteryDetectionCollider))
                {
                    batteryDetectionCollider.radius = batteryDetectionRadius;
                }
                var material = meshObject.GetComponent<MeshRenderer>().material;
                var materialColor = material.color;
                material.color = new Color(materialColor.r, materialColor.g, materialColor.b, 0.4f);
                doesNeedABattery = false;
                gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            StartCoroutine(Blink1());
        }

        private void OnDisable()
        {
            CheckDictionaries();
        }

        private IEnumerator Blink1()
        {
            if(!isBlinking)
            {
                yield return new WaitForSeconds(blinkInterval);
                StartCoroutine(Blink1());
            }
            else
            {
                yield return new WaitForSeconds(blinkInterval);
                StartCoroutine(Blink1());
                light.enabled = !light.enabled;
                enabled = !enabled;
                lightMeshRenderer.enabled = !lightMeshRenderer.enabled;
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

            transform1.rotation = _direction == Vector3.zero ? transform1.rotation : Quaternion.LookRotation(_direction);
            
            // Raycast global values
            var origin = new Vector3(position.x, position.y, position.z); // Origin point
            var mirrorsToReflectRays = new Dictionary<GameObject, List<Vector3>>();

            _rayOutPosition = new Vector3[rayAmount + 1];
            
            // Raycast loop
            for (var i = 0; i < rayAmount + 1; i++)
            {
                // Raycast values
                var currentAngle = transform1.rotation.eulerAngles.y + _halfAngle - _angleInterval * i; // Current angle
                var rot = Quaternion.AngleAxis(currentAngle, Vector3.up); // Quaternion for direction calculation
                var dir = rot * Vector3.forward; // Direction of the raycast
                dir.Normalize();

                ThrowRaycast(origin, dir, distance, i, mirrorsToReflectRays);
            }

            CreateMesh();

            CheckDictionaries(mirrorsToReflectRays);
        }

        private void ThrowRaycast(Vector3 origin, Vector3 dir, float dist, int i, IDictionary<GameObject, List<Vector3>> mirrorsToReflectRays)
        {
            // Raycast
            if (Physics.Raycast(origin, dir, out var raycastHit, dist)) // If the raycast hits something
            {
                Debug.DrawRay(origin, dir * raycastHit.distance, Color.red);

                _rayOutPosition[i] = raycastHit.point;

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
                    else if (dynamicObject.visibilityType == DynamicObject.VisibilityType.DelayedReappear)
                    {
                        dynamicObject.mesh.material.color = Color.clear;
                    }
                }

                else if (hitObject.CompareTag("ElementToLight")) // If the raycast hits an element to light
                {
                    if (_lightedObjects.ContainsKey(hitObject))
                    {
                        _lightedObjects[hitObject] = true;
                    }
                    else
                    {
                        var material = hitObject.GetComponentInChildren<MeshRenderer>().material;
                        var materialColor = material.color;
                        material.color = new Color(materialColor.r, materialColor.g, materialColor.b, 1f); // Light the element
                        hitObject.GetComponent<ElementToLight>().isLighted = true;
                        _lightedObjects.Add(hitObject, true);
                    }
                }

                else if (hitObject.CompareTag("HoleWithObject")) // If the raycast hits a hole
                {
                    if (_lightedHoles.ContainsKey(hitObject))
                    {
                        _lightedHoles[hitObject] = true;
                    }
                    else
                    {
                        hitObject.GetComponent<HoleWithObject>().ShowObject(); // Disable the collider
                        _lightedHoles.Add(hitObject, true);
                    }
                }

                else if (hitObject.CompareTag("Footprint")) // If the raycast hits a footprint
                {
                    if (_revealedObjects.ContainsKey(hitObject))
                    {
                        _revealedObjects[hitObject] = true;
                    }
                    else
                    {
                        hitObject.GetComponent<DynamicObject>().meshObjectForVisibility.SetActive(true); // Reveal the object
                        _revealedObjects.Add(hitObject, true);
                    }
                    
                    var footprintCollider = hitObject.GetComponent<CapsuleCollider>();
                    var radius = footprintCollider.radius;
                    var newOrigin = raycastHit.point + dir * (radius * 2f);
                    var newDistance = dist - raycastHit.distance - radius * 2f;
                    ThrowRaycast(newOrigin, dir,newDistance, i, mirrorsToReflectRays);
                }
            }

            else // If the raycast doesn't hit anything
            {
                Debug.DrawRay(origin, dir * dist, Color.green);
                _rayOutPosition[i] = transform.position + dir * distance;
            }
        }

        private void CheckDictionaries(Dictionary<GameObject, List<Vector3>> mirrorsToReflectRays = null)
        {
            if (mirrorsToReflectRays != null)
            {
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

                    if (!_activeMirrors.ContainsKey(mirrorToReflectRays))
                    {
                        mirrorLightComponent.lightColorType = lightColorType;
                        mirrorLightComponent.respawnPoint = respawnPoint;
                        mirrorLightGameObject.transform.rotation = Quaternion.LookRotation(average);
                        mirrorLightGameObject.SetActive(true);
                        _activeMirrors.Add(mirrorToReflectRays, true);
                    }
                    else
                    {
                        _activeMirrors[mirrorToReflectRays] = true;
                    }

                    mirrorLightComponent._direction = average;
                }
            }

            foreach (var activeMirror in _activeMirrors.Keys.ToList())
            {
                if (!_activeMirrors[activeMirror])
                {
                    activeMirror.transform.GetChild(0).gameObject.SetActive(false);
                    _activeMirrors.Remove(activeMirror);
                }
                else
                {
                    _activeMirrors[activeMirror] = false;
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

            foreach (var lightedObject in _lightedObjects.Keys.ToList())
            {
                if (!_lightedObjects[lightedObject])
                {
                    var material = lightedObject.GetComponentInChildren<MeshRenderer>().material;
                    var materialColor = material.color;
                    material.color = new Color(materialColor.r, materialColor.g, materialColor.b, 0.1f);
                    lightedObject.GetComponent<ElementToLight>().isLighted = false;
                    _lightedObjects.Remove(lightedObject);
                }
                else
                {
                    _lightedObjects[lightedObject] = false;
                }
            }

            foreach (var lightedHole in _lightedHoles.Keys.ToList())
            {
                if (!_lightedHoles[lightedHole])
                {
                    lightedHole.GetComponent<HoleWithObject>().HideObject(); // Enable the collider
                    _lightedHoles.Remove(lightedHole);
                }
                else
                {
                    _lightedHoles[lightedHole] = false;
                }
            }
        }

        public  MeshFilter meshFilter;
        private void CreateMesh()
        {
            var verts = new Vector3[_rayOutPosition.Length * 3];
            var tris  = new int[_rayOutPosition.Length * 3];

            var index = 0;
            for (var i = 0; i < _rayOutPosition.Length-1; i++)
            {
                verts[index + 0] = transform.InverseTransformPoint(_rayOutPosition[i + 0]);
                verts[index + 1] = transform.InverseTransformPoint(_rayOutPosition[i + 1]);
                verts[index + 2] = Vector3.zero;
                        
                tris[index + 0] = index + 0;
                tris[index + 2] = index + 1;
                tris[index + 1] = index + 2;
                
                index +=3;
            }
            
            var mesh = new Mesh { vertices = verts, triangles = tris };

            meshFilter.mesh = mesh;
        }
    }
}
