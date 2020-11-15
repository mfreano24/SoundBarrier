using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class intromusic : MonoBehaviour
{
    AudioSource vol;
    AudioSource buttonClick;
    bool fullDisable = false;
    private void Awake()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        vol = sources[0];
        buttonClick = sources[1];
        buttonClick.volume = PlayerPrefs.GetFloat("sfxSliderValue", 0.5f);
        vol.volume = PlayerPrefs.GetFloat("musicSliderValue", 0.5f);
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoad;
    }


    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name.StartsWith("Level") && !fullDisable)
        {
            fullDisable = true;
            buttonClick.volume = 0.0f;
            vol.volume = 0.0f;
            this.gameObject.SetActive(false);
        }
    }

    public void ButtonClickSFX()
    {
        buttonClick.volume = PlayerPrefs.GetFloat("sfxSliderValue", 0.5f);
        buttonClick.Play();
    }




}
