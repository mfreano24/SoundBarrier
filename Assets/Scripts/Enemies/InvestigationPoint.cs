using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigationPoint : MonoBehaviour
{
    List<EnemyAI> alertedEnemies;
    bool enemyCloseToPoint = false;

    bool enemyCloseCoroutineInProgress = false;
    void Start()
    {
        alertedEnemies = new List<EnemyAI>(); //init
        StartCoroutine(CheckForNearbyEnemies());
    }
    /// <summary>
    /// "One at a time" model of Investigation point simulation
    /// Pros: no clutter, no deleting things.
    /// </summary>

    private void Update()
    {
    }

    public bool ActivateNoise(float distanceToAlert, Vector3 globalPosition)
    {
        bool ret = false;
        //hmmm
        //DeactivateAll();
        transform.position = globalPosition;
        Debug.Log("Attempting to alert enemies at the position " + globalPosition);
        foreach (GameObject e in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if(Vector3.Distance(e.transform.position, globalPosition) <= distanceToAlert)
            {
                ret = true;
                EnemyAI curr = e.GetComponent<EnemyAI>();
                if (alertedEnemies.Contains(curr))
                {
                    curr.ResetIP(transform.TransformDirection(transform.position));
                }
                alertedEnemies.Add(curr);
                curr.NoiseAlert(globalPosition);
            }
        }
        return ret; //returns true if an enemy has been alerted. only really needed for the running.
    }

    public void ActivateSight(EnemyAI enemyToAlert, Vector3 globalPosition)
    {
        Debug.Log("Enemy has lost sight of player, investigating now.");
        DeactivateAll();
        alertedEnemies.Add(enemyToAlert);
        enemyToAlert.NoiseAlert(globalPosition);
        
    }

    public void DeactivateAll()
    {
        foreach(EnemyAI e in alertedEnemies)
        {
            e.ResetIP(transform.TransformDirection(transform.position));
        }
    }

    IEnumerator CheckForNearbyEnemies()
    {
        while (true)
        {
            if (!enemyCloseToPoint)
            {
                foreach (EnemyAI e in alertedEnemies)
                {
                    if (Vector3.Distance(transform.position, e.transform.position) <= 5)
                    {
                        StartCoroutine(WaitForDeactivation());
                        enemyCloseToPoint = true;
                    }
                }
            }   
            yield return new WaitForEndOfFrame();
        }
   
    }

    IEnumerator WaitForDeactivation()
    {
        //TODO: this is being called like hell.
        if (!enemyCloseCoroutineInProgress)
        {
            enemyCloseCoroutineInProgress = true;
            yield return new WaitUntil(() => enemyCloseToPoint);
            Debug.Log("Enemy's close to the point!");
            yield return new WaitForSeconds(2.5f);
            DeactivateAll();
            enemyCloseToPoint = false;
            enemyCloseCoroutineInProgress = false;
        }
        else
        {
            Debug.Log("Breaking, coroutine already in progress...");
            yield return null;
        }
       
    }

}
