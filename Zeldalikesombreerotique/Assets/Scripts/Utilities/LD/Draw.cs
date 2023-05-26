using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;

namespace Utilities.LD
{
    public class Draw : MonoBehaviour
    {
        internal readonly List<(Link link, int index)> Links = new();
        [SerializeField] private bool isEnabledAtStart;
        internal readonly List<TrafficLights> LinkedTrafficLights = new();
        internal bool IsEnabled;
        
        private LightedElementsManager _lightedElementsManager;
        
        private void Start()
        {
            _lightedElementsManager = LightedElementsManager.Instance;
            
            if (isEnabledAtStart)
            {
                Enable();
            }
        }

        private void FixedUpdate()
        {
            if (IsEnabled)
            {
                _lightedElementsManager.RevealedObjects[gameObject] = true;
            }
            
            _lightedElementsManager.CurrentCheckCoroutine ??= StartCoroutine(_lightedElementsManager.CheckDictionariesCoroutine());
        }

        internal void Disable()
        {
            if (LinkedTrafficLights.All(x => x.CheckIfLinked())) return;
            
            IsEnabled = false;
            ChangeLinkValues(false);
            foreach (var trafficLight in LinkedTrafficLights)
            {
                trafficLight.CheckIfLinked();
            }
        }

        internal void Enable()
        {
            IsEnabled = true;
            ChangeLinkValues(true);
            foreach (var trafficLight in LinkedTrafficLights)
            {
                trafficLight.CheckIfLinked();
            }
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
