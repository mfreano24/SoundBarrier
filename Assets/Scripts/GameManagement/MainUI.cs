using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainUI : MonoBehaviour
{
    //button prompts
    public Animator buttonPrompt;
    public Text buttonPromptKey;
    public Text buttonPromptAction;

    //Next Level/Beginning transition screens
    public LevelTransitionScreen beginning;
    public LevelTransitionScreen end;
    

    //Pause layer
    public GameObject pauseLayerScreen;

    //settings menu
    public GameObject settingsMenu;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider mouseSensitivitySlider;
    //required for these sliders' control
    //AudioManager aud;

    //player controller
    PlayerController pc;


    private void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
        pauseLayerScreen.SetActive(false);
        settingsMenu.SetActive(false);
        //set them all to halfway exactly.
        musicVolumeSlider.normalizedValue = 0.5f;
        sfxVolumeSlider.normalizedValue = 0.5f;
        mouseSensitivitySlider.normalizedValue = 0.5f;
        mouseSensitivitySlider.onValueChanged.AddListener(delegate { ChangeMouseSensitivity(); });
    }

    void Start()
    {
       
    }

    private void OnEnable()
    {
        
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
     
    }

    // Update is called once per frame
    void Update()
    {
        //may need to toggle this screen from another script if this script is going to become a singleton.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseScreen();
        }
        
    }

    public void BringInNewButtonPrompt(string keyToPress, string action)
    {
        buttonPromptKey.text = keyToPress;
        buttonPromptAction.text = action;
        buttonPrompt.SetBool("ButtonPrompt", true);
    }

    public void SlideButtonPromptOut()
    {
        buttonPrompt.SetBool("ButtonPrompt", false);
    }
     
    public void ExitFound()
    {
        StartCoroutine(end.EndOfLevel());
        //play the moving up a floor animation, whatever that may be yet LOL
    }

    void TogglePauseScreen()
    {
        //
        pauseLayerScreen.SetActive(!pauseLayerScreen.activeSelf);
        if (pauseLayerScreen.activeSelf)
        {
            Debug.Log("paused!");
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            pc.enabled = false;
        }
        else
        {
            Debug.Log("unpaused");
            settingsMenu.SetActive(false);
            pc.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1;
        }
    }

    public void ResumeGame()
    {
        if (!settingsMenu.activeSelf) //these buttons shouldnt work while in settings mode, otherwise the player could resume play while in the setting menu. bad.
        {
            TogglePauseScreen();
        }
         
    }

    public void OpenSettingsMenu()
    {
        settingsMenu.SetActive(true); //we out here
    }

    public void QuitToMainMenu()
    {
        if (!settingsMenu.activeSelf)
        {
            SceneManager.LoadScene("MainMenu");
        }
        
    }

    public void ExitSettingsMenu()
    {
        settingsMenu.SetActive(false);
    }

    void ChangeMusicVolume()
    {
        //TBD
    }
    
    void ChangeSFXVolume()
    {
        //TBD
    }

    void ChangeMouseSensitivity()
    {
        Debug.Log("Setting mouse sensitivity to VALUE " + 150 * (mouseSensitivitySlider.normalizedValue + 0.01f) + ".");
        PlayerPrefs.SetFloat("MouseSensitivity", 150 * (mouseSensitivitySlider.normalizedValue + 0.01f));
    }

    public void SetFloorNumberText(int _floorNumber)
    {
        Debug.Log("Floor number text has been set!");
        StartCoroutine(beginning.SetFloorNumberText(_floorNumber));
        StartCoroutine(end.SetFloorNumberText(_floorNumber));
    }

}
