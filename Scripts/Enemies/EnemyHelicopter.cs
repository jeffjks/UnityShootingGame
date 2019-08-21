using UnityEngine;
using System.Collections;
using DG.Tweening;

public class EnemyHelicopter : EnemyUnit
{
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    [SerializeField] private Transform m_FirePosition = null;
	[SerializeField] private GameObject m_FanU = null, m_FanB = null;
	[SerializeField] private float m_FanRotationSpeed = 20f;
    
    private bool m_TimeLimitState = false;

    void Start()
    {
        float time_limit = 4f;

        GetCoordinates();
        InvokeRepeating("Pattern1", 1f, m_FireDelay[m_SystemManager.m_Difficulty]);
        RotateImmediately(m_PlayerPosition);

        Invoke("TimeLimit", time_limit);
    }

    protected override void Update()
    {
        if (m_PlayerManager.m_PlayerIsAlive)
            RotateImmediately(m_PlayerPosition);
        else
            RotateSlightly(m_PlayerPosition, 100f);
        
        base.Update();
    }
    
	void FixedUpdate() {
		m_FanU.transform.Rotate(0, 10 * m_FanRotationSpeed, 0);
		m_FanB.transform.Rotate(-10 * m_FanRotationSpeed, 0 , 0);
	}

    private void TimeLimit() {
        m_TimeLimitState = true;
        m_MoveVector.direction = GetAngleToTarget(m_Position2D, m_PlayerPosition);
        DOTween.To(()=>m_MoveVector.speed, x=>m_MoveVector.speed = x, 7.2f, 0.8f).SetEase(Ease.OutQuad);
    }

    private void Pattern1() {
        if (!m_TimeLimitState) {
            Vector3 pos = m_FirePosition.position;
            
            EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
            CreateBullet(1, pos, 6.4f, m_CurrentAngle, accel);
        }
    }
}
