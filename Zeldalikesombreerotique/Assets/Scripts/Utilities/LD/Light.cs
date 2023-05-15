using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Player;
using UnityEngine;

namespace Utilities
{
    public class Light : MonoBehaviour
    {
        [Tooltip("Raycast number (more raycasts = more precision = less FPS)")] [SerializeField] private int rayAmount;
        [Tooltip("Distance")] [SerializeField] internal float distance;
        [Range(1f, 177f)] [Tooltip("Angle")] [SerializeField] private float angle;
        [Tooltip("Angle (margin) not detected by raycasts")] [SerializeField] private float angleTolerance;
        internal float PhysicAngle;
        private float _halfAngle;
        private float _angleInterval;
        [Tooltip("Color (type) of the light")] [SerializeField] internal LightColorType lightColorType;
        [SerializeField] private UnityEngine.Light _lightComponent;
        [Tooltip("Point where the player will respawn if they are killed by the light")] [SerializeField] internal Transform respawnPoint;
        private Vector3 _direction;
        [Tooltip("Does the light need a battery to be switched on?")] [SerializeField] private bool doesNeedABattery;
        [Tooltip("Battery area detection object")] [ShowIf("doesNeedABattery")] [SerializeField] private GameObject batteryDetectionObject;
        [Tooltip("Radius of the detection area for the battery")] [ShowIf("doesNeedABattery")] [SerializeField] private float batteryDetectionRadius;
        [Tooltip("Mesh of the light")] [SerializeField] private GameObject meshObject;
        [SerializeField] private bool isBlinking;

        [ShowIf("isBlinking")] [SerializeField]
        private float blinkInterval;

        public new Light light;
        public MeshRenderer lightMeshRenderer;

        private Vector3[] _rayOutPosition;

        public  MeshFilter meshFilter;
        private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");
        
        private LightedElementsManager _lightedElementsManager;
        
        private bool _stopMesh;

        private void Awake()
        {
            // Light component initialization
            _lightComponent = GetComponent<UnityEngine.Light>();


            // Global values
            PhysicAngle = angle - angleTolerance; // Angle without the tolerance
            _halfAngle = PhysicAngle * 0.5f; // Half angle calculation
            _angleInterval = PhysicAngle / rayAmount; // Angle difference between each raycast
            
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
            _lightedElementsManager = LightedElementsManager.Instance;
            
            // Light component initialization
            _lightComponent.color = lightColorType.color;
            lightMeshRenderer.material.color = new Color(lightColorType.color.r, lightColorType.color.g, lightColorType.color.b, 0.5f);
            lightMeshRenderer.material.SetColor(EmissionColor, lightColorType.color);
            _lightComponent.range = distance;
            _lightComponent.spotAngle = angle;
            _lightComponent.innerSpotAngle = angle;
            if (isBlinking)
            {
                StartCoroutine(Blink1());
            }
        }

        private IEnumerator Blink1()
        {
            yield return new WaitForSeconds(blinkInterval);
            StartCoroutine(Blink1());
            light.enabled = !light.enabled;
            enabled = !enabled;
            lightMeshRenderer.enabled = !lightMeshRenderer.enabled;
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

            _rayOutPosition = new Vector3[rayAmount + 1];
            
            // Raycast loop
            for (var i = 0; i < rayAmount + 1; i++)
            {
                _stopMesh = false;
                
                // Raycast values
                var currentAngle = transform1.rotation.eulerAngles.y + _halfAngle - _angleInterval * i; // Current angle
                var rot = Quaternion.AngleAxis(currentAngle, Vector3.up); // Quaternion for direction calculation
                var dir = rot * Vector3.forward; // Direction of the raycast
                dir.Normalize();

                ThrowRaycast(origin, dir, distance, i);
            }

            CreateMesh();

            _lightedElementsManager.CurrentCheckCoroutine ??= StartCoroutine(_lightedElementsManager.CheckDictionariesCoroutine());
        }

        private void ThrowRaycast(Vector3 origin, Vector3 dir, float dist, int i, bool onlyShadows = false)
        {
            // Raycast
            if (Physics.Raycast(origin, dir, out var raycastHit, dist)) // If the raycast hits something
            {
                Debug.DrawRay(origin, dir * raycastHit.distance, Color.red);

                if (!_stopMesh)
                {
                    _rayOutPosition[i] = raycastHit.point;
                }

                var hitObject = raycastHit.collider.gameObject;

                switch (hitObject.tag)
                {
                    case "Player": // If the raycast hits the player
                        if (!onlyShadows)
                        {
                            if (lightColorType.canKillPlayer) // If the light can kill the player
                            {
                                PlayerController.instance.transform.position = respawnPoint.position; // Respawn the player
                                PlayerController.instance.joint.connectedBody = null;
                                PlayerController.instance.joint.gameObject.SetActive(false);
                                PlayerController.instance.isGrabbing = false;
                            }
                        
                            _stopMesh = true;

                            var newRayOriginPlayer = raycastHit.point + dir * 0.01f;
                            var newRayDistancePlayer = dist - raycastHit.distance - 0.01f;
                            ThrowRaycast(newRayOriginPlayer, dir, newRayDistancePlayer, i, true);
                        }
                        
                        break;
                    
                    case "Shadows": // If the raycast hits a shadow
                        if (lightColorType.canKillShadows) // If the light can kill shadows
                        {
                            var shadow = hitObject.GetComponent<Shadow>();
                            
                            if (_lightedElementsManager.AffectedShadows.ContainsKey(shadow))
                            {
                                if (_lightedElementsManager.AffectedShadows[shadow].ContainsKey(this))
                                {
                                    _lightedElementsManager.AffectedShadows[shadow][this] = true;
                                }
                                else
                                {
                                    _lightedElementsManager.AffectedShadows[shadow].Add(this, true);
                                }
                            }
                            else
                            {
                                _lightedElementsManager.AffectedShadows.Add(shadow, new Dictionary<Light, bool> {{this, true}});
                            }
                        }

                        var newRayOrigin = raycastHit.point + dir * 0.01f;
                        var newRayDistance = dist - raycastHit.distance - 0.01f;
                        ThrowRaycast(newRayOrigin, dir, newRayDistance, i, onlyShadows);
                        break;

                    case "Objects": // If the raycast hits an object
                        if (!onlyShadows)
                        {
                            var dynamicObject = hitObject.GetComponent<DynamicObject>();

                            if (lightColorType.canRevealObjects &&
                                dynamicObject.visibilityType ==
                                DynamicObject.VisibilityType
                                    .CanBeRevealed) // If the light can reveal objects and the object can be revealed
                            {
                                if (_lightedElementsManager.RevealedObjects.ContainsKey(hitObject))
                                {
                                    _lightedElementsManager.RevealedObjects[hitObject] = true;
                                }
                                else
                                {
                                    dynamicObject.meshObjectForVisibility.SetActive(true); // Reveal the object
                                    _lightedElementsManager.RevealedObjects.Add(hitObject, true);
                                }
                            }
                            else if (lightColorType.canHideObjects &&
                                     dynamicObject.visibilityType ==
                                     DynamicObject.VisibilityType
                                         .CanBeHidden) // If the light can hide objects and the object can be hidden
                            {
                                if (_lightedElementsManager.HiddenObjects.ContainsKey(hitObject))
                                {
                                    _lightedElementsManager.HiddenObjects[hitObject] = true;
                                }
                                else
                                {
                                    dynamicObject.meshObjectForVisibility.SetActive(false); // Hide the object
                                    _lightedElementsManager.HiddenObjects.Add(hitObject, true);
                                }
                            }
                            else if (dynamicObject.visibilityType == DynamicObject.VisibilityType.DelayedReappear)
                            {
                                dynamicObject.mesh.material.color = Color.clear;
                            }
                        }
                        break;

                    case "Footprint": // If the raycast hits a footprint
                    case "Draw": // If the raycast hits a draw
                        if (!onlyShadows)
                        {
                            if (lightColorType.canRevealObjects)
                            {
                                if (_lightedElementsManager.RevealedObjects.ContainsKey(hitObject))
                                {
                                    _lightedElementsManager.RevealedObjects[hitObject] = true;
                                }
                                else
                                {
                                    hitObject.GetComponent<DynamicObject>().meshObjectForVisibility
                                        .SetActive(true); // Reveal the object
                                    _lightedElementsManager.RevealedObjects.Add(hitObject, true);
                                }
                            }

                            var newOrigin = raycastHit.point + dir * 0.01f;
                            var newDistance = dist - raycastHit.distance - 0.01f;
                            ThrowRaycast(newOrigin, dir, newDistance, i);
                        }
                        break;
                }
            }

            else // If the raycast doesn't hit anything
            {
                Debug.DrawRay(origin, dir * dist, Color.green);
                if (!_stopMesh)
                {
                    _rayOutPosition[i] = transform.position + dir * distance;
                }
            }
        }

        private void CreateMesh()
        {
            var mesh = meshFilter.mesh;
            mesh.Clear();
            mesh.vertices = null;
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

            if (mesh == null)
            {
                meshFilter.mesh = new Mesh { vertices = verts, triangles = tris };
                return;
            }
            
            mesh.vertices = verts;
            mesh.triangles = tris;
        }
    }
}
