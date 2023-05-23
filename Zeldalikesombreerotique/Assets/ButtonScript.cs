using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonScript : Selectable
{
    private BaseEventData eventData;
    // Start is called before the first frame update

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsHighlighted())
        {
            Debug.Log("Highlighted");
        }
    }
}
