using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickPaint : MonoBehaviour
{
    public Camera mainCamera;
    public Texture2D splashTexture;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit))
            {
                Paintable script = hit.collider.gameObject.GetComponent<Paintable>();
                if (null != script)
                    script.PaintOn(hit.textureCoord, splashTexture);
            }
        }
    }
}
