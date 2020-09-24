using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fence : MonoBehaviour
{

    public FenceSwitchPanel panelSwitch;

    //the color material can stay on the fences.

    Renderer r;
    Material electricity;

    Color matColor;
    
    void Start()
    {
        r = GetComponent<Renderer>();
        electricity = r.material;
        electricity.SetFloat("_Brightness", 2f);
        electricity.SetFloat("_Strength", 5f);
        matColor = electricity.GetColor("_Color");
    }

    void Update()
    {
        
    }

    public void SendInformationOver()
    {

    }

    

    
}
