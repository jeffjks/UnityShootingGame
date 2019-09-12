using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class EnemyBoss1Part : EnemyUnit
{
    public GameObject[] m_PartObj = new GameObject[2];

    public void OpenPart() {
        m_PartObj[0].transform.DOLocalMoveX(-0.8f, 0.6f).SetEase(Ease.Linear);
        m_PartObj[1].transform.DOLocalMoveX(0.8f, 0.6f).SetEase(Ease.Linear);
    }

    public void ClosePart() {
        m_PartObj[0].transform.DOLocalMoveX(-0.65f, 0.6f).SetEase(Ease.Linear);
        m_PartObj[1].transform.DOLocalMoveX(0.65f, 0.6f).SetEase(Ease.Linear);
    }

    protected override void KilledByPlayer() {
        m_SystemManager.BulletsToGems(0f);
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        m_SystemManager.EraseBullets(0.5f);
        ((EnemyBoss1) m_ParentEnemy).ToNextPhase();

        ExplosionEffect(0, -1, new Vector3(-0.66f, 0f, 0f));
        ExplosionEffect(0, -1, new Vector3(0.66f, 0f, 0f));
        ExplosionEffect(0, -1, new Vector3(-0.62f, 0f, 0.33f));
        ExplosionEffect(0, -1, new Vector3(0.62f, 0f, 0.33f));
        ExplosionEffect(1, -1, new Vector3(-0.69f, 0f, -0.4f));
        ExplosionEffect(1, -1, new Vector3(0.69f, 0f, -0.4f));
        Destroy(gameObject);
        yield return null;
    }
}
