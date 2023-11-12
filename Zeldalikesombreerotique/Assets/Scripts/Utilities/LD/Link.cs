using System.Collections.Generic;
using DG.Tweening;
using Player;
using UnityEngine;

namespace Utilities.LD
{
    public class Link : MonoBehaviour
    {
        // Components
        [Header("Components")]
        [SerializeField] private LineRenderer disabledLineRenderer;
        [SerializeField] private LineRenderer enabledLineRenderer;
        
        // Objects to link
        [Header("Objects to link")]
        [SerializeField] private Draw objectToLink1;
        [SerializeField] private Draw objectToLink2;
        internal bool IsObject1Enabled;
        internal bool IsObject2Enabled;
        
        // Draw values
        [Header("Draw values")]
        [SerializeField] private float timeToBeDrawn;
        [SerializeField] private Ease easeToDraw;
        private Sequence _drawSequence;
        private Sequence _drawReversedSequence;
        internal bool IsTotallyLinked;
        
        // Bezier values
        [Header("Bezier values")]
        [SerializeField] private List<Transform> startPoints;
        [SerializeField] private List<Transform> controlPoints;
        [SerializeField] private List<Transform> endPoints;
        [SerializeField] private int pointsNumber;
        private readonly List<Vector3> _bezierPoints = new();
        private float _totalDistance;

        private void Awake()
        {
            InitializeLineRenderers();
            
            enabledLineRenderer.enabled = false;

            objectToLink1.Links.Add((this, 1));
            objectToLink2.Links.Add((this, 2));
        }
        
        public void OnDrawGizmos()
        {
            for (var j = 0; j < startPoints.Count; j++)
            {
                for (var i = 0f; i < 1f; i += 0.05f)
                {
                    var m1 = Vector3.Lerp(startPoints[j].position, controlPoints[j].position, i);
                    var m2 = Vector3.Lerp(controlPoints[j].position, endPoints[j].position, i);
                    Gizmos.DrawSphere(Vector3.Lerp(m1, m2, i),0.1f);
                    Gizmos.DrawLine(startPoints[j].position, controlPoints[j].position);
                    Gizmos.DrawLine(endPoints[j].position, controlPoints[j].position);
                }
            }
        }

        internal void InitializeLineRenderers()
        {
            _bezierPoints.Clear();
            _bezierPoints.Add(SlightlyLift(startPoints[0].position));
            
            for (var i = 0; i < startPoints.Count; i++)
            {
                var timer = 0f;
                
                while (timer < 1)
                {
                    timer += 1f / pointsNumber;
                    var m1 = Vector3.Lerp(startPoints[i].position, controlPoints[i].position, timer);
                    var m2 = Vector3.Lerp(controlPoints[i].position, endPoints[i].position, timer);
                    m1 = new Vector3(m1.x, -0.1f, m1.z);
                    m2 = new Vector3(m2.x, -0.1f, m2.z);
                    _bezierPoints.Add(SlightlyLift(Vector3.Lerp(m1, m2, timer)));
                }
            }
            
            disabledLineRenderer.positionCount = _bezierPoints.Count;
            enabledLineRenderer.positionCount = _bezierPoints.Count;

            for (var i = 0; i < _bezierPoints.Count; i++)
            {
                disabledLineRenderer.SetPosition(i, _bezierPoints[i]);
                enabledLineRenderer.SetPosition(i, _bezierPoints[i]);
            }

            for (var i = 0; i < _bezierPoints.Count - 1; i++)
            {
                _totalDistance += Vector3.Distance(_bezierPoints[i], _bezierPoints[i + 1]);
            }
            
            InitializeSequences();
        }

        private void InitializeSequences()
        {
            _drawSequence = DOTween.Sequence();
            _drawReversedSequence = DOTween.Sequence();
            
            _drawSequence.SetAutoKill(false);
            _drawReversedSequence.SetAutoKill(false);
            
            for (var i = 1; i < enabledLineRenderer.positionCount; i++)
            {
                var tempSequence = DOTween.Sequence();
                
                for (var j = i; j < enabledLineRenderer.positionCount; j++)
                {
                    var index = j;
                    
                    tempSequence.Join(DOTween.To(() => enabledLineRenderer.GetPosition(index), 
                            x => enabledLineRenderer.SetPosition(index, x), _bezierPoints[i], 
                            timeToBeDrawn * (Vector3.Distance(_bezierPoints[i], _bezierPoints[i - 1]) / _totalDistance))
                        .SetEase(easeToDraw));
                }
                
                _drawSequence.Append(tempSequence);
            }
            
            for (var i = enabledLineRenderer.positionCount - 2; i >= 0; i--)
            {
                var tempSequence = DOTween.Sequence();
                
                for (var j = i; j >= 0; j--)
                {
                    var index = j;
                    
                    tempSequence.Join(DOTween.To(() => enabledLineRenderer.GetPosition(index), 
                            x => enabledLineRenderer.SetPosition(index, x), _bezierPoints[i],
                            timeToBeDrawn * (Vector3.Distance(_bezierPoints[i], _bezierPoints[i + 1]) / _totalDistance))
                        .SetEase(easeToDraw));
                }
                
                _drawReversedSequence.Append(tempSequence);
            }
            
            _drawSequence.Rewind();
            _drawReversedSequence.Rewind();
        }

        internal void CheckIfBothObjectsAreEnabled(int index)
        {
            if (IsObject1Enabled && IsObject2Enabled)
            {
                switch (index)
                {
                    case 1:
                        DrawLine(false);
                        break;
                    
                    case 2:
                        DrawLine(true);
                        break;
                }
            }
            else
            {
                if (!IsTotallyLinked)
                {
                    _drawSequence?.Rewind();
                    enabledLineRenderer.enabled = false;
                }
            }
        }

        private void DrawLine(bool isReversed)
        {
            if (isReversed)
            {
                for (var i = 0; i < enabledLineRenderer.positionCount; i++)
                {
                    enabledLineRenderer.SetPosition(i, _bezierPoints[^1]);
                }
            }
            else
            {
                for (var i = 0; i < enabledLineRenderer.positionCount; i++)
                {
                    enabledLineRenderer.SetPosition(i, _bezierPoints[0]);
                }
            }
            
            enabledLineRenderer.enabled = true;
            
            PlayerController.instance.ScribblingSound(timeToBeDrawn);
            
            if (isReversed)
            {
                _drawReversedSequence.Restart();
            }
            else
            {
                _drawSequence.Restart();
            }
        }
        
        private static Vector3 SlightlyLift(Vector3 position)
        {
            return new Vector3(position.x, 0.01f, position.z);
        }
    }
} 
