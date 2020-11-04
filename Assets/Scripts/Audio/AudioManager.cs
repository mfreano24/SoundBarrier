using SoundBarrier;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class AudioManager : SingletonDDOL<AudioManager>
{
    //store all music clips AND sound effect clips in a list from the inspector.
    public List<AudioClip> clipsMusicInspector;
    public List<AudioClip> clipsSFXInspector;

    //then, populate these dictionaries using the clip names
    Dictionary<string, AudioSource> clipsMusic;
    Dictionary<string, AudioClip> clipsSFX;

    //source to load music into.
    AudioSource currentSourceMusic;
    Dictionary<string, AudioSource> currentSourceSFX;
    string currentMusicName; //so we can refer to it in the dict

    //any volume settings we have from (0.0f) to (1.0f).
    //assuming settings menu has player prefs for these?
    float volumeMusic = 1;
    float volumeSFX = 1;

    bool musicFadeLock = false;

    //start
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
        //initializing and setting values
        clipsMusic = new Dictionary<string, AudioSource>();
        clipsSFX = new Dictionary<string, AudioClip>();
        //these will be created with AddComponent.
        currentSourceSFX = new Dictionary<string, AudioSource>();

        //populating the dictionaries based on the list entries.
        for (int i = 0; i < clipsMusicInspector.Count; i++)
        {
            AudioSource curr = gameObject.AddComponent<AudioSource>();
            curr.clip = clipsMusicInspector[i];
            if (i != 0)
            {
                curr.volume = 0;
            }
            else
            {
                currentMusicName = clipsMusicInspector[i].name;
                curr.volume = 0.5f;
            }
            clipsMusic.Add(clipsMusicInspector[i].name, curr);
            curr.loop = true;
            curr.Play(); //always play it no matter what, they should line up.
        }

        foreach (AudioClip aud in clipsSFXInspector)
        {
            clipsSFX.Add(aud.name, aud);
        }

        //begin the sound effect cleanup loop. Works like Update in Monobehaviors.
        StartCoroutine(CleanUpSFX());
    }

    //public methods
    public void PlaySFX(string sfxName)
    {
        //play a sound effect based on the sound effect's name.
        /*
         * TODO: this doesn't yet account for duplicate sounds, which will likely exist.
         * Maybe a solution that checks for the sound in the dictionary and replays it?
         */
        if (currentSourceSFX.ContainsKey(sfxName))
        {
            //if the sound is already being played we could just replay it(?)
            //this may need some testing to ensure it doesn't get caught by the cleanup process.
            Debug.Log("volume = " + volumeSFX);
            currentSourceSFX[sfxName].volume = volumeSFX;
            currentSourceSFX[sfxName].Play();
        }
        else
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.clip = clipsSFX[sfxName];
            Debug.Log("volume = " + volumeSFX);
            newSource.volume = volumeSFX;
            //Play the sound before adding to the dictionary, so that cleanup doesn't catch it before it even starts playing.
            newSource.Play();
            currentSourceSFX.Add(sfxName, newSource);
        }
    }

    //OVERRIDE: If we need to pass in a pitch (TOT_success sound, for example), use this override.
    public void PlaySFX(string sfxName, float pitch)
    {
        if (currentSourceSFX.ContainsKey(sfxName))
        {
            Debug.Log("Playing pre-existing sound with pitch value of: " + pitch + ".");
            //if the sound is already being played we could just replay it(?)
            //this may need some testing to ensure it doesn't get caught by the cleanup process.
            Debug.Log("volume = " + volumeSFX);
            currentSourceSFX[sfxName].volume = volumeSFX;
            currentSourceSFX[sfxName].pitch = pitch;
            currentSourceSFX[sfxName].Play();
        }
        else
        {
            Debug.Log("Playing new sound with pitch value of: " + pitch + ".");
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.name = sfxName;
            newSource.clip = clipsSFX[sfxName];
            //Debug.Log("volume = " + volumeSFX);
            newSource.volume = volumeSFX;
            newSource.pitch = pitch;
            //Play the sound before adding to the dictionary, so that cleanup doesn't catch it before it even starts playing.
            newSource.Play();
            currentSourceSFX.Add(sfxName, newSource);
        }

    }


    public void StopSFX(string sfxName)
    {
        //stops the specified sound effect. Any cleanup is taken care of in the CleanUpSFX coroutine.
        currentSourceSFX[sfxName].Stop();
    }

    IEnumerator CleanUpSFX()
    {
        //check for any finished sound effects every frame
        //NOTE: can be expanded to wait a couple frames or so for efficiency's sake.
        while (Application.isPlaying)
        {
            //make a copy? this seems bad for memory
            foreach (KeyValuePair<string, AudioSource> aud in new Dictionary<string, AudioSource>(currentSourceSFX))
            {
                if (!aud.Value.isPlaying)
                {
                    //if the audio source is no longer playing anything:
                    currentSourceSFX.Remove(aud.Key);
                    Destroy(aud.Value);
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void PlayLevelMusic(string musicName, bool isLooping)
    {
        if(musicName == "Level1_2")
        {
            clipsMusic["Level1_2"].volume = 0.5f;
        }
        else if (musicName != currentMusicName)
        {
            Debug.Log("Starting coroutine for music change!");
            StartCoroutine(FadeLevelMusic(currentMusicName, musicName));
        }
    }

    public void SetMusicVolume()
    {
        //This will get called every time the music slider moves.
        //both the volume and slider values are on a scale from 0.0f to 1.0f.
        Debug.Log("Setting music volume to " + PlayerPrefs.GetFloat("musicSliderValue"));
        clipsMusic[currentMusicName].volume = PlayerPrefs.GetFloat("musicSliderValue", 1.0f);
        volumeMusic = PlayerPrefs.GetFloat("musicSliderValue", 1.0f);
    }

    public void SetSFXVolume()
    {
        //Same concept as the music volume function.
        volumeSFX = PlayerPrefs.GetFloat("sfxSliderValue", 1.0f);
    }

    IEnumerator FadeLevelMusic(string oldTrack, string newTrack)
    {
        Debug.Log("Changing music to: " + newTrack + ".");
        float interval = clipsMusic[oldTrack].volume / 20.0f;
        for (int i = 0; i < 20; i++)
        {
            clipsMusic[oldTrack].volume -= interval;
            clipsMusic[newTrack].volume += interval;
            yield return new WaitForSeconds(0.05f);
        }
        //setting just in case.
        clipsMusic[oldTrack].volume = 0;
        clipsMusic[newTrack].volume = interval * 20.0f;

    }

    public void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "MainMenu")
        {
            Destroy(this.gameObject);
        }
    }


}
