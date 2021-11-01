using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int maxHealth = 1000;
    public int maxSkill = 200;
    public int currentHealth;
    public int currentSkill = 0;
    public HealthBar healthBar;
    public SkillBar skillBar;
    public static bool IsAlive = true;
    public Inventory inventory;
    public static bool damageMultiplier = false;
    PauseMenu pm;
    public GameObject deathMenuUI, gameUI, audioMain, audioDeath;
    
    //public Cinemachine.AxisState xAxis;
    //public Cinemachine.AxisState yAxis;
    //public Transform cameraLookAt;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        skillBar.SetMaxSkill(maxSkill);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0) Invoke(nameof(DestroyPlayer), 1f);

        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventory.UseItem(1);
        }else if(Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            inventory.UseItem(2);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            inventory.UseItem(3);
        }else if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.Alpha4))
        {
            inventory.UseItem(4);
        }else if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5))
        {
            inventory.UseItem(5);
        }else if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Alpha6))
        {
            inventory.UseItem(6);
        }
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
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    public void IncreaseHealth(int health)
    {
        currentHealth += health;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthBar.SetHealth(currentHealth);
    }

    public void IncreaseSkill(int skill)
    {
        currentSkill += skill;
        if (currentSkill > maxSkill)
        {
            currentSkill = maxSkill;
        }
    }

    public void DecreaseSkill(int skill)
    {
        currentSkill -= skill;
        skillBar.SetSkill(currentSkill);
    }

    public void useAmmo()
    {
        ControllerCharacter.spareAmmo += 30;
    }

    public void useHealthPotion()
    {
        
        if(currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthBar.SetHealth(currentHealth);
    }

    public void useSkillPotion()
    {
        IncreaseSkill(75);
    }

    public void useShield()
    {
        IncreaseHealth(200);
    }

    public IEnumerator usePainKiller()
    {
        IncreaseHealth(450);
        for(int i = 0; i <= 5; i++)
        {
            yield return new WaitForSeconds(1f);
            TakeDamage(90);
        }
    }

    public IEnumerator useDamageMultiplier()
    {
        damageMultiplier = true;
        yield return new WaitForSeconds(5f);
        damageMultiplier = false;
    }


}
