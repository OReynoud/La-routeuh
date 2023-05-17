using UnityEngine;

namespace Utilities.Cinematic
{
    public class TornTree : MonoBehaviour
    {
        // Components
        [Header("Components")]
        [SerializeField] private Animator animator;

        // Animator values
        private static readonly int Torn = Animator.StringToHash("Torn");

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                animator.SetTrigger(Torn);
            }
        }
    }
}
