using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class EnemyPlaneSmall3 : EnemyUnit, ITargetPosition
{
    private IEnumerator m_TimeLimit;
    private const int TIME_LIMIT = 5000;

    void Start()
    {
        StartPattern("A", new BulletPattern_EnemyPlaneSmall3_A(this));
        CurrentAngle = AngleToPlayer;
        m_TimeLimit = TimeLimit(TIME_LIMIT);
        StartCoroutine(m_TimeLimit);
        SetRotatePattern(new RotatePattern_TargetPlayer(0f, 100f));
    }

    public void MoveTowardsToTarget(Vector2 target_vec2, int duration) {
        StartCoroutine(MoveTowardsToTargetSequence(target_vec2, duration));
    }

    private IEnumerator MoveTowardsToTargetSequence(Vector2 target_vec2, int duration) {
        Vector3 init_position = transform.position;
        Vector3 target_position = new Vector3(target_vec2.x, target_vec2.y, Depth.ENEMY);
        int frame = duration * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_pos = AC_Ease.ac_ease[(int)EaseType.OutQuad].Evaluate((float) (i+1) / frame);
            
            transform.position = Vector3.Lerp(init_position, target_position, t_pos);
            yield return new WaitForMillisecondFrames(0);
        }
    }

    private IEnumerator TimeLimit(int time_limit = 0) {
        yield return new WaitForMillisecondFrames(time_limit);
        TimeLimitState = true;
        m_MoveVector.direction = AngleToPlayer;

        float init_speed = m_MoveVector.speed;
        int frame = 800 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[(int)EaseType.OutQuad].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_speed, 8f, t_spd);
            yield return new WaitForMillisecondFrames(0);
        }
        SetRotatePattern(new RotatePattern_Stop());
    }
}

public class BulletPattern_EnemyPlaneSmall3_A : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyPlaneSmall3_A(EnemyObject enemyObject) : base(enemyObject) { }
    
    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        int[] fireDelay = { 4000, 2000, 1700 };
        const float gap = 0.3f;
        yield return new WaitForMillisecondFrames(1000);
        
        while (!_enemyObject.TimeLimitState) {
            if (SystemManager.Difficulty <= GameDifficulty.Expert)
            {
                var pos1 = GetFirePos(0, -gap);
                var pos2 = GetFirePos(0, gap);

                CreateBullet(new BulletProperty(pos1, BulletImage.BlueLarge, 4f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueLarge, 6f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos2, BulletImage.BlueLarge, 4f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos2, BulletImage.BlueLarge, 6f, BulletPivot.Current, 0f));
            }
            else
            {
                var pos1 = GetFirePos(0, -gap);
                var pos2 = GetFirePos(0, gap);

                CreateBullet(new BulletProperty(pos1, BulletImage.BlueLarge, 4f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueLarge, 5f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueLarge, 6f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos1, BulletImage.BlueLarge, 7f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos2, BulletImage.BlueLarge, 4f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos2, BulletImage.BlueLarge, 5f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos2, BulletImage.BlueLarge, 6f, BulletPivot.Current, 0f));
                CreateBullet(new BulletProperty(pos2, BulletImage.BlueLarge, 7f, BulletPivot.Current, 0f));
            }
            yield return new WaitForMillisecondFrames(fireDelay[(int) SystemManager.Difficulty]);
        }
        //onCompleted?.Invoke();
    }
}