using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int maxHealth = 1000;
    public int currentHealth;
    public HealthBar healthBar;
    Inventory inventory;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            TakeDamage(20);
        }
    }

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    public void useAmmo()
    {

    }

    public void useHealthPotion()
    {
        currentHealth += 200;
    }

    public void useSkillPotion()
    {

    }

    public void useShield()
    {

    }

    public void usePainKiller()
    {

    }

    public void useDamageMultiplier()
    {

    }
}
