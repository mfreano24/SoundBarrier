using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Renderer menuEffectMat;

    public GameObject creditsPanel;

    public Animator blackFadeAnim;

    public intromusic im;

    public GameObject settingsMenu;

    [Header("Sliders")]
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider mouseSensitivitySlider;
    


    private void Awake()
    {
        //blackFadeAnim.gameObject.SetActive(false); //dont need to bother with fading if its too complicated
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Time.timeScale = 1; //fix from pause screen

    }

    private void Update()
    {
        Debug.Log("UPDATE MAINMENU()");
        menuEffectMat.material.SetFloat("Vector1_2F06040B", 0.25f * (Mathf.Sin(2*Time.time)+2));
    }

    private void Start()
    {
        musicVolumeSlider.normalizedValue = PlayerPrefs.GetFloat("musicSliderValue", 0.5f);
        sfxVolumeSlider.normalizedValue = PlayerPrefs.GetFloat("sfxSliderValue", 0.5f);
        mouseSensitivitySlider.normalizedValue = PlayerPrefs.GetFloat("MSSliderValue", 0.5f);
        PlayerPrefs.SetFloat("MouseSensitivity", 150 * (mouseSensitivitySlider.normalizedValue + 0.01f));

        musicVolumeSlider.onValueChanged.AddListener(delegate { ChangeMusicVolume(); });
        sfxVolumeSlider.onValueChanged.AddListener(delegate { ChangeSFXVolume(); });
        mouseSensitivitySlider.onValueChanged.AddListener(delegate { ChangeMouseSensitivity(); });
    }

    public void StartGame()
    {
        //blackFadeAnim.gameObject.SetActive(true);
        SceneManager.LoadScene("IntroScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void EnableCreditsPanel()
    {

        creditsPanel.SetActive(true);
    }

    public void DisableCreditsPanel()
    {
        creditsPanel.SetActive(false);
    }


    IEnumerator FadeIntoIntro()
    {
        blackFadeAnim.SetTrigger("FadeIn");
        yield return new WaitForSeconds(1.25f); //NOTE: may need to adjust this one?
    }


    public void OpenSettingsMenu()
    {
        settingsMenu.SetActive(true); //we out here
    }

    public void ExitSettingsMenu()
    {
        settingsMenu.SetActive(false);
    }


    void ChangeMusicVolume()
    {
        PlayerPrefs.SetFloat("musicSliderValue", musicVolumeSlider.normalizedValue);
        im.SetMusicVolume();
    }

    void ChangeSFXVolume()
    {
        PlayerPrefs.SetFloat("sfxSliderValue", sfxVolumeSlider.normalizedValue);

    }

    void ChangeMouseSensitivity()
    {
        Debug.Log("Setting mouse sensitivity to VALUE " + 150 * (mouseSensitivitySlider.normalizedValue + 0.01f) + ".");
        PlayerPrefs.SetFloat("MSSliderValue", mouseSensitivitySlider.normalizedValue);
        PlayerPrefs.SetFloat("MouseSensitivity", 150 * (mouseSensitivitySlider.normalizedValue + 0.01f));
    }
}
