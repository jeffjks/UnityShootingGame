using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss4Part : EnemyUnit
{
    public Transform m_FirePosition;
    public EnemyItemCreater m_EnemyItemCreater;
    private IEnumerator m_CurrentPattern;
    private float m_Direction;
    
    protected override void Update()
    {
        base.Update();
        
        m_Direction -= 20f / Application.targetFrameRate * Time.timeScale;
        if (m_Direction < 0f)
            m_Direction += 360f;
        
        m_EnemyHealth.Action_OnHealthChanged += DestroyBonus;
    }

    public void StartPattern(byte num) {
        if (num == 1)
            m_CurrentPattern = Pattern1();
        else if (num == 2)
            m_CurrentPattern = Pattern2();
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern() {
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
    }

    private IEnumerator Pattern1()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos;

        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            pos = m_FirePosition.position;
            CreateBulletsSector(3, pos, 4.8f, m_Direction, accel, 1, 90f);
            CreateBulletsSector(5, pos, 5.2f, m_Direction, accel, 1, 90f);
        }
        else {
            for (int i = 0; i < 4; i++) {
                pos = m_FirePosition.position;
                CreateBulletsSector(3, pos, 4.8f, m_Direction, accel, 4, 90f);
                CreateBulletsSector(5, pos, 5.2f, m_Direction, accel, 4, 90f);
                yield return new WaitForMillisecondFrames(100);
            }
        }
        yield break;
    }

    private IEnumerator Pattern2()
    {
        int timer = 300;
        EnemyBulletAccel accel1 = new EnemyBulletAccel(0f, timer);
        EnemyBulletAccel accel2 = new EnemyBulletAccel(0f, 0);
        Vector3 pos;
        int[] period = { 300, 200, 100 };

        while (true) {
            pos = m_FirePosition.position;
            CreateBullet(5, pos, 2.2f, Random.Range(0f, 360f), accel1, OldBulletType.ERASE_AND_CREATE, timer,
            4, 8f, 0, 0f, accel2);
            yield return new WaitForMillisecondFrames(period[(int) SystemManager.Difficulty]);
        }
    }

    private void DestroyBonus() {
        if (m_EnemyHealth.CurrentHealth == 0) {
            m_EnemyItemCreater.enabled = true;
        }
    }
}
