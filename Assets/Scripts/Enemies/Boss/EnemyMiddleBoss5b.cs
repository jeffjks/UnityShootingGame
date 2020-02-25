using UnityEngine;
using System.Collections;
using DG.Tweening;

public class EnemyMiddleBoss5b : EnemyUnit
{
    [SerializeField] private float[] m_FireDelay = new float[Difficulty.DIFFICULTY_SIZE];
    [SerializeField] private Transform[] m_FirePosition = new Transform[3];
    [SerializeField] private GameObject m_Hull = null;
    [SerializeField] private EnemyMiddleBoss5bTurret m_Turret = null;
    public Transform m_Renderer;
    private IEnumerator m_CurrentPattern1, m_CurrentPattern2;
    
    private Vector3 m_TargetPosition;
    private float m_AppearanceTime = 2f;
    private byte m_Phase;

    void Start()
    {
        float duration = 3f;
        int random_value = Random.Range(-1, 1);
        if (random_value == 0)
            random_value = 1;
        m_TargetPosition = new Vector3(0f, -3.8f, Depth.ENEMY);

        m_Sequence = DOTween.Sequence()
        .Append(transform.DOMove(m_TargetPosition, m_AppearanceTime).SetEase(Ease.OutQuad))
        .Append(transform.DOMove(new Vector3(-3f*random_value, -3.8f, Depth.ENEMY), 2f).SetEase(Ease.InOutQuad))
        .Append(transform.DOMove(new Vector3(3f*random_value, -3.8f, Depth.ENEMY), duration).SetEase(Ease.InOutQuad))
        .Append(transform.DOMove(new Vector3(-3f*random_value, -3.8f, Depth.ENEMY), duration).SetEase(Ease.InOutQuad))
        .Append(transform.DOMove(new Vector3(3f*random_value, -3.8f, Depth.ENEMY), duration).SetEase(Ease.InOutQuad))
        .Append(transform.DOMove(new Vector3(-3f*random_value, -3.8f, Depth.ENEMY), duration).SetEase(Ease.InOutQuad))
        .Append(transform.DOMove(new Vector3(3f*random_value, -3.8f, Depth.ENEMY), duration).SetEase(Ease.InOutQuad))
        .Append(transform.DOMove(new Vector3(-3f*random_value, -3.8f, Depth.ENEMY), duration).SetEase(Ease.InOutQuad))
        .Append(transform.DOMove(new Vector3(0f, -3.8f, Depth.ENEMY), 2f).SetEase(Ease.InOutQuad))
        .Append(transform.DOMoveY(10f, 3f).SetEase(Ease.InQuad));
        
        GetCoordinates();
        InvokeRepeating("Pattern1", 1f, m_FireDelay[m_SystemManager.m_Difficulty]);
        m_CurrentPattern1 = Pattern1();
        StartCoroutine(m_CurrentPattern1);
        RotateImmediately(m_PlayerPosition);
    }

    protected override void Update()
    {
        if (m_Phase == 0) {
            if (m_Health <= m_MaxHealth * 0.4f) { // 체력 40% 이하
                m_Phase = 1;
                if (m_CurrentPattern2 != null)
                    StopCoroutine(m_CurrentPattern2);
                Destroy(m_Hull);
                ExplosionEffect(0, 0);
                
                m_Turret.StartPattern1();
            }
        }

        if (m_PlayerManager.m_PlayerIsAlive)
            RotateSlightly(m_PlayerPosition, 40f);
        else
            RotateSlightly(m_PlayerPosition, 100f);
        
        base.Update();
    }

    private IEnumerator Pattern1() {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        Vector3[] pos = new Vector3[2];
        sbyte state = 1;
        yield return new WaitForSeconds(2f);
        
        while (true) {
            pos[0] = m_FirePosition[0].position;
            pos[1] = m_FirePosition[1].position;
            if (m_Phase == 0) {
                m_CurrentPattern2 = Pattern2(state);
                StartCoroutine(m_CurrentPattern2);
            }
            else if (m_Phase == 1) {
                m_Turret.StartCoroutine("Pattern2");
            }
            state *= -1;
            for (int i = 0; i < 2; i++) {
                if (m_SystemManager.m_Difficulty == 0) {
                    CreateBullet(0, pos[i], 5.4f, m_CurrentAngle - 56.3f, accel); // 108.1
                    CreateBullet(0, pos[i], 6.6f, m_CurrentAngle - 24.3f, accel); // 131.5
                    CreateBullet(0, pos[i], 7.5f, m_CurrentAngle, accel); // 160
                    CreateBullet(0, pos[i], 6.6f, m_CurrentAngle + 24.3f, accel); // 131.5
                    CreateBullet(0, pos[i], 5.4f, m_CurrentAngle + 56.3f, accel); // 108.1
                }
                else if (m_SystemManager.m_Difficulty == 1) {
                    CreateBullet(2, pos[i], 4.1f, m_CurrentAngle - 63.4f, accel); // 82.7
                    CreateBullet(2, pos[i], 4.5f, m_CurrentAngle - 41.4f, accel); // 89.2
                    CreateBullet(2, pos[i], 5.1f, m_CurrentAngle - 23.8f, accel); // 101.6
                    CreateBullet(2, pos[i], 5.8f, m_CurrentAngle - 7.4f, accel); // 116.9
                    CreateBullet(2, pos[i], 5.8f, m_CurrentAngle + 7.4f, accel); // 116.9
                    CreateBullet(2, pos[i], 5.1f, m_CurrentAngle + 23.8f, accel); // 101.6
                    CreateBullet(2, pos[i], 4.5f, m_CurrentAngle + 41.4f, accel); // 89.2
                    CreateBullet(2, pos[i], 4.1f, m_CurrentAngle + 63.4f, accel); // 82.7

                    CreateBullet(0, pos[i], 5.3f, m_CurrentAngle - 75.3f, accel); // 106.4
                    CreateBullet(0, pos[i], 5.4f, m_CurrentAngle - 56.3f, accel); // 108.1
                    CreateBullet(0, pos[i], 5.9f, m_CurrentAngle - 38.8f, accel); // 118.1
                    CreateBullet(0, pos[i], 6.6f, m_CurrentAngle - 24.3f, accel); // 131.5
                    CreateBullet(0, pos[i], 7.5f, m_CurrentAngle - 10.8f, accel); // 150.6
                    CreateBullet(0, pos[i], 7.8f, m_CurrentAngle, accel); // 160
                    CreateBullet(0, pos[i], 7.5f, m_CurrentAngle + 10.8f, accel); // 150.6
                    CreateBullet(0, pos[i], 6.6f, m_CurrentAngle + 24.3f, accel); // 131.5
                    CreateBullet(0, pos[i], 5.9f, m_CurrentAngle + 38.8f, accel); // 118.1
                    CreateBullet(0, pos[i], 5.4f, m_CurrentAngle + 56.3f, accel); // 108.1
                    CreateBullet(0, pos[i], 5.3f, m_CurrentAngle + 75.3f, accel); // 106.4
                }
                else {
                    CreateBullet(2, pos[i], 4.1f, m_CurrentAngle - 63.4f, accel); // 82.7
                    CreateBullet(2, pos[i], 4.5f, m_CurrentAngle - 41.4f, accel); // 89.2
                    CreateBullet(2, pos[i], 5.1f, m_CurrentAngle - 23.8f, accel); // 101.6
                    CreateBullet(2, pos[i], 5.8f, m_CurrentAngle - 7.4f, accel); // 116.9
                    CreateBullet(2, pos[i], 5.8f, m_CurrentAngle + 7.4f, accel); // 116.9
                    CreateBullet(2, pos[i], 5.1f, m_CurrentAngle + 23.8f, accel); // 101.6
                    CreateBullet(2, pos[i], 4.5f, m_CurrentAngle + 41.4f, accel); // 89.2
                    CreateBullet(2, pos[i], 4.1f, m_CurrentAngle + 63.4f, accel); // 82.7

                    CreateBullet(0, pos[i], 5.3f, m_CurrentAngle - 75.3f, accel); // 106.4
                    CreateBullet(0, pos[i], 5.4f, m_CurrentAngle - 56.3f, accel); // 108.1
                    CreateBullet(0, pos[i], 5.9f, m_CurrentAngle - 38.8f, accel); // 118.1
                    CreateBullet(0, pos[i], 6.6f, m_CurrentAngle - 24.3f, accel); // 131.5
                    CreateBullet(0, pos[i], 7.5f, m_CurrentAngle - 10.8f, accel); // 150.6
                    CreateBullet(0, pos[i], 7.8f, m_CurrentAngle, accel); // 160
                    CreateBullet(0, pos[i], 7.5f, m_CurrentAngle + 10.8f, accel); // 150.6
                    CreateBullet(0, pos[i], 6.6f, m_CurrentAngle + 24.3f, accel); // 131.5
                    CreateBullet(0, pos[i], 5.9f, m_CurrentAngle + 38.8f, accel); // 118.1
                    CreateBullet(0, pos[i], 5.4f, m_CurrentAngle + 56.3f, accel); // 108.1
                    CreateBullet(0, pos[i], 5.3f, m_CurrentAngle + 75.3f, accel); // 106.4
                }
            }
            yield return new WaitForSeconds(m_FireDelay[m_SystemManager.m_Difficulty]);
        }
    }

    private IEnumerator Pattern2(sbyte state) {
        EnemyBulletAccel accel = new EnemyBulletAccel(0f, 0f);
        yield return new WaitForSeconds(2f);
        Vector3 pos;
        float random_value = Random.Range(-6f, 6f);
        
        if (m_SystemManager.m_Difficulty == 0) {
            for (int i = 0; i < 4; i++) {
                pos = m_FirePosition[2].position;
                CreateBulletsSector(4, pos, 6.7f - 0.2f*i, m_CurrentAngle + random_value + 4.8f*i*state, accel, 5, 25f);
                yield return new WaitForSeconds(0.26f);
            }
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            for (int i = 0; i < 5; i++) {
                pos = m_FirePosition[2].position;
                CreateBulletsSector(4, pos, 7f - 0.2f*i, m_CurrentAngle + random_value + 4.8f*i*state, accel, 7, 19f);
                yield return new WaitForSeconds(0.21f);
            }
        }
        else {
            for (int i = 0; i < 7; i++) {
                pos = m_FirePosition[2].position;
                CreateBulletsSector(4, pos, 7f - 0.2f*i, m_CurrentAngle + random_value + 4.8f*i*state, accel, 7, 19f);
                yield return new WaitForSeconds(0.15f);
            }
        }
        yield break;
    }


    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        m_MoveVector = new MoveVector(1.2f, 0f);
        m_SystemManager.BulletsToGems(3f);
        m_Phase = 2;
        if (m_CurrentPattern1 != null)
            StopCoroutine(m_CurrentPattern1);
        m_Turret.OnDeath();
        
        m_Sequence = DOTween.Sequence()
        .Append(m_Renderer.DORotateQuaternion(new Quaternion(-0.2f, 0.9f, -0.4f, -0.2f), 2.5f).SetEase(Ease.Linear))
        .Join(m_Renderer.DOScale(new Vector3(0.7f, 0.7f, 0.7f), 2.5f).SetEase(Ease.Linear));

        StartCoroutine(DeathExplosion1());
        StartCoroutine(DeathExplosion2());
        yield return new WaitForSeconds(2.1f);

        ExplosionEffect(0, 2, new Vector2(0f, 0f));
        ExplosionEffect(1, -1, new Vector2(1.3f, 0f));
        ExplosionEffect(1, -1, new Vector2(-1.3f, 0f));
        ExplosionEffect(1, -1, new Vector2(0f, 1.4f));
        m_SystemManager.ScreenEffect(0);
        Destroy(gameObject);
        yield break;
    }

    private IEnumerator DeathExplosion1() {
        float timer = 0f, random_timer = 0f;
        Vector2 random_pos;
        while (timer < 2f) {
            random_timer = Random.Range(0.2f, 0.3f);
            random_pos = (Vector2) Random.insideUnitCircle * 1.2f;
            ExplosionEffect(1, -1, random_pos);
            yield return new WaitForSeconds(random_timer);
        }
        yield break;
    }

    private IEnumerator DeathExplosion2() {
        float timer = 0f, random_timer = 0f;
        Vector2 random_pos;
        while (timer < 2f) {
            random_timer = Random.Range(0.15f, 0.25f);
            random_pos = (Vector2) Random.insideUnitCircle * 1.7f;
            ExplosionEffect(2, 1, random_pos);
            yield return new WaitForSeconds(random_timer);
        }
        yield break;
    }
}
