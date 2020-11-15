using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    AudioSource[] src;
    //GUESS WE HAVE TO DO MULTIPLE LIKE THE GOOD OLD DAYS
    public AudioClip walkingClip;
    public AudioClip runningClip;

    PlayerController pc;


    bool sprinting;
    bool moving;
    int currentClipIndex = 0;
    private void Start()
    {
        pc = GetComponent<PlayerController>();
        src = GetComponents<AudioSource>();
        src[0].clip = walkingClip;
        src[1].clip = runningClip;
        src[0].volume = 0;
        src[1].volume = 0;
        src[0].Play();
        src[1].Play();
        Debug.Log("walking: " + src[0].clip.name + "\trunning: " + src[1].clip.name);
    }

    private void Update()
    {
        if (!pc.enabled || Time.timeScale != 1)
        {
            //time scale is set to 0 when paused i believe?
            src[0].volume = 0;
            src[1].volume = 0;
        }
        else
        {
            if (pc.moving)
            {
                if (src[currentClipIndex].volume == 0)
                {
                    src[currentClipIndex].volume = PlayerPrefs.GetFloat("sfxSliderVolume", 0.5f) * 0.95f; //need this to be a little quieter.
                }
            }
            else
            {
                if (src[currentClipIndex].volume != 0)
                {
                    src[currentClipIndex].volume = 0;

                }
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Debug.Log("NOW SPRINTING SOUND");
                currentClipIndex = 1;
                src[0].volume = 0;
                src[1].volume = PlayerPrefs.GetFloat("sfxSliderVolume", 0.5f) * 0.5f;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                Debug.Log("WALKING SOUND NOW");
                currentClipIndex = 0;
                src[1].volume = 0;
                src[0].volume = PlayerPrefs.GetFloat("sfxSliderVolume", 0.5f) * 0.5f;
            }
        }
        

        

        
    }

    void SwitchClipIndex()
    {
        if(currentClipIndex == 0)
        {
            currentClipIndex = 1;
        }
        else
        {
            currentClipIndex = 0;
        }
    }
}
