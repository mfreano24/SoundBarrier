using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBobbing : MonoBehaviour
{
    PlayerController pc;
    // Start is called before the first frame update
    void Start()
    {
        pc = transform.parent.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pc.enabled) //tutorial might disable the player controller. generally when that happens we dont care for bob check.
        {
            //basic bob formula
            if (pc.moving)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, 0.125f * Mathf.Abs(Mathf.Sin(7.5f * Time.time)) - 0.0625f, transform.localPosition.z);
            }
            else
            {
                transform.localPosition = new Vector3(transform.localPosition.x, 0f, transform.localPosition.z);
            }
        }
        
        
        
    }
}
