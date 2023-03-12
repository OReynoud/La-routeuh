using UnityEngine;

namespace Utilities
{
    public class Light : MonoBehaviour
    {
        [Tooltip("Raycast number (more raycasts = more precision = less FPS)")] [SerializeField] private int rayAmount;
        [Tooltip("Distance")] [SerializeField] private float distance;
        [SerializeField] private float height;
        [Range(1f, 177f)] [Tooltip("Angle")] [SerializeField] private float angle;
        [Tooltip("Angle (margin) not detected by raycasts")] [SerializeField] private float angleTolerance;
        private float _physicAngle;
        private float _halfAngle;
        private float _angleInterval;
        [Tooltip("Color (type) of the light")] [SerializeField] private LightColorType lightColorType;
        private UnityEngine.Light _lightComponent;
        
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
            var origin = new Vector3(position.x, position.y + height, position.z); // Origin point
            
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
                }
                else // If the raycast doesn't hit anything
                {
                    Debug.DrawRay(origin,dir*distance, Color.green);
                }
            }
        }
    }
}
