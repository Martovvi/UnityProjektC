using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenuUI : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject[] UIElements;

    public void Awake()
    {
        PauseMenu.SetActive(false);
    }

    private void PauseGame()
    {
        PauseMenu.SetActive(true);

        Time.timeScale = 0;
        
        for (int i = 0; i < UIElements.Length; i++)
        {
            UIElements[i].SetActive(false);
        }

    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        PauseMenu.SetActive(false);
        for (int i = 0; i < UIElements.Length; i++)
        {
            UIElements[i].SetActive(true);
        }

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

