using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    //button prompts
    public Animator buttonPrompt;
    public Text buttonPromptKey;
    public Text buttonPromptAction;

    //Next Level screen
    public Animator blackBackgroundAnimator;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        blackBackgroundAnimator.SetTrigger("EndOfLevel"); //we need to get the exit time i guess?
        //play the moving up a floor animation, whatever that may be yet LOL
    }
}
