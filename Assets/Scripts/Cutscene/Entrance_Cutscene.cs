using System.Collections;
using System.Collections.Generic;
using FODMapping;
using UnityEngine;

public class Entrance_Cutscene : MonoBehaviour
{
    [Header("Beams")]
    public Transform[] beams;
    public float beamsSpeed = 4;
    public float beamRange = 30;
    
    private Quaternion[] initialRotations;
    
    [Header("Cutscene")]
    public bool isActive = false;
    public GameObject whiteScreen;
    public GameObject mimicBoss;
    
    [Header("Compounds")]
    public Grid_Manager gridManager;
    public Shake_Camera_Manager cameraManager;
    public FOD_Manager manager;
    private Transform player;
    
    void Start()
    {
        initialRotations = new Quaternion[beams.Length];

        for (int i = 0; i < beams.Length; i++)
        {
            initialRotations[i] = beams[i].localRotation;
        }
    }
    
    void LateUpdate()
    {
        for (int i = 0; i < beams.Length; i++)
        {
            if (beams[i] != null)
            {
                float angleOffset = Mathf.Sin(Time.time * beamsSpeed ) * beamRange;
                beams[i].localRotation = initialRotations[i] * Quaternion.Euler(0f, 0f, angleOffset);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive && other.gameObject.CompareTag("Player"))
        {
            player = other.gameObject.transform;
            
            if (player != null)
            {
                isActive = true;
                OnSceneEnd();
            }
        }
    }

    private void OnSceneEnd()
    {
        Music_Manager.instance.PlaySound(Music_Manager.SoundType.LightSwitch);
        Music_Manager.instance.SetToAmbientMusic();

        if (cameraManager != null)
        {
            cameraManager.ShakeCamera(0);
        }
        
        if (manager == null)
        {
            Debug.Log("FOD_Manager is empty");
            return;
        }

        StartCoroutine(DisableWithDelay());
        
        whiteScreen.SetActive(true);
        whiteScreen.GetComponent<Animator>().SetTrigger("FadeIn");
        
        Player_Movement_Manager.Instance.isInvulnerable = true;
        manager.RemoveAgentsGradually(true);
    }

    private IEnumerator DisableWithDelay()
    {
        Player_Movement.Instance.isDisable = true;
        
        yield return new WaitForSeconds(1f);
        
        mimicBoss.SetActive(false);
        
        gridManager.ResetSectorsState();
        
        Player_Movement.Instance.isDisable = false;
    }
}
