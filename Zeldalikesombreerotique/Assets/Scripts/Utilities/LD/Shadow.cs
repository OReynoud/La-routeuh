using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Managers;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

namespace Utilities.LD
{
    public class Shadow : MonoBehaviour
    {
        [Tooltip("Audio source")] [SerializeField] private AudioSource audioSource;
        
        [Tooltip("Point where the player will respawn if they are killed by the shadow")] [SerializeField] private Transform respawnPoint;
        
        [Tooltip("Mesh game object of the shadow")] [SerializeField] internal GameObject meshGameObject;
        private readonly List<(Transform transform, Vector3 position, Vector3 localPosition, float localScaleX)> _meshTransforms = new();
        
        [Tooltip("Whisper sounds of the shadow")] [SerializeField] private List<AudioClip> shadowWhisperSounds;
        private AudioClip _lastWhisperSound;
        [Tooltip("Time before the end of the previous sound to play the next sound")] [SerializeField] private float timeBeforeNextSound;
        
        private Sequence _movingSequence;
        [Tooltip("Duration of the animation of the moving shadow")] [SerializeField] internal float movingDuration;
        [HideIf("hasAnchor")] [Tooltip("Distance traveled by the moving shadow")] [SerializeField] internal float movingDistance;
        [Tooltip("Scale reached by the moving shadow")] [SerializeField] internal float movingScale;
        [Tooltip("Ease followed by the moving shadow")] [SerializeField] internal Ease movingEase;
        
        [Tooltip("Does the shadow have an anchor where to move?")] [SerializeField] private bool hasAnchor;
        [ShowIf("hasAnchor")] [Tooltip("Anchor where to move")] [SerializeField] private Transform anchor;
        private float _anchorDirectionFactor;
        
        [Tooltip("Does the shadow have an anchor where to move?")] [SerializeField] internal bool isPuzzle4Shadow;
        [ShowIf("isPuzzle4Shadow")] [Tooltip("Value to divide to the distance between shadow's center and sphere's center at the end")] [SerializeField] private float divisionValue;
        [ShowIf("isPuzzle4Shadow")] [Tooltip("Point where the player will respawn if they are killed by the shadow")] [SerializeField] private Transform respawnPoint2;
        [ShowIf("isPuzzle4Shadow")] [Tooltip("Anchor where to move at the beginning")] [SerializeField] private Transform anchorBeginning;
        [Tooltip("Duration of the animation of the moving shadow at the beginning")] [SerializeField] internal float movingDurationBeginning;
        [Tooltip("Ease followed by the moving shadow at the beginning")] [SerializeField] internal Ease movingEaseBeginning;
        [ShowIf("isPuzzle4Shadow")] [Tooltip("List of the triggers to reset when the player dies")] [SerializeField] private List<GameObject> triggersToReset;
        [ShowIf("isPuzzle4Shadow")] [Tooltip("List of the audios to reset when the player dies")] [SerializeField] private List<AudioSource> audiosToReset;
        
        [Tooltip("Does the shadow have an anchor where to move?")] [SerializeField] internal bool isPuzzle3Shadow;

        private Vector3 _lastHitPoint;
        private Vector3 _lastLightPosition;

        private void Start()
        {
            foreach (Transform child in meshGameObject.transform)
            {
                var localPosition = child.localPosition;
                var localScaleX = child.localScale.x;
                
                _meshTransforms.Add((child, child.position, localPosition, localScaleX));

                var shadowKill = child.GetChild(0).GetComponent<ShadowKill>();
                shadowKill.RespawnPoint = respawnPoint;
                shadowKill.IsPuzzle4Shadow = isPuzzle4Shadow;
                shadowKill.TriggersToReset = triggersToReset;
                shadowKill.AudiosToReset = audiosToReset;
                shadowKill.Shadow = this;

                if (!isPuzzle4Shadow && !isPuzzle3Shadow)
                {
                    var newCapsuleCollider = gameObject.AddComponent<CapsuleCollider>();
                    newCapsuleCollider.center = localPosition;
                    newCapsuleCollider.radius = localScaleX * 0.5f;
                    newCapsuleCollider.height = 4f;
                    newCapsuleCollider.isTrigger = true;
                }
            }

            if (hasAnchor)
            {
                var anchorPosition = anchor.position;
                var position = meshGameObject.transform.position;
                
                movingDistance = Vector3.Distance(anchorPosition, position);
                _anchorDirectionFactor = anchorPosition.x - position.x > 0 ? 1 : -1;
            }

            StartCoroutine(ShadowWhisperSoundCoroutine());
        }

        internal void Initialize(Vector3 move)
        {
            for (var i = 0; i < _meshTransforms.Count; i++)
            {
                _meshTransforms[i] = (_meshTransforms[i].transform, _meshTransforms[i].position + move, _meshTransforms[i].localPosition, _meshTransforms[i].localScaleX);
            }
            
            if (hasAnchor)
            {
                var anchorPosition = anchor.position;
                var position = meshGameObject.transform.position;
                
                movingDistance = Vector3.Distance(anchorPosition, position);
                _anchorDirectionFactor = anchorPosition.x - position.x > 0 ? 1 : -1;
            }
        }
        
        internal void MoveShadow(float angle, Vector3 hitPoint, Vector3 lightPosition, float lightDistance)
        {
            if (_lastHitPoint != hitPoint || _lastLightPosition != lightPosition)
            {
                _lastHitPoint = hitPoint;
                _lastLightPosition = lightPosition;
                _movingSequence.Kill();
                _movingSequence = DOTween.Sequence();
                
                foreach (var meshTransform in _meshTransforms)
                {
                    var whereToMove = WhereToMove(angle, hitPoint, lightPosition, meshTransform.position, lightDistance);
                    
                    var zPositionFactor = hitPoint.z - meshTransform.position.z > 0 ? -1 : 1;
                
                    if (whereToMove.hasToMove)
                    {
                        _movingSequence.Insert(0f, meshTransform.transform.DOLocalMoveX(meshTransform.localPosition.x + movingDistance * (hasAnchor ? _anchorDirectionFactor : whereToMove.direction * zPositionFactor), movingDuration))
                            .Join(meshTransform.transform.DOScaleX(movingScale + meshTransform.localScaleX - 1f, movingDuration * 0.5f))
                            .Insert(movingDuration * 0.5f, meshTransform.transform.DOScaleX(meshTransform.localScaleX, movingDuration * 0.5f));
                    }
                    else
                    {
                        _movingSequence.Insert(0f, meshTransform.transform.DOLocalMoveX(meshTransform.localPosition.x, movingDuration))
                            .Join(meshTransform.transform.DOScaleX(movingScale + meshTransform.localScaleX - 1f, movingDuration * 0.5f))
                            .Insert(movingDuration * 0.5f, meshTransform.transform.DOScaleX(meshTransform.localScaleX, movingDuration * 0.5f));
                    }
                }
                
                _movingSequence.AppendCallback(() => _movingSequence.Kill());
            }
        }
        
        internal void MoveWholeShadow()
        {
            _movingSequence.Kill();
            _movingSequence = DOTween.Sequence();
            foreach (var meshTransform in _meshTransforms)
            {
                var shadowTransform = transform;
                var shadowPosition = shadowTransform.position;
                var whereToMove = WhereToMove(shadowPosition + shadowTransform.forward, shadowPosition, meshTransform.position);
            
                _movingSequence.Insert(0f, meshTransform.transform.DOLocalMoveX(meshTransform.localPosition.x + movingDistance * (hasAnchor ? _anchorDirectionFactor : whereToMove), movingDuration))
                    .Join(meshTransform.transform.DOScaleX(movingScale + meshTransform.localScaleX - 1f, movingDuration * 0.5f))
                    .Insert(movingDuration * 0.5f, meshTransform.transform.DOScaleX(meshTransform.localScaleX, movingDuration * 0.5f));
            }
            _movingSequence.AppendCallback(() => _movingSequence.Kill());
        }
        
        internal void ResetShadow()
        {
            _movingSequence.Kill();
            _movingSequence = DOTween.Sequence();
            foreach (var meshTransform in _meshTransforms)
            {
                _movingSequence.Insert(0f, meshTransform.transform.DOLocalMoveX(meshTransform.localPosition.x, movingDuration))
                    .Join(meshTransform.transform.DOScaleX(movingScale, movingDuration * 0.5f))
                    .Insert(movingDuration * 0.5f, meshTransform.transform.DOScaleX(1f, movingDuration * 0.5f));
            }
            _movingSequence.AppendCallback(() => _movingSequence.Kill());
        }
        
        private static (bool hasToMove, int direction) WhereToMove(float angle, Vector3 hitPoint, Vector3 lightPosition, Vector3 meshPosition, float lightDistance)
        {
            var baseVector3 = hitPoint - lightPosition;
            var meshVector3 = meshPosition - lightPosition;
            var angleBetweenVectors = Vector3.Angle(new Vector3(baseVector3.x, 0f, baseVector3.z), new Vector3(meshVector3.x, 0f, meshVector3.z));

            if (angleBetweenVectors < angle * 0.5f && meshVector3.magnitude <= lightDistance)
            {
                var angleDifference = Quaternion.LookRotation(baseVector3).eulerAngles.y - Quaternion.LookRotation(meshVector3).eulerAngles.y;

                if (angleDifference > 180f)
                {
                    angleDifference -= 360f;
                }
                else if (angleDifference < -180f)
                {
                    angleDifference += 360f;
                }
                
                if (angleDifference > 0)
                {
                    return (true, -1);
                }

                return (true, 1);
            }

            return (false, 0);
        }
        
        private static int WhereToMove(Vector3 hitPoint, Vector3 lightPosition, Vector3 meshPosition)
        {
            var baseVector3 = hitPoint - lightPosition;
            var meshVector3 = meshPosition - lightPosition;
            
            var angleDifference = Quaternion.LookRotation(baseVector3).eulerAngles.y - Quaternion.LookRotation(meshVector3).eulerAngles.y;

            if (angleDifference > 180f)
            {
                angleDifference -= 360f;
            }
            else if (angleDifference < -180f)
            {
                angleDifference += 360f;
            }
                
            if (angleDifference > 0)
            {
                return -1;
            }

            return 1;
        }

        private IEnumerator ShadowWhisperSoundCoroutine()
        {
            var tempShadowWhisperSounds = shadowWhisperSounds;
            
            if (_lastWhisperSound)
            {
                tempShadowWhisperSounds = tempShadowWhisperSounds.Where(x => x != _lastWhisperSound).ToList();
            }
            
            var randomSound = tempShadowWhisperSounds[Random.Range(0, tempShadowWhisperSounds.Count)];
            _lastWhisperSound = randomSound;
            
            audioSource.PlayOneShot(randomSound);
            
            yield return new WaitForSeconds(randomSound.length - timeBeforeNextSound);

            StartCoroutine(ShadowWhisperSoundCoroutine());
        }

        internal IEnumerator MoveShadowPuzzle4()
        {
            yield return new WaitForNextFrameUnit();
            
            _movingSequence.Kill();
            _movingSequence = DOTween.Sequence();
            
            var shadowPosition = transform.position;
            var anchorPosition = anchor.position;
            var anchorBeginningPosition = anchorBeginning.position;
            var anchorVector = anchorPosition - shadowPosition;
            var anchorBeginningVector = anchorBeginningPosition - shadowPosition;
            
            foreach (var meshTransform in _meshTransforms)
            {
                var diffVector = meshTransform.position - shadowPosition;
                
                _movingSequence.Insert(0f, meshTransform.transform.DOMove(shadowPosition + anchorBeginningVector + diffVector, movingDurationBeginning).SetEase(movingEaseBeginning))
                    .Insert(movingDurationBeginning, meshTransform.transform.DOMove(shadowPosition + anchorVector + diffVector / divisionValue, movingDuration).SetEase(movingEase));
            }
            
            _movingSequence.AppendCallback(() =>
            {
                _movingSequence.Kill();

                foreach (var meshTransform in _meshTransforms)
                {
                    meshTransform.transform.GetChild(0).GetComponent<ShadowKill>().RespawnPoint = respawnPoint2;
                }
                
                CameraManager.Instance.NoMorePuzzle4Cam();
            });
        }

        internal void ResetShadowPuzzle4()
        {
            _movingSequence.Kill();
            
            foreach (var meshTransform in _meshTransforms)
            {
                meshTransform.transform.position = meshTransform.position;
            }
            
            CameraManager.Instance.NoMorePuzzle4Cam();
        }

        internal void MoveShadowPuzzle3()
        {
            _movingSequence.Kill();
            _movingSequence = DOTween.Sequence();
            
            var shadowPosition = transform.position;
            var anchorPosition = anchor.position;
            var anchorVector = anchorPosition - shadowPosition;
            
            foreach (var meshTransform in _meshTransforms)
            {
                var diffVector = meshTransform.position - shadowPosition;
                
                _movingSequence.Insert(0f, meshTransform.transform.DOMove(shadowPosition + anchorVector + diffVector, movingDuration).SetEase(movingEase))
                    .Join(meshTransform.transform.DOScaleX(movingScale + meshTransform.localScaleX - 1f, movingDuration * 0.5f))
                    .Insert(movingDuration * 0.5f, meshTransform.transform.DOScaleX(meshTransform.localScaleX, movingDuration * 0.5f));
            }
            
            _movingSequence.AppendCallback(() => _movingSequence.Kill());
        }
    }
}
