using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenMaterial : MonoBehaviour
{
    public Color wallColor;
    Renderer ren;
    Material mat;
    private void Start()
    {
        ren = GetComponent<Renderer>();
        mat = ren.material;
        ren.material = new Material(mat); //create indep. copy
        ren.material.SetColor("_MaterialColor", wallColor);
    }
}
