using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int maxHealth = 1000;
    public int currentHealth;
    public HealthBar healthBar;
    public static bool IsAlive = true;
    Inventory inventory;
    PauseMenu pm;
    public GameObject deathMenuUI, gameUI, audioMain, audioDeath;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0) Invoke(nameof(DestroyPlayer), 0.5f);
    }
    private void DestroyPlayer()
    {
        //Destroy(gameObject);
        deathMenuUI.SetActive(true);
        gameUI.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        IsAlive = false;
        ControllerCharacter.coreItems = 0;
        audioMain.SetActive(false);
        audioDeath.SetActive(true);
        //pm.Death();
    }

    public void TakeDamage(int damage)
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
