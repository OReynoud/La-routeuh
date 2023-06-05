using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnShadowPuzzle : MonoBehaviour
{
    private GameObject shadowChild;
    private List<Transform> palier;
    // Start is called before the first frame update
    void Start()
    {
        if(gameObject.name != "Ombres_Paliers")
        {
            shadowChild = this.transform.GetChild(0).gameObject;
            shadowChild.SetActive(false);
        }

        if(gameObject.name == "Ombres_Paliers")
        {
            foreach (Transform child in transform)
            {
                palier.Add(child);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter()
    {
        if(gameObject.name == "Ombres_Paliers")
        {
            for (int i = 0; i < palier.Count; i++)
            {
                palier[i].transform.GetComponent<SpawnShadowPuzzle>().ResetShadowChild();
            }
        }
    }

    private void OnTriggerExit()
    {
        if(gameObject.name != "Ombres_Paliers")
        {
            shadowChild.SetActive(true);
        }
    }

    public void ResetShadowChild()
    {
        shadowChild.SetActive(false);
    }
}
