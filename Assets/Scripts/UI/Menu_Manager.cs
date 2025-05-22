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
        PlayButtonSound();
        
        SceneManager.LoadScene(sceneName);
    }

    public void OpenRecentScene()
    {
        PlayButtonSound();
        
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
        PlayButtonSound();
        Application.Quit();
    }

    private void PlayButtonSound()
    {
        Music_Manager.instance.PlaySound(Music_Manager.SoundType.Button);
    }
}
