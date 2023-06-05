using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Player;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
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
    public Slider soundSlider;
    public Slider musicSlider;
    public Toggle vibrationsCheck;
    public Toggle fullScreenCheck;
    // Start is called before the first frame update
    public void Awake()
    {
        if (instance != null)
        {
            DestroyImmediate(this);
        }

        instance = this;
    }
    public void Quit()
    {
        Application.Quit();
    }

    private void FixedUpdate()
    {
        
        if (Keyboard.current.anyKey.isPressed && Cursor.lockState != CursorLockMode.None)
        {
            Debug.Log("switch to mouse kb");
            Cursor.lockState = CursorLockMode.None;
        }

        if (Gamepad.current == null)return;
        if (Gamepad.current.wasUpdatedThisFrame && Cursor.lockState != CursorLockMode.Locked)
        {
            Debug.Log("switch to gamepad");
            Cursor.lockState = CursorLockMode.Locked;
            backButton.Select();
        }
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
        input.enabled = false;
        optionGroup.DOLocalMove(optionGroup.localPosition + Vector3.left * settingsOffset, 0.5f);
        controlsImage.DOLocalMove(controlsImage.localPosition + Vector3.up * controlsOffset, 0.5f);
        background.DOFade(0f, 0.5f);

        StartCoroutine(AvoidSpams2());
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ChangeScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        /*if (!Screen.fullScreen)
        {
            Screen.SetResolution(1366,768,false);
        }
        else
        {
            Screen.SetResolution(1980,1080,true);
        }*/
        
    }
    public IEnumerator AvoidSpams()
    {
        input.enabled = false;
        yield return new WaitForSeconds(0.6f);
        input.enabled = true;
    }

    public IEnumerator AvoidSpams2()
    {
        yield return new WaitForSeconds(0.6f);
        PlayerController.instance.introCinematic = false;
        PlayerController.instance.controls.Enable();
    }
    
    public void UpdateSoundSettings()
    {
        PlayerPrefs.SetFloat("Sound",soundSlider.value);
        SoundSettingsUpdater.instance.UpdateSfx();
    }

    public void UpdateMusicSettings()
    {
        PlayerPrefs.SetFloat("Music",musicSlider.value);
        SoundSettingsUpdater.instance.UpdateMusic();
    }
    public void UpdateVibrationSettings()
    {
        PlayerPrefs.SetInt("Vibrations",vibrationsCheck.isOn? 1:0);
    }
}
