using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public SettingsMenu settingsMenu;

    private void Awake()
    {
        settingsMenu.SetFullscreen(PlayerPrefs.GetInt("fullscreen") == 1);
        //settingsMenu.SetResolution(PlayerPrefs.GetInt("resolution"));
        settingsMenu.SetQuality(PlayerPrefs.GetInt("graphicsQuality"));
        settingsMenu.SetMasterVolume(PlayerPrefs.GetFloat("masterVolume"));
        settingsMenu.fullscreenToggle.isOn = PlayerPrefs.GetInt("fullscreen") == 1;
        //settingsMenu.resolutionDropdown.value = PlayerPrefs.GetInt("resolution");
        //settingsMenu.resolutionDropdown.RefreshShownValue();
        settingsMenu.qualityDropdown.value = PlayerPrefs.GetInt("graphicsQuality");
        settingsMenu.masterVolumeSlider.value = PlayerPrefs.GetFloat("masterVolume");
    }

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
