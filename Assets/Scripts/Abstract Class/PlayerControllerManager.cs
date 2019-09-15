using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerManager : PlayerDamageUnit
{
    [HideInInspector] public bool m_SlowMode = false;
    
    public override void OnDeath() { // Override
        return;
    }
}