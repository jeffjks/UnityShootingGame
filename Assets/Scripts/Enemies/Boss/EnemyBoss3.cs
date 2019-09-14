using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyBoss3 : EnemyUnit
{
    public EnemyBoss3Turret[] m_Turret = new EnemyBoss3Turret[2];
    public EnemyBoss3Part m_Part;
    public Transform[] m_FirePosition = new Transform[3];
    public EnemyBoss3Barrel[] m_EnemyBoss3Barrel = new EnemyBoss3Barrel[2];

    [HideInInspector] public sbyte m_Phase;
    
    private Vector3 m_TargetPosition;
    private Vector3 m_DefaultScale;
    private float m_AppearanceTime1 = 3f, m_AppearanceTime2 = 1.2f;
    private byte m_DirectionState;
    private float m_Direction;
    private int m_RotateDirection;
    private bool m_InPattern = false;
    private float m_MaxRotation = 11f;

    private IEnumerator m_CurrentPhase, m_CurrentPattern1, m_CurrentPattern2, m_CurrentPattern3;

    void Start()
    {
        m_TargetPosition = new Vector2(0f, -4.2f);
        m_DefaultScale = transform.localScale;
        transform.localScale = new Vector3(2f, 2f, 2f);
        m_MoveVector = new MoveVector(1f, -125f);
        RotateImmediately(m_MoveVector.direction);
        
        DisableAttackable(4.2f);
        m_Part.DisableAttackable(4.2f);

        m_Sequence = DOTween.Sequence()
        .Append(DOTween.To(()=>m_MoveVector.speed, x=>m_MoveVector.speed = x, 15f, 2f).SetEase(Ease.InQuad));

        Invoke("Appearance", m_AppearanceTime1);
    }

    protected override void Update()
    {
        if (m_Phase == 1) {
            if (m_Health <= m_MaxHealth * 0.40f) { // 체력 40% 이하
                ToNextPhase();
            }
        }

        if (m_Phase > 0) {
            if (transform.position.x >= m_TargetPosition.x + 0.7f) {
                m_MoveVector.direction = Random.Range(-105f, -75f);
            }
            else if (transform.position.x <= m_TargetPosition.x - 0.7f) {
                m_MoveVector.direction = Random.Range(75f, 105f);
            }
            else if (transform.position.y >= m_TargetPosition.y + 0.2f) {
                m_MoveVector = new MoveVector(new Vector2(m_MoveVector.GetVector().x, -m_MoveVector.GetVector().y));
            }
            else if (transform.position.y <= m_TargetPosition.y - 0.2f) {
                m_MoveVector = new MoveVector(new Vector2(m_MoveVector.GetVector().x, -m_MoveVector.GetVector().y));
            }
        }

        if (m_DirectionState == 1) {
            m_Direction += 90f*Time.deltaTime*m_RotateDirection;
        }
        else if (m_DirectionState == 2) {
            m_Direction += 71f*Time.deltaTime*m_RotateDirection;
        }
        else if (m_DirectionState == 3) {
            m_Direction += m_MaxRotation*Time.deltaTime*m_RotateDirection;
        }
        
        if (m_Direction > 360f)
            m_Direction -= 360f;
        else if (m_Direction < 0f)
            m_Direction += 360f;

        base.Update();
    }

    private void Appearance() {
        transform.localScale = m_DefaultScale;
        transform.position = new Vector3(0f, 4.3f, Depth.ENEMY);
        m_MoveVector = new MoveVector(0f, 0f);
        RotateImmediately(m_MoveVector.direction);
        transform.DOMoveY(m_TargetPosition.y, m_AppearanceTime2).SetEase(Ease.OutQuad);
        Invoke("OnAppearanceComplete", m_AppearanceTime2);
    }

    private void OnAppearanceComplete() {
        float random_direction = Random.Range(75f, 105f) + 180f*Random.Range(0, 2);
        m_MoveVector = new MoveVector(0.5f, random_direction);
        m_Phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);
    }

    private int RandomValue() {
        int random_value = Random.Range(0, 2);
        if (random_value == 0)
            random_value = -1;
        return random_value;
    }

    public void ToNextPhase() {
        float duration = 2f;
        m_Phase++;
        if (m_Phase >= 2) {
            m_SystemManager.EraseBullets(1f);
            
            if (m_CurrentPattern1 != null)
                StopCoroutine(m_CurrentPattern1);
            if (m_CurrentPattern2 != null)
                StopCoroutine(m_CurrentPattern2);
            if (m_CurrentPattern3 != null)
                StopCoroutine(m_CurrentPattern3);
            if (m_CurrentPhase != null)
                StopCoroutine(m_CurrentPhase);
            m_Turret[0].StopPattern();
            m_Turret[1].StopPattern();

            if (m_Phase == 2) {
                m_CurrentPhase = Phase2();
                StartCoroutine(m_CurrentPhase);
                m_Part.OnDeath();
                EnableInvincible(duration);
                NextPhaseExplosion(duration);
            }
        }
    }

    private void NextPhaseExplosion(float duration) {
        StartCoroutine(NextPhaseExplosionEffect1(duration));
        StartCoroutine(NextPhaseExplosionEffect2(duration));
    }

    private IEnumerator NextPhaseExplosionEffect1(float duration) {
        float t = 0f, t_add = 0f;
        while (t < duration) {
            t_add = Random.Range(0.35f, 0.5f);
            ExplosionEffect(1, 1, new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(-0.5f, 0.8f), Depth.EXPLOSION), new MoveVector(Random.Range(0.5f, 1f), Random.Range(0f, 360f)));
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield return null;
    }

    private IEnumerator NextPhaseExplosionEffect2(float duration) {
        float t = 0f, t_add = 0f;
        while (t < duration) {
            t_add = Random.Range(0.2f, 0.4f);
            ExplosionEffect(2, 2, new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(-0.5f, 0.8f), Depth.EXPLOSION), new MoveVector(Random.Range(0.5f, 1f), Random.Range(0f, 360f)));
            ExplosionEffect(2, -1, new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(-0.5f, 0.8f), Depth.EXPLOSION), new MoveVector(Random.Range(0.5f, 1f), Random.Range(0f, 360f)));
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield break;
    }

    private IEnumerator Phase1() { // 페이즈 1 패턴 =================
        int side;
        yield return new WaitForSeconds(1f);
        while(m_Phase == 1) {
            m_CurrentPattern1 = Pattern1A();
            StartCoroutine(m_CurrentPattern1);
            while (m_InPattern)
                yield return null;
            yield return new WaitForSeconds(0.5f);
            
            side = RandomValue();
            
            m_Turret[0].StartPattern(1, side);
            m_Turret[1].StartPattern(1, side);
            m_CurrentPattern1 = Pattern1B();
            StartCoroutine(m_CurrentPattern1);
            while (m_InPattern)
                yield return null;
            m_Turret[0].StopPattern();
            m_Turret[1].StopPattern();
            yield return new WaitForSeconds(2f);

            m_CurrentPattern1 = Pattern1C1();
            m_CurrentPattern2 = Pattern1C2();
            StartCoroutine(m_CurrentPattern1);
            StartCoroutine(m_CurrentPattern2);
            yield return new WaitForSeconds(5f);
            if (m_CurrentPattern1 != null)
                StopCoroutine(m_CurrentPattern1);
            if (m_CurrentPattern2 != null)
                StopCoroutine(m_CurrentPattern2);
            m_DirectionState = 0;
            yield return new WaitForSeconds(0.7f);
            m_CurrentPattern1 = Pattern1D();
            StartCoroutine(m_CurrentPattern1);
            yield return new WaitForSeconds(2.5f);
        }
        yield break;
    }

    private IEnumerator Phase2() { // 페이즈 2 패턴 =================
        int random_value;
        yield return new WaitForSeconds(4f);
        m_CurrentPattern1 = Pattern2A();
        StartCoroutine(m_CurrentPattern1);

        while (m_Phase == 2) {
            random_value = Random.Range(0, 2);
            
            m_Turret[0].StartPattern(2, random_value);
            m_Turret[1].StartPattern(2, 1 - random_value);

            yield return new WaitForSeconds(4f);
            m_Turret[0].StopPattern();
            m_Turret[1].StopPattern();
            m_CurrentPattern2 = Pattern2B();
            StartCoroutine(m_CurrentPattern2);
            yield return new WaitForSeconds(2f);
            if (m_CurrentPattern2 != null)
                StopCoroutine(m_CurrentPattern2);
            yield return new WaitForSeconds(1f);
        }
        yield break;
    }

    private IEnumerator Pattern1A() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float[] fire_delay = { 0.18f, 0.13f, 0.1f };

        m_RotateDirection = RandomValue();
        
        m_InPattern = true;
        m_DirectionState = 1;

        for (int i = 0; i < 3; i++) {
            pos = m_FirePosition[0].position;
            m_Direction = GetAngleToTarget(pos, m_PlayerManager.m_Player.transform.position) - 45f*m_RotateDirection;
            m_CurrentPattern2 = Pattern1A1(i);
            m_CurrentPattern3 = Pattern1A2(i);
            StartCoroutine(m_CurrentPattern2);
            StartCoroutine(m_CurrentPattern3);
            yield return new WaitForSeconds(1f);
            if (m_CurrentPattern2 != null)
                StopCoroutine(m_CurrentPattern2);
            if (m_CurrentPattern3 != null)
                StopCoroutine(m_CurrentPattern3);
            m_RotateDirection *= -1;
        }
        
        m_InPattern = false;
        m_DirectionState = 0;
        yield break;
    }

    private IEnumerator Pattern1A1(int level) {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float duration = 0f;
        float[] fire_delay = { 0.18f, 0.13f, 0.1f };

        while (duration < 1f) {
            pos = m_FirePosition[0].position;
            duration += fire_delay[m_SystemManager.m_Difficulty];
            switch(level) {
                case 0:
                    CreateBullet(1, pos, 7.4f, m_Direction, accel);
                    break;
                case 1:
                    CreateBulletsSector(1, pos, 7.4f, m_Direction, accel, 2, 24f);
                    break;
                case 2:
                    CreateBulletsSector(1, pos, 7.4f, m_Direction, accel, 3, 24f);
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(fire_delay[m_SystemManager.m_Difficulty]);
        }
        yield break;
    }

    private IEnumerator Pattern1A2(int level) {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float duration = 0f;
        float[] fire_delay = { 0.3f, 0.22f, 0.17f };

        while (duration < 1f) {
            pos = m_FirePosition[0].position;
            duration += fire_delay[m_SystemManager.m_Difficulty];
            switch(level) {
                case 0:
                    CreateBulletsSector(3, pos, 6f, m_Direction, accel, 3, 2.25f);
                    break;
                case 1:
                    CreateBulletsSector(3, pos, 6f, m_Direction - 14f, accel, 3, 2.25f);
                    CreateBulletsSector(3, pos, 6f, m_Direction + 14f, accel, 3, 2.25f);
                    break;
                case 2:
                    CreateBulletsSector(3, pos, 6f, m_Direction - 28f, accel, 3, 2.25f);
                    CreateBulletsSector(3, pos, 6f, m_Direction, accel, 3, 2.25f);
                    CreateBulletsSector(3, pos, 6f, m_Direction + 28f, accel, 3, 2.25f);
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(fire_delay[m_SystemManager.m_Difficulty]);
        }
        yield break;
    }

    private IEnumerator Pattern1B() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float duration;
        float[] fire_delay = { 1f, 0.5f, 0.33f };

        m_InPattern = true;
        yield return new WaitForSeconds(1.5f);

        for (int j = 0; j < 4; j++) {
            duration = 0f;
            while (duration < 1.6f) {
                duration += fire_delay[m_SystemManager.m_Difficulty];
                pos = m_FirePosition[0].position;
                for (int i = 0; i <= j; i++) {
                    CreateBullet(3, pos, 7f + i*0.6f, GetAngleToTarget(pos, m_PlayerManager.m_Player.transform.position), accel);
                }
                yield return new WaitForSeconds(fire_delay[m_SystemManager.m_Difficulty]);
            }
        }
        m_InPattern = false;
        yield break;
    }

    private IEnumerator Pattern1C1() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float[] fire_delay = { 0.19f, 0.12f, 0.09f };
        float duration = 0f;
        
        m_Direction = Random.Range(0f, 360f);
        m_DirectionState = 2;
        m_RotateDirection = RandomValue();

        while (true) {
            duration += fire_delay[m_SystemManager.m_Difficulty];
            pos = m_FirePosition[0].position;
            CreateBulletsSector(2, pos, 7.2f, m_Direction, accel, 8, 45f);
            CreateBulletsSector(5, pos, 7.2f, -m_Direction, accel, 8, 45f);
            yield return new WaitForSeconds(fire_delay[m_SystemManager.m_Difficulty]);
        }
    }

    private IEnumerator Pattern1C2() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        
        m_RotateDirection = RandomValue();
        yield return new WaitForSeconds(1f);
        
        if (m_SystemManager.m_Difficulty == 0) {
            while (true) {
                pos = m_FirePosition[0].position;
                CreateBulletsSector(0, pos, 7.2f, Random.Range(0f, 360f), accel, 30, 12f);
                yield return new WaitForSeconds(2.4f);
            }
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            while (true) {
                pos = m_FirePosition[0].position;
                CreateBulletsSector(0, pos, 7.2f, Random.Range(0f, 360f), accel, 30, 12f);
                yield return new WaitForSeconds(0.22f);
                pos = m_FirePosition[0].position;
                CreateBulletsSector(0, pos, 7.2f, Random.Range(0f, 360f), accel, 36, 10f);
                yield return new WaitForSeconds(2f);
            }
        }
        else {
            while (true) {
                pos = m_FirePosition[0].position;
                CreateBulletsSector(0, pos, 7.2f, Random.Range(0f, 360f), accel, 30, 12f);
                yield return new WaitForSeconds(0.22f);
                pos = m_FirePosition[0].position;
                CreateBulletsSector(0, pos, 7.2f, Random.Range(0f, 360f), accel, 36, 10f);
                yield return new WaitForSeconds(0.22f);
                pos = m_FirePosition[0].position;
                CreateBulletsSector(0, pos, 7.2f, Random.Range(0f, 360f), accel, 30, 12f);
                yield return new WaitForSeconds(1.5f);
            }
        }
    }

    private IEnumerator Pattern1D() {
        Vector3 pos1, pos2;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        EnemyBulletAccel new_accel = new EnemyBulletAccel(7.3f, 1.5f);
        
        m_RotateDirection = RandomValue();
        
        pos1 = m_FirePosition[1].position;
        pos2 = m_FirePosition[2].position;

        if (m_SystemManager.m_Difficulty == 0) {
            CreateBullet(3, pos1, 10f, 0f, accel, BulletType.CREATE, Random.Range(0f, 0.1f),
            1, 0.1f, BulletDirection.CURRENT, 0f, new_accel, 2, 180f, new Vector2(0.2f, 0.3f));
            CreateBullet(3, pos2, 10f, 0f, accel, BulletType.CREATE, Random.Range(0f, 0.1f),
            1, 0.1f, BulletDirection.CURRENT, 0f, new_accel, 2, 180f, new Vector2(0.2f, 0.3f));
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            CreateBullet(3, pos1, 10f, 0f, accel, BulletType.CREATE, Random.Range(0f, 0.1f),
            1, 0.1f, BulletDirection.CURRENT, 0f, new_accel, 2, 180f, new Vector2(0.1f, 0.15f));
            CreateBullet(3, pos2, 10f, 0f, accel, BulletType.CREATE, Random.Range(0f, 0.1f),
            1, 0.1f, BulletDirection.CURRENT, 0f, new_accel, 2, 180f, new Vector2(0.1f, 0.15f));
        }
        else {
            CreateBullet(3, pos1, 10f, 0f, accel, BulletType.CREATE, Random.Range(0f, 0.1f),
            1, 0.1f, BulletDirection.CURRENT, 0f, new_accel, 2, 180f, new Vector2(0.1f, 0.15f));
            CreateBullet(3, pos2, 10f, 0f, accel, BulletType.CREATE, Random.Range(0f, 0.1f),
            1, 0.1f, BulletDirection.CURRENT, 0f, new_accel, 2, 180f, new Vector2(0.1f, 0.15f));
            CreateBullet(3, pos1, 10f, 40f, accel, BulletType.CREATE, Random.Range(0f, 0.1f),
            1, 0.1f, BulletDirection.CURRENT, 0f, new_accel, 2, 180f, new Vector2(0.1f, 0.15f));
            CreateBullet(3, pos2, 10f, -40f, accel, BulletType.CREATE, Random.Range(0f, 0.1f),
            1, 0.1f, BulletDirection.CURRENT, 0f, new_accel, 2, 180f, new Vector2(0.1f, 0.15f));
        }
        
        m_EnemyBoss3Barrel[0].BarrelShotAnimation(-0.1f);
        m_EnemyBoss3Barrel[1].BarrelShotAnimation(-0.1f);
        yield break;
    }

    private IEnumerator Pattern2A() {
        Vector3 pos1, pos2;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float total_duration = 1.4f, duration, fire_delay = 0.07f;
        
        m_DirectionState = 3;
        m_RotateDirection = RandomValue();
        m_Direction = - m_RotateDirection * m_MaxRotation * total_duration * 0.5f;

        while (true) {
            for (int i = 0; i < 3; i++) {
                duration = 0f;
                while (duration < total_duration) {
                    pos1 = m_FirePosition[1].position;
                    pos2 = m_FirePosition[2].position;

                    if (m_SystemManager.m_Difficulty == 0) {
                        CreateBulletsSector(0, pos1, 8.3f, m_Direction, accel, 6, 28f);
                        CreateBulletsSector(0, pos2, 8.3f, m_Direction, accel, 6, 28f);
                    }
                    else if (m_SystemManager.m_Difficulty == 1) {
                        CreateBulletsSector(0, pos1, 8.3f, m_Direction, accel, 10, 20f);
                        CreateBulletsSector(0, pos2, 8.3f, m_Direction, accel, 10, 20f);
                    }
                    else {
                        CreateBulletsSector(0, pos1, 8.3f, m_Direction, accel, 12, 18f);
                        CreateBulletsSector(0, pos2, 8.3f, m_Direction, accel, 12, 18f);
                    }
                    duration += fire_delay;
                    yield return new WaitForSeconds(fire_delay);
                }
                m_RotateDirection *= -1;
            }
            yield return new WaitForSeconds(0.6f);
        }
    }

    private IEnumerator Pattern2B() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(4.2f, 0.9f);
        float[] fire_delay = { 0.1f, 0.064f, 0.05f };
        float target_angle;
        
        while (true) {
            pos = m_FirePosition[0].position;
            target_angle = GetAngleToTarget(pos, m_PlayerManager.m_Player.transform.position);
            CreateBulletsSector(4, pos, 8f, target_angle + Random.Range(-40f, 40f), accel, 2, 8f);
            yield return new WaitForSeconds(fire_delay[m_SystemManager.m_Difficulty]);
        }
    }



    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        m_Phase = -1;
        if (m_CurrentPattern1 != null)
            StopCoroutine(m_CurrentPattern1);
        if (m_CurrentPattern2 != null)
            StopCoroutine(m_CurrentPattern2);
        if (m_CurrentPattern3 != null)
            StopCoroutine(m_CurrentPattern3);
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        m_SystemManager.BulletsToGems(2f);
        m_MoveVector = new MoveVector(1f, 0f);
        
        yield return new WaitForSeconds(0.6f);

        StartCoroutine(DeathExplosion1(2.8f));
        StartCoroutine(DeathExplosion2(2.8f));
        StartCoroutine(DeathExplosion3(2.8f));

        yield return new WaitForSeconds(3f);
        ExplosionEffect(2, 4, new Vector2(0f, 0.64f), new MoveVector(Random.Range(0.5f, 1f), Random.Range(0f, 360f))); // 최종 파괴
        ExplosionEffect(2, -1, new Vector2(0f, -0.36f), new MoveVector(Random.Range(0.5f, 1f), Random.Range(0f, 360f)));
        ExplosionEffect(1, -1, new Vector2(0.5f, 0.75f), new MoveVector(Random.Range(0.5f, 1f), Random.Range(0f, 360f)));
        ExplosionEffect(1, -1, new Vector2(0.5f, 0.2f), new MoveVector(Random.Range(0.5f, 1f), Random.Range(0f, 360f)));
        ExplosionEffect(1, -1, new Vector2(-0.5f, 0.75f), new MoveVector(Random.Range(0.5f, 1f), Random.Range(0f, 360f)));
        ExplosionEffect(1, -1, new Vector2(-0.5f, 0.2f), new MoveVector(Random.Range(0.5f, 1f), Random.Range(0f, 360f)));
        ExplosionEffect(1, -1, new Vector2(0f, -0.75f), new MoveVector(Random.Range(0.5f, 1f), Random.Range(0f, 360f)));
        ExplosionEffect(0, -1, new Vector2(0.2f, 0.18f), new MoveVector(Random.Range(0.5f, 1f), Random.Range(0f, 360f)));
        ExplosionEffect(0, -1, new Vector2(-0.2f, 0.18f), new MoveVector(Random.Range(0.5f, 1f), Random.Range(0f, 360f)));
        m_SystemManager.ScreenEffect(1);
        
        CreateItems();
        Destroy(gameObject);
        yield return null;
    }

    private IEnumerator DeathExplosion1(float timer) {
        float t = 0f, t_add = 0f;
        while (t < timer) {
            t_add = Random.Range(0.45f, 0.6f);
            ExplosionEffect(2, 3, new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(-0.5f, 0.8f), Depth.EXPLOSION));
            ExplosionEffect(2, -1, new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(-0.5f, 0.8f), Depth.EXPLOSION));
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield return null;
    }

    private IEnumerator DeathExplosion2(float timer) {
        float t = 0f, t_add = 0f;
        while (t < timer) {
            t_add = Random.Range(0.3f, 0.45f);
            ExplosionEffect(1, 2, new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(-0.5f, 0.8f), Depth.EXPLOSION));
            ExplosionEffect(1, -1, new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(-0.5f, 0.8f), Depth.EXPLOSION));
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield return null;
    }

    private IEnumerator DeathExplosion3(float timer) {
        float t = 0f, t_add = 0f;
        while (t < timer) {
            t_add = Random.Range(0.2f, 0.4f);
            ExplosionEffect(0, -1, new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(-0.5f, 0.8f), Depth.EXPLOSION));
            ExplosionEffect(0, -1, new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(-0.5f, 0.8f), Depth.EXPLOSION));
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield return null;
    }
}
