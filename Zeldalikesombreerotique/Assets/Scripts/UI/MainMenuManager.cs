using DG.Tweening;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    internal class MainMenuManager : MonoBehaviour
    {
        public string[] SceneName;
        public int SceneValue;

        public RectTransform mainGroup;

        public RectTransform optionGroup;

        public Vector3[] mainPos;

        public Vector3[] optionsPos;
        // Start is called before the first frame update
        void Start()
        {
            SceneValue = PlayerPrefs.GetInt("TheValue", SceneValue);
        }

        // Update is called once per frame
        void Update()
        {
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
            mainGroup.DOMove(mainPos[1], 0.5f);
            optionGroup.DOMove(optionsPos[0], 0.5f);
        }

        public void HideOptions()
        {
            mainGroup.DOMove(mainPos[0], 0.5f);
            optionGroup.DOMove(optionsPos[1], 0.5f);
        }

        public void ShowCredits()
        {
            
        }

        public void HideCredits()
        {
            
        }
    }
}
