using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Trap_Web : Entity_Trap
{
    protected override void TrapEffect()
    {
        Player_Movement_Manager.Instance.ActivateSpiderChase();
    }
}
