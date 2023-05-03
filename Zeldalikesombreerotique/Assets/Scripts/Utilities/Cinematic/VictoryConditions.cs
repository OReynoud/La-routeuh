using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities
{
    public class VictoryConditions : MonoBehaviour
    {
        [SerializeField] private GameObject victoryScreen;
        [SerializeField] private List<ElementToLight> elementsToLightToWin = new();

        private void Update()
        {
            var isVictory = elementsToLightToWin.All(element => element.isLighted);

            if (isVictory)
            {
                Victory();
            }
        }
        
        private void Victory()
        {
            victoryScreen.SetActive(true);
        }
    }
}
