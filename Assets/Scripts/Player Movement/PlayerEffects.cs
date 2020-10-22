using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class PlayerEffects : MonoBehaviour
{
    ///
    /// this script will be just for visual effects like particles and post processing related to the player
    ///
    public Volume globalV;
    Vignette vig;

    public AnimationCurve flashValues;

    private void Start()
    {
        Debug.Log("Is this script even working at all");
        globalV.profile.TryGet<Vignette>(out vig); //get the vignette component from the global volume.
        vig.intensity.value = 0; //dont need dat yet
        vig.color.value = Color.red;
    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.K))
        {
            //DEBUG FOR NOW.
            StartCoroutine(BloodFlash());
        }
        */
    }

    public IEnumerator BloodFlash()
    {
        Debug.Log("Flash begin");
        float timer = 0;
        while(timer <= 1)
        {
            vig.intensity.value = flashValues.Evaluate(timer);
            timer += Time.deltaTime * 1.5f; //can increase or decrease the rate by multiplying Time.deltaTime here with something.
            yield return new WaitForSeconds(0.01f);
        }
        Debug.Log("Flash end!");
        vig.intensity.value = 0; //reset her!

    }
}
