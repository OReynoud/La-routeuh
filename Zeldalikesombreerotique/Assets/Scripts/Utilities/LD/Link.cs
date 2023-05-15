using UnityEngine;

namespace Utilities.LD
{
    public class Link : MonoBehaviour
    {
        // Components
        [Header("Components")]
        [SerializeField] private LineRenderer disabledLineRenderer;
        [SerializeField] private LineRenderer activeLineRenderer;
        
        // Objects to link
        [Header("Objects to link")]
        [SerializeField] private Draw objectToLink1;
        [SerializeField] private Draw objectToLink2;
        internal bool IsObject1Active;
        internal bool IsObject2Active;
        
        // Draw values
        [Header("Draw values")]
        [SerializeField] private float timeToBeDrawn;

        private void Start()
        {
            disabledLineRenderer.SetPositions(new[] { objectToLink1.transform.position, objectToLink2.transform.position });
            activeLineRenderer.enabled = false;
            objectToLink1.Links.Add((this, 1));
            objectToLink2.Links.Add((this, 2));
        }
        
        internal void CheckIfBothObjectsAreActive(int index)
        {
            if (IsObject1Active && IsObject2Active)
            {
                switch (index)
                {
                    case 1:
                        break;
                }
            }
        }
    }
}
