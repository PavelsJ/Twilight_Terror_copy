using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_FlashLight : UI_Item
{
    public override void UseItem()
    {
        GameObject obj = Player_Movement.Instance.gameObject;
        obj.GetComponent<Player_Cutscene>().ActivateFlashLight();
    }
}
