using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public Image healthBar;

    float maxHealth = 300.0f;
    float currentHealth;
    void Start()
    {
        currentHealth = maxHealth;  
    }
    void Update()
    {
        
    }

    public void TakeDamage(float amt)
    {
        currentHealth -= amt;
        UpdateHealthUI();
        if (currentHealth <= 0f)
        {
            StartCoroutine(Die());
        }
        
    }
    void UpdateHealthUI()
    {
        healthBar.fillAmount = currentHealth / maxHealth;
    }

    IEnumerator Die()
    {
        //fade out and reset the current scene.
        Debug.Log("you are dead.");
        transform.Rotate(0, 0, 90);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
