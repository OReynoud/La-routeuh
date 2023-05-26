using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangePositionManager : MonoBehaviour
{
    private GameObject player;
    private GameObject pos1,pos2,pos3;

    private int currentPuzzle = 0;

    private string currentScene;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        pos1 = this.transform.GetChild(0).gameObject;
        pos2 = this.transform.GetChild(1).gameObject;
        pos3 = this.transform.GetChild(2).gameObject;
        currentScene = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Keypad0) && currentScene == "Puzzle2 Layout")
        {
                if(currentPuzzle == 3)
              {
                currentPuzzle = 0;
              }
            currentPuzzle += 1;
            if(currentPuzzle == 1)
              {
                player.transform.position = pos1.transform.position;
              }
           else if(currentPuzzle == 2)
              {
                player.transform.position = pos2.transform.position;
              }
            if(currentPuzzle == 3)
              {
                player.transform.position = pos3.transform.position;
              }  
        }

        if(Input.GetKeyDown(KeyCode.Keypad1))
        {
            if(currentScene == "Loop - Puzzle 1")
            {
                SceneManager.LoadScene("LesPetitsPuzzles");
            }
            else if(currentScene == "LesPetitsPuzzles")
            {
                SceneManager.LoadScene("Loop - Puzzle 1");
            }
        }
    }
}
