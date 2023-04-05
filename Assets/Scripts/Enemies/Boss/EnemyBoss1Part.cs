using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyBoss1Part : EnemyUnit
{
    public GameObject[] m_PartObj = new GameObject[2];
    
    private const float CLOSED_LOCAL_X = 0.65f;
    private const float OPENED_LOCAL_X = 0.8f;
    private bool m_OpenState = false;
    private float m_LocalX = CLOSED_LOCAL_X;

    protected override void Update()
    {
        StateAnimation();
        base.Update();
    }

    public void SetOpenState(bool state) {
        m_OpenState = state;
    }

    public void StateAnimation() {
        if (m_OpenState) {
            if (m_LocalX < OPENED_LOCAL_X) {
                m_LocalX += (OPENED_LOCAL_X - CLOSED_LOCAL_X) / (600 * Application.targetFrameRate / 1000);
            }
            else {
                m_LocalX = OPENED_LOCAL_X;
            }
        }
        else {
            if (m_LocalX > CLOSED_LOCAL_X) {
                m_LocalX -= (OPENED_LOCAL_X - CLOSED_LOCAL_X) / (600 * Application.targetFrameRate / 1000);
            }
            else {
                m_LocalX = CLOSED_LOCAL_X;
            }
        }
        
        m_PartObj[0].transform.localPosition = new Vector3(-m_LocalX, m_PartObj[0].transform.localPosition.y, m_PartObj[0].transform.localPosition.z);
        m_PartObj[1].transform.localPosition = new Vector3(m_LocalX, m_PartObj[1].transform.localPosition.y, m_PartObj[1].transform.localPosition.z);
    }

    protected override void KilledByPlayer() {
        m_SystemManager.BulletsToGems(0);
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        m_SystemManager.EraseBullets(500);
        ((EnemyBoss1) m_ParentEnemy).ToNextPhase();

        ExplosionEffect(0, -1, new Vector3(-0.66f, 0f, 0f));
        ExplosionEffect(0, -1, new Vector3(0.66f, 0f, 0f));
        ExplosionEffect(0, -1, new Vector3(-0.62f, 0f, 0.33f));
        ExplosionEffect(0, -1, new Vector3(0.62f, 0f, 0.33f));
        ExplosionEffect(1, -1, new Vector3(-0.69f, 0f, -0.4f));
        ExplosionEffect(1, -1, new Vector3(0.69f, 0f, -0.4f));
        Destroy(gameObject);
        yield break;
    }
}
