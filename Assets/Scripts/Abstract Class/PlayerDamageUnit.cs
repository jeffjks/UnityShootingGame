using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ================ 적에게 데미지를 주는 개체 ================ //

public abstract class PlayerDamageUnit : MonoBehaviour, CanDeath
{
    public string m_ObjectName;
    public float m_Damage;
    
    protected PlayerManager m_PlayerManager = null;
    protected PoolingManager m_PoolingManager = null;
    protected float m_DefaultDamage;

    protected Vector2 m_Vector2 = Vector2.zero;

    public abstract void OnDeath();

    void Awake()
    {
        m_PlayerManager = PlayerManager.instance_pm;
        m_PoolingManager = PoolingManager.instance_op;
        m_DefaultDamage = m_Damage;
    }

    protected void MoveVector() {
        transform.Translate(m_Vector2 * Time.deltaTime, Space.World);
    }
}



public abstract class PlayerMissile : PlayerDamageUnit
{
    [SerializeField] private float m_DamageBonus = 0f;
    [SerializeField] protected float m_Speed = 1f;
    [SerializeField] private bool m_IsPenetrate = false;
    [SerializeField] private GameObject[] m_ActivatedObject = null;
    [Header("-1 : None / 0 : Shot(fire) / 1 : Homing(purple) / 2 : Rocket(metal)")]
    [SerializeField] private int m_HitEffectNumber = -1;

    [HideInInspector] public int m_DamageLevel;
    
    protected bool m_HasDamaged = false;

    protected abstract void OnStart();
    
    void OnEnable()
    {
        OnStart();
        m_Damage = m_DefaultDamage + m_DamageBonus * m_DamageLevel;
        for(int i = 0; i < m_ActivatedObject.Length; i++) {
            m_ActivatedObject[i].SetActive(false);
        }
        if (m_ActivatedObject.Length > 0) {
            m_ActivatedObject[m_DamageLevel].SetActive(true);
        }
        m_HasDamaged = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!m_HasDamaged) {
            if (other.gameObject.CompareTag("Enemy")) { // 대상이 적 유닛이고
            
                EnemyUnit enemyObject = other.gameObject.GetComponentInParent<EnemyUnit>();
                if (m_IsPenetrate) { // 관통 공격이며
                    if ((1 << other.gameObject.layer & Layer.SMALL) != 0) { // 적이 소형이면
                        enemyObject.OnDeath(); // 기냥 죽임
                    }
                }
                else { // 그 이외의 경우에는
                    enemyObject.TakeDamage(m_Damage); // 데미지 주고
                    m_HasDamaged = true;
                    OnDeath(); // 자신 파괴
                }
            }
        }
    }

    public override void OnDeath() {
        if (m_HitEffectNumber != -1) {
            GameObject obj = m_PoolingManager.PopFromPool("PlayerHitEffect", PoolingParent.EXPLOSION); // 히트 이펙트
            HitEffect hitEffect = obj.GetComponent<HitEffect>();
            hitEffect.m_HitEffectType = m_HitEffectNumber;
            hitEffect.transform.position = transform.position;
            obj.SetActive(true);
        }
        m_PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.PLAYER_MISSILE);
    }
}