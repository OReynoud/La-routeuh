using Managers;
using UnityEngine;

namespace Utilities.LD
{
    public class ShadowTrigger : MonoBehaviour
    {
        [SerializeField] private Shadow shadow;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(shadow.MoveShadowPuzzle4());
                CameraManager.Instance.Puzzle4Cam();
                GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
}
