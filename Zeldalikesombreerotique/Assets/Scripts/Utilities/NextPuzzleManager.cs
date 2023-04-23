using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities
{
    public class NextPuzzleManager : MonoBehaviour
    {
        [SerializeField] private List<ElementToLight> elementsToLightToPassPuzzle = new();
        [SerializeField] private GameObject nextRoad1GameObject;
        [SerializeField] private GameObject nextRoad2GameObject;
        [SerializeField] private GameObject[] previousRoadGameObjects = new GameObject[2];
        [SerializeField] private List<LoopingMap> loopingMapsToChange1 = new();
        [SerializeField] private List<LoopingMap> loopingMapsToChange2 = new();
        private bool _hasPassedPuzzle;

        private void Awake()
        {
            _hasPassedPuzzle = false;
        }

        private void Update()
        {
            if (!_hasPassedPuzzle)
            {
                var isNextPuzzle = elementsToLightToPassPuzzle.All(element => element.isLighted);

                if (isNextPuzzle)
                {
                    NextPuzzle();
                    _hasPassedPuzzle = true;
                }
            }
        }
        
        private void NextPuzzle()
        {
            foreach (var loopingMap in loopingMapsToChange1)
            {
                loopingMap.mapPartsToDisable.AddRange(previousRoadGameObjects.ToList());
                loopingMap.mapPartToMove = nextRoad1GameObject;
            }
            
            foreach (var loopingMap in loopingMapsToChange2)
            {
                loopingMap.mapPartsToDisable.AddRange(previousRoadGameObjects.ToList());
                loopingMap.mapPartToMove = nextRoad2GameObject;
            }
        }
    }
}
