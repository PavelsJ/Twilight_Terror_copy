using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mimic_Cutscene4_Script : MonoBehaviour
{
    public float cutsceneDuration = 3f;
    
    public GameObject cutsceneCamera;
    public GameObject mainCamera;
    
    public Enemy_Mimic_Movement mimicMovement;
    private bool isActive;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!isActive && mimicMovement != null)
            {
                isActive = true;
                StartCoroutine(OnCutscene());
                mimicMovement.ActivateMimic();
               
            }
        }
    }

    private IEnumerator OnCutscene()
    {
        Player_Movement.Instance.isDisable = true;
        cutsceneCamera.SetActive(true);
        mainCamera.SetActive(false);

        yield return new WaitForSeconds(cutsceneDuration);

        cutsceneCamera.SetActive(false);
        mainCamera.SetActive(true);
        
        yield return new WaitForSeconds(0.2f);
        
        Player_Movement.Instance.isDisable = false;
    }
}
