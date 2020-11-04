using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prompt : MonoBehaviour
{
    [Header("Components of the UI Object.")]
    public Text promptActionText;
    public Image actionIcon;

    [Header("Things to put in those components.")]
    public string actionPrompt;
    public Sprite iconToDisplay; //may need to be an image? not sure.
}
