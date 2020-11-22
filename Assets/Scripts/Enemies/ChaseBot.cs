using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ChaseBot : MonoBehaviour
{
    #region Public/Serializable Variables
    [Header("Other Object References")]
    //should be all thats needed for this particular bot.
    public PlayerController player;
    public ParticleSystem ps; //muzzle flash particle system.
    public ParticleSystem entranceEffect;
    public SightlineVisualizer viz;

    [Header("Adjustable Values")]
    //distance for which when the bot gets close enough and the player **isn't** cloaked that it starts shooting
    public float minDistance;
    //if we update the navmesh agent every frame that will be a very bad idea. This will be the time (in seconds) for which each step of the reset will happen
    public float resetNavMeshStep;
    //same concept but for shooting
    public float shootGunStep;
    public float damageToDeal;
    #endregion
    #region Non-Public Variables
    PlayerHealth pHealth;
    PlayerEffects pEffects;
    NavMeshAgent nav;

    
    //STATES:
    //STATE 0 = "Chase" - the robot will move towards the player slowly. This is equiv. to the patrol phase of the regular enemy AI.
    //STATE 1 = "Attack" - the robot will (whatever Im gonna do with movement) and shoot at the player.
    //NOTE: no need for an investigation state, this thing will be *much* slower than the enemy AI robot, and it's going to be chasing the player anyway.
    int state = 0;

    float resetNavMeshCurrent = -0.5f; //we can give extra leeway with the timer here, just so the robot starts moving at the right time.

    float shootGunCurrent = 0.0f;

    bool changeBackToChaseInProgress = false;

    ObjectAudio aud;
    #endregion
    #region Functions/Methods
    bool PlayerInRange()
    {
        Vector3 xzPlaneDistance = new Vector3(player.transform.position.x - transform.position.x, 0, player.transform.position.z - transform.position.z);
        if (xzPlaneDistance.magnitude <= minDistance)
        {
            return !(player.cloaked);
        }
        return false;
    }

    #endregion
    #region Primary Loop/State Machine

    private void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        pEffects = player.GetComponent<PlayerEffects>();
        pHealth = player.GetComponent<PlayerHealth>();
        aud = GetComponent<ObjectAudio>();
        viz.CurrentColor = new Color(140f / 255f, 0, 241f / 255f);
    }

    private void OnEnable()
    {
        entranceEffect.Play(); //play once
        aud.PlaySFX("Poof");
    }

    private void Update()
    {
        switch (state)
        {
            case 0: ChaseUpdate(); break;
            case 1: AttackUpdate(); break;
            default: Debug.LogError("Invalid State on " + GetType() + ".cs"); break;
        }
    }

    void ChaseUpdate()
    {
        if (PlayerInRange() && !pHealth.safe)
        {
            aud.PlaySFX("enemyAlerted");
            state = 1; //switch to shooting state
            resetNavMeshCurrent = resetNavMeshStep; //this way the chase resumes immediately.
        }
        else
        {
            if (resetNavMeshCurrent >= resetNavMeshStep)
            {
                nav.SetDestination(player.transform.position);
                if(nav.remainingDistance > 25f)
                {
                    nav.speed = 10.0f;
                }
                else if(nav.remainingDistance > 20f)
                {
                    nav.speed = 6.5f;
                }
                else if(nav.remainingDistance > 10f)
                {
                    nav.speed = 5.5f;
                }
                else if(nav.remainingDistance > 2.5f)
                {
                    nav.speed = 4.0f;
                }
                else
                {
                    nav.speed = 2.5f;
                }
                resetNavMeshCurrent = 0.0f;
                Debug.Log("Distance to player = " + nav.remainingDistance + "\tNav Speed set to " + nav.speed);
            }
            else
            {
                resetNavMeshCurrent += Time.deltaTime; //adding time in seconds to the reset counter.
            }
        }
    }

    void AttackUpdate()
    {
        transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));
        if (!PlayerInRange())
        {
            aud.PlaySFX("enemyPatrol");
            state = 0; //switch to chase phase(??? NOTE: May want to give it a 3 second or so timer so the player can book it in time)
            shootGunCurrent = shootGunStep; //this way the first shot is the exact frame that the player is within range next time.
            if (!changeBackToChaseInProgress)
            {
                changeBackToChaseInProgress = true;
                StartCoroutine(RevertBackToChaseMode());
            }
        }
        //potentially use the same step/current system to shoot? its nicer than a coroutine 
        else
        {
            //if we reach this while waiting for the revert, then we can cancel it this way.
            if (changeBackToChaseInProgress)
            {
                changeBackToChaseInProgress = false;
            }

            if (shootGunCurrent >= shootGunStep && !pHealth.dead)
            {
                //shoot gun
                aud.PlaySFX("enemyGun");
                ps.Play();
                pHealth.TakeDamage(damageToDeal);
                StartCoroutine(pEffects.BloodFlash());
                shootGunCurrent = 0;
            }
            else
            {
                shootGunCurrent += Time.deltaTime;
            }
        }
    }

    IEnumerator RevertBackToChaseMode()
    {
        yield return new WaitForSeconds(2.5f);
        if (changeBackToChaseInProgress)
        {
            
            changeBackToChaseInProgress = false;
            state = 0;
            //aud.PlaySFX("enemyAlerted");
        }
        //else do nothing, just leave the Coroutine.
        //TODO: lag test this, as we could possibly have a lot of coroutines pop up at once. threading is laggy!!

    }
    #endregion



}
