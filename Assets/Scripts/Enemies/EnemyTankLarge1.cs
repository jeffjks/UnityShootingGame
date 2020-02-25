using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTankLarge1 : EnemyUnit
{
    [SerializeField] private Transform[] m_FirePosition = new Transform[2];
    [SerializeField] private Transform m_LauncherRotation = null;

    [HideInInspector] public sbyte m_Phase;
    private Quaternion m_LauncherRotationTarget;
    private float m_Rotation = -240f;

    void Start()
    {
        GetCoordinates();

        m_LauncherRotationTarget = Quaternion.identity;
    }
    
    protected override void Update()
    {
        RotateImmediately(m_MoveVector.direction);

        if (m_Phase == 0) {
            if (m_Position2D.y < - 1f) {
                m_Phase = 1;
            }
        }
        if (3 * m_Health <= m_MaxHealth) {
            if (m_Phase <= 1) {
                if (m_LauncherRotation.localRotation != m_LauncherRotationTarget) {
                    m_Rotation = Mathf.MoveTowards(m_Rotation, 0f, 400f*Time.deltaTime);
                    m_LauncherRotation.localEulerAngles = new Vector3(m_Rotation, 0f, 0f);
                }
                else {
                    StartCoroutine(Pattern2());
                    m_Phase = 2;
                }
            }
        }
        
        base.Update();
    }
    
    private IEnumerator Pattern2() {
        Vector3[] pos = new Vector3[2];
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float[] target_angle = new float[2];
        while(true) {
            if (m_SystemManager.m_Difficulty == 0) {
                for (int j = 0; j < 2; j++) {
                    pos[j] = GetScreenPosition(m_FirePosition[j].position);
                    target_angle[j] = GetAngleToTarget(pos[j], m_PlayerManager.m_Player.transform.position);

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
            else if (m_SystemManager.m_Difficulty == 1) {
                for (int i = 0; i < 10; i++) {
                    for (int j = 0; j < 2; j++) {
                        pos[j] = GetScreenPosition(m_FirePosition[j].position);
                        target_angle[j] = GetAngleToTarget(pos[j], m_PlayerManager.m_Player.transform.position);

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
                    yield return new WaitForSeconds(0.06f);
                }
            }
            else {
                for (int i = 0; i < 10; i++) {
                    for (int j = 0; j < 2; j++) {
                        pos[j] = GetScreenPosition(m_FirePosition[j].position);
                        target_angle[j] = GetAngleToTarget(pos[j], m_PlayerManager.m_Player.transform.position);

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
                    yield return new WaitForSeconds(0.06f);
                }
            }
            yield return new WaitForSeconds(2.5f);
        }
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        ExplosionEffect(0, -1, new Vector3(0f, 0f, 2.5f));
        ExplosionEffect(1, -1, new Vector3(-2f, 0f));
        ExplosionEffect(1, -1, new Vector3(2f, 0f));
        
        CreateItems();
        Destroy(gameObject);
        yield break;
    }
}
