using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigationPoint : MonoBehaviour
{
    [HideInInspector] public bool enemyCloseToPoint = false;

    EnemyAI assignedEnemy;
    public bool activePoint = false;
    bool inDeacCoroutine = false;
    Vector3 activePosition;
    bool breakCoroutineRefreshPoint = false;
    void Start()
    {
        activePosition = transform.position;
        assignedEnemy = transform.parent.GetComponentInChildren<EnemyAI>();//get the enemy AI script in the encapsulator
    }

    private void LateUpdate()
    {
        
        if (Vector3.Distance(assignedEnemy.transform.position, activePosition) <= 5f && activePoint && !inDeacCoroutine)
        {
            StartCoroutine(WaitForDeactivate());
        }
    }

    public bool ActivateNoise(float distanceToAlert, Vector3 globalPosition)
    {
        if(Vector3.Distance(globalPosition, assignedEnemy.transform.position) <= distanceToAlert)
        {
            if (inDeacCoroutine)
            {
                breakCoroutineRefreshPoint = true;
            }
            activePosition = globalPosition;
            assignedEnemy.NoiseAlert(globalPosition, this);
            activePoint = true;
            return true;
        }
        //otherwise ignore the noise attempt
        return false;
    }

    public void ActivateSight(EnemyAI enemyToAlert, Vector3 globalPosition)
    {
        if(enemyToAlert == assignedEnemy)
        {
            if (inDeacCoroutine)
            {
                breakCoroutineRefreshPoint = true;
            }
            activePosition = globalPosition;
            assignedEnemy.NoiseAlert(globalPosition, this);
            activePoint = true;
        }
    }

    IEnumerator WaitForDeactivate()
    {
        Debug.Log("Waiting 3s for deactivation now. Enemy's close to the point.");
        inDeacCoroutine = true;
        yield return new WaitForSeconds(3f);
        if (!breakCoroutineRefreshPoint)
        {
            assignedEnemy.ResetIP(transform.position);
            inDeacCoroutine = false;
            activePoint = false;
            //active position doesnt matter if the point itself isnt active.
        }
        else
        {
            breakCoroutineRefreshPoint = false;
        }

    }
}
