using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Utilities;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Toggle = UnityEngine.UI.Toggle;

public class ButtonScript : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    public Button self;
    public float baseScale = 1f;
    public float highlightedScale = 1.05f;
    public Image checkedBox;
    public Image uncheckedBox;

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

    public void OnSubmit(BaseEventData eventData)
    {
        if (transform.GetComponentInChildren<Toggle>())
        {
            var state = transform.GetComponentInChildren<Toggle>().isOn;
            transform.GetComponentInChildren<Toggle>().isOn = !state;
            if (!state == false)
            {
                uncheckedBox.enabled = true;
                checkedBox.enabled = false;
            }
            else
            {
                uncheckedBox.enabled = false;
                checkedBox.enabled = true;
            }
        }
    }
}
    