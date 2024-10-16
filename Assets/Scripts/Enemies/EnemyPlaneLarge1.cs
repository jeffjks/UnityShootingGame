﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlaneLarge1 : EnemyUnit, IHasPhase
{
    public GameObject[] m_Part = new GameObject[2];
    public EnemyPlaneLarge1_Turret[] m_Turret = new EnemyPlaneLarge1_Turret[2];
    public Transform m_Rotator;
    public EnemyExplosionCreater m_NextPhaseExplosionCreater;
    
    private IEnumerator m_CurrentPattern1, m_CurrentPattern2;
    private IEnumerator _timeLimitCoroutine;
    private const int APPEARANCE_TIME = 2800;
    private const int TIME_LIMIT = 20000;
    private const float ROLLING_ANGLE_MAX = 30f;
    private readonly Vector3 TARGET_POSITION = new (0f, -5.2f, Depth.ENEMY);
    private int _phase;

    private void Start()
    {
        // IsColliderInit = false;
        m_Rotator.rotation = Quaternion.Euler(0f, ROLLING_ANGLE_MAX, 0f);

        DisableInteractableAll();

        StartCoroutine(AppearanceSequence());

        m_EnemyHealth.Action_OnHealthChanged += ToNextPhase;
    }

    private IEnumerator AppearanceSequence() {
        Vector3 init_position = transform.position;
        Quaternion init_rotation = m_Rotator.rotation;
        Quaternion target_rotation = Quaternion.identity;

        int frame = APPEARANCE_TIME * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_pos = AC_Ease.ac_ease[(int)EaseType.OutQuad].Evaluate((float) (i+1) / frame);
            float t_rot = AC_Ease.ac_ease[(int)EaseType.InQuad].Evaluate((float) (i+1) / frame);

            transform.position = Vector3.Lerp(init_position, TARGET_POSITION, t_pos);
            m_Rotator.rotation = Quaternion.Lerp(init_rotation, target_rotation, t_rot);
            yield return new WaitForMillisecondFrames(0);
        }

        OnAppearanceComplete();
    }

    private void OnAppearanceComplete() {
        _phase = 1;
        // IsColliderInit = true;
        EnableInteractableAll();

        StartPattern("1A", new EnemyPlaneLarge1_BulletPattern_1A(this));

        _timeLimitCoroutine = TimeLimit(TIME_LIMIT);
        StartCoroutine(_timeLimitCoroutine);
    }

    private IEnumerator TimeLimit(int time_limit = 0) {
        yield return new WaitForMillisecondFrames(time_limit);
        TimeLimitState = true;
        m_MoveVector.direction = -90f;

        float init_speed = m_MoveVector.speed;
        Quaternion init_rotation = m_Rotator.rotation;
        float target_speed = 7f;
        Quaternion target_rotation = Quaternion.Euler(0f, ROLLING_ANGLE_MAX, 0f);
        int frame = 2000 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);
            float t_rot = AC_Ease.ac_ease[(int)EaseType.InQuad].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, target_speed, t_spd);
            m_Rotator.rotation = Quaternion.Lerp(init_rotation, target_rotation, t_rot);
            yield return new WaitForMillisecondFrames(0);
        }
    }

    public void ToNextPhase()
    {
        switch (_phase)
        {
            case 1:
                if (m_EnemyHealth.HealthRatioScaled > 400) // 체력 40% 이하
                    return;
                break;
            default:
                return;
        }
        
        _phase++;
        m_Turret[0].m_EnemyDeath.KillEnemy();
        m_Turret[1].m_EnemyDeath.KillEnemy();
        Destroy(m_Part[0]);
        Destroy(m_Part[1]);
        m_Part[0] = null;
        m_Part[1] = null;
        
        StopAllPatterns();
        StartPattern("2A", new EnemyPlaneLarge1_BulletPattern_2A(this));
        StartPattern("2B", new EnemyPlaneLarge1_BulletPattern_2B(this));

        NextPhaseExplosion();
    }

    private void NextPhaseExplosion() {
        m_NextPhaseExplosionCreater.StartExplosion();
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        m_MoveVector = new MoveVector(1.2f, 0f);
        _phase = -1;
        
        yield break;
    }
}
