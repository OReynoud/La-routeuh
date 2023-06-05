using System;
using System.Collections;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    internal class MainMenuManager : MonoBehaviour
    {
        public int SceneValue;
        public  InputSystemUIInputModule input;
        public RectTransform title;
        public RectTransform controlsImage;
        public RectTransform mainGroup;
        
        public RectTransform optionGroup;

        public float mainOffset;
        public float settingsOffset;
        public float controlsOffset;

        public Slider soundSlider;
        public Slider musicSlider;
        public Toggle vibrationsCheck;
        public Toggle fullScreenCheck;

        public CanvasGroup credits;
        // Start is called before the first frame update
        

        void Start()
        {
            SceneValue = PlayerPrefs.GetInt("TheValue", SceneValue);
            Cursor.lockState = CursorLockMode.Locked;
        }
        // Update is called once per frame
        private void Update()
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
            }
        }
        public void Quit()
        {
            Application.Quit();
        }

        public void ChangeScreen()
        {
            Debug.Log(Screen.fullScreen);
            //Screen.fullScreenMode = fullScreenCheck.isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            Screen.fullScreen = !Screen.fullScreen;
            Debug.Log(Screen.fullScreenMode);
        }
        public void Play()
        {
            SceneManager.LoadScene("Loop - Puzzle 1");
        }
        public void ShowOptions()
        {
            StartCoroutine(AvoidSpams());
            mainGroup.DOLocalMove(mainGroup.localPosition + Vector3.left * mainOffset, 0.5f);
            title.DOLocalMove(title.localPosition + Vector3.left * mainOffset, 0.5f); 
            optionGroup.DOLocalMove(optionGroup.localPosition + Vector3.up * settingsOffset, 0.5f);
            controlsImage.DOLocalMove(controlsImage.localPosition + Vector3.down * controlsOffset, 0.5f);
        }

        public void HideOptions()
        {
            StartCoroutine(AvoidSpams());
            mainGroup.DOLocalMove(mainGroup.localPosition + Vector3.right * mainOffset, 0.5f);
            title.DOLocalMove(title.localPosition + Vector3.right * mainOffset, 0.5f); 
            optionGroup.DOLocalMove(optionGroup.localPosition + Vector3.down * settingsOffset, 0.5f);
            controlsImage.DOLocalMove(controlsImage.localPosition + Vector3.up * controlsOffset, 0.5f);
        }

        public void ShowCredits()
        {
            StartCoroutine(AvoidSpams());
            if (credits.alpha == 1)
            {
                DOTween.To(() => credits.alpha,x => credits.alpha = x,0,0.4f);
            }
            else
            {
                DOTween.To(() => credits.alpha,x => credits.alpha = x,1,0.4f);
            }
        }

        public void UpdateSoundSettings()
        {
            PlayerPrefs.SetFloat("Sound",soundSlider.value);
        }

        public void UpdateMusicSettings()
        {
            PlayerPrefs.SetFloat("Music",musicSlider.value);
        }
        public void UpdateVibrationSettings()
        {
            PlayerPrefs.SetInt("Vibrations",vibrationsCheck.isOn? 1:0);
        }

        public IEnumerator AvoidSpams()
        {
            input.enabled = false;
            yield return new WaitForSeconds(0.6f);
            input.enabled = true;
        }
    }
}
