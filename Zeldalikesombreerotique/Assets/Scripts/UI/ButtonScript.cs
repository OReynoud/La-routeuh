using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Utilities;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;

public class ButtonScript : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public Button self;
    private BaseEventData eventData;
    public float baseScale = 1f;
    public float highlightedScale = 1.05f;

    private Image sprite;
    private void Awake()
    {
        self = GetComponent<Button>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        transform.localScale = Vector3.one * highlightedScale;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        transform.localScale = Vector3.one * baseScale;
    }
}
    
