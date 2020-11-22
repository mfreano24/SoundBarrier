using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //todo: clean this up please
    [Header("Player Movement and GroundCheck")]
    public CameraController cameraCon;
    PlayerHealth phealth;
    public float playerSpeed, gravity = -9.81f;
    public Transform groundCheck;
    public float groundDist = 0.4f;
    public LayerMask groundLM;
    float xInput, yInput;
    Vector3 moveDirection, forward, right, vel;
    CharacterController cc;
    bool isGrounded;
    [SerializeField]
    float cloakTimer = 7.5f;

    [Header("EnemyRelations")]
    public IPManager IP;

    public bool cloaked = false, cooldown = false, emptyRecover = false;
    //debug
    public Material dissolveMat; //this material should ONLY be on the player and absolutely nothing else.

    public Material wallMat; //this should be the wall material. Should be a standard checker or something like that.
    //hopefully not too much lag happens, but keep a profiler open just in case.

    public Image cloakSlider;

    bool decloakInProgress = false;
    bool cloakInProgress = true;


    [Header("Extraneous Objects")]
    public ParticleSystem muzzlePS;
    public ParticleSystem cloakfizzle;
    public ParticleSystem wallHitPS;
    public Transform gunTransform;
    public Animator gunAnim;

    [Header("State Booleans")]
    [HideInInspector] public bool moving = false;
    bool canFire = true;
    bool sprinting = false;
    bool enemyAlerted = false;

    public bool inSwitchRange = false;
    FenceSwitchPanel currentFSP;

    //AudioSource aud; //walking/running
    ObjectAudio aud;

    float sprintActivateStep = 2.5f;
    float sprintActivateCurrent = 0.0f;

    List<Coroutine> cloakingCoroutines;

    private void OnEnable()
    {
        cameraCon.enabled = true;
    }

    private void OnDisable()
    {
        cameraCon.enabled = false;
    }

    private void Start()
    {
        wallMat.SetFloat("Vector1_2F06040B", 1.00f);
        dissolveMat.SetFloat("Vector1_2F06040B", 0.0f); //needs to always start fully uncloaked!
        cc = GetComponent<CharacterController>();
        groundCheck.localPosition = new Vector3(0, -cc.bounds.extents.y, 0);
        phealth = GetComponent<PlayerHealth>();
        aud = GetComponent<ObjectAudio>();
        cloakingCoroutines = new List<Coroutine>();

    }

    private void Update()
    {
        //MOVEMENT
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundLM);

        if (Input.GetKey(KeyCode.LeftShift) && moving)
        {
            //check if we're moving as well because otherwise you could alert an enemy standing still???????/
            //NOTE: work on this- it works for now tho lmao
            if (canFire && !sprinting)
            {
                //BEGINNING OF SPRINT
                gunAnim.SetTrigger("SprintStart");
                canFire = false;
                sprinting = true;
            }


            playerSpeed = 12.5f;
        }
        else
        {
            if (!canFire && sprinting)
            {
                gunAnim.SetTrigger("SprintEnd");
                canFire = true;
                sprinting = false;
                sprintActivateCurrent = 2.5f; //set to step so that we can immediately alert an anemy next time(???????)
                //issue: if the player stops sprintng and starts again this triggers multiple alerts
            }

            playerSpeed = 7.5f;
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
            if (moveDirection != Vector3.zero)
            {
                moving = true;
            }
            else
            {
                moving = false;
            }
            cc.Move(playerSpeed * moveDirection * Time.deltaTime);

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
                emptyRecover = true;
                cooldown = true;
                cloakfizzle.Play();
                //LAST MINUTE CHANGE HENCE THE TWO DIFFERENT AUDIO OBJECTS
                aud.PlaySFX("decloak");
                AudioManager.singleton.PlaySFX("unpause",2f);
                foreach (Coroutine c in cloakingCoroutines)
                {
                    StopCoroutine(c);
                }
                cloakingCoroutines.Clear();
                cloakingCoroutines.Add(StartCoroutine(decloak(true)));
            }
        }
        else if (cloakTimer < 5.0f)
        {
            cloakTimer += Time.deltaTime;
            if (cloakTimer > 5.0f)
            {
                if (emptyRecover)
                {
                    AudioManager.singleton.PlaySFX("pause", .75f);
                    emptyRecover = false;
                }
                cloakTimer = 5.0f;
            }
        }
        else if (!cloaked && cooldown)
        {
            //if the cooldown is finished
            cooldown = false;
        }
        else if(cloakTimer > 5.0f)
        {
            cloakTimer = 5.0f;
            //fallback
        }

        //UPDATE VALUES
        cloakSlider.fillAmount = cloakTimer / 5.0f;
        if(cloakSlider.fillAmount >= 1.0f)
        {
            cloakSlider.color = new Color(0, 1, 1);
        }

        //INPUT
        if (Input.GetMouseButtonDown(1) && !cloaked && !cooldown)
        {
            //StopCoroutine(decloak()); //if in the middle of a process, interrupt it and reverse. 
            //This will allow for quick decision making on the player's part if new information becomes apparent mid-cloak.
            cloakfizzle.Play();
            aud.PlaySFX("cloak");
            foreach (Coroutine c in cloakingCoroutines)
            {
                StopCoroutine(c);
            }
            cloakingCoroutines.Clear();
            cloakingCoroutines.Add(StartCoroutine(cloak()));

        }
        else if (Input.GetMouseButtonDown(1) && !cooldown)
        {
            cloakfizzle.Play();
            aud.PlaySFX("decloak");
            foreach (Coroutine c in cloakingCoroutines)
            {
                StopCoroutine(c);
            }
            cloakingCoroutines.Clear();
            cloakingCoroutines.Add(StartCoroutine(decloak(false)));
            //investigation points

            IP.NoiseActivate(5, transform.position);

        }

        if (Input.GetMouseButtonDown(0) && canFire)
        {
            AudioManager.singleton.PlaySFX("GunFire", 0.5f);
            StartCoroutine(muzzleFlash());
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, groundLM))
            {
                //THIS HAS GOT TO BE THE SINGLE DUMBEST BUG IVE EVER ENCOUNTERED.
                //Debug.Log("Ground Layermask: " + groundLM);
                //you're going to be indoors so yes, this will always work, unless the distance is restricted. It shouldn't be though.
                //STRETCH: modify this so sound waves reveal a certain amount of wall depending on how far away the sound was fired? should be a neat addition if the design wasnt so shaky.
                Wall wScript = hit.collider.gameObject.GetComponent<Wall>();
                wallHitPS.gameObject.transform.position = hit.point;
                wallHitPS.Play();
                Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 10.0f, Color.green, 0.5f);
                if (wScript != null)
                {
                    wScript.WallReactionHelper();
                }
                else
                {
                    Debug.Log("Could not find a wall collider on game object called " + hit.collider.gameObject);
                }

                Debug.Log("successful hit!");
            }
            else
            {
                Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward, Color.red, 0.5f);
                Debug.Log("wall out of range(?)");
            }

            //INVESTIGATION POINT!
            IP.NoiseActivate(30, transform.position);
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
        cloakInProgress = true;
        cloaked = true;
        float val = 0.01f;
        for (int i = 0; i < 100; i++)
        {
            //fully ignore this thread if there's a decloak in progress.
            yield return new WaitForSeconds(0.005f);
            dissolveMat.SetFloat("Vector1_2F06040B", val);
            val += 0.01f;
        }
        cloakInProgress = false;
    }

    IEnumerator decloak(bool timerOut)
    {
        if (timerOut)
        {
            //make the cloak slider bad color
            cloakSlider.color = new Color(37f/255f, 54f/255f, 54f/ 255f);
        }
        decloakInProgress = true; //act as sort of an interrupting mutex for the cloaking process.
        cloaked = false;
        float val = 0.99f;
        for (int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.005f);
            dissolveMat.SetFloat("Vector1_2F06040B", val);
            val -= 0.01f;

        }

        decloakInProgress = false;
        

    }

    IEnumerator muzzleFlash()
    {
        canFire = false;
        muzzlePS.Play();
        gunAnim.SetTrigger("ShootGun");
        yield return new WaitForSeconds(0.1f);
        gunAnim.SetTrigger("Reload");
        yield return new WaitForSeconds(0.3f);
        AudioManager.singleton.PlaySFX("GunReload");
        Debug.Log("Reload!");

        yield return new WaitForSeconds(.65f);

        Debug.Log("Ready to fire.");
        canFire = true;
    }

    public void InvestigationPointCallback(EnemyAI e)
    {
        //investigation point a SPECIFIC enemy that is passed in!
        IP.SightActivate(e, transform.position);
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
