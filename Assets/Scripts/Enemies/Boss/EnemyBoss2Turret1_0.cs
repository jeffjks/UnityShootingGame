using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss2Turret1_0 : EnemyUnit
{
    public Transform m_FirePosition;

    [HideInInspector] public bool m_InPattern = false;
    
    private IEnumerator m_CurrentPattern;

    void Start()
    {
        RotateImmediately(PlayerManager.GetPlayerPosition());
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
        StartCoroutine(m_CurrentPattern);
    }

    public void StopPattern() {
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
    }

    private IEnumerator Pattern1()
    {
        int duration = 800;
        BulletAccel accel1 = new BulletAccel(3f, duration);
        BulletAccel accel2 = new BulletAccel(7.6f, 600);
        Vector3 pos;
        m_InPattern = true;

        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            int[] num = { 1, 2, 3, 3};
            for (int k = 0; k < 3; k++) {
                for (int i = 0; i < num[k]; i++) {
                    pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                    for (int j = 0; j < 7; j++) {
                        CreateBullet(0, pos, 13.7f, CurrentAngle - 18f*3.5f + j*18f, accel1, BulletSpawnType.EraseAndCreate, duration,
                        3, 3f, BulletPivot.Current, Random.Range(-6f, 6f), accel2);
                    }
                    yield return new WaitForMillisecondFrames(300);
                }
                yield return new WaitForMillisecondFrames(1500);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            int[] num = { 3, 5, 12 };
            for (int k = 0; k < 3; k++) {
                for (int i = 0; i < num[k]; i++) {
                    pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                    for (int j = 0; j < 13; j++) {
                        CreateBullet(0, pos, 13.7f, CurrentAngle - 12f*6.5f + j*12f, accel1, BulletSpawnType.EraseAndCreate, duration,
                        3, 3f, BulletPivot.Current, Random.Range(-7f, 7f), accel2);
                    }
                    yield return new WaitForMillisecondFrames(200);
                }
                yield return new WaitForMillisecondFrames(1200);
            }
        }
        else {
            int[] num = { 3, 5, 12 };
            for (int k = 0; k < 3; k++) {
                for (int i = 0; i < num[k]; i++) {
                    pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                    for (int j = 0; j < 15; j++) {
                        CreateBullet(0, pos, 13.7f, CurrentAngle - 10f*7.5f + j*10f, accel1, BulletSpawnType.EraseAndCreate, 1000,
                        3, 3f, BulletPivot.Current, Random.Range(-8f, 8f), accel2);
                    }
                    yield return new WaitForMillisecondFrames(200);
                }
                yield return new WaitForMillisecondFrames(900);
            }
        }
        m_InPattern = false;
        yield break;
    }
}