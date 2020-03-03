using System.Collections;
using UnityEngine;
using DG.Tweening;

public class EnemyMiddleBoss5a : EnemyUnit
{
    public EnemyMiddleBoss5aMainTurret m_MainTurret;
    public EnemyMiddleBoss5aTurret[] m_Turret = new EnemyMiddleBoss5aTurret[2];
    public EnemyMissile[] m_Missiles = new EnemyMissile[8];
    [HideInInspector] public sbyte m_Phase;
    
    private Vector3 m_TargetPosition;
    private bool m_TimeLimitState = false;
    private float m_AppearanceTime = 2.5f;

    private IEnumerator m_CurrentPhase, m_CurrentPattern1, m_CurrentPattern2;
    private bool m_Pattern1B;

    void Start()
    {
        float time_limit = 38f;
        
        m_TargetPosition = new Vector3(0f, -4f, Depth.ENEMY);
        
        m_Sequence = DOTween.Sequence()
        .Append(transform.DOMoveY(m_TargetPosition.y, m_AppearanceTime).SetEase(Ease.OutQuad));
        
        Invoke("TimeLimit", m_AppearanceTime + time_limit);
        Invoke("OnAppearanceComplete", m_AppearanceTime);
    }

    protected override void Update()
    {
        if (!m_TimeLimitState) {
            if (m_Phase > 0) {
                if (transform.position.x >= m_TargetPosition.x + 0.6f) {
                    m_MoveVector.direction = Random.Range(-100f, -80f);
                }
                else if (transform.position.x <= m_TargetPosition.x - 0.6f) {
                    m_MoveVector.direction = Random.Range(80f, 100f);
                }
                else if (transform.position.y >= m_TargetPosition.y + 0.3f) {
                    m_MoveVector = new MoveVector(new Vector2(m_MoveVector.GetVector().x, -m_MoveVector.GetVector().y));
                }
                else if (transform.position.y <= m_TargetPosition.y - 0.3f) {
                    m_MoveVector = new MoveVector(new Vector2(m_MoveVector.GetVector().x, -m_MoveVector.GetVector().y));
                }
            }
        }

        base.Update();
    }

    private void OnAppearanceComplete() {
        float random_direction = Random.Range(80f, 100f) + 180f*Random.Range(0, 2);
        m_MoveVector = new MoveVector(0.4f, random_direction);
        m_Phase = 1;
        m_CurrentPhase = Phase1();
        StartCoroutine(m_CurrentPhase);
    }

    private void TimeLimit() {
        m_TimeLimitState = true;
        transform.DOMoveY(Size.GAME_BOUNDARY_BOTTOM - 8f, 5f).SetEase(Ease.InQuad);
    }

    private IEnumerator Phase1() { // 페이즈1 패턴 ============================
        int rand;
        yield return new WaitForSeconds(1f);
        while (m_Phase == 1) {
            m_MainTurret.StartPattern(1);
            yield return new WaitForSeconds(2f);
            m_Turret[0].m_RotatePattern = 21;
            m_Turret[1].m_RotatePattern = 22;
            m_Turret[0].StartPattern(1);
            m_Turret[1].StartPattern(1);
            yield return new WaitForSeconds(2f);
            m_MainTurret.StopPattern();
            yield return new WaitForSeconds(2f);
            m_Turret[0].StopPattern();
            m_Turret[1].StopPattern();
            m_Turret[0].m_RotatePattern = 10;
            m_Turret[1].m_RotatePattern = 10;
            yield return new WaitForSeconds(2.2f);
            m_MainTurret.StartPattern(4);
            m_MainTurret.m_RotatePattern = 21;
            yield return new WaitForSeconds(0.5f);
            m_MainTurret.StopPattern();

            yield return new WaitForSeconds(0.5f);
            StartCoroutine(LaunchMissile());
            yield return new WaitForSeconds(1f);
            m_MainTurret.StartPattern(2);
            yield return new WaitForSeconds(6f);
            m_MainTurret.StopPattern();
            m_MainTurret.m_RotatePattern = 10;
            yield return new WaitForSeconds(4f);
            
            m_MainTurret.StartPattern(1);
            yield return new WaitForSeconds(1f);
            rand = Random.Range(0, 2);
            m_Turret[0].StartPattern(2, rand);
            m_Turret[1].StartPattern(2, 1-rand);
            yield return new WaitForSeconds(6f);
            m_MainTurret.StopPattern();
            m_Turret[0].StopPattern();
            m_Turret[1].StopPattern();

            rand = Random.Range(0, 2);
            m_Turret[0].m_RotatePattern = (byte) (31 + rand);
            m_Turret[1].m_RotatePattern = (byte) (32 - rand);
            yield return new WaitForSeconds(2f);
            m_Turret[0].m_RotatePattern = 22;
            m_Turret[1].m_RotatePattern = 21;
            m_Turret[0].StartPattern(3);
            m_Turret[1].StartPattern(3);
            yield return new WaitForSeconds(2f);
            m_MainTurret.StartPattern(3);
            yield return new WaitForSeconds(6f);
            m_Turret[0].StopPattern();
            m_Turret[1].StopPattern();
            m_MainTurret.StopPattern();
            break;
        }
        yield break;
    }


    private IEnumerator LaunchMissile() {
        for (int i = 0; i < 4; i++) {
            yield return new WaitForSeconds(2f);
            if (!m_IsUnattackable) {
                try {
                    m_Missiles[i*2].enabled = true;
                    m_Missiles[i*2 + 1].enabled = true;
                }
                catch {
                }
            }
        }
        yield break;
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        m_SystemManager.BulletsToGems(2.5f);
        m_MoveVector = new MoveVector(1.5f, 0f);
        
        m_Phase = -1;

        StartCoroutine(DeathExplosion1(2f));
        StartCoroutine(DeathExplosion2(2f));

        yield return new WaitForSeconds(2.1f);
        ExplosionEffect(2, 2); // 최종 파괴
        ExplosionEffect(0, -1, new Vector2(-2f, -1.6f));
        ExplosionEffect(0, -1, new Vector2(2f, -1.6f));
        ExplosionEffect(0, -1, new Vector2(-2f, 1.6f));
        ExplosionEffect(0, -1, new Vector2(2f, 1.6f));
        ExplosionEffect(0, -1, new Vector2(0f, -3f));
        m_SystemManager.ScreenEffect(0);
        
        Destroy(gameObject);
        yield break;
    }

    private IEnumerator DeathExplosion1(float timer) {
        float t = 0f, t_add = 0f;
        Vector2 random_pos;
        while (t < timer) {
            t_add = Random.Range(0.2f, 0.35f);
            random_pos = new Vector2(Random.Range(-2.5f, 2.5f), Random.Range(-2.5f, 1.5f));
            ExplosionEffect(0, 0, random_pos, new MoveVector(Random.Range(2f, 3f), Random.Range(0f, 360f)));
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield break;
    }

    private IEnumerator DeathExplosion2(float timer) {
        float t = 0f, t_add = 0f;
        Vector2 random_pos;
        while (t < timer) {
            t_add = Random.Range(0.4f, 0.7f);
            random_pos = new Vector2(Random.Range(-2.5f, 2.5f), Random.Range(-2.5f, 1.5f));
            ExplosionEffect(1, 1, random_pos, new MoveVector(Random.Range(3f, 4f), Random.Range(0f, 360f)));
            t += t_add;
            yield return new WaitForSeconds(t_add);
        }
        yield break;
    }
}
