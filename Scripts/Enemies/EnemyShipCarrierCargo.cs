using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipCarrierCargo : EnemyUnit
{
    void Start()
    {
        m_UpdateTransform = false;
        GetCoordinates();
    }
}
