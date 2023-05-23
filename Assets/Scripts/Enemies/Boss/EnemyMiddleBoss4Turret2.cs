using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss4Turret2 : EnemyUnit
{
    public Transform m_FirePosition;
    
    private IEnumerator m_CurrentPattern;

    void Start()
    {
        RotateImmediately(m_PlayerPosition + new Vector2(0f, 1.5f));
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateImmediately(m_PlayerPosition + new Vector2(0f, 1.5f));
        else
            RotateSlightly(m_PlayerPosition + new Vector2(0f, 1.5f), 100f);
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
        yield return new WaitForMillisecondFrames(1500);

        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            while(true) {
                CreateBullet(4, m_FirePosition.position, 4f, m_CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(2000 + Random.Range(0, 1000));
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            while(true) {
                yield return new WaitForMillisecondFrames(1200 + Random.Range(0, 500));
            }
        }
        else {
            while(true) {
                yield return new WaitForMillisecondFrames(1000 + Random.Range(0, 400));
            }
        }
    }

    private IEnumerator Pattern2()
    {
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            EnemyBulletAccel accel = new EnemyBulletAccel(5.5f, 1000);
            while(true) {
                CreateBullet(4, m_FirePosition.position, 2f, m_CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(200);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            while(true) {
                yield return new WaitForMillisecondFrames(50);
            }
        }
        else {
            while(true) {
                yield return new WaitForMillisecondFrames(50);
            }
        }
    }
}
