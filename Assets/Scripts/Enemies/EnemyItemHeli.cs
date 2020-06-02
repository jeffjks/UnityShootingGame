using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyItemHeli : EnemyUnit {

    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
	[SerializeField] private GameObject m_FanL = null, m_FanR = null, m_FanB = null;
	[SerializeField] private float m_FanRotationSpeed = 240f;
    [SerializeField] private Transform[] m_FirePosition = new Transform[2];
    
    private bool m_TimeLimitState = false;
    private float m_AppearanceTime = 1.5f;
    private float m_PositionY, m_AddPositionY;
    private float m_VSpeed = 1f;

    void Start ()
    {
        float time_limit = 9f;
        m_PositionY = transform.position.y;
        
        StartCoroutine(Pattern1());

        m_Sequence = DOTween.Sequence();
        m_Sequence.Append(DOTween.To(()=>m_PositionY, x=>m_PositionY = x, -2f + m_VSpeed*m_AppearanceTime, m_AppearanceTime).SetEase(Ease.OutQuad));
        m_Sequence.AppendInterval(time_limit);
        m_Sequence.Append(DOTween.To(()=>m_PositionY, x=>m_PositionY = x, -20f, 3f).SetEase(Ease.InQuad));

        Invoke("TimeLimit", m_AppearanceTime + time_limit);
    }

    protected override void Update()
    {
        m_AddPositionY -= m_VSpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, m_PositionY + m_AddPositionY, transform.position.z);
        
        base.Update();
    }

    private void TimeLimit() {
        m_TimeLimitState = true;
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
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3 pos1, pos2;
        float target_angle1, target_angle2;
        yield return new WaitForSeconds(m_AppearanceTime);

        while(!m_TimeLimitState) {
            float random_value = Random.Range(-1f, 1f);

            pos1 = m_FirePosition[0].position;
            pos2 = m_FirePosition[1].position;
            target_angle1 = GetAngleToTarget(pos1, m_PlayerManager.m_Player.transform.position);
            target_angle2 = GetAngleToTarget(pos2, m_PlayerManager.m_Player.transform.position);

            for (int i = 0; i < 3; i++) {
                pos1 = m_FirePosition[0].position;
                pos2 = m_FirePosition[1].position;
                CreateBullet(1, pos1, 5.4f, target_angle1 + random_value, accel);
                CreateBullet(1, pos2, 5.4f, target_angle2 + random_value, accel);
                yield return new WaitForSeconds(0.08f);
            }
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
        yield break;
    }
}


//EnemyBulletAccel accel2 = new EnemyBulletAccel(6f, 0.8f);
//CreateBullet(0, pos, 3.8f, target_angle, accel, 2, 1f, 4, 0.5f, BulletDirection.PLAYER, 0f, accel2);