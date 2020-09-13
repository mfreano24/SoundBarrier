using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public PlayerController pcScript;
    public LayerMask lm;
    public Vector3[] patrol; //assign this through the editor.
    int patrolIndex;

    Vector3 investigationPoint, NULL_IP; //assign to this point whenever a reason to investigate is triggered.
    NavMeshAgent nav;

    int state = 0;
    // STATE 0 = "Patrolling"
    // STATE 1 = "Investigation" - navmesh to the destination marker
    //                              Good way to implement this would be to have an investigation point spawned if the player is within some Vector3 Distance
    // STATE 2 = "Attack" - shoot! dont need to dodge because you do lots of damage.


    void Awake()
    {
        for (int i = 0; i < patrol.Length; i++)
        {
            patrol[i].y  = transform.position.y;
        }
        nav = GetComponent<NavMeshAgent>();
        patrolIndex = Random.Range(1, patrol.Length - 1); //just start somewhere.
        transform.position = patrol[patrolIndex - 1]; //assign it so we arent travelling randomly
        nav.SetDestination(patrol[patrolIndex]);
        investigationPoint = new Vector3(-999, -999, -999); //Going off of the sheer confidence that this value won't be an investigation point?
        NULL_IP = investigationPoint;
        //TODO: may need to change that, because Vector3s are *not* nullable.
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
    }

    void Update()
    {
        //transform.LookAt(patrol[patrolIndex] - transform.position);
        //TODO: Fix this? Please? I'm begging. God.


        Debug.DrawRay(transform.position,(2.0f * nav.velocity), Color.cyan);
        transform.LookAt(transform.position + nav.velocity);

        switch (state)
        {
            case 0: PatrolUpdate(); break;
            case 1: InvestigateUpdate(); break;
            case 2: AttackUpdate(); break;
        }

        if (PlayerVisible())
        {
            state = 2;
            nav.SetDestination(transform.position); //dont move, just spray!
        }

    }

    void PatrolUpdate()
    {
        if (Vector3.Distance(transform.position, patrol[patrolIndex]) <= 0.45f)
        {
            //NOTE: Optimize the magic number in the if statement above (0.45f?), so we can find a seamless transition from patrol target to patrol target.
            patrolIndex++;
            if (patrolIndex == patrol.Length)
            {
                //reset to 0 to accomodate the path
                patrolIndex = 0;
            }
            Debug.Log("Point reached! Patrol Index is now: " + patrolIndex + ".");
            nav.SetDestination(patrol[patrolIndex]);
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
        }

    }

    void AttackUpdate()
    {
        if (!PlayerVisible())
        {
            state = 1;
            pcScript.InvestigationPointCallback(this);
            
        }
        else
        {
            Debug.Log("SHOOTING AT PLAYER!!!!!!!!!!!!!!!!!!!!");
        }
    }

    bool PlayerVisible()
    {
        //float angleRot = Vector3.Angle(transform.forward, player.position - transform.position);
        
        RaycastHit hit;
        if (Physics.Raycast(transform.position, player.position - transform.position, out hit, Vector3.Distance(transform.position, player.position) + 0.01f, lm) || pcScript.cloaked)
        {
            Debug.DrawRay(transform.position, hit.point - transform.position, Color.red);
            return false;
        }
        else
        {
            Debug.DrawRay(transform.position, player.position - transform.position, Color.green);
            return (true && PlayerInFront()); //efficiency, baby!
        }
        
    }

    bool PlayerInFront()
    {
        Debug.Log(Vector3.Angle(transform.position + nav.velocity, player.position - transform.position));
        return (Vector3.Angle(transform.position + nav.velocity, player.position - transform.position) < 90.0f);
    }

    public void NoiseAlert(Vector3 _investigationPoint)
    {
        //change state to investigative and begin navMeshing towards the point passed in.
        //if an investigation point already exists, just replace it, freshest sounds first!
        Debug.Log("New Investigation Point "+_investigationPoint+" has reached enemy.");
        investigationPoint = _investigationPoint;
        state = 1;
        nav.SetDestination(investigationPoint);
    }

    public void ResetIP(Vector3 _reset)
    {
        if(_reset == investigationPoint)
        {
            Debug.Log("Newest investigation point has been cleared!");
            investigationPoint = NULL_IP;
            state = 0;
            nav.SetDestination(GetClosestPatrolPoint());
        }
        else
        {
            Debug.Log("No reset- a fresher Investigation Point is current priority.");
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
