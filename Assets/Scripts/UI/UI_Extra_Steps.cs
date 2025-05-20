using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Extra_Steps : UI_Item
{
    public int extraSteps;
    public int extraLives;

    public override void UseItem()
    {
        Player_Movement_Manager.Instance.AddSteps(extraSteps);

        if (extraLives> 0)
        {
            Player_Movement_Manager.Instance.AddLife();
        }
    }

    private void RemoveItem()
    {
        UI_Inventory.Instance.RemoveItem();
    }
}
