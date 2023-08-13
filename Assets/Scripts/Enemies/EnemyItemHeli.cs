using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyItemHeli : EnemyUnit {

	public GameObject m_FanL, m_FanR, m_FanB;
	public float m_FanRotationSpeed;
    
    private const int APPEARANCE_TIME = 1500;
    private const int TIME_LIMIT = 9000;
    //private float m_PositionY, m_AddPositionY;
    private const float VERTICAL_SPEED = 1f;
    private IEnumerator m_TimeLimit;

    void Start()
    {
        m_MoveVector.speed = 4f;
        
        StartPattern("A", new BulletPattern_EnemyItemHeli(this));

        StartCoroutine(AppearanceSequence());
    }

    private IEnumerator AppearanceSequence() {
        yield return new WaitForMillisecondFrames(APPEARANCE_TIME / 2);

        float init_speed = m_MoveVector.speed;
        int frame = (APPEARANCE_TIME / 2) * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, VERTICAL_SPEED, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }
        m_TimeLimit = TimeLimit(TIME_LIMIT);
        StartCoroutine(m_TimeLimit);
    }

    private IEnumerator TimeLimit(int time_limit = 0) {
        yield return new WaitForMillisecondFrames(time_limit);
        TimeLimitState = true;

        float init_speed = m_MoveVector.speed;
        int frame = 1000 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, 5f, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }
    }

    protected override void Update()
    {
        base.Update();
        
        RotateFan();
    }

    private void RotateFan() {
		m_FanL.transform.Rotate(0, m_FanRotationSpeed * Time.deltaTime, 0);
		m_FanR.transform.Rotate(0, m_FanRotationSpeed * Time.deltaTime, 0);
		m_FanB.transform.Rotate(- m_FanRotationSpeed * Time.deltaTime, 0 , 0);
    }
}

public class BulletPattern_EnemyItemHeli : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyItemHeli(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 4000, 2000, 1500 };
        yield return new WaitForMillisecondFrames(1500);
        
        while (!_enemyObject.TimeLimitState) {
            var dir = Random.Range(-1f, 1f);
            
            for (int i = 0; i < 3; i++)
            {
                var pos1 = GetFirePos(0);
                var pos2 = GetFirePos(1);
                CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 5.4f, BulletPivot.Player, dir));
                CreateBullet(new BulletProperty(pos2, BulletImage.PinkNeedle, 5.4f, BulletPivot.Player, dir));
                yield return new WaitForMillisecondFrames(80);
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        onCompleted?.Invoke();
    }
}
