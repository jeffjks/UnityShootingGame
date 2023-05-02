using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBomb : MonoBehaviour
{
    public Transform m_PlayerTransform;
    public GameObject m_Bomb, m_Explosion, m_BombDamage;
    public ParticleSystem m_ParticleSystem;
    public AudioSource m_AudioSource;

    //private Vector3 m_FixedPosition;

    private SystemManager m_SystemManager = null;

    private const int TARGET_TIMER = 400;
    private const int REMOVE_TIMER = 2600; // TARGET_TIMER 이후
    private bool m_Enable = true;

    void Start()
    {
        m_SystemManager = SystemManager.instance_sm;

        //m_FixedPosition = new Vector3(0f, -Size.GAME_HEIGHT/2, Depth.PLAYER_MISSILE);
        //transform.position = m_FixedPosition;
    }

    public void UseBomb()
    {
        m_Enable = false;
        m_Bomb.SetActive(true);

        m_Bomb.transform.position = m_PlayerTransform.position;

        float target_x = Mathf.Clamp(m_PlayerTransform.position.x, -3f, 3f);
        float target_y = - Size.GAME_HEIGHT/2 - 1f;
        Vector3 target_position = new Vector3(target_x, target_y, Depth.PLAYER_MISSILE);
        Vector3 relativePos = target_position - m_Bomb.transform.position;

        m_Bomb.transform.rotation = Quaternion.LookRotation(relativePos);
        
        StartCoroutine(BombExplosion(m_Bomb.transform.position, target_position));
    }

    /*
    void Update()
    {
        transform.position = m_FixedPosition;
        transform.rotation = Quaternion.identity;
    }*/

    private IEnumerator BombExplosion(Vector3 init_position, Vector3 target_position) {
        int frame = TARGET_TIMER * Application.targetFrameRate / 1000;
        m_AudioSource.Play();

        for (int i = 0; i < frame; ++i) {
            float t_pos = AC_Ease.ac_ease[EaseType.OutQuad].Evaluate((float) (i+1) / frame);
            
            m_Bomb.transform.position = Vector3.Lerp(init_position, target_position, t_pos);
            yield return new WaitForMillisecondFrames(0);
        }
        m_Explosion.transform.position = target_position;
        
        m_SystemManager.EraseBullets(2000);
        m_Bomb.SetActive(false);
        m_Explosion.SetActive(true);
        m_BombDamage.SetActive(true);

        yield return new WaitForMillisecondFrames(REMOVE_TIMER);
        m_Bomb.SetActive(false);
        m_Explosion.SetActive(false);
        m_BombDamage.SetActive(false);
        m_Enable = true;
        yield break;
    }

    public bool GetEnableState() {
        return m_Enable;
    }
}