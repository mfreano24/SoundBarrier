using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPManager : MonoBehaviour
{ 
    //whole point of this class is literally just to call the investigation stuff on every enemy.
    //yeah you're gonna have to do this manually for every level because there's no pattern to the IPs. should be fun!
    public List<InvestigationPoint> enemyIPs;
    public bool NoiseActivate(float noiseStrength, Vector3 sourcePosition)
    {
        bool ret = false;
        foreach(InvestigationPoint i in enemyIPs)
        {
            if (i.ActivateNoise(noiseStrength, sourcePosition))
            {
                ret = true;
            }
        }

        return ret;
    }

    public void SightActivate(EnemyAI enemy, Vector3 sourcePosition)
    {
        foreach(InvestigationPoint i in enemyIPs)
        {
            i.ActivateSight(enemy, sourcePosition);
        }
    }
}
