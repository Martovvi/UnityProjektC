using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool isGameOver = false;
    public bool isLevelCompleted = false;
    public int enemyCount = 0;
    public GameObject gameOverUI;
    public GameObject levelCompletedUI;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (isGameOver && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void GameOver(bool gameOver)
    {
        isGameOver = gameOver;
        gameOverUI.SetActive(true);
    }

    public void LevelComplete()
    {
        if (enemyCount == 0)
        {
            isLevelCompleted = true;
            levelCompletedUI.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
