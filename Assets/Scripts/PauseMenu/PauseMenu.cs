using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public static bool IsRestart = false;
    

    public GameObject pauseMenuUI, deathMenuUI, gameUI;

    // Update is called once per frame
    void Update()
    {
        if(DialogueManager.dialogueActive == false)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }

    }

    public void Restart()
    {
        Player.IsAlive = true;
        SceneManager.LoadScene(1);
        IsRestart = true;
        //Debug.Log("Pause Menu " + IsRestart);
    }

    public void Resume()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gameUI.SetActive(true);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        gameUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void Death()
    {
        Debug.Log("Death");
        deathMenuUI.SetActive(true);
    }

    public void MenuGame()
    {
        SceneManager.LoadScene(0);
    }
}
