using UnityEngine;

namespace Utilities.Cinematic
{
    public class WarpedTree : MonoBehaviour
    {
        // Components
        [Header("Components")]
        [SerializeField] private Animator animator;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                animator.enabled = true;
            }
        }
    }
}
