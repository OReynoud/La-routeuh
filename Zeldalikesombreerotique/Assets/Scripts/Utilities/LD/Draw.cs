using System.Collections.Generic;
using UnityEngine;

namespace Utilities.LD
{
    public class Draw : MonoBehaviour
    {
        internal readonly List<(Link link, int index)> Links = new();

        internal void Disable()
        {
            foreach (var link in Links)
            {
                switch (link.index)
                {
                    case 1:
                        link.link.IsObject1Active = false;
                        link.link.CheckIfBothObjectsAreActive(1);
                        break;
                    case 2:
                        link.link.IsObject2Active = false;
                        link.link.CheckIfBothObjectsAreActive(2);
                        break;
                }
            }
        }
    }
}
