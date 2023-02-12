using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    public GameObject PauseMenu;
    public GameObject[] UIElements;
    public PlayerMovement playerScript;
    public GameManager gameManager;
    [SerializeField] private AudioSource openMenuSound;
    [SerializeField] private AudioSource closeMenuSound;

    public void Awake()
    {
        PauseMenu.SetActive(false);
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseMenu.activeSelf && playerScript.isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        Cursor.lockState = CursorLockMode.Confined;
        playerScript.isGamePaused = true;
        PauseMenu.SetActive(true);

        Time.timeScale = 0;
        
        for (int i = 0; i < UIElements.Length; i++)
        {
            UIElements[i].SetActive(false);
        }

        if (openMenuSound != null)
        {
            openMenuSound.Play();
        }
    }

    public void ResumeGame()
    {
        if (!gameManager.isLevelCompleted)
        { 
            Cursor.lockState = CursorLockMode.Locked;
        }
        playerScript.isGamePaused = false;
        Time.timeScale = 1;
        PauseMenu.SetActive(false);
        for (int i = 0; i < UIElements.Length; i++)
        {
            UIElements[i].SetActive(true);
        }
        
        if (closeMenuSound != null)
        {
            closeMenuSound.Play();
        }

    }
    public void BackToMainMenu()
    {

        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;

    }
}

