using DG.Tweening;
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
        private Vector3 _object1Position;
        private Vector3 _object2Position;
        
        // Draw values
        [Header("Draw values")]
        [SerializeField] private float timeToBeDrawn;
        [SerializeField] private Ease easeToDraw;
        private Sequence _drawSequence;

        private void Awake()
        {
            _object1Position = objectToLink1.transform.position;
            _object2Position = objectToLink2.transform.position;
            
            disabledLineRenderer.SetPositions(new[] { SlightlyLift(_object1Position), SlightlyLift(_object2Position) });
            enabledLineRenderer.enabled = false;
            
            objectToLink1.Links.Add((this, 1));
            objectToLink2.Links.Add((this, 2));
        }
        
        internal void CheckIfBothObjectsAreEnabled(int index)
        {
            if (IsObject1Enabled && IsObject2Enabled)
            {
                switch (index)
                {
                    case 1:
                        DrawLine(SlightlyLift(_object1Position), SlightlyLift(_object2Position), 1);
                        break;
                    
                    case 2:
                        DrawLine(SlightlyLift(_object2Position), SlightlyLift(_object1Position), 0);
                        break;
                }
            }
            else
            {
                _drawSequence?.Kill();
                enabledLineRenderer.enabled = false;
            }
        }

        private void DrawLine(Vector3 startingPosition, Vector3 endingPosition, int positionIndex)
        {
            enabledLineRenderer.SetPositions(new[] { startingPosition, startingPosition });
            enabledLineRenderer.enabled = true;
                
            _drawSequence = DOTween.Sequence();
            _drawSequence.Append(DOTween.To(() => enabledLineRenderer.GetPosition(positionIndex), 
                    x => enabledLineRenderer.SetPosition(positionIndex, x), 
                    endingPosition, timeToBeDrawn).SetEase(easeToDraw)
                .OnComplete(() => _drawSequence = null));
        }
        
        private static Vector3 SlightlyLift(Vector3 position)
        {
            return new Vector3(position.x, 0.01f, position.z);
        }
    }
} 
