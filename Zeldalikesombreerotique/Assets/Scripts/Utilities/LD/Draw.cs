using System.Collections.Generic;
using UnityEngine;

namespace Utilities.LD
{
    public class Draw : MonoBehaviour
    {
        internal readonly List<(Link link, int index)> Links = new();
        [SerializeField] private bool isEnabledAtStart;
        
        private void Start()
        {
            if (isEnabledAtStart)
            {
                Enable();
            }
        }

        internal void Disable()
        {
            ChangeLinkValues(false);
        }

        internal void Enable()
        {
            ChangeLinkValues(true);
        }

        private void ChangeLinkValues(bool isEnabled)
        {
            foreach (var link in Links)
            {
                switch (link.index)
                {
                    case 1:
                        link.link.IsObject1Enabled = isEnabled;
                        link.link.CheckIfBothObjectsAreEnabled(1);
                        break;

                    case 2:
                        link.link.IsObject2Enabled = isEnabled;
                        link.link.CheckIfBothObjectsAreEnabled(2);
                        break;
                }
            }
        }
    }
}
