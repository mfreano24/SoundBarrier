using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : MonoBehaviour
{

    public FenceSwitchPanel panelSwitch;

    //the color material can stay on the fences.

    Renderer r;
    Material electricity;

    public Color matColor;
    ObjectAudio aud;
    void Awake()
    {
        r = GetComponent<Renderer>();
        r.material = new Material(r.material); //copy material (just like walls)
        r.material.SetColor("_Color", matColor);
    }

    private void Start()
    {
        aud = GetComponent<ObjectAudio>();
    }

    void Update()
    {
        
    }

    public void ReceiveSwitchOFFSignal()
    {
        aud.PlaySFX("fencepowerDown");
    }

    public void ReceiveSwitchONSignal()
    {
        aud.PlaySFX("fencepowerUp");
    }




}
