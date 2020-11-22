using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPrompts : MonoBehaviour
{

    Dictionary<string, Animator> prompts; //these should be prompts to do.
    int actionsCompleted = 0;
    private void Start()
    {
        prompts = new Dictionary<string, Animator>();
        for(int i = 0; i < transform.childCount; i++)
        {
            prompts.Add(transform.GetChild(i).GetComponent<Prompt>().actionPrompt, transform.GetChild(i).GetComponent<Animator>());
        }
    }

    private void Update()
    {
        
        if(prompts.Count > 0 && Time.timeSinceLevelLoad > 5)
        {
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                if (prompts.ContainsKey("MOVE"))
                {
                    prompts["MOVE"].SetTrigger("TutorialDone");
                    prompts.Remove("MOVE");
                }

            }

            if (Input.GetMouseButtonDown(0))
            {
                if (prompts.ContainsKey("SHOOT"))
                {
                    prompts["SHOOT"].SetTrigger("TutorialDone");
                    prompts.Remove("SHOOT");
                }

            }



            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("Cloak prompt removed.");
                if (prompts.ContainsKey("CLOAK"))
                {
                    prompts["CLOAK"].SetTrigger("TutorialDone");
                    prompts.Remove("CLOAK");
                }

            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (prompts.ContainsKey("SPRINT"))
                {
                    prompts["SPRINT"].SetTrigger("TutorialDone");
                    prompts.Remove("SPRINT");
                }
            }


        }
        
    }
}
