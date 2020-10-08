﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public Image healthBar;
    [HideInInspector]
    public bool dead;

    float maxHealth = 300.0f;
    float currentHealth;

    bool damageCooldown;
    float damageCooldownProgress = 0.00f;
    float damageCooldownStep = 2.50f;
    float damageRecoveryRate = -1.00f; //MUST BE NEGATIVE!


    public GameManager gm;
    void Start()
    {
        dead = false;
        currentHealth = maxHealth;  
    }

    private void OnEnable()
    {
        Debug.Log("Finding new GameManager for the player!");
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        //Debug.Log("Current player health is at " + 100.0f * (float)(currentHealth / maxHealth) + "%");
        if (damageCooldown)
        {
            damageCooldownProgress += Time.deltaTime;
            if(damageCooldownProgress >= damageCooldownStep)
            {
                damageCooldown = false;
            }
        }
        else if(currentHealth < maxHealth)
        {
            
            TakeDamage(damageRecoveryRate);
        }
        else if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void TakeDamage(float amt)
    {
        //TODO: add red vignette flash
        if(amt > 0)
        {
            damageCooldownProgress = 0;
            damageCooldown = true;
        }
        
        currentHealth -= amt;
        UpdateHealthUI();
        if (currentHealth <= 0f)
        {
            Debug.Log("HELLO??");

            Die();
        }
        
    }
    void UpdateHealthUI()
    {
        healthBar.fillAmount = currentHealth / maxHealth;
    }

    void Die()
    {
        if (!dead)
        {
            //fade out and reset the current scene.
            
            gm.playerWin = false;
            gm.roundOver = true;
            
            Debug.Log("you are dead.");
            transform.Rotate(0, 0, 90);
            dead = true;
            
        }
        
    }


}
