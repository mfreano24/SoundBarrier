using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Opening_Closing_FMV : MonoBehaviour
{
    public Material wallMat;

    public Animator carAnim;
    public Animator blackScreenAnim;

    bool TowerAlmostFinished = false;
    [TextArea(5, 20)]
    public string textToDraw;
    public Text textObj;

    AudioSource typewriter;

    public ParticleSystem dust;
    AudioSource mus;
    private void Start()
    {
        mus = GameObject.Find("IntroMusic").GetComponent<AudioSource>();
        typewriter = GetComponent<AudioSource>();
        typewriter.volume = 0.5f;
        textObj.text = "";
        wallMat.SetFloat("Vector1_2F06040B", 0.0f);
        StartCoroutine(OpeningSequence());
    }

    private void Update()
    {
    }

    public IEnumerator TowerDissolve()
    {
        //dissolve in the intro sequence?
        float val = 0.0f;
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.05f); //take a little longer than usual probably
            wallMat.SetFloat("Vector1_2F06040B", val);
            val += 0.01f;
            if (val >= 0.75f && !TowerAlmostFinished)
            {
                TowerAlmostFinished = true;
            }
        }
        wallMat.SetFloat("Vector1_2F06040B", 1.0f);
    }

    IEnumerator OpeningSequence()
    {
        //FADE IN FROM BLACK SCREEN - PROBABLY LIKE 2 SECONDS
        blackScreenAnim.SetTrigger("FadeAway");
        yield return new WaitForSeconds(2.0f);
        //MAYBE LIKE SOME TEXT? IDK KING DO WHAT U WANT
        //MOVE THE CAR
        carAnim.SetTrigger("MoveCar");
        StartCoroutine(DrawText());
        yield return new WaitForSeconds(6.5f);
        StartCoroutine(TowerDissolve()); //dissolve the tower as the car is pulling up.
        //possibly need a wait here?
        dust.Stop();
        
        yield return new WaitUntil(() => TowerAlmostFinished);
        blackScreenAnim.SetTrigger("FadeIn");
        float step = mus.volume / 20.0f;
        for (int i = 0; i < 20; i++)
        {
            mus.volume -= step;
            yield return new WaitForSeconds(0.1f);
        }
        mus.volume = 0;

        //when the tower's almost finished, we can begin fading into floor 1
        
        
        SceneManager.LoadScene("Level1");
    }

    IEnumerator DrawText()
    {
        for(int i = 0; i < textToDraw.Length; i++)
        {
            typewriter.Play();
            textObj.text += textToDraw[i];
            yield return new WaitForSeconds(0.1f);
        }
    }
}
