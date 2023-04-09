using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ================ 적에게 데미지를 주는 개체 ================ //

public abstract class PlayerDamageUnit : MonoBehaviour
{
    public string m_ObjectName;
    public int m_Damage;
    [Header("단위: %")]
    public int[] m_DamageScale = new int[3];
    
    protected PlayerManager m_PlayerManager = null;
    protected PoolingManager m_PoolingManager = null;
    protected int m_DefaultDamage;

    protected Vector2Int m_Vector2 = Vector2Int.zero;
    public Vector2Int m_Position; // INTEGER 기반 위치

    protected virtual void Awake()
    {
        m_PlayerManager = PlayerManager.instance_pm;
        m_PoolingManager = PoolingManager.instance_op;
        m_DefaultDamage = m_Damage;

        m_Position = Vector2Int.RoundToInt(transform.position*256);
    }

    protected void SetPosition() {
        transform.position = new Vector3((float) m_Position.x / 256, (float) m_Position.y / 256, transform.position.z);
        //transform.position = (Vector2) m_Position / 256;
    }

    protected void MoveVector() {
        if (Time.timeScale == 0) {
            return;
        }
        m_Position += m_Vector2;
    }

    protected void DealDamage(EnemyUnit enemyObject, int damage, sbyte damage_type = -1) {
        try {
            enemyObject.TakeDamage(damage * m_DamageScale[(int) enemyObject.m_Class] / 100, damage_type);
        }
        catch (System.IndexOutOfRangeException) {
            enemyObject.TakeDamage(damage, damage_type);
        }
    }
}



public abstract class PlayerMissile : PlayerDamageUnit, UseObjectPool
{
    [SerializeField] private int[] m_DamageBonus = {0, 0, 0};
    [SerializeField] protected int m_Speed = 0;
    [SerializeField] private bool m_IsPenetrate = false;
    [SerializeField] private GameObject[] m_ActivatedObject = null;
    [Header("-1 : None / 0 : Shot(fire) / 1 : Homing(purple) / 2 : Rocket(metal)")]
    [SerializeField] private int m_HitEffectNumber = -1;

    [HideInInspector] public int m_DamageLevel;
    
    protected bool m_HasDamaged = false;
    
    public virtual void OnStart() {
        m_Position = Vector2Int.RoundToInt(transform.position*256);
        try {
            m_Damage = m_DefaultDamage + m_DamageBonus[m_DamageLevel];
        }
        catch {
            Debug.LogAssertion("Damage Level Index Out Of Bound: "+m_DamageLevel);
        }
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
                    DealDamage(enemyObject, m_Damage); // 데미지 주고
                    m_HasDamaged = true;
                    PlayHitAnimation(); // 자신 파괴
                }
            }
        }
    }

    private void PlayHitAnimation() {
        if (m_HitEffectNumber != -1) {
            GameObject obj = m_PoolingManager.PopFromPool("PlayerHitEffect", PoolingParent.EXPLOSION); // 히트 이펙트
            HitEffect hitEffect = obj.GetComponent<HitEffect>();
            hitEffect.m_HitEffectType = m_HitEffectNumber;
            hitEffect.transform.position = new Vector3(transform.position.x, transform.position.y, Depth.HIT_EFFECT);
            obj.SetActive(true);
            hitEffect.OnStart();
        }
        ReturnToPool();
    }

    public void ReturnToPool() {
        m_PoolingManager.PushToPool(m_ObjectName, gameObject, PoolingParent.PLAYER_MISSILE);
    }
}