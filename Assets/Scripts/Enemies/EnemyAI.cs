using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public PlayerController pcScript;
    public LayerMask lm;

    void Start()
    {
        
    }
    void Update()
    {
        bool hi = PlayerVisible();
    }

    bool PlayerVisible()
    {
        if (Physics.Raycast(transform.position, player.position - transform.position, 15, lm) || pcScript.cloaked)
        {
            Debug.DrawRay(transform.position, player.position - transform.position, Color.red);
        }
        else
        {
            Debug.DrawRay(transform.position, player.position - transform.position, Color.green);
        }
        


        return false;
    }
}
