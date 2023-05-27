using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities.LD
{
    public class TrafficLights : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private GameObject greenLight;
        [SerializeField] private float greenLightIntensityFactor;
        [SerializeField] private GameObject redLight;
        [SerializeField] private float redLightIntensityFactor;
        [SerializeField] private List<Draw> appearedDraws;
        private static readonly int ColorRouge = Shader.PropertyToID("_ColorRouge");
        private static readonly int ColorVert = Shader.PropertyToID("_ColorVert");

        private void Awake()
        {
            var redColor = meshRenderer.material.GetColor(ColorRouge);
            meshRenderer.material.SetColor(ColorRouge, 
                new Color( redColor.r * redLightIntensityFactor, redColor.g * redLightIntensityFactor, redColor.b * redLightIntensityFactor));
            
            var greenColor = meshRenderer.material.GetColor(ColorVert);
            meshRenderer.material.SetColor(ColorVert, 
                new Color( greenColor.r / greenLightIntensityFactor, greenColor.g / greenLightIntensityFactor, greenColor.b / greenLightIntensityFactor));
            
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
            
            var redColor = meshRenderer.material.GetColor(ColorRouge);
            meshRenderer.material.SetColor(ColorRouge, 
                new Color( redColor.r / redLightIntensityFactor, redColor.g / redLightIntensityFactor, redColor.b / redLightIntensityFactor));
            
            var greenColor = meshRenderer.material.GetColor(ColorVert);
            meshRenderer.material.SetColor(ColorVert, 
                new Color( greenColor.r * greenLightIntensityFactor, greenColor.g * greenLightIntensityFactor, greenColor.b * greenLightIntensityFactor));
            
            greenLight.SetActive(true);
            redLight.SetActive(false);
        }
    }
}
