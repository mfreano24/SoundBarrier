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
    public Animator resumeAnimator;
    public Animator optionsAnimator;
    public Animator quitAnimator;
    public Animator pauseTextAnimator;
    public Animator cornerPanelAnimator;
    RectTransform resume;
    RectTransform options;
    RectTransform quit;
    RectTransform pauseText;
    RectTransform cornerPanel;
    Vector3 resumeOriginalPosition;
    Vector3 optionsOriginalPosition;
    Vector3 quitOriginalPosition;
    Vector3 pauseTextOriginalPosition;
    Vector3 cornerPanelOriginalPosition;

    //settings menu
    public GameObject settingsMenu;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider mouseSensitivitySlider;
    //required for these sliders' control
    //AudioManager aud;
    //player controller
    PlayerController pc;

    [Header("Toggle HUD elements")]
    public GameObject cloakImage;
    public GameObject healthImage;
    public GameObject crosshair;
    public GameObject promptContainer;

    [HideInInspector] public bool isPaused = false;

    FootstepAudio fAud;
    private void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);
        pauseLayerScreen.SetActive(false);
        settingsMenu.SetActive(false);
        //set them all to halfway exactly.
        musicVolumeSlider.normalizedValue = PlayerPrefs.GetFloat("musicSliderValue", 0.5f);
        sfxVolumeSlider.normalizedValue = PlayerPrefs.GetFloat("sfxSliderValue", 0.5f);
        mouseSensitivitySlider.normalizedValue = PlayerPrefs.GetFloat("MSSliderValue", 0.5f);

        mouseSensitivitySlider.onValueChanged.AddListener(delegate { ChangeMouseSensitivity(); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { ChangeMusicVolume(); });
        sfxVolumeSlider.onValueChanged.AddListener(delegate { ChangeSFXVolume(); });
    }
    private void OnEnable()
    {
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
        fAud = pc.GetComponent<FootstepAudio>();
    }

    private void Start()
    {
        resume = resumeAnimator.gameObject.GetComponent<RectTransform>();
        options = optionsAnimator.gameObject.GetComponent<RectTransform>();
        quit = quitAnimator.gameObject.GetComponent<RectTransform>();
        pauseText = pauseTextAnimator.gameObject.GetComponent<RectTransform>();
        cornerPanel = cornerPanelAnimator.gameObject.GetComponent<RectTransform>();

        resumeOriginalPosition = resume.anchoredPosition;
        optionsOriginalPosition = options.anchoredPosition;
        quitOriginalPosition = quit.anchoredPosition;
        pauseTextOriginalPosition = pauseText.anchoredPosition;
        cornerPanelOriginalPosition = cornerPanel.anchoredPosition;




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
        ResetButtonsX();
        pauseLayerScreen.SetActive(!pauseLayerScreen.activeSelf);
        if (pauseLayerScreen.activeSelf)
        {
            fAud.MuteFeet();
            AudioManager.singleton.MuteMusic();
            AudioManager.singleton.PlaySFX("pause");
            Debug.Log("paused!");
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            pc.enabled = false;
            //ButtonSlideInOnPause();
            
            StartCoroutine(ButtonSlideInOnPause());
        }
        else
        {
            AudioManager.singleton.UnmuteMusic();
            AudioManager.singleton.PlaySFX("unpause");
            Debug.Log("unpaused");
            settingsMenu.SetActive(false);
            pc.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isPaused = false;
            Time.timeScale = 1;
            fAud.UnmuteFeet();
        }
    }

    void ResetButtonsX()
    {
        resume.anchoredPosition = resumeOriginalPosition;
        options.anchoredPosition = optionsOriginalPosition;
        quit.anchoredPosition = quitOriginalPosition;
        pauseText.anchoredPosition = pauseTextOriginalPosition;
        cornerPanel.anchoredPosition = cornerPanelOriginalPosition;
    }

    public void ResumeGame()
    {
        AudioManager.singleton.PlaySFX("button-click");
        if (!settingsMenu.activeSelf) //these buttons shouldnt work while in settings mode, otherwise the player could resume play while in the setting menu. bad.
        {
            TogglePauseScreen();
        }
    }

    public void OpenSettingsMenu()
    {
        AudioManager.singleton.PlaySFX("button-click");
        settingsMenu.SetActive(true); //we out here
    }

    public void QuitToMainMenu()
    {
        AudioManager.singleton.PlaySFX("button-click");
        if (!settingsMenu.activeSelf)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void ExitSettingsMenu()
    {
        settingsMenu.SetActive(false);
    }


    public void ToggleHUD(bool _toggleTo)
    {
        Debug.Log("Toggling main HUD elements to " + _toggleTo + ".");
        cloakImage.SetActive(_toggleTo);
        healthImage.SetActive(_toggleTo);
        crosshair.SetActive(_toggleTo);
        //promptContainer.SetActive(!_toggleTo);
    }

    void ChangeMusicVolume()
    {
        PlayerPrefs.SetFloat("musicSliderValue", musicVolumeSlider.normalizedValue);
        AudioManager.singleton.SetMusicVolume();
    }
    
    void ChangeSFXVolume()
    {
        PlayerPrefs.SetFloat("sfxSliderValue", sfxVolumeSlider.normalizedValue);
        AudioManager.singleton.SetSFXVolume();

    }

    void ChangeMouseSensitivity()
    {
        Debug.Log("Setting mouse sensitivity to VALUE " + 150 * (mouseSensitivitySlider.normalizedValue + 0.01f) + ".");
        PlayerPrefs.SetFloat("MSSliderValue", mouseSensitivitySlider.normalizedValue);
        PlayerPrefs.SetFloat("MouseSensitivity", 150 * (mouseSensitivitySlider.normalizedValue + 0.01f));
    }

    public void SetFloorNumberText(int _floorNumber)
    {
        Debug.Log("Floor number text has been set!");
        StartCoroutine(beginning.SetFloorNumberText(_floorNumber));
        StartCoroutine(end.SetFloorNumberText(_floorNumber));
    }

    public void ButtonClickSound()
    {
        AudioManager.singleton.PlaySFX("button-click");
    }

    IEnumerator ButtonSlideInOnPause()
    {
        Debug.Log("SLIDE IN THE BUTTONS");
        resumeAnimator.SetTrigger("SlideIn");
        yield return new WaitForSeconds(0.05f);
        optionsAnimator.SetTrigger("SlideIn");
        yield return new WaitForSeconds(0.05f);
        quitAnimator.SetTrigger("SlideIn");
    }

}
