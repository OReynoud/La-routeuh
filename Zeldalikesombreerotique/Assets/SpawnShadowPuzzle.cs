using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnShadowPuzzle : MonoBehaviour
{
    public GameObject shadowParent;
    // Start is called before the first frame update
    void Start()
    {
        shadowParent.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit()
    {
        shadowParent.SetActive(true);
    }
}
