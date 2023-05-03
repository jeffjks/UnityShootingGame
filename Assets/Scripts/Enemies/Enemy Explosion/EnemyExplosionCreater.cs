using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyExplosionCreater : MonoBehaviour
{
    [SerializeField] protected EnemyDeath m_EnemyDeath;
    [SerializeField] protected ExplosionAudioData m_explosionAudioData;

    private SystemManager m_SystemManager = null;
    private PoolingManager m_PoolingManager = null;
    private string[] m_PoolingString;
    private AudioClip[] m_ExplosionAudio;

    protected abstract IEnumerator DyingExplosion();

    private void Awake()
    {
        m_SystemManager = SystemManager.instance_sm;
        m_PoolingManager = PoolingManager.instance_op;
        
        InitExplosionEffectString();
        InitExplosionAudioClip();
        
        m_EnemyDeath.Action_OnDying += StartDyingExplosion;
    }

    private void InitExplosionEffectString()
    {
        m_PoolingString = new string[] {
            "ExplosionGround_1",
            "ExplosionGround_2",
            "ExplosionGround_3",
            "ExplosionGeneral_1",
            "ExplosionGeneral_2",
            "ExplosionGeneral_3",
            "ExplosionSimple_1",
            "ExplosionSimple_2",
            "ExplosionStarShape",
            "ExplosionMineShape"
        };
    }

    private void InitExplosionAudioClip() {
        m_ExplosionAudio = new AudioClip[] {
            m_explosionAudioData.audio_explosionAirMedium_1,
            m_explosionAudioData.audio_explosionAirMedium_2,
            m_explosionAudioData.audio_explosionAirSmall,
            m_explosionAudioData.audio_explosionMediumGround,
            m_explosionAudioData.audio_explosionGroundSmall,
            m_explosionAudioData.audio_explosionHuge_1,
            m_explosionAudioData.audio_explosionHuge_2,
            m_explosionAudioData.audio_explosionLarge
        };
    }

    private void StartDyingExplosion() {
        StartCoroutine(DyingExplosion());
    }

    private string GetPoolingString(ExplosionEffect explosionEffect) {
        return m_PoolingString[(int) explosionEffect];
    }

    protected void CreateExplosionEffect(ExplosionEffect expl_effect, ExplosionAudio expl_audio, Vector3? transformPointPos = null, MoveVector? moveVector = null) {
        try {
            if (expl_effect != ExplosionEffect.None) {
                GameObject obj = m_PoolingManager.PopFromPool(GetPoolingString(expl_effect), PoolingParent.EXPLOSION);
                ExplosionEffecter explosion_effecter = obj.GetComponent<ExplosionEffecter>();

                explosion_effecter.m_MoveVector = moveVector ?? new MoveVector(0f, 0f);
                
                Vector3 explosion_pos = transform.TransformPoint(transformPointPos ?? Vector3.zero);
                
                if ((1 << gameObject.layer & Layer.AIR) != 0)
                    explosion_pos = new Vector3(explosion_pos.x, explosion_pos.y, Depth.EXPLOSION);
                
                obj.transform.position = explosion_pos;
                obj.SetActive(true);
            }
            
            
            if (expl_audio != ExplosionAudio.None) {
                m_SystemManager.m_SoundManager.PlayAudio(m_ExplosionAudio[(int) expl_audio]);
            }
        }
        catch {
            Debug.LogAssertion("Explosion OutOfRangeException has occured.");
            return;
        }
    }
}
