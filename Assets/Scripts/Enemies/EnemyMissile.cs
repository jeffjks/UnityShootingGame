using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyMissile : EnemyUnit
{
    public Transform m_Renderer;
    public GameObject m_Engine;

    private Quaternion m_Rotation;

    protected override void Awake()
    {
        base.Awake();
        m_Rotation = m_Renderer.rotation;
    }

    void OnEnable()
    {
        transform.parent = null;
        m_Collider2D[0].enabled = true;
        StartCoroutine(StartEngine());
        m_MoveVector = new MoveVector(1f, 0f);
        UpdateTransform();
        m_Renderer.rotation = m_Rotation;
        
        m_Sequence = DOTween.Sequence();
        m_Sequence.AppendInterval(2.2f);
        DOTween.To(()=>m_MoveVector.speed, x=>m_MoveVector.speed = x, 6f, 1f).SetEase(Ease.Linear);
    }

    private IEnumerator StartEngine() {
        yield return new WaitForSeconds(1f);
        m_Engine.SetActive(true);
        yield break;
    }

    protected override IEnumerator AdditionalOnDeath() { // 파괴 과정
        Vector3 pos = transform.position;
        
        if (m_SystemManager.m_Difficulty == 0) {
            CreateBulletsSector(0, pos, 0.5f, -15f, new EnemyBulletAccel(4.5f, 0.5f), 12, 30f);
        }
        else if (m_SystemManager.m_Difficulty == 1) {
            CreateBulletsSector(3, pos, 0.25f, -12f, new EnemyBulletAccel(4f, 0.5f), 15, 24f);
            CreateBulletsSector(0, pos, 0.75f, 0f, new EnemyBulletAccel(5f, 0.5f), 15, 24f);
        }
        else {
            CreateBulletsSector(3, pos, 0.25f, -10f, new EnemyBulletAccel(4f, 0.5f), 18, 20f);
            CreateBulletsSector(0, pos, 0.75f, 0f, new EnemyBulletAccel(5f, 0.5f), 18, 20f);
        }
        ExplosionEffect(0, -1, new Vector2(0f, -1f));
        ExplosionEffect(0, -1, new Vector2(0f, 1f));
        
        CreateItems();
        Destroy(gameObject);
        yield break;
    }
}
