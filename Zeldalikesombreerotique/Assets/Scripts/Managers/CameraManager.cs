using UnityEngine;

namespace Managers
{
    public class CameraManager : MonoBehaviour
    {
        // Singleton
        internal static CameraManager Instance;
        
        public Animator animator;
        
        // Animator hashes
        private static readonly int ToBeKilled = Animator.StringToHash("boutToBeKilled");
        private static readonly int TransitioningBackToMain = Animator.StringToHash("transitioningBackToMain");
        private static readonly int EndCredits = Animator.StringToHash("EndCredits");
        private static readonly int Puzzle4 = Animator.StringToHash("puzzle4");
        private static readonly int Killed = Animator.StringToHash("killed");

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

        internal void Credits()
        {
            animator.SetBool(EndCredits,true);
        }

        internal void Puzzle4Cam()
        {
            animator.SetBool(Puzzle4, true);
        }

        internal void NoMorePuzzle4Cam()
        {
            animator.SetBool(Puzzle4, false);
        }
        
        internal void Kill()
        {
            animator.SetBool(Killed, true);
        }
        
        internal void NoMoreKill()
        {
            animator.SetBool(Killed, false);
        }
    }
}
