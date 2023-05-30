using System.Collections.Generic;
using UnityEngine;
using Utilities.LD;

namespace Managers
{
    public class LoopingMap : MonoBehaviour
    {
        [SerializeField] private Transform newPosition;
        [SerializeField] internal GameObject mapPartToMove;
        [SerializeField] internal List<GameObject> mapPartsToDisable = new();
        private readonly List<Shadow> _shadowsToReset = new();
        private readonly List<Link> _linksToReset = new();

        private void Start()
        {
            foreach (var shadow in FindObjectsOfType<Shadow>())
            {
                _shadowsToReset.Add(shadow);
            }
            
            foreach (var link in FindObjectsOfType<Link>())
            {
                _linksToReset.Add(link);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                LightedElementsManager.Instance.DoCheckDictionaries = false;
                
                var initialPositions = new List<Vector3>();
                foreach (var shadow in _shadowsToReset)
                {
                    initialPositions.Add(shadow.transform.position);
                }
                
                mapPartToMove.transform.position = newPosition.position;
                mapPartToMove.SetActive(true);
                foreach (var mapPart in mapPartsToDisable)
                {
                    mapPart.SetActive(false);
                }

                for (var i = 0; i < _shadowsToReset.Count; i++)
                {
                    var shadow = _shadowsToReset[i];
                    shadow.Initialize(shadow.transform.position - initialPositions[i]);
                }

                foreach (var link in _linksToReset)
                {
                    link.InitializeLineRenderers();
                }
            }
        }
    }
}
