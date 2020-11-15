using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraTutorialTrigger : MonoBehaviour
{
    public ChaseBot chaseBotEnemy;
    [TextArea(15, 20)]
    public string textToDisplay;
    public Vector3 tutorialPosition;
    public Transform tutorialLookAt; //we need an object for the camera to aim at.

    public MainUI mui; //so we can disable UI

    public GameObject tutorialContainer;


    public Text textObject;
    public Image backPanel;

    bool enabled = true;
    bool tutorialInProgress = false;

    bool textDrawingInProgress = false;

    PlayerController enteredPlayer;
    CameraController playerCamera;
    ObjectAudio aud;
    private void Start()
    {
        chaseBotEnemy.gameObject.SetActive(false); //backup just in case
        aud = GetComponent<ObjectAudio>();
    }
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
        Debug.Log("Obscuring gun from camera view.");
        Camera.main.cullingMask = Camera.main.cullingMask & ~(1 << 11);
    }

    void UnobscureGun()
    {
        Debug.Log("Unobscuring gun from camera view.");
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
        mui.ToggleHUD(false);
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
        //tutorialContainer.SetActive(true);
        Debug.Log("Finished moving the camera.");
        playerCamera.transform.position = tutorialPosition;
        playerCamera.transform.LookAt(tutorialLookAt);

        chaseBotEnemy.gameObject.SetActive(true);
        aud.PlaySFX("Poof"); //cant do it on the enemy because it doesn't initialize quite right...
        yield return new WaitForSeconds(2.0f);


        mui.ToggleHUD(true); 
        UnobscureGun();

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
        for (int i = 0; i < _t.Length; i++)
        {
            textObject.text += _t[i];
            yield return new WaitForSeconds(0.05f);
        }
        textDrawingInProgress = false; //this should trigger the WaitWhile lambda function in the parent thread.
        yield return null;
    }
}
