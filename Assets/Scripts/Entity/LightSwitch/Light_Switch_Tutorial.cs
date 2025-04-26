using System;
using System.Collections;
using System.Collections.Generic;
using FODMapping;
using UnityEngine;

public class Light_Switch_Tutorial : Light_Switch_Interaction
{
    [Header("Hints")]
    public GameObject hintToShow;
    public GameObject hintToHide;

    protected override void HandleHintsInteraction()
    {
        SetActiveIfNotNull(hintToShow, true);
        SetActiveIfNotNull(hintToHide, false);
    }
    
    private void SetActiveIfNotNull(GameObject obj, bool state)
    {
        if (obj != null)
        {
            obj.SetActive(state);
        }
    }
}
