using SoundBarrier;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Wall : MonoBehaviour
{
    Renderer r;
    Material mat;

    bool dissolveMidBreak = false;
    bool dissolveCoroutineLock = false;

    public GameObject clearWall; //make this the clear counterpart to the dissolve wall. Set it inactive initially, then set it active upon activating wallreaction.
    bool activated = false;

    bool DEBUG_NoGlass = false;

    int numberofActiveCoroutines = 0;
    bool wallCoroutineInProgress = false;

    List<Coroutine> runningWallCoroutines;
    private void Start()
    {
        runningWallCoroutines = new List<Coroutine>();
        clearWall.SetActive(false);
        //not sure if this absolutely needs to be in start or awake
        r = GetComponent<Renderer>();
        mat = new Material(r.material); //copy the material and set it to just this block, creating an instance on startup so that each one can be modified
        //this should allow the player controller to change the "time" value individually.
        r.material = mat;
        r.material.SetFloat("Vector1_2F06040B", 1.00f);

        //DEBUG VARIABLES (SHOULD ALL BE FALSE IF TESTING REAL GAMEPLAY)
        //DEBUG_NoGlass = true;
        //r.material.SetFloat("Vector1_2F06040B", 0.00f); //comment out when not testing

    }

    public void WallReactionHelper()
    {

        Material wallMat = r.material;
        Debug.Log("Gunshot! " + Time.time);

        if (!activated)
        {
            if (!DEBUG_NoGlass)
            {
                clearWall.SetActive(true);
            }

            activated = true;
        }
        //praying
        if(runningWallCoroutines.Count > 0)
        {
            foreach(Coroutine e in runningWallCoroutines)
            {
                //stop the coroutine
                StopCoroutine(e);
            }
            runningWallCoroutines.Clear();
        }
        runningWallCoroutines.Add(StartCoroutine(WallReaction(wallMat)));
    }


    public IEnumerator WallReaction(Material wallMat)
    {
        numberofActiveCoroutines++;
        wallCoroutineInProgress = true;
        wallMat.SetFloat("Vector1_2F06040B", 0.00f); //put inside the thread??? maybe
        yield return new WaitForSeconds(2.5f);
        float val = 0.01f;
        for (int i = 0; i < 100; i++)
        {
            
            yield return new WaitForSeconds(0.02f);
            wallMat.SetFloat("Vector1_2F06040B", val);
            val += 0.01f;
        }
        wallCoroutineInProgress = false;
        numberofActiveCoroutines--;
        //this should be added to the running coroutine list so that it can be cleared and the wall will wait for 7.5 seconds always.
        runningWallCoroutines.Add(StartCoroutine(ClearWallDisappear())); //time the clear wall disappearing now
    }


    public IEnumerator ClearWallDisappear()
    {
        
        yield return new WaitForSeconds(7.5f);
        clearWall.SetActive(false);
        activated = false;
    }
}
