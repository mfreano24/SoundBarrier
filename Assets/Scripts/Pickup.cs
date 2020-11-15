using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    float centralizedY, currentY;
    private void Start()
    {
        centralizedY = transform.position.y;
    }

    private void Update()
    {
        currentY = 0.1f * Mathf.Sin(Time.time) + centralizedY;
        transform.Rotate(new Vector3(0, 0.25f, 0)); //stock rotation in place, may need to change the magic value in the Y pos?
        transform.position = new Vector3(transform.position.x, currentY, transform.position.z);
    }
}
