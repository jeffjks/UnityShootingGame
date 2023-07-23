using System.Collections;
using System;
using UnityEngine;

public class EnemyTankLarge1Turret1 : EnemyUnit
{
    private sbyte m_State = -1;

    public event Func<float> Func_GetDirection;

    void Start()
    {
        RotateUnit(AngleToPlayer);
    }

    protected override void Update()
    {
        base.Update();

        if (m_State == 0) {
            RotateSlightly(PlayerManager.GetPlayerPosition(), 100f, -48f);
        }
        else if (m_State == 1) {
            RotateSlightly(CurrentAngle + 20f, 90f);
        }
        else if (m_State == 2) {
            RotateSlightly(PlayerManager.GetPlayerPosition(), 100f, 48f);
        }
        else if (m_State == 3) {
            RotateSlightly(CurrentAngle - 20f, 90f);
        }
        else {
            RotateSlightly(PlayerManager.GetPlayerPosition(), 100f, -48f);
        }
    }

    public void ToNextPhase() {
        if (m_State == -1) {
            StartCoroutine(Pattern1());
        }
    }

    
    private IEnumerator Pattern1() {
        Vector3[] pos = new Vector3[2];
        BulletAccel accel1 = new BulletAccel(0f, 0);
        BulletAccel accel2 = new BulletAccel(0f, 500);
        BulletAccel accel3 = new BulletAccel(7.4f, 800);

        while(true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                m_State = 1;
                for (int i = 0; i < 7; i++) {
                    pos[0] = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                    CreateBullet(5, pos[0], 6.5f, CurrentAngle, accel1);
                    yield return new WaitForMillisecondFrames(137);
                }
                m_State = 2;
                yield return new WaitForMillisecondFrames(900);
                for (int j = 0; j < 3; j++) {
                    pos[1] = BackgroundCamera.GetScreenPosition(m_FirePosition[1].position);
                    CreateBullet(0, pos[1], UnityEngine.Random.Range(5f, 12f), Func_GetDirection.Invoke() + UnityEngine.Random.Range(-20f, 20f), accel2,
                        BulletSpawnType.EraseAndCreate, 500, 0, 0.1f, BulletPivot.Player, UnityEngine.Random.Range(-25f, 25f), accel3);
                    yield return new WaitForMillisecondFrames(320);
                }
                yield return new WaitForMillisecondFrames(750);
                m_State = 3;
                for (int i = 0; i < 7; i++) {
                    pos[0] = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                    CreateBullet(5, pos[0], 6.5f, CurrentAngle, accel1);
                    yield return new WaitForMillisecondFrames(137);
                }
                m_State = 0;
                yield return new WaitForMillisecondFrames(750);
                for (int j = 0; j < 3; j++) {
                    pos[1] = BackgroundCamera.GetScreenPosition(m_FirePosition[1].position);
                    CreateBullet(0, pos[1], UnityEngine.Random.Range(5f, 12f), Func_GetDirection.Invoke() + UnityEngine.Random.Range(-20f, 20f), accel2,
                        BulletSpawnType.EraseAndCreate, 500, 0, 0.1f, BulletPivot.Player, UnityEngine.Random.Range(-25f, 25f), accel3);
                    yield return new WaitForMillisecondFrames(320);
                }
                yield return new WaitForMillisecondFrames(900);
            }

            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                m_State = 1;
                for (int i = 0; i < 10; i++) {
                    for (int j = 0; j < 5; j++) {
                        pos[0] = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                        CreateBullet(5, pos[0], 4.2f + j*0.8f, CurrentAngle - j*3f, accel1);
                    }
                    yield return new WaitForMillisecondFrames(96);
                }
                m_State = 2;
                yield return new WaitForMillisecondFrames(800);
                for (int j = 0; j < 3; j++) {
                    for (int k = 0; k < 3; k++) {
                        pos[1] = BackgroundCamera.GetScreenPosition(m_FirePosition[1].position);
                        CreateBullet(0, pos[1], UnityEngine.Random.Range(5f, 12f), Func_GetDirection.Invoke() + UnityEngine.Random.Range(-20f, 20f), accel2,
                            BulletSpawnType.EraseAndCreate, 500, 0, 0.1f, BulletPivot.Player, UnityEngine.Random.Range(-25f, 25f), accel3);
                    }
                    yield return new WaitForMillisecondFrames(240);
                }
                yield return new WaitForMillisecondFrames(600);
                m_State = 3;
                for (int i = 0; i < 10; i++) {
                    for (int j = 0; j < 5; j++) {
                        pos[0] = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                        CreateBullet(5, pos[0], 4.2f + j*0.8f, CurrentAngle + j*3f, accel1);
                    }
                    yield return new WaitForMillisecondFrames(96);
                }
                m_State = 0;
                yield return new WaitForMillisecondFrames(600);
                for (int j = 0; j < 3; j++) {
                    for (int k = 0; k < 3; k++) {
                        pos[1] = BackgroundCamera.GetScreenPosition(m_FirePosition[1].position);
                        CreateBullet(0, pos[1], UnityEngine.Random.Range(5f, 12f), Func_GetDirection.Invoke() + UnityEngine.Random.Range(-20f, 20f), accel2,
                            BulletSpawnType.EraseAndCreate, 500, 0, 0.1f, BulletPivot.Player, UnityEngine.Random.Range(-25f, 25f), accel3);
                    }
                    yield return new WaitForMillisecondFrames(240);
                }
                yield return new WaitForMillisecondFrames(800);
            }

            else {
                m_State = 1;
                for (int i = 0; i < 12; i++) {
                    for (int j = 0; j < 6; j++) {
                        pos[0] = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                        CreateBullet(5, pos[0], 4f + j*0.8f, CurrentAngle - j*3f, accel1);
                    }
                    yield return new WaitForMillisecondFrames(80);
                }
                m_State = 2;
                yield return new WaitForMillisecondFrames(800);
                for (int j = 0; j < 3; j++) {
                    for (int k = 0; k < 3; k++) {
                        pos[1] = BackgroundCamera.GetScreenPosition(m_FirePosition[1].position);
                        CreateBullet(0, pos[1], UnityEngine.Random.Range(5f, 12f), Func_GetDirection.Invoke() + UnityEngine.Random.Range(-20f, 20f), accel2,
                            BulletSpawnType.EraseAndCreate, 500, 0, 0.1f, BulletPivot.Player, UnityEngine.Random.Range(-25f, 25f), accel3);
                    }
                    yield return new WaitForMillisecondFrames(240);
                }
                yield return new WaitForMillisecondFrames(600);
                m_State = 3;
                for (int i = 0; i < 12; i++) {
                    for (int j = 0; j < 6; j++) {
                        pos[0] = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
                        CreateBullet(5, pos[0], 4f + j*0.8f, CurrentAngle + j*3f, accel1);
                    }
                    yield return new WaitForMillisecondFrames(80);
                }
                m_State = 0;
                yield return new WaitForMillisecondFrames(600);
                for (int j = 0; j < 3; j++) {
                    for (int k = 0; k < 3; k++) {
                        pos[1] = BackgroundCamera.GetScreenPosition(m_FirePosition[1].position);
                        CreateBullet(0, pos[1], UnityEngine.Random.Range(5f, 12f), Func_GetDirection.Invoke() + UnityEngine.Random.Range(-20f, 20f), accel2,
                            BulletSpawnType.EraseAndCreate, 500, 0, 0.1f, BulletPivot.Player, UnityEngine.Random.Range(-25f, 25f), accel3);
                    }
                    yield return new WaitForMillisecondFrames(240);
                }
                yield return new WaitForMillisecondFrames(800);
            }
        }
    }
}
