using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelTransitionScreen : MonoBehaviour
{
    public Text floorNumber;
    public Image playerMarker;
    CanvasGroup thisCanvasGroup;
    Image backingPanel;
    int currentFloor;

    private void OnEnable()
    {
        string SceneNumber = "" + SceneManager.GetActiveScene().name.ToString()[SceneManager.GetActiveScene().name.Length - 1];
        Debug.Log("Starting Level # " + SceneNumber);
        //if you're reading this im sorry
        floorNumber.text = SceneNumber;
        Debug.Log("Floornumber text = " + floorNumber.text);

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
        for(int i = 1; i <= 20; i++)
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

    public IEnumerator SetFloorNumberText(int floornum)
    {
        //little cinematic effect here.
        yield return new WaitForSeconds(1.25f);
        floorNumber.text = "";
        yield return new WaitForSeconds(.5f);
        floorNumber.text = floornum.ToString();
        yield return new WaitForSeconds(.5f);
        float beginningYPos = playerMarker.rectTransform.anchoredPosition.y;
        for (int i = 1; i <= 40; i++)
        {
            playerMarker.rectTransform.anchoredPosition = new Vector2(206, beginningYPos + (0.025f * i) * 65);
            yield return new WaitForSeconds(.025f);
        }
    }
}
