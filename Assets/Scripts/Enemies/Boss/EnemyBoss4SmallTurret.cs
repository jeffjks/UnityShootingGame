using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss4SmallTurret : EnemyUnit
{
    public Transform m_FirePosition;
    
    private IEnumerator m_CurrentPattern;
    private int m_KillScore = 0;

    void Start()
    {
        RotateImmediately(PlayerManager.GetPlayerPosition());
        m_KillScore = m_Score;
        m_Score = 0;
        
        m_EnemyHealth.Action_OnHealthChanged += DestroyBonus;
    }

    protected override void Update()
    {
        base.Update();
        
        if (PlayerManager.IsPlayerAlive)
            RotateImmediately(PlayerManager.GetPlayerPosition());
        else
            RotateSlightly(PlayerManager.GetPlayerPosition(), 100f);
    }

    public void StartPattern(byte num) {
        if (num == 1)
            m_CurrentPattern = Pattern1();
        else
            return;
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

        while(true) {
            pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                CreateBullet(2, pos, 4f, CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(3000);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBullet(2, pos, 4f, CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(2000);
            }
            else {
                CreateBullet(2, pos, 4f, CurrentAngle, accel);
                yield return new WaitForMillisecondFrames(1000);
            }
        }
    }

    private void DestroyBonus() {
        if (m_EnemyHealth.CurrentHealth == 0) {
            m_Score = m_KillScore;
        }
    }
}