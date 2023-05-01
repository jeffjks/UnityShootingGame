using UnityEngine;
using System.Collections;

public class EnemyHelicopter : EnemyUnit, ITargetPosition
{
    public Transform m_FirePosition;
	public GameObject m_FanU, m_FanB;
	public float m_FanRotationSpeed;

    private int[] m_FireDelay = { 2000, 1000, 500 };
    private const int TIME_LIMIT = 4000;

    void Start()
    {
        StartCoroutine(Pattern1());
        RotateImmediately(m_PlayerPosition);

        StartCoroutine(TimeLimit(TIME_LIMIT));
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateImmediately(m_PlayerPosition);
        else
            RotateSlightly(m_PlayerPosition, 100f);
    }

    void LateUpdate()
    {
        RotateFan();
    }
    
	private void RotateFan() {
		m_FanU.transform.Rotate(0, m_FanRotationSpeed * Time.deltaTime, 0);
		m_FanB.transform.Rotate(-m_FanRotationSpeed * Time.deltaTime, 0 , 0);
	}

    public void MoveTowardsToTarget(Vector2 target_vec2, int duration) {
        StartCoroutine(MoveTowardsToTargetSequence(target_vec2, duration));
    }

    private IEnumerator MoveTowardsToTargetSequence(Vector2 target_vec2, int duration) {
        Vector3 init_position = transform.position;
        Vector3 target_position = new Vector3(target_vec2.x, target_vec2.y, Depth.ENEMY);
        int frame = duration * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_pos = AC_Ease.ac_ease[EaseType.OutQuad].Evaluate((float) (i+1) / frame);
            
            transform.position = Vector3.Lerp(init_position, target_position, t_pos);
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }

    private IEnumerator TimeLimit(int time_limit = 0) {
        yield return new WaitForMillisecondFrames(time_limit);
        m_TimeLimitState = true;
        m_MoveVector.direction = GetAngleToTarget(m_Position2D, m_PlayerPosition);
        
        float init_speed = m_MoveVector.speed;
        int frame = 800 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[EaseType.OutQuad].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, 7.2f, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }

    private IEnumerator Pattern1() {
        while (!m_TimeLimitState) {
            Vector3 pos = m_FirePosition.position;
            float[] speed = {7f, 8.3f, 8.3f};
            
            EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
            CreateBullet(1, pos, speed[m_SystemManager.GetDifficulty()], m_CurrentAngle, accel);
            yield return new WaitForMillisecondFrames(m_FireDelay[m_SystemManager.GetDifficulty()]);
        }
        yield break;
    }
}
