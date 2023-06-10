using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Player;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;
using Utilities.LD;

public class Puzzle2ProgressManager : MonoBehaviour
{
    [SerializeField] private List<UnityEngine.Light> lights = new List<UnityEngine.Light>();
    [SerializeField] private UnityEngine.Light spotLight; 
    [SerializeField] private Transform pairDetector;

    [SerializeField] private float detectionRadius;
    [SerializeField] private List<Rigidbody> pairs1 = new List<Rigidbody>();
    [SerializeField] private List<Rigidbody> pairs2 = new List<Rigidbody>();
    [SerializeField] private List<bool> completedPairs = new List<bool>();
    

    [SerializeField] private List<GameObject> wallsToDisable= new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < pairs1.Count; i++)
        {
            completedPairs.Add(false);
        }
        spotLight.color = lights[0].color;
        spotLight.transform.SetParent(lights[0].transform);
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(pairDetector.position, detectionRadius);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var pairElements = Physics.OverlapSphere(pairDetector.position,detectionRadius);
        for (int i = 0; i < completedPairs.Count; i++)
        {
            if (completedPairs[i])
            {
                continue;
            }

            Rigidbody currentPair1= new Rigidbody();
            Rigidbody currentPair2 = new Rigidbody();
            for (int j = 0; j < pairElements.Length; j++)
            {
                if (pairElements[j].attachedRigidbody == pairs1[i])
                {
                    currentPair1 = pairElements[j].attachedRigidbody;
                }
                if (pairElements[j].attachedRigidbody == pairs2[i])
                {
                    currentPair2 = pairElements[j].attachedRigidbody;
                }
            }

            if (currentPair1 && currentPair2 && !PlayerController.instance.isGrabbing)
            {
                completedPairs[i] = true;
                int oui = 0;
                foreach (var complete in completedPairs)
                {
                    if (complete)
                    {
                        oui++;
                    }
                }
                lights[oui - 1].transform.parent.gameObject.SetActive(false);
                lights[oui].transform.parent.gameObject.SetActive(true);
                spotLight.color = lights[oui].color;
                spotLight.transform.SetParent(lights[oui].transform);
                if (wallsToDisable[oui - 1])
                {
                    wallsToDisable[oui - 1].SetActive(false);
                }

                PlayerController.instance.objectType.mobilityType = DynamicObject.MobilityType.None;
            }
        }
    }


}
