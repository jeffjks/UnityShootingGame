using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyBoss3 : EnemyUnit
{
    public EnemyBoss3Turret[] m_Turret = new EnemyBoss3Turret[2];
    public GameObject m_Part;
    public Transform[] m_FirePosition = new Transform[3];

    [HideInInspector] public sbyte m_Phase;
    
    private Vector3 m_TargetPosition;
    private Vector3 m_DefaultScale;
    private float m_AppearanceTime1 = 3f, m_AppearanceTime2 = 1.2f;
    private float m_Direction;
    private bool m_Pattern1Direction;
    private bool m_InPattern = false;

    private IEnumerator m_CurrentPhase, m_CurrentPattern, m_CurrentSubPattern1, m_CurrentSubPattern2;

    void Start()
    {
        m_Phase = -1;
        m_TargetPosition = new Vector2(0f, -4.2f);
        m_DefaultScale = transform.localScale;
        transform.localScale = new Vector3(2f, 2f, 2f);
        m_MoveVector = new MoveVector(1f, -125f);
        RotateImmediately(m_MoveVector.direction);
        
        DisableAttackable(4.2f);

        m_Sequence = DOTween.Sequence()
        .Append(DOTween.To(()=>m_MoveVector.speed, x=>m_MoveVector.speed = x, 15f, 2f).SetEase(Ease.InQuad));
        //ToNextPhase(m_AppearanceTime);

        Invoke("Appearance", m_AppearanceTime1);
    }

    protected override void Update()
    {
        if (m_Phase == 0) {
            if (m_Health <= m_MaxHealth * 0.40f) { // 체력 40% 이하
                ToNextPhase();
            }
        }

        if (m_Phase >= 0) {
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

        if (m_Pattern1Direction)
            m_Direction += 90f*Time.deltaTime;
        else
            m_Direction -= 90f*Time.deltaTime;
        
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
        m_Phase = 0;
        m_CurrentPhase = PatternPhase0();
        StartCoroutine(m_CurrentPhase);
    }

    public void ToNextPhase() {
        float duration = 2f;
        m_Phase++;
        if (m_Phase > 0) {
            m_SystemManager.EraseBullets(1f);
            
            if (m_CurrentPattern != null)
                StopCoroutine(m_CurrentPattern);
            if (m_CurrentPhase != null)
                StopCoroutine(m_CurrentPhase);

            if (m_Phase == 1) {
                m_CurrentPhase = PatternPhase1();
                StartCoroutine(m_CurrentPhase);
                m_Part.SetActive(false);
                m_Collider2D[0].gameObject.SetActive(false);
                m_Collider2D[1].gameObject.SetActive(true);
                EnableInvincible(duration);
                NextPhaseExplosion(duration);
            }
        }
    }

    private void NextPhaseExplosion(float duration) {
        ExplosionEffect(0, 0, new Vector2(1f, 0.44f));
        ExplosionEffect(0, -1, new Vector2(-1f, 0.44f));
        StartCoroutine(NextPhaseExplosionEffect1(duration));
        StartCoroutine(NextPhaseExplosionEffect2(duration));
    }

    private IEnumerator NextPhaseExplosionEffect1(float duration) {
        float t = 0f, t_add = 0f;
        while (t < duration) {
            t_add = Random.Range(0.35f, 0.5f);
            ExplosionEffect(1, 1, new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(-0.5f, 0.8f), Depth.EXPLOSION));
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield return null;
    }

    private IEnumerator NextPhaseExplosionEffect2(float duration) {
        float t = 0f, t_add = 0f;
        while (t < duration) {
            t_add = Random.Range(0.2f, 0.4f);
            ExplosionEffect(2, 2, new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(-0.5f, 0.8f), Depth.EXPLOSION));
            ExplosionEffect(2, -1, new Vector3(Random.Range(-0.7f, 0.7f), Random.Range(-0.5f, 0.8f), Depth.EXPLOSION));
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield break;
    }

    private IEnumerator PatternPhase0() {
        int side;
        yield return new WaitForSeconds(1f);
        while(m_Phase == 0) {
            m_CurrentPattern = Pattern1();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return null;
            yield return new WaitForSeconds(2f);
            
            side = Random.Range(0, 2);
            if (side == 0)
                side = -1;
            
            m_Turret[0].StartPattern(1, side);
            m_Turret[1].StartPattern(1, side);
            m_CurrentPattern = Pattern2();
            StartCoroutine(m_CurrentPattern);
            while (m_InPattern)
                yield return null;
            m_Turret[0].StopPattern();
            m_Turret[1].StopPattern();
            yield return new WaitForSeconds(2f);
        }
        yield break;
    }

    private IEnumerator PatternPhase1() {
        yield return null;
    }

    private IEnumerator Pattern1() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float[] fire_delay = { 0.18f, 0.13f, 0.1f };
        int random_value = Random.Range(0, 2);

        if (random_value == 0) {
            random_value = -1;
            m_Pattern1Direction = false;
        }
        else
            m_Pattern1Direction = true;
        
        m_InPattern = true;

        for (int i = 0; i < 3; i++) {
            pos = m_FirePosition[0].position;
            m_Direction = GetAngleToTarget(pos, m_PlayerManager.m_Player.transform.position) - 45f*random_value;
            m_CurrentSubPattern1 = Pattern1A(i);
            m_CurrentSubPattern2 = Pattern1B(i);
            StartCoroutine(m_CurrentSubPattern1);
            StartCoroutine(m_CurrentSubPattern2);
            yield return new WaitForSeconds(1f);
            if (m_CurrentSubPattern1 != null)
                StopCoroutine(m_CurrentSubPattern1);
            if (m_CurrentSubPattern2 != null)
                StopCoroutine(m_CurrentSubPattern2);
            m_Pattern1Direction ^= true;
            random_value *= -1;
        }
        
        m_InPattern = false;
        yield break;
    }

    private IEnumerator Pattern1A(int level) {
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

    private IEnumerator Pattern1B(int level) {
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

    private IEnumerator Pattern2() {
        Vector3 pos;
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        float duration;
        float[] fire_delay = { 1f, 0.5f, 0.33f };

        m_InPattern = true;

        for (int j = 0; j < 4; j++) {
            duration = 0f;
            while (duration < 1.6f) {
                duration += fire_delay[m_SystemManager.m_Difficulty];
                pos = m_FirePosition[0].position;
                for (int i = 0; i < j; i++) {
                    CreateBullet(3, pos, 7f + i*0.6f, GetAngleToTarget(pos, m_PlayerManager.m_Player.transform.position), accel);
                }
                yield return new WaitForSeconds(fire_delay[m_SystemManager.m_Difficulty]);
            }
        }
        m_InPattern = false;
        yield break;
    }



    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        if (m_CurrentPattern != null)
            StopCoroutine(m_CurrentPattern);
        if (m_CurrentPhase != null)
            StopCoroutine(m_CurrentPhase);
        m_SystemManager.BulletsToGems(2f);
        m_MoveVector = new MoveVector(1f, 0f);
        
        yield return new WaitForSeconds(0.6f);

        StartCoroutine(DeathExplosion1(2.8f));
        StartCoroutine(DeathExplosion2(2.8f));
        StartCoroutine(DeathExplosion3(2.8f));

        yield return new WaitForSeconds(3f);
        ExplosionEffect(2, 4, new Vector2(0f, 0.64f)); // 최종 파괴
        ExplosionEffect(2, -1, new Vector2(0f, -0.36f));
        ExplosionEffect(1, -1, new Vector2(0.5f, 0.75f));
        ExplosionEffect(1, -1, new Vector2(0.5f, 0.2f));
        ExplosionEffect(1, -1, new Vector2(-0.5f, 0.75f));
        ExplosionEffect(1, -1, new Vector2(-0.5f, 0.2f));
        ExplosionEffect(1, -1, new Vector2(0f, -0.75f));
        ExplosionEffect(0, -1, new Vector2(0.2f, 0.18f));
        ExplosionEffect(0, -1, new Vector2(-0.2f, 0.18f));
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
