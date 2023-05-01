using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyItemHeli : EnemyUnit {

	public GameObject m_FanL, m_FanR, m_FanB;
	public float m_FanRotationSpeed;
    public Transform[] m_FirePosition = new Transform[2];
    private int[] m_FireDelay = { 4000, 2000, 1500 };
    
    private const int APPEARANCE_TIME = 1500;
    private const int TIME_LIMIT = 9000;
    //private float m_PositionY, m_AddPositionY;
    private float m_VSpeed = 1f;
    private IEnumerator m_TimeLimit;

    void Start()
    {
        //m_PositionY = transform.position.y;

        m_MoveVector.speed = 4f;
        
        StartCoroutine(Pattern1());

        StartCoroutine(AppearanceSequence());
        /*
        m_Sequence = DOTween.Sequence();
        m_Sequence.Append(DOTween.To(()=>m_PositionY, x=>m_PositionY = x, -2f + m_VSpeed*APPEARANCE_TIME, APPEARANCE_TIME).SetEase(Ease.OutQuad));
        m_Sequence.AppendInterval(time_limit);
        m_Sequence.Append(DOTween.To(()=>m_PositionY, x=>m_PositionY = x, -20f, 3f).SetEase(Ease.InQuad));*/
    }
    /*
    protected override void Update()
    {
        m_AddPositionY -= m_VSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, m_PositionY + m_AddPositionY, transform.position.z);
        
        base.Update();
    }*/

    public IEnumerator AppearanceSequence() {
        yield return new WaitForMillisecondFrames(APPEARANCE_TIME / 2);

        float init_speed = m_MoveVector.speed;
        int frame = (APPEARANCE_TIME / 2) * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, m_VSpeed, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }
        m_TimeLimit = TimeLimit(TIME_LIMIT);
        StartCoroutine(m_TimeLimit);
        yield break;
    }

    private IEnumerator TimeLimit(int time_limit = 0) {
        yield return new WaitForMillisecondFrames(time_limit);
        m_TimeLimitState = true;

        float init_speed = m_MoveVector.speed;
        int frame = 1000 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, 5f, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }

    void LateUpdate()
    {
        RotateFan();
    }

    private void RotateFan() {
		m_FanL.transform.Rotate(0, m_FanRotationSpeed * Time.deltaTime, 0);
		m_FanR.transform.Rotate(0, m_FanRotationSpeed * Time.deltaTime, 0);
		m_FanB.transform.Rotate(- m_FanRotationSpeed * Time.deltaTime, 0 , 0);
    }
    
    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        Vector3 pos1, pos2;
        float target_angle1, target_angle2;
        yield return new WaitForMillisecondFrames(APPEARANCE_TIME);

        while(!m_TimeLimitState) {
            float random_value = Random.Range(-1f, 1f);

            pos1 = m_FirePosition[0].position;
            pos2 = m_FirePosition[1].position;
            target_angle1 = GetAngleToTarget(pos1, m_PlayerManager.GetPlayerPosition());
            target_angle2 = GetAngleToTarget(pos2, m_PlayerManager.GetPlayerPosition());

            for (int i = 0; i < 3; i++) {
                pos1 = m_FirePosition[0].position;
                pos2 = m_FirePosition[1].position;
                CreateBullet(1, pos1, 5.4f, target_angle1 + random_value, accel);
                CreateBullet(1, pos2, 5.4f, target_angle2 + random_value, accel);
                yield return new WaitForMillisecondFrames(80);
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[m_SystemManager.GetDifficulty()]);
        }
        yield break;
    }
}


//EnemyBulletAccel accel2 = new EnemyBulletAccel(6f, 0.8f);
//CreateBullet(0, pos, 3.8f, target_angle, accel, 2, 1f, 4, 0.5f, BulletDirection.PLAYER, 0f, accel2);