using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;
    public  InputSystemUIInputModule input;
    
    public RectTransform optionGroup;
    public RectTransform controlsImage;

    public Button backButton;
    
    public float settingsOffset;
    public float controlsOffset;

    public CanvasGroup background;
    // Start is called before the first frame update
    public void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
        }

        instance = this;
    }

    // Update is called once per frame
    public void ShowOption(InputAction.CallbackContext context)
    {
        PlayerController.instance.introCinematic = true;
        PlayerController.instance.controls.Disable();
        StartCoroutine(AvoidSpams());
        optionGroup.DOLocalMove(optionGroup.localPosition + Vector3.right * settingsOffset, 0.5f);
        controlsImage.DOLocalMove(controlsImage.localPosition + Vector3.down * controlsOffset, 0.5f);
        background.DOFade(0.5f, 0.5f);
        backButton.Select();
    }

    public void HideOption()
    {
        StartCoroutine(AvoidSpams());
        optionGroup.DOLocalMove(optionGroup.localPosition + Vector3.left * settingsOffset, 0.5f);
        controlsImage.DOLocalMove(controlsImage.localPosition + Vector3.up * controlsOffset, 0.5f);
        background.DOFade(0f, 0.5f);
        
        PlayerController.instance.introCinematic = false;
        PlayerController.instance.controls.Enable();
    }
    public IEnumerator AvoidSpams()
    {
        input.enabled = false;
        yield return new WaitForSeconds(0.6f);
        input.enabled = true;
    }
}
