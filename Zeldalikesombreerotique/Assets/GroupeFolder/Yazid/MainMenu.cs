using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public string[] SceneName;
    public int SceneValue;
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
}
