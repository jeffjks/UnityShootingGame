using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class EnemyPlaneMedium1 : EnemyUnit
{
    private const int APPEARANCE_TIME = 1600;
    private const int TIME_LIMIT = 10000;
    //private float m_PositionY, m_AddPositionY;
    private float m_VSpeed = 0.2f;
    private IEnumerator m_TimeLimit;

    void Start ()
    {
        m_MoveVector.speed = 4.3f;

        StartPattern("A", new EnemyPlaneMedium1_BulletPattern_A(this, APPEARANCE_TIME));

        StartCoroutine(AppearanceSequence());
    }

    private IEnumerator AppearanceSequence() {
        yield return new WaitForMillisecondFrames(APPEARANCE_TIME / 2);

        float init_speed = m_MoveVector.speed;
        int frame = (APPEARANCE_TIME / 2) * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, m_VSpeed, t_spd);
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
        
        if (!TimeLimitState) { // Retreat when boss or middle boss state
            if (SystemManager.PlayState != PlayState.OnField) {
                if (m_TimeLimit != null)
                    StopCoroutine(m_TimeLimit);
                m_TimeLimit = TimeLimit();
                StartCoroutine(m_TimeLimit);
                TimeLimitState = true;
            }
        }
    }
}

public class EnemyPlaneMedium1_BulletPattern_A : BulletFactory, IBulletPattern
{
    private readonly int _appearanceTime;

    public EnemyPlaneMedium1_BulletPattern_A(EnemyObject enemyObject, int appearanceTime) : base(enemyObject)
    {
        _appearanceTime = appearanceTime;
    }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 1500, 1200, 1200 };
        var accel = new BulletAccel(7.2f, 1000);
        yield return new WaitForMillisecondFrames(_appearanceTime);

        while(!_enemyObject.TimeLimitState) {
            if (SystemManager.Difficulty == GameDifficulty.Normal)
            {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.4f, BulletPivot.Player, 0f, 6, 12f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.4f, BulletPivot.Player, 0f, 5, 12f));
                yield return new WaitForMillisecondFrames(1000);
                
                for (int i = 0; i < 4; i++)
                {
                    var pos1 = GetFirePos(1);
                    var pos2 = GetFirePos(2);
                    CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 6f, BulletPivot.Fixed, 0f));
                    CreateBullet(new BulletProperty(pos2, BulletImage.PinkNeedle, 6f, BulletPivot.Fixed, 0f));
                    yield return new WaitForMillisecondFrames(60);
                }
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert)
            {
                for (int i = 0; i < 3; i++) {
                    var pos = GetFirePos(0);
                    var dir = Random.Range(-3f, 3f);
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.4f, BulletPivot.Player, dir, 9, 10f));
                    yield return new WaitForMillisecondFrames(200);
                }
                yield return new WaitForMillisecondFrames(1000);
                for (int i = 0; i < 4; i++) {
                    var pos1 = GetFirePos(1);
                    var pos2 = GetFirePos(2);
                    CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 4.2f, BulletPivot.Fixed, 0f, accel));
                    CreateBullet(new BulletProperty(pos2, BulletImage.PinkNeedle, 4.2f, BulletPivot.Fixed, 0f, accel));
                    yield return new WaitForMillisecondFrames(60);
                }
            }
            else
            {
                var pos = GetFirePos(0);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.4f, BulletPivot.Player, -1.5f, 10, 9f));
                CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.4f, BulletPivot.Player, 1.5f, 10, 9f));
                yield return new WaitForMillisecondFrames(200);
                for (int i = 0; i < 4; i++) {
                    pos = GetFirePos(0);
                    var dir = Random.Range(-3f, 3f);
                    CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 6.4f, BulletPivot.Player, dir, 9, 10f));
                    yield return new WaitForMillisecondFrames(200);
                }
                yield return new WaitForMillisecondFrames(100);
                CreateBullet(new BulletProperty(pos, BulletImage.BlueSmall, 6.4f, BulletPivot.Player, 0f, 13, 8f));
                yield return new WaitForMillisecondFrames(1000);
                for (int i = 0; i < 4; i++) {
                    var pos1 = GetFirePos(1);
                    var pos2 = GetFirePos(2);
                    CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 5f, BulletPivot.Fixed, -39f, accel));
                    CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 5f, BulletPivot.Fixed, -26f, accel));
                    CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 5f, BulletPivot.Fixed, -13f, accel));
                    CreateBullet(new BulletProperty(pos1, BulletImage.PinkNeedle, 5f, BulletPivot.Fixed, 0f, accel));
                    CreateBullet(new BulletProperty(pos2, BulletImage.PinkNeedle, 5f, BulletPivot.Fixed, 0f, accel));
                    CreateBullet(new BulletProperty(pos2, BulletImage.PinkNeedle, 5f, BulletPivot.Fixed, 13f, accel));
                    CreateBullet(new BulletProperty(pos2, BulletImage.PinkNeedle, 5f, BulletPivot.Fixed, 26f, accel));
                    CreateBullet(new BulletProperty(pos2, BulletImage.PinkNeedle, 5f, BulletPivot.Fixed, 39f, accel));
                    yield return new WaitForMillisecondFrames(60);
                }
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int)SystemManager.Difficulty]);
        }
        onCompleted?.Invoke();
    }
}
