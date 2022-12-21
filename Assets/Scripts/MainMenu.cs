using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("TestScene");
    }

    public void CloseGame()
    {
        if (Application.isEditor)
        {
            EditorApplication.isPlaying = false;

        }
        else
        {
         
            Application.Quit();
        }


    }
}
