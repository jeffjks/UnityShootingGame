using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerBomb : MonoBehaviour
{
    public GameObject m_Bomb, m_Explosion, m_BombDamage;
    public ParticleSystem m_ParticleSystem;

    private Vector3 m_Target;
    private Vector3 m_FixedPosition;

    private SystemManager m_SystemManager = null;
    private PlayerManager m_PlayerManager = null;

    void Awake()
    {
        m_SystemManager = SystemManager.instance_sm;
        m_PlayerManager = PlayerManager.instance_pm;

        m_FixedPosition = new Vector3(0f, -Size.GAME_HEIGHT/2, Depth.PLAYER_MISSILE);
        transform.position = m_FixedPosition;
    }

    void OnEnable()
    {
        m_Bomb.SetActive(true);
        m_Bomb.transform.position = m_PlayerManager.m_Player.transform.position;

        float target_timer = 0.4f;
        float remove_timer = 3f;

        float target_x = Mathf.Clamp(transform.position.x, -3f, 3f);
        float target_y = - Size.GAME_HEIGHT/2 - 1f;
        m_Target = new Vector3(target_x, target_y, Depth.PLAYER_MISSILE);
        Vector3 relativePos = m_Target - m_Bomb.transform.position;
        
        m_Explosion.transform.position = new Vector3(transform.position.x, transform.position.y, Depth.EXPLOSION);
        m_Bomb.transform.DOMove(m_Target, target_timer).SetEase(Ease.OutQuad);
        m_Bomb.transform.rotation = Quaternion.LookRotation(relativePos);
        
        Invoke("BombExplosion", target_timer);
        Invoke("BombEnd", remove_timer);
    }

    void Update()
    {
        transform.position = m_FixedPosition;
        transform.rotation = Quaternion.identity;
    }

    private void BombExplosion() {
        m_SystemManager.EraseBullets(2f);
        m_Bomb.SetActive(false);
        m_Explosion.SetActive(true);
        m_BombDamage.SetActive(true);
    }

    private void BombEnd() {
        transform.DOKill();
        CancelInvoke();
        m_Bomb.SetActive(false);
        m_Explosion.SetActive(false);
        m_BombDamage.SetActive(false);
        gameObject.SetActive(false);
    }
}