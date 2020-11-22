using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    WaitForSeconds PreLevel, PostLevel;

    int CURRENT_LEVEL = 1;
    bool gameFinished = false;

    public PlayerController pc;
    PlayerHealth ph;
    [HideInInspector]
    public bool playerWin, roundOver;
    bool endOfGame = false;

    //beginning and end animations
    public float beforeLevelWait;
    public float afterLevelWait;
    public MainUI mui;

    void Start()
    {
        DontDestroyOnLoad(this);
        ph = pc.GetComponent<PlayerHealth>();
        PreLevel = new WaitForSeconds(1.5f); //this can be changed later methinks
        PostLevel = new WaitForSeconds(3.5f);
        StartCoroutine(GameLoop());
    }
    //on scene load delegate
    //thanks live lab <3
    private void OnEnable()
    {
        
        SceneManager.sceneLoaded += OnSceneLoad;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }
    IEnumerator GameLoop()
    {
        while (!endOfGame)
        {
            yield return StartCoroutine(LevelStart());
            yield return StartCoroutine(LevelInProgress());
            yield return StartCoroutine(LevelEnd());
        }
    }

    public IEnumerator LevelStart()
    {
        //play the floor advancing animation and fade that screen out
        //reset the playerhealth? maybe? havent decided, will do when I make the first level also.
        Debug.Log("LEVEL START...");
        pc.enabled = false;
        yield return PreLevel;
        pc.enabled = true;
    }

    public IEnumerator LevelInProgress()
    {
        roundOver = false; //i hope this works before the game loop catches on.
        Debug.Log("Now waiting for round over flag. roundOver = " + roundOver);
        yield return new WaitUntil(() => roundOver);
        
        Debug.Log("Round is over. The player " + (playerWin ? "found the exit." : "died."));
    }

    public IEnumerator LevelEnd()
    {
        //Debug.Log("LEVEL END...");
        pc.enabled = false;
        if (playerWin)
        {
            Debug.Log("The player got to the exit!");
            CURRENT_LEVEL++; //advance to the next level.
            mui.SetFloorNumberText(CURRENT_LEVEL);
            if (CURRENT_LEVEL > 8)
            {
                //call the ending and dont return.
                endOfGame = true;
                Debug.Log("winnar");
                
            }
            
            yield return PostLevel;
        }
        else
        {
            //pre wait: "you died"
            yield return PostLevel;
        }


        if (!endOfGame)
        {
            //load the current level scene.
            Debug.Log("Loading level " + CURRENT_LEVEL);
            SceneManager.LoadScene("Level" + CURRENT_LEVEL);
        }
        else
        {
            Debug.Log("Load scene 'ending'.");
            SceneManager.LoadScene("OutroScene"); //tha end B)
        }
        
        
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if(scene.name[0] == 'L')
        {
            //we're in a level because i named the scenes that way
            Debug.Log("SCENE NUMBER == " + scene.name[scene.name.Length - 1]);
            switch (scene.name[scene.name.Length - 1])
            {
                case '1': AudioManager.singleton.PlayLevelMusic("Level1_2", true); break;
                case '3': AudioManager.singleton.PlayLevelMusic("Level3_4", true); break;
                case '5': AudioManager.singleton.PlayLevelMusic("Level5_6", true); break;
                case '7': AudioManager.singleton.PlayLevelMusic("Level7_8", true); break;
            }

            try
            {
                pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
                mui = GameObject.Find("Canvas").GetComponent<MainUI>();
            }
            catch (MissingReferenceException e)
            {
                Debug.Log("No player found");
            }
        }
        else
        {
            //we're in the main menu
            //AudioManager.singleton.PlayMusic("MainMenuTrack", true); break;
            Destroy(this.gameObject);
        }
        
        
    }
}
