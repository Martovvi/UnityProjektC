using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenuUI : MonoBehaviour
{
    public GameObject PauseMenu;

    public void Awake()
    {
        PauseMenu.SetActive(false);
    }

    private void PauseGame()
    {
        PauseMenu.SetActive(true);

        Time.timeScale = 0;

    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        PauseMenu.SetActive(false);

    }
    public void BackToMainMenu()
    {

        SceneManager.LoadScene("MainMenu");

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseMenu.activeSelf)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
}

