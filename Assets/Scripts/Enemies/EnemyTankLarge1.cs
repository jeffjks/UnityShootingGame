using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankLarge1 : EnemyUnit
{
    public EnemyTankLarge1Turret1[] m_Turret1 = new EnemyTankLarge1Turret1[2];
    public EnemyTankLarge1Turret2 m_Turret2;
    public Transform[] m_FirePosition = new Transform[2];
    public Transform m_LauncherRotation;

    private int m_Phase;
    private Quaternion m_LauncherRotationTarget;
    private float m_Rotation = -240f;
    private bool m_TurretState = true;

    void Start()
    {
        m_LauncherRotationTarget = Quaternion.identity;
        m_EnemyHealth.Action_OnHealthChanged += DestroyChildEnemy;

        m_Turret1[0].Func_GetDirection += GetDirection;
        m_Turret1[1].Func_GetDirection += GetDirection;
    }

    private void DestroyChildEnemy() {
        if (m_EnemyHealth.m_HealthPercent <= 0.33f) { // 체력 33% 이하
            m_Turret1[0]?.m_EnemyDeath.OnDying();
            m_Turret1[2]?.m_EnemyDeath.OnDying();
            m_Turret2?.m_EnemyDeath.OnDying();

            if (m_Phase <= 1) {
                if (m_LauncherRotation.localRotation != m_LauncherRotationTarget) {
                    m_Rotation = Mathf.MoveTowards(m_Rotation, 0f, 400f / Application.targetFrameRate * Time.timeScale);
                    m_LauncherRotation.localEulerAngles = new Vector3(m_Rotation, 0f, 0f);
                }
                else {
                    StartCoroutine(Pattern2());
                    m_Phase = 2;
                }
            }
        }
    }
    
    protected override void Update()
    {
        base.Update();
        
        RotateImmediately(m_MoveVector.direction);
        m_Turret2.RotateImmediately(m_CurrentAngle);

        if (m_Phase == 0) {
            if (m_Position2D.y < - 1f) {
                m_Phase = 1;
            }
        }

        if (m_TurretState) {
            m_Turret1[0].ToNextPhase();
            m_Turret1[1].ToNextPhase();
            m_TurretState = false;
        }
    }

    private float GetDirection() {
        return m_MoveVector.direction;
    }
    
    private IEnumerator Pattern2() {
        Vector3[] pos = new Vector3[2];
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        float[] target_angle = new float[2];
        while(true) {
            yield return new WaitForMillisecondFrames(500);
            if (m_SystemManager.GetDifficulty() == 0) {
                for (int j = 0; j < 2; j++) {
                    pos[j] = GetScreenPosition(m_FirePosition[j].position);
                    target_angle[j] = GetAngleToTarget(pos[j], m_PlayerManager.GetPlayerPosition());

                    float delta_angle = Mathf.DeltaAngle(target_angle[j], m_CurrentAngle); // ~-45, -45~45, 45~
                    if (delta_angle < -45f) {
                        target_angle[j] = m_CurrentAngle + 45f;
                    }
                    else if (delta_angle > 45f) {
                        target_angle[j] = m_CurrentAngle - 45f;
                    }

                    //target_angle[j] = Mathf.Clamp(target_angle[j], m_CurrentAngle - 45f, m_CurrentAngle + 45f);
                    CreateBulletsSector(0, pos[j], 7f, target_angle[j], accel, 3, 22f);
                }
            }
            else if (m_SystemManager.GetDifficulty() == 1) {
                for (int i = 0; i < 10; i++) {
                    for (int j = 0; j < 2; j++) {
                        pos[j] = GetScreenPosition(m_FirePosition[j].position);
                        target_angle[j] = GetAngleToTarget(pos[j], m_PlayerManager.GetPlayerPosition());

                        float delta_angle = Mathf.DeltaAngle(target_angle[j], m_CurrentAngle); // ~-45, -45~45, 45~
                        if (delta_angle < -45f) {
                            target_angle[j] = m_CurrentAngle + 45f;
                        }
                        else if (delta_angle > 45f) {
                            target_angle[j] = m_CurrentAngle - 45f;
                        }
                        
                        //target_angle[j] = Mathf.Clamp(target_angle[j], m_CurrentAngle - 45f, m_CurrentAngle + 45f);
                        CreateBulletsSector(0, pos[j], 6f + i*0.8f, target_angle[j], accel, 3, 22f);
                    }
                    yield return new WaitForMillisecondFrames(60);
                }
            }
            else {
                for (int i = 0; i < 10; i++) {
                    for (int j = 0; j < 2; j++) {
                        pos[j] = GetScreenPosition(m_FirePosition[j].position);
                        target_angle[j] = GetAngleToTarget(pos[j], m_PlayerManager.GetPlayerPosition());

                        float delta_angle = Mathf.DeltaAngle(target_angle[j], m_CurrentAngle); // ~-45, -45~45, 45~
                        if (delta_angle < -45f) {
                            target_angle[j] = m_CurrentAngle + 45f;
                        }
                        else if (delta_angle > 45f) {
                            target_angle[j] = m_CurrentAngle - 45f;
                        }

                        //target_angle[j] = Mathf.Clamp(target_angle[j], m_CurrentAngle - 45f, m_CurrentAngle + 45f);
                        CreateBulletsSector(2, pos[j], 6f + i*0.8f, target_angle[j], accel, 2, 22f);
                        CreateBulletsSector(0, pos[j], 6f + i*0.8f, target_angle[j], accel, 3, 22f);
                    }
                    yield return new WaitForMillisecondFrames(60);
                }
            }
            yield return new WaitForMillisecondFrames(2000);
        }
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        CreateExplosionEffect(0, -1, new Vector3(0f, 0f, -0.4f));
        CreateExplosionEffect(2, -1, new Vector3(0f, 3.2f, 2.5f));
        CreateExplosionEffect(1, -1, new Vector3(-2f, 0f, 1.4f));
        CreateExplosionEffect(1, -1, new Vector3(2f, 0f, 1.4f));
        CreateExplosionEffect(1, -1, new Vector3(-2f, 0f, -1.4f));
        CreateExplosionEffect(1, -1, new Vector3(2f, 0f, -1.4f));
        
        yield break;
    }
}
