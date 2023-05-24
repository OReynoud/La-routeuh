using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;

public class ButtonScript : Selectable
{
    private BaseEventData eventData;
    public Color baseColor;
    public Color highlightedColor;
    public float baseScale = 0.25f;
    public float highlightedScale = 0.3f;

    private Image sprite;
    // Start is called before the first frame update

    // Update is called once per frame
    void FixedUpdate()
    {
        if (IsHighlighted())
        {
            targetGraphic.color = highlightedColor;
            transform.localScale = Vector3.one * highlightedScale;
            Debug.Log("Highlighted");
        }
        else
        {
            transform.localScale = Vector3.one * baseScale;
            targetGraphic.color = baseColor;
        }
    }
}
