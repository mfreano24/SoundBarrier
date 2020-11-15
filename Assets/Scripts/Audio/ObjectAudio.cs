using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAudio : MonoBehaviour
{
    //list/dict storage model - makes it easier to use this the same way as the singleton
    public List<AudioClip> clipsInspector;
    Dictionary<string, AudioClip> sfxClips;
    Dictionary<string, AudioSource> sfxSources;

    public bool isDiegetic = false;

    float volume;
    private void Start()
    {
        volume = PlayerPrefs.GetFloat("sfxSliderValue", 1.0f); //sure hope this is the one
        //instantiate
        sfxClips = new Dictionary<string, AudioClip>();
        sfxSources = new Dictionary<string, AudioSource>();
        //assign
        foreach(AudioClip c in clipsInspector)
        {
            sfxClips.Add(c.name, c);
        }
        Debug.Log("OBJECT AUDIO INIT ON GAMEOBJECT " + gameObject.name);
    }


    //dont clean these up, make it more like lazy instantiation instead? most objects wont have that many sound effects and this is only for one timers and not loops.
    public void PlaySFX(string sfxName)
    {
        if (sfxSources.ContainsKey(sfxName))
        {
            //if the sound is already being played we could just replay it(?)
            //this may need some testing to ensure it doesn't get caught by the cleanup process.
            Debug.Log("volume = " + volume);
            sfxSources[sfxName].volume = volume;
            sfxSources[sfxName].Play();
        }
        else
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            if (isDiegetic)
            {
                newSource.rolloffMode = AudioRolloffMode.Linear;
                newSource.spatialBlend = 1.0f;
                newSource.dopplerLevel = 0.0f;
                newSource.maxDistance = 30 * Mathf.Sqrt(2);
                newSource.minDistance = 1;
            }
            

            newSource.clip = sfxClips[sfxName];
            Debug.Log("CREATED NEW AUDIO SOURCE FOR CLIP " + sfxName);
            newSource.volume = volume;
            //Play the sound before adding to the dictionary, so that cleanup doesn't catch it before it even starts playing.
            newSource.Play();
            sfxSources.Add(sfxName, newSource);
        }
    }
}
