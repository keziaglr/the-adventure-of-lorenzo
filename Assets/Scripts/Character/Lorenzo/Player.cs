using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int maxHealth = 1000;
    public int maxSkill = 200;
    public int currentHealth;
    public static int currentSkill;
    public HealthBar healthBar;
    public SkillBar skillBar;
    public static bool IsAlive = true;
    Inventory inventory;
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
        //xAxis.Update(Time.fixedDeltaTime);
        //yAxis.Update(Time.fixedDeltaTime);

        //cameraLookAt.eulerAngles = new Vector3(yAxis.Value, xAxis.Value, 0);

        skillBar.SetSkill(currentSkill); 
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
        Debug.Log(currentHealth);
        healthBar.SetHealth(currentHealth);
    }

    public void IncreaseSkill(int skill)
    {
        currentSkill += skill;
        //Debug.Log(currentSkill);
        skillBar.SetSkill(currentSkill);
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
