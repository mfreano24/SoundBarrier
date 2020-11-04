using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraTutorialTrigger : MonoBehaviour
{
    [TextArea(15, 20)]
    public string[] textToDisplay;
    public Vector3 tutorialPosition;
    public Transform tutorialLookAt; //we need an object for the camera to aim at.

    public MainUI mui; //so we can disable UI

    public GameObject tutorialContainer;


    public Text textObject;
    public Image backPanel;

    bool enabled = true;
    bool tutorialInProgress = false;

    PlayerController enteredPlayer;
    CameraController playerCamera;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (enabled)
            {
                enabled = false; //this never gets turned on again!
                tutorialInProgress = true;
                enteredPlayer = other.gameObject.GetComponent<PlayerController>();
                playerCamera = other.gameObject.GetComponentsInChildren<CameraController>()[0]; //there should be one only.
                
                StartCoroutine(TutorialCameraText());
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0.5f, 0.5f);
        Gizmos.DrawSphere(tutorialPosition, 0.75f);
    }

    void ObscurePlayerObjectFromCamera()
    {
        Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << 11);
    }

    void UnobscureGun()
    {
        Camera.main.cullingMask = Camera.main.cullingMask | (1 << 11);
    }

    IEnumerator TutorialCameraText()
    {
        Debug.Log("info text triggered!");
        enteredPlayer.enabled = false; //disable movement

        //unparent the camera.
        Transform BobTransform = playerCamera.transform.parent; //store this for reassignment later.
        playerCamera.transform.parent = null; //free that camewa!
        ObscurePlayerObjectFromCamera();
        mui.ToggleHUD();
        //todo: make this fade in maybe
        
        //move the camera to the desired position with a loop!
        Vector3 starter = playerCamera.transform.position;
        playerCamera.transform.LookAt(tutorialLookAt);
        for (int i = 0; i < 50; i++)
        {
            playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, tutorialPosition, 0.1f); //may need to adjust.
            playerCamera.transform.LookAt(tutorialLookAt);
            yield return new WaitForSeconds(0.01f);
            //Debug.Log("POSITION: " + transform.position);
        }
        tutorialContainer.SetActive(true);
        Debug.Log("Finished moving the camera.");
        playerCamera.transform.position = tutorialPosition;
        playerCamera.transform.LookAt(tutorialLookAt);


        Debug.Log("Now displaying text on screen");
        
        for(int i = 0; i < textToDisplay.Length; i++)
        {
            StartCoroutine(TextDrawer(textToDisplay[i]));
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            AudioManager.singleton.PlaySFX("button-click");
            StopCoroutine(TextDrawer(textToDisplay[i]));
        }
        
        tutorialContainer.SetActive(false);
        mui.ToggleHUD();
        UnobscureGun();
        Debug.Log("returning to player control.");
        
        textObject.text = "";
        playerCamera.transform.parent = BobTransform;
        Vector3 goalTransform = new Vector3(0, 0.85f, 0f);
        
        enteredPlayer.enabled = true; //re-enable movement.

        playerCamera.transform.localPosition = goalTransform;
        playerCamera.transform.eulerAngles = Vector3.zero;
        
        yield return null;
    }
    
    IEnumerator TextDrawer(string _t)
    {
        textObject.text = "";
        for(int i = 0; i < _t.Length; i++)
        {
            textObject.text += _t[i];
            yield return new WaitForSeconds(0.05f);
        }
        yield return null;
    }
}
