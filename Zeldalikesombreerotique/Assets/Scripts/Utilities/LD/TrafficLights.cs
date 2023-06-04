using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Managers;
using UnityEngine;
using Utilities.Cinematic;

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
        [SerializeField] private List<Transform> StreetLights;
        private static readonly int ColorRouge = Shader.PropertyToID("_ColorRouge");
        private static readonly int ColorVert = Shader.PropertyToID("_ColorVert");
        
        // Changer de trigger
        [SerializeField] private GameObject nextHubGameObject;
        [SerializeField] private List<GameObject> previousHubGameObjects = new();
        [SerializeField] private LoopingMap loopingMapToChange;

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
                draw.IsPermanentlyEnabled = true;
                
                foreach (var link in draw.Links)
                {
                    link.link.IsTotallyLinked = true;
                }
            }

            ChangeLightColor();
            
            NextPuzzle();
            
            return true;
        }

        private void ChangeLightColor()
        {
            foreach (var streetLight in StreetLights)
            {
                streetLight.GetChild(3).GetComponent<StreetLightTriggerBehavior>().LightStayOn();
            }
            
            var redColor = meshRenderer.material.GetColor(ColorRouge);
            meshRenderer.material.SetColor(ColorRouge, 
                new Color( redColor.r / redLightIntensityFactor, redColor.g / redLightIntensityFactor, redColor.b / redLightIntensityFactor));
            
            var greenColor = meshRenderer.material.GetColor(ColorVert);
            meshRenderer.material.SetColor(ColorVert, 
                new Color( greenColor.r * greenLightIntensityFactor, greenColor.g * greenLightIntensityFactor, greenColor.b * greenLightIntensityFactor));
            
            greenLight.SetActive(true);
            redLight.SetActive(false);
        }
        
        private void NextPuzzle()
        {
            loopingMapToChange.mapPartsToDisable.AddRange(previousHubGameObjects);
            loopingMapToChange.mapPartToMove = nextHubGameObject;
        }
    }
}
