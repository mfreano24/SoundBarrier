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

    void Start()
    {
        DontDestroyOnLoad(this);
        ph = pc.GetComponent<PlayerHealth>();
        PreLevel = new WaitForSeconds(3.5f); //this can be changed later methinks
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
        Debug.Log("LEVEL END...");
        pc.enabled = false;
        if (playerWin)
        {
            CURRENT_LEVEL++; //advance to the next level.
            if (CURRENT_LEVEL > 10)
            {
                //call the ending and dont return.

                Debug.Log("winnar");
            }
            //pre wait, such as an animation to go up or something, idk
            yield return PostLevel;
        }
        else
        {
            //pre wait: "you died", voice message saying bro wtf
            yield return PostLevel;
        }

        //load the current level scene.
        Debug.Log("Loading level " + CURRENT_LEVEL);
        SceneManager.LoadScene("Level" + CURRENT_LEVEL);
        
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
}
