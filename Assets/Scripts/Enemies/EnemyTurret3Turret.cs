using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret3Turret : EnemyUnit
{
    public Transform[] m_FirePosition = new Transform[2];
    public Transform m_TurretAnimation;
    private int[] m_FireDelay = { 2000, 2000, 1800 };

    private float m_InitaialTurretPosition, m_CurrentTurretPosition, m_TargetTurretPosition = -1f;
    private bool m_Active = false; // 총알 생성 없이 총알 쏘는 모션 등 방지용

    void Start()
    {
        m_InitaialTurretPosition = m_TurretAnimation.localPosition.z;
        RotateImmediately(PlayerManager.GetPlayerPosition());
    }

    protected override void Update()
    {
        base.Update();
        
        if (PlayerManager.IsPlayerAlive)
            RotateImmediately(PlayerManager.GetPlayerPosition());
        else
            RotateSlightly(PlayerManager.GetPlayerPosition(), 100f);
        
        if (!m_Active) {
            if (m_Position2D.y < 0f) {
                StartCoroutine(Pattern1());
                m_Active = true;
            }
        }

        m_CurrentTurretPosition = Mathf.MoveTowards(m_CurrentTurretPosition, m_InitaialTurretPosition, 0.02f);
        m_TurretAnimation.localPosition = new Vector3(m_TurretAnimation.localPosition.x, m_TurretAnimation.localPosition.y, m_CurrentTurretPosition);
    }

    private IEnumerator Pattern1() {
        Vector3 pos1, pos2;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        yield return new WaitForMillisecondFrames(Random.Range(0, m_FireDelay[(int) SystemManager.Difficulty]));
        while(true) {
            pos1 = BackgroundCamera.GetScreenPosition(m_FirePosition[0].position);
            pos2 = BackgroundCamera.GetScreenPosition(m_FirePosition[1].position);
            
            if (BulletCondition((pos1 + pos2)/2)) {
                if (SystemManager.Difficulty == GameDifficulty.Normal) {
                    for (int i = 0; i < 4; i++) {
                        CreateBullet(4, pos1, 4.8f + i*0.3f, CurrentAngle, accel);
                        CreateBullet(4, pos2, 4.8f + i*0.3f, CurrentAngle, accel);
                    }
                }
                else {
                    for (int i = 0; i < 4; i++) {
                        CreateBullet(4, pos1, 4.6f + i*0.3f, CurrentAngle + 2f, accel);
                        CreateBullet(4, pos2, 4.6f + i*0.3f, CurrentAngle - 2f, accel);
                        CreateBullet(4, pos1, 5f + i*0.3f, CurrentAngle - 1.5f, accel);
                        CreateBullet(4, pos2, 5f + i*0.3f, CurrentAngle + 1.5f, accel);
                    }
                }
                PlayFireAnimation();
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }

    private void PlayFireAnimation() {
        m_CurrentTurretPosition = m_TargetTurretPosition;
    }
}
