using System;
using DG.Tweening;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

namespace UI
{
    internal class MainMenuManager : MonoBehaviour
    {
        public string[] SceneName;
        public int SceneValue;
        private InputSystemUIInputModule input;
        public RectTransform title;
        public RectTransform controlsImage;
        public RectTransform mainGroup;
        
        public RectTransform optionGroup;

        public float mainOffset;
        public float settingsOffset;
        public float controlsOffset;
        public enum Menus
        {
            Main,
            Option,
            Credits
        }

        public Menus currentMenu = Menus.Main;
        // Start is called before the first frame update
        

        void Start()
        {
            SceneValue = PlayerPrefs.GetInt("TheValue", SceneValue);
            Cursor.lockState = CursorLockMode.Locked;
        }
        // Update is called once per frame
        void Update()
        {
            switch (currentMenu)
            {
                case Menus.Main:
                    break;
                case Menus.Option:
                    break;
                case Menus.Credits:
                    break;
            }
            /*if(Input.GetKeyDown(KeyCode.Return))
            {
                SceneValue += 1;
                if(SceneValue >= 4)
                {
                    SceneValue = 0;
                }
                PlayerPrefs.SetInt("TheValue", SceneValue);
                SceneManager.LoadScene(SceneName[SceneValue]);
            }*/
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
    }
}
