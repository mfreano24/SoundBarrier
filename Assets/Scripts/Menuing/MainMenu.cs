using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Renderer menuEffectMat;

    public GameObject creditsPanel;

    public Animator blackFadeAnim;

    private void Start()
    {
        //blackFadeAnim.gameObject.SetActive(false); //dont need to bother with fading if its too complicated
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        menuEffectMat.material.SetFloat("Vector1_2F06040B", 0.25f * (Mathf.Sin(2*Time.time)+2));
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
}
