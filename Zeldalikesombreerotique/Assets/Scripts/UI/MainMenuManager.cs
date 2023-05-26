using System;
using DG.Tweening;
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
        private InputSystemUIInputModule input;
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

            if (Gamepad.current.wasUpdatedThisFrame && Cursor.lockState != CursorLockMode.Locked)
            {
                Debug.Log("switch to gamepad");
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        public void Play()
        {
            SceneManager.LoadScene("Loop - Puzzle 1");
        }

        public void ShowOptions()
        {
            mainGroup.DOLocalMove(mainGroup.localPosition + Vector3.left * mainOffset, 0.5f);
            title.DOLocalMove(title.localPosition + Vector3.left * mainOffset, 0.5f); 
            optionGroup.DOLocalMove(optionGroup.localPosition + Vector3.up * settingsOffset, 0.5f);
            controlsImage.DOLocalMove(controlsImage.localPosition + Vector3.down * controlsOffset, 0.5f);
        }

        public void HideOptions()
        {
            mainGroup.DOLocalMove(mainGroup.localPosition + Vector3.right * mainOffset, 0.5f);
            title.DOLocalMove(title.localPosition + Vector3.right * mainOffset, 0.5f); 
            optionGroup.DOLocalMove(optionGroup.localPosition + Vector3.down * settingsOffset, 0.5f);
            controlsImage.DOLocalMove(controlsImage.localPosition + Vector3.up * controlsOffset, 0.5f);
        }

        public void ShowCredits()
        {
            
        }

        public void HideCredits()
        {
            
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
    }
}
