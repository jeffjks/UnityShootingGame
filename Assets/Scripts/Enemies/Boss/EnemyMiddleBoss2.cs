using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMiddleBoss2 : EnemyUnit, IEnemyBossMain
{
    public Transform m_FirePosition0;
    public Transform[] m_FirePosition2 = new Transform[2];
    public EnemyMiddleBoss2Turret0 m_Turret0;
    public EnemyMiddleBoss2Turret1[] m_Turret1 = new EnemyMiddleBoss2Turret1[2];
    private int m_Phase;
    
    private float m_Direction;

    private IEnumerator m_MovementPattern;
    private IEnumerator m_CurrentPattern1, m_CurrentPattern2;

    void Start()
    {
        m_Phase = 1;
        m_MoveVector = new MoveVector(-3f, 120f + 180f);

        m_MovementPattern = AppearanceSequence();
        StartCoroutine(m_MovementPattern);

        m_CurrentPattern1 = Pattern1A();
        StartCoroutine(m_CurrentPattern1);
        
        DisableInteractableAll();

        m_EnemyDeath.Action_OnDying += OnBossDying;
        m_EnemyDeath.Action_OnDeath += OnBossDeath;
        m_EnemyDeath.Action_OnRemoved += OnBossDying;
        m_Turret0.m_EnemyDeath.Action_OnDying += ToNextPhase;

        SystemManager.OnMiddleBossStart();

        /*
        m_Sequence = DOTween.Sequence()
        .AppendInterval(4f)
        .Append(DOTween.To(()=>m_MoveVector.speed, x=>m_MoveVector.speed = x, 0f, 1f).SetEase(Ease.OutQuad)) // stop
        .AppendInterval(2f)
        .Append(DOTween.To(()=>m_MoveVector.speed, x=>m_MoveVector.speed = x, 2f, 1f).SetEase(Ease.InQuad))
        .AppendInterval(1f)
        .Append(DOTween.To(()=>m_MoveVector.direction, x=>m_MoveVector.direction = x, 220f, 3.5f).SetEase(Ease.Linear))
        .Append(DOTween.To(()=>m_MoveVector.speed, x=>m_MoveVector.speed = x, 0f, 1f).SetEase(Ease.OutQuad)) // stop
        .AppendInterval(0.5f)
        .Append(DOTween.To(()=>m_MoveVector.speed, x=>m_MoveVector.speed = x, 2f, 1f).SetEase(Ease.InQuad))
        .Append(DOTween.To(()=>m_MoveVector.direction, x=>m_MoveVector.direction = x, 170f, 2.5f).SetEase(Ease.Linear))
        .Append(DOTween.To(()=>m_MoveVector.speed, x=>m_MoveVector.speed = x, 0f, 1f).SetEase(Ease.OutQuad)) // stop
        .AppendInterval(1f)
        .Append(DOTween.To(()=>m_MoveVector.speed, x=>m_MoveVector.speed = x, 2f, 1.5f).SetEase(Ease.InQuad))
        .Append(DOTween.To(()=>m_MoveVector.direction, x=>m_MoveVector.direction = x, 240f, 2.5f).SetEase(Ease.Linear));*/
    }

    public IEnumerator AppearanceSequence() {
        //MoveVector init_moveVector;
        yield return new WaitForMillisecondFrames(4000);
        yield return MovementPattern(new MoveVector(0f, m_MoveVector.direction), EaseType.OutQuad, EaseType.InOutQuad, 1000); // stop
        yield return new WaitForMillisecondFrames(2000);
        yield return MovementPattern(new MoveVector(-2f, m_MoveVector.direction), EaseType.InQuad, EaseType.InOutQuad, 1000); // speed to 2f
        yield return new WaitForMillisecondFrames(1000);
        yield return MovementPattern(new MoveVector(m_MoveVector.speed, 220f + 180f), EaseType.Linear, EaseType.InOutQuad, 3500); // direction to 220f
        yield return MovementPattern(new MoveVector(0f, m_MoveVector.direction), EaseType.OutQuad, EaseType.InOutQuad, 1000); // stop
        yield return new WaitForMillisecondFrames(500);
        yield return MovementPattern(new MoveVector(-2f, m_MoveVector.direction), EaseType.InQuad, EaseType.InOutQuad, 1000); // speed to 2f
        yield return MovementPattern(new MoveVector(m_MoveVector.speed, 170f + 180f), EaseType.Linear, EaseType.InOutQuad, 2500); // direction to 170f
        yield return MovementPattern(new MoveVector(0f, m_MoveVector.direction), EaseType.OutQuad, EaseType.InOutQuad, 1000); // stop
        yield return new WaitForMillisecondFrames(1000);
        yield return MovementPattern(new MoveVector(-2f, m_MoveVector.direction), EaseType.InQuad, EaseType.InOutQuad, 1500); // speed to 2f
        yield return MovementPattern(new MoveVector(m_MoveVector.speed, 240f + 180f), EaseType.Linear, EaseType.InOutQuad, 2500); // direction to 240f
        yield return new WaitForMillisecondFrames(6500);
        m_EnemyDeath.OnRemoved();
    }

    private IEnumerator MovementPattern(MoveVector target_moveVector, int speed_ease, int direction_ease, int duration) {
        MoveVector init_moveVector = m_MoveVector;
        int frame = duration * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_spd = AC_Ease.ac_ease[speed_ease].Evaluate((float) (i+1) / frame);
            float t_dir = AC_Ease.ac_ease[direction_ease].Evaluate((float) (i+1) / frame);

            m_MoveVector.speed = Mathf.Lerp(init_moveVector.speed, target_moveVector.speed, t_spd);
            m_MoveVector.direction = Mathf.Lerp(init_moveVector.direction, target_moveVector.direction, t_dir);
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }


    protected override void Update()
    {
        base.Update();
        
        if (m_Phase == 1) {
            if (m_EnemyHealth.m_HealthPercent <= 0.375f) { // 체력 37.5% 이하
                ToNextPhase();
            }
        }

        m_Direction += 200f / Application.targetFrameRate * Time.timeScale;
        if (m_Direction >= 360f)
            m_Direction -= 360f;

        RotateImmediately(m_MoveVector.direction);
    }

    public void ToNextPhase() {
        if (m_Phase == 2)
            return;
        m_Phase = 2;
        if (m_CurrentPattern1 != null)
            StopCoroutine(m_CurrentPattern1);

        m_CurrentPattern1 = Pattern2A();
        m_CurrentPattern2 = Pattern2B();
        StartCoroutine(m_CurrentPattern1);
        StartCoroutine(m_CurrentPattern2);

        m_Turret0?.m_EnemyDeath.OnDying();
        m_Turret1[0]?.m_EnemyDeath.OnDying();
        m_Turret1[1]?.m_EnemyDeath.OnDying();
        
        //m_Collider2D[0].gameObject.SetActive(true);
        EnableInteractableAll();
        BulletManager.SetBulletFreeState(500);
    }


    private IEnumerator Pattern1A() {
        BulletAccel accel = new BulletAccel(0f, 0);
        yield return new WaitForMillisecondFrames(2500);
        while(true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                CreateBulletsSector(2, BackgroundCamera.GetScreenPosition(m_FirePosition2[0].position), 6.5f, Random.Range(0f, 360f), accel, 18, 20f);
                yield return new WaitForMillisecondFrames(500);
                CreateBulletsSector(2, BackgroundCamera.GetScreenPosition(m_FirePosition2[0].position), 6.5f, Random.Range(0f, 360f), accel, 18, 20f);
                yield return new WaitForMillisecondFrames(2500);
                CreateBulletsSector(2, BackgroundCamera.GetScreenPosition(m_FirePosition2[1].position), 6.5f, Random.Range(0f, 360f), accel, 18, 20f);
                yield return new WaitForMillisecondFrames(500);
                CreateBulletsSector(2, BackgroundCamera.GetScreenPosition(m_FirePosition2[1].position), 6.5f, Random.Range(0f, 360f), accel, 18, 20f);
                yield return new WaitForMillisecondFrames(2500);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBulletsSector(2, BackgroundCamera.GetScreenPosition(m_FirePosition2[0].position), 6.8f, Random.Range(0f, 360f), accel, 20, 18f);
                yield return new WaitForMillisecondFrames(500);
                CreateBulletsSector(2, BackgroundCamera.GetScreenPosition(m_FirePosition2[0].position), 6.8f, Random.Range(0f, 360f), accel, 20, 18f);
                yield return new WaitForMillisecondFrames(500);
                CreateBulletsSector(2, BackgroundCamera.GetScreenPosition(m_FirePosition2[0].position), 6.8f, Random.Range(0f, 360f), accel, 20, 18f);
                yield return new WaitForMillisecondFrames(2000);
                CreateBulletsSector(2, BackgroundCamera.GetScreenPosition(m_FirePosition2[1].position), 6.8f, Random.Range(0f, 360f), accel, 20, 18f);
                yield return new WaitForMillisecondFrames(500);
                CreateBulletsSector(2, BackgroundCamera.GetScreenPosition(m_FirePosition2[1].position), 6.8f, Random.Range(0f, 360f), accel, 20, 18f);
                yield return new WaitForMillisecondFrames(500);
                CreateBulletsSector(2, BackgroundCamera.GetScreenPosition(m_FirePosition2[1].position), 6.8f, Random.Range(0f, 360f), accel, 20, 18f);
                yield return new WaitForMillisecondFrames(2000);
            }
            else {
                CreateBulletsSector(2, BackgroundCamera.GetScreenPosition(m_FirePosition2[0].position), 6.8f, Random.Range(0f, 360f), accel, 24, 15f);
                yield return new WaitForMillisecondFrames(500);
                CreateBulletsSector(2, BackgroundCamera.GetScreenPosition(m_FirePosition2[0].position), 6.8f, Random.Range(0f, 360f), accel, 24, 15f);
                yield return new WaitForMillisecondFrames(500);
                CreateBulletsSector(2, BackgroundCamera.GetScreenPosition(m_FirePosition2[0].position), 6.8f, Random.Range(0f, 360f), accel, 24, 15f);
                yield return new WaitForMillisecondFrames(1500);
                CreateBulletsSector(2, BackgroundCamera.GetScreenPosition(m_FirePosition2[1].position), 6.8f, Random.Range(0f, 360f), accel, 24, 15f);
                yield return new WaitForMillisecondFrames(500);
                CreateBulletsSector(2, BackgroundCamera.GetScreenPosition(m_FirePosition2[1].position), 6.8f, Random.Range(0f, 360f), accel, 24, 15f);
                yield return new WaitForMillisecondFrames(500);
                CreateBulletsSector(2, BackgroundCamera.GetScreenPosition(m_FirePosition2[1].position), 6.8f, Random.Range(0f, 360f), accel, 24, 15f);
                yield return new WaitForMillisecondFrames(1500);
            }
        }
    }

    private IEnumerator Pattern2A() {
        BulletAccel accel = new BulletAccel(0f, 0);
        while(true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                CreateBullet(3, BackgroundCamera.GetScreenPosition(m_FirePosition2[0].position), 6.4f, m_Direction, accel);
                CreateBullet(3, BackgroundCamera.GetScreenPosition(m_FirePosition2[1].position), 6.4f, -m_Direction, accel);
                yield return new WaitForMillisecondFrames(110);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                CreateBulletsSector(5, BackgroundCamera.GetScreenPosition(m_FirePosition2[0].position), 6.6f, m_Direction, accel, 2, 180f);
                CreateBulletsSector(5, BackgroundCamera.GetScreenPosition(m_FirePosition2[1].position), 6.6f, -m_Direction, accel, 2, 180f);
                yield return new WaitForMillisecondFrames(70);
            }
            else {
                CreateBulletsSector(5, BackgroundCamera.GetScreenPosition(m_FirePosition2[0].position), 6.8f, m_Direction, accel, 2, 180f);
                CreateBulletsSector(5, BackgroundCamera.GetScreenPosition(m_FirePosition2[1].position), 6.8f, -m_Direction, accel, 2, 180f);
                yield return new WaitForMillisecondFrames(40);
            }
        }
    }

    private IEnumerator Pattern2B() {
        Vector3 pos;
        BulletAccel accel = new BulletAccel(0f, 0);
        float random_value, target_angle;
        yield return new WaitForMillisecondFrames(1000);
        while(true) {
            if (SystemManager.Difficulty == GameDifficulty.Normal) {
                random_value = Random.Range(0f, 360f);
                pos = BackgroundCamera.GetScreenPosition(m_FirePosition0.position);
                target_angle = GetAngleToTarget(pos, PlayerManager.GetPlayerPosition());
                CreateBulletsSector(4, pos, 6f, random_value + target_angle, accel, 20, 18f);
                yield return new WaitForMillisecondFrames(1800);
            }
            else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                random_value = Random.Range(0f, 360f);
                for (int i = 0; i < 3; i++) {
                    pos = BackgroundCamera.GetScreenPosition(m_FirePosition0.position);
                    target_angle = GetAngleToTarget(pos, PlayerManager.GetPlayerPosition());
                    CreateBulletsSector(4, pos, 6f + i*0.6f, random_value + target_angle, accel, 24, 15f);
                    yield return new WaitForMillisecondFrames(80);
                }
                yield return new WaitForMillisecondFrames(900);
            }
            else {
                random_value = Random.Range(0f, 360f);
                for (int i = 0; i < 3; i++) {
                    pos = BackgroundCamera.GetScreenPosition(m_FirePosition0.position);
                    target_angle = GetAngleToTarget(pos, PlayerManager.GetPlayerPosition());
                    CreateBulletsSector(4, pos, 6f + i*0.6f, random_value + target_angle, accel, 30, 12f);
                    yield return new WaitForMillisecondFrames(80);
                }
                yield return new WaitForMillisecondFrames(500);
            }
        }
    }


    protected override IEnumerator DyingEffect() { // 파괴 과정
        BulletManager.BulletsToGems(2000);
        m_MoveVector.speed = 0f;
        m_Phase = -1;

        if (m_MovementPattern != null) {
            StopCoroutine(m_MovementPattern);
        }
        
        yield break;
    }

    public void OnBossDying() {
        SystemManager.OnMiddleBossClear();
    }

    public void OnBossDeath() {
        InGameScreenEffectService.WhiteEffect(false);
    }
}
