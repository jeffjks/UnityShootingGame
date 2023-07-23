using UnityEngine;
using System.Collections;

public class EnemyMiddleBoss5b : EnemyUnit, IEnemyBossMain
{
    public GameObject m_Hull;
    public EnemyMiddleBoss5bTurret m_Turret;
    public Transform m_Renderer;
    public EnemyExplosionCreater m_NextPhaseExplosionCreater;

    private int[] m_FireDelay = { 3200, 2600, 2000 };
    private IEnumerator m_MovementPattern;
    private IEnumerator m_CurrentPattern1, m_CurrentPattern2;
    
    private readonly Vector3 TARGET_POSITION = new (0f, -3.8f, Depth.ENEMY);
    private const int APPEARANCE_TIME = 2000;
    private int m_Phase;

    void Start()
    {
        m_MovementPattern = AppearanceSequence();
        StartCoroutine(m_MovementPattern);
        
        RotateUnit(AngleToPlayer);

        m_EnemyDeath.Action_OnDying += OnBossDying;
        m_EnemyDeath.Action_OnDeath += OnBossDeath;
        m_EnemyDeath.Action_OnRemoved += OnBossDying;

        SystemManager.OnMiddleBossStart();
        /*
        m_Sequence = DOTween.Sequence()
        .Append(transform.DOMove(TARGET_POSITION, APPEARANCE_TIME).SetEase(Ease.OutQuad))
        .Append(transform.DOMove(new Vector3(-3f*random_value, -3.8f, Depth.ENEMY), 2f).SetEase(Ease.InOutQuad))
        .Append(transform.DOMove(new Vector3(3f*random_value, -3.8f, Depth.ENEMY), duration).SetEase(Ease.InOutQuad))
        .Append(transform.DOMove(new Vector3(-3f*random_value, -3.8f, Depth.ENEMY), duration).SetEase(Ease.InOutQuad))
        .Append(transform.DOMove(new Vector3(3f*random_value, -3.8f, Depth.ENEMY), duration).SetEase(Ease.InOutQuad))
        .Append(transform.DOMove(new Vector3(-3f*random_value, -3.8f, Depth.ENEMY), duration).SetEase(Ease.InOutQuad))
        .Append(transform.DOMove(new Vector3(3f*random_value, -3.8f, Depth.ENEMY), duration).SetEase(Ease.InOutQuad))
        .Append(transform.DOMove(new Vector3(-3f*random_value, -3.8f, Depth.ENEMY), duration).SetEase(Ease.InOutQuad))
        .Append(transform.DOMove(new Vector3(0f, -3.8f, Depth.ENEMY), 2f).SetEase(Ease.InOutQuad))
        .Append(transform.DOMoveY(10f, 3f).SetEase(Ease.InQuad));*/
    }

    public IEnumerator AppearanceSequence() {
        int duration = 3000;
        int random_sign = Random.Range(-1, 1);
        if (random_sign == 0)
            random_sign = 1;

        yield return MovementPattern(TARGET_POSITION, EaseType.OutQuad, APPEARANCE_TIME);
        m_CurrentPattern1 = Pattern1();
        StartCoroutine(m_CurrentPattern1);
        
        yield return MovementPattern(new Vector3(-3f*random_sign, -3.8f, Depth.ENEMY), EaseType.InOutQuad, 2000);
        yield return MovementPattern(new Vector3(3f*random_sign, -3.8f, Depth.ENEMY), EaseType.InOutQuad, duration);
        yield return MovementPattern(new Vector3(-3f*random_sign, -3.8f, Depth.ENEMY), EaseType.InOutQuad, duration);
        yield return MovementPattern(new Vector3(3f*random_sign, -3.8f, Depth.ENEMY), EaseType.InOutQuad, duration);
        yield return MovementPattern(new Vector3(-3f*random_sign, -3.8f, Depth.ENEMY), EaseType.InOutQuad, duration);
        yield return MovementPattern(new Vector3(3f*random_sign, -3.8f, Depth.ENEMY), EaseType.InOutQuad, duration);
        yield return MovementPattern(new Vector3(-3f*random_sign, -3.8f, Depth.ENEMY), EaseType.InOutQuad, duration);
        yield return MovementPattern(new Vector3(0f, -3.8f, Depth.ENEMY), EaseType.InOutQuad, 2000);
        yield return MovementPattern(new Vector3(0f, 10f, Depth.ENEMY), EaseType.InQuad, 3000);
    }

    private IEnumerator MovementPattern(Vector3 target_position, EaseType positionEase, int duration) {
        Vector3 init_position = transform.position;
        int frame = duration * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_pos = AC_Ease.ac_ease[(int)positionEase].Evaluate((float) (i+1) / frame);

            transform.position = Vector3.Lerp(init_position, target_position, t_pos);
            yield return new WaitForMillisecondFrames(0);
        }
    }

    protected override void Update()
    {
        base.Update();
        
        if (m_Phase == 0) {
            if (m_EnemyHealth.HealthPercent <= 0.40f) { // 체력 40% 이하
                ToNextPhase();
            }
        }

        if (PlayerManager.IsPlayerAlive)
            RotateSlightly(PlayerManager.GetPlayerPosition(), 40f);
        else
            RotateSlightly(PlayerManager.GetPlayerPosition(), 100f);
    }

    public void ToNextPhase() {
        m_Phase++;
        if (m_CurrentPattern2 != null)
            StopCoroutine(m_CurrentPattern2);
        m_Hull.SetActive(false);
        
        m_Turret.StartPattern1();
        
        NextPhaseExplosion();
    }

    private void NextPhaseExplosion() {
        m_NextPhaseExplosionCreater.StartExplosion();
    }

    private IEnumerator Pattern1() {
        BulletAccel accel = new BulletAccel(0f, 0);
        Vector3[] pos = new Vector3[2];
        int state = 1;
        float[,] bulletInfo1 = { {5.4f, 6.6f, 7.5f, 6.6f, 5.4f}, {-56.3f, -24.3f, 0f, 24.3f, 56.3f} };
        float[,] bulletInfo2a = { {4.1f, 4.5f, 5.1f, 5.8f, 5.8f, 5.1f, 4.5f, 4.1f}, {-63.4f, -41.4f, -23.8f, -7.4f, 7.4f, 23.8f, 41.4f, 63.4f} };
        float[,] bulletInfo2b = {
            {5.3f, 5.4f, 5.9f, 6.6f, 7.5f, 7.8f, 7.5f, 6.6f, 5.9f, 5.4f, 5.3f},
            {-75.3f, -56.3f, -38.8f, -24.3f, -10.8f, 0f, 10.8f, 24.3f, 38.8f, 56.3f, 75.3f}
        };
        float[,] bulletInfo3a = { {4.1f, 4.5f, 5.1f, 5.8f, 5.8f, 5.1f, 4.5f, 4.1f}, {-63.4f, -41.4f, -23.8f, -7.4f, 7.4f, 23.8f, 41.4f, 63.4f} };
        float[,] bulletInfo3b = {
            {5.3f, 5.4f, 5.9f, 6.6f, 7.5f, 7.8f, 7.5f, 6.6f, 5.9f, 5.4f, 5.3f},
            {-75.3f, -56.3f, -38.8f, -24.3f, -10.8f, 0f, 10.8f, 24.3f, 38.8f, 56.3f, 75.3f}
        };
        
        while (true) {
            pos[0] = m_FirePosition[0].position;
            pos[1] = m_FirePosition[1].position;
            if (m_Phase == 0) {
                m_CurrentPattern2 = Pattern2(state);
                StartCoroutine(m_CurrentPattern2);
            }
            else if (m_Phase == 1) {
                m_Turret.StartPattern2();
            }
            state *= -1;
            for (int i = 0; i < 2; i++) {
                if (SystemManager.Difficulty == GameDifficulty.Normal) {
                    for (int j = 0; j < bulletInfo1.GetLength(1); ++j) {
                        CreateBullet(0, pos[i], bulletInfo1[0,j], CurrentAngle + bulletInfo1[1,j], accel);
                    }
                }
                else if (SystemManager.Difficulty == GameDifficulty.Expert) {
                    for (int j = 0; j < bulletInfo2a.GetLength(1); ++j) {
                        CreateBullet(2, pos[i], bulletInfo2a[0,j], CurrentAngle + bulletInfo2a[1,j], accel);
                    }
                    for (int j = 0; j < bulletInfo2b.GetLength(1); ++j) {
                        CreateBullet(0, pos[i], bulletInfo2b[0,j], CurrentAngle + bulletInfo2b[1,j], accel);
                    }
                }
                else {
                    for (int j = 0; j < bulletInfo3a.GetLength(1); ++j) {
                        CreateBullet(2, pos[i], bulletInfo3a[0,j], CurrentAngle + bulletInfo3a[1,j], accel);
                    }
                    for (int j = 0; j < bulletInfo3b.GetLength(1); ++j) {
                        CreateBullet(0, pos[i], bulletInfo3b[0,j], CurrentAngle + bulletInfo3b[1,j], accel);
                    }
                }
            }
            yield return new WaitForMillisecondFrames(m_FireDelay[(int) SystemManager.Difficulty]);
        }
    }

    private IEnumerator Pattern2(int state) {
        BulletAccel accel = new BulletAccel(0f, 0);
        yield return new WaitForMillisecondFrames(2000);
        Vector3 pos;
        float random_value = Random.Range(-6f, 6f);
        
        if (SystemManager.Difficulty == GameDifficulty.Normal) {
            for (int i = 0; i < 4; i++) {
                pos = m_FirePosition[2].position;
                CreateBulletsSector(4, pos, 6.7f - 0.2f*i, CurrentAngle + random_value + 4.8f*i*state, accel, 5, 25f);
                yield return new WaitForMillisecondFrames(260);
            }
        }
        else if (SystemManager.Difficulty == GameDifficulty.Expert) {
            for (int i = 0; i < 5; i++) {
                pos = m_FirePosition[2].position;
                CreateBulletsSector(4, pos, 7f - 0.2f*i, CurrentAngle + random_value + 4.8f*i*state, accel, 7, 19f);
                yield return new WaitForMillisecondFrames(210);
            }
        }
        else {
            for (int i = 0; i < 7; i++) {
                pos = m_FirePosition[2].position;
                CreateBulletsSector(4, pos, 7f - 0.2f*i, CurrentAngle + random_value + 4.8f*i*state, accel, 7, 19f);
                yield return new WaitForMillisecondFrames(150);
            }
        }
        yield break;
    }


    private IEnumerator DeathPattern(Quaternion target_rotation, Vector3 target_scale, EaseType rotationEase, EaseType scaleEase, int duration) {
        Quaternion init_rotation = transform.rotation;
        Vector3 init_scale = m_Renderer.transform.localScale;
        int frame = duration * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_rot = AC_Ease.ac_ease[(int)rotationEase].Evaluate((float) (i+1) / frame);
            float t_scale = AC_Ease.ac_ease[(int)scaleEase].Evaluate((float) (i+1) / frame);

            transform.rotation = Quaternion.Lerp(init_rotation, target_rotation, t_rot);
            m_Renderer.transform.localScale = Vector3.Lerp(init_scale, target_scale, t_scale);
            yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }

    protected override IEnumerator DyingEffect() { // 파괴 과정
        m_MoveVector = new MoveVector(1.2f, 0f);
        BulletManager.BulletsToGems(3000);
        m_Phase = 2;
        if (m_CurrentPattern1 != null)
            StopCoroutine(m_CurrentPattern1);
        m_Turret.m_EnemyDeath.OnDying();

        if (m_MovementPattern != null) {
            StopCoroutine(m_MovementPattern);
        }

        StartCoroutine(DeathPattern(new Quaternion(-0.2f, 0.9f, -0.4f, -0.2f), new Vector3(0.7f, 0.7f, 0.7f), EaseType.Linear, EaseType.Linear, 2500));
        
        /*
        m_Sequence = DOTween.Sequence()
        .Append(m_Renderer.DORotateQuaternion(new Quaternion(-0.2f, 0.9f, -0.4f, -0.2f), 2.5f).SetEase(Ease.Linear))
        .Join(m_Renderer.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 2.5f).SetEase(Ease.Linear));*/
        
        yield break;
    }

    public void OnBossDying() {
        SystemManager.OnMiddleBossClear();
    }

    public void OnBossDeath() {
        InGameScreenEffectService.WhiteEffect(false);
    }
}
