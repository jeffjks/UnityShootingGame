using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBoss4SubTurret : EnemyUnit
{
    public Transform m_FirePosition;
    
    private IEnumerator m_CurrentPattern;
    [HideInInspector] public byte m_RotatePattern = 10;

    void Start()
    {
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        base.Update();
        
        switch (m_RotatePattern) {
            case 10:
                if (PlayerManager.IsPlayerAlive) {
                    RotateSlightly(m_PlayerPosition, 130f);
                }
                else {
                    RotateSlightly(m_PlayerPosition, 100f);
                }
                break;
            
            case 20:
                RotateSlightly(0f, 150f);
                break;
            case 30:
                RotateSlightly(m_CurrentAngle + 90f, 140f);
                break;
            case 31:
                RotateSlightly(270f, 150f);
                break;
            case 32:
                RotateSlightly(90f, 150f);
                break;
            case 41:
                RotateSlightly(m_PlayerPosition, 180f, -70f);
                break;
            case 42:
                RotateSlightly(m_PlayerPosition, 180f, 70f);
                break;
            case 43:
                RotateSlightly(m_CurrentAngle + 90f, 280f);
                break;
            case 44:
                RotateSlightly(m_CurrentAngle - 90f, 280f);
                break;
        }
    }

    public void StartPattern(byte num) {
        if (num == 1)
            m_CurrentPattern = Pattern1();
        else if (num == 2)
            m_CurrentPattern = Pattern2();
        else if (num == 3)
            m_CurrentPattern = Pattern3();
        else if (num == 4)
            m_CurrentPattern = Pattern4();
        else if (num == 5)
            m_CurrentPattern = Pattern5();
        else if (num == 6)
            m_CurrentPattern = Pattern6();
        else if (num == 7)
            m_CurrentPattern = Pattern7();
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
        Vector3 pos0, pos1, pos2;
        float gap = 0.6f, rand = Random.Range(-3f, 3f);
        m_RotatePattern = 0;
        
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            pos0 = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
            pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
            pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
            CreateBulletsSector(4, pos0, 6f, m_CurrentAngle + rand, accel, 3, 12f);
            CreateBulletsSector(4, pos1, 6f, m_CurrentAngle + rand - 27f, accel, 2, 10f);
            CreateBulletsSector(4, pos2, 6f, m_CurrentAngle + rand + 27f, accel, 2, 10f);
        }
        else {
            for (int i = 0; i < 3; i++) {
                pos0 = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
                pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
                if (SystemManager.Difficulty == GameDifficulty.Expert) {
                    CreateBulletsSector(4, pos0, 7f, m_CurrentAngle + rand, accel, 3, 10f);
                    CreateBulletsSector(4, pos1, 7f, m_CurrentAngle + rand - 26f, accel, 3, 8f);
                    CreateBulletsSector(4, pos2, 7f, m_CurrentAngle + rand + 26f, accel, 3, 8f);
                    yield return new WaitForMillisecondFrames(50);
                }
                else {
                    CreateBulletsSector(4, pos0, 8f, m_CurrentAngle + rand, accel, 3, 8f);
                    CreateBulletsSector(4, pos1, 8f, m_CurrentAngle + rand - 20f, accel, 3, 6f);
                    CreateBulletsSector(4, pos2, 8f, m_CurrentAngle + rand + 20f, accel, 3, 6f);
                    yield return new WaitForMillisecondFrames(50);
                }
            }
        }
        m_RotatePattern = 10;
        yield break;
    }

    private IEnumerator Pattern2()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos0, pos1, pos2;
        float gap = 0.6f;
        
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            pos0 = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
            pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
            pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
            CreateBullet(1, pos0, 5.4f, m_CurrentAngle, accel);
            CreateBullet(1, pos1, 5.4f, m_CurrentAngle, accel);
            CreateBullet(1, pos2, 5.4f, m_CurrentAngle, accel);
        }
        else {
            for (int i = 0; i < 5; i++) {
                pos0 = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
                pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
                if (SystemManager.Difficulty == GameDifficulty.Expert) {
                    CreateBullet(1, pos0, 5f+i*0.8f, m_CurrentAngle, accel);
                    CreateBullet(1, pos1, 5f+i*0.8f, m_CurrentAngle, accel);
                    CreateBullet(1, pos2, 5f+i*0.8f, m_CurrentAngle, accel);
                    yield return new WaitForMillisecondFrames(50);
                }
                else {
                    CreateBullet(1, pos0, 5f+i*0.8f, m_CurrentAngle, accel);
                    CreateBullet(1, pos1, 5f+i*0.8f, m_CurrentAngle, accel);
                    CreateBullet(1, pos2, 5f+i*0.8f, m_CurrentAngle, accel);
                    yield return new WaitForMillisecondFrames(20);
                }
            }
        }
        yield break;
    }

    private IEnumerator Pattern3()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos0, pos1, pos2;
        float gap = 0.6f;
        
        while (true) {
            pos0 = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
            pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
            pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                CreateBulletsSector(3, pos0, 6.2f, m_CurrentAngle, accel, 5, 15f);
                yield return new WaitForMillisecondFrames(450);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBullet(3, pos0, 5.7f, m_CurrentAngle, accel);
                CreateBullet(3, pos1, 5.7f, m_CurrentAngle - 15f, accel);
                CreateBullet(3, pos2, 5.7f, m_CurrentAngle + 15f, accel);
                CreateBulletsSector(5, pos0, 6.7f, m_CurrentAngle, accel, 5, 13f);
                yield return new WaitForMillisecondFrames(250);
            }
            else {
                CreateBullet(3, pos0, 5.7f, m_CurrentAngle, accel);
                CreateBullet(3, pos1, 5.7f, m_CurrentAngle - 15f, accel);
                CreateBullet(3, pos2, 5.7f, m_CurrentAngle + 15f, accel);
                CreateBulletsSector(5, pos0, 6.7f, m_CurrentAngle, accel, 5, 12f);
                yield return new WaitForMillisecondFrames(180);
            }
        }
    }

    private IEnumerator Pattern4()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos0, pos1, pos2;
        float gap = 0.6f, rand = Random.Range(-3f, 3f);
        m_RotatePattern = 0;
        
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            pos0 = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
            pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
            pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
            CreateBulletsSector(4, pos0, 6f, m_CurrentAngle + rand, accel, 3, 10f);
            CreateBulletsSector(4, pos1, 6f, m_CurrentAngle + rand - 19f, accel, 2, 8f);
            CreateBulletsSector(4, pos2, 6f, m_CurrentAngle + rand + 19f, accel, 2, 8f);
        }
        else {
            for (int i = 0; i < 3; i++) {
                pos0 = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
                pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
                pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
                if (SystemManager.Difficulty == GameDifficulty.Expert) {
                    CreateBulletsSector(4, pos0, 7.25f, m_CurrentAngle + rand, accel, 4, 6f);
                    CreateBulletsSector(4, pos1, 7.25f, m_CurrentAngle + rand - 23f, accel, 4, 6f);
                    CreateBulletsSector(4, pos2, 7.25f, m_CurrentAngle + rand + 23f, accel, 4, 6f);
                    yield return new WaitForFrames(3);
                }
                else {
                    CreateBulletsSector(4, pos0, 8.5f, m_CurrentAngle + rand, accel, 4, 6f);
                    CreateBulletsSector(4, pos1, 8.5f, m_CurrentAngle + rand - 23f, accel, 4, 6f);
                    CreateBulletsSector(4, pos2, 8.5f, m_CurrentAngle + rand + 23f, accel, 4, 6f);
                    yield return new WaitForFrames(3);
                }
            }
        }
        m_RotatePattern = 10;
        yield break;
    }

    private IEnumerator Pattern5()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos0, pos1, pos2;
        float gap = 0.6f;
        int frame = 0;
        int[] frame_add = {4, 3, 2};

        m_RotatePattern += 2;
        
        while (frame < 30) {
            pos0 = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
            pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(gap, 0f, 0f)));
            pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition.TransformPoint(new Vector3(-gap, 0f, 0f)));
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                CreateBullet(5, pos1, 5.6f, m_CurrentAngle - 5f, accel);
                CreateBullet(5, pos2, 6.8f, m_CurrentAngle + 5f, accel);
                CreateBullet(5, pos0, 8f, m_CurrentAngle, accel);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBullet(3, pos1, 6f, m_CurrentAngle - 5f, accel);
                CreateBullet(3, pos2, 7.2f, m_CurrentAngle + 5f, accel);
                CreateBullet(3, pos0, 8.4f, m_CurrentAngle, accel);
            }
            else {
                CreateBullet(3, pos0, 5.4f, m_CurrentAngle, accel);
                CreateBullet(3, pos1, 6.6f, m_CurrentAngle - 5f, accel);
                CreateBullet(3, pos2, 7.8f, m_CurrentAngle + 5f, accel);
                CreateBullet(3, pos0, 9f, m_CurrentAngle, accel);
            }
            frame += frame_add[(int) SystemManager.Difficulty];
            yield return new WaitForFrames(frame_add[(int) SystemManager.Difficulty]);
        }
        m_RotatePattern = 10;
        yield break;
    }

    private IEnumerator Pattern6()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos;
        float rand;
        
        while (true) {
            pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
            rand = Random.Range(-24f, 22f);
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                for (int j = 0; j < 2; j++) {
                    CreateBullet(1, pos, 4f + j*0.15f, m_CurrentAngle + rand, accel);
                }
                yield return new WaitForMillisecondFrames(370);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                for (int j = 0; j < 3; j++) {
                    CreateBullet(1, pos, 4f + j*0.15f, m_CurrentAngle + rand, accel);
                }
                yield return new WaitForMillisecondFrames(140);
            }
            else {
                for (int j = 0; j < 4; j++) {
                    CreateBullet(1, pos, 4f + j*0.15f, m_CurrentAngle + rand, accel);
                }
                yield return new WaitForMillisecondFrames(80);
            }
        }
    }

    private IEnumerator Pattern7()
    {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos;
        
        pos = BackgroundCamera.GetScreenPosition(m_FirePosition.position);
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            CreateBulletsSector(4, pos, 4.5f, m_CurrentAngle, accel, 3, 13f);
            yield return new WaitForMillisecondFrames(330);
            CreateBulletsSector(4, pos, 4.5f, m_CurrentAngle, accel, 5, 13f);
            yield return new WaitForMillisecondFrames(330);
            CreateBulletsSector(4, pos, 4.5f, m_CurrentAngle, accel, 8, 13f);
            yield return new WaitForMillisecondFrames(330);
            CreateBulletsSector(4, pos, 4.5f, m_CurrentAngle, accel, 9, 13f);
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            CreateBulletsSector(4, pos, 4.5f, m_CurrentAngle, accel, 5, 9f);
            yield return new WaitForMillisecondFrames(250);
            CreateBulletsSector(4, pos, 4.5f, m_CurrentAngle, accel, 8, 9f);
            yield return new WaitForMillisecondFrames(250);
            CreateBulletsSector(4, pos, 4.5f, m_CurrentAngle, accel, 11, 9f);
            yield return new WaitForMillisecondFrames(250);
            CreateBulletsSector(4, pos, 4.5f, m_CurrentAngle, accel, 12, 9f);
            yield return new WaitForMillisecondFrames(250);
            CreateBulletsSector(4, pos, 4.5f, m_CurrentAngle, accel, 13, 9f);
        }
        else {
            CreateBulletsSector(4, pos, 4.5f, m_CurrentAngle, accel, 5, 7.5f);
            yield return new WaitForMillisecondFrames(190);
            CreateBulletsSector(4, pos, 4.5f, m_CurrentAngle, accel, 8, 7.5f);
            yield return new WaitForMillisecondFrames(190);
            CreateBulletsSector(4, pos, 4.5f, m_CurrentAngle, accel, 11, 7.5f);
            yield return new WaitForMillisecondFrames(190);
            CreateBulletsSector(4, pos, 4.5f, m_CurrentAngle, accel, 14, 7.5f);
            yield return new WaitForMillisecondFrames(190);
            CreateBulletsSector(4, pos, 4.5f, m_CurrentAngle, accel, 15, 7.5f);
            yield return new WaitForMillisecondFrames(190);
            CreateBulletsSector(4, pos, 4.5f, m_CurrentAngle, accel, 16, 7.5f);
        }
        yield break;
    }
}