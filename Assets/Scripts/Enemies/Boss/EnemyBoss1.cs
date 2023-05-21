using System.Collections;
using UnityEngine;
using DG.Tweening; // 파괴 후 Quaternion.identity 상태로 돌아가는 용도

public class EnemyBoss1 : EnemyUnit, IHasAppearance, IEnemyBossMain
{
    public EnemyBoss1Turret0 m_Turret0;
    public EnemyBoss1Turret1[] m_Turret1 = new EnemyBoss1Turret1[2];
    public EnemyBoss1Turret2[] m_Turret2 = new EnemyBoss1Turret2[2];
    public EnemyBoss1Part m_Part;
    public Transform[] m_FirePosition = new Transform[4];
    public Transform m_Rotator;
    public AnimationCurve m_AnimationCurve_Turn;

    private int m_Phase;

    private Vector3[] m_TargetPosition = new Vector3[2];
    private const int APPEARANCE_TIME = 2000;
    private const float ROLLING_ANGLE_START = -35f;
    private const float ROLLING_ANGLE_MID = 20f;
    private const float ROLLING_ANGLE_MAX = 15f;

    private IEnumerator m_CurrentPhase, m_CurrentPattern, m_CurrentMovement;
    private bool m_InPattern = false;

    void Start()
    {
        m_TargetPosition[0] = new Vector3(4f, -1f, Depth.ENEMY);
        m_TargetPosition[1] = new Vector3(0f, -5f, Depth.ENEMY);

        m_Rotator.rotation = Quaternion.Euler(0f, ROLLING_ANGLE_START, 0f);

        DisableInteractableAll();

        StartCoroutine(AppearanceSequence());

        m_EnemyDeath.Action_OnDying += OnBossDying;
        m_EnemyDeath.Action_OnDeath += OnBossDeath;
        m_EnemyDeath.Action_OnRemoved += OnBossDeath;
        m_EnemyHealth.Action_OnHealthChanged += DestroyChildEnemy;
        m_Part.m_EnemyDeath.Action_OnDying += ToNextPhase;
    }

    protected override void Update()
    {
        base.Update();

        OnPhase1();
    }

    private void DestroyChildEnemy() {
        if (m_Phase == 1) {
            if (m_EnemyHealth.m_HealthPercent <= 0.30f) { // 체력 30% 이하
                m_Part?.m_EnemyDeath.OnDying();
            }
        }
    }

    public void ToNextPhase() {
        m_Phase++;
        m_MoveVector = new MoveVector(0f, 0f);
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        m_Turret0.StopPattern();
        m_Turret1[0].StopPattern();
        m_Turret1[1].StopPattern();
        m_Turret2[0].StopPattern();
        m_Turret2[1].StopPattern();
        m_CurrentPhase = Phase2();
        StartCoroutine(m_CurrentPhase);
        //m_Sequence.Kill();
        m_CurrentMovement = OnPhase2();
        StartCoroutine(m_CurrentMovement);
    }


    public IEnumerator AppearanceSequence() {
        /*
        float appearance_time_1 = 0.55f;
        float appearance_time_2 = 1f - 0.55f;
        
        m_Sequence = DOTween.Sequence()
        .Append(transform.DOMoveX(3f, APPEARANCE_TIME*appearance_time_1).SetEase(Ease.OutQuad))
        .Join(transform.DOMoveY(-2f, APPEARANCE_TIME*appearance_time_1).SetEase(Ease.Linear))
        .Join(transform.DORotateQuaternion(m_TargetQuaternion[0], APPEARANCE_TIME*appearance_time_1).SetEase(Ease.InOutQuad))
        .Append(transform.DOMoveX(0f, APPEARANCE_TIME*appearance_time_2).SetEase(Ease.InOutQuad))
        .Join(transform.DOMoveY(-4.5f, APPEARANCE_TIME*appearance_time_2).SetEase(Ease.OutQuad))
        .Join(transform.DORotateQuaternion(Quaternion.identity, APPEARANCE_TIME*appearance_time_2).SetEase(Ease.InQuad));*/

        
        int frame1 = 1100 * Application.targetFrameRate / 1000;
        int frame2 = 900 * Application.targetFrameRate / 1000;

        Vector3 init_vector = transform.position;
        Quaternion init_quarternion = transform.rotation;
        Quaternion tilt_quaternion = Quaternion.Euler(0f, ROLLING_ANGLE_MID, 0f);

        for (int i = 0; i < frame1; ++i) {
            float t_posx = AC_Ease.ac_ease[EaseType.OutQuad].Evaluate((float) (i+1) / frame1);
            float t_posy = AC_Ease.ac_ease[EaseType.Linear].Evaluate((float) (i+1) / frame1);
            float t_rot = AC_Ease.ac_ease[EaseType.InOutQuad].Evaluate((float) (i+1) / frame1);

            float position_x = Mathf.Lerp(init_vector.x, 3f, t_posx);
            float position_y = Mathf.Lerp(init_vector.y, -2f, t_posy);
            transform.position = new Vector3(position_x, position_y, transform.position.z);
            m_Rotator.rotation = Quaternion.Lerp(init_quarternion, tilt_quaternion, t_rot);
            yield return new WaitForMillisecondFrames(0);
        }

        init_vector = transform.position;
        init_quarternion = m_Rotator.rotation;

        for (int i = 0; i < frame2; ++i) {
            float t_posx = AC_Ease.ac_ease[EaseType.InOutQuad].Evaluate((float) (i+1) / frame2);
            float t_posy = AC_Ease.ac_ease[EaseType.OutQuad].Evaluate((float) (i+1) / frame2);
            float t_rot = AC_Ease.ac_ease[EaseType.InQuad].Evaluate((float) (i+1) / frame2);

            float position_x = Mathf.Lerp(init_vector.x, 0f, t_posx);
            float position_y = Mathf.Lerp(init_vector.y, -4.5f, t_posy);
            transform.position = new Vector3(position_x, position_y, transform.position.z);
            m_Rotator.rotation = Quaternion.Lerp(init_quarternion, Quaternion.identity, t_rot);
            yield return new WaitForMillisecondFrames(0);
        }
        
        OnAppearanceComplete();
        yield break;
    }

    public void OnAppearanceComplete() {
        m_Phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);

        int rand = Random.Range(0, 2);
        m_MoveVector = new MoveVector(1f, 90f + 180f*rand);

        EnableInteractableAll();
        /*
        m_Sequence.Kill();
        m_Sequence = DOTween.Sequence()
        .Append(transform.DOMoveX(-1f, 2f).SetEase(Ease.Linear))
        .Append(transform.DOMoveX(1f, 4f).SetEase(Ease.Linear))
        .Append(transform.DOMoveX(0f, 2f).SetEase(Ease.Linear))
        .SetLoops(-1, LoopType.Restart);
        yield break;*/
    }

    private void OnPhase1() {
        if (m_Phase != 1) {
            return;
        }
        if (transform.position.x < -1f) {
            m_MoveVector.direction -= 180f;
        }
        else if (transform.position.x > 1f) {
            m_MoveVector.direction += 180f;
        }
    }

    private IEnumerator OnPhase2() {
        int frame = 1500 * Application.targetFrameRate / 1000;
        Vector3 init_vector;
        Vector3 target_vector;
        Quaternion init_quarternion;
        Quaternion quaternion_rightTurn = Quaternion.Euler(0f, -ROLLING_ANGLE_MAX, 0f);
        Quaternion quaternion_leftTurn = Quaternion.Euler(0f, ROLLING_ANGLE_MAX, 0f);

        yield return new WaitForMillisecondFrames(700);

        while (true) {
            if (transform.position.x < 0f) {
                init_vector = transform.position;
                target_vector = new Vector3(Random.Range(1f, 2f), Random.Range(-4.5f, -5.5f), Depth.ENEMY);
                init_quarternion = m_Rotator.rotation;

                for (int i = 0; i < frame; ++i) {
                    float t_pos = AC_Ease.ac_ease[EaseType.InOutQuad].Evaluate((float) (i+1) / frame);
                    float t_rot = m_AnimationCurve_Turn.Evaluate((float) (i+1) / frame);
                    
                    transform.position = Vector3.Lerp(init_vector, target_vector, t_pos);
                    m_Rotator.rotation = Quaternion.Lerp(init_quarternion, quaternion_rightTurn, t_rot);
                    //m_Rotator.rotation = Quaternion.Lerp(init_quarternion, m_TargetQuaternion[0], t_rot);
                    yield return new WaitForMillisecondFrames(0);
                }
            }
            else {
                init_vector = transform.position;
                target_vector = new Vector3(Random.Range(-2f, -1f), Random.Range(-4.5f, -5.5f), Depth.ENEMY);
                init_quarternion = m_Rotator.rotation;

                for (int i = 0; i < frame; ++i) {
                    float t_pos = AC_Ease.ac_ease[EaseType.InOutQuad].Evaluate((float) (i+1) / frame);
                    float t_rot = m_AnimationCurve_Turn.Evaluate((float) (i+1) / frame);
                    
                    transform.position = Vector3.Lerp(init_vector, target_vector, t_pos);
                    m_Rotator.rotation = Quaternion.Lerp(init_quarternion, quaternion_leftTurn, t_rot);
                    //m_Rotator.rotation = Quaternion.Lerp(init_quarternion, m_TargetQuaternion[0], t_rot);
                    yield return new WaitForMillisecondFrames(0);
                }
            }
            yield return new WaitForMillisecondFrames(700);
        }
    }

    

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        yield return new WaitForMillisecondFrames(1000);
        while(m_Phase == 1) {
            m_CurrentPattern = Pattern1A();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return new WaitForMillisecondFrames(0);
                
            m_CurrentPattern = Pattern1B();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return new WaitForMillisecondFrames(0);

            m_Part.SetOpenState(true);
            m_CurrentPattern = Pattern1C();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return new WaitForMillisecondFrames(0);
            m_Part.SetOpenState(false);
            yield return new WaitForMillisecondFrames(2000);
        }
        yield break;
    }

    private IEnumerator Phase2() { // 페이즈2 패턴 ============================
        yield return new WaitForMillisecondFrames(1000);
        while(m_Phase == 2) {
            m_CurrentPattern = Pattern2A();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return new WaitForMillisecondFrames(0);
        }
        yield break;
    }


    private IEnumerator Pattern1A() {
        int random_value = Random.Range(0, 2);
        m_InPattern = true;
        m_Turret2[0].StartPattern(1);
        m_Turret2[1].StartPattern(1);
        yield return new WaitForMillisecondFrames(300);

        m_Turret1[random_value].StartPattern(1);
        yield return new WaitForMillisecondFrames(1700);
        m_Turret1[1 - random_value].StartPattern(1);
        yield return new WaitForMillisecondFrames(1700);
        m_Turret1[random_value].StartPattern(1);
        yield return new WaitForMillisecondFrames(1700);
        m_Turret1[1 - random_value].StartPattern(1);
        
        m_Turret2[0].StopPattern();
        m_Turret2[1].StopPattern();
        yield return new WaitForMillisecondFrames(1700);
        m_Turret0.StartPattern(1);
        if (m_SystemManager.GetDifficulty() <= 1)
            yield return new WaitForMillisecondFrames(3000);

        m_InPattern = false;
        yield break;
    }

    private IEnumerator Pattern1B() { // Blue Bomb
        Vector3 pos;
        EnemyBulletAccel accel1 = new EnemyBulletAccel(0.1f, 800);
        EnemyBulletAccel accel2 = new EnemyBulletAccel(0f, 0);
        int random_value = Random.Range(0, 2);
        float random_dir;
        m_InPattern = true;
        
        pos = m_FirePosition[random_value].position;
        for (int i = 0; i < 2; i++) {
            random_dir = Random.Range(0f, 360f);
            if (m_SystemManager.GetDifficulty() == 0) {
                CreateBullet(3, pos, 8.2f, 0f, accel1, BulletType.ERASE_AND_CREATE, 800,
                3, 5.4f, BulletDirection.FIXED, random_dir, accel2, 15, 24f);
            }
            else if (m_SystemManager.GetDifficulty() == 1) {
                CreateBullet(3, pos, 8.2f, 0f, accel1, BulletType.ERASE_AND_CREATE, 800,
                3, 5.4f, BulletDirection.FIXED, random_dir, accel2, 20, 18f);
                CreateBullet(3, pos, 8.2f, 0f, accel1, BulletType.ERASE_AND_CREATE, 800,
                5, 4.2f, BulletDirection.FIXED, random_dir + 9f, accel2, 20, 18f);
            }
            else {
                CreateBullet(3, pos, 8.2f, 0f, accel1, BulletType.ERASE_AND_CREATE, 800,
                3, 5.4f, BulletDirection.FIXED, random_dir, accel2, 24, 15f);
                CreateBullet(3, pos, 8.2f, 0f, accel1, BulletType.ERASE_AND_CREATE, 800,
                5, 4.6f, BulletDirection.FIXED, random_dir + 7.5f, accel2, 24, 15f);
                CreateBullet(3, pos, 8.2f, 0f, accel1, BulletType.ERASE_AND_CREATE, 800,
                5, 3.8f, BulletDirection.FIXED, random_dir, accel2, 24, 15f);
            }
            pos = m_FirePosition[1 - random_value].position;
            yield return new WaitForMillisecondFrames(500);
        }
        yield return new WaitForMillisecondFrames(2000);

        m_InPattern = false;
        yield break;
    }

    private IEnumerator Pattern1C() { // 청침탄 흩뿌리기
        Vector3 pos1, pos2;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        float target_angle1, target_angle2;
        int[] fire_delay = { 150, 100, 83 };
        int[] fire_number = { 20, 30, 36 };
        m_InPattern = true;
        yield return new WaitForMillisecondFrames(1000);
        
        for (int i = 0; i < fire_number[m_SystemManager.GetDifficulty()]; i++) {
            pos1 = m_FirePosition[2].position;
            pos2 = m_FirePosition[3].position;
            target_angle1 = GetAngleToTarget(pos1, m_PlayerManager.GetPlayerPosition());
            target_angle2 = GetAngleToTarget(pos2, m_PlayerManager.GetPlayerPosition());
            
            CreateBullet(4, pos1, 4.8f, target_angle1 + Random.Range(-52f, 52f), accel);
            CreateBullet(4, pos2, 4.8f, target_angle2 + Random.Range(-52f, 52f), accel);
                
            yield return new WaitForMillisecondFrames(fire_delay[m_SystemManager.GetDifficulty()]);
        }
        yield return new WaitForMillisecondFrames(500);

        m_InPattern = false;
        yield break;
    }

    private IEnumerator Pattern2A() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0);
        int random_value;
        m_InPattern = true;
        int difficulty_timer = 0;
        if (m_SystemManager.GetDifficulty() == 0)
            difficulty_timer = 400;
        
        m_Turret0.StartPattern(2);
        yield return new WaitForMillisecondFrames(900 + difficulty_timer);

        random_value = Random.Range(0, 2);
        if (random_value == 0) {
            m_Turret1[0].StartPattern(2, true);
            m_Turret1[1].StartPattern(2, false);
        }
        else {
            m_Turret1[0].StartPattern(2, false);
            m_Turret1[1].StartPattern(2, true);
        }
        yield return new WaitForMillisecondFrames(2000);

        m_Turret0.StartPattern(2);
        yield return new WaitForMillisecondFrames(600);
        m_Turret1[0].StartPattern(3);
        m_Turret1[1].StartPattern(3);

        random_value = Random.Range(0, 2);
        m_Turret2[random_value].StartPattern(2);
        yield return new WaitForMillisecondFrames(500 + difficulty_timer);
        m_Turret2[1 - random_value].StartPattern(2);
        yield return new WaitForMillisecondFrames(1800);

        m_InPattern = false;
        yield break;
    }



    protected override IEnumerator DyingEffect() { // 파괴 과정
        m_Phase = -1;
        if (m_CurrentMovement != null)
            StopCoroutine(m_CurrentMovement);
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        m_SystemManager.BulletsToGems(2000);
        m_MoveVector = new MoveVector(0.6f, 0f);
        m_Turret0.m_EnemyDeath.OnDying();

        m_Rotator.DORotateQuaternion(Quaternion.identity, 1f).SetEase(Ease.Linear);
        yield break;
    }

    public void OnBossDying() {
        m_SystemManager.BossClear();
    }

    public void OnBossDeath() {
        m_SystemManager.StartStageClearCoroutine();
        ScreenEffectService.ScreenWhiteEffect(true);
        m_SystemManager.ShakeCamera(1f);
    }
}
