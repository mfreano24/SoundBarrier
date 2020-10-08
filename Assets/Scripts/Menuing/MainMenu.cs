using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Renderer menuEffectMat;

    private void Start()
    {
        
    }

    private void Update()
    {
        menuEffectMat.material.SetFloat("Vector1_2F06040B", 0.25f * (Mathf.Sin(2*Time.time)+2));
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level1"); //begin this shitshow
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
