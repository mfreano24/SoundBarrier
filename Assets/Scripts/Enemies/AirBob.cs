using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirBob : MonoBehaviour
{
    //I guess we can do sound here too maybe? Dunno yet.
    float yPos, defaultY;
    void Start()
    {
        yPos = transform.position.y;
        defaultY = yPos;
    }

    // Update is called once per frame
    void Update()
    {
        yPos = defaultY + 0.2f*Mathf.Sin(2.0f * Time.time); //this only works if we're on the same level the whole time. Might need a different implementation
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
    }
}
