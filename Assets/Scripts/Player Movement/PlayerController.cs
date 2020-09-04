using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed, gravity = -9.81f;
    public Transform groundCheck;
    public float groundDist = 0.4f;
    public LayerMask groundLM;

    public bool cloaked = false;
    //debug
    public Material dissolveMat; //this material should ONLY be on the player and absolutely nothing else.

    public Material wallMat; //this should be the wall material. Should be a standard checker or something like that.
    //hopefully not too much lag happens, but keep a profiler open just in case.

    float xInput, yInput;
    Vector3 moveDirection, forward, right, vel;
    CharacterController cc;
    bool isGrounded;
    bool wallDecloakInProgress = false, cancelWallDecloak = false;

    private void Start()
    {
        wallMat.SetFloat("Vector1_2F06040B", 1.00f);
        dissolveMat.SetFloat("Vector1_2F06040B", 0.0f); //needs to always start fully uncloaked!
        cc = GetComponent<CharacterController>();
        groundCheck.localPosition = new Vector3(0, -cc.bounds.extents.y, 0);
        
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundLM);

        if(isGrounded && vel.y < 0)
        {
            vel.y = -2;
        }


        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");
        forward = transform.forward;
        forward.y = 0;
        right = transform.right;

        moveDirection = forward * yInput + right * xInput;
        cc.Move(playerSpeed * moveDirection);

        vel.y += gravity * Time.deltaTime;

        cc.Move(vel * Time.deltaTime);

        if (Input.GetMouseButtonDown(1) && !cloaked)
        {
            StopCoroutine(decloak()); //if in the middle of a process, interrupt it and reverse. 
            //This will allow for quick decision making on the player's part if new information becomes apparent mid-cloak.
            StartCoroutine(cloak());
        }
        else if(Input.GetMouseButtonDown(1))
        {
            StopCoroutine(cloak());
            StartCoroutine(decloak());
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (wallDecloakInProgress)
            {
                cancelWallDecloak = true;
            }
            //StopCoroutine(WallReaction()); //if it was dissolving, this should reset it. But no! See, unity's StopCoroutine() function just doesnt work.
            StartCoroutine(WallReaction());
        }
    }


    IEnumerator cloak()
    {
        
        float val = 0.01f;
        for(int i = 0; i < 100; i++)
        {
            if(val >= 0.5f && !cloaked)
            {
                cloaked = true;
            }
            yield return new WaitForSeconds(0.01f);
            dissolveMat.SetFloat("Vector1_2F06040B", val);
            val += 0.01f;
        }
        
        yield return null;
    }

    IEnumerator decloak()
    {
        float val = 0.99f;
        for (int i = 0; i < 100; i++)
        {
            if (val <= 0.49f && cloaked)
            {
                cloaked = false;
            }
            yield return new WaitForSeconds(0.01f);
            dissolveMat.SetFloat("Vector1_2F06040B", val);
            val -= 0.01f;
        }
        
        yield return null;

    }

    IEnumerator WallReaction()
    {
        Debug.Log("Gunshot! " + Time.time);
        wallMat.SetFloat("Vector1_2F06040B", 0.00f);
        yield return new WaitForSeconds(2.5f);
        float val = 0.01f;
        wallDecloakInProgress = true;
        for (int i = 0; i < 100; i++)
        {
            if (!cancelWallDecloak)
            {
                yield return new WaitForSeconds(0.02f);
                wallMat.SetFloat("Vector1_2F06040B", val);
                val += 0.01f;
            }
            else
            {
                wallMat.SetFloat("Vector1_2F06040B", 0.00f);
                cancelWallDecloak = false;
                break;
            }
            
        }
        wallDecloakInProgress = false;
    }


}
