using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class EnemyBoss3Barrel : MonoBehaviour
{
    private float m_DefaultZ;
    private Sequence m_Sequence;

    void Start()
    {
        m_DefaultZ = transform.position.z;
    }

    public void BarrelShotAnimation(float target_z) {
        m_Sequence = DOTween.Sequence()
        .Append(transform.DOLocalMoveZ(target_z, 0.1f))
        .Append(transform.DOLocalMoveZ(m_DefaultZ, 0.5f));
    }
}
