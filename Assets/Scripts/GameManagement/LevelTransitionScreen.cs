using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelTransitionScreen : MonoBehaviour
{
    public Text FLOOR;
    public Text floorNumber;
    public Image playerMarker;
    CanvasGroup thisCanvasGroup;
    Image backingPanel;
    int currentFloor;
    string SceneNumber;

    private void OnEnable()
    {
        SceneNumber = "" + SceneManager.GetActiveScene().name.ToString()[SceneManager.GetActiveScene().name.Length - 1];
        Debug.Log("Starting Level # " + SceneNumber);
        //if you're reading this im sorry
        if(SceneNumber != "8")
        {
            floorNumber.text = SceneNumber;
            Debug.Log("Floornumber text = " + floorNumber.text);
        }
        else
        {
            floorNumber.text = "";
            FLOOR.text = "Rooftop";
        }

        

        int i = Int32.Parse(SceneNumber) - 1;
        playerMarker.rectTransform.anchoredPosition = new Vector2(206, -274 + (i * 65));
        Debug.Log("SETTING PLAYERMARKER POS TO " + playerMarker.rectTransform.anchoredPosition);
        thisCanvasGroup = GetComponent<CanvasGroup>();
        backingPanel = GetComponent<Image>();
        if (gameObject.name == "Background_LevelAdvance")
        {
            //this should be fully invisible to start
            Debug.Log("starting level - Leveltransitionscreen");
            thisCanvasGroup.alpha = 0;

        }
        else
        {
            thisCanvasGroup.alpha = 1;
            StartCoroutine(BeginningOfLevel());
            //this should begin visible then fade
        }

    }

    IEnumerator BeginningOfLevel()
    {
        Color panelColor = backingPanel.color;
        yield return new WaitForSeconds(1.5f); //hold the screen
        for (int i = 1; i <= 20; i++)
        {
            backingPanel.color = panelColor;
            panelColor.a = 1.0f - (i * 0.05f);
            thisCanvasGroup.alpha = 1.0f - (i * 0.05f);
            yield return new WaitForSeconds(.05f);
        }
        thisCanvasGroup.alpha = 0.0f; //just to make sure :)
    }

    public IEnumerator EndOfLevel()
    {
        if(SceneNumber != "8")
        {
            Color panelColor = backingPanel.color;
            Debug.Log("EndOfLevel transition is playing...");
            for (int i = 1; i <= 20; i++)
            {

                panelColor.a = i * 0.05f;
                backingPanel.color = panelColor;
                thisCanvasGroup.alpha = i * 0.05f;
                yield return new WaitForSeconds(.05f);
            }
            thisCanvasGroup.alpha = 1.0f; //just to make sure :)
            yield return new WaitForSeconds(1.5f); //hold the screen
        }
        else
        {
            Color panelColor = backingPanel.color;
            Debug.Log("EndOfLevel transition is playing...");
            for (int i = 1; i <= 20; i++)
            {

                panelColor.a = i * 0.05f;
                backingPanel.color = panelColor;
                //thisCanvasGroup.alpha = i * 0.05f;
                yield return new WaitForSeconds(.05f);
            }
        }
        
    }

    public IEnumerator SetFloorNumberText(int floornum)
    {
        if (floornum < 8)
        {
            yield return new WaitForSeconds(1.25f);
            floorNumber.text = "";
            yield return new WaitForSeconds(.5f);
            floorNumber.text = floornum.ToString();
            float beginningYPos = playerMarker.rectTransform.anchoredPosition.y;
            for (int i = 1; i <= 40; i++)
            {
                playerMarker.rectTransform.anchoredPosition = new Vector2(206, beginningYPos + (0.025f * i) * 65);
                yield return new WaitForSeconds(.025f);
            }
        }
        else
        {
            yield return new WaitForSeconds(1.25f);
            floorNumber.text = "";
            FLOOR.text = "";
            yield return new WaitForSeconds(.5f);
            float beginningYPos = playerMarker.rectTransform.anchoredPosition.y;
            for (int i = 0; i < 40; i++)
            {
                if (i % 5 == 0)
                {
                    FLOOR.text += "Rooftop"[i / 5];
                }

                playerMarker.rectTransform.anchoredPosition = new Vector2(206, beginningYPos + (0.025f * (i + 1)) * 65); //kind of messy
                yield return new WaitForSeconds(.025f);
            }
        }


    }
}
