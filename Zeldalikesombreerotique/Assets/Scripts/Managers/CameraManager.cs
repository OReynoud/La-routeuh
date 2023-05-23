using UnityEngine;

namespace Managers
{
    public class CameraManager : MonoBehaviour
    {
        // Singleton
        internal static CameraManager Instance;
        
        [SerializeField] private Animator animator;
        
        // Animator hashes
        private static readonly int ToBeKilled = Animator.StringToHash("boutToBeKilled");
        private static readonly int TransitioningBackToMain = Animator.StringToHash("transitioningBackToMain");

        private void Awake()
        {
            if (Instance != null)
            {
                DestroyImmediate(gameObject);
                return;
            }
            Instance = this;
        }
        
        internal void BoutToBeKilled()
        {
            animator.SetBool(ToBeKilled, true);
        }
        
        internal void NoMoreBoutToBeKilled(bool transitioningBackToMain = false)
        {
            if (transitioningBackToMain) animator.SetTrigger(TransitioningBackToMain);
            animator.SetBool(ToBeKilled, false);
        }
    }
}