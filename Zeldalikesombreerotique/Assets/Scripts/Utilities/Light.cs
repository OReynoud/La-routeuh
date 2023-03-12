using UnityEngine;

namespace Utilities
{
    public class Light : MonoBehaviour
    {
        [Tooltip("Raycast number (more raycasts = more precision)")] [SerializeField] private int rayAmount;
        [Tooltip("Distance")] [SerializeField] private float distance;
        [SerializeField] private float height;
        [Tooltip("Angle")] [SerializeField] private float angle;
        private float _halfAngle;
        [Tooltip("Color (type) of the light")] [SerializeField] private LightColorType lightColorType;
        
        private void Awake()
        {
            // Half angle calculation
            _halfAngle = angle * 0.5f;
        }

        private void FixedUpdate()
        {
            // Optimization to avoid calling transform multiple times
            var transform1 = transform;
            var position = transform1.position;
                
            // Raycast global values
            var origin = new Vector3(position.x, position.y + height, position.z); // Origin point
            var diff = angle / rayAmount; // Angle difference between each raycast
            
            // Raycast loop
            for (var i = 0; i < rayAmount; i++)
            {
                // Raycast values
                var currentAngle = transform1.rotation.eulerAngles.y + _halfAngle - diff * i; // Current angle
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
