using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigationPoint : MonoBehaviour
{
    List<EnemyAI> Attracts;
    void Start()
    {
        
    }

    public void AlertNearbyEnemies(float _soundVolume)
    {
        StartCoroutine(destNoise());
        Attracts = new List<EnemyAI>();
        foreach (GameObject e in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Debug.Log("IP of distance " + _soundVolume + " on enemy " + e + ":\t" + Vector3.Distance(e.transform.position, transform.position));
            if (Vector3.Distance(e.transform.position, transform.position) < _soundVolume)
            {
                Attracts.Add(e.GetComponent<EnemyAI>());
                Attracts[Attracts.Count - 1].NoiseAlert(transform.position);
            }
        }

    }

    public void AlertSpecificEnemy(EnemyAI ea)
    {
        StartCoroutine(destSight(ea));
        ea.NoiseAlert(transform.position);
    }

    IEnumerator destNoise()
    {
        //Use destNoise when a noise happens and potentially alerts lots of enemies.
        yield return new WaitForSeconds(5);
        foreach(EnemyAI ea in Attracts)
        {
            ea.ResetIP(transform.position);
        }

        Destroy(this.gameObject);
    }

    IEnumerator destSight(EnemyAI ea)
    {
        //Use destSight when an enemy loses sight of you and they need to go investigate.
        yield return new WaitForSeconds(5);
        ea.ResetIP(transform.position);
        Destroy(this.gameObject);

    }

}
