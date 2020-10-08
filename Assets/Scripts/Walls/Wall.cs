﻿using System.Collections;
using UnityEngine;

public class Wall : MonoBehaviour
{

    /*
     *  MICHAEL FREANEY
     * TAGD FALL 2020 GAME JAM
     * Note: The "clear" wall should be maintained at a scale of (0.999,0.999,0.999) so that the dissolve wall appears to fully cover it.
     * 
     * 
     * 
     */
    Renderer r;
    Material mat;

    public GameObject clearWall; //make this the clear counterpart to the dissolve wall. Set it inactive initially, then set it active upon activating wallreaction.
    bool activated = false;

    bool DEBUG_NoGlass = false;
    private void Start()
    {
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


    public IEnumerator WallReaction()
    {
        Material wallMat = r.material;
        Debug.Log("Gunshot! " + Time.time);
        wallMat.SetFloat("Vector1_2F06040B", 0.00f);
        if (!activated)
        {
            if (!DEBUG_NoGlass)
            {
                clearWall.SetActive(true);
            }
            
            activated = true;
        }
        
        yield return new WaitForSeconds(2.5f);
        float val = 0.01f;
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.02f);
            wallMat.SetFloat("Vector1_2F06040B", val);
            val += 0.01f;
        }
    }
}
