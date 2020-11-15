using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Outro : MonoBehaviour
{
    public Material wallMat;

    public Animator carAnim;
    public Animator blackScreenAnim;
    public Animator creditsAnim;

    public Text textObj;

    public ParticleSystem carDust;

    bool TowerAlmostFinished = false;

    AudioSource typewriter;

    private void Start()
    {
        typewriter = GetComponent<AudioSource>();
        typewriter.volume = 0.5f;
        textObj.text = "";
        wallMat.SetFloat("Vector1_2F06040B", 1.0f);
        StartCoroutine(EndingSequence());
    }

    private void Update()
    {
    }

    public IEnumerator TowerDissolve()
    {
        //dissolve in the intro sequence?
        float val = 1.0f;
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.075f); //take a little longer than usual probably
            wallMat.SetFloat("Vector1_2F06040B", val);
            val -= 0.01f;
            if (val <= 0.75f && !TowerAlmostFinished)
            {
                TowerAlmostFinished = true;
            }
        }
        wallMat.SetFloat("Vector1_2F06040B", 0.0f);
    }

    IEnumerator EndingSequence()
    {
        //FADE IN FROM BLACK SCREEN - PROBABLY LIKE 2 SECONDS
        blackScreenAnim.SetTrigger("FadeAway");
        yield return new WaitForSeconds(2.0f);
        //MAYBE LIKE SOME TEXT? IDK KING DO WHAT U WANT
        //MOVE THE CAR
        StartCoroutine(DrawText());
        StartCoroutine(TowerDissolve());
        carAnim.SetTrigger("MoveCar");
        carDust.Play();
        yield return new WaitForSeconds(1.5f); //take much less time for this since we can already see the car
         //dissolve the tower as the car is pulling up.
        yield return new WaitUntil(() => TowerAlmostFinished);
        //when the tower's almost finished, we can begin fading into floor 1
        blackScreenAnim.SetTrigger("FadeIn");
        yield return new WaitForSeconds(2.0f);
        //set credits trigger
        creditsAnim.SetTrigger("RollCredits");
        yield return new WaitForSeconds(15.0f);
        SceneManager.LoadScene("MainMenu"); //Back to menu, pray to lucifer that the thing works
    }


    IEnumerator DrawText()
    {
        string textToDraw = "Mission Accomplished, Agent.";
        for (int i = 0; i < textToDraw.Length; i++)
        {
            textObj.text += textToDraw[i];
            typewriter.Play();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
