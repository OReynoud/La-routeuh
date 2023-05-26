using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities.LD
{
    public class TrafficLights : MonoBehaviour
    {
        [SerializeField] private GameObject greenLight;
        [SerializeField] private GameObject redLight;
        [SerializeField] private List<Draw> appearedDraws;

        private void Awake()
        {
            foreach (var draw in appearedDraws)
            {
                draw.LinkedTrafficLights.Add(this);
            }
        }

        internal bool CheckIfLinked()
        {
            if (appearedDraws.Any(draw => !draw.IsEnabled)) return false;

            foreach (var draw in appearedDraws)
            {
                foreach (var link in draw.Links)
                {
                    link.link.IsTotallyLinked = true;
                }
            }

            StartCoroutine(ChangeLightColor());
            
            return true;
        }

        private IEnumerator ChangeLightColor()
        {
            yield return new WaitUntil(() => appearedDraws.All(draw => draw.Links.All(link => link.link.IsDrawn)));
            
            greenLight.SetActive(true);
            redLight.SetActive(false);
        }
    }
}
