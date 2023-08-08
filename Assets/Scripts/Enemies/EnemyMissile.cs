using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyMissile : EnemyUnit
{
    public Transform m_Renderer;
    public GameObject m_Engine;

    //private Quaternion m_Rotation;

    protected override void Awake()
    {
        base.Awake();
        CurrentAngle = 0f;
        //m_Rotation = m_Renderer.rotation;

        DisableInteractableAll();
    }

    void OnEnable()
    {
        transform.SetParent(null);
        m_MoveVector = new MoveVector(1f, 0f);
        //m_Renderer.rotation = m_Rotation;
        StartCoroutine(AppearanceSequence());

        EnableInteractableAll();
    }

    private IEnumerator AppearanceSequence() {
        yield return new WaitForMillisecondFrames(1000);
        m_Engine.SetActive(true);

        yield return new WaitForMillisecondFrames(1200);

        float init_speed = transform.position.z;
        int frame = 1000 * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) { // 1초간 1->6
            float t_spd = AC_Ease.ac_ease[(int)EaseType.Linear].Evaluate((float) (i+1) / frame);
            
            float speed = Mathf.Lerp(init_speed, 6f, t_spd);
            m_MoveVector.speed = speed;
            yield return new WaitForMillisecondFrames(0);
        }
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        yield return StartPattern("A", new BulletPattern_EnemyMissile(this));
    }
}

public class BulletPattern_EnemyMissile : BulletFactory, IBulletPattern
{
    public BulletPattern_EnemyMissile(EnemyObject enemyObject) : base(enemyObject) { }

    public IEnumerator ExecutePattern(UnityAction onCompleted)
    {
        var pos = _enemyObject.transform.position;
        
        if (SystemManager.Difficulty == GameDifficulty.Normal)
        {
            var accel = new BulletAccel(4.5f, 500);
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 0.5f, BulletPivot.Fixed, -15f, accel, 12, 30f));
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert)
        {
            var accel1 = new BulletAccel(4f, 500);
            var accel2 = new BulletAccel(5f, 500);
            CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 0.25f, BulletPivot.Fixed, -12f, accel1, 15, 24f));
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 0.75f, BulletPivot.Fixed, -0f, accel2, 15, 24f));
        }
        else {
            var accel1 = new BulletAccel(4f, 500);
            var accel2 = new BulletAccel(5f, 500);
            CreateBullet(new BulletProperty(pos, BulletImage.BlueLarge, 0.25f, BulletPivot.Fixed, -10f, accel1, 18, 20f));
            CreateBullet(new BulletProperty(pos, BulletImage.PinkLarge, 0.75f, BulletPivot.Fixed, 0f, accel2, 18, 20f));
        }
        onCompleted?.Invoke();
        yield break;
    }
}
