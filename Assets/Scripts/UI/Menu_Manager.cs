using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu_Manager : MonoBehaviour
{
    public int defaultSceneIndex = 1;
    public Button continueButton;

    private void Awake()
    {
        Time.timeScale = 1;
    }

    private void Start()
    {
        if(continueButton == null) return;
        continueButton.interactable = PlayerPrefs.HasKey("SceneSave");
    }

    public void OpenScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OpenRecentScene()
    {
        if (PlayerPrefs.HasKey("SceneSave"))
        {
            SceneManager.LoadScene(PlayerPrefs.GetInt("SceneSave"));
        }
        else
        {
            SceneManager.LoadScene(defaultSceneIndex);
        }
    }
    
    public void QuitApp()
    {
        Application.Quit();
    }
}
