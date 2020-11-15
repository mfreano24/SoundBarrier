using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FenceSwitchPanel : MonoBehaviour
{
    Material fenceMat;
    public GameObject fence; //NOTE: Adapt this for multiple fences at once so that it becomes more of a switch.
    Fence f;
    Renderer r;

    PlayerController pc;

    Color fenceOn;

    public MainUI mui;

    ObjectAudio aud;

    void Start()
    {
        r = GetComponent<Renderer>();
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        fenceMat = fence.GetComponent<Renderer>().material;
        r.material.color = fenceMat.GetColor("_Color"); //set the color of this switch to the fence's electricity color!
        fenceOn = r.material.color; //store the original color of the fence for later.
        f = fence.GetComponent<Fence>();
        aud = GetComponent<ObjectAudio>();
    }

    void Update()
    {
        
    }

    public void EnableFence()
    {
        r.material.color = fenceOn;
        StartCoroutine(TurnOn());
    }

    public IEnumerator TurnOn()
    {
        f.ReceiveSwitchONSignal();
        fence.SetActive(true);
        for (int i = 0; i < 30; i++)
        {
            fenceMat.SetFloat("_Strength", fenceMat.GetFloat("_Strength") + 0.1f);
            fenceMat.SetFloat("_Brightness", fenceMat.GetFloat("_Brightness") + 0.01f);
            yield return new WaitForSeconds(0.05f);
        }

        fenceMat.SetFloat("_Brightness", 2);

    }

    public void DisableFence()
    {
        r.material.color = Color.red;
        StartCoroutine(TurnOff());
    }

    public IEnumerator TurnOff()
    {
        f.ReceiveSwitchOFFSignal();
        for (int i = 0; i < 30; i++)
        {
            fenceMat.SetFloat("_Strength", fenceMat.GetFloat("_Strength") - 0.1f);
            fenceMat.SetFloat("_Brightness", fenceMat.GetFloat("_Brightness") - 0.1f);
            yield return new WaitForSeconds(0.05f);
        }
        fence.SetActive(false);
    }

    public void HandlePlayerInteraction()
    {
        aud.PlaySFX("ButtonPress");
        if (fence.activeInHierarchy)
        {
            Debug.Log("Turning off fence...");
            DisableFence();
        }
        else
        {
            Debug.Log("Turning on fence...");
            EnableFence();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mui.BringInNewButtonPrompt("E", "Press Button");
            pc.FenceSwitchCallback(true, this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mui.SlideButtonPromptOut();
            pc.FenceSwitchCallback(false, null);
        }
    }
}
