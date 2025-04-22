using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Extra_Steps : MonoBehaviour
{
    public int extraSteps;
    public int extraLives;

    public int GetSteps()
    {
        RemoveItem();
        return extraSteps;
    }
    
    public int GetLives()
    {
        RemoveItem();
        return extraLives;
    }

    private void RemoveItem()
    {
        UI_Inventory.Instance.RemoveItem();
    }
}
