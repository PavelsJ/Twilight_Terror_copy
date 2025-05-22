using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Save_Manager : MonoBehaviour
{
    public static Save_Manager Instance { get; private set; }
    
    private List<string> saveFiles = new List<string>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        
        LoadSaveList();
    }

    public void SaveScene()
    {
        PlayerPrefs.SetInt("SceneSave", SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.Save();

        SetSave("SceneSave");
    }
    
    public void SetSave(string saveName)
    {
        if (!saveFiles.Contains(saveName))
        {
            saveFiles.Add(saveName);
            SaveSaveList();
        }
    }

    private void SaveSaveList()
    {
        PlayerPrefs.SetString("SaveList", string.Join(",", saveFiles));
        PlayerPrefs.Save();
    }
    
    private void LoadSaveList()
    {
        if (PlayerPrefs.HasKey("SaveList"))
        {
            string savedData = PlayerPrefs.GetString("SaveList");
            saveFiles = new List<string>(savedData.Split(','));
        }
    }

    public void DeleteSavesFromList()
    {
        foreach (string save in saveFiles)
        {
            PlayerPrefs.DeleteKey(save);
        }

        PlayerPrefs.DeleteKey("SaveList");
        saveFiles.Clear();
    }
    
    public void DeleteAllSaves()
    {
        PlayButtonSound();
        PlayerPrefs.DeleteAll();
        Debug.Log("Deleted all saves");
    }
    
    private void PlayButtonSound()
    {
        Music_Manager.instance.PlaySound(Music_Manager.SoundType.Button);
    }
}
