using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvestigationPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject e in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if(Vector3.Distance(e.transform.position, transform.position) < 15f)
            {
                e.GetComponent<EnemyAI>().NoiseAlert(transform.position);
            }
        }
        StartCoroutine(dest());
    }

    IEnumerator dest()
    {
        //auto-destroy this.
        yield return new WaitForSeconds(5);
        Destroy(this.gameObject);
    }

}
