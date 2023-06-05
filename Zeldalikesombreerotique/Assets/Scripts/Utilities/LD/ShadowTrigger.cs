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
                if (shadow.isPuzzle4Shadow)
                {
                    StartCoroutine(shadow.MoveShadowPuzzle4());
                    CameraManager.Instance.Puzzle4Cam();
                }
                else if (shadow.isPuzzle3Shadow)
                {
                    shadow.MoveShadowPuzzle3();
                }
                GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
}
