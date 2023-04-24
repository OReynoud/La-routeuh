using UnityEngine;

namespace Utilities
{
    public class HoleWithObject : MonoBehaviour
    {
        [SerializeField] private GameObject objectToShow;
        [SerializeField] private BoxCollider boxCollider;

        internal void ShowObject()
        {
            boxCollider.enabled = false;
            objectToShow.SetActive(true);
        }
        
        internal void HideObject()
        {
            boxCollider.enabled = true;
            objectToShow.SetActive(false);
        }
    }
}
