using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBomb : MonoBehaviour
{
    public GameObject m_Bomb, m_Explosion, m_BombDamage;
    public ParticleSystem m_ParticleSystem;

    private Vector3 m_Target;
    private Vector3 m_FixedPosition;

    private SystemManager m_SystemManager = null;
    private PlayerManager m_PlayerManager = null;

    private const int TARGET_TIMER = 400;
    private const int REMOVE_TIMER = 2600; // TARGET_TIMER 이후

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

        float target_x = Mathf.Clamp(transform.position.x, -3f, 3f);
        float target_y = - Size.GAME_HEIGHT/2 - 1f;
        m_Target = new Vector3(target_x, target_y, Depth.PLAYER_MISSILE);
        Vector3 relativePos = m_Target - m_Bomb.transform.position;

        m_Bomb.transform.rotation = Quaternion.LookRotation(relativePos);
        
        StartCoroutine(BombExplosion());
    }

    void Update()
    {
        transform.position = m_FixedPosition;
        transform.rotation = Quaternion.identity;
    }

    private IEnumerator BombExplosion() {
        Vector3 init_position = transform.position;
        int frame = TARGET_TIMER * Application.targetFrameRate / 1000;

        for (int i = 0; i < frame; ++i) {
            float t_pos = AC_Ease.ac_ease[EaseType.OutQuad].Evaluate((float) (i+1) / frame);
            
            transform.position = Vector3.Lerp(init_position, m_Target, t_pos);
            yield return new WaitForMillisecondFrames(0);
        }
        
        m_SystemManager.EraseBullets(2000);
        m_Bomb.SetActive(false);
        m_Explosion.SetActive(true);
        m_BombDamage.SetActive(true);

        yield return new WaitForMillisecondFrames(REMOVE_TIMER);
        m_Bomb.SetActive(false);
        m_Explosion.SetActive(false);
        m_BombDamage.SetActive(false);
        gameObject.SetActive(false);
        yield break;
    }
}