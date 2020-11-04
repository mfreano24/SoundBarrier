using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;


public class EnemyAI : MonoBehaviour
{
    public float TESTGunDamage = 30.0f;
    public float TESTSightAngle = 45.0f;
    public Transform player;
    public PlayerController pcScript;
    public PlayerHealth pHealth;
    PlayerEffects pEffects;
    public LayerMask lm;
    public ParticleSystem ps;
    public Vector3[] patrol; //assign this through the editor.
    public Text textDebug;


    int patrolIndex;

    bool shooting = false;
    bool DoubleShotBuffer = false;
    Vector3 investigationPoint, NULL_IP; //assign to this point whenever a reason to investigate is triggered.
    NavMeshAgent nav;

    int state = 0;
    // STATE 0 = "Patrolling"
    // STATE 1 = "Investigation" - navmesh to the destination marker
    //                              Good way to implement this would be to have an investigation point spawned if the player is within some Vector3 Distance
    // STATE 2 = "Attack" - shoot! dont need to dodge because you do lots of damage.

    public SightlineVisualizer viz;

    InvestigationPoint IP;

    

    void Awake()
    {
        /*
        for (int i = 0; i < patrol.Length; i++)
        {
            patrol[i].y  = transform.position.y;
        }
        */
        nav = GetComponent<NavMeshAgent>();
        if(patrol.Length > 1)
        {
            patrolIndex = Random.Range(1, patrol.Length - 1); //just start somewhere.
            transform.position = patrol[patrolIndex - 1]; //assign it so we arent travelling randomly
            nav.SetDestination(patrol[patrolIndex]);
        }
        else
        {
            //dont need to do anything i guess?
        }
        
        investigationPoint = new Vector3(-999, -999, -999); //Going off of the sheer confidence that this value won't be an investigation point?
        //watch it be, you silly man
        NULL_IP = investigationPoint;
        //TODO: may need to change that, because Vector3s are *not* nullable.
        IP = transform.parent.GetComponentInChildren<InvestigationPoint>();

    }

    private void OnDrawGizmos()
    {
        //draw the patrol path for this enemy
        Gizmos.color = Color.magenta;
        foreach (Vector3 p in patrol)
        {
            
            Gizmos.DrawCube(p, new Vector3(1f,1f,1f));
        }
        for(int i = 1; i < patrol.Length; i++)
        {
            Gizmos.DrawLine(patrol[i - 1], patrol[i]);
        }
        Gizmos.DrawLine(patrol[0], patrol[patrol.Length - 1]);


        //draw the look direction
        Gizmos.color = Color.green;
        //Gizmos.DrawLine(transform.position, transform.TransformDirection(transform.forward));
    }

    private void Start()
    {
        //viz = GetComponent<SightlineVisualizer>(); //these should be on the same object, right?
        pEffects = pHealth.gameObject.GetComponent<PlayerEffects>();
        viz.CurrentColor = Color.green;
        StartCoroutine(Shoot());
    }

    void Update()
    {
        //transform.LookAt(patrol[patrolIndex] - transform.position);
        //TODO: Fix this? Please? I'm begging. God.

        //debug
        //PlayerInFront();

        //Debug.DrawRay(transform.position,(2.0f * nav.velocity), Color.cyan);
        //transform.LookAt(nav.velocity);

        switch (state)
        {
            case 0:
                PatrolUpdate();
                transform.LookAt(transform.position + nav.velocity);
                break;
            case 1:
                InvestigateUpdate();
                transform.LookAt(transform.position + nav.velocity);
                break;
            case 2:
                AttackUpdate();
                transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                break;
        }
        //Debug.Log("Enemy " + (PlayerVisible() ? "can see " : "cannot see ") + "the player. pHealth.dead == " + pHealth.dead + " and shooting == " + shooting);
        if (PlayerVisible() && !pHealth.dead && state != 2)
        {
            state = 2;
            nav.SetDestination(transform.position);
            viz.CurrentColor = Color.red;
        }

    }

    void PatrolUpdate()
    {
        //Debug.Log("Distance to next patrol point = " + Vector3.Distance(transform.position, patrol[patrolIndex]));
        if (Vector3.Distance(transform.position, patrol[patrolIndex]) <= 1f)
        {
            //NOTE: Optimize the magic number in the if statement above (0.45f?), so we can find a seamless transition from patrol target to patrol target.
            patrolIndex++;
            if (patrolIndex == patrol.Length)
            {
                //reset to 0 to accomodate the path
                patrolIndex = 0;
            }
            if(patrol.Length > 1)
            {
                Debug.Log("Point reached! Patrol Index is now: " + patrolIndex + ".");
                nav.SetDestination(patrol[patrolIndex]);
            }
            
        }
        else
        {
            //Debug.Log("Current distance to next checkpoint = " + Vector3.Distance(transform.position, patrol[patrolIndex]));
        }

    }

    void InvestigateUpdate()
    {
        if (Vector3.Equals(NULL_IP, investigationPoint))
        {
            state = 0; //if the player hasnt been seen and we're in this state still, just go back to patrolling.
            nav.SetDestination(GetClosestPatrolPoint());
            viz.CurrentColor = Color.green;
        }

    }

    void AttackUpdate()
    {
        if (!PlayerVisible())
        {
            Debug.Log("Player isn't visible anymore.");
            state = 1;
            viz.CurrentColor = Color.yellow;
            pcScript.InvestigationPointCallback(this);
            
        }
    }

    IEnumerator Shoot()
    {
        float shootStep = 0.75f;
        float shootTimer = 0.0f;
        while (true)
        {
            
            if (state == 2)
            {
                if(shootTimer >= shootStep)
                {
                    ps.Play();
                    pHealth.TakeDamage(TESTGunDamage);
                    StartCoroutine(pEffects.BloodFlash());
                    shootTimer = 0;
                }
                else
                {
                    shootTimer += Time.deltaTime;
                }
                
            }
            else
            {
                shootTimer = 0.75f;
            }
            yield return new WaitForEndOfFrame();
        }

    }

    bool PlayerVisible()
    {
        //viz.DrawFieldOfView();
        RaycastHit hit;
        if ((Physics.Raycast(transform.position, player.position - transform.position, out hit, 
            Vector3.Distance(transform.position, player.position) + 0.01f, lm) 
            || pcScript.cloaked)&& transform.position.y - player.position.y < 10.0f)
        {
            Debug.DrawRay(transform.position, hit.point - transform.position, Color.red);
            return false;
        }
        else
        {
            Debug.DrawRay(transform.position, player.position - transform.position, Color.green);
            return PlayerInFront() && Vector3.Distance(transform.position, player.position) <= 22.5f; //efficiency, so that PIF only runs here.
            //they dont need to have all seeing vision otherwise the player may have trouble seeing them when they get spotted!
        }
        
    }

    

    bool PlayerInFront()
    {
        //Debug.LogWarning(Vector3.Angle(transform.forward, player.position - transform.position) + "\t"+(Vector3.Angle(transform.forward, player.position - transform.position) < 90.0f ? "Player is in front" : "Player is behind"));
        return (Vector3.Angle(transform.forward, player.position - transform.position) < 35.0f);
        //this works, dont touch it! please. god.
    }

    public void NoiseAlert(Vector3 _investigationPoint, InvestigationPoint _IP)
    {
        //change state to investigative and begin navMeshing towards the point passed in.
        //if an investigation point already exists, just replace it, freshest sounds first!
        if(Mathf.Abs(_investigationPoint.y - transform.position.y) <= 6f)
        {
            //Debug.Log("Investigation Point " + _investigationPoint + " has reached enemy.");
            Debug.Log("Difference in Y positions: " + (_investigationPoint.y - transform.position.y));
            investigationPoint = _investigationPoint;
            if (state != 2)
            {
                state = 1;
                viz.CurrentColor = Color.yellow;
                nav.SetDestination(investigationPoint);
            }
        }
        else
        {
            Debug.Log("Difference in Y positions: " + (_investigationPoint.y - transform.position.y));
        }
        
        
    }

    public void ResetIP(Vector3 _reset)
    {

        Debug.Log("Investigation state has been cleared!");
        investigationPoint = NULL_IP;
        if (state != 2)
        {
            Debug.Log("Return to normalcy.");
            state = 0;
            viz.CurrentColor = Color.green;
            nav.SetDestination(GetClosestPatrolPoint());
        }
        else
        {
            Debug.Log("State is 2. Something is wrong.");
        }
        
    }

    Vector3 GetClosestPatrolPoint()
    {
        Vector3 ret = new Vector3(0, 0, 0);
        float min = float.MaxValue;
        for(int i = 0; i < patrol.Length; i++)
        {
            if(Vector3.Distance(patrol[i],transform.position) < min)
            {
                ret = patrol[i];
                patrolIndex = i;
            }
        }
        return ret;
    }
}
