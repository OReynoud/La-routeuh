using System.Collections.Generic;
using DG.Tweening;
using Player;
using UnityEngine;

namespace Utilities
{
    public class Shadow : MonoBehaviour
    {
        [Tooltip("Point where the player will respawn if they are killed by the shadow")] [SerializeField] internal Transform respawnPoint;
        
        [Tooltip("Mesh game object of the shadow")] [SerializeField] internal GameObject meshGameObject;
        private readonly List<(Transform transform, Vector3 position)> _meshTransforms = new();
        
        private Sequence _movingSequence;
        [Tooltip("Duration of the animation of the moving shadow")] [SerializeField] internal float movingDuration;
        [Tooltip("Distance traveled by the moving shadow")] [SerializeField] internal float movingDistance;
        [Tooltip("Scale reached by the moving shadow")] [SerializeField] internal float movingScale;

        private void Start()
        {
            foreach (Transform child in meshGameObject.transform)
            {
                _meshTransforms.Add((child, child.position));
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && !PlayerController.instance.isProtected)
            {
                other.gameObject.transform.position = respawnPoint.position;
            }
        }
        
        internal void MoveShadow(float angle, Vector3 hitPoint, Vector3 lightPosition)
        {
            _movingSequence = DOTween.Sequence();
            foreach (var meshTransform in _meshTransforms)
            {
                /*if (expr)
                {
                    _movingSequence.Insert(0f, meshTransform.transform.DOMoveX(meshTransform.position.x + movingDistance, movingDuration))
                        .Join(meshTransform.transform.DOScaleX(movingScale, movingDuration * 0.5f))
                        .Insert(movingDuration * 0.5f, meshTransform.transform.DOScaleX(0f, movingDuration * 0.5f));
                }*/
            }
            _movingSequence.AppendCallback(() => _movingSequence.Kill());
        }
        
        /*private bool HasToMove(float angle)
        {
            if ()
            {
                return true;
            }
        }*/
    }
}
