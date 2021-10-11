using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public static bool ShootingMode = false;

    public GameObject pauseMenuUI, mainCam, shootingCam;

    // Update is called once per frame
    void Update()
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

        if (Input.GetKeyDown(KeyCode.C))
        {
            if (ShootingMode)
            {
                mainCam.SetActive(true);
                shootingCam.SetActive(false);
                ShootingMode = false;
            }
            else
            {
                mainCam.SetActive(false);
                shootingCam.SetActive(true);
                ShootingMode = true;
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void MenuGame()
    {
        SceneManager.LoadScene(0);
    }
}
