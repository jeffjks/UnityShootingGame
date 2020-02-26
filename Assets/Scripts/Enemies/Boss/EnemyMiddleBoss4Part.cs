using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class EnemyMiddleBoss4Part : EnemyUnit
{
    public Transform m_FirePosition;
    private IEnumerator m_CurrentPattern;
    private float m_Direction;

    void Start()
    {
        GetCoordinates();
    }

    protected override void Update()
    {
        m_Direction -= 20f * Time.deltaTime;
        if (m_Direction < 0f)
            m_Direction += 360f;

        base.Update();
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
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;

        if (m_SystemManager.m_Difficulty == 0) {
            pos = m_FirePosition.position;
            CreateBulletsSector(3, pos, 4.8f, m_Direction, accel, 1, 90f);
            CreateBulletsSector(5, pos, 5.2f, m_Direction, accel, 1, 90f);
        }
        else {
            for (int i = 0; i < 4; i++) {
                pos = m_FirePosition.position;
                CreateBulletsSector(3, pos, 4.8f, m_Direction, accel, 4, 90f);
                CreateBulletsSector(5, pos, 5.2f, m_Direction, accel, 4, 90f);
                yield return new WaitForSeconds(0.1f);
            }
        }
        yield break;
    }

    private IEnumerator Pattern2()
    {
        float timer = 0.3f;
        EnemyBulletAccel accel1 = new EnemyBulletAccel(0f, timer);
        EnemyBulletAccel accel2 = new EnemyBulletAccel(0f, 0f);
        Vector3 pos;
        float[] period = {0.3f, 0.2f, 0.1f};

        while (true) {
            pos = m_FirePosition.position;
            CreateBullet(5, pos, 2.2f, Random.Range(0f, 360f), accel1, 2, timer,
            4, 8f, 0, 0f, accel2);
            yield return new WaitForSeconds(period[m_SystemManager.m_Difficulty]);
        }
    }

    protected override void KilledByPlayer() {
        m_GemNumber = 12;
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        CreateItems();
        ExplosionEffect(Random.Range(0, 2), -1, new Vector3(Random.Range(-0.1f, 0.3f), 0f, 1.6f));
        ExplosionEffect(Random.Range(0, 2), -1, new Vector3(Random.Range(-0.1f, 0.3f), 0f, 0.6f));
        ExplosionEffect(Random.Range(0, 2), -1, new Vector3(Random.Range(-0.1f, 0.3f), 0f, -0.4f));
        ExplosionEffect(Random.Range(0, 2), -1, new Vector3(Random.Range(-0.1f, 0.3f), 0f, -1.4f));
        Destroy(gameObject);
        yield break;
    }
}
