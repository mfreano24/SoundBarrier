using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    PlayerHealth phealth;

    public float playerSpeed, gravity = -9.81f;
    public Transform groundCheck;
    public float groundDist = 0.4f;
    public LayerMask groundLM;

    public GameObject investigationPoint; //spawn this upon shooting OR decloaking, and it should last for 5 seconds.
    InvestigationPoint IP_INST;

    public bool cloaked = false, cooldown = false;
    //debug
    public Material dissolveMat; //this material should ONLY be on the player and absolutely nothing else.

    public Material wallMat; //this should be the wall material. Should be a standard checker or something like that.
    //hopefully not too much lag happens, but keep a profiler open just in case.

    public Image cloakSlider;

    float xInput, yInput;
    Vector3 moveDirection, forward, right, vel;
    CharacterController cc;
    bool isGrounded;

    [SerializeField]
    float cloakTimer = 5.0f;

    public ParticleSystem muzzlePS;
    public ParticleSystem cloakfizzle;
    public Transform gunTransform;
    [SerializeField]
    bool canFire = true;
    //public GameObject muzzleFlash;

    public bool inSwitchRange = false;
    FenceSwitchPanel currentFSP;

    //List<InvestigationPoint> spawnedPoints; //this will be for optimization's sake, i guess. dont want too many of these spawning
    public GameObject IPStorage;


    
    private void Start()
    {
        wallMat.SetFloat("Vector1_2F06040B", 1.00f);
        dissolveMat.SetFloat("Vector1_2F06040B", 0.0f); //needs to always start fully uncloaked!
        cc = GetComponent<CharacterController>();
        groundCheck.localPosition = new Vector3(0, -cc.bounds.extents.y, 0);
        phealth = GetComponent<PlayerHealth>();

    }

    private void Update()
    {
        //MOVEMENT
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundLM);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            playerSpeed = 0.3f;
        }
        else
        {
            playerSpeed = 0.15f;
        }


        if (isGrounded && vel.y < 0)
        {
            vel.y = -2;
        }
        if (!phealth.dead)
        {
            xInput = Input.GetAxis("Horizontal");
            yInput = Input.GetAxis("Vertical");
            forward = transform.forward;
            forward.y = 0;
            right = transform.right;

            moveDirection = forward * yInput + right * xInput;
            cc.Move(playerSpeed * moveDirection);

            vel.y += gravity * Time.deltaTime;

            cc.Move(vel * Time.deltaTime);
        }
        

        //STATE MANAGEMENT
        //cloak
        if (cloaked)
        {
            cloakTimer -= Time.deltaTime;
            if (cloakTimer < 0.15f)
            {
                cooldown = true;
                cloakfizzle.Play();
                StartCoroutine(decloak());
            }
        }
        else if (cloakTimer < 5.00f)
        {
            cloakTimer += Time.deltaTime;
            if(cloakTimer > 5.00f)
            {
                cloakTimer = 5.00f;
            }
        }
        else if (!cloaked && cooldown)
        {
            //if the cooldown is finished
            cooldown = false;
        }

        //UPDATE VALUES
        cloakSlider.fillAmount = cloakTimer/5.0f;

        //INPUT
        if (Input.GetMouseButtonDown(1) && !cloaked && !cooldown)
        {
            StopCoroutine(decloak()); //if in the middle of a process, interrupt it and reverse. 
            //This will allow for quick decision making on the player's part if new information becomes apparent mid-cloak.
            cloakfizzle.Play();
            StartCoroutine(cloak());
            
        }
        else if (Input.GetMouseButtonDown(1) && !cooldown)
        {
            cloakfizzle.Play();
            StopCoroutine(cloak());
            StartCoroutine(decloak());
            GameObject[] oldIPs = GameObject.FindGameObjectsWithTag("IP");
            IP_INST = Instantiate(investigationPoint, IPStorage.transform).GetComponent<InvestigationPoint>();
            IP_INST.transform.position = transform.position;
            IP_INST.AlertNearbyEnemies(5f);
            foreach(GameObject e in oldIPs)
            {
                Destroy(e);
            }
        }

        if (Input.GetMouseButtonDown(0) && canFire)
        {
            StartCoroutine(muzzleFlash());
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, groundLM))
            {
                //you're going to be indoors so yes, this will always work, unless the distance is restricted. It shouldn't be though.
                //surprise, it doesnt work on floors very well :)
                Wall wScript = hit.collider.gameObject.GetComponent<Wall>();
                Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 10.0f, Color.green, 0.5f);
                StartCoroutine(wScript.WallReaction());
                Debug.Log("successful hit!");
            }
            else
            {
                Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward, Color.red, 0.5f);
                Debug.Log("wall out of range(?)");
            }

            IP_INST = Instantiate(investigationPoint, IPStorage.transform).GetComponent<InvestigationPoint>();
            IP_INST.transform.position = transform.position;
            IP_INST.AlertNearbyEnemies(30f);
             //duh

        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            //PLACEHOLDER INPUT FOR NOW
            //WILL BE CALLED "INTERACT"

            currentFSP.HandlePlayerInteraction();
            
        }
    }


    IEnumerator cloak()
    {
        cloaked = true;
        float val = 0.01f;
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.001f);
            dissolveMat.SetFloat("Vector1_2F06040B", val);
            val += 0.01f;
        }
        

        yield return null;
    }

    IEnumerator decloak()
    {
        cloaked = false;
        float val = 0.99f;
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.01f);
            dissolveMat.SetFloat("Vector1_2F06040B", val);
            val -= 0.01f;
        }

        yield return null;

    }

    IEnumerator muzzleFlash()
    {
        canFire = false;
        //77.5 z == Gun is up
        //120 z == Gun is being reloaded.
        muzzlePS.Play();
        yield return new WaitForSeconds(0.25f);
        for(int i = 1; i <= 10; i++)
        {
            gunTransform.localRotation = Quaternion.Euler(0, 84.64f, 77.5f + i * (120 - 77.5f) / 10);
            yield return new WaitForSeconds(0.005f);
        }

        yield return new WaitForSeconds(0.3f);

        for (int i = 1; i <= 10; i++)
        {
            gunTransform.localRotation = Quaternion.Euler(0, 84.64f, 120f - i * (120 - 77.5f) / 10);
            yield return new WaitForSeconds(0.005f);
        }
        canFire = true;
    }

    public void InvestigationPointCallback(EnemyAI e)
    {
        IP_INST = Instantiate(investigationPoint, IPStorage.transform).GetComponent<InvestigationPoint>();
        IP_INST.transform.position = transform.position;
        IP_INST.AlertSpecificEnemy(e);
        //duh
    }

    public void FenceSwitchCallback(bool inRange, FenceSwitchPanel panel)
    {
        if (inRange)
        {
            Debug.Log("In range of the switch.");
            currentFSP = panel;
        }
        else
        {
            Debug.Log("Out of range of the switch.");
            currentFSP = null;
        }

        inSwitchRange = inRange;
    }

    


}
