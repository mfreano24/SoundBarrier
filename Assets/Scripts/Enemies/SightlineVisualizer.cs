using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SightlineVisualizer : MonoBehaviour
{
    public LineRenderer left;
    public LineRenderer right;
    public LayerMask lm;
    public Renderer domeRenderer;
    public Light robotLight;

    Color currentColor;
    public Color CurrentColor
    {
        get
        {
            return currentColor;
        }
        set
        {
            StartCoroutine(FadeColor(value));   
        }
    }
    
    void Start()
    {
        left.positionCount = 2;
        right.positionCount = 2;
        left.widthMultiplier = 0.2f;
        right.widthMultiplier = 0.2f;

    }

    
    void Update()
    {
        left.SetPosition(0, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z));
        right.SetPosition(0, new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z));
        //render a line that goes until it hits a wall?
        RenderLine(-45f,left);
        RenderLine(45f,right);
        /* debug color changing.
        if (Input.GetKeyDown(KeyCode.R))
        { 
            CurrentColor = Color.cyan;
        }
        */
    }

    public void RenderLine(float angleDisplacement, LineRenderer side)
    {
        side.SetPosition(1, 10.0f*transform.TransformDirection(new Vector3(angleDisplacement/45f, 0, 1)) + transform.position);
        //side.transform.localRotation = Quaternion.AngleAxis(angleDisplacement, Vector3.up);
        //Color sideCheck = angleDisplacement < 0 ? Color.yellow : Color.blue;
    }

    IEnumerator FadeColor(Color newColor)
    {
        for(int i = 0; i < 20; i++)
        {
            domeRenderer.material.color = Color.Lerp(currentColor, newColor, 1);
            robotLight.color = Color.Lerp(currentColor, newColor, 1);
            yield return new WaitForSeconds(0.1f);
        }
        
        currentColor = newColor;
    }
}
