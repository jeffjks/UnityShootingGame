using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss2 : EnemyUnit
{
    [HideInInspector] public byte m_Phase = 0;
    
    private Vector3 m_TargetPosition;
    private Quaternion m_TargetQuaternion;
    private bool m_TimeLimitState = false;

    private IEnumerator m_Pattern1, m_Pattern2;


}
