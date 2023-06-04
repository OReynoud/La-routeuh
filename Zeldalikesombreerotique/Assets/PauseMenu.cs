using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public  InputSystemUIInputModule input;
    
    public RectTransform optionGroup;
    public RectTransform controlsImage;

    public Button backButton;
    
    public float settingsOffset;
    public float controlsOffset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void ShowOption()
    {
        StartCoroutine(AvoidSpams());
        optionGroup.DOLocalMove(optionGroup.localPosition + Vector3.right * settingsOffset, 0.5f);
        controlsImage.DOLocalMove(controlsImage.localPosition + Vector3.down * controlsOffset, 0.5f);
        backButton.Select();
    }

    public void HideOption()
    {
        StartCoroutine(AvoidSpams());
        optionGroup.DOLocalMove(optionGroup.localPosition + Vector3.left * settingsOffset, 0.5f);
        controlsImage.DOLocalMove(controlsImage.localPosition + Vector3.up * controlsOffset, 0.5f);
    }
    public IEnumerator AvoidSpams()
    {
        input.enabled = false;
        yield return new WaitForSeconds(0.6f);
        input.enabled = true;
    }
}
