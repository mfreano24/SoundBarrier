using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirBob : MonoBehaviour
{

    float yPos;
    public float defaultY;
    void Start()
    {
        yPos = transform.position.y;
        defaultY = yPos;
    }

    // Update is called once per frame
    void Update()
    {
        yPos = defaultY + 0.2f*Mathf.Sin(2.0f * Time.time); //HOPE THIS WORKS //lol ofc it dont
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
    }
}
