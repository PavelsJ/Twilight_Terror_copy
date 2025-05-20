using System.Collections;
using System.Collections.Generic;
using FODMapping;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bed_Interaction : MonoBehaviour
{
    public bool endScene = false;
    
    public GameObject endScreen;
    
    private FOD_Manager manager;
    
    private void Start()
    {
        endScreen.SetActive(false);
        manager = FindObjectOfType<FOD_Manager>(true).GetComponent<FOD_Manager>();
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (endScene && other.gameObject.CompareTag("Player"))
        {
            EndNight();
        }
    }

    private void SaveNight()
    {
        if (!PlayerPrefs.HasKey("NightCount"))
        {
            PlayerPrefs.SetInt("NightCount", 1);
           
        }
        else
        {
            int nightCount = PlayerPrefs.GetInt("NightCount");
            nightCount += 1;
            
            PlayerPrefs.SetInt("NightCount", nightCount);
        }
        
        Save_Manager.Instance.SetSave("NightCount");
        PlayerPrefs.Save();
        
        Debug.Log("NightCount: " + PlayerPrefs.GetInt("NightCount"));
    }

    public void EndNight()
    {
        SaveNight();
        StartCoroutine(OnNextScene());
    }
    
    private IEnumerator OnNextScene()
    {
        if (manager != null)
        {
            Player_Movement.Instance.isDisable = true;
            manager.GetComponent<SpriteRenderer>().color = Color.white;
            manager.SetFogVisibility(true);
            yield return new WaitForSeconds(1.2f);
            
            endScreen.SetActive(true);
        }
    }
}
