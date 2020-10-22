using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    //todo: clean this up please
    PlayerHealth phealth;

    public float playerSpeed, gravity = -9.81f;
    public Transform groundCheck;
    public float groundDist = 0.4f;
    public LayerMask groundLM;

    public InvestigationPoint IP;

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
    public ParticleSystem wallHitPS;
    public Transform gunTransform;
    public Animator gunAnim;

    bool canFire = true;
    bool sprinting = false;
    bool enemyAlerted = false;
    

    public bool inSwitchRange = false;
    FenceSwitchPanel currentFSP;

    public CameraController cameraCon;

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

    }

    private void Update()
    {
        //MOVEMENT
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundLM);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (canFire && !sprinting)
            {
                StartCoroutine(gunRotateDown());
                canFire = false;
                sprinting = true;
            }
            //re-activate the investigation point every frame UNTIL it catches someone.
            if (!enemyAlerted)
            {
                //need a case to reset this thing but like, if you activate this you're probably going to get shot.
                enemyAlerted = IP.ActivateNoise(5f, transform.position);
            }
            else
            {
                Debug.Log("Enemy is aware of your sprinting and would like to hurt you.");
            }
            playerSpeed = 0.2f;
        }
        else
        {
            if (!canFire && sprinting)
            {
                StartCoroutine(gunRotateUp());
                canFire = true;
                sprinting = false;
            }

            playerSpeed = 0.1f;
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
            if (cloakTimer > 5.00f)
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
        cloakSlider.fillAmount = cloakTimer / 5.0f;

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
            //investigation points

            IP.ActivateNoise(5, transform.position);

        }

        if (Input.GetMouseButtonDown(0) && canFire)
        {
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
                if(wScript != null)
                {
                    StartCoroutine(wScript.WallReaction());
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
            IP.ActivateNoise(30, transform.position);
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
        muzzlePS.Play();
        gunAnim.SetTrigger("ShootGun");
        yield return new WaitForSeconds(0.1f);
        gunAnim.SetTrigger("Reload");

        Debug.Log("Reload!");
        
        yield return new WaitForSeconds(1f);

        Debug.Log("Ready to fire.");
        canFire = true;
    }

    IEnumerator gunRotateDown()
    {
        Debug.Log("Sprinting... gun down.");
        for (int i = 1; i <= 10; i++)
        {
            gunTransform.position = new Vector3(gunTransform.position.x, gunTransform.position.y - 1f, gunTransform.position.z);
            yield return new WaitForSeconds(0.005f);
        }

    }

    IEnumerator gunRotateUp()
    {
        Debug.Log("Walking... gun up.");
        for (int i = 1; i <= 10; i++)
        {
            gunTransform.position = new Vector3(gunTransform.position.x, gunTransform.position.y + 1f, gunTransform.position.z);
            yield return new WaitForSeconds(0.005f);
        }

    }

    public void InvestigationPointCallback(EnemyAI e)
    {
        //investigation point a SPECIFIC enemy that is passed in!
        IP.ActivateSight(e, transform.position);
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
